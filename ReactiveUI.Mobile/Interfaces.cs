using System;
using System.Reactive;
using Xamarin.Geolocation;

namespace ReactiveUI.Mobile
{
    public interface IReactiveGeolocator
    {
        IObservable<Position> Listen(int minUpdateTime, double minUpdateDist, bool includeHeading = false);
        IObservable<Position> GetPosition(bool includeHeading = false);
    }

    /* Nicked from http://caliburnmicro.codeplex.com/wikipage?title=Working%20with%20Windows%20Phone%207%20v1.1
     *
     * Launching - Occurs when a fresh instance of the application is launching.
     * Activated - Occurs when a previously paused/tombstoned app is resumed/resurrected.
     * Deactivated - Occurs when the application is being paused or tombstoned.
     * Closing - Occurs when the application is closing.
     * Continuing - Occurs when the app is continuing from a temporarily paused state.
     * Continued - Occurs after the app has continued from a temporarily paused state.
     * Resurrecting - Occurs when the app is "resurrecting" from a tombstoned state.
     * Resurrected - Occurs after the app has "resurrected" from a tombstoned state.
    */

    public interface ISuspensionHost
    {
        IObservable<Unit> IsLaunchingNew { get; }
        IObservable<Unit> IsResuming { get; }
        IObservable<Unit> IsUnpaused { get; }
        IObservable<Unit> ShouldPersistState { get; }
    }

    public interface ISuspensionDriver
    {
        IObservable<T> LoadState<T>();
        IObservable<Unit> SaveState<T>(T state);
    }
}