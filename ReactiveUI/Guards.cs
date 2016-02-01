using System;
using System.Reflection;

namespace ReactiveUI
{
    internal static class Guard
    {
        public static void EnsureNotNull<T>(this T @this, string argumentName)
            where T : class
        {
            if (@this == null) {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void EnsureNotNull<T>(this T? @this, string argumentName)
            where T : struct
        {
            if (@this == null) {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void EnsureGenericArgumentNotNull<T>(this T @this, string argumentName)
        {
            var typeInfo = typeof(T).GetTypeInfo();
            var isNullable = !typeInfo.IsValueType || (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>));

            if (!isNullable) {
                return;
            }
            
            if ((object)@this == null) {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void Ensure(bool condition, string message, string argumentName = null)
        {
            if (!condition) {
                throw new ArgumentException(message, argumentName);
            }
        }
    }
}