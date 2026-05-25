// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting DateTime to strings.
/// </summary>
public class DateTimeToStringTypeConverterTests
{
    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new DateTimeToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that a DateTime value converts to its string representation.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S6566:Prefer using DateTimeOffset instead of DateTime",
        Justification = "Converter under test operates on DateTime.")]
    public async Task TryConvert_DateTime_Succeeds()
    {
        var converter = new DateTimeToStringTypeConverter();
        var value = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Unspecified);

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that the minimum DateTime value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S6566:Prefer using DateTimeOffset instead of DateTime",
        Justification = "Converter under test operates on DateTime.")]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new DateTimeToStringTypeConverter();
        var value = DateTime.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(DateTime.MinValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that the maximum DateTime value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S6566:Prefer using DateTimeOffset instead of DateTime",
        Justification = "Converter under test operates on DateTime.")]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new DateTimeToStringTypeConverter();
        var value = DateTime.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(DateTime.MaxValue.ToString(CultureInfo.InvariantCulture));
    }
}
