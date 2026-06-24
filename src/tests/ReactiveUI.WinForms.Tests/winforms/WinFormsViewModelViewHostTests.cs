// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using TUnit.Core.Executors;
#if REACTIVE_SHIM
using WinFormsViewModelViewHost = ReactiveUI.Reactive.Winforms.ViewModelControlHost;
#else
using WinFormsViewModelViewHost = ReactiveUI.Winforms.ViewModelControlHost;
#endif

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for the WinForms view model view host (ViewModelControlHost).</summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class WinFormsViewModelViewHostTests
{
    /// <summary>Enables view caching before each test.</summary>
    [Before(Test)]
    public void SetUp() => WinFormsViewModelViewHost.DefaultCacheViewsEnabled = true;

    /// <summary>Disables view caching after each test.</summary>
    [After(Test)]
    public void TearDown() => WinFormsViewModelViewHost.DefaultCacheViewsEnabled = false;

    /// <summary>Tests that setting the view model adds the resolved view to the host controls.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Tests that the host disposes the previous view when the view model changes and caching is disabled.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
        currentView!.Disposed += (_, _) => isDisposed = true;

        // switch the viewmodel
        target.ViewModel = new FakeWinformViewModel();

        await Assert.That(isDisposed).IsTrue();
    }

    /// <summary>Tests that the host shows the default content when the view model is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Tests that the host caches the view when caching is enabled.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Tests that the host does not cache the view when caching is disabled.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Setting a property raises the host's PropertyChanging and PropertyChanged notifications.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SettingProperty_RaisesPropertyChangingAndChanged()
    {
        using var target = new WinFormsViewModelViewHost();

        // A manually implemented IReactiveObject only forwards classic events once these are enabled.
        target.SubscribePropertyChangingEvents();
        target.SubscribePropertyChangedEvents();

        var changing = false;
        var changed = false;
        target.PropertyChanging += (_, e) => changing |= e.PropertyName == nameof(target.ViewModel);
        target.PropertyChanged += (_, e) => changed |= e.PropertyName == nameof(target.ViewModel);

        target.ViewModel = new FakeWinformViewModel();

        using (Assert.Multiple())
        {
            await Assert.That(changing).IsTrue();
            await Assert.That(changed).IsTrue();
        }
    }

    /// <summary>Disposing the host tears down its subscriptions without error.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_TearsDownCleanly()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var target = new WinFormsViewModelViewHost
        {
            ViewLocator = viewLocator,
            ViewModel = new FakeWinformViewModel(),
        };

        target.Dispose();

        await Assert.That(target.IsDisposed).IsTrue();
    }

    /// <summary>When the view model is null but a default content is set, the host shows the default content.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullViewModelWithDefaultContent_ShowsDefaultContent()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var defaultContent = new Control();
        using var target = new WinFormsViewModelViewHost
        {
            ViewLocator = viewLocator,
            DefaultContent = defaultContent,
        };

        // ViewModel stays null, so the view-model/contract combine resolves to the default content.
        target.ViewModel = new FakeWinformViewModel();
        target.ViewModel = null;

        await Assert.That(target.CurrentView).IsSameReferenceAs(defaultContent);
    }

    /// <summary>When the view locator resolves no view, the host leaves the current content unchanged.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnresolvableViewModel_LeavesContentUnchanged()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => null! };
        using var target = new WinFormsViewModelViewHost
        {
            CacheViews = false,
            ViewLocator = viewLocator,
            ViewModel = new FakeWinformViewModel(),
        };

        await Assert.That(target.CurrentView).IsNull();
    }
}
