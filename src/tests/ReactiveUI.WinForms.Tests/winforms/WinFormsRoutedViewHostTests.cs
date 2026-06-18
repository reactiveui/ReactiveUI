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
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var router = new RoutingState(Sequencer.Immediate);
        var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };
        router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        var currentView = target.Controls.OfType<FakeWinformsView>().Single();
        var isDisposed = false;
        currentView.Disposed += (_, _) => isDisposed = true;

        // switch the viewmodel
        router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

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
        router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        await Assert.That(target.Controls.OfType<FakeWinformsView>().Count()).IsEqualTo(1);
    }
}
