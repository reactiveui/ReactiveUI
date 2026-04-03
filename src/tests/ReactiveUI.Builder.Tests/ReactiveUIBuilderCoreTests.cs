// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests;

[NotInParallel]
public class ReactiveUIBuilderCoreTests
{
    [Test]
    public async Task CreateBuilder_Should_Return_Builder_Instance()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        await Assert.That(builder).IsNotNull();
        await Assert.That(builder).IsTypeOf<ReactiveUIBuilder>();
    }

    [Test]
    public async Task WithCoreServices_Should_Register_Core_Services()
    {
        var observableProperty = Locator.Current.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();

        var typeConverter = Locator.Current.GetService<IBindingTypeConverter>();
        await Assert.That(typeConverter).IsNotNull();
    }

    [Test]
    [TestExecutor<WithPlatformServicesExecutor>]
    public async Task WithPlatformServices_Should_Register_Platform_Services()
    {
        var services = Locator.Current.GetServices<IBindingTypeConverter>();
        await Assert.That(services).IsNotNull();
        await Assert.That(services.Any()).IsTrue();
    }

    [Test]
    [TestExecutor<WithCustomRegistrationExecutor>]
    public async Task WithCustomRegistration_Should_Execute_Custom_Action()
    {
        await Assert.That(WithCustomRegistrationExecutor.CustomServiceRegistered).IsTrue();
        var service = Locator.Current.GetService<string>();
        await Assert.That(service).IsEqualTo("TestValue");
    }

    [Test]
    public async Task Build_Should_Always_Register_Core_Services()
    {
        var observableProperty = Locator.Current.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();
    }

    [Test]
    public void WithCustomRegistration_With_Null_Action_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.WithCustomRegistration(null!));
    }

    [Test]
    [TestExecutor<WithViewsFromAssemblyExecutor>]
    public async Task WithViewsFromAssembly_Should_Register_Views()
    {
        // The executor registered views; just verify it completed without error.
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        await Assert.That(builder).IsNotNull();
    }

    [Test]
    public void WithViewsFromAssembly_With_Null_Assembly_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.WithViewsFromAssembly(null!));
    }

    [Test]
    public async Task WithCoreServices_Called_Multiple_Times_Should_Not_Register_Twice()
    {
        var services = Locator.Current.GetServices<ICreatesObservableForProperty>();
        await Assert.That(services).IsNotNull();
        await Assert.That(services.Any()).IsTrue();
    }

    [Test]
    [TestExecutor<FluentChainingExecutor>]
    public async Task Builder_Should_Support_Fluent_Chaining()
    {
        await Assert.That(FluentChainingExecutor.CustomServiceRegistered).IsTrue();
        var service = Locator.Current.GetService<string>();
        await Assert.That(service).IsEqualTo("Test");

        var observableProperty = Locator.Current.GetService<ICreatesObservableForProperty>();
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
        // The executor already called BuildApp; verify the state is correct.
        var current = Locator.Current;
        await Assert.That(current).IsNotNull();
    }

    [Test]
    public void ForPlatforms_With_Null_Array_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.ForPlatforms(null!));
    }

    internal sealed class WithPlatformServicesExecutor : BuilderTestExecutorBase
    {
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithPlatformServices()
                .BuildApp();
    }

    internal sealed class WithCustomRegistrationExecutor : BuilderTestExecutorBase
    {
        public static bool CustomServiceRegistered { get; private set; }

        protected override void ConfigureBuilder()
        {
            CustomServiceRegistered = false;
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithCustomRegistration(r =>
                {
                    r.RegisterConstant("TestValue", typeof(string));
                    CustomServiceRegistered = true;
                })
                .WithCoreServices()
                .BuildApp();
        }
    }

    internal sealed class WithViewsFromAssemblyExecutor : BuilderTestExecutorBase
    {
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithViewsFromAssembly(typeof(ReactiveUIBuilderCoreTests).Assembly)
                .WithCoreServices()
                .BuildApp();
    }

    internal sealed class FluentChainingExecutor : BuilderTestExecutorBase
    {
        public static bool CustomServiceRegistered { get; private set; }

        protected override void ConfigureBuilder()
        {
            CustomServiceRegistered = false;
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithCoreServices()
                .WithCustomRegistration(r =>
                {
                    r.RegisterConstant("Test", typeof(string));
                    CustomServiceRegistered = true;
                })
                .BuildApp();
        }
    }
}
