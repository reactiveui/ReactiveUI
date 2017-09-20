// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive;
using System.Threading.Tasks;

namespace Services.State
{
    public delegate IObservable<Unit> SaveCallback(IStateService stateService);

    public interface IStateService
    {
        IObservable<T> Get<T>(string key);

        /// <summary>
        ///     Immediately return a cached version of an object if available, but *always* also execute fetchFunc to retrieve the
        ///     latest version of an object.
        /// </summary>
        IObservable<T> GetAndFetchLatest<T>(string key, Func<Task<T>> fetchFunc,
            Func<DateTimeOffset, bool> fetchPredicate = null, DateTimeOffset? absoluteExpiration = null);

        /// <summary>
        ///     Attempt to return an object from the cache. If the item doesn't exist or returns an error, call a Func to return
        ///     the latest version of an object and insert the result in the cache.
        /// </summary>
        IObservable<T> GetOrFetch<T>(string key, Func<Task<T>> fetchFunc, DateTimeOffset? absoluteExpiration = null);
        IObservable<Unit> Invalidate(string key);

        IDisposable RegisterSaveCallback(SaveCallback saveCallback);

        IObservable<Unit> Remove<T>(string key);

        IObservable<Unit> Save();

        IObservable<Unit> Set<T>(string key, T value);

        IObservable<Unit> Set<T>(string key, T value, TimeSpan expiration);
    }
}
