// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using Foundation;
using UIKit;

using NSAction = System.Action;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Adapter that wraps a <see cref="UICollectionView"/> and implements <see cref="IUICollViewAdapter{TView,TCell}"/>.</summary>
internal class UICollectionViewAdapter : IUICollViewAdapter<UICollectionView, UICollectionViewCell>, IDisposable
{
    /// <summary>The underlying collection view being adapted.</summary>
    private readonly UICollectionView _view;

    /// <summary>Subject that tracks whether a data reload is currently in progress.</summary>
    private readonly BehaviorSignal<bool> _isReloadingData;

    /// <summary>The number of <see cref="ReloadData"/> calls that have not yet completed on the main thread.</summary>
    private int _inFlightReloads;

    /// <summary>Whether this instance has already been disposed.</summary>
    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="UICollectionViewAdapter"/> class.</summary>
    /// <param name="view">The collection view to adapt.</param>
    internal UICollectionViewAdapter(UICollectionView view)
    {
        _view = view;
        _isReloadingData = new BehaviorSignal<bool>(false);
    }

    /// <inheritdoc/>
    public IObservable<bool> IsReloadingData => _isReloadingData;

    /// <inheritdoc/>
    public void ReloadData()
    {
        ++_inFlightReloads;
        _view.ReloadData();

        if (_inFlightReloads == 1)
        {
            Debug.Assert(!_isReloadingData.Value, "There is a reload already happening");
            _isReloadingData.OnNext(true);
        }

        // since ReloadData() queues the appropriate messages on the UI thread, we know we're done reloading
        // when this subsequent message is processed (with one caveat - see FinishReloadData for details)
        RxSchedulers.MainThreadScheduler.Schedule(FinishReloadData);
    }

    /// <summary>UICollectionView no longer has these methods so these are no-ops.</summary>
    public void BeginUpdates()
    {
    }

    /// <inheritdoc/>
    public void EndUpdates()
    {
    }

    /// <inheritdoc/>
    public void PerformUpdates(NSAction updates, NSAction completion) => _view.PerformBatchUpdates(updates, _ => completion());

    /// <inheritdoc/>
    public void InsertSections(NSIndexSet indexes) => _view.InsertSections(indexes);

    /// <inheritdoc/>
    public void DeleteSections(NSIndexSet indexes) => _view.DeleteSections(indexes);

    /// <inheritdoc/>
    public void ReloadSections(NSIndexSet indexes) => _view.ReloadSections(indexes);

    /// <inheritdoc/>
    public void MoveSection(int fromIndex, int toIndex) => _view.MoveSection(fromIndex, toIndex);

    /// <inheritdoc/>
    public void InsertItems(NSIndexPath[] paths) => _view.InsertItems(paths);

    /// <inheritdoc/>
    public void DeleteItems(NSIndexPath[] paths) => _view.DeleteItems(paths);

    /// <inheritdoc/>
    public void ReloadItems(NSIndexPath[] paths) => _view.ReloadItems(paths);

    /// <inheritdoc/>
    public void MoveItem(NSIndexPath path, NSIndexPath newPath) => _view.MoveItem(path, newPath);

    /// <inheritdoc/>
    public UICollectionViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path) => (UICollectionViewCell)_view.DequeueReusableCell(cellKey, path);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases managed and unmanaged resources held by this instance.</summary>
    /// <param name="isDisposing"><see langword="true"/> when called from <see cref="Dispose()"/>; <see langword="false"/> when called from a finalizer.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (isDisposing)
        {
            _isReloadingData?.Dispose();
        }

        _isDisposed = true;
    }

    /// <summary>Decrements the in-flight reload counter and signals completion when all reloads are done.</summary>
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
