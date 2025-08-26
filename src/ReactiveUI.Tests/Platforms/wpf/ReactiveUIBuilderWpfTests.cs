// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Tests.Platforms.Wpf;

/// <summary>
/// Tests for WPF-specific ReactiveUIBuilder functionality.
/// </summary>
public class ReactiveUIBuilderWpfTests
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUIBuilderWpfTests"/> class.
    /// </summary>
    public ReactiveUIBuilderWpfTests()
    {
        RxApp.EnsureInitialized();
    }

    /// <summary>
    /// Test that WPF services can be registered using the builder.
    /// </summary>
    [Fact]
    public void WithWpf_Should_Register_Wpf_Services()
    {
        Splat.Builder.AppBuilder.ResetBuilderStateForTests();

        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        // Act
        builder.WithWpf().Build();

        // Assert
        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.NotNull(platformOperations);

        var activationFetcher = locator.GetService<IActivationForViewFetcher>();
        Assert.NotNull(activationFetcher);
    }

    /// <summary>
    /// Test that the builder can chain WPF registration with core services.
    /// </summary>
    [Fact]
    public void WithCoreServices_AndWpf_Should_Register_All_Services()
    {
        Splat.Builder.AppBuilder.ResetBuilderStateForTests();

        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        // Act
        builder.WithWpf().Build();

        // Assert
        // Core services
        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);

        // WPF-specific services
        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.NotNull(platformOperations);
    }
}
