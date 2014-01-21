using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Com.PhilChuang.Utils.MvvmCommandWirer
{
    // references
    // http://xamlblog.tumblr.com/post/46187145555/fixing-mvvm-part-1-commands
    // http://stackoverflow.com/questions/658316/runtime-creation-of-generic-funct

    /// <summary>
    /// Uses CommandWirerAttributes to determine how to wire up an ICommand
    /// </summary>
    public class CommandWirer
    {
        public String Key { get; set; }

        public Object InvokeOn { get; set; }

        public PropertyInfo CommandProperty { get; set; }

        public Type CommandType { get; set; }

        public Type ParameterType { get; set; }

        public MethodInfo InstantiationMethod { get; set; }

        public MethodInfo InitializationMethod { get; set; }

        public MethodInfo CanExecuteMethod { get; set; }

        public MethodInfo ExecuteMethod { get; set; }

        /* TODO TEST
         * WHEN WIRING
         * - CommandProperty is required
         * - InvokeOn is required
         * - CommandType OR InstantiationMethod is required
         * - ExecuteMethod is required
         */

        /// <summary>
        /// Wires up a single Command
        /// </summary>
        public void Wire ()
        {
            if (CommandProperty == null)
                throw new InvalidOperationException ("{0} requires an CommandProperty value for key: \"{1}\"".FormatWith (GetType ().Name, Key));

            if (InvokeOn == null)
                throw new InvalidOperationException ("{0} requires an InvokeOn value for key: \"{1}\"".FormatWith (GetType ().Name, Key));

            if (CommandType == null && InstantiationMethod == null)
                throw new InvalidOperationException ("Either CommandProperty.CommandType or CommandInstantiationMethod must be defined for key: \"{0}\"".FormatWith (Key));

            if (ExecuteMethod == null)
                throw new InvalidOperationException ("CommandExecuteMethod must be defined for key: \"{0}\"".FormatWith (Key));

            // create delegates for the Command
            var canExecuteDelegate = CommandCanExecuteMethodAttribute.CreateCanExecuteDelegate (CanExecuteMethod, ParameterType, InvokeOn);
            var executeDelegate = CommandExecuteMethodAttribute.CreateExecuteDelegate (ExecuteMethod, ParameterType, InvokeOn);

            // create the Command and set it on the Property
            Object command = null;
            if (InstantiationMethod != null)
            {
                command = InstantiationMethod.Invoke (InvokeOn, new object[] { executeDelegate, canExecuteDelegate });
            }
            else if (CommandType != null)
            {
                if (CommandType.IsAbstract || CommandType.IsInterface)
                    throw new InvalidOperationException ("CommandProperty.CommandType must be a concrete class for key: \"{0}\"".FormatWith (Key));

                command = Activator.CreateInstance (CommandType, executeDelegate, canExecuteDelegate);
            }
            else
            {
                // this line shouldn't ever be hit
                throw new InvalidOperationException (String.Format ("Did not have an Command instantiation method for key: \"{0}\"", Key));
            }
            CommandProperty.SetValue (InvokeOn, command, null);

            // initialize the command
            if (InitializationMethod != null)
            {
                if (InitializationMethod.GetParameters ().Count () == 1)
                {
                    InitializationMethod.Invoke (InvokeOn, new[] { command });
                }
                else
                {
                    InitializationMethod.Invoke (InvokeOn, null);
                }
            }
        }

        /// <summary>
        /// Wires up an entire object according to its usage of CommandWirerAttributes
        /// </summary>
        /// <param name="toWire"></param>
        /// <returns></returns>
        public static IList<CommandWirer> WireAll (Object toWire)
        {
            toWire.ThrowIfNull ("toWire");

            var toWireType = toWire.GetType ();

            var helperMap = new Dictionary<String, CommandWirer> ();

            foreach (var prop in 
                toWireType.GetProperties (BindingFlags.Public | BindingFlags.Instance)
                .Union (toWireType.GetProperties (BindingFlags.NonPublic | BindingFlags.Instance))
                .Union (toWireType.GetProperties (BindingFlags.Public | BindingFlags.Static))
                .Union (toWireType.GetProperties (BindingFlags.NonPublic | BindingFlags.Static)))
            {
                foreach (var attr in prop.GetCustomAttributes (typeof (CommandWirerAttribute), true).Cast<CommandWirerAttribute> ())
                {
                    attr.SetKeyFromMethodName (prop.Name);

                    CommandWirer helper;
                    if (!helperMap.TryGetValue (attr.Key, out helper))
                        helperMap[attr.Key] = helper = new CommandWirer { Key = attr.Key, InvokeOn = toWire };

                    attr.Configure (helper, prop);
                }
            }

            foreach (var method in 
                toWireType.GetMethods (BindingFlags.Public | BindingFlags.Instance)
                .Union (toWireType.GetMethods (BindingFlags.NonPublic | BindingFlags.Instance))
                .Union (toWireType.GetMethods (BindingFlags.Public | BindingFlags.Static))
                .Union (toWireType.GetMethods (BindingFlags.NonPublic | BindingFlags.Static)))
            {
                foreach (var attr in method.GetCustomAttributes (typeof (CommandWirerAttribute), true).Cast<CommandWirerAttribute> ())
                {
                    attr.SetKeyFromMethodName (method.Name);

                    CommandWirer helper;
                    if (!helperMap.TryGetValue (attr.Key, out helper))
                        helperMap[attr.Key] = helper = new CommandWirer { Key = attr.Key };

                    attr.Configure (helper, method);
                }
            }

            foreach (var helper in helperMap.Values)
                helper.Wire ();

            return helperMap.Values.ToList ();
        }
    }
}