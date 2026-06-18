// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>Mixin associated with the ReactiveUI <see cref="IReactiveChangeSet"/> change-set type.</summary>
public static class ChangeSetMixins
{
    /// <summary>Provides count-change filtering extension members for <see cref="IObservable{T}"/> streams of <see cref="IReactiveChangeSet{T}"/>.</summary>
    /// <typeparam name="T">The change set item type.</typeparam>
    /// <param name="changeSet">The change-set stream to evaluate.</param>
    extension<T>(IObservable<IReactiveChangeSet<T>> changeSet)
    {
        /// <summary>Filters a change-set stream to only those sets that change the collection count.</summary>
        /// <returns>An observable of change sets that only have count changes.</returns>
        public IObservable<IReactiveChangeSet<T>> WhenCountChanged()
        {
            ArgumentExceptionHelper.ThrowIfNull(changeSet);

            return new CountChangedObservable<T>(changeSet);
        }
    }

    /// <summary>Provides count-change extension members for <see cref="IReactiveChangeSet"/>.</summary>
    /// <param name="changeSet">The change set to evaluate.</param>
    extension(IReactiveChangeSet changeSet)
    {
        /// <summary>Is the change set associated with a count change.</summary>
        /// <returns>If the change set is caused by the count being changed.</returns>
        public bool CountHasChanged()
        {
            ArgumentExceptionHelper.ThrowIfNull(changeSet);

            return changeSet.Adds > 0 || changeSet.Removes > 0;
        }
    }

    /// <summary>Forwards only the change sets that alter the collection count.</summary>
    /// <typeparam name="T">The change set item type.</typeparam>
    /// <param name="source">The source change-set stream.</param>
    private sealed class CountChangedObservable<T>(IObservable<IReactiveChangeSet<T>> source) : IObservable<IReactiveChangeSet<T>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IReactiveChangeSet<T>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer));
        }

        /// <summary>Gates the source change sets on a count change.</summary>
        /// <param name="downstream">The observer receiving count-changing sets.</param>
        private sealed class Sink(IObserver<IReactiveChangeSet<T>> downstream) : IObserver<IReactiveChangeSet<T>>
        {
            /// <inheritdoc/>
            public void OnNext(IReactiveChangeSet<T> value)
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
