// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class SingleToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_SingleToString_Returns10()
    {
        var converter = new SingleToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(float), typeof(string));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_StringToSingle_Returns10()
    {
        var converter = new SingleToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(float));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_WrongTypes_Returns0()
    {
        var converter = new SingleToStringTypeConverter();

        await Assert.That(converter.GetAffinityForObjects(typeof(int), typeof(string))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(string), typeof(int))).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_SingleToString_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        float value = 123.456f;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_StringToSingle_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();

        var result = converter.TryConvert("123.456", typeof(float), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That((float)output!).IsEqualTo(123.456f).Within(0.001f);
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new SingleToStringTypeConverter();

        var result = converter.TryConvert("invalid", typeof(float), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new SingleToStringTypeConverter();
        float value = 42.5f;

        var result = converter.TryConvert(value, typeof(string), 2, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        float value = float.MinValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(float.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        float value = float.MaxValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(float.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        float value = -123.456f;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new SingleToStringTypeConverter();

        var result = converter.TryConvert(string.Empty, typeof(float), null, out var output);

        await Assert.That(result).IsFalse();
    }
}
