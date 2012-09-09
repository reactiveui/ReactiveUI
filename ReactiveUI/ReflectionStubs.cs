using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    internal enum BindingFlags {
        Public = 1, 
        NonPublic = 1 << 1, 
        Instance = 1 << 2, 
        Static = 1 << 3, 
        FlattenHierarchy = 1 << 4
    }

    internal static class ReflectionStubs
    {
        public static FieldInfo GetField(this Type This, string name, BindingFlags flags = default(BindingFlags))
        {
            var ti = This.GetTypeInfo();
            var ret = ti.GetDeclaredField(name);
            if (ret != null || !flags.HasFlag(BindingFlags.FlattenHierarchy) || ti.BaseType == null) return ret;

            return ti.BaseType.GetField(name, flags);
        }

        public static PropertyInfo GetProperty(this Type This, string name, BindingFlags flags = default(BindingFlags))
        {
            var ti = This.GetTypeInfo();
            var ret = ti.GetDeclaredProperty(name);
            if (ret != null || !flags.HasFlag(BindingFlags.FlattenHierarchy) || ti.BaseType == null) return ret;

            return ti.BaseType.GetProperty(name, flags);
        }

        public static EventInfo GetEvent(this Type This, string name, BindingFlags flags = default(BindingFlags))
        {
            var ti = This.GetTypeInfo();
            var ret = ti.GetDeclaredEvent(name);
            if (ret != null || !flags.HasFlag(BindingFlags.FlattenHierarchy) || ti.BaseType == null) return ret;

            return ti.BaseType.GetEvent(name, flags);
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type This, BindingFlags flags = default(BindingFlags))
        {
            return This.GetTypeInfo().DeclaredProperties;
        }

        public static MethodInfo GetMethod(this Type This, string methodName, Type[] paramTypes, BindingFlags flags = default(BindingFlags))
        {
            var ti = This.GetTypeInfo();
            var ret = ti.GetDeclaredMethods(methodName)
                .FirstOrDefault(x => {
                    return paramTypes.Zip(x.GetParameters().Select(y => y.ParameterType), (l, r) => l == r).All(y => y != false);
                });

            if (ret != null || !flags.HasFlag(BindingFlags.FlattenHierarchy) || ti.BaseType == null) return ret;
            return ti.BaseType.GetMethod(methodName, paramTypes, flags);
        }

        public static IEnumerable<MethodInfo> GetMethods(this Type This)
        {
            return This.GetTypeInfo().DeclaredMethods;
        }

        public static IEnumerable<object> GetCustomAttributes(this Type This, Type attributeType, bool inherit)
        {
            return This.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
        }
        
        public static bool IsAssignableFrom(this Type This, Type anotherType)
        {
            return This.GetTypeInfo().IsAssignableFrom(anotherType.GetTypeInfo());
        }
    }
}
