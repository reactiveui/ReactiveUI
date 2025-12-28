// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class EqualityTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_AssignableTypes_Returns100()
    {
        var converter = new EqualityTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(object));
        await Assert.That(affinity).IsEqualTo(100);
    }

    [Test]
    public async Task GetAffinityForObjects_SameType_Returns100()
    {
        var converter = new EqualityTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(int), typeof(int));
        await Assert.That(affinity).IsEqualTo(100);
    }

    [Test]
    public async Task GetAffinityForObjects_ObjectType_Returns100()
    {
        var converter = new EqualityTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(object), typeof(string));
        await Assert.That(affinity).IsEqualTo(100);
    }

    [Test]
    public async Task GetAffinityForObjects_NullableTypes_ReturnsAffinity()
    {
        var converter = new EqualityTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(int?), typeof(int));
        await Assert.That(affinity).IsGreaterThan(0);
    }

    [Test]
    public async Task GetAffinityForObjects_ToNullableTypes_ReturnsAffinity()
    {
        var converter = new EqualityTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(int), typeof(int?));
        await Assert.That(affinity).IsGreaterThan(0);
    }

    [Test]
    public async Task GetAffinityForObjects_IncompatibleTypes_Returns0()
    {
        var converter = new EqualityTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(int), typeof(string));
        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_SameType_Succeeds()
    {
        var converter = new EqualityTypeConverter();
        var value = "test";

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value);
    }

    [Test]
    public async Task TryConvert_AssignableType_Succeeds()
    {
        var converter = new EqualityTypeConverter();
        var value = "test";

        var result = converter.TryConvert(value, typeof(object), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value);
    }

    [Test]
    public async Task TryConvert_NullToReferenceType_Succeeds()
    {
        var converter = new EqualityTypeConverter();

        var result = converter.TryConvert(null, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task TryConvert_NullToNullableValueType_Succeeds()
    {
        var converter = new EqualityTypeConverter();

        var result = converter.TryConvert(null, typeof(int?), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task TryConvert_NullToValueType_Fails()
    {
        var converter = new EqualityTypeConverter();

        var result = converter.TryConvert(null, typeof(int), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_IncompatibleTypes_Fails()
    {
        var converter = new EqualityTypeConverter();
        var value = "test";

        var result = converter.TryConvert(value, typeof(int), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_ValueToNullableType_Succeeds()
    {
        var converter = new EqualityTypeConverter();
        int value = 42;

        var result = converter.TryConvert(value, typeof(int?), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(42);
    }

    [Test]
    public async Task DoReferenceCast_NullToReferenceType_ReturnsNull()
    {
        var result = EqualityTypeConverter.DoReferenceCast(null, typeof(string));
        await Assert.That(result).IsNull();
    }

    [Test]
    public void DoReferenceCast_NullToValueType_Throws()
    {
        Assert.Throws<InvalidCastException>(() => EqualityTypeConverter.DoReferenceCast(null, typeof(int)));
    }

    [Test]
    public async Task DoReferenceCast_SameType_ReturnsValue()
    {
        var value = "test";
        var result = EqualityTypeConverter.DoReferenceCast(value, typeof(string));
        await Assert.That(result).IsEqualTo(value);
    }

    [Test]
    public async Task DoReferenceCast_NullToNullableType_ReturnsNull()
    {
        var result = EqualityTypeConverter.DoReferenceCast(null, typeof(int?));
        await Assert.That(result).IsNull();
    }

    [Test]
    public void DoReferenceCast_IncompatibleTypes_Throws()
    {
        Assert.Throws<InvalidCastException>(() => EqualityTypeConverter.DoReferenceCast("test", typeof(int)));
    }

    [Test]
    public void DoReferenceCast_NullTargetType_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => EqualityTypeConverter.DoReferenceCast("test", null!));
    }
}
