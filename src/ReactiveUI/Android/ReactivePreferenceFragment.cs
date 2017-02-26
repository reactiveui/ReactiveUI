using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Preferences;
using Android.Runtime;

namespace ReactiveUI
{
    /// <summary>
    /// This is a PreferenceFragment that is both an Activity and has ReactiveObject powers (i.e. you
    /// can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactivePreferenceFragment<TViewModel> : ReactivePreferenceFragment, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        private TViewModel _ViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceFragment{TViewModel}"/> class.
        /// </summary>
        protected ReactivePreferenceFragment() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceFragment{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactivePreferenceFragment(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
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
            get { return this._ViewModel; }
            set { this._ViewModel = (TViewModel)value; }
        }
    }

    /// <summary>
    /// This is a PreferenceFragment that is both an Activity and has ReactiveObject powers (i.e. you
    /// can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactivePreferenceFragment : PreferenceFragment, IReactiveNotifyPropertyChanged<ReactivePreferenceFragment>, IReactiveObject, IHandleObservableErrors
    {
        private readonly Subject<Unit> activated = new Subject<Unit>();

        private readonly Subject<Unit> deactivated = new Subject<Unit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceFragment"/> class.
        /// </summary>
        protected ReactivePreferenceFragment() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceFragment"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactivePreferenceFragment(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
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
        /// Gets the activated.
        /// </summary>
        /// <value>The activated.</value>
        public IObservable<Unit> Activated { get { return this.activated.AsObservable(); } }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceFragment>> Changed
        {
            get { return this.getChangedObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceFragment>> Changing
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
        /// Called when the Fragment is no longer resumed.
        /// </summary>
        /// <remarks>
        /// <para tool="javadoc-to-mdoc">
        /// Called when the Fragment is no longer resumed. This is generally tied to <c><see
        /// cref="M:Android.App.Activity.OnPause"/></c> of the containing Activity's lifecycle.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// <format type="text/html"><a
        /// href="http://developer.android.com/reference/android/app/Fragment.html#onPause()"
        /// target="_blank">[Android Documentation]</a></format>
        /// </para>
        /// </remarks>
        /// <since version="Added in API level 11"/>
        public override void OnPause()
        {
            base.OnPause();
            this.deactivated.OnNext(Unit.Default);
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        /// <remarks>
        /// <para tool="javadoc-to-mdoc">
        /// Called when the fragment is visible to the user and actively running. This is generally
        /// tied to <c><see cref="M:Android.App.Activity.OnResume"/></c> of the containing Activity's lifecycle.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// <format type="text/html"><a
        /// href="http://developer.android.com/reference/android/app/Fragment.html#onResume()"
        /// target="_blank">[Android Documentation]</a></format>
        /// </para>
        /// </remarks>
        /// <since version="Added in API level 11"/>
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