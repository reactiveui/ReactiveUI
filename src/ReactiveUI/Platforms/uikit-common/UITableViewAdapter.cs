// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using Foundation;
using UIKit;

namespace ReactiveUI;

/// <summary>
/// Adapter that wraps a <see cref="UITableView"/> and implements <see cref="IUICollViewAdapter{TView,TCell}"/>.
/// </summary>
internal class UITableViewAdapter : IUICollViewAdapter<UITableView, UITableViewCell>, IDisposable
{
    /// <summary>The underlying table view being adapted.</summary>
    private readonly UITableView _view;

    /// <summary>Subject that tracks whether a data reload is currently in progress.</summary>
    private readonly BehaviorSubject<bool> _isReloadingData;

    /// <summary>The number of <see cref="ReloadData"/> calls that have not yet completed on the main thread.</summary>
    private int _inFlightReloads;

    /// <summary>Whether this instance has already been disposed.</summary>
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="UITableViewAdapter"/> class.
    /// </summary>
    /// <param name="view">The table view to adapt.</param>
    internal UITableViewAdapter(UITableView view)
    {
        _view = view;
        _isReloadingData = new BehaviorSubject<bool>(false);
    }

    /// <inheritdoc/>
    public IObservable<bool> IsReloadingData => _isReloadingData;

    /// <summary>Gets or sets the row animation used when inserting sections.</summary>
    public UITableViewRowAnimation InsertSectionsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    /// <summary>Gets or sets the row animation used when deleting sections.</summary>
    public UITableViewRowAnimation DeleteSectionsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    /// <summary>Gets or sets the row animation used when reloading sections.</summary>
    public UITableViewRowAnimation ReloadSectionsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    /// <summary>Gets or sets the row animation used when inserting rows.</summary>
    public UITableViewRowAnimation InsertRowsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    /// <summary>Gets or sets the row animation used when deleting rows.</summary>
    public UITableViewRowAnimation DeleteRowsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    /// <summary>Gets or sets the row animation used when reloading rows.</summary>
    public UITableViewRowAnimation ReloadRowsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    /// <inheritdoc/>
    public void ReloadData()
    {
        ++_inFlightReloads;
        _view.ReloadData();

        if (_inFlightReloads == 1)
        {
            Debug.Assert(!_isReloadingData.Value, "There is reload already happening.");
            _isReloadingData.OnNext(true);
        }

        // since ReloadData() queues the appropriate messages on the UI thread, we know we're done reloading
        // when this subsequent message is processed (with one caveat - see FinishReloadData for details)
        RxSchedulers.MainThreadScheduler.Schedule(FinishReloadData);
    }

    /// <inheritdoc/>
    public void BeginUpdates() => _view.BeginUpdates();

    /// <inheritdoc/>
    public void PerformUpdates(Action updates, Action completion)
    {
        _view.BeginUpdates();
        try
        {
            updates();
        }
        finally
        {
            _view.EndUpdates();
            completion();
        }
    }

    /// <inheritdoc/>
    public void EndUpdates() => _view.EndUpdates();

    /// <inheritdoc/>
    public void InsertSections(NSIndexSet indexes) => _view.InsertSections(indexes, InsertSectionsAnimation);

    /// <inheritdoc/>
    public void DeleteSections(NSIndexSet indexes) => _view.DeleteSections(indexes, DeleteSectionsAnimation);

    /// <inheritdoc/>
    public void ReloadSections(NSIndexSet indexes) => _view.ReloadSections(indexes, ReloadSectionsAnimation);

    /// <inheritdoc/>
    public void MoveSection(int fromIndex, int toIndex) => _view.MoveSection(fromIndex, toIndex);

    /// <inheritdoc/>
    public void InsertItems(NSIndexPath[] paths) => _view.InsertRows(paths, InsertRowsAnimation);

    /// <inheritdoc/>
    public void DeleteItems(NSIndexPath[] paths) => _view.DeleteRows(paths, DeleteRowsAnimation);

    /// <inheritdoc/>
    public void ReloadItems(NSIndexPath[] paths) => _view.ReloadRows(paths, ReloadRowsAnimation);

    /// <inheritdoc/>
    public void MoveItem(NSIndexPath path, NSIndexPath newPath) => _view.MoveRow(path, newPath);

    /// <inheritdoc/>
    public UITableViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path) => _view.DequeueReusableCell(cellKey, path);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases managed and unmanaged resources held by this instance.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose()"/>; <see langword="false"/> when called from a finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            _isReloadingData?.Dispose();
        }

        _isDisposed = true;
    }

    /// <summary>
    /// Decrements the in-flight reload counter and signals completion when all reloads are done.
    /// </summary>
    private void FinishReloadData()
    {
        --_inFlightReloads;

        if (_inFlightReloads != 0)
        {
            return;
        }

        // this is required because sometimes iOS schedules further work that results in calls to GetCell
        // that work could happen after FinishReloadData unless we force layout here
        // of course, we can't have that work running after IsReloading ticks to false because otherwise
        // some updates may occur before the calls to GetCell and thus the calls to GetCell could fail due to invalid indexes
        _view.LayoutIfNeeded();
        Debug.Assert(_isReloadingData.Value, "There are no reloads happening");
        _isReloadingData.OnNext(false);
    }
}
