// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using ReactiveUI.Wpf;
using Splat.Builder;

namespace ReactiveUI.Tests.Platforms.Wpf;

/// <summary>
/// Tests for WPF-specific ReactiveUIBuilder functionality.
/// </summary>
public class ReactiveUIBuilderWpfTests
{
    /// <summary>
    /// Test that WPF services can be registered using the builder.
    /// </summary>
    [Fact]
    public void WithWpf_Should_Register_Wpf_Services()
    {
        ResetAppBuilderState();

        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

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
        ResetAppBuilderState();

        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        // Act
        builder.WithCoreServices().WithWpf().Build();

        // Assert
        // Core services
        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);

        // WPF-specific services
        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.NotNull(platformOperations);
    }

    private static void ResetAppBuilderState()
    {
        // Reset the static state of the AppBuilder.HasBeenBuilt property
        // This is necessary to ensure that tests can run independently
        var prop = typeof(AppBuilder).GetProperty("HasBeenBuilt", BindingFlags.Static | BindingFlags.Public);

        // Get the non-public setter method
        var setter = prop?.GetSetMethod(true); // 'true' includes non-public methods

        // Invoke the setter to set the value to false
        setter?.Invoke(null, new object[] { false });
    }
}
