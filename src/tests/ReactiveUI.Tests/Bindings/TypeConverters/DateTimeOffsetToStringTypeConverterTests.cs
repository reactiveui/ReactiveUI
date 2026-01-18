// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting DateTimeOffset to strings.
/// </summary>
public class DateTimeOffsetToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new DateTimeOffsetToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_DateTimeOffset_Succeeds()
    {
        var converter = new DateTimeOffsetToStringTypeConverter();
        var value = new DateTimeOffset(2024, 1, 15, 10, 30, 45, TimeSpan.FromHours(-5));

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new DateTimeOffsetToStringTypeConverter();
        var value = DateTimeOffset.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(DateTimeOffset.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new DateTimeOffsetToStringTypeConverter();
        var value = DateTimeOffset.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(DateTimeOffset.MaxValue.ToString());
    }
}
