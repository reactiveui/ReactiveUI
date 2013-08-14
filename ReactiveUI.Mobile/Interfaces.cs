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
}