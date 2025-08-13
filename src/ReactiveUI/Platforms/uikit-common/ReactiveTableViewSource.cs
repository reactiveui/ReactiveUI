// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;

using Foundation;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// ReactiveTableViewSource is a Table View Source that is connected to
/// a List that automatically updates the View based on the
/// contents of the list. The collection changes are buffered and View
/// items are animated in and out as items are added.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("ReactiveTableViewSource uses methods that require dynamic code generation")]
[RequiresUnreferencedCode("ReactiveTableViewSource uses methods that may require unreferenced code")]
#endif
public class ReactiveTableViewSource<TSource> : UITableViewSource, IReactiveNotifyPropertyChanged<ReactiveTableViewSource<TSource>>, IHandleObservableErrors, IReactiveObject
{
    private readonly CommonReactiveSource<TSource, UITableView, UITableViewCell, TableSectionInformation<TSource>> _commonSource;
    private readonly Subject<object?> _elementSelected = new();
    private readonly UITableViewAdapter _adapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewSource{TSource}"/> class.
    /// </summary>
    /// <param name="tableView">The table view.</param>
    /// <param name="collection">The collection.</param>
    /// <param name="cellKey">The cell key.</param>
    /// <param name="sizeHint">The size hint.</param>
    /// <param name="initializeCellAction">The initialize cell action.</param>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("ReactiveTableViewSource uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("ReactiveTableViewSource uses methods that may require unreferenced code")]
#endif
    public ReactiveTableViewSource(UITableView tableView, INotifyCollectionChanged collection, NSString cellKey, float sizeHint, Action<UITableViewCell>? initializeCellAction = null)
        : this(tableView) =>
        Data = new[] { new TableSectionInformation<TSource, UITableViewCell>(collection, cellKey, sizeHint, initializeCellAction) };

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTableViewSource{TSource}"/> class.
    /// </summary>
    /// <param name="tableView">The table view.</param>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("ReactiveTableViewSource uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("ReactiveTableViewSource uses methods that may require unreferenced code")]
#endif
    public ReactiveTableViewSource(UITableView tableView)
    {
        _adapter = new UITableViewAdapter(tableView);
        _commonSource = new CommonReactiveSource<TSource, UITableView, UITableViewCell, TableSectionInformation<TSource>>(_adapter);
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the data that should be displayed by this
    /// <see cref="ReactiveTableViewSource{TSource}"/>.  You should
    /// probably bind your view model to this property.
    /// </summary>
    /// <value>The data.</value>
    public IReadOnlyList<TableSectionInformation<TSource>> Data
    {
        get => _commonSource.SectionInfo;

        set
        {
            if (Equals(_commonSource.SectionInfo, value))
            {
                return;
            }

            this.RaisingPropertyChanging(nameof(Data));
            _commonSource.SectionInfo = value;
            this.RaisingPropertyChanged(nameof(Data));
        }
    }

    /// <summary>
    /// Gets an IObservable that is a hook to <see cref="RowSelected"/> calls.
    /// </summary>
    public IObservable<object?> ElementSelected => _elementSelected;

    /// <summary>
    /// Gets or sets the row animation to use when UITableView.InsertSections is invoked.
    /// </summary>
    public UITableViewRowAnimation InsertSectionsAnimation
    {
        get => _adapter.InsertSectionsAnimation;
        set => _adapter.InsertSectionsAnimation = value;
    }

    /// <summary>
    /// Gets or sets the row animation to use when UITableView.DeleteSections is invoked.
    /// </summary>
    public UITableViewRowAnimation DeleteSectionsAnimation
    {
        get => _adapter.DeleteSectionsAnimation;
        set => _adapter.DeleteSectionsAnimation = value;
    }

    /// <summary>
    /// Gets or sets the row animation to use when UITableView.ReloadSections is invoked.
    /// </summary>
    public UITableViewRowAnimation ReloadSectionsAnimation
    {
        get => _adapter.ReloadSectionsAnimation;
        set => _adapter.ReloadSectionsAnimation = value;
    }

    /// <summary>
    /// Gets or sets the row animation to use when UITableView.InsertRows is invoked.
    /// </summary>
    public UITableViewRowAnimation InsertRowsAnimation
    {
        get => _adapter.InsertRowsAnimation;
        set => _adapter.InsertRowsAnimation = value;
    }

    /// <summary>
    /// Gets or sets the row animation to use when UITableView.DeleteRows is invoked.
    /// </summary>
    public UITableViewRowAnimation DeleteRowsAnimation
    {
        get => _adapter.DeleteRowsAnimation;
        set => _adapter.DeleteRowsAnimation = value;
    }

    /// <summary>
    /// Gets or sets the row animation to use when UITableView.ReloadRows is invoked.
    /// </summary>
    public UITableViewRowAnimation ReloadRowsAnimation
    {
        get => _adapter.ReloadRowsAnimation;
        set => _adapter.ReloadRowsAnimation = value;
    }

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewSource<TSource>>> Changing => this.GetChangingObservable();

    /// <inheritdoc />
    public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewSource<TSource>>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <inheritdoc/>
    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        ArgumentNullException.ThrowIfNull(indexPath);

        return _commonSource.GetCell(indexPath);
    }

