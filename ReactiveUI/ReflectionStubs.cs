using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    internal enum BindingFlags {
        Public, NonPublic, Instance, Static 
    }

    internal static class ReflectionStubs
    {
        public static FieldInfo GetField(this Type This, string name)
        {
            return This.GetTypeInfo().GetDeclaredField(name);
        }

        public static PropertyInfo GetProperty(this Type This, string name)
        {
            return This.GetTypeInfo().GetDeclaredProperty(name);
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type This, BindingFlags dontcare)
        {
            return This.GetTypeInfo().DeclaredProperties;
        }
    }
}
