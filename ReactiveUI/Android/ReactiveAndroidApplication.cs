using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using Android.App;
using ReactiveUI;

namespace ReactiveUI
{
    public class ReactiveAndroidApplication<TViewModel> : ReactiveAndroidApplication, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        protected ReactiveAndroidApplication(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        { }

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

    public class ReactiveAndroidApplication : Application, IReactiveObject, IReactiveNotifyPropertyChanged<ReactiveAndroidApplication>, IHandleObservableErrors
    {
        protected ReactiveAndroidApplication(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new AndroidHandlerScheduler());
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
            
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveAndroidApplication>> Changing {
            get { return this.getChangingObservable(); }
        }
            
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveAndroidApplication>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IDisposable SuppressChangeNotifications()
        {
            return this.suppressChangeNotifications();
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        readonly Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }

        readonly Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

    }
}

