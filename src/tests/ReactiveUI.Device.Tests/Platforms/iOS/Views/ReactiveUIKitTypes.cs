// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;
using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>Concrete subclasses of the reactive UIKit types, whose constructors are protected.</summary>
internal static class ReactiveUIKitTypes
{
    /// <summary>A plain <see cref="ReactiveTableViewController"/>.</summary>
    internal sealed class TableViewController : ReactiveTableViewController;

    /// <summary>A plain <see cref="ReactiveCollectionViewController"/> with a flow layout.</summary>
    internal sealed class CollectionViewController() : ReactiveCollectionViewController(new UICollectionViewFlowLayout());

    /// <summary>A plain <see cref="ReactivePageViewController"/>.</summary>
    internal sealed class PageViewController : ReactivePageViewController;

    /// <summary>A plain <see cref="ReactiveNavigationController"/>.</summary>
    internal sealed class NavigationController : ReactiveNavigationController;

    /// <summary>A plain <see cref="ReactiveTabBarController"/>.</summary>
    internal sealed class TabBarController : ReactiveTabBarController;

    /// <summary>A plain <see cref="ReactiveSplitViewController"/>.</summary>
    internal sealed class SplitViewController : ReactiveSplitViewController;

    /// <summary>A plain <see cref="ReactiveView"/>.</summary>
    internal sealed class View : ReactiveView;

    /// <summary>A plain <see cref="ReactiveControl"/>.</summary>
    internal sealed class Control : ReactiveControl;

    /// <summary>A plain <see cref="ReactiveImageView"/>.</summary>
    internal sealed class ImageView : ReactiveImageView;

    /// <summary>A plain <see cref="ReactiveTableView"/>.</summary>
    internal sealed class TableView : ReactiveTableView;

    /// <summary>A plain <see cref="ReactiveTableViewCell"/>.</summary>
    internal sealed class TableViewCell : ReactiveTableViewCell;

    /// <summary>A plain <see cref="ReactiveCollectionView"/> with a flow layout.</summary>
    internal sealed class CollectionView() : ReactiveCollectionView(CGRect.Empty, new UICollectionViewFlowLayout());

    /// <summary>A plain <see cref="ReactiveCollectionViewCell"/>.</summary>
    internal sealed class CollectionViewCell : ReactiveCollectionViewCell;

    /// <summary>A plain <see cref="ReactiveCollectionReusableView"/>.</summary>
    internal sealed class CollectionReusableView : ReactiveCollectionReusableView;
}
