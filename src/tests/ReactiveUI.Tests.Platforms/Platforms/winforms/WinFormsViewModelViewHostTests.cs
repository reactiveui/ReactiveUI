// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using WinFormsViewModelViewHost = ReactiveUI.Winforms.ViewModelControlHost;

namespace ReactiveUI.Tests.Winforms;

public class WinFormsViewModelViewHostTests
{
    public WinFormsViewModelViewHostTests() => WinFormsViewModelViewHost.DefaultCacheViewsEnabled = true;

    [Test]
    public async Task SettingViewModelShouldAddTheViewtoItsControls()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var target = new WinFormsViewModelViewHost
        {
            ViewLocator = viewLocator,

            ViewModel = new FakeWinformViewModel()
        };

        using (Assert.Multiple())
        {
            await Assert.That(target.CurrentView).IsTypeOf<FakeWinformsView>();
            await Assert.That(target.Controls.OfType<FakeWinformsView>().Count()).IsEqualTo(1);
        }
    }

    [Test]
    public async Task ShouldDisposePreviousView()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var target = new WinFormsViewModelViewHost
        {
            CacheViews = false,
            ViewLocator = viewLocator,

            ViewModel = new FakeWinformViewModel()
        };

        var currentView = target.CurrentView;
        var isDisposed = false;
        currentView!.Disposed += (o, e) => isDisposed = true;

        // switch the viewmodel
        target.ViewModel = new FakeWinformViewModel();

        await Assert.That(isDisposed).IsTrue();
    }

    [Test]
    public async Task ShouldSetDefaultContentWhenViewModelIsNull()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var defaultContent = new Control();
        var target = new WinFormsViewModelViewHost { DefaultContent = defaultContent, ViewLocator = viewLocator };

        using (Assert.Multiple())
        {
            await Assert.That(defaultContent).IsEqualTo(target.CurrentView);
            await Assert.That(target.Controls.Contains(defaultContent)).IsTrue();
        }
    }

    [Test]
    public async Task ShouldCacheViewWhenEnabled()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var defaultContent = new Control();
        var target = new WinFormsViewModelViewHost
        {
            DefaultContent = defaultContent,
            ViewLocator = viewLocator,
            CacheViews = true,
            ViewModel = new FakeWinformViewModel()
        };
        var cachedView = target.Content;
        target.ViewModel = new FakeWinformViewModel();
        await Assert.That(ReferenceEquals(cachedView, target.Content)).IsTrue();
    }

    [Test]
    public async Task ShouldNotCacheViewWhenDisabled()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var defaultContent = new Control();
        var target = new WinFormsViewModelViewHost
        {
            DefaultContent = defaultContent,
            ViewLocator = viewLocator,
            CacheViews = false,
            ViewModel = new FakeWinformViewModel()
        };
        var cachedView = target.CurrentView;
        target.ViewModel = new FakeWinformViewModel();
        await Assert.That(ReferenceEquals(cachedView, target.CurrentView)).IsFalse();
    }
}