    /// <inheritdoc/>
    public override nint NumberOfSections(UITableView tableView) => _commonSource.NumberOfSections();

    /// <inheritdoc/>
    public override nint RowsInSection(UITableView tableView, nint section)
    {
        // iOS may call this method even when we have no sections, but only if we've overriden
        // EstimatedHeight(UITableView, NSIndexPath) in our UITableViewSource
        if (section >= _commonSource.NumberOfSections())
        {
            return 0;
        }

        return _commonSource.RowsInSection((int)section);
    }

    /// <inheritdoc/>
    public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath) => false;

    /// <inheritdoc/>
    public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath) => false;

    /// <inheritdoc/>
    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        ArgumentNullException.ThrowIfNull(indexPath);

        _elementSelected.OnNext(_commonSource.ItemAt(indexPath));
    }

    /// <inheritdoc/>
    public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
    {
        ArgumentNullException.ThrowIfNull(indexPath);

        return _commonSource.SectionInfo[indexPath.Section].SizeHint;
    }

    /// <inheritdoc/>
    public override nfloat GetHeightForHeader(UITableView tableView, nint section)
    {
        // iOS may call this method even when we have no sections, but only if we've overriden
        // EstimatedHeight(UITableView, NSIndexPath) in our UITableViewSource
        if (section >= _commonSource.NumberOfSections())
        {
            return 0;
        }

        var header = _commonSource.SectionInfo[(int)section].Header;

        // NB: -1 is a magic # that causes iOS to use the regular height. go figure.
        return header?.View is null ? -1 : header.Height;
    }

    /// <inheritdoc/>
    public override nfloat GetHeightForFooter(UITableView tableView, nint section)
    {
        // iOS may call this method even when we have no sections, but only if we've overriden
        // EstimatedHeight(UITableView, NSIndexPath) in our UITableViewSource
        if (section >= _commonSource.NumberOfSections())
        {
            return 0;
        }

        var footer = _commonSource.SectionInfo[(int)section].Footer;
        return footer?.View is null ? -1 : footer.Height;
    }

#pragma warning disable CS8603 // Possible null reference return.

    /// <inheritdoc/>
    public override string TitleForHeader(UITableView tableView, nint section)
    {
        var header = _commonSource.SectionInfo[(int)section].Header;
        return header?.Title;
    }

    /// <inheritdoc/>
    public override string TitleForFooter(UITableView tableView, nint section)
    {
        var footer = _commonSource.SectionInfo[(int)section].Footer;
        return footer?.Title;
    }

    /// <inheritdoc/>
    public override UIView GetViewForHeader(UITableView tableView, nint section)
    {
        var header = _commonSource.SectionInfo[(int)section].Header;
        return header?.View?.Invoke();
    }

    /// <inheritdoc/>
    public override UIView GetViewForFooter(UITableView tableView, nint section)
    {
        var footer = _commonSource.SectionInfo[(int)section].Footer;
        return footer?.View?.Invoke();
    }

#pragma warning restore CS8603 // Possible null reference return.

    /// <summary>
    /// Items at.
    /// </summary>
    /// <param name="indexPath">The index path.</param>
    /// <returns>The item.</returns>
    public object? ItemAt(NSIndexPath indexPath)
    {
        ArgumentNullException.ThrowIfNull(indexPath);

        return _commonSource.ItemAt(indexPath);
    }

    /// <summary>
    /// When this method is called, an object will not fire change
    /// notifications (neither traditional nor Observable notifications)
    /// until the return value is disposed.
    /// </summary>
    /// <returns>An object that, when disposed, re-enables change
    /// notifications.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("SuppressChangeNotifications uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("SuppressChangeNotifications uses methods that may require unreferenced code")]
#endif
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _commonSource.Dispose();
            _elementSelected.Dispose();
            _adapter.Dispose();
        }

        base.Dispose(disposing);
    }
}
