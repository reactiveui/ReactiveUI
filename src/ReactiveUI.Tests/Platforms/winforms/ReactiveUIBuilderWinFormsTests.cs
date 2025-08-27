// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Tests.Platforms.Winforms;

/// <summary>
/// Tests for WinForms-specific ReactiveUIBuilder functionality.
/// </summary>
public class ReactiveUIBuilderWinFormsTests : AppBuilderTestBase
{
    /// <summary>
    /// Test that WinForms services can be registered using the builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task WithWinForms_Should_Register_WinForms_Services() =>
        await RunAppBuilderTestAsync(() =>
        {
            // Arrange
            using var locator = new ModernDependencyResolver();
            var builder = locator.CreateReactiveUIBuilder();

            // Act
            builder.WithWinForms().Build();

            // Assert
            var platformOperations = locator.GetService<IPlatformOperations>();
            Assert.NotNull(platformOperations);

            var activationFetcher = locator.GetService<IActivationForViewFetcher>();
            Assert.NotNull(activationFetcher);
        });

    /// <summary>
    /// Test that the builder can chain WinForms registration with core services.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task WithCoreServices_AndWinForms_Should_Register_All_Services() =>
        await RunAppBuilderTestAsync(() =>
        {
            // Arrange
            using var locator = new ModernDependencyResolver();
            var builder = locator.CreateReactiveUIBuilder();

            // Act
            builder.WithWinForms().Build();

            // Assert
            // Core services
            var observableProperty = locator.GetService<ICreatesObservableForProperty>();
            Assert.NotNull(observableProperty);

            // WinForms-specific services
            var platformOperations = locator.GetService<IPlatformOperations>();
            Assert.NotNull(platformOperations);
        });
}
