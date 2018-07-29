// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Akavache;

namespace Cinephile.Core.Rest
{
    /// <summary>
    /// Cache.
    /// </summary>
    public sealed class Cache : ICache
    {
        const double CacheValidityInMinutes = 5d;

        /// <summary>
        /// Initialize the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public void Initialize(string name)
        {
            BlobCache.ApplicationName = name;
        }

        /// <summary>
        /// Gets the and fetch latest.
        /// </summary>
        /// <returns>The and fetch latest.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="fetchFunction">Fetch function.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        public IObservable<TResult> GetAndFetchLatest<TResult>(string cacheKey, Func<IObservable<TResult>> fetchFunction)
        {
            return BlobCache
                .LocalMachine
                .GetAndFetchLatest(cacheKey, fetchFunction, offset =>
                {
                    var elapsed = DateTimeOffset.Now - offset;
                    return elapsed > TimeSpan.FromMinutes(CacheValidityInMinutes);
                });
        }

        /// <summary>
        /// Invalidates all.
        /// </summary>
        public void InvalidateAll()
        {
            BlobCache.LocalMachine.InvalidateAll();
        }

        /// <summary>
        /// Invalidates all objects.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void InvalidateAllObjects<T>() where T : class
        {
            BlobCache.LocalMachine.InvalidateAllObjects<T>();
        }

        /// <summary>
        /// Invalidates all.
        /// </summary>
        public void Invalidate(string key)
        {
            BlobCache.LocalMachine.Invalidate(key);
        }


        /// <summary>
        /// Gets the method signature.
        /// </summary>
        /// <returns>The method signature.</returns>
        /// <param name="filePath">File path.</param>
        /// <param name="memberName">Member name.</param>
        /// <param name="parameters">Parameters.</param>
        public static string GetMethodSignature([CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", params object[] parameters)
        {
            var fileName = filePath.Substring(filePath.LastIndexOf("/", StringComparison.CurrentCulture) + 1);
            var className = fileName.Replace(".cs", "");
            var methodParameters = string.Join(",", parameters);

            return $"{className}.{memberName}({methodParameters})";
        }
    }
}
