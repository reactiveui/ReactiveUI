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
    /// An adapter for the Android <see cref="RecyclerView"/>. /// Override the <see
    /// cref="RecyclerView.Adapter.CreateViewHolder(ViewGroup, int)"/> method to create the your <see
    /// cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> based ViewHolder
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
    public abstract class ReactiveRecyclerViewAdapter<TViewModel> : RecyclerView.Adapter
        where TViewModel : class, IReactiveObject
    {
        private readonly IReadOnlyReactiveList<TViewModel> list;

        private IDisposable _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveRecyclerViewAdapter{TViewModel}"/> class.
        /// </summary>
        /// <param name="backingList">The backing list.</param>
        protected ReactiveRecyclerViewAdapter(IReadOnlyReactiveList<TViewModel> backingList)
        {
            this.list = backingList;

            this._inner = this.list.Changed.Subscribe(_ => NotifyDataSetChanged());
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <value>The item count.</value>
        public override int ItemCount { get { return this.list.Count; } }

        /// <summary>
        /// Called when [bind view holder].
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <param name="position">The position.</param>
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((IViewFor)holder).ViewModel = this.list[position];
        }

        /// <summary>
        /// To be added.
        /// </summary>
        /// <param name="disposing">To be added.</param>
        /// <remarks>To be added.</remarks>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref this._inner, Disposable.Empty).Dispose();
        }
    }

    /// <summary>
    /// Reactive Recycler View ViewHolder
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="ReactiveUI.ILayoutViewHost"/>
    /// <seealso cref="ReactiveUI.IViewFor{TViewModel}"/>
    /// <seealso cref="ReactiveUI.IReactiveObject"/>
    public class ReactiveRecyclerViewViewHolder<TViewModel> : RecyclerView.ViewHolder, ILayoutViewHost, IViewFor<TViewModel>, IReactiveNotifyPropertyChanged<ReactiveRecyclerViewViewHolder<TViewModel>>, IReactiveObject
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// All public properties
        /// </summary>
        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        private TViewModel _ViewModel;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        protected ReactiveRecyclerViewViewHolder(View view)
            : base(view)
        {
            setupRxObj();

            this.Selected = Observable.FromEventPattern(h => view.Click += h, h => view.Click -= h).Select(_ => this.AdapterPosition);
        }

        /// <summary>
        /// To be added.
        /// </summary>
        /// <remarks>To be added.</remarks>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        /// <summary>
        /// Occurs when [property changing].
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changed
        {
            get { return this.getChangedObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to be changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changing
        {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Signals that this ViewHolder has been selected. /// The <see cref="int"/> is the position
        /// of this ViewHolder in the <see cref="RecyclerView"/> and corresponds to the <see
        /// cref="RecyclerView.ViewHolder.AdapterPosition"/> property.
        /// </summary>
        public IObservable<int> Selected { get; private set; }

        /// <summary>
        /// Gets the thrown exceptions.
        /// </summary>
        /// <value>The thrown exceptions.</value>
        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        public View View
        {
            get { return this.ItemView; }
        }

        /// <summary>
        /// The ViewModel corresponding to this specific View. This should be a DependencyProperty if
        /// you're using XAML.
        /// </summary>
        public TViewModel ViewModel
        {
            get { return this._ViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._ViewModel, value); }
        }

        object IViewFor.ViewModel
        {
            get { return this.ViewModel; }
            set { this.ViewModel = (TViewModel)value; }
        }

        /// <summary>
        /// Ares the change notifications enabled.
        /// </summary>
        /// <returns></returns>
        public bool AreChangeNotificationsEnabled()
        {
            return this.areChangeNotificationsEnabled();
        }

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// When this method is called, an object will not fire change notifications (neither
        /// traditional nor Observable notifications) until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            return this.suppressChangeNotifications();
        }

        [OnDeserialized]
        private void setupRxObj(StreamingContext sc)
        { setupRxObj(); }

        private void setupRxObj()
        {
            this.allPublicProperties = new Lazy<PropertyInfo[]>(() =>
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
        }
    }
}