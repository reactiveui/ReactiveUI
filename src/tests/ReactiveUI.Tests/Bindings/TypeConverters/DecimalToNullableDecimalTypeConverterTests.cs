// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting decimal to nullable decimal.
/// </summary>
public class DecimalToNullableDecimalTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new DecimalToNullableDecimalTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_AlwaysSucceeds()
    {
        var converter = new DecimalToNullableDecimalTypeConverter();
        decimal value = 123.456789m;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((decimal?)123.456789m);
    }

    [Test]
    public async Task FromType_ReturnsDecimal()
    {
        var converter = new DecimalToNullableDecimalTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(decimal));
    }

    [Test]
    public async Task ToType_ReturnsDecimalNullable()
    {
        var converter = new DecimalToNullableDecimalTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(decimal?));
    }
}
