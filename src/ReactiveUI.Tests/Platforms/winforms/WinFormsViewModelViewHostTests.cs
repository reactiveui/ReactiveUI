﻿// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using WinFormsViewModelViewHost = ReactiveUI.Winforms.ViewModelControlHost;

namespace ReactiveUI.Tests.Winforms;

public class WinFormsViewModelViewHostTests
{
    public WinFormsViewModelViewHostTests() => WinFormsViewModelViewHost.DefaultCacheViewsEnabled = true;

    [Fact]
    public void SettingViewModelShouldAddTheViewtoItsControls()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var target = new WinFormsViewModelViewHost
        {
            ViewLocator = viewLocator,

            ViewModel = new FakeWinformViewModel()
        };

        Assert.IsType<FakeWinformsView>(target.CurrentView);
        Assert.Equal(1, target.Controls.OfType<FakeWinformsView>().Count());
    }

    [Fact]
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

        Assert.True(isDisposed);
    }

    [Fact]
    public void ShouldSetDefaultContentWhenViewModelIsNull()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = _ => new FakeWinformsView() };
        var defaultContent = new Control();
        var target = new WinFormsViewModelViewHost { DefaultContent = defaultContent, ViewLocator = viewLocator };

        Assert.Equal(target.CurrentView, defaultContent);
        Assert.True(target.Controls.Contains(defaultContent));
    }

    [Fact]
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
        Assert.True(ReferenceEquals(cachedView, target.Content));
    }

    [Fact]
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
        Assert.False(ReferenceEquals(cachedView, target.CurrentView));
    }
}
