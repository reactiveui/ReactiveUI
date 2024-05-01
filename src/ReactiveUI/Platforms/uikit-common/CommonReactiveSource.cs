// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;

using DynamicData;
using DynamicData.Binding;

using Foundation;

namespace ReactiveUI;

internal sealed class CommonReactiveSource<TSource, TUIView, TUIViewCell, TSectionInfo> : ReactiveObject, IDisposable
    where TSectionInfo : ISectionInformation<TUIViewCell>
{
    private readonly IUICollViewAdapter<TUIView, TUIViewCell> _adapter;
    private readonly int _mainThreadId;
    private readonly CompositeDisposable _mainDisposables;
    private readonly SerialDisposable _sectionInfoDisposable;
    private readonly IList<(int section, PendingChange pendingChange)> _pendingChanges;
    private bool _isCollectingChanges;
    private IReadOnlyList<TSectionInfo> _sectionInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonReactiveSource{TSource, TUIView, TUIViewCell, TSectionInfo}"/> class.
    /// </summary>
    /// <param name="adapter">The adapter to use which we want to display information for.</param>
    public CommonReactiveSource(IUICollViewAdapter<TUIView, TUIViewCell> adapter)
    {
        _adapter = adapter;
        _mainThreadId = Environment.CurrentManagedThreadId;
        _mainDisposables = [];
        _sectionInfoDisposable = new SerialDisposable();
        _mainDisposables.Add(_sectionInfoDisposable);
        _pendingChanges = [];
        _sectionInfo = [];

        _mainDisposables.Add(
                             this
                                 .ObservableForProperty(
                                                        x => x.SectionInfo,
                                                        beforeChange: true,
                                                        skipInitial: true)
                                 .Subscribe(
                                            _ => SectionInfoChanging(),
                                            ex => this.Log().Error(ex, "Error occurred whilst SectionInfo changing.")));

        _mainDisposables.Add(
                             this
                                 .WhenAnyValue(x => x.SectionInfo)
                                 .Subscribe(
                                            SectionInfoChanged,
                                            ex => this.Log().Error(ex, "Error occurred when SectionInfo changed.")));
    }

    public IReadOnlyList<TSectionInfo> SectionInfo
    {
        get => _sectionInfo;
        set => this.RaiseAndSetIfChanged(ref _sectionInfo, value);
    }

    private bool IsDebugEnabled => this.Log().Level <= LogLevel.Debug;

    public int NumberOfSections()
    {
        Debug.Assert(Environment.CurrentManagedThreadId == _mainThreadId, "The thread is not the main thread.");

        var count = SectionInfo.Count;
        this.Log().Debug(CultureInfo.InvariantCulture, "Reporting number of sections = {0}", count);

        return count;
    }

    public int RowsInSection(int section)
    {
        Debug.Assert(Environment.CurrentManagedThreadId == _mainThreadId, "The thread is not the main thread.");

        var list = (IList)SectionInfo[section].Collection!;
        var count = list.Count;
        this.Log().Debug(CultureInfo.InvariantCulture, "Reporting rows in section {0} = {1}", section, count);

        return count;
    }

    public object? ItemAt(NSIndexPath path)
    {
        Debug.Assert(Environment.CurrentManagedThreadId == _mainThreadId, "The thread is not the main thread.");

        var list = (IList)SectionInfo[path.Section].Collection!;
        this.Log().Debug(CultureInfo.InvariantCulture, "Returning item at {0}-{1}", path.Section, path.Row);

        return list[path.Row];
    }

    public TUIViewCell GetCell(NSIndexPath indexPath)
    {
        Debug.Assert(Environment.CurrentManagedThreadId == _mainThreadId, "The thread is not the main thread.");

        this.Log().Debug(CultureInfo.InvariantCulture, "Getting cell for index path {0}-{1}", indexPath.Section, indexPath.Row);
        var section = SectionInfo[indexPath.Section];
        var vm = ((IList)section.Collection!)[indexPath.Row];
        var cell = _adapter.DequeueReusableCell(section?.CellKeySelector?.Invoke(vm) ?? NSString.Empty, indexPath);

        if (cell is IViewFor view)
        {
            this.Log().Debug(CultureInfo.InvariantCulture, "Setting VM for index path {0}-{1}", indexPath.Section, indexPath.Row);
            view.ViewModel = vm;
        }

        var initializeCellAction = section?.InitializeCellAction ?? (_ => { });
        initializeCellAction(cell);

        return cell;
    }

    public void Dispose()
    {
        _mainDisposables?.Dispose();
        _sectionInfoDisposable?.Dispose();
    }

    private static IEnumerable<Update> GetUpdatesForEvent(PendingChange pendingChange) =>
        pendingChange.Action switch
        {
            NotifyCollectionChangedAction.Add =>
                Enumerable
                    .Range(pendingChange.NewStartingIndex, pendingChange.NewItems is null ? 1 : pendingChange.NewItems.Count)
                    .Select(Update.CreateAdd),
            NotifyCollectionChangedAction.Remove =>
                Enumerable
                    .Range(pendingChange.OldStartingIndex, pendingChange.OldItems is null ? 1 : pendingChange.OldItems.Count)
                    .Select(_ => Update.CreateDelete(pendingChange.OldStartingIndex)),

            // Use OldStartingIndex for each "Update.Index" because the batch update processes and removes items sequentially
            // opposed to as one Range operation.
            // For example if we are removing the items from indexes 1 to 5.
            // When item at index 1 is removed item at index 2 is now at index 1 and so on down the line.
            NotifyCollectionChangedAction.Move =>
                Enumerable
                    .Range(pendingChange.OldStartingIndex, pendingChange.OldItems is null ? 1 : pendingChange.OldItems.Count)
                    .Select(Update.CreateDelete)
                    .Concat(
                            Enumerable
                                .Range(pendingChange.NewStartingIndex, pendingChange.NewItems is null ? 1 : pendingChange.NewItems.Count)
                                .Select(Update.CreateAdd)),
            NotifyCollectionChangedAction.Replace =>
                Enumerable
                    .Range(pendingChange.NewStartingIndex, pendingChange.NewItems is null ? 1 : pendingChange.NewItems.Count)
                    .SelectMany(x => new[] { Update.CreateDelete(x), Update.CreateAdd(x) }),
            _ => throw new NotSupportedException("Don't know how to deal with " + pendingChange.Action),
        };

    private void SectionInfoChanging()
    {
        VerifyOnMainThread();

        this.Log().Debug(CultureInfo.InvariantCulture, "SectionInfo about to change, disposing of any subscriptions for previous SectionInfo.");
        _sectionInfoDisposable.Disposable = null;
    }

    private void SectionInfoChanged(IReadOnlyList<TSectionInfo>? sectionInfo)
    {
        VerifyOnMainThread();

        // this ID just makes it possible to correlate log messages with a specific SectionInfo
        var sectionInfoId = SectionInfoIdGenerator.Generate();
        this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] SectionInfo changed to {1}.", sectionInfoId, sectionInfo);

        if (sectionInfo is null)
        {
            _sectionInfoDisposable.Disposable = null;
            return;
        }

        var notifyCollectionChanged = sectionInfo as INotifyCollectionChanged;

        if (notifyCollectionChanged is null)
        {
            this.Log().Warn(CultureInfo.InvariantCulture, "[#{0}] SectionInfo {1} does not implement INotifyCollectionChanged - any added or removed sections will not be reflected in the UI.", sectionInfoId, sectionInfo);
        }

        var sectionChanged = (notifyCollectionChanged is null ?
                                  Observable<Unit>.Never :
                                  notifyCollectionChanged.ObserveCollectionChanges().Select(_ => Unit.Default))
            .StartWith(Unit.Default);

        var disposables = new CompositeDisposable
        {
            Disposable.Create(() => this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Disposed of section info", sectionInfoId))
        };

        _sectionInfoDisposable.Disposable = disposables;
        SubscribeToSectionInfoChanges(sectionInfoId, sectionInfo, sectionChanged, disposables);
    }

    private void SubscribeToSectionInfoChanges(int sectionInfoId, IReadOnlyList<TSectionInfo> sectionInfo, IObservable<Unit> sectionChanged, CompositeDisposable disposables)
    {
        // holds a single disposable representing the monitoring of sectionInfo.
        // once disposed, any changes to sectionInfo will no longer trigger any of the logic below
        var sectionInfoDisposable = new SerialDisposable();
        disposables.Add(sectionInfoDisposable);

        disposables.Add(
                        sectionChanged.Subscribe(x =>
                        {
                            VerifyOnMainThread();

                            this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Calling ReloadData()", sectionInfoId);
                            _adapter.ReloadData();

                            // holds all the disposables created to monitor stuff inside the section
                            var sectionDisposables = new CompositeDisposable();
                            sectionInfoDisposable.Disposable = sectionDisposables;

                            // holds a single disposable for any outstanding request to apply pending changes
                            var applyPendingChangesDisposable = new SerialDisposable();
                            sectionDisposables.Add(applyPendingChangesDisposable);

                            var isReloading = _adapter
                                              .IsReloadingData
                                              .DistinctUntilChanged()
                                              .Do(y => this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] IsReloadingData = {1}", sectionInfoId, y))
                                              .Publish();

                            var anySectionChanged = sectionInfo
                                                    .Select((y, index) => y.Collection!.ObserveCollectionChanges().Select(z => new { Section = index, Change = z }))
                                                    .Merge()
                                                    .Publish();

                            // since reloads are applied asynchronously, it is possible for data to change whilst the reload is occurring
                            // thus, we need to ensure any such changes result in another reload
                            sectionDisposables.Add(
                                                   isReloading
                                                       .Where(y => y)
                                                       .Join(
                                                             anySectionChanged,
                                                             _ => isReloading,
                                                             _ => Observable<Unit>.Empty,
                                                             (_, __) => Unit.Default)
                                                       .Subscribe(_ =>
                                                       {
                                                           VerifyOnMainThread();
                                                           this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] A section changed whilst a reload is in progress - forcing another reload.", sectionInfoId);

                                                           _adapter.ReloadData();
                                                           _pendingChanges.Clear();
                                                           _isCollectingChanges = false;
                                                       }));

                            sectionDisposables.Add(
                                                   isReloading
                                                       .Where(y => !y)
                                                       .Join(
                                                             anySectionChanged,
                                                             _ => isReloading,
                                                             _ => Observable<Unit>.Empty,
                                                             (_, changeDetails) => (changeDetails.Change, changeDetails.Section))
                                                       .Subscribe(
                                                                  y =>
                                                                  {
                                                                      VerifyOnMainThread();

                                                                      if (IsDebugEnabled)
                                                                      {
                                                                          this.Log().Debug(
                                                                           CultureInfo.InvariantCulture,
                                                                           "[#{0}] Change detected in section {1} : Action = {2}, OldStartingIndex = {3}, NewStartingIndex = {4}, OldItems.Count = {5}, NewItems.Count = {6}",
                                                                           sectionInfoId,
                                                                           y.Section,
                                                                           y.Change.EventArgs.Action,
                                                                           y.Change.EventArgs.OldStartingIndex,
                                                                           y.Change.EventArgs.NewStartingIndex,
                                                                           y.Change.EventArgs.OldItems is null ? "null" : y.Change.EventArgs.OldItems.Count.ToString(CultureInfo.InvariantCulture),
                                                                           y.Change.EventArgs.NewItems is null ? "null" : y.Change.EventArgs.NewItems.Count.ToString(CultureInfo.InvariantCulture));
                                                                      }

                                                                      if (!_isCollectingChanges)
                                                                      {
                                                                          this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] A section changed whilst no reload is in progress - instigating collection of updates for later application.", sectionInfoId);
                                                                          _isCollectingChanges = true;

                                                                          // immediately indicate to the view that there are changes underway, even though we don't apply them immediately
                                                                          // this ensures that if application code itself calls BeginUpdates/EndUpdates on the view before the changes have been applied, those inconsistencies
                                                                          // between what's in the data source versus what the view believes is in the data source won't trigger any errors because of the outstanding
                                                                          // BeginUpdates call (calls to BeginUpdates/EndUpdates can be nested)
                                                                          this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] BeginUpdates", sectionInfoId);
                                                                          _adapter.BeginUpdates();

                                                                          applyPendingChangesDisposable.Disposable = RxApp.MainThreadScheduler.Schedule(
                                                                           () =>
                                                                           {
                                                                               ApplyPendingChanges(sectionInfoId);
                                                                               this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] EndUpdates", sectionInfoId);
                                                                               _adapter.EndUpdates();
                                                                               _isCollectingChanges = false;
                                                                               applyPendingChangesDisposable.Disposable = null;
                                                                           });
                                                                      }

                                                                      _pendingChanges.Add((y.Section, new PendingChange(y.Change.EventArgs)));
                                                                  },
                                                                  ex => this.Log().Error(CultureInfo.InvariantCulture, "[#{0}] Error while watching section collection: {1}", sectionInfoId, ex)));

                            sectionDisposables.Add(isReloading.Connect());
                            sectionDisposables.Add(anySectionChanged.Connect());
                        }));
    }

    private void ApplyPendingChanges(int sectionInfoId)
    {
        Debug.Assert(Environment.CurrentManagedThreadId == _mainThreadId, "The thread is not the main thread.");
        Debug.Assert(_isCollectingChanges, "Currently there are no changes to collect");
        this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Applying pending changes", sectionInfoId);

        try
        {
            _adapter.PerformUpdates(
                                    () =>
                                    {
                                        if (IsDebugEnabled)
                                        {
                                            this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] The pending changes (in order received) are:", sectionInfoId);

                                            foreach (var (section, pendingChange) in _pendingChanges)
                                            {
                                                this.Log().Debug(
                                                                 CultureInfo.InvariantCulture,
                                                                 "[#{0}] Section {1}: Action = {2}, OldStartingIndex = {3}, NewStartingIndex = {4}, OldItems.Count = {5}, NewItems.Count = {6}",
                                                                 sectionInfoId,
                                                                 section,
                                                                 pendingChange.Action,
                                                                 pendingChange.OldStartingIndex,
                                                                 pendingChange.NewStartingIndex,
                                                                 pendingChange.OldItems is null ? "null" : pendingChange.OldItems.Count.ToString(CultureInfo.InvariantCulture),
                                                                 pendingChange.NewItems is null ? "null" : pendingChange.NewItems.Count.ToString(CultureInfo.InvariantCulture));
                                            }
                                        }

                                        foreach (var sectionedUpdates in _pendingChanges.GroupBy(x => x.section))
                                        {
                                            var section = sectionedUpdates.First().section;

                                            this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Processing updates for section {1}", sectionInfoId, section);

                                            var allSectionChanges = sectionedUpdates
                                                                    .Select(x => x.pendingChange)
                                                                    .ToList();

                                            if (allSectionChanges.Any(x => x.Action == NotifyCollectionChangedAction.Reset))
                                            {
                                                this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Section {1} included a reset notification, so reloading that section.", sectionInfoId, section);
                                                _adapter.ReloadSections(new NSIndexSet((nuint)section));
                                                continue;
                                            }

                                            var updates = allSectionChanges
                                                          .SelectMany(GetUpdatesForEvent)
                                                          .ToList();
                                            var normalizedUpdates = IndexNormalizer.Normalize(updates);

                                            if (IsDebugEnabled)
                                            {
                                                this.Log().Debug(
                                                                 CultureInfo.InvariantCulture,
                                                                 "[#{0}] Updates for section {1}: {2}",
                                                                 sectionInfoId,
                                                                 section,
                                                                 string.Join(":", updates));

                                                this.Log().Debug(
                                                                 CultureInfo.InvariantCulture,
                                                                 "[#{0}] Normalized updates for section {1}: {2}",
                                                                 sectionInfoId,
                                                                 section,
                                                                 string.Join(":", normalizedUpdates));
                                            }

                                            foreach (var normalizedUpdate in normalizedUpdates)
                                            {
                                                switch (normalizedUpdate?.Type)
                                                {
                                                    case UpdateType.Add:
                                                        DoUpdate(_adapter.InsertItems, [normalizedUpdate.Index], section);
                                                        break;

                                                    case UpdateType.Delete:
                                                        DoUpdate(_adapter.DeleteItems, [normalizedUpdate.Index], section);
                                                        break;

                                                    default:
                                                        throw new NotSupportedException();
                                                }
                                            }
                                        }
                                    },
                                    () => this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Pending changes applied", sectionInfoId));
        }
        finally
        {
            _pendingChanges.Clear();
        }
    }

    private void DoUpdate(Action<NSIndexPath[]> method, IEnumerable<int> update, int section)
    {
        var toChange = update
                       .Select(x => NSIndexPath.FromRowSection(x, section))
                       .ToArray();

        if (IsDebugEnabled)
        {
            this.Log().Debug(
                             CultureInfo.InvariantCulture,
                             "Calling {0}: [{1}]",
                             method.Method.Name,
                             string.Join(",", toChange.Select(x => x.Section + "-" + x.Row)));
        }

        method(toChange);
    }

    private void VerifyOnMainThread()
    {
        if (Environment.CurrentManagedThreadId != _mainThreadId)
        {
            throw new InvalidOperationException("An operation has occurred off the main thread that must be performed on it. Be sure to schedule changes to the underlying data correctly.");
        }
    }

    // rather than storing NotifyCollectionChangeEventArgs instances, we store instances of this class instead
    // storing NotifyCollectionChangeEventArgs doesn't always work because external code can mutate the instance before we get a chance to apply it
    private sealed class PendingChange(NotifyCollectionChangedEventArgs ea)
    {
        public NotifyCollectionChangedAction Action { get; } = ea.Action;

        public IList? OldItems { get; } = ea.OldItems?.Cast<object>().ToList();

        public IList? NewItems { get; } = ea.NewItems?.Cast<object>().ToList();

        public int OldStartingIndex { get; } = ea.OldStartingIndex;

        public int NewStartingIndex { get; } = ea.NewStartingIndex;
    }
}
