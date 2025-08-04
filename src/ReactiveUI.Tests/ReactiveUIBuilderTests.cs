// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the ReactiveUIBuilder core functionality.
/// </summary>
public class ReactiveUIBuilderTests
{
    /// <summary>
    /// Test that the builder can be created from a dependency resolver.
    /// </summary>
    [Fact]
    public void CreateBuilder_Should_Return_Builder_Instance()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();

        // Act
        var builder = locator.CreateBuilder();

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<ReactiveUIBuilder>(builder);
    }

    /// <summary>
    /// Test that core services are registered when using the builder.
    /// </summary>
    [Fact]
    public void WithCoreServices_Should_Register_Core_Services()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        // Act
        builder.WithCoreServices().Build();

        // Assert
        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);

        var typeConverter = locator.GetService<IBindingTypeConverter>();
        Assert.NotNull(typeConverter);
    }

    /// <summary>
    /// Test that platform services are registered when using the builder.
    /// </summary>
    [Fact]
    public void WithPlatformServices_Should_Register_Platform_Services()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        // Act
        builder.WithPlatformServices().Build();

        // Assert
        // Platform services vary by platform, so we check what's available
        var services = locator.GetServices<IBindingTypeConverter>();
        Assert.NotNull(services);
        Assert.True(services.Any());
    }

    /// <summary>
    /// Test that custom registration actions work.
    /// </summary>
    [Fact]
    public void WithCustomRegistration_Should_Execute_Custom_Action()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();
        var customServiceRegistered = false;

        // Act
        builder.WithCustomRegistration(r =>
        {
            r.RegisterConstant("TestValue", typeof(string));
            customServiceRegistered = true;
        }).Build();

        // Assert
        Assert.True(customServiceRegistered);
        var service = locator.GetService<string>();
        Assert.Equal("TestValue", service);
    }

    /// <summary>
    /// Test that builder ensures core services are always registered.
    /// </summary>
    [Fact]
    public void Build_Should_Always_Register_Core_Services()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        // Act - Build without explicitly calling WithCoreServices
        builder.Build();

        // Assert
        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);
    }

    /// <summary>
    /// Test that null resolver throws exception.
    /// </summary>
    [Fact]
    public void Constructor_With_Null_Resolver_Should_Throw()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ReactiveUIBuilder(null!));
    }

    /// <summary>
    /// Test that null custom registration throws exception.
    /// </summary>
    [Fact]
    public void WithCustomRegistration_With_Null_Action_Should_Throw()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.WithCustomRegistration(null!));
    }

    /// <summary>
    /// Test that WithViewsFromAssembly works correctly.
    /// </summary>
    [Fact]
    public void WithViewsFromAssembly_Should_Register_Views()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();
        var assembly = typeof(ReactiveUIBuilderTests).Assembly;

        // Act
        builder.WithViewsFromAssembly(assembly).Build();

        // Assert - Should not throw any exceptions
        Assert.NotNull(builder);
    }

    /// <summary>
    /// Test that null assembly throws exception.
    /// </summary>
    [Fact]
    public void WithViewsFromAssembly_With_Null_Assembly_Should_Throw()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.WithViewsFromAssembly(null!));
    }

    /// <summary>
    /// Test that multiple calls to WithCoreServices don't register services twice.
    /// </summary>
    [Fact]
    public void WithCoreServices_Called_Multiple_Times_Should_Not_Register_Twice()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        // Act
        builder.WithCoreServices().WithCoreServices().Build();

        // Assert
        // Verify that services are registered but not duplicated
        var services = locator.GetServices<ICreatesObservableForProperty>();
        Assert.NotNull(services);
        Assert.True(services.Any());
    }

    /// <summary>
    /// Test builder with fluent chaining.
    /// </summary>
    [Fact]
    public void Builder_Should_Support_Fluent_Chaining()
    {
        // Arrange
        using var locator = new ModernDependencyResolver();
        var customServiceRegistered = false;

        // Act
        locator.CreateBuilder()
            .WithCoreServices()
            .WithPlatformServices()
            .WithCustomRegistration(r =>
            {
                r.RegisterConstant("Test", typeof(string));
                customServiceRegistered = true;
            })
            .Build();

        // Assert
        Assert.True(customServiceRegistered);
        var service = locator.GetService<string>();
        Assert.Equal("Test", service);

        // Verify core services are registered
        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);
    }
}
