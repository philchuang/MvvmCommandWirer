using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
