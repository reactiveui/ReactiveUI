// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

using Foundation;

using UIKit;

using NSAction = System.Action;

namespace ReactiveUI;

internal class UICollectionViewAdapter : IUICollViewAdapter<UICollectionView, UICollectionViewCell>, IDisposable
{
    private readonly UICollectionView _view;
    private readonly BehaviorSubject<bool> _isReloadingData;
    private int _inFlightReloads;
    private bool _isDisposed;

    internal UICollectionViewAdapter(UICollectionView view)
    {
        _view = view;
        _isReloadingData = new BehaviorSubject<bool>(false);
    }

    public IObservable<bool> IsReloadingData => _isReloadingData.AsObservable();

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

    // UICollectionView no longer has these methods so these are no-ops
    public void BeginUpdates()
    {
    }

    public void EndUpdates()
    {
    }

    public void PerformUpdates(NSAction updates, NSAction completion) => _view.PerformBatchUpdates(new NSAction(updates), (completed) => completion());

    public void InsertSections(NSIndexSet indexes) => _view.InsertSections(indexes);

    public void DeleteSections(NSIndexSet indexes) => _view.DeleteSections(indexes);

    public void ReloadSections(NSIndexSet indexes) => _view.ReloadSections(indexes);

    public void MoveSection(int fromIndex, int toIndex) => _view.MoveSection(fromIndex, toIndex);

    public void InsertItems(NSIndexPath[] paths) => _view.InsertItems(paths);

    public void DeleteItems(NSIndexPath[] paths) => _view.DeleteItems(paths);

    public void ReloadItems(NSIndexPath[] paths) => _view.ReloadItems(paths);

    public void MoveItem(NSIndexPath path, NSIndexPath newPath) => _view.MoveItem(path, newPath);

    public UICollectionViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path) => (UICollectionViewCell)_view.DequeueReusableCell(cellKey, path);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

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

    private void FinishReloadData()
    {
        --_inFlightReloads;

        if (_inFlightReloads == 0)
        {
            // this is required because sometimes iOS schedules further work that results in calls to GetCell
            // that work could happen after FinishReloadData unless we force layout here
            // of course, we can't have that work running after IsReloading ticks to false because otherwise
            // some updates may occur before the calls to GetCell and thus the calls to GetCell could fail due to invalid indexes
            _view.LayoutIfNeeded();
            Debug.Assert(_isReloadingData.Value, "There are no reloads happening");
            _isReloadingData.OnNext(false);
        }
    }
}
