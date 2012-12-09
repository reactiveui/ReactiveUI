using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Geolocation;

namespace ReactiveUI.Mobile_WinRT
{
    public interface IReactiveGeolocator
    {
        IObservable<Position> Listen(int minUpdateTime, double minUpdateDist, bool includeHeading = false);
        IObservable<Position> GetPosition(bool includeHeading = false);
    }

    public static class ReactiveGeolocator
    {
        [ThreadStatic] static IReactiveGeolocator _UnitTestImplementation;
        static IReactiveGeolocator _Implementation;

        internal static IReactiveGeolocator Implementation {
            get { return _UnitTestImplementation ?? _Implementation; }
            set {
                if (RxApp.InUnitTestRunner()) {
                    _UnitTestImplementation = value;
                    _Implementation = _Implementation ?? value;
                } else {
                    _Implementation = value;
                }
            }
        }

        public static IObservable<Position> Listen(int minUpdateTime, double minUpdateDist, bool includeHeading = false)
        {
            if (Implementation != null) {
                return Implementation.Listen(minUpdateTime, minUpdateDist, includeHeading);
            }

            var ret = Observable.Create<Position>(subj => {
                var geo = new Geolocator();
                var disp = new CompositeDisposable();
                bool isDead = false;

                if (!geo.IsGeolocationAvailable || !geo.IsGeolocationEnabled) {
                    return Observable.Throw<Position>(new Exception("Geolocation isn't available")).Subscribe(subj);
                }

                // NB: This isn't very Functional, but I'm lazy.
                disp.Add(Observable.FromEventPattern<PositionEventArgs>(x => geo.PositionChanged += x, x => geo.PositionChanged -= x).Subscribe(x => {
                    if (isDead) return;
                    subj.OnNext(x.EventArgs.Position);
                }));

                disp.Add(Observable.FromEventPattern<PositionErrorEventArgs>(x => geo.PositionError += x, x => geo.PositionError -= x).Subscribe(ex => {
                    isDead = true;
                    var toDisp = Interlocked.Exchange(ref disp, null);
                    if (toDisp != null) toDisp.Dispose();
                    subj.OnError(new GeolocationException(ex.EventArgs.Error));
                }));

                return disp;
            });

            return ret.Multicast(new Subject<Position>()).RefCount();
        }

        public static IObservable<Position> GetPosition(bool includeHeading = false)
        {
            if (Implementation != null) {
                return Implementation.GetPosition(includeHeading);
            }

            var ret = Observable.Create<Position>(subj => {
                var geo = new Geolocator();
                var cts = new CancellationTokenSource();
                var disp = new CompositeDisposable();

                if (!geo.IsGeolocationAvailable || !geo.IsGeolocationEnabled) {
                    return Observable.Throw<Position>(new Exception("Geolocation isn't available")).Subscribe(subj);
                }

                disp.Add(new CancellationDisposable(cts));
                disp.Add(geo.GetPositionAsync(cts.Token, includeHeading).ToObservable().Subscribe(subj));
                return disp;
            }).Multicast(new AsyncSubject<Position>());

            ret.Connect();
            return ret;
        }
    }

    public class GeolocationException : Exception
    {
        public GeolocationException(GeolocationError error)
        {
            Info = error;
        }

        public GeolocationError Info { get; protected set; }
    }
}