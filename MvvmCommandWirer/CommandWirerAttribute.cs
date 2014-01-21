using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Com.PhilChuang.Utils.MvvmCommandWirer
{
    public abstract class CommandWirerAttribute : Attribute
    {
        public String Key { get; protected set; }

        protected CommandWirerAttribute()
        {
        }

        protected CommandWirerAttribute(String key)
        {
            Key = key;
        }

        /// <summary>
        /// If Key is null, attempt to determine it from the given method name
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public abstract void SetKeyFromMethodName(String methodName);

        public abstract void Configure (CommandWirer wirer, PropertyInfo prop);
        public abstract void Configure (CommandWirer wirer, MethodInfo method);
    }

    public class CommandPropertyAttribute : CommandWirerAttribute
    {
        public Type CommandType { get; private set; }
        public Type ParameterType { get; private set; }

        public CommandPropertyAttribute(String key = null, Type commandType = null, Type paramType = null)
            : base(key)
        {
            CommandType = commandType;
            ParameterType = paramType;
        }

        public override void SetKeyFromMethodName(String methodName)
        {
            if (!Key.IsNullOrBlank()) return;
            methodName.ThrowIfNullOrBlank("propertyName");

            var k = methodName;
            var match = Regex.Match(k, @"^(.+)Command$");
            if (match.Success && match.Groups.Count == 2)
                k = match.Groups[1].Value;

            Key = k;
        }

        public override void Configure (CommandWirer wirer, PropertyInfo prop)
        {
            wirer.CommandProperty = prop; // mandatory
            wirer.CommandType = CommandType ?? prop.PropertyType; // optional if CommandInstantiationMethod is used
            wirer.ParameterType = ParameterType; // optional if not parameterized
        }

        public override void Configure (CommandWirer wirer, MethodInfo method)
        {
            throw new InvalidOperationException ("CommandPropertyAttribute must be applied to a property, not method \"{0}\".".FormatWith (method.Name));
        }
    }

    public class CommandInstantiationMethodAttribute : CommandWirerAttribute
    {
        public CommandInstantiationMethodAttribute () { }

        public CommandInstantiationMethodAttribute (String key) : base (key) { }

        public override void SetKeyFromMethodName (String methodName)
        {
            if (!Key.IsNullOrBlank ()) return;
            methodName.ThrowIfNullOrBlank ("methodName");

            var initPrefixMatch = Regex.Match (methodName, @"^(?:Instantiate|Create)([A-Z].*?)(?:Command)?$");
            var initSuffixMatch = Regex.Match (methodName, @"^([A-Z].*?)(?:Command)?(?:Instantiate|Instantiation|Create)$");

            if (initPrefixMatch.Success && initPrefixMatch.Groups.Count == 2)
                Key = initPrefixMatch.Groups[1].Value;
            else if (initSuffixMatch.Success && initSuffixMatch.Groups.Count == 2)
                Key = initSuffixMatch.Groups[1].Value;
            else
                throw new ArgumentException ("Expecting method name like \"Instantiate[Key]Command\"");
        }

        public override void Configure (CommandWirer wirer, PropertyInfo prop)
        {
            throw new InvalidOperationException ("CommandInstantiationMethodAttribute must be applied to a method, not property \"{0}\".".FormatWith (prop.Name));
        }

        public override void Configure (CommandWirer wirer, MethodInfo method)
        {
            if (wirer.ParameterType == null)
            {
                if (method.GetParameters ().Count () != 2
                    || method.GetParameters ().ElementAt (0).ParameterType != typeof (Action)
                    || method.GetParameters ().ElementAt (1).ParameterType != typeof (Func<bool>))
                    throw new InvalidOperationException (
                        "CommandInstantiationMethodAttribute target \"{0}\" must have 2 parameters: (Action commandExecute, Func<bool> commandCanExecute)."
                            .FormatWith (method.Name));
            }
            else
            {
                if (method.GetParameters ().Count () != 2
                    || method.GetParameters ().ElementAt (0).ParameterType != typeof (Action<>).MakeGenericType (wirer.ParameterType)
                    || method.GetParameters ().ElementAt (1).ParameterType != typeof (Func<,>).MakeGenericType (wirer.ParameterType, typeof (bool)))
                    throw new InvalidOperationException (
                        "CommandInstantiationMethodAttribute target \"{0}\" must have 2 parameters: (Action<{1}> commandExecute, Func<{1}, bool> commandCanExecute)."
                            .FormatWith (method.Name, wirer.ParameterType.Name));
            }

            if (method.ReturnType == typeof (void))
                throw new InvalidOperationException ("CommandInstantiationMethodAttribute target \"{0}\" must have a return type".FormatWith (method.Name));

            // TODO CONSIDER check that return type implements ICommand?

            wirer.InstantiationMethod = method;
        }
    }

    public class CommandInitializationMethodAttribute : CommandWirerAttribute
    {
        public CommandInitializationMethodAttribute() { }

        public CommandInitializationMethodAttribute(String key) : base(key) { }

        public override void SetKeyFromMethodName(String methodName)
        {
            if (!Key.IsNullOrBlank()) return;
            methodName.ThrowIfNullOrBlank("methodName");

            var initPrefixMatch = Regex.Match(methodName, @"^(?:Initialize|Init)([A-Z].*?)(?:Command)?$");
            var initSuffixMatch = Regex.Match(methodName, @"^([A-Z].*?)(?:Command)?(?:Initialize|Init)$");

            if (initPrefixMatch.Success && initPrefixMatch.Groups.Count == 2)
                Key = initPrefixMatch.Groups[1].Value;
            else if (initSuffixMatch.Success && initSuffixMatch.Groups.Count == 2)
                Key = initSuffixMatch.Groups[1].Value;
            else
                throw new ArgumentException("Expecting method name like \"Initialize[Key]Command\"");
        }

        public override void Configure (CommandWirer wirer, PropertyInfo prop)
        {
            throw new InvalidOperationException ("CommandInitializationMethodAttribute must be applied to a method, not property \"{0}\".".FormatWith (prop.Name));
        }

        public override void Configure (CommandWirer wirer, MethodInfo method)
        {
            if (method.GetParameters ().Count () > 1)
            {
                throw new InvalidOperationException ("CommandInitializationMethodAttribute target \"{0}\" can have either no parameters or a single parameter for the ICommand instance."
                                                         .FormatWith (method.Name));
            }

            // TODO CONSIDER check that parameter type implements ICommand?

            wirer.InitializationMethod = method;
        }
    }

    public class CommandCanExecuteMethodAttribute : CommandWirerAttribute
    {
        public CommandCanExecuteMethodAttribute() { }

        public CommandCanExecuteMethodAttribute(String key) : base(key) { }

        public override void SetKeyFromMethodName(String methodName)
        {
            if (!Key.IsNullOrBlank()) return;
            methodName.ThrowIfNullOrBlank("methodName");

            var match = Regex.Match(methodName, @"^(?:CanExecute|Can)([A-Z].*)$");
            if (match.Success && match.Groups.Count == 2)
                Key = match.Groups[1].Value;
            else
                throw new ArgumentException("Expecting method name like \"Can[Key]\"");
        }

        public override void Configure (CommandWirer wirer, PropertyInfo prop)
        {
            if (prop.PropertyType != typeof (bool))
                throw new InvalidOperationException ("CommandCanExecuteMethodAttribute target \"{0}\" must have a bool return type.".FormatWith (prop.Name));

            wirer.CanExecuteMethod = prop.GetGetMethod (false) ?? prop.GetGetMethod (true);
        }

        public override void Configure (CommandWirer wirer, MethodInfo method)
        {
            if (method.ReturnType != typeof (bool))
                throw new InvalidOperationException ("CommandCanExecuteMethodAttribute target \"{0}\" must have a bool return type.".FormatWith (method.Name));

            var paramCount = method.GetParameters ().Count ();
            if (paramCount == 1)
            {
                if (wirer.ParameterType == null)
                    throw new InvalidOperationException ("CommandCanExecuteMethodAttribute target \"{0}\" must be parameterless because CommandProperty.ParameterType was not defined.".FormatWith (method.Name));
                // TODO CONSIDER allowing matching types (i.e. int -> double or impl -> interface)
                if (method.GetParameters ().ElementAt (0).ParameterType != wirer.ParameterType)
                    throw new InvalidOperationException ("CommandCanExecuteMethodAttribute target \"{0}\" parameter type \"{1}\" does not match CommandProperty.ParameterType \"{2}\"."
                                                             .FormatWith (method.Name,
                                                                          typeof (int?).Name,
                                                                          typeof (String).Name));
            }
            else if (paramCount > 1)
            {
                if (method.GetParameters ().Count () > 1)
                    throw new InvalidOperationException (
                        "CommandCanExecuteMethodAttribute target \"{0}\" can have either no parameters or a single {1} parameter".FormatWith (method.Name, wirer.ParameterType));
            }

            wirer.CanExecuteMethod = method;
        }

        public static Delegate CreateCanExecuteDelegate (MethodInfo canExecuteMethod, Type commandParameterType, Object invokeOn)
        {
            if (canExecuteMethod != null)
            {
                if (canExecuteMethod.IsStatic)
                    invokeOn = null;

                if (commandParameterType == null)
                    return Delegate.CreateDelegate (typeof (Func<bool>), invokeOn, canExecuteMethod);

                if (!canExecuteMethod.GetParameters ().Any ())
                {
                    var del = Delegate.CreateDelegate (typeof (Func<bool>), invokeOn, canExecuteMethod);
                    return CreateParameterizedFuncBoolWrap (commandParameterType, (Func<bool>) del);
                }

                return Delegate.CreateDelegate (typeof (Func<,>).MakeGenericType (commandParameterType, typeof (bool)), invokeOn, canExecuteMethod);
            }

            if (commandParameterType != null)
            {
                return CreateParameterizedFuncBoolWrap (commandParameterType, () => true);
            }

            return (Func<bool>) (() => true);
        }

        private static Delegate CreateParameterizedFuncBoolWrap (Type parameterType, Func<bool> func)
        {
            var wrapMethodInfo = WrapFuncBoolMethodInfo.MakeGenericMethod (parameterType);
            return (Delegate) wrapMethodInfo.Invoke (null, new object[] { func });
        }

        private static readonly MethodInfo WrapFuncBoolMethodInfo = Extensions.GetMethodInfo (() => WrapFuncBool<Object> (null)).GetGenericMethodDefinition ();

        private static Func<T, bool> WrapFuncBool<T> (Func<bool> func)
        {
            return _ => func ();
        }

    }

    public class CommandExecuteMethodAttribute : CommandWirerAttribute
    {
        public CommandExecuteMethodAttribute() { }

        public CommandExecuteMethodAttribute(String key) : base(key) { }

        public override void SetKeyFromMethodName(String methodName)
        {
            if (!Key.IsNullOrBlank()) return;
            methodName.ThrowIfNullOrBlank("methodName");

            var match = Regex.Match(methodName, @"^(?:Execute|Do)([A-Z].*)$");
            String k;
            if (match.Success && match.Groups.Count == 2)
                k = match.Groups[1].Value;
            else
                k = methodName;

            if (k.EndsWith ("Async", StringComparison.OrdinalIgnoreCase))
                k = k.Substring (0, k.Length - 5);

            Key = k;
        }

        public override void Configure (CommandWirer wirer, PropertyInfo prop)
        {
            throw new InvalidOperationException ("CommandExecuteMethodAttribute target \"{0}\" must be a method.".FormatWith (prop.Name));
        }

        public override void Configure (CommandWirer wirer, MethodInfo method)
        {
            // NOTE very similar to CommandCanExecuteMethodAttribute.Configure

            var paramCount = method.GetParameters ().Count ();
            if (paramCount == 1)
            {
                if (wirer.ParameterType == null)
                    throw new InvalidOperationException ("CommandExecuteMethodAttribute target \"{0}\" must be parameterless because CommandProperty.ParameterType was not defined.".FormatWith (method.Name));
                // TODO CONSIDER allowing matching types (i.e. int -> double or impl -> interface)
                if (method.GetParameters ().ElementAt (0).ParameterType != wirer.ParameterType)
                    throw new InvalidOperationException ("CommandExecuteMethodAttribute target \"{0}\" parameter type \"{1}\" does not match CommandProperty.ParameterType \"{2}\"."
                                                             .FormatWith (method.Name,
                                                                          typeof (int?).Name,
                                                                          typeof (String).Name));
            }
            else if (paramCount > 1)
            {
                if (method.GetParameters ().Count () > 1)
                    throw new InvalidOperationException (
                        "CommandExecuteMethodAttribute target \"{0}\" can have either no parameters or a single {1} parameter".FormatWith (method.Name, wirer.ParameterType));
            }

            wirer.ExecuteMethod = method;
        }

        public static Delegate CreateExecuteDelegate (MethodInfo executeMethod, Type commandParameterType, Object invokeOn)
        {
            if (executeMethod.IsStatic)
                invokeOn = null;

            if (commandParameterType != null)
                return Delegate.CreateDelegate (typeof (Action<>).MakeGenericType (commandParameterType), invokeOn, executeMethod);

            return Delegate.CreateDelegate (typeof (Action), invokeOn, executeMethod);
        }
    }
}
