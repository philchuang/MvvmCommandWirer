using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Com.PhilChuang.Utils
{
    internal static class Extensions
    {
        public static void ThrowIfNull(this Object self, String objName)
        {
            if (objName == null) throw new ArgumentNullException("objName");
            if (self == null) throw new ArgumentNullException(objName);
        }
        
        public static void ThrowIfNullOrBlank(this String self, String objName)
        {
            objName.ThrowIfNull("objName");
            if (self.IsNullOrBlank()) throw new ArgumentNullException(objName);
        }

        public static bool IsNullOrBlank(this String self)
        {
            return String.IsNullOrEmpty(self) || String.IsNullOrEmpty(self.Trim());
        }

        public static String FormatWith(this String self, params Object[] args)
        {
            self.ThrowIfNull("self");
            return String.Format(self, args);
        }

        public static String GetMethodName (this Expression<Action> self)
        {
            if (self == null) throw new ArgumentNullException ("self");

            if (self.Body.NodeType == ExpressionType.Call)
            {
                var methodExpr = (MethodCallExpression) self.Body;
                return methodExpr.Method.Name;
            }

            throw new Exception (String.Format ("Expected MethodCallExpression, got {0}", self.Body.NodeType));
        }
    }
}
