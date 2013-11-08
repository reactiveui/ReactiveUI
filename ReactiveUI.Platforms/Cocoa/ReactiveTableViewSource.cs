using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace ReactiveUI.Cocoa
{
    public class TableSectionInformation : ISectionInformation<UITableView, UITableViewCell>
    {
        public IReactiveNotifyCollectionChanged Collection { get; protected set; }
        public Action<UITableViewCell> InitializeCellAction { get; protected set; }
        public NSString CellKey { get; protected set; }
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
    }

    public class TableSectionInformation<TCell> : TableSectionInformation
        where TCell : UITableViewCell
    {
        public TableSectionInformation(IReactiveNotifyCollectionChanged collection, string cellKey, float sizeHint, Action<TCell> initializeCellAction = null)
        {
            Collection = collection;
            CellKey = new NSString(cellKey);
            SizeHint = sizeHint;
            if (initializeCellAction != null)
                InitializeCellAction = cell => initializeCellAction((TCell)cell);
        }
    }

    class UITableViewAdapter : IUICollViewAdapter<UITableView, UITableViewCell>
    {
        readonly UITableView view;
        internal UITableViewAdapter(UITableView view) { this.view = view; }
        public void ReloadData() { view.ReloadData(); }
        public void PerformBatchUpdates(Action updates)
        {
            view.BeginUpdates();
            try { updates(); }
            finally { view.EndUpdates(); }
        }
        public void InsertItems(NSIndexPath[] paths) { view.InsertRows(paths, UITableViewRowAnimation.Automatic); }
        public void DeleteItems(NSIndexPath[] paths) { view.DeleteRows(paths, UITableViewRowAnimation.Automatic); }
        public void ReloadItems(NSIndexPath[] paths) { view.ReloadRows(paths, UITableViewRowAnimation.Automatic); }
        public void MoveItem(NSIndexPath path, NSIndexPath newPath) { view.MoveRow(path, newPath); }
        public UITableViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path)
        {
            return view.DequeueReusableCell(cellKey, path);
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
        readonly CommonReactiveSource<UITableView, UITableViewCell> commonSource;
        readonly Subject<object> elementSelected = new Subject<object>();

        public ReactiveTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged collection, string cellKey, float sizeHint, Action<UITableViewCell> initializeCellAction = null)
            : this (tableView, new[] { new TableSectionInformation<UITableViewCell>(collection, cellKey, sizeHint, initializeCellAction)})
        {
        }

        public ReactiveTableViewSource(UITableView tableView, IEnumerable<TableSectionInformation> sectionInformation)
        {
            var adapter = new UITableViewAdapter(tableView);
            this.commonSource = new CommonReactiveSource<UITableView, UITableViewCell>(adapter, sectionInformation);
        }

        /// <summary>
        /// Gets an IObservable that is a hook to <see cref="RowSelected"/> calls.
        /// </summary>
        public IObservable<object> ElementSelected {
            get { return elementSelected; }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return commonSource.GetCell(indexPath);
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return commonSource.NumberOfSections();
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return commonSource.RowsInSection(section);
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
            elementSelected.OnNext(commonSource.ItemAt(indexPath));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) commonSource.Dispose();
            base.Dispose(disposing);
        }

        TableSectionInformation GetSectionInfo(int section)
        {
            return (TableSectionInformation)commonSource.GetSectionInfo(section);
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return GetSectionInfo(indexPath.Section).SizeHint;
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            var header = GetSectionInfo(section).Header;
            return header == null ? 0 : header.Height;
        }

        public override float GetHeightForFooter(UITableView tableView, int section)
        {
            var footer = GetSectionInfo(section).Footer;
            return footer == null ? 0 : footer.Height;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            var header = GetSectionInfo(section).Header;
            return header == null ? null : header.View.Invoke();
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var footer = GetSectionInfo(section).Footer;
            return footer == null ? null : footer.View.Invoke();
        }
    }
}
