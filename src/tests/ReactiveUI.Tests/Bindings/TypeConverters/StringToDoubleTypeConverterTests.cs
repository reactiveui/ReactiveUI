// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to doubles.
/// </summary>
public class StringToDoubleTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns10()
    {
        var converter = new StringToDoubleTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task TryConvert_StringToDouble_Succeeds()
    {
        var converter = new StringToDoubleTypeConverter();

        var result = converter.TryConvert("123.456", null, out double output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123.456);
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToDoubleTypeConverter();

        var result = converter.TryConvert("invalid", null, out double output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_ScientificNotation_Succeeds()
    {
        var converter = new StringToDoubleTypeConverter();

        var result = converter.TryConvert("1.23E+10", null, out double output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(1.23E+10);
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new StringToDoubleTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out double output);

        await Assert.That(result).IsFalse();
    }
}
