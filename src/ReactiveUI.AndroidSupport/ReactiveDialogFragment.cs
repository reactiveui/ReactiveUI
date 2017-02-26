using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUI.AndroidSupport
{
    /// <summary>
    /// This is a DialogFragment that is both a DialogFragment and has ReactiveObject powers (i.e.
    /// you can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactiveDialogFragment<TViewModel> : ReactiveDialogFragment, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        private TViewModel _ViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveDialogFragment{TViewModel}"/> class.
        /// </summary>
        protected ReactiveDialogFragment() { }

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
            get { return this._ViewModel; }
            set { this._ViewModel = (TViewModel)value; }
        }
    }

    /// <summary>
    /// This is a Fragment that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactiveDialogFragment : global::Android.Support.V4.App.DialogFragment, IReactiveNotifyPropertyChanged<ReactiveDialogFragment>, IReactiveObject, IHandleObservableErrors
    {
        private readonly Subject<Unit> activated = new Subject<Unit>();

        private readonly Subject<Unit> deactivated = new Subject<Unit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveDialogFragment"/> class.
        /// </summary>
        protected ReactiveDialogFragment() { }

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
        /// Gets the activated.
        /// </summary>
        /// <value>The activated.</value>
        public IObservable<Unit> Activated { get { return this.activated.AsObservable(); } }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveDialogFragment>> Changed
        {
            get { return this.getChangedObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveDialogFragment>> Changing
        {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Gets the deactivated.
        /// </summary>
        /// <value>The deactivated.</value>
        public IObservable<Unit> Deactivated { get { return this.deactivated.AsObservable(); } }

        /// <summary>
        /// Fires whenever an exception would normally terminate ReactiveUI internal state.
        /// </summary>
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        /// <summary>
        /// Called when [pause].
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            this.deactivated.OnNext(Unit.Default);
        }

        /// <summary>
        /// Called when [resume].
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            this.activated.OnNext(Unit.Default);
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
    }
}