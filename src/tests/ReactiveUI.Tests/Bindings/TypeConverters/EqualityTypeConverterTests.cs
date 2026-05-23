// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
///     Tests for the EqualityTypeConverter which compares objects for equality.
/// </summary>
public class EqualityTypeConverterTests
{
    /// <summary>
    /// Verifies the converter source type is <see cref="object"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsObjectType()
    {
        var converter = new EqualityTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(object));
    }

    /// <summary>
    /// Verifies the converter reports an affinity of 1.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns1()
    {
        var converter = new EqualityTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(1);
    }

    /// <summary>
    /// Verifies the converter target type is <see cref="bool"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsBoolType()
    {
        var converter = new EqualityTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(bool));
    }

    /// <summary>
    /// Verifies that two null values are considered equal.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_BothNull_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsTrue();
    }

    /// <summary>
    /// Verifies that two different integers are not considered equal.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_DifferentIntegers_ReturnsFalse()
    {
        var converter = new EqualityTypeConverter();
        const int Obj = 42;
        const int Other = 43;

        var result = converter.TryConvertTyped(Obj, Other, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsFalse();
    }

    /// <summary>
    /// Verifies that values of different types are not considered equal.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_DifferentTypes_ReturnsFalse()
    {
        var converter = new EqualityTypeConverter();
        const string Obj = "42";
        const int OtherValue = 42;

        var result = converter.TryConvertTyped(Obj, OtherValue, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsFalse();
    }

    /// <summary>
    /// Verifies that two different string values are not considered equal.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_DifferentValues_ReturnsFalse()
    {
        var converter = new EqualityTypeConverter();
        const string Obj = "test";

        var result = converter.TryConvertTyped(Obj, "other", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsFalse();
    }

    /// <summary>
    /// Verifies that two equal integers are considered equal.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_EqualIntegers_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        const int Obj = 42;

        var result = converter.TryConvertTyped(Obj, Obj, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsTrue();
    }

    /// <summary>
    /// Verifies that two equal strings are considered equal.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_EqualStrings_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        const string Obj = "hello";

        var result = converter.TryConvertTyped(Obj, "hello", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsTrue();
    }

    /// <summary>
    /// Verifies that two equal values are considered equal.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_EqualValues_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        const string Obj = "test";

        var result = converter.TryConvertTyped(Obj, "test", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsTrue();
    }

    /// <summary>
    /// Verifies that a null conversion hint falls back to null comparison.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_NoConversionHint_UseNullComparison()
    {
        var converter = new EqualityTypeConverter();
        const string Obj = "test";

        var result = converter.TryConvertTyped(Obj, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsFalse();
    }

    /// <summary>
    /// Verifies that comparing a value against null is not considered equal.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_OneNull_ReturnsFalse()
    {
        var converter = new EqualityTypeConverter();

        var result1 = converter.TryConvertTyped("test", null, out var output1);
        var result2 = converter.TryConvertTyped(null, "test", out var output2);

        await Assert.That(result1).IsTrue();
        await Assert.That(output1).IsNotNull();
        await Assert.That((bool)output1!).IsFalse();
        await Assert.That(result2).IsTrue();
        await Assert.That(output2).IsNotNull();
        await Assert.That((bool)output2!).IsFalse();
    }

    /// <summary>
    /// Verifies that the same reference is considered equal to itself.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_ReferenceEquality_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        var obj = new object();

        var result = converter.TryConvertTyped(obj, obj, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsTrue();
    }

    /// <summary>
    /// Verifies that two structurally equal values are considered equal.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_ValueEquality_ReturnsTrue()
    {
        var converter = new EqualityTypeConverter();
        var obj = new { Value = "test" };
        var other = new { Value = "test" };

        var result = converter.TryConvertTyped(obj, other, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That((bool)output!).IsTrue();
    }
}
