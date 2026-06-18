// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI.Tests;

/// <summary>Tests for the built-in binding type converters.</summary>
public class BindingTypeConvertersUnitTests
{
    /// <summary>Verifies that <see cref="ByteToStringTypeConverter" /> converts a byte to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ByteToStringTypeConverter_Converts_Correctly()
    {
        var converter = new ByteToStringTypeConverter();
        const byte Val = 123;

        // Byte to String
        var result = converter.TryConvert(Val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123");
    }

    /// <summary>Verifies that <see cref="DecimalToStringTypeConverter" /> converts a decimal to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DecimalToStringTypeConverter_Converts_Correctly()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Val = 123.456m;

        var result = converter.TryConvert(Val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Val.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that <see cref="DoubleToStringTypeConverter" /> converts a double to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DoubleToStringTypeConverter_Converts_Correctly()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Val = 123.456789;

        var result = converter.TryConvert(Val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Val.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that <see cref="IntegerToStringTypeConverter" /> converts an integer to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IntegerToStringTypeConverter_Converts_Correctly()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Val = 123_456_789;

        var result = converter.TryConvert(Val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123456789");
    }

    /// <summary>Verifies that <see cref="LongToStringTypeConverter" /> converts a long to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task LongToStringTypeConverter_Converts_Correctly()
    {
        var converter = new LongToStringTypeConverter();
        const long Val = 1_234_567_890_123_456_789;

        var result = converter.TryConvert(Val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("1234567890123456789");
    }

    /// <summary>Verifies that <see cref="NullableByteToStringTypeConverter" /> converts a nullable byte to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableByteToStringTypeConverter_Converts_Correctly()
    {
        var converter = new NullableByteToStringTypeConverter();
        byte? val = 123;

        // Byte? to String
        var result = converter.TryConvert(val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123");
    }

    /// <summary>Verifies that <see cref="NullableDecimalToStringTypeConverter" /> converts a nullable decimal to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableDecimalToStringTypeConverter_Converts_Correctly()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? val = 123.456m;

        var result = converter.TryConvert(val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(val.Value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that <see cref="NullableDoubleToStringTypeConverter" /> converts a nullable double to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableDoubleToStringTypeConverter_Converts_Correctly()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        double? val = 123.456789;

        var result = converter.TryConvert(val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(val.Value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that <see cref="NullableIntegerToStringTypeConverter" /> converts a nullable integer to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableIntegerToStringTypeConverter_Converts_Correctly()
    {
        var converter = new NullableIntegerToStringTypeConverter();
        int? val = 123_456_789;

        var result = converter.TryConvert(val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123456789");
    }

    /// <summary>Verifies that <see cref="NullableLongToStringTypeConverter" /> converts a nullable long to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableLongToStringTypeConverter_Converts_Correctly()
    {
        var converter = new NullableLongToStringTypeConverter();
        long? val = 1_234_567_890_123_456_789;

        var result = converter.TryConvert(val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("1234567890123456789");
    }

    /// <summary>Verifies that <see cref="NullableShortToStringTypeConverter" /> converts a nullable short to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableShortToStringTypeConverter_Converts_Correctly()
    {
        var converter = new NullableShortToStringTypeConverter();
        short? val = 12_345;

        var result = converter.TryConvert(val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("12345");
    }

    /// <summary>Verifies that <see cref="NullableSingleToStringTypeConverter" /> converts a nullable single to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableSingleToStringTypeConverter_Converts_Correctly()
    {
        var converter = new NullableSingleToStringTypeConverter();
        float? val = 123.45f;

        var result = converter.TryConvert(val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(val.Value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that <see cref="ShortToStringTypeConverter" /> converts a short to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShortToStringTypeConverter_Converts_Correctly()
    {
        var converter = new ShortToStringTypeConverter();
        const short Val = 12_345;

        var result = converter.TryConvert(Val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("12345");
    }

    /// <summary>Verifies that <see cref="SingleToStringTypeConverter" /> converts a single to a string.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingleToStringTypeConverter_Converts_Correctly()
    {
        var converter = new SingleToStringTypeConverter();
        const float Val = 123.45f;

        var result = converter.TryConvert(Val, null, out var output);
        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Val.ToString(CultureInfo.InvariantCulture));
    }
}
