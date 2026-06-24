// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests for the reactive notify property changed mixin (WhenAny, WhenAnyValue, ObservableForProperty).</summary>
public partial class ReactiveNotifyPropertyChangedMixinTest
{
    /// <summary>WhenAnyValue with ten parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Selector arity mirrors the variadic production WhenAny API.")]
    public async Task WhenAnyValueWith10ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            x => x.Value6,
            x => x.Value7,
            x => x.Value8,
            x => x.Value9,
            x => x.Value10,
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10)).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5, value6, value7, value8, value9, value10) = tuple;

            return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8 + value9 + value10;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo("13579");
    }

    /// <summary>WhenAnyValue with eleven parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Selector arity mirrors the variadic production WhenAny API.")]
    public async Task WhenAnyValueWith11ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            x => x.Value6,
            x => x.Value7,
            x => x.Value8,
            x => x.Value9,
            x => x.Value10,
            x => x.Value11,
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) =>
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11)).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11) =
                tuple;

            return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8 + value9 + value10 +
                   value11;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo("1357911");
    }

    /// <summary>WhenAnyValue with twelve parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Selector arity mirrors the variadic production WhenAny API.")]
    public async Task WhenAnyValueWith12ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            x => x.Value6,
            x => x.Value7,
            x => x.Value8,
            x => x.Value9,
            x => x.Value10,
            x => x.Value11,
            x => x.Value12,
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) =>
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12)).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11,
                value12) = tuple;

            return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8 + value9 + value10 +
                   value11 + value12;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo("1357911");
    }

    /// <summary>WhenAnyValue with one parameter returns the value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith1Paramerters()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(x => x.Value1).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneText);
    }

    /// <summary>WhenAnyValue with one parameter reflects sequential changes (nullable target set later).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith1ParamertersSequentialCheck()
    {
        var fixture = new WhenAnyTestFixture();

        var result = string.Empty;

        fixture.Value1 = null!;

        _ = fixture.WhenAnyValue(x => x.Value1).Subscribe(value => result = value);

        await Assert.That(result).IsNull();

        fixture.Value1 = AText;

        await Assert.That(result).IsEqualTo(AText);

        fixture.Value1 = BText;

        await Assert.That(result).IsEqualTo(BText);

        fixture.Value1 = null!;

        await Assert.That(result).IsNull();
    }

    /// <summary>WhenAnyValue with one parameter (already nullable) reflects sequential changes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith1ParamertersSequentialCheckNullable()
    {
        var fixture = new WhenAnyTestFixture();

        var result = string.Empty;

        _ = fixture.WhenAnyValue(x => x.Value2).Subscribe(value => result = value);

        await Assert.That(result).IsNull();

        fixture.Value2 = AText;

        await Assert.That(result).IsEqualTo(AText);

        fixture.Value2 = BText;

        await Assert.That(result).IsEqualTo(BText);

        fixture.Value2 = null;

        await Assert.That(result).IsNull();
    }

    /// <summary>WhenAnyValue with two parameters (tuple result).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith2ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2).Select(tuple =>
        {
            var (value1, value2) = tuple;

            return value1 + value2;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneText);
    }

    /// <summary>WhenAnyValue with two parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith2ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            (v1, v2) => (v1, v2)).Select(tuple =>
        {
            var (value1, value2) = tuple;

            return value1 + value2;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneText);
    }

    /// <summary>WhenAnyValue with three parameters (tuple result).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith3ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3).Select(tuple =>
        {
            var (value1, value2, value3) = tuple;

            return value1 + value2 + value3;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeText);
    }

    /// <summary>WhenAnyValue with three parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith3ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            (v1, v2, v3) => (v1, v2, v3)).Select(tuple =>
        {
            var (value1, value2, value3) = tuple;

            return value1 + value2 + value3;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeText);
    }

    /// <summary>WhenAnyValue with four parameters (tuple result).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith4ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4).Select(tuple =>
        {
            var (value1, value2, value3, value4) = tuple;

            return value1 + value2 + value3 + value4;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeText);
    }

    /// <summary>WhenAnyValue with four parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith4ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            (v1, v2, v3, v4) => (v1, v2, v3, v4)).Select(tuple =>
        {
            var (value1, value2, value3, value4) = tuple;

            return value1 + value2 + value3 + value4;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeText);
    }

    /// <summary>WhenAnyValue with five parameters (tuple result).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith5ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5) = tuple;

            return value1 + value2 + value3 + value4 + value5;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeFiveText);
    }

    /// <summary>WhenAnyValue with five parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith5ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            (v1, v2, v3, v4, v5) => (v1, v2, v3, v4, v5)).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5) = tuple;

            return value1 + value2 + value3 + value4 + value5;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeFiveText);
    }

    /// <summary>WhenAnyValue with six parameters (tuple result).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith6ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            x => x.Value6).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5, value6) = tuple;

            return value1 + value2 + value3 + value4 + value5 + value6;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeFiveText);
    }

    /// <summary>WhenAnyValue with six parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith6ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            x => x.Value6,
            (v1, v2, v3, v4, v5, v6) => (v1, v2, v3, v4, v5, v6)).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5, value6) = tuple;

            return value1 + value2 + value3 + value4 + value5 + value6;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeFiveText);
    }

    /// <summary>WhenAnyValue with seven parameters (tuple result).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith7ParamertersReturnsTuple()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            x => x.Value6,
            x => x.Value7).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5, value6, value7) = tuple;

            return value1 + value2 + value3 + value4 + value5 + value6 + value7;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeFiveSevenText);
    }

    /// <summary>WhenAnyValue with seven parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValueWith7ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            x => x.Value6,
            x => x.Value7,
            (v1, v2, v3, v4, v5, v6, v7) => (v1, v2, v3, v4, v5, v6, v7)).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5, value6, value7) = tuple;

            return value1 + value2 + value3 + value4 + value5 + value6 + value7;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeFiveSevenText);
    }

    /// <summary>WhenAnyValue with eight parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Selector arity mirrors the variadic production WhenAny API.")]
    public async Task WhenAnyValueWith8ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            x => x.Value6,
            x => x.Value7,
            x => x.Value8,
            (v1, v2, v3, v4, v5, v6, v7, v8) => (v1, v2, v3, v4, v5, v6, v7, v8)).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5, value6, value7, value8) = tuple;

            return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo(OneThreeFiveSevenText);
    }

    /// <summary>WhenAnyValue with nine parameters (values projector).</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Selector arity mirrors the variadic production WhenAny API.")]
    public async Task WhenAnyValueWith9ParamertersReturnsValues()
    {
        var fixture = new WhenAnyTestFixture();

        string? result = null;

        _ = fixture.WhenAnyValue(
            x => x.Value1,
            x => x.Value2,
            x => x.Value3,
            x => x.Value4,
            x => x.Value5,
            x => x.Value6,
            x => x.Value7,
            x => x.Value8,
            x => x.Value9,
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => (v1, v2, v3, v4, v5, v6, v7, v8, v9)).Select(tuple =>
        {
            var (value1, value2, value3, value4, value5, value6, value7, value8, value9) = tuple;

            return value1 + value2 + value3 + value4 + value5 + value6 + value7 + value8 + value9;
        }).Subscribe(value => result = value);

        await Assert.That(result).IsEqualTo("13579");
    }
}
