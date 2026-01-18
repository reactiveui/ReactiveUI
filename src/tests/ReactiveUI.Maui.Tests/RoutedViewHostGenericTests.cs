// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for the generic <see cref="RoutedViewHost{TViewModel}"/>.
/// </summary>
[NotInParallel]
[TestExecutor<MauiTestExecutor>]
public class RoutedViewHostGenericTests
{
    /// <summary>
    /// Tests that RouterProperty is registered for the generic type.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RouterProperty_IsRegistered()
    {
        await Assert.That(RoutedViewHost<TestRoutableViewModel>.RouterProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that SetTitleOnNavigateProperty is registered for the generic type.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetTitleOnNavigateProperty_IsRegistered()
    {
        await Assert.That(RoutedViewHost<TestRoutableViewModel>.SetTitleOnNavigateProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that the generic RoutedViewHost type can be referenced and used in type constraints.
    /// Instance creation requires IScreen registration and MAUI infrastructure.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GenericType_CanBeReferenced()
    {
        // Verify the generic type is properly defined
        var type = typeof(RoutedViewHost<TestRoutableViewModel>);

        await Assert.That(type).IsNotNull();
        await Assert.That(type.IsGenericType).IsTrue();
    }

    /// <summary>
    /// Test routable view model.
    /// </summary>
    private sealed class TestRoutableViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment { get; set; } = "test";

        public IScreen HostScreen { get; } = null!;
    }
}
