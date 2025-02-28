// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

using Foundation;

using UIKit;

namespace ReactiveUI;

#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
internal class UITableViewAdapter : IUICollViewAdapter<UITableView, UITableViewCell>, IDisposable
{
    private readonly UITableView _view;
    private readonly BehaviorSubject<bool> _isReloadingData;
    private int _inFlightReloads;
    private bool _isDisposed;

    internal UITableViewAdapter(UITableView view)
    {
        _view = view;
        _isReloadingData = new BehaviorSubject<bool>(false);
    }

    public IObservable<bool> IsReloadingData => _isReloadingData.AsObservable();

    public UITableViewRowAnimation InsertSectionsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    public UITableViewRowAnimation DeleteSectionsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    public UITableViewRowAnimation ReloadSectionsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    public UITableViewRowAnimation InsertRowsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    public UITableViewRowAnimation DeleteRowsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

    public UITableViewRowAnimation ReloadRowsAnimation { get; set; } = UITableViewRowAnimation.Automatic;

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
        RxApp.MainThreadScheduler.Schedule(FinishReloadData);
    }

    public void BeginUpdates() => _view.BeginUpdates();

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

    public void EndUpdates() => _view.EndUpdates();

    public void InsertSections(NSIndexSet indexes) => _view.InsertSections(indexes, InsertSectionsAnimation);

    public void DeleteSections(NSIndexSet indexes) => _view.DeleteSections(indexes, DeleteSectionsAnimation);

    public void ReloadSections(NSIndexSet indexes) => _view.ReloadSections(indexes, ReloadSectionsAnimation);

    public void MoveSection(int fromIndex, int toIndex) => _view.MoveSection(fromIndex, toIndex);

    public void InsertItems(NSIndexPath[] paths) => _view.InsertRows(paths, InsertRowsAnimation);

    public void DeleteItems(NSIndexPath[] paths) => _view.DeleteRows(paths, DeleteRowsAnimation);

    public void ReloadItems(NSIndexPath[] paths) => _view.ReloadRows(paths, ReloadRowsAnimation);

    public void MoveItem(NSIndexPath path, NSIndexPath newPath) => _view.MoveRow(path, newPath);

    public UITableViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path) => _view.DequeueReusableCell(cellKey, path);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _isReloadingData?.Dispose();
            }

            _isDisposed = true;
        }
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
