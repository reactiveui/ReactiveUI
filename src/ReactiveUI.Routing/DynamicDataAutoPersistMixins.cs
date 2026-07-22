// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI;

/// <summary>AutoPersist <c>ActOnEveryObject</c> support for DynamicData change-set streams.</summary>
public static class DynamicDataAutoPersistMixins
{
    /// <summary>Provides ActOnEveryObject extension members for DynamicData change-set streams.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="this">The observable change set to watch for changes.</param>
    extension<TItem>(IObservable<IChangeSet<TItem>> @this)
        where TItem : IReactiveObject
    {
        /// <summary>
        /// Call methods <paramref name="onAdd"/> and <paramref name="onRemove"/> whenever an object is added or
        /// removed from a collection. This method correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="onAdd">A method to be called when an object is added to the collection.</param>
        /// <param name="onRemove">A method to be called when an object is removed from the collection.</param>
        /// <returns>A disposable that deactivates this behavior.</returns>
        public IDisposable ActOnEveryObject(
            Action<TItem> onAdd,
            Action<TItem> onRemove)
        {
            ArgumentExceptionHelper.ThrowIfNull(@this);
            ArgumentExceptionHelper.ThrowIfNull(onAdd);
            ArgumentExceptionHelper.ThrowIfNull(onRemove);

            return @this.Subscribe(new DelegateWitness<IChangeSet<TItem>>(changeSet =>
            {
                foreach (var change in changeSet)
                {
                    ApplyDynamicDataChange(change, onAdd, onRemove);
                }
            }));
        }
    }

    /// <summary>Applies a single DynamicData list change by invoking the add or remove callback for the affected items.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="change">The change to apply.</param>
    /// <param name="onAdd">The callback invoked for added items.</param>
    /// <param name="onRemove">The callback invoked for removed items.</param>
    private static void ApplyDynamicDataChange<TItem>(Change<TItem> change, Action<TItem> onAdd, Action<TItem> onRemove)
        where TItem : IReactiveObject
    {
        switch (change.Reason)
        {
            case ListChangeReason.Add:
                {
                    onAdd(change.Item.Current);
                    break;
                }

            case ListChangeReason.Remove:
                {
                    onRemove(change.Item.Current);
                    break;
                }

            case ListChangeReason.Replace:
                {
                    onRemove(change.Item.Previous.Value);
                    onAdd(change.Item.Current);
                    break;
                }

            case ListChangeReason.AddRange:
                {
                    ForEachItem(change.Range, onAdd);
                    break;
                }

            case ListChangeReason.RemoveRange or ListChangeReason.Clear:
                {
                    ForEachItem(change.Range, onRemove);
                    break;
                }

            case ListChangeReason.Refresh:
                {
                    // Preserve original ordering: remove all, then add all.
                    ForEachItem(change.Range, onRemove);
                    ForEachItem(change.Range, onAdd);
                    break;
                }

            case ListChangeReason.Moved:
                {
                    // A move only reorders an existing item; nothing is added or removed.
                    break;
                }

            default:
                {
                    // Any other change reason does not add or remove tracked objects.
                    break;
                }
        }
    }

    /// <summary>Invokes an action for each item in a DynamicData range change.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="range">The range of items.</param>
    /// <param name="action">The action to invoke per item.</param>
    private static void ForEachItem<TItem>(RangeChange<TItem> range, Action<TItem> action)
        where TItem : IReactiveObject
    {
        foreach (var item in range)
        {
            action(item);
        }
    }
}
