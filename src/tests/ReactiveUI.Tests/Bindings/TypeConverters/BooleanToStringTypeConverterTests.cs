// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting booleans to strings.</summary>
public class BooleanToStringTypeConverterTests
{
    /// <summary>Verifies the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new BooleanToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that converting a true value produces the string "True".</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_True_ReturnsTrue()
    {
        var converter = new BooleanToStringTypeConverter();

        var result = converter.TryConvert(true, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("True");
    }

    /// <summary>Verifies that converting a false value produces the string "False".</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_False_ReturnsFalse()
    {
        var converter = new BooleanToStringTypeConverter();

        var result = converter.TryConvert(false, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("False");
    }
}
