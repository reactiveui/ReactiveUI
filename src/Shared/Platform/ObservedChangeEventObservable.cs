// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace ReactiveUI.Internal;

/// <summary>
/// Turns a platform notification source — a CLR event, a UIKit control event, an <c>NSNotificationCenter</c>
/// notification — into a stream of observed changes for one fixed sender/member pair. Every such source needs the same
/// three steps on subscribe: attach something, emit a change per raise, detach on dispose. Only the attaching differs,
/// so that is the one thing the caller supplies.
/// </summary>
/// <param name="sender">The object surfaced as the sender on every emitted change.</param>
/// <param name="expression">The expression surfaced on every emitted change.</param>
/// <param name="hook">
/// Attaches to the notification source and returns the <see cref="IDisposable"/> that detaches from it again. Call the
/// supplied <see cref="Action"/> from the handler to emit a change; it captures nothing beyond the subscription, so the
/// handler delegate stays stable and can be used verbatim for the matching detach.
/// </param>
internal sealed class ObservedChangeEventObservable(
    object sender,
    Expression expression,
    Func<Action, IDisposable> hook) : IObservable<IObservedChange<object, object?>>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<IObservedChange<object, object?>> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        return hook(() => observer.OnNext(new ObservedChange<object, object?>(sender, expression, null)));
    }
}
