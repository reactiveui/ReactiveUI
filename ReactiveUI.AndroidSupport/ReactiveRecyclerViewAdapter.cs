using System;
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

namespace ReactiveUI.Android.Support
{
    /// <summary>
    /// An adapter for the Android <see cref="RecyclerView"/>.
    /// 
    /// Override the <see cref="RecyclerView.Adapter.CreateViewHolder(ViewGroup, int)"/> method 
    /// to create the your <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> based ViewHolder
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
    public abstract class ReactiveRecyclerViewAdapter<TViewModel> : RecyclerView.Adapter 
        where TViewModel : class, IReactiveObject
    {
        readonly IReadOnlyReactiveList<TViewModel> list;

        IDisposable _inner;

        protected ReactiveRecyclerViewAdapter(IReadOnlyReactiveList<TViewModel> backingList)
        {
            this.list = backingList;

            _inner = this.list.Changed.Subscribe(_ => NotifyDataSetChanged()); 
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((ReactiveRecyclerViewViewHolder<TViewModel>)holder).ViewModel = list[position];
        }

        public override int ItemCount { get { return list.Count; } }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
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

        public View View
        {
            get { return this.ItemView; }
        }

        TViewModel _ViewModel;
        public TViewModel ViewModel
        {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
        }

        object IViewFor.ViewModel
        {
            get { return _ViewModel; }
            set { _ViewModel = (TViewModel)value; }
        }

        public event PropertyChangingEventHandler PropertyChanging
        {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
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
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changing
        {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changed
        {
            get { return this.getChangedObservable(); }
        }

        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

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