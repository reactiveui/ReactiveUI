using System;
using System.Linq;
using System.Reactive.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading;
using System.Reactive.Subjects;
using System.Diagnostics;

namespace ReactiveUI.Cocoa
{
    public class CollectionViewSectionInformation
    {
        public IReactiveNotifyCollectionChanged Collection { get; protected set; }

        public string CellKey { get; protected set; }

        protected internal virtual void initializeCell(object cell)
        {
        }
    }

    public class CollectionViewSectionInformation<TCell> : CollectionViewSectionInformation
    {
        public Action<TCell> InitializeCellAction { get; protected set; }

        public CollectionViewSectionInformation(IReactiveNotifyCollectionChanged collection, string cellKey, Action<TCell> initializeCellAction = null)
        {
            Collection = collection;
            CellKey = cellKey;
            InitializeCellAction = initializeCellAction;
        }

        protected internal override void initializeCell(object cell)
        {
            if (InitializeCellAction == null)
                return;
            InitializeCellAction((TCell)cell);
        }
    }

    public class ReactiveCollectionViewSource : UICollectionViewSource, IEnableLogger, IDisposable
    {
        IDisposable innerDisp = Disposable.Empty;

        readonly UICollectionView collectionView;

        readonly List<CollectionViewSectionInformation> sectionInformation;

        readonly Subject<object> elementSelected = new Subject<object>();

        public ReactiveCollectionViewSource(UICollectionView collectionView, IReactiveNotifyCollectionChanged collection, string cellKey, Action<UICollectionViewCell> initializeCellAction = null)
            : this(collectionView, new[] { new CollectionViewSectionInformation<UICollectionViewCell>(collection, cellKey, initializeCellAction), })
        {
        }

        public ReactiveCollectionViewSource(UICollectionView collectionView, IEnumerable<CollectionViewSectionInformation> sectionInformation)
        {
            this.collectionView = collectionView;
            this.sectionInformation = sectionInformation.ToList();

            var compositeDisp = new CompositeDisposable();
            this.innerDisp = compositeDisp;

            for (int i = 0; i < this.sectionInformation.Count; i++)
            {
                var current = this.sectionInformation[i].Collection;

                var section = i;
                var disp = current.Changed.Buffer(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler).Subscribe(xs =>
                {
                    if (xs.Count == 0)
                        return;

                    this.Log().Info("Changed contents: [{0}]", String.Join(",", xs.Select(x => x.Action.ToString())));
                    if (xs.Any(x => x.Action == NotifyCollectionChangedAction.Reset))
                    {
                        this.Log().Info("About to call ReloadData");
                        collectionView.ReloadData();
                        return;
                    }

                    var updates = xs.Select(ea => Tuple.Create(ea, getChangedIndexes(ea)));
                    var allChangedIndexes = updates.SelectMany(u => u.Item2).ToList();
                    // Detect if we're changing the same cell more than 
                    // once - if so, issue a reset and be done
                    if (allChangedIndexes.Count != allChangedIndexes.Distinct().Count())
                    {
                        this.Log().Info("Detected a dupe in the changelist. Issuing Reset");
                        collectionView.ReloadData();
                        return;
                    }

                    this.Log().Info("Beginning update");
                    collectionView.PerformBatchUpdates(() =>
                    {
                        foreach (var update in updates.Reverse())
                        {
                            var changeAction = update.Item1.Action;
                            var changedIndexes = update.Item2;
                            switch (changeAction)
                            {
                                case NotifyCollectionChangedAction.Add:
                                    doUpdate(collectionView.InsertItems, changedIndexes, section);
                                    break;
                                case NotifyCollectionChangedAction.Remove:
                                    doUpdate(collectionView.DeleteItems, changedIndexes, section);
                                    break;
                                case NotifyCollectionChangedAction.Replace:
                                    doUpdate(collectionView.ReloadItems, changedIndexes, section);
                                    break;
                                case NotifyCollectionChangedAction.Move:
                                // NB: ReactiveList currently only supports single-item 
                                // moves
                                    var ea = update.Item1;
                                    this.Log().Info("Calling MoveRow: {0}-{1} => {0}{2}", section, ea.OldStartingIndex, ea.NewStartingIndex);
                                    collectionView.MoveItem(
                                        NSIndexPath.FromRowSection(ea.OldStartingIndex, section),
                                        NSIndexPath.FromRowSection(ea.NewStartingIndex, section));
                                    break;
                                default:
                                    this.Log().Info("Unknown Action: {0}", changeAction);
                                    break;
                            }
                        }

                        this.Log().Info("Ending update");
                    }, null);
                });

                compositeDisp.Add(disp);
            }
        }

        /// <summary>
        /// Gets an IObservable that is a hook to <see cref="ItemSelected"/> calls.
        /// </summary>
        public IObservable<object> ElementSelected
        {
            get { return elementSelected; }
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var sectionInfo = sectionInformation[indexPath.Section];
            var cellObject = collectionView.DequeueReusableCell((NSString)sectionInfo.CellKey, indexPath);
            var cell = cellObject as UICollectionViewCell;
            Debug.Assert(cell != null, "collectionView.DequeueReusableCell did not return a UICollectionViewCell");

            var view = (IViewFor)cell;
            if (view != null)
            {
                this.Log().Info("GetCell: Setting vm for Row: " + indexPath.Row);
                view.ViewModel = ((IList)sectionInfo.Collection)[indexPath.Row];
            }

            sectionInfo.initializeCell(cell);
            return cell;
        }

        public override int NumberOfSections(UICollectionView collectionView)
        {
            return sectionInformation.Count;
        }

        public override int GetItemsCount(UICollectionView collectionView, int section)
        {
            var list = (IList)(sectionInformation[section].Collection);
            this.Log().Info("RowsInSection: {0}-{1}", section, list.Count);
            return list.Count;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var sectionInfo = sectionInformation[indexPath.Section];
            var element = ((IList)sectionInfo.Collection)[indexPath.Row];
            elementSelected.OnNext(element);
        }

        public new void Dispose()
        {
            base.Dispose();

            var disp = Interlocked.Exchange(ref innerDisp, Disposable.Empty);
            disp.Dispose();
        }

        static IEnumerable<int> getChangedIndexes(NotifyCollectionChangedEventArgs ea)
        {
            switch (ea.Action)
            {
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
