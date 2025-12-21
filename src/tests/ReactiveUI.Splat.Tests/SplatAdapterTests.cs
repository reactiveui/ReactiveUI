// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Autofac;

using DryIoc;

using Ninject;

using Splat.Autofac;
using Splat.DryIoc;
using Splat.Ninject;

namespace ReactiveUI.Splat.Tests;

/// <summary>
/// Tests for checking the various adapters in splat.
/// </summary>
[TestFixture]
public class SplatAdapterTests
{
    /// <summary>
    /// Should register ReactiveUI binding type converters.
    /// </summary>
    [Test]
    public void DryIocDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new DryIoc.Container();
        container.UseDryIocDependencyResolver();
        Locator.CurrentMutable.InitializeReactiveUI();

        var converters = container.Resolve<IEnumerable<IBindingTypeConverter>>().ToList();

        Assert.That(converters, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(converters.Any(static x => x.GetType() == typeof(StringConverter)), Is.True);
            Assert.That(converters.Any(static x => x.GetType() == typeof(EqualityTypeConverter)), Is.True);
        }
    }

    /// <summary>
    /// Should register ReactiveUI creates command bindings.
    /// </summary>
    [Test]
    public void DryIocDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new DryIoc.Container();
        container.UseDryIocDependencyResolver();
        Locator.CurrentMutable.InitializeReactiveUI();

        var converters = container.Resolve<IEnumerable<ICreatesCommandBinding>>().ToList();

        Assert.That(converters, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaEvent)), Is.True);
            Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter)), Is.True);
        }
    }

    /// <summary>
    /// Should register ReactiveUI binding type converters.
    /// </summary>
    [Test]
    public void AutofacDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var builder = new ContainerBuilder();
        var locator = new AutofacDependencyResolver(builder);
        locator.InitializeReactiveUI();
        var container = builder.Build();

        var converters = container.Resolve<IEnumerable<IBindingTypeConverter>>().ToList();

        Assert.That(converters, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(converters.Any(static x => x.GetType() == typeof(StringConverter)), Is.True);
            Assert.That(converters.Any(static x => x.GetType() == typeof(EqualityTypeConverter)), Is.True);
        }
    }

    /// <summary>
    /// Should register ReactiveUI creates command bindings.
    /// </summary>
    [Test]
    public void AutofacDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var builder = new ContainerBuilder();
        var locator = new AutofacDependencyResolver(builder);
        locator.InitializeReactiveUI();
        Locator.SetLocator(locator);
        var container = builder.Build();

        var converters = container.Resolve<IEnumerable<ICreatesCommandBinding>>().ToList();

        Assert.That(converters, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaEvent)), Is.True);
            Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter)), Is.True);
        }
    }

    /// <summary>
    /// Should register ReactiveUI binding type converters.
    /// </summary>
    [Test]
    public void NinjectDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new StandardKernel();
        container.UseNinjectDependencyResolver();
        Locator.CurrentMutable.InitializeReactiveUI();

        var converters = container.GetAll<IBindingTypeConverter>().ToList();

        Assert.That(converters, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(converters.Any(static x => x.GetType() == typeof(StringConverter)), Is.True);
            Assert.That(converters.Any(static x => x.GetType() == typeof(EqualityTypeConverter)), Is.True);
        }
    }

    /// <summary>
    /// Should register ReactiveUI creates command bindings.
    /// </summary>
    [Test]
    public void NinjectDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
    {
        // Invoke RxApp which initializes the ReactiveUI platform.
        var container = new StandardKernel();
        container.UseNinjectDependencyResolver();
        Locator.CurrentMutable.InitializeReactiveUI();

        var converters = container.GetAll<ICreatesCommandBinding>().ToList();

        Assert.That(converters, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaEvent)), Is.True);
            Assert.That(converters.Any(static x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter)), Is.True);
        }
    }
}
