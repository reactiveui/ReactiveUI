// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="VisibilityToBooleanTypeConverter"/> which converts
/// Visibility enum values to boolean values.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class VisibilityToBooleanTypeConverterTests
{
    /// <summary>
    /// Tests that GetAffinityForObjects returns correct affinity.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_ReturnsCorrectAffinity()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var affinity = converter.GetAffinityForObjects();

        await Assert.That(affinity).IsEqualTo(2);
    }

    /// <summary>
    /// Tests that TryConvert converts Visibility.Visible to true.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsVisibleToTrue()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var success = converter.TryConvert(Visibility.Visible, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Tests that TryConvert converts Visibility.Collapsed to false.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsCollapsedToFalse()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var success = converter.TryConvert(Visibility.Collapsed, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Tests that TryConvert converts Visibility.Hidden to false.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsHiddenToFalse()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var success = converter.TryConvert(Visibility.Hidden, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Tests that TryConvert with Inverse hint inverts the result (Visible becomes false).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithInverseHint_ConvertsVisibleToFalse()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var success = converter.TryConvert(Visibility.Visible, BooleanToVisibilityHint.Inverse, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Tests that TryConvert with Inverse hint inverts the result (Collapsed becomes true).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithInverseHint_ConvertsCollapsedToTrue()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var success = converter.TryConvert(Visibility.Collapsed, BooleanToVisibilityHint.Inverse, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Tests that TryConvert with Inverse hint inverts the result (Hidden becomes true).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithInverseHint_ConvertsHiddenToTrue()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var success = converter.TryConvert(Visibility.Hidden, BooleanToVisibilityHint.Inverse, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Tests that TryConvert with None hint (default) works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithNoneHint_WorksAsDefault()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var successVisible = converter.TryConvert(Visibility.Visible, BooleanToVisibilityHint.None, out var visibleResult);
        var successCollapsed = converter.TryConvert(Visibility.Collapsed, BooleanToVisibilityHint.None, out var collapsedResult);

        await Assert.That(successVisible).IsTrue();
        await Assert.That(visibleResult).IsTrue();
        await Assert.That(successCollapsed).IsTrue();
        await Assert.That(collapsedResult).IsFalse();
    }

    /// <summary>
    /// Tests that TryConvert always returns true (successful conversion).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_AlwaysReturnsTrue()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var success1 = converter.TryConvert(Visibility.Visible, null, out _);
        var success2 = converter.TryConvert(Visibility.Collapsed, null, out _);
        var success3 = converter.TryConvert(Visibility.Hidden, null, out _);

        await Assert.That(success1).IsTrue();
        await Assert.That(success2).IsTrue();
        await Assert.That(success3).IsTrue();
    }

    /// <summary>
    /// Tests that TryConvert with non-BooleanToVisibilityHint conversion hint uses default behavior.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithNonBooleanToVisibilityHint_UsesDefaultBehavior()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        // Pass a different type as conversion hint
        var success = converter.TryConvert(Visibility.Visible, "some string", out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Tests that TryConvert treats only Visible as true, all others as false.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_TreatsOnlyVisibleAsTrue()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var visibleSuccess = converter.TryConvert(Visibility.Visible, null, out var visibleResult);
        var collapsedSuccess = converter.TryConvert(Visibility.Collapsed, null, out var collapsedResult);
        var hiddenSuccess = converter.TryConvert(Visibility.Hidden, null, out var hiddenResult);

        await Assert.That(visibleSuccess).IsTrue();
        await Assert.That(visibleResult).IsTrue();

        await Assert.That(collapsedSuccess).IsTrue();
        await Assert.That(collapsedResult).IsFalse();

        await Assert.That(hiddenSuccess).IsTrue();
        await Assert.That(hiddenResult).IsFalse();
    }
}
