// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class StringConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_ToStringType_Returns2()
    {
        var converter = new StringConverter();
        var affinity = converter.GetAffinityForObjects(typeof(int), typeof(string));
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task GetAffinityForObjects_FromStringType_Returns2()
    {
        var converter = new StringConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(string));
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task GetAffinityForObjects_NotStringType_Returns0()
    {
        var converter = new StringConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(int));
        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_IntToString_Succeeds()
    {
        var converter = new StringConverter();
        var value = 123;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123");
    }

    [Test]
    public async Task TryConvert_StringToString_Succeeds()
    {
        var converter = new StringConverter();
        var value = "test";

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("test");
    }

    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new StringConverter();

        var result = converter.TryConvert(null, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task TryConvert_ObjectToString_Succeeds()
    {
        var converter = new StringConverter();
        var value = new TestObject { Value = "test" };

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("TestObject: test");
    }

    [Test]
    public async Task TryConvert_DoubleToString_Succeeds()
    {
        var converter = new StringConverter();
        var value = 123.456;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_BoolToString_Succeeds()
    {
        var converter = new StringConverter();
        var value = true;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("True");
    }

    [Test]
    public async Task TryConvert_DateTimeToString_Succeeds()
    {
        var converter = new StringConverter();
        var value = new DateTime(2025, 1, 1, 12, 0, 0);

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_IgnoresConversionHint()
    {
        var converter = new StringConverter();
        var value = 123;

        var result = converter.TryConvert(value, typeof(string), "some hint", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123");
    }

    private class TestObject
    {
        public string Value { get; set; } = string.Empty;

        public override string ToString() => $"TestObject: {Value}";
    }
}
