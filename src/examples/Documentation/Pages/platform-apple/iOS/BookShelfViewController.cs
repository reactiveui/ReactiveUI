// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;
using Foundation;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The shelf tab: every book as a cover tile in a grid.</summary>
[System.Diagnostics.DebuggerDisplay("BookShelfViewController")]
public sealed class BookShelfViewController : ReactiveCollectionViewController<BookCatalogViewModel>
{
    /// <summary>The reuse identifier every cover shares; the shelf never needs a different cell per book.</summary>
    private static readonly NSString CoverCellKey = new(nameof(BookCoverCell));

    /// <summary>Initializes a new instance of the <see cref="BookShelfViewController"/> class with a two-column grid layout.</summary>
    public BookShelfViewController()
        : base(new UICollectionViewFlowLayout
        {
            ItemSize = new CGSize(140, 90),
            MinimumInteritemSpacing = 12,
            MinimumLineSpacing = 12,
            SectionInset = new UIEdgeInsets(12, 12, 12, 12),
            HeaderReferenceSize = new CGSize(0, 32),
        })
    {
    }

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Shelf";
        CollectionView!.BackgroundColor = UIColor.SystemBackground;
        CollectionView.RegisterClassForCell(typeof(BookCoverCell), CoverCellKey);
        CollectionView.RegisterClassForSupplementaryView(typeof(ShelfHeaderView), UICollectionElementKindSection.Header, BookShelfCollectionViewSource.HeaderKey);

        _ = this.WhenActivated(d =>
        {
            CollectionViewSectionInformation<Book, BookCoverCell> section = new(
                ViewModel!.Books,
                static _ => CoverCellKey,
                static cell => Console.WriteLine($"Initializing a {cell.GetType().Name}."));
            IReadOnlyList<CollectionViewSectionInformation<Book, BookCoverCell>> sections = [section];

            BookShelfCollectionViewSource source = new(CollectionView!, ViewModel);
            source.Data = sections;
            CollectionView!.Source = source;

            CollectionViewSectionInformation<Book> readBack = source.Data[0];
            Console.WriteLine(
                $"Shelf has {source.Data.Count} section(s); every cover shares one reuse key: {readBack.CellKeySelector is not null}.");

            d(source);
            d(source.ElementSelected.Subscribe(static item => Console.WriteLine($"Selected cover {((Book)item!).Title}.")));
        });
    }
}
