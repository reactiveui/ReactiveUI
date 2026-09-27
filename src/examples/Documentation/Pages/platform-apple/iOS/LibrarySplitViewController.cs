// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The app's root on iPad: the catalog as the master column, a <see cref="ViewModelViewHost"/> as the detail.</summary>
[System.Diagnostics.DebuggerDisplay("LibrarySplitViewController")]
public sealed class LibrarySplitViewController : ReactiveSplitViewController<LibraryShellViewModel>
{
    /// <summary>Shows the detail view for whichever book the master column selects.</summary>
    private readonly ViewModelViewHost _detailHost;

    /// <summary>Initializes a new instance of the <see cref="LibrarySplitViewController"/> class.</summary>
    /// <param name="shell">The shell view model.</param>
    /// <param name="catalog">The catalog shown in the master column.</param>
    public LibrarySplitViewController(LibraryShellViewModel shell, BookCatalogViewModel catalog)
    {
        ViewModel = shell;
        PreferredDisplayMode = UISplitViewControllerDisplayMode.OneBesideSecondary;

        DefaultViewLocator detailLocator = new();
        detailLocator.Map<Book, BookDetailViewController>();
        _detailHost = new ViewModelViewHost
        {
            DefaultContent = new UIViewController(),
            ViewLocator = detailLocator,
        };

        BookListViewController master = new() { ViewModel = catalog };
        ViewControllers = [master, _detailHost];

        _ = this.WhenActivated(d =>
            d(catalog.WhenAnyValue(static vm => vm.SelectedBook)
                .WhereNotNull()
                .Subscribe(book => _detailHost.ViewModel = book)));
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _detailHost.Dispose();
        }

        base.Dispose(disposing);
    }
}
