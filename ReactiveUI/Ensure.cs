namespace ReactiveUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    internal static class Ensure
    {
        public static void ArgumentNotNull<T>(T arg, string argName)
            where T : class
        {
            if (arg == null) {
                throw new ArgumentNullException(argName);
            }
        }

        public static void ArgumentNotNull<T>(T? arg, string argName)
            where T : struct
        {
            if (!arg.HasValue) {
                throw new ArgumentNullException(argName);
            }
        }

        public static void GenericArgumentNotNull<T>(T arg, string argName)
        {
            var typeInfo = typeof(T).GetTypeInfo();

            if (!typeInfo.IsValueType || (typeInfo.IsGenericType && (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>)))) {
                ArgumentNotNull((object)arg, argName);
            }
        }

        public static void ArgumentNotNull<T>(IEnumerable<T> arg, string argName, bool assertContentsNotNull)
        {
            // make sure the enumerable item itself isn't null
            ArgumentNotNull(arg, argName);

            if (assertContentsNotNull && !typeof(T).GetTypeInfo().IsValueType) {
                // make sure each item in the enumeration isn't null
                foreach (var item in arg) {
                    if (item == null) {
                        throw new ArgumentException("An item inside the enumeration was null.", argName);
                    }
                }
            }
        }

        public static void ArgumentNotNullOrEmpty(string arg, string argName)
        {
            if (string.IsNullOrEmpty(arg)) {
                throw new ArgumentException("Cannot be null or empty.", argName);
            }
        }

        public static void ArgumentNotNullOrEmpty(IEnumerable arg, string argName)
        {
            if (arg == null || !arg.GetEnumerator().MoveNext()) {
                throw new ArgumentException("Cannot be null or empty.", argName);
            }
        }

        public static void ArgumentNotNullOrEmpty(ICollection arg, string argName)
        {
            if (arg == null || arg.Count == 0) {
                throw new ArgumentException("Cannot be null or empty.", argName);
            }
        }

        public static void ArgumentNotNullOrWhiteSpace(string arg, string argName)
        {
            if (string.IsNullOrWhiteSpace(arg)) {
                throw new ArgumentException("Cannot be null or white-space.", argName);
            }
        }

        public static void ArgumentCondition(bool condition, string message, string argName = null)
        {
            if (!condition) {
                throw new ArgumentException(message, argName);
            }
        }

        public static void ArgumentIsValidEnum<TEnum>(TEnum enumValue, string argName)
            where TEnum : struct
        {
            if (typeof(TEnum).GetTypeInfo().IsDefined(typeof(FlagsAttribute), false)) {
                // flag enumeration - we can only get here if TEnum is a valid enumeration type, since the FlagsAttribute can
                // only be applied to enumerations
                bool throwEx;
                var longValue = Convert.ToInt64(enumValue, CultureInfo.InvariantCulture);

                if (longValue == 0) {
                    // only throw if zero isn't defined in the enum - we have to convert zero to the underlying type of the enum
                    throwEx = !Enum.IsDefined(typeof(TEnum), default(TEnum));
                } else {
                    foreach (TEnum value in GetEnumValues<TEnum>()) {
                        longValue &= ~Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    }

                    // throw if there is a value left over after removing all valid values
                    throwEx = longValue != 0;
                }

                if (throwEx) {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not valid for flags enumeration '{1}'.",
                            enumValue,
                            typeof(TEnum).FullName),
                        argName);
                }
            } else {
                // not a flag enumeration
                if (!Enum.IsDefined(typeof(TEnum), enumValue)) {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not defined for enumeration '{1}'.",
                            enumValue,
                            typeof(TEnum).FullName),
                        argName);
                }
            }
        }

        public static void ArgumentIsValidEnum<TEnum>(TEnum enumValue, string argName, params TEnum[] validValues)
            where TEnum : struct
        {
            ArgumentNotNull(validValues, "validValues");

            if (typeof(TEnum).GetTypeInfo().GetCustomAttribute<FlagsAttribute>(false) != null) {
                // flag enumeration
                bool throwEx;
                var longValue = Convert.ToInt64(enumValue, CultureInfo.InvariantCulture);

                if (longValue == 0) {
                    // only throw if zero isn't permitted by the valid values
                    throwEx = true;

                    foreach (TEnum value in validValues) {
                        if (Convert.ToInt64(value, CultureInfo.InvariantCulture) == 0) {
                            throwEx = false;
                            break;
                        }
                    }
                } else {
                    foreach (var value in validValues) {
                        longValue &= ~Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    }

                    // throw if there is a value left over after removing all valid values
                    throwEx = longValue != 0;
                }

                if (throwEx) {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not allowed for flags enumeration '{1}'.",
                            enumValue,
                            typeof(TEnum).FullName),
                        argName);
                }
            } else {
                // not a flag enumeration
                foreach (var value in validValues) {
                    if (enumValue.Equals(value)) {
                        return;
                    }
                }

                // at this point we know an exception is required - however, we want to tailor the message based on whether the
                // specified value is undefined or simply not allowed
                if (!Enum.IsDefined(typeof(TEnum), enumValue)) {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not defined for enumeration '{1}'.",
                            enumValue,
                            typeof(TEnum).FullName),
                        argName);
                } else {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is defined for enumeration '{1}' but it is not permitted in this context.",
                            enumValue,
                            typeof(TEnum).FullName),
                        argName);
                }
            }
        }

        public static void ConditionSupported(bool condition, string message, Exception innerException = null)
        {
            if (!condition) {
                throw new NotSupportedException(message, innerException);
            }
        }

        public static void ConditionSupported<T0>(bool condition, string format, T0 arg0, Exception innerException = null)
        {
            if (!condition) {
                throw new NotSupportedException(String.Format(format, arg0), innerException);
            }
        }

        public static void ConditionSupported<T0, T1>(bool condition, string format, T0 arg0, T1 arg1, Exception innerException = null)
        {
            if (!condition) {
                throw new NotSupportedException(String.Format(format, arg0, arg1), innerException);
            }
        }

        public static void ConditionSupported<T0, T1, T2>(bool condition, string format, T0 arg0, T1 arg1, T2 arg2, Exception innerException = null)
        {
            if (!condition) {
                throw new NotSupportedException(String.Format(format, arg0, arg1, arg2), innerException);
            }
        }

        public static void ConditionValid(bool condition, string message, Exception innerException = null)
        {
            if (!condition) {
                throw new InvalidOperationException(message, innerException);
            }
        }

        public static void ConditionValid<T0>(bool condition, string format, T0 arg0, Exception innerException = null)
        {
            if (!condition) {
                throw new InvalidOperationException(String.Format(format, arg0), innerException);
            }
        }

        public static void ConditionValid<T0, T1>(bool condition, string format, T0 arg0, T1 arg1, Exception innerException = null)
        {
            if (!condition) {
                throw new InvalidOperationException(String.Format(format, arg0, arg1), innerException);
            }
        }

        public static void ConditionValid<T0, T1, T2>(bool condition, string format, T0 arg0, T1 arg1, T2 arg2, Exception innerException = null)
        {
            if (!condition) {
                throw new InvalidOperationException(String.Format(format, arg0, arg1, arg2), innerException);
            }
        }

        // General purpose - avoid if possible. Only value it really adds is to make sure all guard logic routes through Ensure.
        public static void Condition(bool condition, Func<Exception> getException)
        {
            if (!condition) {
                throw getException();
            }
        }

        private static IEnumerable<T> GetEnumValues<T>()
        {
            var type = typeof(T);

            if (!type.GetTypeInfo().IsEnum) {
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");
            }

            return from field in type.GetTypeInfo().DeclaredFields.Where(field => field.IsPublic && field.IsStatic)
                   where field.IsLiteral
                   select (T)field.GetValue(null);
        }
    }
}