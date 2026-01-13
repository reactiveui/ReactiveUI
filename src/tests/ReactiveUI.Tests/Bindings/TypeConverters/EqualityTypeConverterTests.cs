// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
///     Tests for the EqualityTypeConverter which compares objects for equality.
/// </summary>
public class EqualityTypeConverterTests
{
    [Test]
    public async Task FromType_ReturnsObjectType()
    {
        var converter = new EqualityTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(object));
    }

    [Test]
    public async Task GetAffinityForObjects_Returns1()
    {
        var converter = new EqualityTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(1);
    }

    [Test]
    public async Task ToType_ReturnsBoolType()
    {
        var converter = new EqualityTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(bool));
    }

    [Test]
    public async Task TryConvertTyped_BothNull_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(true);
    }

    [Test]
    public async Task TryConvertTyped_DifferentIntegers_ReturnsFalse()
    {
        var converter = new EqualityTypeConverter();
        var obj = 42;

        var result = converter.TryConvertTyped(obj, 43, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(false);
    }

    [Test]
    public async Task TryConvertTyped_DifferentTypes_ReturnsFalse()
    {
        var converter = new EqualityTypeConverter();
        var obj = "42";

        var result = converter.TryConvertTyped(obj, 42, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(false);
    }

    [Test]
    public async Task TryConvertTyped_DifferentValues_ReturnsFalse()
    {
        var converter = new EqualityTypeConverter();
        var obj = "test";

        var result = converter.TryConvertTyped(obj, "other", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(false);
    }

    [Test]
    public async Task TryConvertTyped_EqualIntegers_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        var obj = 42;

        var result = converter.TryConvertTyped(obj, 42, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(true);
    }

    [Test]
    public async Task TryConvertTyped_EqualStrings_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        var obj = "hello";

        var result = converter.TryConvertTyped(obj, "hello", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(true);
    }

    [Test]
    public async Task TryConvertTyped_EqualValues_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        var obj = "test";

        var result = converter.TryConvertTyped(obj, "test", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(true);
    }

    [Test]
    public async Task TryConvertTyped_NoConversionHint_UseNullComparison()
    {
        var converter = new EqualityTypeConverter();
        var obj = "test";

        var result = converter.TryConvertTyped(obj, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(false);
    }

    [Test]
    public async Task TryConvertTyped_OneNull_ReturnsFalse()
    {
        var converter = new EqualityTypeConverter();

        var result1 = converter.TryConvertTyped("test", null, out var output1);
        var result2 = converter.TryConvertTyped(null, "test", out var output2);

        await Assert.That(result1).IsTrue();
        await Assert.That(output1).IsEqualTo(false);
        await Assert.That(result2).IsTrue();
        await Assert.That(output2).IsEqualTo(false);
    }

    [Test]
    public async Task TryConvertTyped_ReferenceEquality_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        var obj = new object();

        var result = converter.TryConvertTyped(obj, obj, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(true);
    }

    [Test]
    public async Task TryConvertTyped_ValueEquality_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        var obj = new { Value = "test" };
        var other = new { Value = "test" };

        var result = converter.TryConvertTyped(obj, other, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(true);
    }
}
