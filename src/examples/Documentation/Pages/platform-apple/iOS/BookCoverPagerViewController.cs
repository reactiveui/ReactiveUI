// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>Pages through one <see cref="BookDetailViewController"/> per book, left to right.</summary>
[System.Diagnostics.DebuggerDisplay("BookCoverPagerViewController")]
public sealed class BookCoverPagerViewController : ReactivePageViewController<BookCatalogViewModel>, IUIPageViewControllerDataSource
{
    /// <summary>Initializes a new instance of the <see cref="BookCoverPagerViewController"/> class with a scroll transition.</summary>
    public BookCoverPagerViewController()
        : base(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal)
    {
    }

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        DataSource = this;

        _ = this.WhenActivated((Action<IDisposable> onDispose) =>
        {
            _ = onDispose;
            if (ViewModel!.Books.Count == 0)
            {
                return;
            }

            SetViewControllers([CreatePage(ViewModel.Books[0])], UIPageViewControllerNavigationDirection.Forward, false, null);
        });
    }

    /// <inheritdoc/>
    public new UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
    {
        Book current = ((BookDetailViewController)referenceViewController).ViewModel!;
        int index = ViewModel!.Books.IndexOf(current) - 1;
        return index >= 0 ? CreatePage(ViewModel.Books[index]) : null!;
    }

    /// <inheritdoc/>
    public new UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
    {
        Book current = ((BookDetailViewController)referenceViewController).ViewModel!;
        int index = ViewModel!.Books.IndexOf(current) + 1;
        return index < ViewModel.Books.Count ? CreatePage(ViewModel.Books[index]) : null!;
    }

    /// <summary>Creates the detail page for one book, with its <c>ViewModel</c> set before the pager displays it.</summary>
    /// <param name="book">The book the page shows.</param>
    /// <returns>The page.</returns>
    private static BookDetailViewController CreatePage(Book book) => new() { ViewModel = book };
}
