using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Reactive.Subjects;
using Splat;

namespace ReactiveUI.Cocoa
{
    /// <summary>
    /// Interface used to extract a common API between <see cref="MonoTouch.UIKit.UITableView"/>
    /// and <see cref="MonoTouch.UIKit.UICollectionView"/>.
    /// </summary>
    interface IUICollViewAdapter<TUIView, TUIViewCell>
    {
        void ReloadData();
        void PerformBatchUpdates(Action updates);
        void InsertItems(NSIndexPath[] paths);
        void DeleteItems(NSIndexPath[] paths);
        void ReloadItems(NSIndexPath[] paths);
        void MoveItem(NSIndexPath path, NSIndexPath newPath);
        TUIViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path);
    }

    interface ISectionInformation<TUIView, TUIViewCell>
    {
        IReactiveNotifyCollectionChanged Collection { get; }
        NSString CellKey { get; }
        Action<TUIViewCell> InitializeCellAction { get; }
    }

    /// <summary>
    /// Internal class containing the common code between <see cref="ReactiveTableViewSource"/>
    /// and <see cref="ReactiveCollectionViewSource"/>.
    /// </summary>
    sealed class CommonReactiveSource<TUIView, TUIViewCell, TSectionInfo> : ReactiveObject, IDisposable, IEnableLogger
        where TSectionInfo : ISectionInformation<TUIView, TUIViewCell>
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

        // <summary>
        // These UI{Table,Collection}Views are tricky.  Calling ReloadData
        // won't immediately cause the data to be reloaded [1], contrary
        // to popular belief.  If we setup these bindings right here,
        // it is possible that *before* the table really reloads its data,
        // we'll already put some changes on the queue to be processed.
        // For example:
        //
        //  (a) ReloadData() is called.
        //  (b) A change on the source is buffered.
        //  (c) ReloadData() is processed.
        //  (d) The change is processed.
        //
        // The problem here is that step (d) assumes that (b) came after
        // (c), but it doesn't.  This leads the view into an inconsistent
        // state, probably triggering an iOS assertion.
        //
        // In order to avoid this problem, we setup the change bindings
        // only after the ReloadData() process has started.  And this
        // is where things get even trickier!  We don't have any means
        // to reliably know this!  Instead we assume that a call to
        // NumberOfSections() after a ReloadData() means that the
        // reload process has started.  Assuming that just one ReloadData()
        // is in flight, I don't know if there is any other way
        // NumberOfSections() would be called otherwise.
        //
        // And this is where we get to the last of our tricky problems:
        // it is possible to call ReloadData() multiple times before
        // the view gets the chance to process any of them [1]!
        //
        // So here's the current solution:
        //
        //  - Everytime NumberOfSections() is called,
        //    we save the time when it occurred.
        //
        //  - Everytime a collection is changed,
        //    we save the time it did so.
        //
        //  - When processing collection changes, we discard
        //    everything that comes *before* the latest call to
        //    NumberOfSections().
        //
        // Nasty!  Yes, I feel dirty!  But I'm not able to come up
        // with a scenario that breaks this code.
        //
        // Instead of using DateTime.Now, which has bad performance and
        // smells, we keep a counter that is incremented everytime
        // NumberOfSections() is called.  You can't reset your table
        // more than 2^64 times due to this solution, though.
        //
        // [1] http://stackoverflow.com/a/20115479
        // </summary>
        UInt64 lastCallToNumberOfSections = 0;

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

            mainDisp.Add(setupDisp);
            mainDisp.Add(this
                .ObservableForProperty(
                    x => x.SectionInfo,
                    true /* beforeChange */,
                    true /* skipInitial */)
                .Subscribe(
                    _ => unsetupAll(),
                    ex => this.Log().ErrorException("Error while SectionInfo was changing.", ex)));

            mainDisp.Add(this
                .WhenAnyValue(x => x.SectionInfo)
                .Subscribe(resetupAll, ex => this.Log().ErrorException("Error while watching for SectionInfo.", ex)));
        }

        public TUIViewCell GetCell(NSIndexPath indexPath) 
        {
            var section = SectionInfo[indexPath.Section];
            var cell = adapter.DequeueReusableCell(section.CellKey, indexPath);
            var view = cell as IViewFor;

            if (view != null) {
                this.Log().Debug("GetCell: Setting vm for Row: " + indexPath.Row);
                view.ViewModel = ((IList)section.Collection) [indexPath.Row];
            }

            (section.InitializeCellAction ?? (_ => {}))(cell);
            return cell;
        }

        public int NumberOfSections() 
        {
            var count = SectionInfo.Count;
            this.Log().Debug("NumberOfSections: {0} (from {1})", count, SectionInfo);
            this.lastCallToNumberOfSections++;

            return count;
        }

        public int RowsInSection(int section) 
        {
            var list = (IList)SectionInfo[section].Collection;
            var count = list.Count;
            this.Log().Debug("RowsInSection: {0}-{1} (from {2} / {3})", section, count, SectionInfo, list);

            return count;
        }

        public object ItemAt(NSIndexPath path) 
        {
            var list = (IList)SectionInfo[path.Section].Collection;
            this.Log().Debug("ItemAt: {0}.{1} (from {2} / {3})", path.Section, path.Row, SectionInfo, list);

            return list[path.Row];
        }

        public void Dispose() 
        {
            mainDisp.Dispose();
        }

        void unsetupAll() 
        {
            // Dispose every binding.  Ensures that no matter what,
            // we won't let events from the old data reach us while
            // we morph into the new data.
            this.Log().Debug("SectionInfo about to change, disposing all bindings...");
            setupDisp.Disposable = Disposable.Empty;
        }

        void resetupAll(IReadOnlyList<TSectionInfo> newSectionInfo) 
        {
            this.Log().Debug("SectionInfo changed to {0}, resetup data and bindings...", newSectionInfo);
            UIApplication.EnsureUIThread();

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
            var reactiveSectionInfo = newSectionInfo as IReactiveNotifyCollectionChanged;

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

            disp.Add(sectionChanged.Subscribe(_ => {
                this.Log().Debug("{0} is changed, resetup section data and bindings...", newSectionInfo);
                UIApplication.EnsureUIThread();

                var disp2 = new CompositeDisposable();
                subscrDisp.Disposable = disp2;

                for (int i = 0; i < newSectionInfo.Count; i++) {
                    var section = i;
                    var current = newSectionInfo[i].Collection;
                    this.Log().Debug("Setting up section {0} binding...", section);

                    disp2.Add(current
                        .Changed
                        .Select(timestamped)
                        .Buffer(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
                        .Subscribe(
                            xs => sectionCollectionChanged(section, xs),
                            ex => this.Log().ErrorException("Error while watching section " + i + "'s Collection.", ex)));
                }

                this.Log().Debug("Done resetuping section data and bindings!");

                // Tell the view that the data needs to be reloaded.
                this.Log().Debug("Calling ReloadData()...", newSectionInfo);
                adapter.ReloadData();
            }));

            this.Log().Debug("Done resetuping all bindings!");
        }

        void sectionCollectionChanged(int section, IList<Timestamped<NotifyCollectionChangedEventArgs>> teas) 
        {
            if (teas.Count == 0)
                return;

            var resetOnlyNotification = new [] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) };

            this.Log().Debug(
                "Changed contents: [{0}] (from {1})",
                String.Join(",", teas.Select(x => x.Value.Action.ToString())),
                SectionInfo);

            IList<NotifyCollectionChangedEventArgs> eas;
            if (teas[0].Timestamp < lastCallToNumberOfSections) {
                var toDiscard = teas.TakeWhile(x => x.Timestamp < lastCallToNumberOfSections).Count();
                this.Log().Warn("Ignoring {0} changes that ocurred before the last call to NumberOfSections() from {1}.", toDiscard, SectionInfo);

                if (toDiscard == teas.Count) {
                    this.Log().Warn("Nothing remains!");
                    return;
                }

                eas = teas.Skip(toDiscard).Select(x => x.Value).ToList();
            } else {
                eas = teas.Select(x => x.Value).ToList();
            }

            if (eas.Any(x => x.Action == NotifyCollectionChangedAction.Reset)) {
                this.Log().Debug("About to call ReloadData");
                adapter.ReloadData();

                didPerformUpdates.OnNext(resetOnlyNotification);
                return;
            }

            var updates = eas.Select(ea => Tuple.Create(ea, getChangedIndexes(ea))).ToList();
            var allChangedIndexes = updates.SelectMany(u => u.Item2).ToList();

            // Detect if we're changing the same cell more than
            // once - if so, issue a reset and be done
            if (allChangedIndexes.Count != allChangedIndexes.Distinct().Count()) {
                this.Log().Debug("Detected a dupe in the changelist. Issuing Reset");
                adapter.ReloadData();

                didPerformUpdates.OnNext(resetOnlyNotification);
                return;
            }

            this.Log().Debug("Beginning update");
            adapter.PerformBatchUpdates(() => {
                foreach (var update in updates.AsEnumerable().Reverse()) {
                    var changeAction = update.Item1.Action;
                    var changedIndexes = update.Item2;

                    switch (changeAction) {
                    case NotifyCollectionChangedAction.Add:
                        doUpdate(adapter.InsertItems, changedIndexes, section);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        doUpdate(adapter.DeleteItems, changedIndexes, section);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        doUpdate(adapter.ReloadItems, changedIndexes, section);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        // NB: ReactiveList currently only supports single-item
                        // moves
                        var ea = update.Item1;
                        this.Log().Debug("Calling MoveRow: {0}-{1} => {0}{2}", section, ea.OldStartingIndex, ea.NewStartingIndex);

                        adapter.MoveItem(
                            NSIndexPath.FromRowSection(ea.OldStartingIndex, section),
                            NSIndexPath.FromRowSection(ea.NewStartingIndex, section));
                        break;
                    default:
                        this.Log().Debug("Unknown Action: {0}", changeAction);
                        break;
                    }
                }

                this.Log().Debug("Ending update");
                didPerformUpdates.OnNext(eas);
            });
        }

        static IEnumerable<int> getChangedIndexes(NotifyCollectionChangedEventArgs ea)
        {
            switch (ea.Action) {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Replace:
                return Enumerable.Range(ea.NewStartingIndex, ea.NewItems != null ? ea.NewItems.Count : 1);
            case NotifyCollectionChangedAction.Move:
                return new[] { ea.OldStartingIndex, ea.NewStartingIndex };
            case NotifyCollectionChangedAction.Remove:
                return Enumerable.Range(ea.OldStartingIndex, ea.OldItems != null ? ea.OldItems.Count : 1);
            default:
                throw new ArgumentException("Don't know how to deal with " + ea.Action);
            }
        }

        void doUpdate(Action<NSIndexPath[]> method, IEnumerable<int> update, int section)
        {
            var toChange = update
                 .Select(x => NSIndexPath.FromRowSection(x, section))
                 .ToArray();

            this.Log().Debug("Calling {0}: [{1}]", method.Method.Name,
                String.Join(",", toChange.Select(x => x.Section + "-" + x.Row)));

            method(toChange);
        }

        Timestamped<T> timestamped<T>(T newValue) 
        {
            return new Timestamped<T>(this.lastCallToNumberOfSections, newValue);
        }

        class Timestamped<T> 
        {
            public readonly UInt64 Timestamp;
            public readonly T Value;

            public Timestamped(UInt64 timestamp, T newValue) 
            {
                Timestamp = timestamp;
                Value = newValue;
            }
        }
    }
}

