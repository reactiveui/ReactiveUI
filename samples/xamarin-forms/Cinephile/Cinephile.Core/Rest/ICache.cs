// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Cinephile.Core.Rest
{
    /// <summary>
    /// A cache where we store entries until we need them.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Initializes the cache with the specified name. The name is used for where to store the cache.
        /// </summary>
        /// <param name="name">The name of the cache. Usually the application name.</param>
        void Initialize(string name);

        /// <summary>
        /// Gets or fetches the latest value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cacheKey">The key to use to check the cache.</param>
        /// <param name="fetchFunction">The function used for retrieving the value if not in the cache.</param>
        /// <returns>The value from the cache.</returns>
        IObservable<TResult> GetAndFetchLatest<TResult>(string cacheKey, Func<IObservable<TResult>> fetchFunction);

        /// <summary>
        /// Invalidates all entries inside the cache forcing all subsequent fetch request to retrieve the value.
        /// </summary>
        void InvalidateAll();

        /// <summary>
        /// Invalidates all objects of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to invalidate.</typeparam>
        void InvalidateAllObjects<T>()
            where T : class;

        /// <summary>
        /// Invalidates the value at the specified key which forces all subsequent fetch requests for the key to retrieve the value.
        /// </summary>
        /// <param name="key">The key to invalidate.</param>
        void Invalidate(string key);
    }
}
