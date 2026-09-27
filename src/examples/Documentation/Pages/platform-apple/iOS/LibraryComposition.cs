// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>Builds the sample library data and the app's root view controller.</summary>
internal static class LibraryComposition
{
    /// <summary>Creates the root view controller: a split view on iPad, a tab bar everywhere else.</summary>
    /// <returns>The root view controller for <see cref="AppDelegate.Window"/>.</returns>
    internal static UIViewController CreateRootViewController()
    {
        Member ada = new("M-1", "Ada");
        Member grace = new("M-2", "Grace");
        List<Member> members = [ada, grace];

        List<Book> books =
        [
            new("Pride and Prejudice", "Jane Austen"),
            new("The Hobbit", "J. R. R. Tolkien"),
            new("Dune", "Frank Herbert"),
        ];

        LibraryShellViewModel shell = new();
        BookCatalogViewModel catalog = new(shell, books, members);
        MembersViewModel membersViewModel = new(shell, members);

        if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
        {
            return new LibrarySplitViewController(shell, catalog);
        }

        DefaultViewLocator catalogViewLocator = new();
        catalogViewLocator.Map<BookCatalogViewModel, BookListViewController>();
        catalogViewLocator.Map<LoanViewModel, LoanViewController>();

        return new LibraryTabBarController(shell, catalogViewLocator, catalog, membersViewModel);
    }
}
