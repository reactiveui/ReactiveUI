// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Merges several sources and forwards only values that differ from the previously forwarded one — a fused,
/// allocation-light replacement for <c>Merge(...).DistinctUntilChanged()</c>. It holds a single shared subscription
/// set, has no operator pipeline, forwards errors, and ignores per-source completion (it completes only via dispose).
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="sources">The sources to merge.</param>
internal sealed class MergedDistinctObservable<T>(params IObservable<T>[] sources) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        var sink = new Sink(observer);
        sink.Run(sources);
        return sink;
    }

    /// <summary>Forwards distinct merged values downstream and tears down every source on dispose.</summary>
    /// <param name="downstream">The downstream observer.</param>
    private sealed class Sink(IObserver<T> downstream) : IDisposable
    {
        /// <summary>Serializes value forwarding across the merged sources.</summary>
#if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
#else
        private readonly object _gate = new();
#endif

        /// <summary>The per-source subscriptions, torn down on dispose.</summary>
        private IDisposable[]? _subscriptions;

        /// <summary>The most recently forwarded value (valid only once <see cref="_hasLast"/> is set), used to suppress duplicates.</summary>
        private T _last = default!;

        /// <summary>Whether a value has been forwarded yet.</summary>
        private bool _hasLast;

        /// <summary>Subscribes to every merged source.</summary>
        /// <param name="sources">The sources to merge.</param>
        public void Run(IObservable<T>[] sources)
        {
            var subscriptions = new IDisposable[sources.Length];
            for (var i = 0; i < sources.Length; i++)
            {
                subscriptions[i] = sources[i].Subscribe(new Element(this));
            }

            _subscriptions = subscriptions;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            var subscriptions = _subscriptions;
            if (subscriptions is null)
            {
                return;
            }

            _subscriptions = null;
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }
        }

        /// <summary>Forwards a value when it differs from the previously forwarded one.</summary>
        /// <param name="value">The merged value.</param>
        private void Forward(T value)
        {
            lock (_gate)
            {
                if (_hasLast && EqualityComparer<T>.Default.Equals(_last, value))
                {
                    return;
                }

                _last = value;
                _hasLast = true;
                downstream.OnNext(value);
            }
        }

        /// <summary>Forwards an error downstream.</summary>
        /// <param name="error">The error.</param>
        private void ForwardError(Exception error)
        {
            lock (_gate)
            {
                downstream.OnError(error);
            }
        }

        /// <summary>Observes a single merged source.</summary>
        /// <param name="parent">The owning sink.</param>
        private sealed class Element(Sink parent) : IObserver<T>
        {
            /// <inheritdoc/>
            public void OnNext(T value) => parent.Forward(value);

            /// <inheritdoc/>
            public void OnError(Exception error) => parent.ForwardError(error);

            /// <inheritdoc/>
            public void OnCompleted()
            {
            }
        }
    }
}
