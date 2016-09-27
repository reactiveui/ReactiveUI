using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Splat;
using System.Reactive.Concurrency;
using System.Text;

#if UNIFIED
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// Interface used to extract a common API between <see cref="MonoTouch.UIKit.UITableView"/>
    /// and <see cref="MonoTouch.UIKit.UICollectionView"/>.
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

    /// <summary>
    /// Internal class containing the common code between <see cref="ReactiveTableViewSource"/>
    /// and <see cref="ReactiveCollectionViewSource"/>.
    /// </summary>
    sealed class CommonReactiveSource<TSource, TUIView, TUIViewCell, TSectionInfo> : ReactiveObject, IDisposable, IEnableLogger
        where TSectionInfo : ISectionInformation<TSource, TUIView, TUIViewCell>
    {
        /// <summary>
        /// Main disposable which is disposed when this object is disposed.
        /// </summary>
        readonly CompositeDisposable mainDisp = new CompositeDisposable();

        /// <summary>
        /// Disposable used by the setup procedure.
        /// </summary>
        readonly SerialDisposable setupDisp = new SerialDisposable();

        /// <summary>
        /// The adapter of the UIKit view.
        /// </summary>
        readonly IUICollViewAdapter<TUIView, TUIViewCell> adapter;

        /// <summary>
        /// Maps a section to the subscription for that section's data.
        /// </summary>
        IDictionary<TSectionInfo, IDisposable> sectionSubscriptions;

        /// <summary>
        /// Collects all changes to sections or items within a section until such time that they can be applied.
        /// </summary>
        IList<Tuple<int, NotifyCollectionChangedEventArgs>> pendingChanges;

        /// <summary>
        /// Tracks whether changes are currently being collected for later application.
        /// </summary>
        bool isCollectingChanges;

        /// <summary>
        /// Gets or sets the list of sections that this <see cref="CommonReactiveSource"/>
        /// should display.  Setting a new value always causes the table view to be reloaded.
        /// If the list implements <see cref="IReactiveNotifyCollectionChanged"/>,
        /// then the source will react to changes to the contents of the list as well.
        /// </summary>
        public IReadOnlyList<TSectionInfo> SectionInfo {
            get { return sectionInfo; }
            set { this.RaiseAndSetIfChanged(ref sectionInfo, value); }
        }

        /// <summary>
        /// Field backing <see cref="SectionInfo"/>.  Initialized to an empty
        /// array in order to avoid null reference exceptions if the source is
        /// used before a real section info has been set.
        /// </summary>
        IReadOnlyList<TSectionInfo> sectionInfo = new TSectionInfo[] {};

        /// <summary>
        /// IObservable that pushes a new value after the corresponding IUICollViewAdapter
        /// finishes processing changes from the underlying collection. Due to the buffered
        /// nature of the processing, the value is an IEnumerable of the changes.
        /// </summary>
        /// <value>An IEnumerable containing all the changes processed. Note that in
        /// some cases those can be different than the change events published by the
        /// underlying collection (for example the return value will contain a single
        /// Reset event arg even though the collection did not send a Reset, but the adapter
        /// performed a Reload nevertheless)</value>
        public IObservable<IEnumerable<NotifyCollectionChangedEventArgs>> DidPerformUpdates {
            get { return didPerformUpdates; }
        }

        readonly ISubject<IEnumerable<NotifyCollectionChangedEventArgs>> didPerformUpdates =
            new Subject<IEnumerable<NotifyCollectionChangedEventArgs>>();

        public CommonReactiveSource(IUICollViewAdapter<TUIView, TUIViewCell> adapter) 
        {
            this.adapter = adapter;
            this.sectionSubscriptions = new Dictionary<TSectionInfo, IDisposable>();
            this.pendingChanges = new List<Tuple<int, NotifyCollectionChangedEventArgs>>();

            mainDisp.Add(setupDisp);
            mainDisp.Add(this
                .ObservableForProperty(
                    x => x.SectionInfo,
                    true /* beforeChange */,
                    true /* skipInitial */)
                .Subscribe(
                    _ => DetachFromSectionInfo(),
                    ex => this.Log().ErrorException("Error while SectionInfo was changing.", ex)));

            mainDisp.Add(this
                .WhenAnyValue(x => x.SectionInfo)
                .Subscribe(AttachToSectionInfo, ex => this.Log().ErrorException("Error while watching for SectionInfo.", ex)));
        }

        public TUIViewCell GetCell(NSIndexPath indexPath) 
        {
            var section = SectionInfo[indexPath.Section];
            var vm = ((IList)section.Collection) [indexPath.Row];
            var cell = adapter.DequeueReusableCell(section.CellKeySelector(vm), indexPath);
            var view = cell as IViewFor;

            if (view != null) {
                this.Log().Debug("GetCell: Setting vm for Row: " + indexPath.Row);
                view.ViewModel = vm;
            }

            (section.InitializeCellAction ?? (_ => {}))(cell);
            return cell;
        }

        public int NumberOfSections() 
        {
            var count = SectionInfo.Count;
            this.Log().Debug(string.Format("NumberOfSections: {0} (from {1})", count, SectionInfo));

            return count;
        }

        public int RowsInSection(int section) 
        {
            var list = (IList)SectionInfo[section].Collection;
            var count = list.Count;
            this.Log().Debug(string.Format("RowsInSection: {0}-{1} (from {2} / {3})", section, count, SectionInfo, list));

            return count;
        }

        public object ItemAt(NSIndexPath path) 
        {
            var list = (IList)SectionInfo[path.Section].Collection;
            this.Log().Debug(string.Format("ItemAt: {0}.{1} (from {2} / {3})", path.Section, path.Row, SectionInfo, list));

            return list[path.Row];
        }

        public void Dispose() 
        {
            mainDisp.Dispose();
        }

        void DetachFromSectionInfo() 
        {
            // Dispose every binding.  Ensures that no matter what,
            // we won't let events from the old data reach us while
            // we morph into the new data.
            this.Log().Debug("SectionInfo about to change, disposing all bindings...");
            setupDisp.Disposable = Disposable.Empty;
        }

        void AttachToSectionInfo(IReadOnlyList<TSectionInfo> newSectionInfo) 
        {
            this.Log().Debug("SectionInfo changed to {0}, resetup data and bindings...", newSectionInfo);

            if (newSectionInfo == null) {
                this.Log().Debug("Null SectionInfo, done!");
                setupDisp.Disposable = Disposable.Empty;
                return;
            }

            // Disposable that holds garbage from this method.
            var disp = new CompositeDisposable();
            setupDisp.Disposable = disp;

            // Disposable that holds the subscriptions to individual sections.
            var subscrDisp = new SerialDisposable();
            disp.Add(subscrDisp);

            // Decide when we should check for section changes.
            var reactiveSectionInfo = newSectionInfo as IReactiveNotifyCollectionChanged<TSectionInfo>;

            var sectionChanging = reactiveSectionInfo == null ? Observable.Never<Unit>() : reactiveSectionInfo
                .Changing
                .Select(_ => Unit.Default);

            var sectionChanged = reactiveSectionInfo == null ? Observable.Return(Unit.Default) : reactiveSectionInfo
                .Changed
                .Select(_ => Unit.Default)
                .StartWith(Unit.Default);

            if (reactiveSectionInfo == null) {
                this.Log().Warn("New section info {0} does not implement IReactiveNotifyCollectionChanged.", newSectionInfo);
            }

            // NOTE: the implicit use of the immediate scheduler in various places below is intentional
            //       without it, iOS can interject its own logic amongst ours and therefore could see an inconsistent view of the data

            // Add section change listeners.  Always will run once right away
            // due to sectionChanged's construction.
            //
            // TODO: Instead of listening to Changed events and then reseting,
            // we could listen to more specific events and avoid some reloads.
            disp.Add(sectionChanging.Subscribe(_ => {
                // Dispose the old bindings.  Ensures that old events won't
                // arrive while we morph into the new data.
                this.Log().Debug("{0} is about to change, disposing section bindings...", newSectionInfo);
                subscrDisp.Disposable = Disposable.Empty;
            }));

            disp.Add(sectionChanged.Subscribe(x => {
                var disp2 = new CompositeDisposable();
                subscrDisp.Disposable = disp2;

                var sectionChangedWhilstNotReloadingList = new List<IObservable<NotifyCollectionChangedEventArgs>>();
                var sectionChangedList = new List<IObservable<NotifyCollectionChangedEventArgs>>();

                for (int i = 0; i < newSectionInfo.Count; i++)
                {
                    var section = i;
                    var current = newSectionInfo[i].Collection;
                    this.Log().Debug("Setting up section {0} binding...", section);

                    sectionChangedWhilstNotReloadingList.Add(adapter
                        .IsReloadingData
                        .DistinctUntilChanged()
                        .Do(y => this.Log().Debug("IsReloadingData: {0} (for section {1})", y, section))
                        .Select(y => y ? Observable.Empty<NotifyCollectionChangedEventArgs>() : current.Changed)
                        .Switch());
                    sectionChangedList.Add(current.Changed);
                }

                var anySectionChangedWhilstNotReloading = Observable.Merge(sectionChangedWhilstNotReloadingList);
                var anySectionChanged = Observable.Merge(sectionChangedList);

                disp2.Add(adapter
                    .IsReloadingData
                    .DistinctUntilChanged()
                    .Select(y => y ? anySectionChanged : Observable.Never<NotifyCollectionChangedEventArgs>())
                    .Switch()
                    .Subscribe(_ =>
                    {
                        adapter.ReloadData();
                        pendingChanges.Clear();
                    }));

                disp2.Add(anySectionChangedWhilstNotReloading
                    .Subscribe(_ =>
                    {
                        if (!isCollectingChanges)
                        {
                            isCollectingChanges = true;

                            // immediately indicate to the view that there are changes underway, even though we don't apply them immediately
                            // this ensures that if application code itself calls BeginUpdates/EndUpdates on the view before the changes have been applied, those inconsistencies
                            // between what's in the data source versus what the view believes is in the data source won't trigger any errors because of the outstanding
                            // BeginUpdates call (calls to BeginUpdates/EndUpdates can be nested)
                            adapter.BeginUpdates();

                            RxApp.MainThreadScheduler.Schedule(
                                () =>
                                {
                                    this.ApplyPendingChanges();
                                    adapter.EndUpdates();
                                });
                        }
                    }));

                for (var section = 0; section < sectionChangedWhilstNotReloadingList.Count; ++section)
                {
                    var sectionNum = section;
                    var specificSectionChanged = sectionChangedWhilstNotReloadingList[section];
                    disp2.Add(
                        specificSectionChanged
                            .Subscribe(
                                xs => this.pendingChanges.Add(Tuple.Create(sectionNum, xs)),
                                ex => this.Log().Error("Error while watching section {0}'s Collection: {1}", sectionNum, ex)));
                }

                this.Log().Debug("Done resetuping section data and bindings!");

                // Tell the view that the data needs to be reloaded.
                this.Log().Debug("Calling ReloadData()...", newSectionInfo);
                adapter.ReloadData();
                pendingChanges.Clear();
            }));

            this.Log().Debug("Done resetuping all bindings!");
        }

        private void ApplyPendingChanges()
        {
            try
            {
                List<NotifyCollectionChangedEventArgs> allEventArgs = new List<NotifyCollectionChangedEventArgs>();

                this.Log().Debug("Beginning update");
                adapter.PerformUpdates(() =>
                {
                    if (this.Log().Level >= LogLevel.Debug)
                    {
                        this.Log().Debug("The pending changes (in order received) are:");

                        foreach (var pendingChange in pendingChanges)
                        {
                            this.Log().Debug(
                                "Section {0}: Action={1}, OldStartingIndex={2}, NewStartingIndex={3}, OldItems.Count={4}, NewItems.Count={5}",
                                pendingChange.Item1,
                                pendingChange.Item2.Action,
                                pendingChange.Item2.OldStartingIndex,
                                pendingChange.Item2.NewStartingIndex,
                                pendingChange.Item2.OldItems == null ? "null" : pendingChange.Item2.OldItems.Count.ToString(),
                                pendingChange.Item2.NewItems == null ? "null" : pendingChange.Item2.NewItems.Count.ToString());
                        }
                    }

                    foreach (var sectionedUpdates in pendingChanges.GroupBy(x => x.Item1))
                    {
                        var section = sectionedUpdates.First().Item1;

                        this.Log().Debug("Processing updates for section {0}", section);

                        var allSectionEas = sectionedUpdates
                            .Select(x => x.Item2)
                            .ToList();

                        if (allSectionEas.Any(x => x.Action == NotifyCollectionChangedAction.Reset))
                        {
                            this.Log().Debug("Section {0} included a reset notification, so reloading that section.", section);
#if UNIFIED
                            adapter.ReloadSections(new NSIndexSet((nuint)section));
#else
                            adapter.ReloadSections(new NSIndexSet((uint)section));
#endif
                            continue;
                        }

                        var updates = allSectionEas
                            .SelectMany(GetUpdatesForEvent)
                            .ToList();

                        if (this.Log().Level >= LogLevel.Debug)
                        {
                            this.Log().Debug(
                                "Updates for section {0}: {1}",
                                section,
                                updates
                                    .Aggregate(
                                        new StringBuilder(),
                                        (current, next) =>
                                        {
                                            if (current.Length > 0)
                                            {
                                                current.Append(":");
                                            }

                                            return current.Append(next);
                                        },
                                        x => x.ToString()));
                        }

                        var normalizedUpdates = IndexNormalizer.Normalize(updates);

                        if (this.Log().Level >= LogLevel.Debug)
                        {
                            this.Log().Debug(
                                "Normalized updates for section {0}: {1}",
                                section,
                                normalizedUpdates
                                    .Aggregate(
                                        new StringBuilder(),
                                        (current, next) =>
                                        {
                                            if (current.Length > 0)
                                            {
                                                current.Append(":");
                                            }

                                            return current.Append(next);
                                        },
                                        x => x.ToString()));
                        }

                        foreach (var normalizedUpdate in normalizedUpdates)
                        {
                            switch (normalizedUpdate.Type)
                            {
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
                }, () =>
                {
                    this.Log().Debug("Ending update");
                    didPerformUpdates.OnNext(allEventArgs);
                });
            }
            finally
            {
                pendingChanges.Clear();
                isCollectingChanges = false;
            }
        }

        static IEnumerable<Update> GetUpdatesForEvent(NotifyCollectionChangedEventArgs ea)
        {
            switch (ea.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return Enumerable
                        .Range(ea.NewStartingIndex, ea.NewItems == null ? 1 : ea.NewItems.Count)
                        .Select(x => Update.CreateAdd(x));
                case NotifyCollectionChangedAction.Remove:
                    return Enumerable
                        .Range(ea.OldStartingIndex, ea.OldItems == null ? 1 : ea.OldItems.Count)
                        .Select(x => Update.CreateDelete(x));
                case NotifyCollectionChangedAction.Move:
                    return Enumerable
                        .Range(ea.OldStartingIndex, ea.OldItems == null ? 1 : ea.OldItems.Count)
                        .Select(x => Update.CreateDelete(x))
                        .Concat(
                            Enumerable
                                .Range(ea.NewStartingIndex, ea.NewItems == null ? 1 : ea.NewItems.Count)
                                .Select(x => Update.CreateAdd(x)));
                case NotifyCollectionChangedAction.Replace:
                    return Enumerable
                        .Range(ea.NewStartingIndex, ea.NewItems == null ? 1 : ea.NewItems.Count)
                        .SelectMany(x => new[] { Update.CreateDelete(x), Update.CreateAdd(x) });
                default:
                    throw new NotSupportedException("Don't know how to deal with " + ea.Action);
            }
        }

        void DoUpdate(Action<NSIndexPath[]> method, IEnumerable<int> update, int section)
        {
            var toChange = update
                 .Select(x => NSIndexPath.FromRowSection(x, section))
                 .ToArray();

            this.Log().Debug("Calling {0}: [{1}]", method.Method.Name,
                String.Join(",", toChange.Select(x => x.Section + "-" + x.Row)));

            method(toChange);
        }
    }
}

