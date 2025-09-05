// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for binding type converters.
/// </summary>
public class BindingTypeConvertersTest
{
    /// <summary>
    /// Tests that equality type converter do reference cast should convert null nullable values.
    /// </summary>
    [Test]
    public void EqualityTypeConverterDoReferenceCastShouldConvertNullNullableValues()
    {
        double? nullDouble = null;
        double? expected = null;
        var result = EqualityTypeConverter.DoReferenceCast(nullDouble, typeof(double?));
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that equality type converter do reference cast should convert nullable values.
    /// </summary>
    [Test]
    public void EqualityTypeConverterDoReferenceCastShouldConvertNullableValues()
    {
        double? doubleValue = 1.0;
        double? expected = 1.0;
        var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double?));
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that equality type converter do reference cast should throw when converting from null nullable to value.
    /// </summary>
    [Test]
    public void EqualityTypeConverterDoReferenceCastShouldThrowWhenConvertingFromNullNullableToValueType()
    {
        double? nullDouble = null;
        Assert.Throws<InvalidCastException>(() => EqualityTypeConverter.DoReferenceCast(nullDouble, typeof(double)));
    }

    /// <summary>
    /// Tests that equality type converter do reference cast nullable to value.
    /// </summary>
    [Test]
    public void EqualityTypeConverterDoReferenceCastNullableToValueType()
    {
        double? doubleValue = 1.0;
        double? expected = 1.0;
        var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double));
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that equality type converter do reference cast should convert value types.
    /// </summary>
    [Test]
    public void EqualityTypeConverterDoReferenceCastShouldConvertValueTypes()
    {
        const double doubleValue = 1.0;
        var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double));
        Assert.That(result, Is.EqualTo(doubleValue));
    }
}
