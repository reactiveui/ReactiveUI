// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to Guid.
/// </summary>
public class StringToGuidTypeConverterTests
{
    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToGuidTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that a valid GUID string converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_ValidString_Succeeds()
    {
        var converter = new StringToGuidTypeConverter();
        var expected = Guid.Parse("12345678-1234-1234-1234-123456789abc");

        var result = converter.TryConvert("12345678-1234-1234-1234-123456789abc", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(expected);
    }

    /// <summary>
    /// Verifies that an empty GUID string converts to <see cref="Guid.Empty"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_EmptyGuidString_Succeeds()
    {
        var converter = new StringToGuidTypeConverter();

        var result = converter.TryConvert("00000000-0000-0000-0000-000000000000", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Guid.Empty);
    }

    /// <summary>
    /// Verifies that a null input fails to convert.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new StringToGuidTypeConverter();

        var result = converter.TryConvert(null, null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that an empty string fails to convert.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new StringToGuidTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that an invalid string fails to convert.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToGuidTypeConverter();

        var result = converter.TryConvert("invalid", null, out _);

        await Assert.That(result).IsFalse();
    }
}
