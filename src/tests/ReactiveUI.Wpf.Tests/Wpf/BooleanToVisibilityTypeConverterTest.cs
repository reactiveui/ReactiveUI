// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="BooleanToVisibilityTypeConverter"/> in WPF.
/// </summary>
[NotInParallel]
public class BooleanToVisibilityTypeConverterTest
{
    private WpfAppBuilderScope? _appBuilderScope;

    /// <summary>
    /// Sets up the WPF app builder scope for each test.
    /// </summary>
    [Before(Test)]
    public void Setup()
    {
        _appBuilderScope = new WpfAppBuilderScope();
    }

    /// <summary>
    /// Tears down the WPF app builder scope after each test.
    /// </summary>
    [After(Test)]
    public void TearDown()
    {
        _appBuilderScope?.Dispose();
    }

    /// <summary>
    /// Tests that FromType and ToType properties are correctly set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TypeProperties_AreCorrectlySet()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        await Assert.That(converter.FromType).IsEqualTo(typeof(bool));
        await Assert.That(converter.ToType).IsEqualTo(typeof(Visibility));
    }

    /// <summary>
    /// Tests that GetAffinityForObjects returns correct affinity.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_ReturnsCorrectAffinity()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var affinity = converter.GetAffinityForObjects();

        await Assert.That(affinity).IsEqualTo(2);
    }

    /// <summary>
    /// Tests that TryConvert converts true to Visibility.Visible.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsTrueToVisible()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(true, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }

    /// <summary>
    /// Tests that TryConvert converts false to Visibility.Collapsed (default).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsFalseToCollapsed()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(false, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Collapsed);
    }

    /// <summary>
    /// Tests that TryConvert with Inverse hint inverts the conversion.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithInverseHint_InvertsConversion()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(true, BooleanToVisibilityHint.Inverse, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Collapsed);
    }

    /// <summary>
    /// Tests that TryConvert with UseHidden hint uses Hidden instead of Collapsed (WPF-specific).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithUseHiddenHint_UsesHidden()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(false, BooleanToVisibilityHint.UseHidden, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Hidden);
    }

    /// <summary>
    /// Tests that TryConvert with both Inverse and UseHidden hints works correctly (WPF-specific).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithInverseAndUseHidden_UsesHidden()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(
            true,
            BooleanToVisibilityHint.Inverse | BooleanToVisibilityHint.UseHidden,
            out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Hidden);
    }

    /// <summary>
    /// Tests that TryConvert with UseHidden hint on true stays Visible.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_TrueWithUseHidden_StaysVisible()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(true, BooleanToVisibilityHint.UseHidden, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }
}
