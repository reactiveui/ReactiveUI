using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using Splat;

namespace ReactiveUI.Cocoa
{
    public class TableSectionInformation : ISectionInformation<UITableView, UITableViewCell>
    {
        public IReactiveNotifyCollectionChanged Collection { get; protected set; }
        public Action<UITableViewCell> InitializeCellAction { get; protected set; }
        public Func<object, NSString> CellKeySelector { get; protected set; }
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
        public TableSectionInformation(IReactiveNotifyCollectionChanged collection, Func<object, NSString> cellKeySelector, float sizeHint, Action<TCell>initializeCellAction = null)
        {
            Collection = collection;
            SizeHint = sizeHint;
            CellKeySelector = cellKeySelector;
            if (initializeCellAction != null)
                InitializeCellAction = cell => initializeCellAction((TCell)cell);
        }

        public TableSectionInformation(IReactiveNotifyCollectionChanged collection, NSString cellKey, float sizeHint, Action<TCell> initializeCellAction = null)
            : this(collection, _ => cellKey, sizeHint, initializeCellAction)
        {
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

    public class ReactiveTableViewSource : UITableViewSource, IEnableLogger, IDisposable, IReactiveNotifyPropertyChanged, IHandleObservableErrors, IReactiveObjectExtension
    {
        readonly CommonReactiveSource<UITableView, UITableViewCell, TableSectionInformation> commonSource;
        readonly Subject<object> elementSelected = new Subject<object>();

        public ReactiveTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged collection, NSString cellKey, float sizeHint, Action<UITableViewCell> initializeCellAction = null)
            : this(tableView) {
            this.Data = new[] { new TableSectionInformation<UITableViewCell>(collection, cellKey, sizeHint, initializeCellAction)};
        }

        [Obsolete("Please bind your view model to the Data property.")]
        public ReactiveTableViewSource(UITableView tableView, IReadOnlyList<TableSectionInformation> sectionInformation)
            : this(tableView) {
            this.Data = sectionInformation;
        }

        public ReactiveTableViewSource(UITableView tableView) {
            setupRxObj();
            var adapter = new UITableViewAdapter(tableView);
            this.commonSource = new CommonReactiveSource<UITableView, UITableViewCell, TableSectionInformation>(adapter);
        }

        /// <summary>
        /// Gets or sets the data that should be displayed by this
        /// <see cref="ReactiveTableViewSource"/>.  You should
        /// probably bind your view model to this property.
        /// If the list implements <see cref="IReactiveNotifyCollectionChanged"/>,
        /// then the source will react to changes to the contents of the list as well.
        /// </summary>
        /// <value>The data.</value>
        public IReadOnlyList<TableSectionInformation> Data {
            get { return commonSource.SectionInfo; }
            set {
                if (commonSource.SectionInfo == value)  return;

                this.raisePropertyChanging("Data");
                commonSource.SectionInfo = value;
                this.raisePropertyChanged("Data");
            }
        }

        /// <summary>
        /// Gets an IObservable that is a hook to <see cref="RowSelected"/> calls.
        /// </summary>
        public IObservable<object> ElementSelected {
            get { return elementSelected; }
        }

        public IObservable<IEnumerable<NotifyCollectionChangedEventArgs>> DidPerformUpdates {
            get { return commonSource.DidPerformUpdates; }
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

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return commonSource.SectionInfo[indexPath.Section].SizeHint;
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            var header = commonSource.SectionInfo[section].Header;
            return header == null ? 0 : header.Height;
        }

        public override float GetHeightForFooter(UITableView tableView, int section)
        {
            var footer = commonSource.SectionInfo[section].Footer;
            return footer == null ? 0 : footer.Height;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            var header = commonSource.SectionInfo[section].Header;
            return header == null ? null : header.View.Invoke();
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var footer = commonSource.SectionInfo[section].Footer;
            return footer == null ? null : footer.View.Invoke();
        }

        public object ItemAt(NSIndexPath indexPath)
        {
            return commonSource.ItemAt(indexPath);
        }

        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObjectExtension.RaisePropertyChanging(PropertyChangingEventArgs args) 
	{
            var handler = PropertyChanging;
            if (handler != null) {
                handler(this, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void IReactiveObjectExtension.RaisePropertyChanged(PropertyChangedEventArgs args) 
	{
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, args);
            }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IObservedChange<object, object>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IObservedChange<object, object>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        void setupRxObj()
        {
            this.setupReactiveExtension();
        }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            return this.suppressChangeNotifications();
        }
    }

    /// <summary>
    /// Extension methods for <see cref="ReactiveTableViewSource"/>.
    /// </summary>
    public static class ReactiveTableViewSourceExtensions
    {
        /// <summary>
        /// <para>Extension method that binds an observable of a list of table
        /// sections as the source of a <see cref="UITableView"/>.</para>
        /// <para>If your <see cref="IReadOnlyList"/> is also an instance of
        /// <see cref="IReactiveNotifyCollectionChanged"/>, then this method
        /// will silently update the bindings whenever it changes as well.
        /// Otherwise, it will just log a message.</para>
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sectionsObservable">Sections observable.</param>
        /// <param name="tableView">Table view.</param>
        /// <param name="initSource">Optionally initializes some property of
        /// the <see cref="ReactiveTableViewSource"/>.</param>
        /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
        public static IDisposable BindTo<TCell>(
            this IObservable<IReadOnlyList<TableSectionInformation<TCell>>> sectionsObservable,
            UITableView tableView,
            Func<ReactiveTableViewSource, IDisposable> initSource = null)
            where TCell : UITableViewCell
        {
            var source = new ReactiveTableViewSource(tableView);
            if (initSource != null) initSource(source);
            var bind = sectionsObservable.BindTo(source, x => x.Data);
            tableView.Source = source;
            return new CompositeDisposable(bind, source);
        }

        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UITableView"/>.
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sourceObservable">Source collection observable.</param>
        /// <param name="tableView">Table view.</param>
        /// <param name="cellKey">Cell key.</param>
        /// <param name="sizeHint">Size hint.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>
        /// <param name="initSource">Optionally initializes some property of
        /// the <see cref="ReactiveTableViewSource"/>.</param>
        /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
        public static IDisposable BindTo<TCell>(
            this IObservable<IReactiveNotifyCollectionChanged> sourceObservable,
            UITableView tableView,
            NSString cellKey,
            float sizeHint,
            Action<TCell> initializeCellAction = null,
            Func<ReactiveTableViewSource, IDisposable> initSource = null)
            where TCell : UITableViewCell
        {
            return sourceObservable
                .Select(
                    src => new[]
                    {
                        new TableSectionInformation<TCell>(
                            src,
                            cellKey,
                            sizeHint,
                            initializeCellAction)
                    })
                .BindTo(tableView, initSource);
        }

        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UITableView"/>.  Also registers
        /// the given class with an unspecified cellKey (you should probably
        /// not specify any other cellKeys).
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sourceObservable">Source collection observable.</param>
        /// <param name="tableView">Table view.</param>
        /// <param name="sizeHint">Size hint.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>
        /// <param name="initSource">Optionally initializes some property of
        /// the <see cref="ReactiveTableViewSource"/>.</param>
        /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
        public static IDisposable BindTo<TCell>(
            this IObservable<IReactiveNotifyCollectionChanged> sourceObservable,
            UITableView tableView,
            float sizeHint,
            Action<TCell> initializeCellAction = null,
            Func<ReactiveTableViewSource, IDisposable> initSource = null)
            where TCell : UITableViewCell
        {
            var type = typeof(TCell);
            var cellKey = new NSString(type.ToString());
            tableView.RegisterClassForCellReuse(type, new NSString(cellKey));
            return sourceObservable
                .BindTo(tableView, cellKey, sizeHint, initializeCellAction, initSource);
        }
    }
}
