using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Com.PhilChuang.Utils.MvvmCommandWirer
{
    // references
    // http://xamlblog.tumblr.com/post/46187145555/fixing-mvvm-part-1-commands
    // http://stackoverflow.com/questions/658316/runtime-creation-of-generic-funct

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

        // TODO TEST with async

        public void Wire()
        {
            if (CommandProperty == null)
                throw new InvalidOperationException("{0} requires an CommandProperty value for key: \"{1}\"".FormatWith(GetType().Name, Key));

            if (InvokeOn == null)
                throw new InvalidOperationException("{0} requires an InvokeOn value for key: \"{1}\"".FormatWith(GetType().Name, Key));

            if (CommandType == null && InstantiationMethod == null)
                throw new InvalidOperationException("Either CommandProperty.CommandType or CommandInstantiationMethod must be defined for key: \"{0}\"".FormatWith(Key));

            if (ExecuteMethod == null)
                throw new InvalidOperationException("CommandExecuteMethod must be defined for key: \"{0}\"".FormatWith(Key));

            Delegate canExecuteDelegate = null; // needs to be Func<bool> or Func<{ParameterType}, bool>
            if (CanExecuteMethod != null)
            {
                if (CanExecuteMethod.ReturnType != typeof(bool))
                    throw new InvalidOperationException("CommandCanExecuteMethod must have a bool return type for key: \"{0}\"".FormatWith(Key));

                if (ParameterType != null)
                {
                    if (CanExecuteMethod.GetParameters().Count() != 1
                        || CanExecuteMethod.GetParameters()[0].ParameterType != ParameterType)
                        throw new InvalidOperationException("CommandProperty.ParameterType is defined but does not match parameters for CommandCanExecuteMethod for key: \"{0}\"".FormatWith(Key));

                    canExecuteDelegate = Delegate.CreateDelegate (typeof (Func<,>).MakeGenericType (ParameterType, typeof (bool)), InvokeOn, CanExecuteMethod);
                }
                else
                {
                    canExecuteDelegate = Delegate.CreateDelegate (typeof (Func<bool>), InvokeOn, CanExecuteMethod);
                }
            }
            else
            {
                if (ParameterType != null)
                {
                    // TODO make wrapMethodInfo(w/o MakeGenericMethod) static field and get the method name via Expression reflection
                    var wrapMethodInfo = typeof (CommandWirer).GetMethod ("WrapFuncBool", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod (ParameterType);
                    canExecuteDelegate = (Delegate) wrapMethodInfo.Invoke (this, new object[] { (Func<bool>) (() => true) });
                }
                else
                {
                    canExecuteDelegate = (Func<bool>) (() => true);
                }
            }

            Delegate executeDelegate = null; // needs to be Action or Action<{ParameterType}>
            if (ParameterType != null)
            {
                executeDelegate = Delegate.CreateDelegate (typeof (Action<>).MakeGenericType (ParameterType), InvokeOn, ExecuteMethod);
            }
            else
            {
                executeDelegate = Delegate.CreateDelegate(typeof(Action), InvokeOn, ExecuteMethod);
            }

            Object command = null;
            if (InstantiationMethod != null)
            {
                command = InstantiationMethod.Invoke (InvokeOn, new object[] { executeDelegate, canExecuteDelegate });
            }
            else if (CommandType != null)
            {
                command = Activator.CreateInstance(CommandType, executeDelegate, canExecuteDelegate);
            }
            else
            {
                throw new InvalidOperationException (String.Format ("Did not have an Command instantiation method for key: \"{0}\"", Key));
            }
            CommandProperty.SetValue (InvokeOn, command, null);

            if (InitializationMethod != null)
            {
                if (InitializationMethod.GetParameters ().Count () == 1)
                {
                    var paramType = InitializationMethod.GetParameters ().First ().ParameterType;
                    if (!paramType.IsInstanceOfType (command))
                        throw new InvalidOperationException (String.Format ("Unable to pass {0} as {1} parameter to {2}", command.GetType ().Name, paramType.Name, InitializationMethod.Name));

                    InitializationMethod.Invoke (InvokeOn, new[] { command });
                }
                else
                {
                    InitializationMethod.Invoke (InvokeOn, null);
                }
            }
        }

        private static Func<T, bool> WrapFuncBool<T> (Func<bool> func)
        {
            return _ => func ();
        }

        public static void WireAll(Object toWire)
        {
            toWire.ThrowIfNull("toWire");

            var toWireType = toWire.GetType();

            var helperMap = new Dictionary<String, CommandWirer>();

            foreach (var prop in toWireType.GetProperties())
            {
                foreach (var attr in prop.GetCustomAttributes (typeof (CommandWirerAttribute), true).Cast<CommandWirerAttribute> ())
                {
                    attr.SetKeyFromMethodName (prop.Name);

                    CommandWirer helper;
                    if (!helperMap.TryGetValue (attr.Key, out helper))
                        helperMap[attr.Key] = helper = new CommandWirer {Key = attr.Key, InvokeOn = toWire};

                    var propertyAttr = attr as CommandPropertyAttribute;
                    if (propertyAttr != null)
                    {
                        helper.CommandProperty = prop;
                        helper.CommandType = propertyAttr.CommandType;
                        helper.ParameterType = propertyAttr.ParameterType;
                        continue;
                    }

                    var canExecuteMethodAttr = attr as CommandCanExecuteMethodAttribute;
                    if (canExecuteMethodAttr != null)
                    {
                        helper.CanExecuteMethod = prop.GetGetMethod ();
                        continue;
                    }
                }
            }

            foreach (var method in toWireType.GetMethods (BindingFlags.Public).Union (toWireType.GetMethods (BindingFlags.NonPublic | BindingFlags.Instance)))
            {
                foreach (var attr in method.GetCustomAttributes (typeof (CommandWirerAttribute), true).Cast<CommandWirerAttribute> ())
                {
                    attr.SetKeyFromMethodName (method.Name);

                    CommandWirer helper;
                    if (!helperMap.TryGetValue (attr.Key, out helper))
                        helperMap[attr.Key] = helper = new CommandWirer {Key = attr.Key};

                    var createAttr = attr as CommandInstantiationMethodAttribute;
                    if (createAttr != null)
                    {
                        // TODO validate that method has expected parameters & return type
                        helper.InstantiationMethod = method;
                        continue;
                    }

                    var initAttr = attr as CommandInitializationMethodAttribute;
                    if (initAttr != null)
                    {
                        // TODO validate that if method is not parameterless, that it only has 1 parameter that implements ICommand
                        helper.InitializationMethod = method;
                        continue;
                    }
                    var canExecuteAttr = attr as CommandCanExecuteMethodAttribute;
                    if (canExecuteAttr != null)
                    {
                        // TODO validate that method returns bool
                        // TODO validate that method has 0 or 1 parameters
                        helper.CanExecuteMethod = method;
                        continue;
                    }

                    var executeAttr = attr as CommandExecuteMethodAttribute;
                    if (executeAttr != null)
                    {
                        // TODO validate that method has 0 or 1 parameters
                        helper.ExecuteMethod = method;
                        continue;
                    }
                }
            }

            foreach (var helper in helperMap.Values)
                helper.Wire();
        }
    }
}
