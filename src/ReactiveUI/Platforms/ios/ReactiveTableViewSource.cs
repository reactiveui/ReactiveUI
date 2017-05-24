using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using Splat;
using UIKit;

namespace ReactiveUI
{
    public class TableSectionInformation<TSource> : ISectionInformation<TSource, UITableView, UITableViewCell>
    {
        public IReactiveNotifyCollectionChanged<TSource> Collection { get; protected set; }
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

    public class TableSectionInformation<TSource, TCell> : TableSectionInformation<TSource>
        where TCell : UITableViewCell
    {
        public TableSectionInformation(IReactiveNotifyCollectionChanged<TSource> collection, Func<object, NSString> cellKeySelector, float sizeHint, Action<TCell> initializeCellAction = null)
        {
            Collection = collection;
            SizeHint = sizeHint;
            CellKeySelector = cellKeySelector;
            if (initializeCellAction != null)
                InitializeCellAction = cell => initializeCellAction((TCell)cell);
        }

        public TableSectionInformation(IReactiveNotifyCollectionChanged<TSource> collection, NSString cellKey, float sizeHint, Action<TCell> initializeCellAction = null)
            : this(collection, _ => cellKey, sizeHint, initializeCellAction)
        {
        }
    }

    class UITableViewAdapter : IUICollViewAdapter<UITableView, UITableViewCell>
    {
        readonly UITableView view;
        readonly BehaviorSubject<bool> isReloadingData;
        int inFlightReloads;

        internal UITableViewAdapter(UITableView view)
        {
            this.view = view;
            this.isReloadingData = new BehaviorSubject<bool>(false);
        }

        public IObservable<bool> IsReloadingData {
            get { return this.isReloadingData.AsObservable(); }
        }

        public void ReloadData()
        {
            ++inFlightReloads;
            view.ReloadData();

            if (inFlightReloads == 1) {
                Debug.Assert(!this.isReloadingData.Value);
                this.isReloadingData.OnNext(true);
            }

            // since ReloadData() queues the appropriate messages on the UI thread, we know we're done reloading
            // when this subsequent message is processed (with one caveat - see FinishReloadData for details)
            RxApp.MainThreadScheduler.Schedule(FinishReloadData);
        }

        public void BeginUpdates()
        {
            view.BeginUpdates();
        }

        public void PerformUpdates(Action updates, Action completion)
        {
            view.BeginUpdates();
            try {
                updates();
            } finally {
                view.EndUpdates();
                completion();
            }
        }

        public void EndUpdates()
        {
            view.EndUpdates();
        }

        public void InsertSections(NSIndexSet indexes) { view.InsertSections(indexes, UITableViewRowAnimation.Automatic); }
        public void DeleteSections(NSIndexSet indexes) { view.DeleteSections(indexes, UITableViewRowAnimation.Automatic); }
        public void ReloadSections(NSIndexSet indexes) { view.ReloadSections(indexes, UITableViewRowAnimation.Automatic); }
        public void MoveSection(int fromIndex, int toIndex) { view.MoveSection(fromIndex, toIndex); }
        public void InsertItems(NSIndexPath[] paths) { view.InsertRows(paths, UITableViewRowAnimation.Automatic); }
        public void DeleteItems(NSIndexPath[] paths) { view.DeleteRows(paths, UITableViewRowAnimation.Automatic); }
        public void ReloadItems(NSIndexPath[] paths) { view.ReloadRows(paths, UITableViewRowAnimation.Automatic); }
        public void MoveItem(NSIndexPath path, NSIndexPath newPath) { view.MoveRow(path, newPath); }

        public UITableViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path)
        {
            return view.DequeueReusableCell(cellKey, path);
        }

        void FinishReloadData()
        {
            --inFlightReloads;

            if (inFlightReloads == 0) {
                // this is required because sometimes iOS schedules further work that results in calls to GetCell
                // that work could happen after FinishReloadData unless we force layout here
                // of course, we can't have that work running after IsReloading ticks to false because otherwise
                // some updates may occur before the calls to GetCell and thus the calls to GetCell could fail due to invalid indexes
                this.view.LayoutIfNeeded();
                Debug.Assert(this.isReloadingData.Value);
                this.isReloadingData.OnNext(false);
            }
        }
    }

    /// <summary>
    /// A header or footer of a table section.
    /// </summary>
    public class TableSectionHeader
    {
        /// <summary>
        /// Gets the function that creates the <see cref="UIView"/>
        /// used as header for this section. Overrides Title
        /// </summary>
        public Func<UIView> View { get; protected set; }

        /// <summary>
        /// Gets the height of the header.
        /// </summary>
        public float Height { get; protected set; }

        /// <summary>
        /// Gets the title for the section header, only used if View is null.
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableSectionHeader"/>
        /// struct.
        /// </summary>
        /// <param name="view">Function that creates header's <see cref="UIView"/>.</param>
        /// <param name="height">Height of the header.</param>
        public TableSectionHeader(Func<UIView> view, float height)
        {
            this.View = view;
            this.Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveUI.Cocoa.TableSectionHeader"/> class.
        /// </summary>
        /// <param name="title">Title to use.</param>
        public TableSectionHeader(string title)
        {
            this.Title = title;
        }
    }

