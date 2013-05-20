using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Reactive.Subjects;

namespace ReactiveUI
{
    /// <summary>
    /// This class helps you take existing objects and make them compatible with
    /// ReactiveUI and Rx.Net. To use this, declare an instance field of this
    /// class in your class, initialize it in your Constructor, make your class
    /// derive from IReactiveNotifyPropertyChanged, then implement all of the
    /// properties/methods using MakeObjectReactiveHelper.
    /// </summary>
    public class MakeObjectReactiveHelper : IReactiveNotifyPropertyChanged
    {
        public MakeObjectReactiveHelper(INotifyPropertyChanged hostObject)
        {
            var hostChanging = hostObject as INotifyPropertyChanging;
            if (hostChanging != null) {
                hostChanging.PropertyChanging += (o, e) => _Changing.OnNext(
                    new ObservedChange<object, object>() { Sender = o, PropertyName = e.PropertyName });
            } else {
                this.Log().Error("'{0}' does not implement INotifyPropertyChanging - RxUI may return duplicate change notifications",
                    hostObject.GetType().FullName);
            }

            hostObject.PropertyChanged += (o, e) => _Changed.OnNext(
                new ObservedChange<object, object>() { Sender = o, PropertyName = e.PropertyName });
        }

        long _changeCountSuppressed = 0;
        public IDisposable SuppressChangeNotifications()
        {
            Interlocked.Increment(ref _changeCountSuppressed);
            return Disposable.Create(() => Interlocked.Decrement(ref _changeCountSuppressed));
        }

        readonly ISubject<IObservedChange<object, object>> _Changing = 
            new ScheduledSubject<IObservedChange<object, object>>(RxApp.MainThreadScheduler);

        public IObservable<IObservedChange<object, object>> Changing {
            get { return _Changing.Where(_ => Interlocked.Read(ref _changeCountSuppressed) == 0); }
        }

        readonly ISubject<IObservedChange<object, object>> _Changed = 
            new ScheduledSubject<IObservedChange<object, object>>(RxApp.MainThreadScheduler);

        public IObservable<IObservedChange<object, object>> Changed {
            get { return _Changed.Where(_ => Interlocked.Read(ref _changeCountSuppressed) == 0); }
        }

        public event PropertyChangedEventHandler PropertyChanged 
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public event PropertyChangingEventHandler PropertyChanging
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }
}
