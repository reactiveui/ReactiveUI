// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>A <see cref="ReactiveCollectionViewSource{TSource}"/> that also supplies the shelf's section header. The
/// <c>ReactiveCollectionViewSourceExtensions.BindTo</c> family always builds a plain <see cref="ReactiveCollectionViewSource{TSource}"/>,
/// which has no header hook of its own, so a grid that needs one subclasses the source directly instead, the same
/// way that extension method does internally.</summary>
internal sealed class BookShelfCollectionViewSource : ReactiveCollectionViewSource<Book>
{
    /// <summary>The reuse identifier the shelf registers <see cref="ShelfHeaderView"/> under.</summary>
    internal static readonly NSString HeaderKey = new(nameof(ShelfHeaderView));

    /// <summary>The catalog the header's book count comes from.</summary>
    private readonly BookCatalogViewModel _catalog;

    /// <summary>Initializes a new instance of the <see cref="BookShelfCollectionViewSource"/> class.</summary>
    /// <param name="collectionView">The grid this source drives.</param>
    /// <param name="catalog">The catalog the header's book count comes from.</param>
    internal BookShelfCollectionViewSource(UICollectionView collectionView, BookCatalogViewModel catalog)
        : base(collectionView) =>
        _catalog = catalog;

    /// <inheritdoc/>
    public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
    {
        ShelfHeaderView header = (ShelfHeaderView)collectionView.DequeueReusableSupplementaryView(UICollectionElementKindSection.Header, HeaderKey, indexPath);
        header.ViewModel = _catalog;
        return header;
    }
}
