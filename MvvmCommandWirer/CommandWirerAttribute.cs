using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (match.Success && match.Groups.Count == 2)
                Key = match.Groups[1].Value;
            else
                Key = methodName;
        }
    }
}
