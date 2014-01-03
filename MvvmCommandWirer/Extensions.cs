using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Com.PhilChuang.Utils
{
    internal static class Extensions
    {
        public static void ThrowIfNull (this Object self, String objName)
        {
            if (objName == null) throw new ArgumentNullException ("objName");
            if (self == null) throw new ArgumentNullException (objName);
        }
        
        public static void ThrowIfNullOrBlank (this String self, String objName)
        {
            objName.ThrowIfNull ("objName");
            if (self.IsNullOrBlank ()) throw new ArgumentNullException (objName);
        }

        public static bool IsNullOrBlank (this String self)
        {
            return String.IsNullOrEmpty (self) || String.IsNullOrEmpty (self.Trim());
        }

        public static String FormatWith (this String self, params Object[] args)
        {
            self.ThrowIfNull ("self");
            return String.Format (self, args);
        }

        public static MethodInfo GetMethodInfo (this Expression<Action> self)
        {
            if (self == null) throw new ArgumentNullException ("self");

            if (self.Body.NodeType == ExpressionType.Call)
            {
                var methodExpr = (MethodCallExpression) self.Body;
                return methodExpr.Method;
            }

            throw new Exception (String.Format ("Expected MethodCallExpression, got {0}", self.Body.NodeType));
        }

        public static String GetMethodName (this Expression<Action> self)
        {
            return GetMethodInfo (self).Name;
        }

        public static PropertyInfo GetPropertyInfo<T> (this Expression<Func<T>> self)
        {
            if (self == null) throw new ArgumentNullException ("self");

            if (self.Body.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpr = (MemberExpression) self.Body;
                return (PropertyInfo) memberExpr.Member;
            }

            if (self.Body.NodeType == ExpressionType.Convert
                && self.Body is UnaryExpression
                && ((UnaryExpression) self.Body).Operand.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpr = (MemberExpression) ((UnaryExpression) self.Body).Operand;
                return (PropertyInfo) memberExpr.Member;
            }

            throw new Exception (String.Format ("Expected MemberAccess expression, got {0}", self.Body.NodeType));
        }

        public static String GetPropertyName<T> (this Expression<Func<T>> self)
        {
            return GetPropertyInfo (self).Name;
        }
    }
}
