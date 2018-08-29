// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using Android.Support.V7.Widget;
using Android.Views;
using DynamicData;
using DynamicData.Binding;

namespace ReactiveUI.AndroidSupport
{
    /// <summary>
    /// An adapter for the Android <see cref="RecyclerView"/>.
    /// Override the <see cref="RecyclerView.Adapter.CreateViewHolder(ViewGroup, int)"/> method
    /// to create the your <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> based ViewHolder.
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
    public abstract class ReactiveRecyclerViewAdapter<TViewModel> : RecyclerView.Adapter
        where TViewModel : class, IReactiveObject
    {
        private readonly ISourceList<TViewModel> _list;

        private IDisposable _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveRecyclerViewAdapter{TViewModel}"/> class.
        /// </summary>
        /// <param name="backingList">The backing list.</param>
        protected ReactiveRecyclerViewAdapter(IObservable<IChangeSet<TViewModel>> backingList)
        {
            _list = new SourceList<TViewModel>(backingList);

            _inner = _list
                            .Connect()
                            .ForEachChange(UpdateBindings)
                            .Subscribe();
        }

        /// <inheritdoc/>
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((IViewFor)holder).ViewModel = _list.Items.ElementAt(position);
        }

        /// <inheritdoc/>
        public override int ItemCount => _list.Count;

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
        }

        private void UpdateBindings(Change<TViewModel> change)
        {
            switch (change.Reason)
            {
            case ListChangeReason.Add:
                NotifyItemInserted(change.Item.CurrentIndex);
                break;
            case ListChangeReason.Remove:
                NotifyItemRemoved(change.Item.CurrentIndex);
                break;
            case ListChangeReason.Moved:
                NotifyItemMoved(change.Item.PreviousIndex, change.Item.CurrentIndex);
                break;
            case ListChangeReason.Replace:
            case ListChangeReason.Refresh:
                NotifyItemChanged(change.Item.CurrentIndex);
                break;
            case ListChangeReason.AddRange:
                NotifyItemRangeInserted(change.Range.Index, change.Range.Count);
                break;
            case ListChangeReason.RemoveRange:
            case ListChangeReason.Clear:
                NotifyItemRangeRemoved(change.Range.Index, change.Range.Count);
                break;
            }
        }
    }

    /// <summary>
    /// An adapter for the Android <see cref="RecyclerView"/>.
    /// Override the <see cref="RecyclerView.Adapter.CreateViewHolder(ViewGroup, int)"/> method
    /// to create the your <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> based ViewHolder.
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
    /// <typeparam name="TCollection">The type of collection.</typeparam>
    public abstract class ReactiveRecyclerViewAdapter<TViewModel, TCollection> : ReactiveRecyclerViewAdapter<TViewModel>
        where TViewModel : class, IReactiveObject
        where TCollection : ICollection<TViewModel>, INotifyCollectionChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveRecyclerViewAdapter{TViewModel, TCollection}"/> class.
        /// </summary>
        /// <param name="backingList">The backing list.</param>
        protected ReactiveRecyclerViewAdapter(TCollection backingList)
            : base(backingList.ToObservableChangeSet<TCollection, TViewModel>())
        {
        }
    }

    /// <summary>
    /// A <see cref="RecyclerView.ViewHolder"/> implementation that binds to a reactive view model.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public class ReactiveRecyclerViewViewHolder<TViewModel> : RecyclerView.ViewHolder, ILayoutViewHost, IViewFor<TViewModel>, IReactiveNotifyPropertyChanged<ReactiveRecyclerViewViewHolder<TViewModel>>, IReactiveObject
            where TViewModel : class, IReactiveObject
    {
        private TViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        protected ReactiveRecyclerViewViewHolder(View view)
            : base(view)
        {
            SetupRxObj();

            Selected = Observable.FromEventPattern(
                h => view.Click += h,
                h => view.Click -= h)
                    .Select(_ => AdapterPosition);

            LongClicked = Observable.FromEventPattern<View.LongClickEventArgs>(
                h => view.LongClick += h,
                h => view.LongClick -= h)
                    .Select(_ => AdapterPosition);
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add => PropertyChangingEventManager.AddHandler(this, value);
            remove => PropertyChangingEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => PropertyChangedEventManager.AddHandler(this, value);
            remove => PropertyChangedEventManager.RemoveHandler(this, value);
        }

        /// <summary>
        /// Signals that this ViewHolder has been selected.
        ///
        /// The <see cref="int"/> is the position of this ViewHolder in the <see cref="RecyclerView"/>
        /// and corresponds to the <see cref="RecyclerView.ViewHolder.AdapterPosition"/> property.
        /// </summary>
        public IObservable<int> Selected { get; }

        /// <summary>
        /// Signals that this ViewHolder has been long-clicked.
        ///
        /// The <see cref="int"/> is the position of this ViewHolder in the <see cref="RecyclerView"/>
        /// and corresponds to the <see cref="RecyclerView.ViewHolder.AdapterPosition"/> property.
        /// </summary>
        public IObservable<int> LongClicked { get; }

        /// <inheritdoc/>
        public View View => ItemView;

        /// <inheritdoc/>
        public TViewModel ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        /// <inheritdoc/>
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changed => this.GetChangedObservable();

        /// <summary>
        /// Gets all public accessible properties.
        /// </summary>
        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> AllPublicProperties;

        /// <summary>
        /// An observable sequence of thrown exceptions.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, re-enables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            return IReactiveObjectExtensions.SuppressChangeNotifications(this);
        }

        /// <summary>
        /// Gets a value indicating if the change notifications are enabled.
        /// </summary>
        /// <returns>A value indicating whether the change notifications are enabled.</returns>
        public bool AreChangeNotificationsEnabled()
        {
            return IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        [OnDeserialized]
        private void SetupRxObj(StreamingContext sc)
        {
            SetupRxObj();
        }

        private void SetupRxObj()
        {
            AllPublicProperties = new Lazy<PropertyInfo[]>(() =>
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
        }
    }
}
