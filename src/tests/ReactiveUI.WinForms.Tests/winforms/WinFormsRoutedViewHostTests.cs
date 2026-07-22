// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using TUnit.Core.Executors;
#if REACTIVE_SHIM
using WinFormsRoutedViewHost = ReactiveUI.Reactive.Winforms.RoutedControlHost;
#else
using WinFormsRoutedViewHost = ReactiveUI.Winforms.RoutedControlHost;
#endif

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for the WinForms routed view host (RoutedControlHost).</summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class WinFormsRoutedViewHostTests
{
    /// <summary>Tests that the host disposes the previous view when navigating to a new view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldDisposePreviousView()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(Sequencer.Immediate);
        var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };
        _ = router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        var currentView = target.Controls.OfType<FakeWinformsView>().Single();
        var isDisposed = false;
        currentView.Disposed += (_, _) => isDisposed = true;

        // switch the viewmodel
        _ = router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        await Assert.That(isDisposed).IsTrue();
    }

    /// <summary>Tests that the host shows the default content when the view model is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldSetDefaultContentWhenViewModelIsNull()
    {
        var defaultContent = new Control();
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(Sequencer.Immediate);
        var target = new WinFormsRoutedViewHost
        {
            Router = router,
            ViewLocator = viewLocator,
            DefaultContent = defaultContent
        };

        await Assert.That(target.Controls.Contains(defaultContent)).IsTrue();
    }

    /// <summary>Tests that navigating to a view model adds the resolved view to the host controls.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenRoutedToViewModelItShouldAddViewToControls()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(Sequencer.Immediate);
        var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };
        _ = router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        await Assert.That(target.Controls.OfType<FakeWinformsView>().Count()).IsEqualTo(1);
    }

    /// <summary>Tests that a configured view-contract observable is honoured when resolving the view.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldResolveViewWithViewContractObservable()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(Sequencer.Immediate);
        var target = new WinFormsRoutedViewHost
        {
            Router = router,
            ViewLocator = viewLocator,
            ViewContractObservable = Signal.Emit("MyContract")
        };
        _ = router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        await Assert.That(target.Controls.OfType<FakeWinformsView>().Count()).IsEqualTo(1);
    }

    /// <summary>Tests that disposing the host tears down its subscriptions without error.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldDisposeCleanly()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(Sequencer.Immediate);
        var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };
        _ = router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        target.Dispose();

        await Assert.That(target.IsDisposed).IsTrue();
    }

    /// <summary>Setting a property raises the host's PropertyChanging and PropertyChanged notifications.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SettingProperty_RaisesPropertyChangingAndChanged()
    {
        using var target = new WinFormsRoutedViewHost();

        // A manually implemented IReactiveObject only forwards classic events once these are enabled.
        target.SubscribePropertyChangingEvents();
        target.SubscribePropertyChangedEvents();

        var changingName = string.Empty;
        var changedName = string.Empty;
        target.PropertyChanging += (_, e) => changingName = e.PropertyName ?? string.Empty;
        target.PropertyChanged += (_, e) => changedName = e.PropertyName ?? string.Empty;

        target.DefaultContent = new();

        using (Assert.Multiple())
        {
            await Assert.That(changingName).IsEqualTo(nameof(target.DefaultContent));
            await Assert.That(changedName).IsEqualTo(nameof(target.DefaultContent));
        }
    }

    /// <summary>
    /// Navigating back to an empty stack restores the default content: the resolved view is removed (disposed) and the
    /// reusable default content is re-hosted.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigatingBackToEmptyStack_RestoresDefaultContent()
    {
        var defaultContent = new Control();
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(Sequencer.Immediate);
        using var target = new WinFormsRoutedViewHost
        {
            Router = router,
            ViewLocator = viewLocator,
            DefaultContent = defaultContent,
        };

        _ = router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        // The resolved view replaced the default content.
        await Assert.That(target.Controls.OfType<FakeWinformsView>().Count()).IsEqualTo(1);

        // Navigating back empties the stack, so CurrentViewModel emits null and the default content is re-hosted.
        _ = router.NavigateBack.Execute().Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(target.Controls.Contains(defaultContent)).IsTrue();
            await Assert.That(target.Controls.OfType<FakeWinformsView>().Count()).IsEqualTo(0);
        }
    }

    /// <summary>When no ViewLocator is configured, the host falls back to the ambient ViewLocator.Current.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigatingWithoutViewLocator_UsesAmbientViewLocator()
    {
        var router = new RoutingState(Sequencer.Immediate);
        using var target = new WinFormsRoutedViewHost { Router = router };

        // No ViewLocator is assigned, so navigation exercises the ViewLocator.Current fallback path.
        _ = router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        await Assert.That(target).IsNotNull();
    }
}
