// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The app's root on iPhone: a catalog tab, a members tab, and a cover-paging tab.</summary>
[System.Diagnostics.DebuggerDisplay("LibraryTabBarController")]
public sealed class LibraryTabBarController : ReactiveTabBarController<LibraryShellViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="LibraryTabBarController"/> class.</summary>
    /// <param name="shell">The shell that owns the catalog's router.</param>
    /// <param name="catalogViewLocator">The view locator the catalog tab's <see cref="RoutedViewHost"/> uses to
    /// resolve a view for each pushed view model.</param>
    /// <param name="catalog">The catalog view model shown first in the catalog tab.</param>
    /// <param name="members">The members view model shown in the members tab.</param>
    public LibraryTabBarController(LibraryShellViewModel shell, IViewLocator catalogViewLocator, BookCatalogViewModel catalog, MembersViewModel members)
    {
        ViewModel = shell;

        RoutedViewHost catalogHost = new()
        {
            Router = shell.Router,
            ViewLocator = catalogViewLocator,
        };
        catalogHost.TabBarItem = new UITabBarItem("Catalog", null, 0);
        _ = shell.Router.Navigate.Execute(catalog).Subscribe();

        MembersNavigationController membersNav = new(members)
        {
            TabBarItem = new UITabBarItem("Members", null, 1),
        };

        BookCoverPagerViewController coverPager = new()
        {
            ViewModel = catalog,
            TabBarItem = new UITabBarItem("Covers", null, 2),
        };

        BookShelfViewController shelf = new()
        {
            ViewModel = catalog,
            TabBarItem = new UITabBarItem("Shelf", null, 3),
        };

        ViewControllers = [catalogHost, membersNav, coverPager, shelf];

        _ = this.WhenActivated(d =>
        {
            d(Activated.Subscribe(static _ => Console.WriteLine("LibraryTabBarController activated.")));
            d(Deactivated.Subscribe(static _ => Console.WriteLine("LibraryTabBarController deactivated.")));
        });
    }
}
