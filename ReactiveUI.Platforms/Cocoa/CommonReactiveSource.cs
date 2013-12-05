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
        IReadOnlyList<TSectionInfo> sectionInfo = null;

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

        public CommonReactiveSource(IUICollViewAdapter<TUIView, TUIViewCell> adapter) {
            this.adapter = adapter;

            mainDisp.Add(setupDisp);

            mainDisp.Add(this
                .WhenAnyValue(x => x.SectionInfo)
                .Subscribe(resetup, exc => this.Log().ErrorException("Error while watching for SectionInfo.", exc)));
        }

        public TUIViewCell GetCell(NSIndexPath indexPath)
        {
            var section = SectionInfo[indexPath.Section];
            var cell = adapter.DequeueReusableCell(section.CellKey, indexPath);
            var view = cell as IViewFor;

            if (view != null) {
                this.Log().Info("GetCell: Setting vm for Row: " + indexPath.Row);
                view.ViewModel = ((IList)section.Collection) [indexPath.Row];
            }

            (section.InitializeCellAction ?? (_ => {}))(cell);
            return cell;
        }

        public int NumberOfSections()
        {
            return SectionInfo.Count;
        }

        public int RowsInSection(int section)
        {
            var count = ((IList)SectionInfo[section].Collection).Count;
            this.Log().Info("RowsInSection: {0}-{1}", section, count);
            return count;
        }

        public object ItemAt(NSIndexPath path)
        {
            var list = (IList)SectionInfo[path.Section].Collection;
            return list[path.Row];
        }

        public void Dispose()
        {
            mainDisp.Dispose();
        }

        void resetup(IReadOnlyList<TSectionInfo> newSectionInfo) {
            UIApplication.EnsureUIThread();

            if (newSectionInfo == null) {
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
            var sectionChanged = reactiveSectionInfo == null ? Observable.Return(Unit.Default) : reactiveSectionInfo
                .Changed
                .Select(_ => Unit.Default)
                .StartWith(Unit.Default);

            if (reactiveSectionInfo == null) {
                this.Log().Warn("New section info does not implement IReactiveNotifyCollectionChanged.");
            }

            // Add section change listeners.  Always will run once right away
            // due to sectionChanged's construction.
            disp.Add(sectionChanged.Subscribe(_ => {
                UIApplication.EnsureUIThread();
                // TODO: Instead of listening to Changed events and then reseting,
                // we could listen to more specific events and avoid some reloads.
                var disp2 = new CompositeDisposable();
                subscrDisp.Disposable = disp2;

                for (int i = 0; i < newSectionInfo.Count; i++) {
                    var section = i;
                    var current = newSectionInfo[i].Collection;
                    disp2.Add(current
                        .Changed
                        .Buffer(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
                        .Subscribe(
                            xs => sectionCollectionChanged(section, xs),
                            ex => this.Log().ErrorException("Error while watching section " + i + "'s Collection.", ex)));
                }

                adapter.ReloadData();
            }));
        }

        void sectionCollectionChanged(int section, IList<NotifyCollectionChangedEventArgs> xs) {
            if (xs.Count == 0)
                return;

            var resetOnlyNotification = new [] {new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)};

            this.Log().Info("Changed contents: [{0}]", String.Join(",", xs.Select(x => x.Action.ToString())));

            if (xs.Any(x => x.Action == NotifyCollectionChangedAction.Reset)) {
                this.Log().Info("About to call ReloadData");
                adapter.ReloadData();

                didPerformUpdates.OnNext(resetOnlyNotification);
                return;
            }

            var updates = xs.Select(ea => Tuple.Create(ea, getChangedIndexes(ea))).ToList();
            var allChangedIndexes = updates.SelectMany(u => u.Item2).ToList();
            // Detect if we're changing the same cell more than
            // once - if so, issue a reset and be done
            
            if (allChangedIndexes.Count != allChangedIndexes.Distinct().Count()) {
                this.Log().Info("Detected a dupe in the changelist. Issuing Reset");
                adapter.ReloadData();

                didPerformUpdates.OnNext(resetOnlyNotification);
                return;
            }

            this.Log().Info("Beginning update");
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
                        this.Log().Info("Calling MoveRow: {0}-{1} => {0}{2}", section, ea.OldStartingIndex, ea.NewStartingIndex);

                        adapter.MoveItem(
                            NSIndexPath.FromRowSection(ea.OldStartingIndex, section),
                            NSIndexPath.FromRowSection(ea.NewStartingIndex, section));
                        break;
                    default:
                        this.Log().Info("Unknown Action: {0}", changeAction);
                        break;
                    }
                }

                this.Log().Info("Ending update");
                didPerformUpdates.OnNext(xs);
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

            this.Log().Info("Calling {0}: [{1}]", method.Method.Name,
                String.Join(",", toChange.Select(x => x.Section + "-" + x.Row)));

            method(toChange);
        }
    }
}

