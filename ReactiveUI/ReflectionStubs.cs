using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    internal enum BindingFlags {
        Public, NonPublic, Instance, Static, FlattenHierarchy
    }

    internal static class ReflectionStubs
    {
        public static FieldInfo GetField(this Type This, string name, BindingFlags dontcare = default(BindingFlags))
        {
            return This.GetTypeInfo().GetDeclaredField(name);
        }

        public static PropertyInfo GetProperty(this Type This, string name, BindingFlags dontcare = default(BindingFlags))
        {
            return This.GetTypeInfo().GetDeclaredProperty(name);
        }

        public static EventInfo GetEvent(this Type This, string name, BindingFlags dontcare)
        {
            return This.GetTypeInfo().GetDeclaredEvent(name);
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type This, BindingFlags dontcare)
        {
            return This.GetTypeInfo().DeclaredProperties;
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
