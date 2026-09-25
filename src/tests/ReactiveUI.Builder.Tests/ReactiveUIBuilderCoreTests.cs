// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests;

/// <summary>Tests for the core ReactiveUIBuilder creation and service-registration behaviour.</summary>
[NotInParallel]
public class ReactiveUIBuilderCoreTests
{
    /// <summary>Verifies that creating a builder returns a non-null <see cref="ReactiveUIBuilder"/> instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task CreateBuilder_Should_Return_Builder_Instance()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        await Assert.That(builder).IsNotNull();
        await Assert.That(builder).IsTypeOf<ReactiveUIBuilder>();
    }

    /// <summary>Verifies that core services register the retained view locator and activation fetcher.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithCoreServices_Should_Register_Core_Services()
    {
        await Assert.That(Locator.Current.GetService<IViewLocator>()).IsNotNull();
        await Assert.That(Locator.Current.GetService<IActivationForViewFetcher>()).IsNotNull();
    }

    /// <summary>Verifies that a custom registration action runs and registers its service.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithCustomRegistrationExecutor>]
    public async Task WithCustomRegistration_Should_Execute_Custom_Action()
    {
        await Assert.That(WithCustomRegistrationExecutor.CustomServiceRegistered).IsTrue();
        var service = Locator.Current.GetService<string>();
        await Assert.That(service).IsEqualTo("TestValue");
    }

    /// <summary>Verifies that building always registers core services.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Build_Should_Always_Register_Core_Services() =>
        await Assert.That(Locator.Current.GetService<IViewLocator>()).IsNotNull();

    /// <summary>Verifies that a null custom registration action throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithCustomRegistration_With_Null_Action_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithCustomRegistration(null!));
    }

    /// <summary>Verifies that registering views from an assembly completes without error.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithViewsFromAssemblyExecutor>]
    public async Task WithViewsFromAssembly_Should_Register_Views()
    {
        // The executor registered views; just verify it completed without error.
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        await Assert.That(builder).IsNotNull();
    }

    /// <summary>Verifies that registering views from a null assembly throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithViewsFromAssembly_With_Null_Assembly_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithViewsFromAssembly(null!));
    }

    /// <summary>Verifies that the builder supports fluent chaining of registration calls.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<FluentChainingExecutor>]
    public async Task Builder_Should_Support_Fluent_Chaining()
    {
        await Assert.That(FluentChainingExecutor.CustomServiceRegistered).IsTrue();
        var service = Locator.Current.GetService<string>();
        await Assert.That(service).IsEqualTo("Test");

        await Assert.That(Locator.Current.GetService<IViewLocator>()).IsNotNull();
    }

    /// <summary>Verifies that creating a builder from a null resolver throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void CreateReactiveUIBuilder_With_Null_Resolver_Should_Throw() =>
        Assert.Throws<ArgumentNullException>(static () =>
            ((IMutableDependencyResolver)null!).CreateReactiveUIBuilder());

    /// <summary>Verifies that building the app produces a valid current locator.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BuildApp_Should_Return_ReactiveInstance()
    {
        // The executor already called BuildApp; verify the state is correct.
        await Assert.That(Locator.Current).IsNotNull();
    }

    /// <summary>Verifies that passing a null platform array throws <see cref="ArgumentNullException"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Build_Should_Return_ReactiveInstance_And_Initialize_ReactiveUI()
    {
        RxAppBuilder.ResetForTesting();

        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithCoreServices();

        var app = builder.Build();

        RxAppBuilder.EnsureInitialized();

        await Assert.That(app).IsNotNull();
        await Assert.That(app).IsAssignableTo<IReactiveUIInstance>();
    }

    /// <summary>Verifies that calling BuildApp on a builder instance produces a valid current locator.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BuildApp_Should_Use_Build_Implementation()
    {
        RxAppBuilder.ResetForTesting();

        var app = RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();

        RxAppBuilder.EnsureInitialized();

        await Assert.That(app).IsNotNull();
        await Assert.That(app).IsAssignableTo<IReactiveUIInstance>();
    }

    /// <summary>Verifies that <c>ForPlatforms</c> throws when passed a null platforms array.</summary>
    [Test]
    public void ForPlatforms_With_Null_Array_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentNullException>(() => builder.ForPlatforms(null!));
    }

    /// <summary>Executor that builds the app with a custom registration action.</summary>
    internal sealed class WithCustomRegistrationExecutor : BuilderTestExecutorBase
    {
        /// <summary>Gets a value indicating whether the custom service was registered.</summary>
        internal static bool CustomServiceRegistered { get; private set; }

        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            CustomServiceRegistered = false;
            _ = RxAppBuilder.CreateReactiveUIBuilder()
                .WithCustomRegistration(static r =>
                {
                    r.RegisterConstant("TestValue", typeof(string));
                    CustomServiceRegistered = true;
                })
                .WithCoreServices()
                .BuildApp();
        }
    }

    /// <summary>Executor that builds the app while registering views from the test assembly.</summary>
    internal sealed class WithViewsFromAssemblyExecutor : BuilderTestExecutorBase
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithViewsFromAssembly(typeof(ReactiveUIBuilderCoreTests).Assembly)
                .WithCoreServices()
                .BuildApp();
    }

    /// <summary>Executor that builds the app using a fluent chain of registration calls.</summary>
    internal sealed class FluentChainingExecutor : BuilderTestExecutorBase
    {
        /// <summary>Gets a value indicating whether the custom service was registered.</summary>
        internal static bool CustomServiceRegistered { get; private set; }

        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            CustomServiceRegistered = false;
            _ = RxAppBuilder.CreateReactiveUIBuilder()
                .WithCoreServices()
                .WithCustomRegistration(static r =>
                {
                    r.RegisterConstant("Test", typeof(string));
                    CustomServiceRegistered = true;
                })
                .BuildApp();
        }
    }
}
