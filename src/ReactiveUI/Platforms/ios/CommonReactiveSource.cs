using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Foundation;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Interface used to extract a common API between <see cref="UIKit.UITableView"/>
    /// and <see cref="UIKit.UICollectionView"/>.
    /// </summary>
    interface IUICollViewAdapter<TUIView, TUIViewCell>
    {
        IObservable<bool> IsReloadingData { get; }
        void ReloadData();
        void BeginUpdates();
        void PerformUpdates(Action updates, Action completion);
        void EndUpdates();
        void InsertSections(NSIndexSet indexes);
        void DeleteSections(NSIndexSet indexes);
        void ReloadSections(NSIndexSet indexes);
        void MoveSection(int fromIndex, int toIndex);
        void InsertItems(NSIndexPath[] paths);
        void DeleteItems(NSIndexPath[] paths);
        void ReloadItems(NSIndexPath[] paths);
        void MoveItem(NSIndexPath path, NSIndexPath newPath);
        TUIViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path);
    }

    interface ISectionInformation<TSource, TUIView, TUIViewCell>
    {
        IReactiveNotifyCollectionChanged<TSource> Collection { get; }
        Func<object, NSString> CellKeySelector { get; }
        Action<TUIViewCell> InitializeCellAction { get; }
    }

    internal static class SectionInfoIdGenerator
    {
        private static int nextSectionInfoId;

        public static int Generate()
        {
            return nextSectionInfoId++;
        }
    }

    sealed class CommonReactiveSource<TSource, TUIView, TUIViewCell, TSectionInfo> : ReactiveObject, IDisposable, IEnableLogger
        where TSectionInfo : ISectionInformation<TSource, TUIView, TUIViewCell>
    {
        private readonly IUICollViewAdapter<TUIView, TUIViewCell> adapter;
        private readonly int mainThreadId;
        private readonly CompositeDisposable mainDisposables;
        private readonly SerialDisposable sectionInfoDisposable;
        private readonly IList<Tuple<int, PendingChange>> pendingChanges;
        private bool isCollectingChanges;
        private IReadOnlyList<TSectionInfo> sectionInfo;

        public CommonReactiveSource(IUICollViewAdapter<TUIView, TUIViewCell> adapter)
        {
            this.adapter = adapter;
            this.mainThreadId = Thread.CurrentThread.ManagedThreadId;
            this.mainDisposables = new CompositeDisposable();
            this.sectionInfoDisposable = new SerialDisposable();
            this.mainDisposables.Add(this.sectionInfoDisposable);
            this.pendingChanges = new List<Tuple<int, PendingChange>>();
            this.sectionInfo = new TSectionInfo[0];

            this.mainDisposables.Add(
                this
                .ObservableForProperty(
                    x => x.SectionInfo,
                    beforeChange: true,
                    skipInitial: true)
                .Subscribe(
                    _ => this.SectionInfoChanging(),
                    ex => this.Log().ErrorException("Error occurred whilst SectionInfo changing.", ex)));

            this.mainDisposables.Add(
                this
                .WhenAnyValue(x => x.SectionInfo)
                .Subscribe(
                    this.SectionInfoChanged,
                    ex => this.Log().ErrorException("Error occurred when SectionInfo changed.", ex)));
        }

        private bool IsDebugEnabled {
            get { return this.Log().Level <= LogLevel.Debug; }
        }

        public IReadOnlyList<TSectionInfo> SectionInfo {
            get { return sectionInfo; }
            set { this.RaiseAndSetIfChanged(ref sectionInfo, value); }
        }

        public int NumberOfSections()
        {
            Debug.Assert(Thread.CurrentThread.ManagedThreadId == this.mainThreadId);

            var count = SectionInfo.Count;
            this.Log().Debug("Reporting number of sections = {0}", count);

            return count;
        }

        public int RowsInSection(int section)
        {
            Debug.Assert(Thread.CurrentThread.ManagedThreadId == this.mainThreadId);

            var list = (IList)SectionInfo[section].Collection;
            var count = list.Count;
            this.Log().Debug("Reporting rows in section {0} = {1}", section, count);

            return count;
        }

        public object ItemAt(NSIndexPath path)
        {
            Debug.Assert(Thread.CurrentThread.ManagedThreadId == this.mainThreadId);

            var list = (IList)SectionInfo[path.Section].Collection;
            this.Log().Debug("Returning item at {0}-{1}", path.Section, path.Row);

            return list[path.Row];
        }

        public TUIViewCell GetCell(NSIndexPath indexPath)
        {
            Debug.Assert(Thread.CurrentThread.ManagedThreadId == this.mainThreadId);

            this.Log().Debug("Getting cell for index path {0}-{1}", indexPath.Section, indexPath.Row);
            var section = SectionInfo[indexPath.Section];
            var vm = ((IList)section.Collection)[indexPath.Row];
            var cell = adapter.DequeueReusableCell(section.CellKeySelector(vm), indexPath);
            var view = cell as IViewFor;

            if (view != null) {
                this.Log().Debug("Setting VM for index path {0}-{1}", indexPath.Section, indexPath.Row);
                view.ViewModel = vm;
            }

            var initializeCellAction = section.InitializeCellAction ?? (_ => { });
            initializeCellAction(cell);

            return cell;
        }

        public void Dispose()
        {
            this.mainDisposables.Dispose();
        }

        private void SectionInfoChanging()
        {
            this.VerifyOnMainThread();

            this.Log().Debug("SectionInfo about to change, disposing of any subscriptions for previous SectionInfo.");
            this.sectionInfoDisposable.Disposable = null;
        }

        private void SectionInfoChanged(IReadOnlyList<TSectionInfo> sectionInfo)
        {
            this.VerifyOnMainThread();

            // this ID just makes it possible to correlate log messages with a specific SectionInfo
            var sectionInfoId = SectionInfoIdGenerator.Generate();
            this.Log().Debug("[#{0}] SectionInfo changed to {1}.", sectionInfoId, sectionInfo);

            if (sectionInfo == null) {
                this.sectionInfoDisposable.Disposable = null;
                return;
            }

            var reactiveSectionInfo = sectionInfo as IReactiveNotifyCollectionChanged<TSectionInfo>;

            if (reactiveSectionInfo == null) {
                this.Log().Warn("[#{0}] SectionInfo {1} does not implement IReactiveNotifyCollectionChanged - any added or removed sections will not be reflected in the UI.", sectionInfoId, sectionInfo);
            }

            var sectionChanging = reactiveSectionInfo == null
                ? Observable<Unit>.Never
                : reactiveSectionInfo
                .Changing
                .Select(_ => Unit.Default);

            var sectionChanged =
                (reactiveSectionInfo == null
                    ? Observable<Unit>.Never
                    : reactiveSectionInfo
                    .Changed
                    .Select(_ => Unit.Default))
                    .StartWith(Unit.Default);

            var disposables = new CompositeDisposable();
            disposables.Add(Disposable.Create(() => this.Log().Debug("[#{0}] Disposed of section info", sectionInfoId)));
            this.sectionInfoDisposable.Disposable = disposables;
            this.SubscribeToSectionInfoChanges(sectionInfoId, sectionInfo, sectionChanging, sectionChanged, disposables);
        }

        private void SubscribeToSectionInfoChanges(int sectionInfoId, IReadOnlyList<TSectionInfo> sectionInfo, IObservable<Unit> sectionChanging, IObservable<Unit> sectionChanged, CompositeDisposable disposables)
        {
            // holds a single disposable representing the monitoring of sectionInfo.
            // once disposed, any changes to sectionInfo will no longer trigger any of the logic below
            var sectionInfoDisposable = new SerialDisposable();
            disposables.Add(sectionInfoDisposable);

            disposables.Add(
                sectionChanging.Subscribe(_ => {
                    this.VerifyOnMainThread();

                    this.Log().Debug("[#{0}] SectionInfo is changing.", sectionInfoId);
                    sectionInfoDisposable.Disposable = Disposable.Empty;
                }));

            disposables.Add(
                sectionChanged.Subscribe(x => {
                    this.VerifyOnMainThread();

                    this.Log().Debug("[#{0}] Calling ReloadData()", sectionInfoId);
                    this.adapter.ReloadData();

                    // holds all the disposables created to monitor stuff inside the section
                    var sectionDisposables = new CompositeDisposable();
                    sectionInfoDisposable.Disposable = sectionDisposables;

                    // holds a single disposable for any outstanding request to apply pending changes
                    var applyPendingChangesDisposable = new SerialDisposable();
                    sectionDisposables.Add(applyPendingChangesDisposable);

                    var isReloading = adapter
                        .IsReloadingData
                        .DistinctUntilChanged()
                        .Do(y => this.Log().Debug("[#{0}] IsReloadingData = {1}", sectionInfoId, y))
                        .Publish();

                    var anySectionChanged = Observable
                        .Merge(
                            sectionInfo
                            .Select((y, index) => y.Collection.Changed.Select(z => new { Section = index, Change = z })))
                        .Publish();

                    // since reloads are applied asynchronously, it is possible for data to change whilst the reload is occuring
                    // thus, we need to ensure any such changes result in another reload
                    sectionDisposables.Add(
                        isReloading
                        .Where(y => y)
                        .Join(
                            anySectionChanged,
                            _ => isReloading,
                            _ => Observable<Unit>.Empty,
                            (_, __) => Unit.Default)
                        .Subscribe(_ => {
                            this.VerifyOnMainThread();
                            this.Log().Debug("[#{0}] A section changed whilst a reload is in progress - forcing another reload.", sectionInfoId);

                            adapter.ReloadData();
                            this.pendingChanges.Clear();
                            isCollectingChanges = false;
                        }));

                    sectionDisposables.Add(
                        isReloading
                        .Where(y => !y)
                        .Join(
                            anySectionChanged,
                            _ => isReloading,
                            _ => Observable<Unit>.Empty,
                            (reloading, changeDetails) => new { Change = changeDetails.Change, Section = changeDetails.Section })
                        .Subscribe(y => {
                            this.VerifyOnMainThread();

                            if (this.IsDebugEnabled) {
                                this.Log().Debug(
                                        "[#{0}] Change detected in section {1} : Action = {2}, OldStartingIndex = {3}, NewStartingIndex = {4}, OldItems.Count = {5}, NewItems.Count = {6}",
                                        sectionInfoId,
                                        y.Section,
                                        y.Change.Action,
                                        y.Change.OldStartingIndex,
                                        y.Change.NewStartingIndex,
                                        y.Change.OldItems == null ? "null" : y.Change.OldItems.Count.ToString(),
                                        y.Change.NewItems == null ? "null" : y.Change.NewItems.Count.ToString());
                            }

                            if (!this.isCollectingChanges) {
                                this.Log().Debug("[#{0}] A section changed whilst no reload is in progress - instigating collection of updates for later application.", sectionInfoId);
                                this.isCollectingChanges = true;

                                // immediately indicate to the view that there are changes underway, even though we don't apply them immediately
                                // this ensures that if application code itself calls BeginUpdates/EndUpdates on the view before the changes have been applied, those inconsistencies
                                // between what's in the data source versus what the view believes is in the data source won't trigger any errors because of the outstanding
                                // BeginUpdates call (calls to BeginUpdates/EndUpdates can be nested)
                                this.Log().Debug("[#{0}] BeginUpdates", sectionInfoId);
                                adapter.BeginUpdates();

                                applyPendingChangesDisposable.Disposable = RxApp.MainThreadScheduler.Schedule(
                                        () => {
                                            this.ApplyPendingChanges(sectionInfoId);
                                            this.Log().Debug("[#{0}] EndUpdates", sectionInfoId);
                                            adapter.EndUpdates();
                                            isCollectingChanges = false;
                                            applyPendingChangesDisposable.Disposable = null;
                                        });
                            }

                            this.pendingChanges.Add(Tuple.Create(y.Section, new PendingChange(y.Change)));
                        },
                            ex => this.Log().Error("[#{0}] Error while watching section collection: {1}", sectionInfoId, ex)));

                    sectionDisposables.Add(isReloading.Connect());
                    sectionDisposables.Add(anySectionChanged.Connect());
                }));
        }

        private void ApplyPendingChanges(int sectionInfoId)
        {
            Debug.Assert(Thread.CurrentThread.ManagedThreadId == this.mainThreadId);
            Debug.Assert(this.isCollectingChanges);
            this.Log().Debug("[#{0}] Applying pending changes", sectionInfoId);

            try {
                adapter.PerformUpdates(
                    () => {
                        if (this.IsDebugEnabled) {
                            this.Log().Debug("[#{0}] The pending changes (in order received) are:", sectionInfoId);

                            foreach (var pendingChange in pendingChanges) {
                                this.Log().Debug(
                                    "[#{0}] Section {1}: Action = {2}, OldStartingIndex = {3}, NewStartingIndex = {4}, OldItems.Count = {5}, NewItems.Count = {6}",
                                    sectionInfoId,
                                    pendingChange.Item1,
                                    pendingChange.Item2.Action,
                                    pendingChange.Item2.OldStartingIndex,
                                    pendingChange.Item2.NewStartingIndex,
                                    pendingChange.Item2.OldItems == null ? "null" : pendingChange.Item2.OldItems.Count.ToString(),
                                    pendingChange.Item2.NewItems == null ? "null" : pendingChange.Item2.NewItems.Count.ToString());
                            }
                        }

                        foreach (var sectionedUpdates in pendingChanges.GroupBy(x => x.Item1)) {
                            var section = sectionedUpdates.First().Item1;

                            this.Log().Debug("[#{0}] Processing updates for section {1}", sectionInfoId, section);

                            var allSectionChanges = sectionedUpdates
                                .Select(x => x.Item2)
                                .ToList();

                            if (allSectionChanges.Any(x => x.Action == NotifyCollectionChangedAction.Reset)) {
                                this.Log().Debug("[#{0}] Section {1} included a reset notification, so reloading that section.", sectionInfoId, section);
                                adapter.ReloadSections(new NSIndexSet((nuint)section));
                                continue;
                            }

                            var updates = allSectionChanges
                                .SelectMany(GetUpdatesForEvent)
                                .ToList();
                            var normalizedUpdates = IndexNormalizer.Normalize(updates);

                            if (this.IsDebugEnabled) {
                                this.Log().Debug(
                                    "[#{0}] Updates for section {1}: {2}",
                                    sectionInfoId,
                                    section,
                                    string.Join(":", updates));

                                this.Log().Debug(
                                    "[#{0}] Normalized updates for section {1}: {2}",
                                    sectionInfoId,
                                    section,
                                    string.Join(":", normalizedUpdates));
                            }

                            foreach (var normalizedUpdate in normalizedUpdates) {
                                switch (normalizedUpdate.Type) {
                                    case UpdateType.Add:
                                        DoUpdate(adapter.InsertItems, new[] { normalizedUpdate.Index }, section);
                                        break;
                                    case UpdateType.Delete:
                                        DoUpdate(adapter.DeleteItems, new[] { normalizedUpdate.Index }, section);
                                        break;
                                    default:
                                        throw new NotSupportedException();
                                }
                            }
                        }
                    },
                    () => this.Log().Debug("[#{0}] Pending changes applied", sectionInfoId));
            } finally {
                pendingChanges.Clear();
            }
        }

        private static IEnumerable<Update> GetUpdatesForEvent(PendingChange pendingChange)
        {
            switch (pendingChange.Action) {
                case NotifyCollectionChangedAction.Add:
                    return Enumerable
                        .Range(pendingChange.NewStartingIndex, pendingChange.NewItems == null ? 1 : pendingChange.NewItems.Count)
                        .Select(x => Update.CreateAdd(x));
                case NotifyCollectionChangedAction.Remove:
                    return Enumerable
                        .Range(pendingChange.OldStartingIndex, pendingChange.OldItems == null ? 1 : pendingChange.OldItems.Count)
                        .Select(x => Update.CreateDelete(x));
                case NotifyCollectionChangedAction.Move:
                    return Enumerable
                        .Range(pendingChange.OldStartingIndex, pendingChange.OldItems == null ? 1 : pendingChange.OldItems.Count)
                        .Select(x => Update.CreateDelete(x))
                        .Concat(
                            Enumerable
                            .Range(pendingChange.NewStartingIndex, pendingChange.NewItems == null ? 1 : pendingChange.NewItems.Count)
                            .Select(x => Update.CreateAdd(x)));
                case NotifyCollectionChangedAction.Replace:
                    return Enumerable
                        .Range(pendingChange.NewStartingIndex, pendingChange.NewItems == null ? 1 : pendingChange.NewItems.Count)
                        .SelectMany(x => new[] { Update.CreateDelete(x), Update.CreateAdd(x) });
                default:
                    throw new NotSupportedException("Don't know how to deal with " + pendingChange.Action);
            }
        }

        private void DoUpdate(Action<NSIndexPath[]> method, IEnumerable<int> update, int section)
        {
            var toChange = update
                .Select(x => NSIndexPath.FromRowSection(x, section))
                .ToArray();

            if (this.IsDebugEnabled) {
                this.Log().Debug(
                    "Calling {0}: [{1}]",
                    method.Method.Name,
                    string.Join(",", toChange.Select(x => x.Section + "-" + x.Row)));
            }

            method(toChange);
        }

        private void VerifyOnMainThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != this.mainThreadId) {
                throw new InvalidOperationException("An operation has occurred off the main thread that must be performed on it. Be sure to schedule changes to the underlying data correctly.");
            }
        }

        // rather than storing NotifyCollectionChangeEventArgs instances, we store instances of this class instead
        // storing NotifyCollectionChangeEventArgs doesn't always work because external code can mutate the instance before we get a chance to apply it
        private sealed class PendingChange
        {
            private readonly NotifyCollectionChangedAction action;
            private readonly IList oldItems;
            private readonly IList newItems;
            private readonly int oldStartingIndex;
            private readonly int newStartingIndex;

            public PendingChange(NotifyCollectionChangedEventArgs ea)
            {
                this.action = ea.Action;
                this.oldItems = ea.OldItems == null ? null : ea.OldItems.Cast<object>().ToList();
                this.newItems = ea.NewItems == null ? null : ea.NewItems.Cast<object>().ToList();
                this.oldStartingIndex = ea.OldStartingIndex;
                this.newStartingIndex = ea.NewStartingIndex;
            }

            public NotifyCollectionChangedAction Action {
                get { return this.action; }
            }

            public IList OldItems {
                get { return this.oldItems; }
            }

            public IList NewItems {
                get { return this.newItems; }
            }

            public int OldStartingIndex {
                get { return this.oldStartingIndex; }
            }

            public int NewStartingIndex {
                get { return this.newStartingIndex; }
            }
        }
    }
}

