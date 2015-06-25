using System;
using System.Reflection;

namespace ReactiveUI
{
    /// <summary>
    /// This file provides mixins for aiding in using the .NET 4.5 API in .NET 4.0.
    /// It mostly involves delegating to the right methods
    /// </summary>
#if NET_40
    internal static class TypeExtensions
    {
        public static TypeInfo GetTypeInfo(this Type type)
        {
            return new TypeInfo(type);
        }

        public static MethodInfo[] GetRuntimeMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static PropertyInfo GetRuntimeProperty(this Type type, string property)
        {
            return type.GetProperty(property);
        }
        public static EventInfo GetRuntimeEvent(this Type type, string eventName)
        {
            return type.GetEvent(eventName);
        }
        public static FieldInfo GetRuntimeField(this Type type, string field)
        {
            return type.GetField(field);
        }
    }

    internal class TypeInfo
    {
        private static readonly BindingFlags declaredFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        private readonly Type underlyingType;

        public TypeInfo(Type type)
        {
            underlyingType = type;
        }

        public PropertyInfo[] DeclaredProperties
        {
            get { return underlyingType.GetProperties(declaredFlags); }
        }
        public MethodInfo[] DeclaredMethods
        {
            get { return underlyingType.GetMethods(declaredFlags); }
        }

        public bool IsAssignableFrom(TypeInfo typeInfo)
        {
            if (typeInfo == null)
                return false;

            if (this == typeInfo)
                return true;

            return underlyingType.IsAssignableFrom(typeInfo.underlyingType);
        }

        public bool IsSubclassOf(TypeInfo typeInfo)
        {
            return underlyingType.IsSubclassOf(typeInfo.underlyingType);
        }

        public bool IsSubclassOf(Type type)
        {
            return underlyingType.IsSubclassOf(type);
        }

        public object[] GetCustomAttributes(Type type, bool inherit)
        {
            return underlyingType.GetCustomAttributes(type, inherit);
        }

        public FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            return underlyingType.GetFields(bindingFlags);
        }

        public MethodInfo GetDeclaredMethod(string bind)
        {
            return underlyingType.GetMethod(bind, declaredFlags);
        }
    }
#endif
}
