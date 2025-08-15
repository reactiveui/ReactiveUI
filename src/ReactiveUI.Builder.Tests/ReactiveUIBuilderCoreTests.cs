// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Tests for the ReactiveUIBuilder core functionality.
/// </summary>
public class ReactiveUIBuilderCoreTests
{
    [Fact]
    public void CreateBuilder_Should_Return_Builder_Instance()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();
        Assert.NotNull(builder);
        Assert.IsType<ReactiveUIBuilder>(builder);
    }

    [Fact]
    public void WithCoreServices_Should_Register_Core_Services()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();
        builder.WithCoreServices().Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);

        var typeConverter = locator.GetService<IBindingTypeConverter>();
        Assert.NotNull(typeConverter);
    }

    [Fact]
    public void WithPlatformServices_Should_Register_Platform_Services()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();
        builder.WithPlatformServices().Build();

        var services = locator.GetServices<IBindingTypeConverter>();
        Assert.NotNull(services);
        Assert.True(services.Any());
    }

    [Fact]
    public void WithCustomRegistration_Should_Execute_Custom_Action()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();
        var customServiceRegistered = false;

        builder.WithCustomRegistration(r =>
        {
            r.RegisterConstant("TestValue", typeof(string));
            customServiceRegistered = true;
        }).Build();

        Assert.True(customServiceRegistered);
        var service = locator.GetService<string>();
        Assert.Equal("TestValue", service);
    }

    [Fact]
    public void Build_Should_Always_Register_Core_Services()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        builder.Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);
    }

    [Fact]
    public void WithCustomRegistration_With_Null_Action_Should_Throw()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.WithCustomRegistration(null!));
    }

    [Fact]
    public void WithViewsFromAssembly_Should_Register_Views()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();
        var assembly = typeof(ReactiveUIBuilderCoreTests).Assembly;

        builder.WithViewsFromAssembly(assembly).Build();
        Assert.NotNull(builder);
    }

    [Fact]
    public void WithViewsFromAssembly_With_Null_Assembly_Should_Throw()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.WithViewsFromAssembly(null!));
    }

    [Fact]
    public void WithCoreServices_Called_Multiple_Times_Should_Not_Register_Twice()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        builder.WithCoreServices().WithCoreServices().Build();

        var services = locator.GetServices<ICreatesObservableForProperty>();
        Assert.NotNull(services);
        Assert.True(services.Any());
    }

    [Fact]
    public void Builder_Should_Support_Fluent_Chaining()
    {
        using var locator = new ModernDependencyResolver();
        var customServiceRegistered = false;

        locator.CreateBuilder()
               .WithCoreServices()
               .WithPlatformServices()
               .WithCustomRegistration(r =>
               {
                   r.RegisterConstant("Test", typeof(string));
                   customServiceRegistered = true;
               })
               .Build();

        Assert.True(customServiceRegistered);
        var service = locator.GetService<string>();
        Assert.Equal("Test", service);

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);
    }
}
