// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting int to nullable int.
/// </summary>
public class IntegerToNullableIntegerTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_AlwaysSucceeds()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();
        int value = 123456;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((int?)123456);
    }

    [Test]
    public async Task FromType_ReturnsInt()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(int));
    }

    [Test]
    public async Task ToType_ReturnsIntNullable()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(int?));
    }
}
