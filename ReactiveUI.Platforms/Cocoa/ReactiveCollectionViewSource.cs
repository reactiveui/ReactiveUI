using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Collections.Specialized;

namespace ReactiveUI.Cocoa
{
    public class CollectionViewSectionInformation : ISectionInformation<UICollectionView, UICollectionViewCell>
    {
        public IReactiveNotifyCollectionChanged Collection { get; protected set; }
        public Action<UICollectionViewCell> InitializeCellAction { get; protected set; }
        public NSString CellKey { get; protected set; }
    }

    public class CollectionViewSectionInformation<TCell> : CollectionViewSectionInformation
        where TCell : UICollectionViewCell
    {
        public CollectionViewSectionInformation(IReactiveNotifyCollectionChanged collection, string cellKey, Action<TCell> initializeCellAction = null)
        {
            Collection = collection;
            CellKey = new NSString(cellKey);

            if (initializeCellAction != null) {
                InitializeCellAction = cell => initializeCellAction((TCell)cell);
            }
        }
    }

    class UICollectionViewAdapter : IUICollViewAdapter<UICollectionView, UICollectionViewCell>
    {
        readonly UICollectionView view;
        internal UICollectionViewAdapter(UICollectionView view) { this.view = view; }

        public void ReloadData() { view.ReloadData(); }
        public void PerformBatchUpdates(Action updates) { view.PerformBatchUpdates(new NSAction(updates), null); }
        public void InsertItems(NSIndexPath[] paths) { view.InsertItems(paths); }
        public void DeleteItems(NSIndexPath[] paths) { view.DeleteItems(paths); }
        public void ReloadItems(NSIndexPath[] paths) { view.ReloadItems(paths); }
        public void MoveItem(NSIndexPath path, NSIndexPath newPath) { view.MoveItem(path, newPath); }
        public UICollectionViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path)
        {
            return (UICollectionViewCell)view.DequeueReusableCell(cellKey, path);
        }
    }

    public class ReactiveCollectionViewSource : UICollectionViewSource, IEnableLogger, IDisposable
    {
        readonly CommonReactiveSource<UICollectionView, UICollectionViewCell> commonSource;
        readonly Subject<object> elementSelected = new Subject<object>();

        public ReactiveCollectionViewSource(UICollectionView collectionView, IReactiveNotifyCollectionChanged collection, string cellKey, Action<UICollectionViewCell> initializeCellAction = null)
            : this(collectionView, new[] { new CollectionViewSectionInformation<UICollectionViewCell>(collection, cellKey, initializeCellAction) })
        {
        }

        public ReactiveCollectionViewSource(UICollectionView collectionView, IReadOnlyList<CollectionViewSectionInformation> sectionInformation)
        {
            var adapter = new UICollectionViewAdapter(collectionView);
            this.commonSource = new CommonReactiveSource<UICollectionView, UICollectionViewCell>(adapter);
            this.commonSource.SectionInfo = sectionInformation;
        }

        /// <summary>
        /// Gets an IObservable that is a hook to <see cref="ItemSelected"/> calls.
        /// </summary>
        public IObservable<object> ElementSelected
        {
            get { return elementSelected; }
        }

        public IObservable<IEnumerable<NotifyCollectionChangedEventArgs>> DidPerformUpdates {
            get { return commonSource.DidPerformUpdates; }
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return commonSource.GetCell(indexPath);
        }

        public override int NumberOfSections(UICollectionView collectionView)
        {
            return commonSource.NumberOfSections();
        }

        public override int GetItemsCount(UICollectionView collectionView, int section)
        {
            return commonSource.RowsInSection(section);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            elementSelected.OnNext(commonSource.ItemAt(indexPath));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) commonSource.Dispose();
            base.Dispose(disposing);
        }
    }
}
