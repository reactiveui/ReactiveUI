using System;
using System.ComponentModel;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// This binding type converter uses the built-in WPF component model conversions to get a whole
    /// bunch of conversions for free. Unfortunately, these are pretty gutted on some other platforms
    /// like Silverlight.
    /// </summary>
    public class ComponentModelTypeConverter : IBindingTypeConverter
    {
        private readonly MemoizingMRUCache<Tuple<Type, Type>, TypeConverter> typeConverterCache = new MemoizingMRUCache<Tuple<Type, Type>, TypeConverter>((types, _) => {

            // NB: String is a Magical Type(tm) to TypeConverters. If we are converting from string
            // => int, we need the Int converter, not the string converter :-/
            if (types.Item1 == typeof(string)) {
                types = Tuple.Create(types.Item2, types.Item1);
            }

            var converter = TypeDescriptor.GetConverter(types.Item1);
            return converter.CanConvertTo(types.Item2) ? converter : null;
        }, RxApp.SmallCacheLimit);

        /// <summary>
        /// Returns a positive integer when this class supports TryConvert for this particular Type.
        /// If the method isn't supported at all, return a non-positive integer. When multiple
        /// implementations return a positive value, the host will use the one which returns the
        /// highest value. When in doubt, return '2' or '0'.
        /// </summary>
        /// <param name="fromType">The source type to convert from</param>
        /// <param name="toType">The target type to convert to</param>
        /// <returns>
        /// A positive integer if TryConvert is supported, zero or a negative value otherwise
        /// </returns>
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            var converter = this.typeConverterCache.Get(Tuple.Create(fromType, toType));
            return converter != null ? 10 : 0;
        }

        /// <summary>
        /// Convert a given object to the specified type.
        /// </summary>
        /// <param name="from">The object to convert.</param>
        /// <param name="toType">The type to coerce the object to.</param>
        /// <param name="conversionHint">
        /// An implementation-defined value, usually to specify things like locale awareness.
        /// </param>
        /// <param name="result"></param>
        /// <returns>An object that is of the type 'to'</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Exception"></exception>
        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            if (from == null) {
                result = null;
                return true;
            }

            var fromType = from.GetType();
            var converter = this.typeConverterCache.Get(Tuple.Create(fromType, toType));

            if (converter == null) {
                throw new ArgumentException(string.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", fromType, toType));
            }

            try {

                // TODO: This should use conversionHint to determine whether this is locale-aware or not
                result = (fromType == typeof(string)) ?
                    converter.ConvertFrom(from) : converter.ConvertTo(from, toType);

                return true;
            } catch (FormatException) {
                result = null;
                return false;
            } catch (Exception e) {

                // Errors from ConvertFrom end up here but wrapped in outer exception. Add more types
                // here as required. IndexOutOfRangeException is given when trying to convert empty
                // strings with some/all? converters
                if (e.InnerException is IndexOutOfRangeException ||
                    e.InnerException is FormatException) {
                    result = null;
                    return false;
                } else {
                    throw new Exception(string.Format("Can't convert from {0} to {1}.", from.GetType(), toType), e);
                }
            }
        }
    }
}