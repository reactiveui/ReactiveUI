// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI;

/// <summary>Count-change extension members for DynamicData <see cref="IChangeSet"/> streams.</summary>
public static class DynamicDataChangeSetMixins
{
    /// <summary>Provides count-change extension members for <see cref="IChangeSet"/>.</summary>
    /// <param name="changeSet">The change set to evaluate.</param>
    extension(IChangeSet changeSet)
    {
        /// <summary>Is the change set associated with a count change.</summary>
        /// <returns>If the change set is caused by the count being changed.</returns>
        public bool HasCountChanged()
        {
            ArgumentExceptionHelper.ThrowIfNull(changeSet);

            return changeSet.Adds > 0 || changeSet.Removes > 0;
        }
    }

    /// <summary>Provides count-change filtering extension members for <see cref="IObservable{T}"/> streams of <see cref="IChangeSet{T}"/>.</summary>
    /// <typeparam name="T">The change set item type.</typeparam>
    /// <param name="changeSet">The change-set stream to evaluate.</param>
    extension<T>(IObservable<IChangeSet<T>> changeSet)
        where T : notnull
    {
        /// <summary>Filters a change-set stream to only those sets that change the collection count.</summary>
        /// <returns>An observable of change sets that only have count changes.</returns>
        public IObservable<IChangeSet<T>> CountChanged()
        {
            ArgumentExceptionHelper.ThrowIfNull(changeSet);

            return new CountFilterObservable<IChangeSet<T>>(changeSet);
        }
    }

    /// <summary>Provides count-change filtering extension members for <see cref="IObservable{T}"/> streams of <see cref="IChangeSet"/>.</summary>
    /// <param name="changeSet">The change-set stream to evaluate.</param>
    extension(IObservable<IChangeSet> changeSet)
    {
        /// <summary>Filters a change-set stream to only those sets that change the collection count.</summary>
        /// <returns>An observable of change sets that only have count changes.</returns>
        public IObservable<IChangeSet> CountChanged()
        {
            ArgumentExceptionHelper.ThrowIfNull(changeSet);

            return new CountFilterObservable<IChangeSet>(changeSet);
        }
    }

    /// <summary>Forwards only the DynamicData change sets that alter the collection count.</summary>
    /// <typeparam name="TChangeSet">The change-set type.</typeparam>
    /// <param name="source">The source change-set stream.</param>
    private sealed class CountFilterObservable<TChangeSet>(IObservable<TChangeSet> source) : IObservable<TChangeSet>
        where TChangeSet : IChangeSet
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TChangeSet> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer));
        }

        /// <summary>Gates the source change sets on a count change.</summary>
        /// <param name="downstream">The observer receiving count-changing sets.</param>
        private sealed class Sink(IObserver<TChangeSet> downstream) : IObserver<TChangeSet>
        {
            /// <inheritdoc/>
            public void OnNext(TChangeSet value)
            {
                if (value.Adds <= 0 && value.Removes <= 0)
                {
                    return;
                }

                downstream.OnNext(value);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }
}
