// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting shorts to strings.</summary>
public class ShortToStringTypeConverterTests
{
    /// <summary>Verifies the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new ShortToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that the maximum short value converts successfully.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new ShortToStringTypeConverter();
        const short Value = short.MaxValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(short.MaxValue.ToString());
    }

    /// <summary>Verifies that the minimum short value converts successfully.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new ShortToStringTypeConverter();
        const short Value = short.MinValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(short.MinValue.ToString());
    }

    /// <summary>Verifies that a short value converts to its string representation.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_ShortToString_Succeeds()
    {
        var converter = new ShortToStringTypeConverter();
        const short Value = 12_345;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("12345");
    }

    /// <summary>Verifies that a conversion hint applies the expected formatting.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new ShortToStringTypeConverter();
        const short Value = 42;
        const int PaddingWidth = 5;

        var result = converter.TryConvert(Value, PaddingWidth, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("00042");
    }
}
