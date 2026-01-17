// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class SingleToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new SingleToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        var value = float.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(float.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        var value = float.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(float.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        var value = -123.456f;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_SingleToString_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        var value = 123.456f;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new SingleToStringTypeConverter();
        var value = 42.5f;

        var result = converter.TryConvert(value, 2, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }
}
