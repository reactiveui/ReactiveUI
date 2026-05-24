// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Walks an expression member chain (<c>x.A.B.C</c>) as a single switching engine: one watcher per link, each
/// observing its link on the value produced by the previous link and re-subscribing the deeper links when an
/// intermediate value changes. Emits the leaf value as an observed change, applying skip-initial (counted against the
/// raw stream), the non-null-parent filter, the cast to <typeparamref name="TValue"/>, and optional distinct-by-value
/// inline. Collapses the nested <c>Select</c>+<c>Switch</c> fold plus <c>Skip</c>/<c>Where</c>/<c>Select</c>/
/// <c>DistinctUntilChanged</c> into one layer.
/// </summary>
/// <typeparam name="TSender">The root sender type surfaced on the emitted change.</typeparam>
/// <typeparam name="TValue">The leaf value type.</typeparam>
/// <param name="parameters">The configuration of the chain to observe.</param>
[RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
internal sealed class ExpressionChainSink<TSender, TValue>(ExpressionChainParameters<TSender> parameters)
    : IObservable<IObservedChange<TSender, TValue>>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<IObservedChange<TSender, TValue>> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        var sink = new Sink(observer, parameters);
        sink.Run();
        return sink;
    }

    /// <summary>The running state of one chain subscription.</summary>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    private sealed class Sink : IDisposable
    {
#if NET9_0_OR_GREATER
        /// <summary>Serializes the chain mutations and emission.</summary>
        private readonly Lock _gate = new();
#else
        /// <summary>Serializes the chain mutations and emission.</summary>
        private readonly object _gate = new();
#endif

        /// <summary>The observer receiving the leaf observed changes.</summary>
        private readonly IObserver<IObservedChange<TSender, TValue>> _downstream;

        /// <summary>The root object of the chain.</summary>
        private readonly TSender? _source;

        /// <summary>The full expression surfaced on the emitted change.</summary>
        private readonly Expression? _expression;

        /// <summary>The member-access links of the chain, in order.</summary>
        private readonly Expression[] _links;

        /// <summary>Whether values are observed before they change.</summary>
        private readonly bool _beforeChange;

        /// <summary>Whether POCO observation warnings are suppressed.</summary>
        private readonly bool _suppressWarnings;

        /// <summary>Whether consecutive equal leaf values are suppressed.</summary>
        private readonly bool _isDistinct;

        /// <summary>Produces the change notifications for a link on a given parent value.</summary>
        private readonly Func<object, Expression, bool, bool, IObservable<IObservedChange<object?, object?>>> _notify;

        /// <summary>The per-link watchers.</summary>
        private readonly Level[] _levels;

        /// <summary>Whether the next raw emission should be skipped (skip-initial).</summary>
        private bool _skipNext;

        /// <summary>The last emitted leaf value, used by the distinct gate.</summary>
        private TValue _last = default!;

        /// <summary>Whether <see cref="_last"/> holds a value yet.</summary>
        private bool _hasLast;

        /// <summary>Latched once this chain subscription has been disposed.</summary>
        private bool _disposed;

        /// <summary>Initializes a new instance of the <see cref="Sink"/> class.</summary>
        /// <param name="downstream">The observer receiving the leaf observed changes.</param>
        /// <param name="parameters">The configuration of the chain to observe.</param>
        public Sink(
            IObserver<IObservedChange<TSender, TValue>> downstream,
            ExpressionChainParameters<TSender> parameters)
        {
            _downstream = downstream;
            _source = parameters.Source;
            _expression = parameters.Expression;
            _links = parameters.Links;
            _beforeChange = parameters.BeforeChange;
            _suppressWarnings = parameters.SuppressWarnings;
            _isDistinct = parameters.IsDistinct;
            _notify = parameters.Notify;
            _skipNext = parameters.SkipInitial;
            _levels = new Level[_links.Length];
            for (var i = 0; i < _links.Length; i++)
            {
                _levels[i] = new(this, i, i == _links.Length - 1);
            }
        }

        /// <summary>Establishes the chain from the root value.</summary>
        public void Run()
        {
            lock (_gate)
            {
                if (_links.Length == 0)
                {
                    return;
                }

                _levels[0].SetParent(_source);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_gate)
            {
                _disposed = true;
                for (var i = 0; i < _levels.Length; i++)
                {
                    _levels[i].Dispose();
                }
            }
        }

        /// <summary>Sets the parent value of the level after <paramref name="level"/>.</summary>
        /// <param name="level">The level index that produced the value.</param>
        /// <param name="value">The value the link produced (the parent for the next level).</param>
        private void SetNextParent(int level, object? value) => _levels[level + 1].SetParent(value);

        /// <summary>Handles a leaf raw emission: applies skip-initial, the non-null-parent filter, the cast and the distinct gate.</summary>
        /// <param name="parentMissing">Whether the leaf's parent was null (a raw emission that the non-null filter drops).</param>
        /// <param name="value">The leaf value when the parent is present.</param>
        private void Emit(bool parentMissing, object? value)
        {
            if (_skipNext)
            {
                _skipNext = false;
                return;
            }

            if (parentMissing)
            {
                return;
            }

            TValue typed;
            if (value is null)
            {
                typed = default!;
            }
            else if (value is TValue cast)
            {
                typed = cast;
            }
            else
            {
                _downstream.OnError(new InvalidCastException($"Unable to cast from {value.GetType()} to {typeof(TValue)}."));
                return;
            }

            if (_isDistinct && _hasLast && EqualityComparer<TValue>.Default.Equals(typed, _last))
            {
                return;
            }

            _last = typed;
            _hasLast = true;
            _downstream.OnNext(new ObservedChange<TSender, TValue>(_source!, _expression, typed));
        }

        /// <summary>A single chain link's watcher: re-subscribes on parent change and reads the link's value.</summary>
        /// <param name="sink">The owning chain sink.</param>
        /// <param name="index">This watcher's position in the chain.</param>
        /// <param name="isLeaf">Whether this is the final link in the chain.</param>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        private sealed class Level(Sink sink, int index, bool isLeaf) : IDisposable
        {
            /// <summary>The current link-notification subscription; swapped on each re-parent.</summary>
            private readonly SwapDisposable _subscription = new();

            /// <summary>This link's value fetcher, compiled once at construction rather than re-derived from the
            /// expression on every notification (the chain and fetcher were the dominant per-emit allocation).</summary>
            private readonly Func<object?, object?[]?, object?>? _getter = GetLinkFetcher(sink._links[index]);

            /// <summary>This link's index/argument array (non-null only for indexer links), cached once.</summary>
            private readonly object?[]? _arguments = sink._links[index].GetArgumentsArray();

            /// <summary>Re-establishes this watcher on a new parent value and propagates the current value downward.</summary>
            /// <param name="parent">The object this link is read from.</param>
            public void SetParent(object? parent)
            {
                if (parent is null)
                {
                    _subscription.Disposable = null;
                    if (isLeaf)
                    {
                        sink.Emit(parentMissing: true, null);
                    }
                    else
                    {
                        sink.SetNextParent(index, null);
                    }

                    return;
                }

                var link = sink._links[index];

                // Kicker: propagate the current value immediately, then subscribe for updates.
                Push(ReadValue(parent));
                _subscription.Disposable = sink._notify(parent, link, sink._beforeChange, sink._suppressWarnings)
                    .Subscribe(new Observer(this));
            }

            /// <inheritdoc/>
            public void Dispose() => _subscription.Dispose();

            /// <summary>Handles a notification for this link by re-reading the value and propagating it.</summary>
            /// <param name="change">The notification (its value is read via reflection).</param>
            public void OnNotification(IObservedChange<object?, object?> change)
            {
                lock (sink._gate)
                {
                    if (sink._disposed)
                    {
                        return;
                    }

                    Push(ReadValue(change.Sender));
                }
            }

            /// <summary>Forwards a link-subscription error to the downstream observer.</summary>
            /// <param name="error">The error to forward.</param>
            public void ForwardError(Exception error) => sink._downstream.OnError(error);

            /// <summary>Builds the value fetcher for a link once, returning null when the member has no fetcher.</summary>
            /// <param name="link">The member-access link.</param>
            /// <returns>The cached fetcher, or null for an unsupported member.</returns>
            private static Func<object?, object?[]?, object?>? GetLinkFetcher(Expression link)
            {
                var member = link.GetMemberInfo();
                return member is null ? null : Reflection.GetValueFetcherForProperty(member);
            }

            /// <summary>Reads the current value of this link from a parent using the cached fetcher.</summary>
            /// <param name="parent">The object the link is read from.</param>
            /// <returns>The link's current value, or the default when the parent is null.</returns>
            private object? ReadValue(object? parent)
            {
                if (parent is null)
                {
                    return null;
                }

                // Fast path: the per-link fetcher cached at construction. Fall back to the reflective read only
                // for the rare member with no compiled fetcher (kept for behavioural parity).
                return _getter is not null
                    ? _getter(parent, _arguments)
                    : new ObservedChange<object?, object?>(parent, sink._links[index], null).GetValueOrDefault();
            }

            /// <summary>Forwards this link's value to the next level, or emits it at the leaf.</summary>
            /// <param name="value">The value this link produced.</param>
            private void Push(object? value)
            {
                if (isLeaf)
                {
                    sink.Emit(parentMissing: false, value);
                }
                else
                {
                    sink.SetNextParent(index, value);
                }
            }

            /// <summary>Forwards a link's notifications back into the level.</summary>
            /// <param name="level">The owning level.</param>
            private sealed class Observer(Level level) : IObserver<IObservedChange<object?, object?>>
            {
                /// <inheritdoc/>
                public void OnNext(IObservedChange<object?, object?> value) => level.OnNotification(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => level.ForwardError(error);

                /// <inheritdoc/>
                public void OnCompleted()
                {
                }
            }
        }
    }
}
