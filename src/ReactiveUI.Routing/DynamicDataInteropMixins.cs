// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;
using DynamicData.Kernel;

namespace ReactiveUI;

/// <summary>
/// Projects the internal <see cref="IReactiveChangeSet{T}"/> stream onto DynamicData's <see cref="IChangeSet{T}"/>
/// surface, so the public ReactiveUI API stays DynamicData-compatible while the change tracking is produced by
/// ReactiveUI's own tailored sinks rather than DynamicData's source/operator machinery.
/// </summary>
public static class DynamicDataInteropMixins
{
    /// <summary>Provides DynamicData change-set adaptation extension members for <see cref="IObservable{T}"/> change-set streams.</summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    /// <param name="source">The ReactiveUI change-set stream.</param>
    extension<T>(IObservable<IReactiveChangeSet<T>> source)
        where T : notnull
    {
        /// <summary>Adapts a ReactiveUI change-set stream to a DynamicData change-set stream.</summary>
        /// <returns>A DynamicData change-set stream.</returns>
        public IObservable<IChangeSet<T>> ToDynamicDataChangeSet()
        {
            ArgumentExceptionHelper.ThrowIfNull(source);
            return new ChangeSetAdapter<T>(source);
        }
    }

    /// <summary>Forwards each ReactiveUI change set re-projected as a DynamicData change set.</summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    /// <param name="source">The ReactiveUI change-set stream.</param>
    private sealed class ChangeSetAdapter<T>(IObservable<IReactiveChangeSet<T>> source) : IObservable<IChangeSet<T>>
        where T : notnull
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IChangeSet<T>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer));
        }

        /// <summary>Re-projects each change set downstream as a lightweight DynamicData change set.</summary>
        /// <param name="downstream">The observer receiving DynamicData change sets.</param>
        private sealed class Sink(IObserver<IChangeSet<T>> downstream) : IObserver<IReactiveChangeSet<T>>
        {
            /// <inheritdoc/>
            public void OnNext(IReactiveChangeSet<T> value) => downstream.OnNext(Convert(value));

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();

            /// <summary>Converts a ReactiveUI change set into a lightweight DynamicData change set.</summary>
            /// <param name="source">The ReactiveUI change set.</param>
            /// <returns>The DynamicData change set.</returns>
            private static LightChangeSet<T> Convert(IReactiveChangeSet<T> source)
            {
                if (source.Count == 1)
                {
                    return new(ToDynamicDataChange(source[0]));
                }

                var changes = new Change<T>[source.Count];
                for (var i = 0; i < source.Count; i++)
                {
                    changes[i] = ToDynamicDataChange(source[i]);
                }

                return new(changes);
            }

            /// <summary>Translates a ReactiveUI change into the equivalent DynamicData list change.</summary>
            /// <param name="change">The ReactiveUI change.</param>
            /// <returns>The DynamicData change.</returns>
            private static Change<T> ToDynamicDataChange(in ReactiveChange<T> change) =>
                change.Reason switch
                {
                    ReactiveChangeReason.Add => new(ListChangeReason.Add, change.Current, change.CurrentIndex),
                    ReactiveChangeReason.Remove => new(ListChangeReason.Remove, change.Current, change.CurrentIndex),
                    ReactiveChangeReason.Replace => new(ListChangeReason.Replace, change.Current, Optional.Some(change.Previous!), change.CurrentIndex, change.CurrentIndex),
                    ReactiveChangeReason.Move => new(change.Current, change.CurrentIndex, change.PreviousIndex),
                    ReactiveChangeReason.Refresh => throw new NotSupportedException(
                        "Converting a Refresh change to a DynamicData change is not supported by the ReactiveUI DynamicData interop adapter."),
                    _ => new(ListChangeReason.Refresh, change.Current, change.CurrentIndex),
                };
        }
    }
}
