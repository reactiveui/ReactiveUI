﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Cinephile.Core.Models;
using DynamicData;
using DynamicData.Kernel;

namespace Cinephile.Core.Infrastructure
{
    public static class DynamicDataExtensions
    {
        public static void EditDiff<TDomainEntity, TKey>(this SourceCache<TDomainEntity, TKey> sourceCache, IEnumerable<TDomainEntity> items, int? offset = null, int pageSize = 25) where TDomainEntity : Movie
        {
            var keyComparer = new KeyComparer<TDomainEntity, TKey>();
            Func<TDomainEntity, TDomainEntity, bool> areEqual = EqualityComparer<TDomainEntity>.Default.Equals;

            sourceCache.Edit(innerCache =>
            {
                var originalItems = offset == null
                    ? innerCache.KeyValues.AsArray()
                    : innerCache.KeyValues.Skip((int)offset).Take(pageSize).AsArray();
                var newItems = innerCache.GetKeyValues(items).AsArray();

                var removes = originalItems.Except(newItems, keyComparer).ToArray();
                var adds = newItems.Except(originalItems, keyComparer).ToArray();
                var intersect = newItems
                    .Select(kvp => new
                    {
                        Original = originalItems
                            .Where(x => keyComparer.Equals(kvp, x))
                            .Select(found => new { found.Key, found.Value })
                            .FirstOrDefault(),
                        NewItem = kvp
                    })
                    .Where(x => x.Original != null && !areEqual(x.Original.Value, x.NewItem.Value))
                    .Select(x => new KeyValuePair<TKey, TDomainEntity>(x.NewItem.Key, x.NewItem.Value))
                    .ToArray();

                //Now we are invalidating the cache if there are items to be removed and the sum of intersections is greater
                //than or equal to the page size on the first page.
                //This fixes a problem on the search and potentially in other places too
                if (offset == 0 && removes.Any() && removes.Count() + intersect.Count() >= pageSize)
                {
                    innerCache.Clear();
                }

                innerCache.Remove(removes.Select(kvp => kvp.Key));
                innerCache.AddOrUpdate(adds.Union(intersect));
            });
        }

        public static IObservable<IChangeSet<TDestination, TKey>> Transform<TObject, TKey, TDestination>(
            this IObservable<IChangeSet<TObject, TKey>> source,
            Func<TObject, TDestination> factory,
            Action<TDestination, TObject> updateAction)
        {
            return source
                .Scan(new ChangeAwareCache<TDestination, TKey>(), (cache, changes) =>
                {
                    foreach (var change in changes)
                    {
                        switch (change.Reason)
                        {
                            case ChangeReason.Add:
                                cache.AddOrUpdate(factory(change.Current), change.Key);
                                break;
                            case ChangeReason.Update:

                                //get the transformed item: 
                                var previousTransform = cache.Lookup(change.Key)
                                    .ValueOrThrow(() => new MissingKeyException($"There is no key matching {change.Key} in the cache"));

                                //apply the update action
                                updateAction(previousTransform, change.Current);

                                //send a refresh so sort or filter on this item can work on the inline change [this is an optional step]
                                cache.Refresh(change.Key);
                                break;
                            case ChangeReason.Remove:
                                cache.Remove(change.Key);
                                break;
                            case ChangeReason.Refresh:
                                cache.Refresh(change.Key);
                                break;
                            case ChangeReason.Moved:
                                //Do nothing !
                                break;
                        }
                    }
                    return cache;
                })
                .Select(cache => cache.CaptureChanges()) //convert the change aware cache to a changeset
                .NotEmpty();                            // suppress changeset.count==0 results
        }

}



    internal class KeyComparer<TObject, TKey> : IEqualityComparer<KeyValuePair<TKey, TObject>>
    {
        public bool Equals(KeyValuePair<TKey, TObject> x, KeyValuePair<TKey, TObject> y)
        {
            return x.Key.Equals(y.Key);
        }

        public int GetHashCode(KeyValuePair<TKey, TObject> obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}
