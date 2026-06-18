// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting nullable Guid to strings.</summary>
public class NullableGuidToStringTypeConverterTests
{
    /// <summary>Verifies that the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableGuidToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that converting a GUID value to a string succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_Guid_Succeeds()
    {
        var converter = new NullableGuidToStringTypeConverter();
        Guid? value = Guid.Parse("12345678-1234-1234-1234-123456789abc");

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("12345678-1234-1234-1234-123456789abc");
    }

    /// <summary>Verifies that converting a null value succeeds and yields a null string.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_Null_ReturnsNullString()
    {
        var converter = new NullableGuidToStringTypeConverter();
        Guid? value = null;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNull();
    }
}
