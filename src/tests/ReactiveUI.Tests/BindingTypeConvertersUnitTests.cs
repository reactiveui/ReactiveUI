// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace ReactiveUI.Tests
{
    public class BindingTypeConvertersUnitTests
    {
        [Test]
        public async Task ByteToStringTypeConverter_Converts_Correctly()
        {
            var converter = new ByteToStringTypeConverter();
            byte val = 123;

            // Byte to String
            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo("123");
        }

        [Test]
        public async Task NullableByteToStringTypeConverter_Converts_Correctly()
        {
            var converter = new NullableByteToStringTypeConverter();
            byte? val = 123;

            // Byte? to String
            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo("123");
        }

        [Test]
        public async Task ShortToStringTypeConverter_Converts_Correctly()
        {
            var converter = new ShortToStringTypeConverter();
            short val = 12345;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo("12345");
        }

        [Test]
        public async Task NullableShortToStringTypeConverter_Converts_Correctly()
        {
            var converter = new NullableShortToStringTypeConverter();
            short? val = 12345;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo("12345");
        }

        [Test]
        public async Task IntegerToStringTypeConverter_Converts_Correctly()
        {
            var converter = new IntegerToStringTypeConverter();
            int val = 123456789;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo("123456789");
        }

        [Test]
        public async Task NullableIntegerToStringTypeConverter_Converts_Correctly()
        {
            var converter = new NullableIntegerToStringTypeConverter();
            int? val = 123456789;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo("123456789");
        }

        [Test]
        public async Task LongToStringTypeConverter_Converts_Correctly()
        {
            var converter = new LongToStringTypeConverter();
            long val = 1234567890123456789;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo("1234567890123456789");
        }

        [Test]
        public async Task NullableLongToStringTypeConverter_Converts_Correctly()
        {
            var converter = new NullableLongToStringTypeConverter();
            long? val = 1234567890123456789;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo("1234567890123456789");
        }

        [Test]
        public async Task SingleToStringTypeConverter_Converts_Correctly()
        {
            var converter = new SingleToStringTypeConverter();
            float val = 123.45f;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo(val.ToString());
        }

        [Test]
        public async Task NullableSingleToStringTypeConverter_Converts_Correctly()
        {
            var converter = new NullableSingleToStringTypeConverter();
            float? val = 123.45f;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo(val.ToString());
        }

        [Test]
        public async Task DoubleToStringTypeConverter_Converts_Correctly()
        {
            var converter = new DoubleToStringTypeConverter();
            double val = 123.456789;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo(val.ToString());
        }

        [Test]
        public async Task NullableDoubleToStringTypeConverter_Converts_Correctly()
        {
            var converter = new NullableDoubleToStringTypeConverter();
            double? val = 123.456789;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo(val.ToString());
        }

        [Test]
        public async Task DecimalToStringTypeConverter_Converts_Correctly()
        {
            var converter = new DecimalToStringTypeConverter();
            decimal val = 123.456m;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo(val.ToString());
        }

        [Test]
        public async Task NullableDecimalToStringTypeConverter_Converts_Correctly()
        {
            var converter = new NullableDecimalToStringTypeConverter();
            decimal? val = 123.456m;

            var result = converter.TryConvert(val, null, out string? output);
            await Assert.That(result).IsTrue();
            await Assert.That(output).IsEqualTo(val.ToString());
        }
    }
}
