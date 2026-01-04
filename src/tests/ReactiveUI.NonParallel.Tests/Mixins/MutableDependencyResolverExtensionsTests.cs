// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests.Mixins;

/// <summary>
/// Tests for MutableDependencyResolverExtensions.
/// </summary>
[NotInParallel]
public class MutableDependencyResolverExtensionsTests : IDisposable
{
    private RxSchedulersSchedulersScope? _schedulersScope;

    [Before(Test)]
    public void SetUp()
    {
        _schedulersScope = new RxSchedulersSchedulersScope();
    }

    [After(Test)]
    public void TearDown()
    {
        _schedulersScope?.Dispose();
    }

    [Test]
    public async Task RegisterViewForViewModelRegistersView()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterViewForViewModel<TestView, TestViewModel>();

        var view = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsOfType(typeof(TestView));
        }
    }

    [Test]
    public async Task RegisterViewForViewModelWithContractRegistersView()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterViewForViewModel<TestView, TestViewModel>("TestContract");

        var view = resolver.GetService<IViewFor<TestViewModel>>("TestContract");

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsOfType(typeof(TestView));
        }
    }

    [Test]
    public async Task RegisterViewForViewModelReturnsResolver()
    {
        var resolver = new ModernDependencyResolver();
        var result = resolver.RegisterViewForViewModel<TestView, TestViewModel>();

        await Assert.That(result).IsEqualTo(resolver);
    }

    [Test]
    public async Task RegisterViewForViewModelCreatesNewInstanceEachTime()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterViewForViewModel<TestView, TestViewModel>();

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view2).IsNotNull();
            await Assert.That(ReferenceEquals(view1, view2)).IsFalse();
        }
    }

    [Test]
    public void RegisterViewForViewModelThrowsOnNullResolver()
    {
        IMutableDependencyResolver? resolver = null;
        Assert.Throws<ArgumentNullException>(() =>
            resolver!.RegisterViewForViewModel<TestView, TestViewModel>());
    }

    [Test]
    public async Task RegisterSingletonViewForViewModelRegistersSingleton()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterSingletonViewForViewModel<TestView, TestViewModel>();

        var view = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsOfType(typeof(TestView));
        }
    }

    [Test]
    public async Task RegisterSingletonViewForViewModelWithContractRegistersSingleton()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterSingletonViewForViewModel<TestView, TestViewModel>("TestContract");

        var view = resolver.GetService<IViewFor<TestViewModel>>("TestContract");

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsOfType(typeof(TestView));
        }
    }

    [Test]
    public async Task RegisterSingletonViewForViewModelReturnsResolver()
    {
        var resolver = new ModernDependencyResolver();
        var result = resolver.RegisterSingletonViewForViewModel<TestView, TestViewModel>();

        await Assert.That(result).IsEqualTo(resolver);
    }

    [Test]
    public async Task RegisterSingletonViewForViewModelReturnsSameInstance()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterSingletonViewForViewModel<TestView, TestViewModel>();

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view2).IsNotNull();
            await Assert.That(ReferenceEquals(view1, view2)).IsTrue();
        }
    }

    [Test]
    public void RegisterSingletonViewForViewModelThrowsOnNullResolver()
    {
        IMutableDependencyResolver? resolver = null;
        Assert.Throws<ArgumentNullException>(() =>
            resolver!.RegisterSingletonViewForViewModel<TestView, TestViewModel>());
    }

    [Test]
    public async Task RegisterViewForViewModelSupportsChaining()
    {
        var resolver = new ModernDependencyResolver();
        resolver
            .RegisterViewForViewModel<TestView, TestViewModel>()
            .RegisterViewForViewModel<AlternateTestView, AlternateTestViewModel>();

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<AlternateTestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view2).IsNotNull();
        }
    }

    [Test]
    public async Task RegisterSingletonViewForViewModelSupportsChaining()
    {
        var resolver = new ModernDependencyResolver();
        resolver
            .RegisterSingletonViewForViewModel<TestView, TestViewModel>()
            .RegisterSingletonViewForViewModel<AlternateTestView, AlternateTestViewModel>();

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<AlternateTestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view2).IsNotNull();
        }
    }

    public void Dispose()
    {
        _schedulersScope?.Dispose();
        _schedulersScope = null;
    }

    /// <summary>
    /// Test view model.
    /// </summary>
    private class TestViewModel : ReactiveObject
    {
    }

    /// <summary>
    /// Test view.
    /// </summary>
    private class TestView : IViewFor<TestViewModel>
    {
        public TestViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TestViewModel;
        }
    }

    /// <summary>
    /// Alternate test view model.
    /// </summary>
    private class AlternateTestViewModel : ReactiveObject
    {
    }

    /// <summary>
    /// Alternate test view.
    /// </summary>
    private class AlternateTestView : IViewFor<AlternateTestViewModel>
    {
        public AlternateTestViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as AlternateTestViewModel;
        }
    }
}
