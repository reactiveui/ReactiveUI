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

namespace ReactiveUI.Cocoa
{
    public class TableSectionInformation
    {
        public IReactiveNotifyCollectionChanged Collection { get; protected set; }
        public string CellKey { get; protected set; }
        public float SizeHint { get; protected set; }

        /// <summary>
        /// Gets or sets the header of this section.
        /// </summary>
        /// <value>The header, or null if a header shouldn't be used.</value>
        public TableSectionHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the footer of this section.
        /// </summary>
        /// <value>The footer, or null if a footer shouldn't be used.</value>
        public TableSectionHeader Footer { get; set; }

        protected internal virtual void initializeCell(object cell) { }
    }

    public class TableSectionInformation<TCell> : TableSectionInformation
    {
        public Action<TCell> InitializeCellAction { get; protected set; }

        public TableSectionInformation(IReactiveNotifyCollectionChanged collection, string cellKey, float sizeHint, Action<TCell> initializeCellAction = null)
        {
            Collection = collection;
            CellKey = cellKey;
            SizeHint = sizeHint;
            InitializeCellAction = initializeCellAction;
        }

        protected internal override void initializeCell(object cell)
        {
            if (InitializeCellAction == null) return;
            InitializeCellAction((TCell)cell);
        }
    }

    /// <summary>
    /// A header or footer of a table section.
    /// </summary>
    public class TableSectionHeader
    {
        /// <summary>
        /// Gets the function that creates the <see cref="UIView"/>
        /// used as header for this section.
        /// </summary>
        public Func<UIView> View { get; protected set; }

        /// <summary>
        /// Gets the height of the header.
        /// </summary>
        public float Height { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableSectionHeader"/>
        /// struct.
        /// </summary>
        /// <param name="view">Function that creates header's <see cref="UIView"/>.</param>
        /// <param name="height">Height of the header.</param>
        public TableSectionHeader (Func<UIView> view, float height)
        {
            this.View = view;
            this.Height = height;
        }
    }

    public class ReactiveTableViewSource : UITableViewSource, IEnableLogger, IDisposable
    {
        IDisposable innerDisp = Disposable.Empty;

        readonly UITableView tableView;
        readonly List<TableSectionInformation> sectionInformation;
        readonly Subject<object> elementSelected = new Subject<object>();

        bool tableViewReloadInProgress = false;

        public ReactiveTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged collection, string cellKey, float sizeHint, Action<UITableViewCell> initializeCellAction = null)
            : this (tableView, new[] { new TableSectionInformation<UITableViewCell>(collection, cellKey, sizeHint, initializeCellAction), })
        {
        }

