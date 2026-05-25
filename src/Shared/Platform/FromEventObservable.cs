// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// A minimal event-to-observable bridge. Unlike <c>Observable.FromEvent</c>/<c>FromEventPattern</c> it performs no
/// reflection, has no scheduler, queue, or delegate-type juggling: each subscription simply invokes the supplied
/// <paramref name="subscribe"/> function, which wires the CLR event (<c>+=</c>) directly and returns the disposable
/// that unwires it (<c>-=</c>). This keeps event sourcing explicit and allocation-light — one captured action plus
/// whatever disposable the wiring returns, per subscription.
/// </summary>
/// <typeparam name="T">The value projected from each event raise and pushed to observers.</typeparam>
/// <param name="subscribe">
/// Wires the event and returns an <see cref="IDisposable"/> that unwires it. The supplied <see cref="Action{T}"/> is
/// the observer's <see cref="IObserver{T}.OnNext"/>; call it with the projected value when the event raises.
/// </param>
internal sealed class FromEventObservable<T>(Func<Action<T>, IDisposable> subscribe) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        return subscribe(observer.OnNext);
    }
}
