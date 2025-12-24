// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

[NotInParallel]
public class ReactiveUIBuilderCoreTests
{
    [Test]
    public async Task CreateBuilder_Should_Return_Builder_Instance()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        await Assert.That(builder).IsNotNull();
        await Assert.That(builder).IsTypeOf<ReactiveUIBuilder>();
    }

    [Test]
    public async Task WithCoreServices_Should_Register_Core_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        builder.WithCoreServices().Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();

        var typeConverter = locator.GetService<IBindingTypeConverter>();
        await Assert.That(typeConverter).IsNotNull();
    }

    [Test]
    public async Task WithPlatformServices_Should_Register_Platform_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        builder.WithPlatformServices().Build();

        var services = locator.GetServices<IBindingTypeConverter>();
        await Assert.That(services).IsNotNull();
        await Assert.That(services.Any()).IsTrue();
    }

    [Test]
    public async Task WithCustomRegistration_Should_Execute_Custom_Action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        var customServiceRegistered = false;

        builder.WithCustomRegistration(r =>
        {
            r.RegisterConstant("TestValue", typeof(string));
            customServiceRegistered = true;
        }).Build();

        await Assert.That(customServiceRegistered).IsTrue();
        var service = locator.GetService<string>();
        await Assert.That(service).IsEqualTo("TestValue");
    }

    [Test]
    public async Task Build_Should_Always_Register_Core_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();
    }

    [Test]
    public void WithCustomRegistration_With_Null_Action_Should_Throw()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.WithCustomRegistration(null!));
    }

    [Test]
    public async Task WithViewsFromAssembly_Should_Register_Views()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        var assembly = typeof(ReactiveUIBuilderCoreTests).Assembly;

        builder.WithViewsFromAssembly(assembly).Build();
        await Assert.That(builder).IsNotNull();
    }

    [Test]
    public void WithViewsFromAssembly_With_Null_Assembly_Should_Throw()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.WithViewsFromAssembly(null!));
    }

    [Test]
    public async Task WithCoreServices_Called_Multiple_Times_Should_Not_Register_Twice()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithCoreServices().WithCoreServices().Build();

        var services = locator.GetServices<ICreatesObservableForProperty>();
        await Assert.That(services).IsNotNull();
        await Assert.That(services.Any()).IsTrue();
    }

    [Test]
    public async Task Builder_Should_Support_Fluent_Chaining()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var customServiceRegistered = false;

        locator.CreateReactiveUIBuilder()
               .WithCoreServices()
               .WithCustomRegistration(r =>
               {
                   r.RegisterConstant("Test", typeof(string));
                   customServiceRegistered = true;
               })
               .Build();

        await Assert.That(customServiceRegistered).IsTrue();
        var service = locator.GetService<string>();
        await Assert.That(service).IsEqualTo("Test");

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();
    }

    [Test]
    public void CreateReactiveUIBuilder_With_Null_Resolver_Should_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => RxAppBuilder.CreateReactiveUIBuilder((IMutableDependencyResolver)null!));
    }

    [Test]
    public async Task BuildApp_Should_Return_ReactiveInstance()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithCoreServices();
        var instance = builder.BuildApp();

        await Assert.That(instance).IsNotNull();
        await Assert.That(instance.Current).IsNotNull();
    }

    [Test]
    public void ForPlatforms_With_Null_Array_Should_Throw()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.ForPlatforms(null!));
    }
}
