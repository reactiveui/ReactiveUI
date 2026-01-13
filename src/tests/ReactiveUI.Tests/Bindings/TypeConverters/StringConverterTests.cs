// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class StringConverterTests
{
    [Test]
    public async Task FromType_ReturnsStringType()
    {
        var converter = new StringConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(string));
    }

    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task ToType_ReturnsStringType()
    {
        var converter = new StringConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(string));
    }

    [Test]
    public async Task TryConvertTyped_EmptyString_Succeeds()
    {
        var converter = new StringConverter();
        var value = string.Empty;

        var result = converter.TryConvertTyped(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task TryConvertTyped_IgnoresConversionHint()
    {
        var converter = new StringConverter();
        var value = "test";

        var result = converter.TryConvertTyped(value, "some hint", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("test");
    }

    [Test]
    public async Task TryConvertTyped_NonStringValue_ReturnsFalse()
    {
        var converter = new StringConverter();
        var value = 123;

        var result = converter.TryConvertTyped(value, null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvertTyped_NullValue_ReturnsFalse()
    {
        var converter = new StringConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task TryConvertTyped_StringToString_Succeeds()
    {
        var converter = new StringConverter();
        var value = "test";

        var result = converter.TryConvertTyped(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("test");
    }

    private class TestObject
    {
        public string Value { get; } = string.Empty;

        public override string ToString() => $"TestObject: {Value}";
    }
}
