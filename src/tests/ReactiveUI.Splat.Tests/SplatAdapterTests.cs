// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;

using Autofac;

using DryIoc;

using Ninject;

using Splat.Autofac;
using Splat.DryIoc;
using Splat.Ninject;

namespace ReactiveUI.Splat.Tests;

/// <summary>
/// Tests for checking the splat adapters register ReactiveUI services.
/// </summary>
[NotInParallel] // These tests modify global state (Locator.CurrentMutable)
public class SplatAdapterTests
{
    /// <summary>
    /// Should register ReactiveUI binding type converters.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DryIocDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new Container();
        container.UseDryIocDependencyResolver();
        Locator.CurrentMutable.InitializeReactiveUI();

        var converters = container.Resolve<IEnumerable<IBindingTypeConverter>>().ToList();

        await Assert.That(converters).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(converters.Any(static x => x.GetType() == typeof(StringConverter))).IsTrue();
            await Assert.That(converters.Any(static x => x.GetType() == typeof(EqualityTypeConverter))).IsTrue();
        }
    }

    /// <summary>
    /// Should register ReactiveUI creates command bindings.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DryIocDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new Container();
        container.UseDryIocDependencyResolver();
        Locator.CurrentMutable.InitializeReactiveUI();

        var converters = container.Resolve<IEnumerable<ICreatesCommandBinding>>().ToList();

        await Assert.That(converters).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaEvent))).IsTrue();
            await Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter))).IsTrue();
        }
    }

    /// <summary>
    /// Should register ReactiveUI binding type converters.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutofacDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var builder = new ContainerBuilder();
        var locator = new AutofacDependencyResolver(builder);
        locator.InitializeReactiveUI();
        var container = builder.Build();

        var converters = container.Resolve<IEnumerable<IBindingTypeConverter>>().ToList();

        await Assert.That(converters).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(converters.Any(static x => x.GetType() == typeof(StringConverter))).IsTrue();
            await Assert.That(converters.Any(static x => x.GetType() == typeof(EqualityTypeConverter))).IsTrue();
        }
    }

    /// <summary>
    /// Should register ReactiveUI creates command bindings.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutofacDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var builder = new ContainerBuilder();
        var locator = new AutofacDependencyResolver(builder);
        locator.InitializeReactiveUI();
        Locator.SetLocator(locator);
        var container = builder.Build();

        var converters = container.Resolve<IEnumerable<ICreatesCommandBinding>>().ToList();

        await Assert.That(converters).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaEvent))).IsTrue();
            await Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter))).IsTrue();
        }
    }

    /// <summary>
    /// Should register ReactiveUI binding type converters.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NinjectDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new StandardKernel();
        container.UseNinjectDependencyResolver();
        Locator.CurrentMutable.InitializeReactiveUI();

        var converters = container.GetAll<IBindingTypeConverter>().ToList();

        await Assert.That(converters).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(converters.Any(static x => x.GetType() == typeof(StringConverter))).IsTrue();
            await Assert.That(converters.Any(static x => x.GetType() == typeof(EqualityTypeConverter))).IsTrue();
        }
    }

    /// <summary>
    /// Should register ReactiveUI creates command bindings.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NinjectDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new StandardKernel();
        container.UseNinjectDependencyResolver();
        Locator.CurrentMutable.InitializeReactiveUI();

        var converters = container.GetAll<ICreatesCommandBinding>().ToList();

        await Assert.That(converters).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaEvent))).IsTrue();
            await Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter))).IsTrue();
        }
    }
}
