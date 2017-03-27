﻿using System;
using Android.Runtime;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Reactive;
using Android.Preferences;
using System.Reactive.Linq;

namespace ReactiveUI
{
    /// <summary>
    /// This is a PreferenceFragment that is both an Activity and has ReactiveObject powers 
    /// (i.e. you can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactivePreferenceFragment<TViewModel> : ReactivePreferenceFragment, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        protected ReactivePreferenceFragment() { }

        protected ReactivePreferenceFragment(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        TViewModel _ViewModel;
        public TViewModel ViewModel {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
        }

        object IViewFor.ViewModel {
            get { return _ViewModel; }
            set { _ViewModel = (TViewModel)value; }
        }
    }

    /// <summary>
    /// This is a PreferenceFragment that is both an Activity and has ReactiveObject powers 
    /// (i.e. you can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactivePreferenceFragment : PreferenceFragment, IReactiveNotifyPropertyChanged<ReactivePreferenceFragment>, IReactiveObject, IHandleObservableErrors
    {
        protected ReactivePreferenceFragment() { }

        protected ReactivePreferenceFragment(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
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
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceFragment>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceFragment>> Changed {
            get { return this.getChangedObservable(); }
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

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        readonly Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated.AsObservable(); } }

        readonly Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated.AsObservable(); } }

        public override void OnPause()
        {
            base.OnPause();
            deactivated.OnNext(Unit.Default);
        }

        public override void OnResume()
        {
            base.OnResume();
            activated.OnNext(Unit.Default);
        }
    }
}
