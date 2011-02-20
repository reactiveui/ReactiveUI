using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Disposables;
using System.Linq;
using System.Text;
using System.Threading;

namespace ReactiveUI
{
    /// <summary>
    /// 
    /// </summary>
    public class MakeObjectReactiveHelper  : IReactiveNotifyPropertyChanged
    {
        public MakeObjectReactiveHelper(INotifyPropertyChanged hostObject)
        {
            var hostChanging = hostObject as INotifyPropertyChanging;
            if (hostChanging != null) {
                hostChanging.PropertyChanging += (o, e) => _Changing.OnNext(
                    new ObservedChange<object, object>() { Sender = o, PropertyName = e.PropertyName });
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

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        readonly Subject<IObservedChange<object, object>> _Changing = 
            new Subject<IObservedChange<object, object>>(RxApp.DeferredScheduler);

        public IObservable<IObservedChange<object, object>> Changing {
#if SILVERLIGHT
            get { return _Changing.Where(_ => _changeCountSuppressed == 0); }
#else 
            get { return _Changing.Where(_ => Interlocked.Read(ref _changeCountSuppressed) == 0); }
#endif
        }

        Subject<IObservedChange<object, object>> _Changed = 
            new Subject<IObservedChange<object, object>>(RxApp.DeferredScheduler);

        public IObservable<IObservedChange<object, object>> Changed {
#if SILVERLIGHT
            get { return _Changed.Where(_ => _changeCountSuppressed == 0); }
#else 
            get { return _Changed.Where(_ => Interlocked.Read(ref _changeCountSuppressed) == 0); }
#endif
        }
    }
}