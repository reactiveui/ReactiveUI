// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting nullable float to float.
/// </summary>
public class NullableSingleToSingleTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableSingleToSingleTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_WithValue_Succeeds()
    {
        var converter = new NullableSingleToSingleTypeConverter();
        float? value = 123.45f;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123.45f);
    }

    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new NullableSingleToSingleTypeConverter();
        float? value = null;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task FromType_ReturnsFloatNullable()
    {
        var converter = new NullableSingleToSingleTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(float?));
    }

    [Test]
    public async Task ToType_ReturnsFloat()
    {
        var converter = new NullableSingleToSingleTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(float));
    }
}
