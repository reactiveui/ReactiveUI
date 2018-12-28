// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Foundation;

namespace ReactiveUI
{
    /// <summary>
    /// Binding Type Converter for DateTime to NSDateTime.
    /// </summary>
    public class DateTimeNSDateConverter : IBindingTypeConverter
    {
        internal static Lazy<DateTimeNSDateConverter> Instance { get; } = new Lazy<DateTimeNSDateConverter>();

        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (fromType == typeof(DateTime) && toType == typeof(NSDate)) ||
                (toType == typeof(DateTime) && fromType == typeof(NSDate)) ? 4 : -1;
        }

        /// <inheritdoc/>
        public bool TryConvert(object val, Type toType, object conversionHint, out object result)
        {
            result = null;

            if (val.GetType() == typeof(DateTime) && toType == typeof(NSDate))
            {
                var dt = (DateTime)val;
                result = (NSDate)dt;
                return true;
            }
            else if (val.GetType() == typeof(NSDate) && toType == typeof(DateTime))
            {
                var dt = (NSDate)val;
                result = (DateTime)dt;
                return true;
            }

            return false;
        }
    }
}
