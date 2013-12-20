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

        public MethodInfo OnInitialize { get; set; }

        public MethodInfo CanExecute { get; set; }

        public MethodInfo Execute { get; set; }

        // TODO TEST with async

        public void Wire()
        {
            if (CommandProperty == null)
                throw new InvalidOperationException("{0} requires an CommandProperty value for key: \"{1}\"".FormatWith(GetType().Name, Key));

            if (InvokeOn == null)
                throw new InvalidOperationException("{0} requires an InvokeOn value for key: \"{1}\"".FormatWith(GetType().Name, Key));

            if (CommandType == null && OnInitialize == null)
                throw new InvalidOperationException("Either CommandProperty.CommandType or CommandOnInitialize must be defined for key: \"{0}\"".FormatWith(Key));

            if (Execute == null)
                throw new InvalidOperationException("CommandExecuteMethod must be defined for key: \"{0}\"".FormatWith(Key));

            Delegate canExecuteDelegate = null; // needs to be Func<bool> or Func<{ParameterType}, bool>
            if (CanExecute != null)
            {
                if (CanExecute.ReturnType != typeof(bool))
                    throw new InvalidOperationException("CommandCanExecuteMethod must have a bool return type for key: \"{0}\"".FormatWith(Key));

                if (ParameterType != null)
                {
                    if (CanExecute.GetParameters().Count() != 1
                        || CanExecute.GetParameters()[0].ParameterType != ParameterType)
                        throw new InvalidOperationException("CommandProperty.ParameterType is defined but does not match parameters for CommandCanExecuteMethod for key: \"{0}\"".FormatWith(Key));

                    return;
                    // TODO implement
                    //canExecuteDelegate = CanExecute.CreateDelegate(typeof(Func<,>).MakeGenericType(ParameterType, typeof(bool)));
                }
                else
                {
                    canExecuteDelegate = Delegate.CreateDelegate (typeof (Func<bool>), InvokeOn, CanExecute);
                }
            }
            else
            {
                // alternative way: dynamically create CanExecute MethodInfo if null then run above if block
                if (ParameterType != null)
                {
                    return;
                    // TODO implement
                    //var call = Expression.Call (GetReturnTrueMethodInfo ());
                    //var lambda = Expression.Lambda (call);
                    //canExecuteDelegate = lambda.Compile ();

                    // is this going to work? the underlying method is Func<bool> but trying to create a delegate that is Func<{ParameterType}, bool>
                    //canExecuteDelegate = ((Func<bool>)(() => true)).GetMethodInfo()
                    //                                                .CreateDelegate(typeof(Func<,>).MakeGenericType(ParameterType, typeof(bool)));
                }
                else
                {
                    canExecuteDelegate = (Func<bool>)(() => true);
                }
            }

            Delegate executeDelegate = null; // needs to be Action or Action<{ParameterType}>
            if (ParameterType != null)
            {
                return;
                // TODO create Action<ParameterType> dynamically
                //executeDelegate = Execute.CreateDelegate(typeof(Action<>).MakeGenericType(ParameterType));
            }
            else
            {
                executeDelegate = Delegate.CreateDelegate(typeof(Action), InvokeOn, Execute);
            }

            if (CommandType != null)
            {
                var command = Activator.CreateInstance(CommandType, executeDelegate, canExecuteDelegate);
                CommandProperty.SetValue(InvokeOn, command, null);
            }

            if (OnInitialize != null)
            {
                OnInitialize.Invoke(InvokeOn, null);
            }
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
                        helper.CanExecute = prop.GetGetMethod ();
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

                    var canExecuteAttr = attr as CommandCanExecuteMethodAttribute;
                    if (canExecuteAttr != null)
                    {
                        helper.CanExecute = method;
                        continue;
                    }

                    var executeAttr = attr as CommandExecuteMethodAttribute;
                    if (executeAttr != null)
                    {
                        helper.Execute = method;
                        continue;
                    }

                    var onInitAttr = attr as CommandOnInitializeMethodAttribute;
                    if (onInitAttr != null)
                    {
                        helper.OnInitialize = method;
                        continue;
                    }
                }
            }

            foreach (var helper in helperMap.Values)
                helper.Wire();
        }
    }
}
