// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;
using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using WinFormsRoutedViewHost = ReactiveUI.Winforms.RoutedControlHost;

namespace ReactiveUI.WinForms.Tests.Winforms;

[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class WinFormsRoutedViewHostTests
{
    [Test]
    public async Task ShouldDisposePreviousView()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var router = new RoutingState(ImmediateScheduler.Instance);
        var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };
        router?.Navigate?.Execute(new FakeWinformViewModel());

        var currentView = target.Controls.OfType<FakeWinformsView>().Single();
        var isDisposed = false;
        currentView.Disposed += (o, e) => isDisposed = true;

        // switch the viewmodel
        router?.Navigate?.Execute(new FakeWinformViewModel());

        await Assert.That(isDisposed).IsTrue();
    }

    [Test]
    public async Task ShouldSetDefaultContentWhenViewModelIsNull()
    {
        var defaultContent = new Control();
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(ImmediateScheduler.Instance);
        var target = new WinFormsRoutedViewHost
        {
            Router = router,
            ViewLocator = viewLocator,
            DefaultContent = defaultContent
        };

        await Assert.That(target.Controls.Contains(defaultContent)).IsTrue();
    }

    [Test]
    public async Task WhenRoutedToViewModelItShouldAddViewToControls()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(ImmediateScheduler.Instance);
        var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };
        router?.Navigate?.Execute(new FakeWinformViewModel());

        await Assert.That(target.Controls.OfType<FakeWinformsView>().Count()).IsEqualTo(1);
    }
}
