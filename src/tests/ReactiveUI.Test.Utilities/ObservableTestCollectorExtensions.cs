// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>Lightweight test helpers for accumulating an observable's emissions into a collection.</summary>
public static class ObservableTestCollectorExtensions
{
    /// <summary>Provides emission-collecting helpers for an observable.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The observable whose emissions are collected.</param>
    extension<T>(IObservable<T> source)
    {
        /// <summary>Collects every value an observable emits into a live-updating list for assertion.</summary>
        /// <remarks>
        /// Replaces the heavyweight <c>source.ToObservableChangeSet(...).Bind(out var xs).Subscribe()</c> idiom:
        /// the change-set + collection-binding pipeline is overkill when a test only needs to accumulate
        /// emissions and assert on their count/contents. The subscription lives for the returned list's lifetime.
        /// Uses the <see cref="IObservable{T}"/> interface <c>Subscribe</c> directly to stay agnostic of which
        /// (Primitives vs System.Reactive) operator surface is in scope.
        /// </remarks>
        /// <returns>A collection that appends each emitted value as it arrives.</returns>
        public Collection<T> Collect()
        {
            ArgumentExceptionHelper.ThrowIfNull(source);

            var items = new Collection<T>();
            source.Subscribe(new CollectingObserver<T>(items));
            return items;
        }
    }

    /// <summary>An observer that appends each received value to a backing collection.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="items">The collection values are appended to.</param>
    private sealed class CollectingObserver<T>(Collection<T> items) : IObserver<T>
    {
        /// <summary>Guards the backing collection against concurrent appends — some tests deliver emissions from many
        /// threads at once (e.g. concurrent MessageBus sends), and <see cref="Collection{T}"/> is not thread-safe.</summary>
        private readonly Lock _gate = new();

        /// <inheritdoc/>
        public void OnNext(T value)
        {
            lock (_gate)
            {
                items.Add(value);
            }
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => throw error;

        /// <inheritdoc/>
        public void OnCompleted()
        {
            // No completion handling needed for a test collector.
        }
    }
}
