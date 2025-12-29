// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Binding;

/// <summary>
/// Tests for <see cref="ComponentModelTypeConverter"/>.
/// </summary>
public class ComponentModelTypeConverterTest
{
    /// <summary>
    /// Tests that GetAffinityForObjects returns correct affinity for types with TypeConverter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_WithTypeConverter_Returns10()
    {
        var converter = new ComponentModelTypeConverter();

        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(int));

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that GetAffinityForObjects returns zero for types without converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_WithoutTypeConverter_Returns0()
    {
        var converter = new ComponentModelTypeConverter();

        // Object to StringBuilder has no standard converter
        var affinity = converter.GetAffinityForObjects(typeof(object), typeof(System.Text.StringBuilder));

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>
    /// Tests that TryConvert converts string to int successfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_StringToInt_ConvertsSuccessfully()
    {
        var converter = new ComponentModelTypeConverter();

        var success = converter.TryConvert("42", typeof(int), null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(42);
    }

    /// <summary>
    /// Tests that TryConvert converts int to string successfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_IntToString_ConvertsSuccessfully()
    {
        var converter = new ComponentModelTypeConverter();

        var success = converter.TryConvert(42, typeof(string), null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo("42");
    }

    /// <summary>
    /// Tests that TryConvert handles null values correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_NullValue_ReturnsNullAndTrue()
    {
        var converter = new ComponentModelTypeConverter();

        var success = converter.TryConvert(null, typeof(int), null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Tests that TryConvert handles format exceptions gracefully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_InvalidFormat_ReturnsFalse()
    {
        var converter = new ComponentModelTypeConverter();

        var success = converter.TryConvert("not a number", typeof(int), null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Tests that TryConvert handles empty strings gracefully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new ComponentModelTypeConverter();

        var success = converter.TryConvert(string.Empty, typeof(int), null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Tests that TryConvert throws exception for incompatible types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_IncompatibleTypes_Throws()
    {
        var converter = new ComponentModelTypeConverter();

        await Assert.That(() => converter.TryConvert(new object(), typeof(System.Text.StringBuilder), null, out _))
            .Throws<ArgumentException>();
    }

    /// <summary>
    /// Tests that TryConvert converts string to double successfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_StringToDouble_ConvertsSuccessfully()
    {
        var converter = new ComponentModelTypeConverter();

        var success = converter.TryConvert("3.14", typeof(double), null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(3.14);
    }

    /// <summary>
    /// Tests that TryConvert converts string to bool successfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_StringToBool_ConvertsSuccessfully()
    {
        var converter = new ComponentModelTypeConverter();

        var successTrue = converter.TryConvert("true", typeof(bool), null, out var resultTrue);
        var successFalse = converter.TryConvert("false", typeof(bool), null, out var resultFalse);

        await Assert.That(successTrue).IsTrue();
        await Assert.That(resultTrue).IsEqualTo(true);
        await Assert.That(successFalse).IsTrue();
        await Assert.That(resultFalse).IsEqualTo(false);
    }

    /// <summary>
    /// Tests that GetAffinityForObjects caches results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_CachesResults()
    {
        var converter = new ComponentModelTypeConverter();

        var affinity1 = converter.GetAffinityForObjects(typeof(string), typeof(int));
        var affinity2 = converter.GetAffinityForObjects(typeof(string), typeof(int));

        await Assert.That(affinity1).IsEqualTo(affinity2);
        await Assert.That(affinity1).IsEqualTo(10);
    }
}
