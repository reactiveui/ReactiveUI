// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using WinFormsRoutedViewHost = ReactiveUI.Winforms.RoutedControlHost;

namespace ReactiveUI.Tests.Winforms;

[TestFixture]
public class WinFormsRoutedViewHostTests
{
    [Test]
    public void ShouldDisposePreviousView()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var router = new RoutingState();
        var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };
        router?.Navigate?.Execute(new FakeWinformViewModel());

        var currentView = target.Controls.OfType<FakeWinformsView>().Single();
        var isDisposed = false;
        currentView.Disposed += (o, e) => isDisposed = true;

        // switch the viewmodel
        router?.Navigate?.Execute(new FakeWinformViewModel());

        Assert.That(isDisposed, Is.True);
    }

    [Test]
    public void ShouldSetDefaultContentWhenViewModelIsNull()
    {
        var defaultContent = new Control();
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var router = new RoutingState();
        var target = new WinFormsRoutedViewHost
        {
            Router = router,
            ViewLocator = viewLocator,
            DefaultContent = defaultContent
        };

        Assert.That(target.Controls.Contains(defaultContent, Is.True));
    }

    [Test]
    public void WhenRoutedToViewModelItShouldAddViewToControls()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var router = new RoutingState();
        var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };
        router?.Navigate?.Execute(new FakeWinformViewModel());

        Assert.That(target.Controls.OfType<FakeWinformsView>(, Is.EqualTo(1)).Count());
    }
}