    /// <summary>
    /// ReactiveTableViewSource is a Table View Source that is connected to 
    /// a ReactiveList that automatically updates the View based on the 
    /// contents of the list. The collection changes are buffered and View 
    /// items are animated in and out as items are added.
    /// </summary>
    public class ReactiveTableViewSource<TSource> : UITableViewSource, IEnableLogger, IDisposable, IReactiveNotifyPropertyChanged<ReactiveTableViewSource<TSource>>, IHandleObservableErrors, IReactiveObject
    {
        readonly CommonReactiveSource<TSource, UITableView, UITableViewCell, TableSectionInformation<TSource>> commonSource;
        readonly Subject<object> elementSelected = new Subject<object>();

        public ReactiveTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<TSource> collection, NSString cellKey, float sizeHint, Action<UITableViewCell> initializeCellAction = null)
            : this(tableView)
        {
            this.Data = new[] { new TableSectionInformation<TSource, UITableViewCell>(collection, cellKey, sizeHint, initializeCellAction) };
        }

        [Obsolete("Please bind your view model to the Data property.")]
        public ReactiveTableViewSource(UITableView tableView, IReadOnlyList<TableSectionInformation<TSource>> sectionInformation)
            : this(tableView)
        {
            this.Data = sectionInformation;
        }

        public ReactiveTableViewSource(UITableView tableView)
        {
            setupRxObj();
            var adapter = new UITableViewAdapter(tableView);
            this.commonSource = new CommonReactiveSource<TSource, UITableView, UITableViewCell, TableSectionInformation<TSource>>(adapter);
        }

        /// <summary>
        /// Gets or sets the data that should be displayed by this
        /// <see cref="ReactiveTableViewSource"/>.  You should
        /// probably bind your view model to this property.
        /// If the list implements <see cref="IReactiveNotifyCollectionChanged"/>,
        /// then the source will react to changes to the contents of the list as well.
        /// </summary>
        /// <value>The data.</value>
        public IReadOnlyList<TableSectionInformation<TSource>> Data {
            get { return commonSource.SectionInfo; }
            set {
                if (commonSource.SectionInfo == value) return;

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

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return commonSource.GetCell(indexPath);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return commonSource.NumberOfSections();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            // iOS may call this method even when we have no sections, but only if we've overridden
            // EstimatedHeight(UITableView, NSIndexPath) in our UITableViewSource
            if (section >= commonSource.NumberOfSections()) {
                return 0;
            }

            return commonSource.RowsInSection((int)section);
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

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return commonSource.SectionInfo[indexPath.Section].SizeHint;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            // iOS may call this method even when we have no sections, but only if we've overridden
            // EstimatedHeight(UITableView, NSIndexPath) in our UITableViewSource
            if (section >= commonSource.NumberOfSections()) {
                return 0;
            }

            var header = commonSource.SectionInfo[(int)section].Header;

            // NB: -1 is a magic # that causes iOS to use the regular height. go figure.
            return header == null || header.View == null ? -1 : header.Height;
        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            // iOS may call this method even when we have no sections, but only if we've overridden
            // EstimatedHeight(UITableView, NSIndexPath) in our UITableViewSource
            if (section >= commonSource.NumberOfSections()) {
                return 0;
            }

            var footer = commonSource.SectionInfo[(int)section].Footer;
            return footer == null || footer.View == null ? -1 : footer.Height;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            var header = commonSource.SectionInfo[(int)section].Header;
            return header == null || header.Title == null ? null : header.Title;
        }

        public override string TitleForFooter(UITableView tableView, nint section)
        {
            var footer = commonSource.SectionInfo[(int)section].Footer;
            return footer == null || footer.Title == null ? null : footer.Title;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var header = commonSource.SectionInfo[(int)section].Header;
            return header == null || header.View == null ? null : header.View.Invoke();
        }

        public override UIView GetViewForFooter(UITableView tableView, nint section)
        {
            var footer = commonSource.SectionInfo[(int)section].Footer;
            return footer == null || footer.View == null ? null : footer.View.Invoke();
        }

        public object ItemAt(NSIndexPath indexPath)
        {
            return commonSource.ItemAt(indexPath);
        }

        public event PropertyChangingEventHandler PropertyChanging {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewSource<TSource>>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewSource<TSource>>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        void setupRxObj()
        {
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
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<IReadOnlyList<TableSectionInformation<TSource, TCell>>> sectionsObservable,
            UITableView tableView,
            Func<ReactiveTableViewSource<TSource>, IDisposable> initSource = null)
            where TCell : UITableViewCell
        {
            var source = new ReactiveTableViewSource<TSource>(tableView);
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
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            UITableView tableView,
            NSString cellKey,
            float sizeHint,
            Action<TCell> initializeCellAction = null,
            Func<ReactiveTableViewSource<TSource>, IDisposable> initSource = null)
            where TCell : UITableViewCell
        {
            return sourceObservable
                .Select(src => new[] {
                    new TableSectionInformation<TSource, TCell>(
                        src,
                        cellKey,
                        sizeHint,
                        initializeCellAction),
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
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            UITableView tableView,
            float sizeHint,
            Action<TCell> initializeCellAction = null,
            Func<ReactiveTableViewSource<TSource>, IDisposable> initSource = null)
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
