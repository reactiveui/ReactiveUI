// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to booleans.
/// </summary>
public class StringToBooleanTypeConverterTests
{
    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToBooleanTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that the string "True" converts to a true boolean.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_TrueString_Succeeds()
    {
        var converter = new StringToBooleanTypeConverter();

        var result = converter.TryConvert("True", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsTrue();
    }

    /// <summary>
    /// Verifies that the string "False" converts to a false boolean.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_FalseString_Succeeds()
    {
        var converter = new StringToBooleanTypeConverter();

        var result = converter.TryConvert("False", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsFalse();
    }

    /// <summary>
    /// Verifies that the lowercase string "true" converts to a true boolean.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_TrueLowercase_Succeeds()
    {
        var converter = new StringToBooleanTypeConverter();

        var result = converter.TryConvert("true", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsTrue();
    }

    /// <summary>
    /// Verifies that the lowercase string "false" converts to a false boolean.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_FalseLowercase_Succeeds()
    {
        var converter = new StringToBooleanTypeConverter();

        var result = converter.TryConvert("false", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsFalse();
    }

    /// <summary>
    /// Verifies that a null input fails to convert.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new StringToBooleanTypeConverter();

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
        var converter = new StringToBooleanTypeConverter();

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
        var converter = new StringToBooleanTypeConverter();

        var result = converter.TryConvert("invalid", null, out _);

        await Assert.That(result).IsFalse();
    }
}
