// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Autofac;
using DryIoc;
using Ninject;
using ReactiveUI.Binding.ObservableForProperty;
using ReactiveUI.Builder;
using Splat;
using Splat.Autofac;
using Splat.DryIoc;
using Splat.Ninject;

namespace ReactiveUI.Splat.Tests;

/// <summary>Tests for checking the splat adapters register ReactiveUI services.</summary>
[NotInParallel] // These tests modify global state (Locator.CurrentMutable)
public class SplatAdapterTests
{
    /// <summary>Should expose Binding's converter service.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DryIocDependencyResolver_Should_Register_BindingConverterService()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new Container();
        container.UseDryIocDependencyResolver();
        _ = Locator.CurrentMutable.CreateReactiveUIBuilder()
            .WithCoreServices()
            .Build();

        var converters = container.Resolve<ConverterService>();

        await Assert.That(converters).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(converters.ResolveConverter(typeof(string), typeof(string)) is StringConverter).IsTrue();
            await Assert.That(converters.ResolveConverter(typeof(object), typeof(bool)) is EqualityTypeConverter).IsTrue();
        }
    }

    /// <summary>Should register Binding's runtime property observers.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DryIocDependencyResolver_Should_Register_BindingPropertyObservers()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new Container();
        container.UseDryIocDependencyResolver();
        _ = Locator.CurrentMutable.CreateReactiveUIBuilder()
            .WithCoreServices()
            .Build();

        var observers = container.Resolve<IEnumerable<ICreatesObservableForProperty>>().ToList();

        await Assert.That(observers).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(observers.Exists(static x => x is INPCObservableForProperty))
                .IsTrue();
            await Assert
                .That(observers.Exists(static x => x is POCOObservableForProperty))
                .IsTrue();
        }
    }

    /// <summary>Should expose Binding's converter service.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutofacDependencyResolver_Should_Register_BindingConverterService()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var builder = new ContainerBuilder();
        var locator = new AutofacDependencyResolver(builder);
        _ = locator.CreateReactiveUIBuilder()
            .WithCoreServices()
            .Build();
        var container = builder.Build();

        var converters = container.Resolve<ConverterService>();

        await Assert.That(converters).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(converters.ResolveConverter(typeof(string), typeof(string)) is StringConverter).IsTrue();
            await Assert.That(converters.ResolveConverter(typeof(object), typeof(bool)) is EqualityTypeConverter).IsTrue();
        }
    }

    /// <summary>Should register Binding's runtime property observers.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutofacDependencyResolver_Should_Register_BindingPropertyObservers()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var builder = new ContainerBuilder();
        var locator = new AutofacDependencyResolver(builder);
        _ = locator.CreateReactiveUIBuilder()
            .WithCoreServices()
            .Build();
        Locator.SetLocator(locator);
        var container = builder.Build();

        var observers = container.Resolve<IEnumerable<ICreatesObservableForProperty>>().ToList();

        await Assert.That(observers).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(observers.Exists(static x => x is INPCObservableForProperty))
                .IsTrue();
            await Assert
                .That(observers.Exists(static x => x is POCOObservableForProperty))
                .IsTrue();
        }
    }

    /// <summary>Should expose Binding's converter service.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NinjectDependencyResolver_Should_Register_BindingConverterService()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new StandardKernel();
        container.UseNinjectDependencyResolver();
        _ = Locator.CurrentMutable.CreateReactiveUIBuilder()
            .WithCoreServices()
            .Build();

        var converters = container.Get<ConverterService>();

        await Assert.That(converters).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(converters.ResolveConverter(typeof(string), typeof(string)) is StringConverter).IsTrue();
            await Assert.That(converters.ResolveConverter(typeof(object), typeof(bool)) is EqualityTypeConverter).IsTrue();
        }
    }

    /// <summary>Should register Binding's runtime property observers.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NinjectDependencyResolver_Should_Register_BindingPropertyObservers()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new StandardKernel();
        container.UseNinjectDependencyResolver();
        _ = Locator.CurrentMutable.CreateReactiveUIBuilder()
            .WithCoreServices()
            .Build();

        var observers = container.GetAll<ICreatesObservableForProperty>().ToList();

        await Assert.That(observers).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(observers.Exists(static x => x is INPCObservableForProperty))
                .IsTrue();
            await Assert
                .That(observers.Exists(static x => x is POCOObservableForProperty))
                .IsTrue();
        }
    }
}
