using System;
using System.Reactive;
using ReactiveUI;
using Xamarin.Geolocation;

namespace ReactiveUI.Mobile
{
    public interface IReactiveGeolocator
    {
        /// <summary>
        /// Returns an IObservable that continuously updates as the user's
        /// physical location changes. It is super important to make sure to
        /// dispose all subscriptions to this IObservable.
        /// </summary>
        /// <param name="minUpdateTime">Minimum update time.</param>
        /// <param name="minUpdateDist">Minimum update dist.</param>
        /// <param name="includeHeading">If set to <c>true</c> include heading.</param>
        IObservable<Position> Listen(int minUpdateTime, double minUpdateDist, bool includeHeading = false);

        /// <summary>
        /// Returns a single lookup of the user's current physical position
        /// </summary>
        /// <returns>The current physical location.</returns>
        /// <param name="includeHeading">If set to <c>true</c> include heading.</param>
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

    /// <summary>
    /// ISuspensionHost represents a standardized version of the events that the
    /// host operating system publishes. Subscribe to these events in order to
    /// handle app suspend / resume.
    /// </summary>
    public interface ISuspensionHost
    {
        /// <summary>
        /// Signals when the application is launching new. This can happen when
        /// an app has recently crashed, as well as the firs time the app has
        /// been launched. Apps should create their state from scratch.
        /// </summary>
        IObservable<Unit> IsLaunchingNew { get; }

        /// <summary>
        /// Signals when the application is resuming from suspended state (i.e. 
        /// it was previously running but its process was destroyed). 
        /// </summary>
        IObservable<Unit> IsResuming { get; }

        /// <summary>
        /// Signals when the application is activated. Note that this may mean 
        /// that your process was not actively running before this signal.
        /// </summary>
        IObservable<Unit> IsUnpausing { get; }

        /// <summary>
        /// Signals when the application should persist its state to disk.
        /// </summary>
        /// <value>Returns an IDisposable that should be disposed once the 
        /// application finishes persisting its state</value>
        IObservable<IDisposable> ShouldPersistState { get; }

        /// <summary>
        /// Signals that the saved application state should be deleted, this
        /// usually is called after an app has crashed
        /// </summary>
        IObservable<Unit> ShouldInvalidateState { get; }

        /// <summary>
        /// Sets up the default suspend resume behavior, which is to use the
        /// ISuspensionDriver to save / reload application state. Using this also
        /// requires you to register an IApplicationRootState that will create a
        /// new application root state from scratch.
        /// </summary>
        void SetupDefaultSuspendResume(ISuspensionDriver driver = null);
    }

    /// <summary>
    /// ISuspensionDriver represents a class that can load/save state to persistent
    /// storage. Most platforms have a basic implementation of this class, but you
    /// probably want to write your own.
    /// </summary>
    public interface ISuspensionDriver
    {
        /// <summary>
        /// Loads the application state from persistent storage
        /// </summary>
        IObservable<T> LoadState<T>() where T : class, IApplicationRootState;

        /// <summary>
        /// Saves the application state to disk.
        /// </summary>
        IObservable<Unit> SaveState<T>(T state) where T : class, IApplicationRootState;

        /// <summary>
        /// Invalidates the application state (i.e. deletes it from disk)
        /// </summary>
        IObservable<Unit> InvalidateState();
    }

    public interface IApplicationRootState : IScreen { }
}