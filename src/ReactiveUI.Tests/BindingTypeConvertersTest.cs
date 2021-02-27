// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Xunit;

namespace ReactiveUI.Tests
{
    using System;

    /// <summary>
    /// Tests for binding type converters.
    /// </summary>
    public class BindingTypeConvertersTest
    {
        /// <summary>
        /// Tests that equality type converter do reference cast should convert null nullable values.
        /// </summary>
        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertNullNullableValues()
        {
            double? nullDouble = null;
            double? expected = null;
            var result = EqualityTypeConverter.DoReferenceCast(nullDouble, typeof(double?));
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Tests that equality type converter do reference cast should convert nullable values.
        /// </summary>
        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertNullableValues()
        {
            double? doubleValue = 1.0;
            double? expected = 1.0;
            var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double?));
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Tests that equality type converter do reference cast should throw when converting from null nullable to value.
        /// </summary>
        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldThrowWhenConvertingFromNullNullableToValueType()
        {
            double? nullDouble = null;
            Assert.Throws<InvalidCastException>(() => EqualityTypeConverter.DoReferenceCast(nullDouble, typeof(double)));
        }

        /// <summary>
        /// Tests that equality type converter do reference cast nullable to value.
        /// </summary>
        [Fact]
        public void EqualityTypeConverterDoReferenceCastNullableToValueType()
        {
            double? doubleValue = 1.0;
            double? expected = 1.0;
            var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double));
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Tests that equality type converter do reference cast should convert value types.
        /// </summary>
        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertValueTypes()
        {
            var doubleValue = 1.0;
            var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double));
            Assert.Equal(doubleValue, result);
        }
    }
}
