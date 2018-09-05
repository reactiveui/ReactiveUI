// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
namespace Cinephile.Core.Rest
{
    public interface ICache
    {
        void Initialize(string name);
        IObservable<TResult> GetAndFetchLatest<TResult>(string cacheKey, Func<IObservable<TResult>> fetchFunction);
        void InvalidateAll();
        void InvalidateAllObjects<T>() where T : class;
        void Invalidate(string key);
    }
}