        public ReactiveTableViewSource(UITableView tableView, IEnumerable<TableSectionInformation> sectionInformation)
        {
            this.tableView = tableView;
            this.sectionInformation = sectionInformation.ToList();

            var compositeDisp = new CompositeDisposable();
            this.innerDisp = compositeDisp;

            for (int i=0; i < this.sectionInformation.Count; i++) {
                var current = this.sectionInformation[i].Collection;

                var section = i;
                var disp = current.Changed.Buffer(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler).Subscribe(xs => {
                    if (xs.Count == 0) return;

                    this.Log().Info("Changed contents: [{0}]", String.Join(",", xs.Select(x => x.Action.ToString())));
                    if (xs.Any(x => x.Action == NotifyCollectionChangedAction.Reset)) {
                        this.Log().Info("About to call ReloadData");
                        tableView.ReloadData();
                        return;
                    }

                    int prevItem = -1;
                    var changedIndexes = xs.SelectMany(x => getChangedIndexes(x)).ToList();
                    changedIndexes.Sort();
                    for(int j=0; j < changedIndexes.Count; j++) {
                        // Detect if we're changing the same cell more than 
                        // once - if so, issue a reset and be done
                        if (prevItem == changedIndexes[j] && j > 0) {
                            this.Log().Info("Detected a dupe in the changelist. Issuing Reset");
                            tableView.ReloadData();
                            return;
                        }

                        prevItem = changedIndexes[j];
                    }

                    this.Log().Info("Beginning update");
                    tableView.BeginUpdates();

                    var toChange = default(NSIndexPath[]);
                    foreach(var update in xs.Reverse()) {
                        switch(update.Action) {
                        case NotifyCollectionChangedAction.Add:
                            toChange = Enumerable.Range(update.NewStartingIndex, update.NewItems != null ? update.NewItems.Count : 1)
                                .Select(x => NSIndexPath.FromRowSection(x, section))
                                .ToArray();
                            this.Log().Info("Calling InsertRows: [{0}]", String.Join(",", toChange.Select(x => x.Section + "-" + x.Row)));
                            tableView.InsertRows(toChange, UITableViewRowAnimation.Automatic);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            toChange = Enumerable.Range(update.OldStartingIndex, update.OldItems != null ? update.OldItems.Count : 1)
                                .Select(x => NSIndexPath.FromRowSection(x, section))
                                .ToArray();
                            this.Log().Info("Calling DeleteRows: [{0}]", String.Join(",", toChange.Select(x => x.Section + "-" + x.Row)));
                            tableView.DeleteRows(toChange, UITableViewRowAnimation.Automatic);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            toChange = Enumerable.Range(update.NewStartingIndex, update.NewItems != null ? update.NewItems.Count : 1)
                                .Select(x => NSIndexPath.FromRowSection(x, section))
                                .ToArray();
                            this.Log().Info("Calling ReloadRows: [{0}]", String.Join(",", toChange.Select(x => x.Section + "-" + x.Row)));
                            tableView.ReloadRows(toChange, UITableViewRowAnimation.Automatic);
                            break;
                        case NotifyCollectionChangedAction.Move:
                            // NB: ReactiveList currently only supports single-item 
                            // moves
                            this.Log().Info("Calling MoveRow: {0}-{1} => {0}{2}", section, update.OldStartingIndex, update.NewStartingIndex);
                            tableView.MoveRow(
                                NSIndexPath.FromRowSection(update.OldStartingIndex, section),
                                NSIndexPath.FromRowSection(update.NewStartingIndex, section));
                            break;
                        default:
                            this.Log().Info("Unknown Action: {0}", update.Action);
                            break;
                        }
                    }

                    this.Log().Info("Ending update");
                    tableView.EndUpdates();
                });

                compositeDisp.Add(disp);
            }
        }

        /// <summary>
        /// Gets an IObservable that is a hook to <see cref="RowSelected"/> calls.
        /// </summary>
        public IObservable<object> ElementSelected {
            get { return elementSelected; }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var sectionInfo = sectionInformation[indexPath.Section];
            var cell = tableView.DequeueReusableCell(sectionInfo.CellKey);

            var view = (IViewFor)cell;
            if (view != null) {
                this.Log().Info("GetCell: Setting vm for Row: " + indexPath.Row);
                view.ViewModel = ((IList)sectionInfo.Collection)[indexPath.Row];
            }

            sectionInfo.initializeCell(cell);
            tableViewReloadInProgress = false;
            return cell;
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return sectionInformation[indexPath.Section].SizeHint;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return sectionInformation.Count;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            var list = (IList)(sectionInformation[section].Collection);
            this.Log().Info("RowsInSection: {0}-{1}", section, list.Count);
            return list.Count;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return false;
        }

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return false;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
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

        IEnumerable<int> getChangedIndexes(NotifyCollectionChangedEventArgs ea)
        {
            switch (ea.Action) {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Replace:
                return Enumerable.Range(ea.NewStartingIndex, ea.NewItems.Count);
            case NotifyCollectionChangedAction.Move:
                return new[] { ea.OldStartingIndex, ea.NewStartingIndex };
            case NotifyCollectionChangedAction.Remove:
                return Enumerable.Range(ea.OldStartingIndex, ea.OldItems.Count);
            default:
                throw new ArgumentException("Don't know how to deal with " + ea.Action);
            }
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            var header = sectionInformation[section].Header;
            return header == null ? 0 : header.Height;
        }

        public override float GetHeightForFooter(UITableView tableView, int section)
        {
            var footer = sectionInformation[section].Footer;
            return footer == null ? 0 : footer.Height;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            var header = sectionInformation[section].Header;
            return header == null ? null : header.View.Invoke();
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var footer = sectionInformation[section].Footer;
            return footer == null ? null : footer.View.Invoke();
        }
    }
}
