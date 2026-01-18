// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting TimeSpan to strings.
/// </summary>
public class TimeSpanToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new TimeSpanToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_TimeSpan_Succeeds()
    {
        var converter = new TimeSpanToStringTypeConverter();
        var value = TimeSpan.FromHours(2.5);

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_ZeroTimeSpan_Succeeds()
    {
        var converter = new TimeSpanToStringTypeConverter();
        var value = TimeSpan.Zero;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("00:00:00");
    }

    [Test]
    public async Task TryConvert_NegativeTimeSpan_Succeeds()
    {
        var converter = new TimeSpanToStringTypeConverter();
        var value = TimeSpan.FromMinutes(-30);

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }
}
