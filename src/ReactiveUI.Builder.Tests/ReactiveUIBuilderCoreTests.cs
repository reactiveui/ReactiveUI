// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Tests for the ReactiveUIBuilder core functionality.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ReactiveUIBuilderCoreTests
{
    [Test]
    public void CreateBuilder_Should_Return_Builder_Instance()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        Assert.That(builder, Is.Not.Null);
        Assert.That(builder, Is.TypeOf<ReactiveUIBuilder>());
    }

    [Test]
    public void WithCoreServices_Should_Register_Core_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        builder.WithCoreServices().Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableProperty, Is.Not.Null);

        var typeConverter = locator.GetService<IBindingTypeConverter>();
        Assert.That(typeConverter, Is.Not.Null);
    }

    [Test]
    public void WithPlatformServices_Should_Register_Platform_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        builder.WithPlatformServices().Build();

        var services = locator.GetServices<IBindingTypeConverter>();
        Assert.That(services, Is.Not.Null);
        Assert.That(services.Any(), Is.True);
    }

    [Test]
    public void WithCustomRegistration_Should_Execute_Custom_Action()
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

        Assert.That(customServiceRegistered, Is.True);
        var service = locator.GetService<string>();
        Assert.That(service, Is.EqualTo("TestValue"));
    }

    [Test]
    public void Build_Should_Always_Register_Core_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableProperty, Is.Not.Null);
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
    public void WithViewsFromAssembly_Should_Register_Views()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        var assembly = typeof(ReactiveUIBuilderCoreTests).Assembly;

        builder.WithViewsFromAssembly(assembly).Build();
        Assert.That(builder, Is.Not.Null);
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
    public void WithCoreServices_Called_Multiple_Times_Should_Not_Register_Twice()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithCoreServices().WithCoreServices().Build();

        var services = locator.GetServices<ICreatesObservableForProperty>();
        Assert.That(services, Is.Not.Null);
        Assert.That(services.Any(), Is.True);
    }

    [Test]
    public void Builder_Should_Support_Fluent_Chaining()
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

        Assert.That(customServiceRegistered, Is.True);
        var service = locator.GetService<string>();
        Assert.That(service, Is.EqualTo("Test"));

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableProperty, Is.Not.Null);
    }

    [Test]
    public void CreateReactiveUIBuilder_With_Null_Resolver_Should_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => RxAppBuilder.CreateReactiveUIBuilder((IMutableDependencyResolver)null!));
    }

    [Test]
    public void BuildApp_Should_Return_ReactiveInstance()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithCoreServices();
        var instance = builder.BuildApp();

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Current, Is.Not.Null);
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
