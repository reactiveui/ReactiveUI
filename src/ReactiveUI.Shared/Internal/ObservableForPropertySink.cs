// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif
/// <summary>
/// Single-layer observable for a single property: optionally emits the current value on subscribe, then re-reads and
/// emits the property value on each notification from the underlying change source, optionally suppressing duplicate
/// values. Replaces an <c>Observable.Create</c> + <c>DistinctUntilChanged</c> pair.
/// </summary>
/// <typeparam name="TSender">The type of the observed object.</typeparam>
/// <typeparam name="TValue">The property value type.</typeparam>
/// <param name="sender">The observed object.</param>
/// <param name="expression">The expression surfaced on the observed change.</param>
/// <param name="propertyName">The name of the observed property.</param>
/// <param name="notifications">The underlying change-notification source (a tick re-reads the value).</param>
/// <param name="getValue">Reads the current property value from the sender.</param>
/// <param name="skipInitial">When true, the current value is not emitted on subscribe.</param>
/// <param name="isDistinct">When true, consecutive equal values are suppressed.</param>
internal sealed class ObservableForPropertySink<TSender, TValue>(
    TSender sender,
    Expression expression,
    string propertyName,
    IObservable<IObservedChange<object?, object?>> notifications,
    Func<TSender, string, TValue> getValue,
    bool skipInitial,
    bool isDistinct) : IObservable<IObservedChange<TSender, TValue>>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<IObservedChange<TSender, TValue>> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        var sink = new Sink(observer, sender, expression, propertyName, getValue, isDistinct);
        return sink.Run(notifications, skipInitial);
    }

    /// <summary>Reads the property value on each notification and forwards it as an observed change.</summary>
    private sealed class Sink : IObserver<IObservedChange<object?, object?>>
    {
        /// <summary>The observer receiving observed changes.</summary>
        private readonly IObserver<IObservedChange<TSender, TValue>> _downstream;

        /// <summary>The observed object.</summary>
        private readonly TSender _sender;

        /// <summary>The expression surfaced on the observed change.</summary>
        private readonly Expression _expression;

        /// <summary>The name of the observed property.</summary>
        private readonly string _propertyName;

        /// <summary>Reads the current property value from the sender.</summary>
        private readonly Func<TSender, string, TValue> _getValue;

        /// <summary>Whether consecutive equal values are suppressed.</summary>
        private readonly bool _isDistinct;

        /// <summary>The last emitted value, used by the distinct gate.</summary>
        private TValue _last = default!;

        /// <summary>Whether <see cref="_last"/> holds a value yet.</summary>
        private bool _hasLast;

        /// <summary>Initializes a new instance of the <see cref="Sink"/> class.</summary>
        /// <param name="downstream">The observer receiving observed changes.</param>
        /// <param name="sender">The observed object.</param>
        /// <param name="expression">The expression surfaced on the observed change.</param>
        /// <param name="propertyName">The name of the observed property.</param>
        /// <param name="getValue">Reads the current property value.</param>
        /// <param name="isDistinct">Whether consecutive equal values are suppressed.</param>
        public Sink(
            IObserver<IObservedChange<TSender, TValue>> downstream,
            TSender sender,
            Expression expression,
            string propertyName,
            Func<TSender, string, TValue> getValue,
            bool isDistinct)
        {
            _downstream = downstream;
            _sender = sender;
            _expression = expression;
            _propertyName = propertyName;
            _getValue = getValue;
            _isDistinct = isDistinct;
        }

        /// <summary>Optionally emits the current value, then subscribes to the notification source.</summary>
        /// <param name="notifications">The change-notification source.</param>
        /// <param name="skipInitial">When true, the current value is not emitted on subscribe.</param>
        /// <returns>The notification-source subscription.</returns>
        public IDisposable Run(IObservable<IObservedChange<object?, object?>> notifications, bool skipInitial)
        {
            if (!skipInitial)
            {
                Emit();
            }

            return notifications.Subscribe(this);
        }

        /// <inheritdoc/>
        public void OnNext(IObservedChange<object?, object?> value) => Emit();

        /// <inheritdoc/>
        public void OnError(Exception error) => _downstream.OnError(error);

        /// <inheritdoc/>
        public void OnCompleted() => _downstream.OnCompleted();

        /// <summary>Reads the current property value and forwards it as an observed change, honoring the distinct gate.</summary>
        private void Emit()
        {
            TValue current;
            try
            {
                current = _getValue(_sender, _propertyName);
            }
            catch (Exception ex)
            {
                _downstream.OnError(ex);
                return;
            }

            if (_isDistinct && _hasLast && EqualityComparer<TValue>.Default.Equals(current, _last))
            {
                return;
            }

            _last = current;
            _hasLast = true;
            _downstream.OnNext(new ObservedChange<TSender, TValue>(_sender, _expression, current));
        }
    }
}
