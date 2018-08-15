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

namespace ReactiveUI.Android.Support
{
    /// <summary>
    /// An adapter for the Android <see cref="RecyclerView"/>.
    /// 
    /// Override the <see cref="RecyclerView.Adapter.CreateViewHolder(ViewGroup, int)"/> method 
    /// to create the your <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> based ViewHolder
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
    public abstract class ReactiveRecyclerViewAdapter<TViewModel, TCollection> : RecyclerView.Adapter 
        where TViewModel : class, IReactiveObject
        where TCollection : ICollection<TViewModel>, INotifyCollectionChanged
    {
        readonly TCollection list;

        IDisposable inner;

        protected ReactiveRecyclerViewAdapter(TCollection backingList)
        {
            this.list = backingList;

            this.inner = this
                .list
                .ToObservableChangeSet<TCollection, TViewModel>()
                .ForEachChange(UpdateBindings)
                .Subscribe();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((IViewFor)holder).ViewModel = this.list.ElementAt(position);
        }

        public override int ItemCount => this.list.Count;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref this.inner, Disposable.Empty).Dispose();
        }

        private void UpdateBindings(Change<TViewModel> change)
        {
            switch(change.Reason)
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

    public class ReactiveRecyclerViewViewHolder<TViewModel> : RecyclerView.ViewHolder, ILayoutViewHost, IViewFor<TViewModel>, IReactiveNotifyPropertyChanged<ReactiveRecyclerViewViewHolder<TViewModel>>, IReactiveObject
        where TViewModel : class, IReactiveObject
    {
        protected ReactiveRecyclerViewViewHolder(View view)
            : base(view)
        {
            setupRxObj();

            this.Selected = Observable.FromEventPattern(h => view.Click += h, h => view.Click -= h).Select(_ => this.AdapterPosition);
        }

        /// <summary>
        /// Signals that this ViewHolder has been selected. 
        /// 
        /// The <see cref="int"/> is the position of this ViewHolder in the <see cref="RecyclerView"/> 
        /// and corresponds to the <see cref="RecyclerView.ViewHolder.AdapterPosition"/> property.
        /// </summary>
        public IObservable<int> Selected { get; private set; }

        public View View => this.ItemView;

        TViewModel _ViewModel;
        public TViewModel ViewModel
        {
            get => _ViewModel;
            set => this.RaiseAndSetIfChanged(ref _ViewModel, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        public event PropertyChangingEventHandler PropertyChanging
        {
            add => PropertyChangingEventManager.AddHandler(this, value);
            remove => PropertyChangingEventManager.RemoveHandler(this, value);
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => PropertyChangedEventManager.AddHandler(this, value);
            remove => PropertyChangedEventManager.RemoveHandler(this, value);
        }

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.         
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changing => this.getChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changed => this.getChangedObservable();

        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions => this.getThrownExceptionsObservable();

        [OnDeserialized]
        void setupRxObj(StreamingContext sc) { setupRxObj(); }

        void setupRxObj()
        {
            allPublicProperties = new Lazy<PropertyInfo[]>(() =>
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
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

        public bool AreChangeNotificationsEnabled()
        {
            return this.areChangeNotificationsEnabled();
        }
    }
}
