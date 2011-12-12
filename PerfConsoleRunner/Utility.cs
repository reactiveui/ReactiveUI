using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PerfConsoleRunner
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utility
    {
        public static bool ImplementsInterface(Type target, Type interfaceToCheck) {
            if (target == null)
                throw new ArgumentNullException("target");

            if (target.GetInterfaces().Contains(interfaceToCheck)) {
                return true;
            }

            if (target.BaseType != typeof(object)) {
                return ImplementsInterface(target.BaseType, interfaceToCheck);
            }

            return false;
        }

        static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
    }

    public static class ModuleHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static Type[] SafeGetTypes(this Module This)
        {
            try {
                return This.GetTypes();
            } catch(ReflectionTypeLoadException _) {
                return new Type[0];
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
