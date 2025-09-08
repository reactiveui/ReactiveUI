// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using WinFormsViewModelViewHost = ReactiveUI.Winforms.ViewModelControlHost;

namespace ReactiveUI.Tests.Winforms;

[TestFixture]
public class WinFormsViewModelViewHostTests
{
    public WinFormsViewModelViewHostTests() => WinFormsViewModelViewHost.DefaultCacheViewsEnabled = true;

    [Test]
    public void SettingViewModelShouldAddTheViewtoItsControls()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var target = new WinFormsViewModelViewHost
        {
            ViewLocator = viewLocator,

            ViewModel = new FakeWinformViewModel()
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(target.CurrentView, Is.TypeOf<FakeWinformsView>());
            Assert.That(target.Controls.OfType<FakeWinformsView>().Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public void ShouldDisposePreviousView()
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

        Assert.That(isDisposed, Is.True);
    }

    [Test]
    public void ShouldSetDefaultContentWhenViewModelIsNull()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var defaultContent = new Control();
        var target = new WinFormsViewModelViewHost { DefaultContent = defaultContent, ViewLocator = viewLocator };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(defaultContent, Is.EqualTo(target.CurrentView));
            Assert.That(target.Controls.Contains(defaultContent), Is.True);
        }
    }

    [Test]
    public void ShouldCacheViewWhenEnabled()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
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
        Assert.That(ReferenceEquals(cachedView, target.Content), Is.True);
    }

    [Test]
    public void ShouldNotCacheViewWhenDisabled()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
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
        Assert.That(ReferenceEquals(cachedView, target.CurrentView), Is.False);
    }
}
