// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// An observable that signals an error to every subscriber on subscription — a tailored replacement for
/// <c>Observable.Throw</c>.
/// </summary>
/// <typeparam name="T">The element type the observable would have produced.</typeparam>
/// <param name="exception">The error to signal to each subscriber.</param>
internal sealed class ThrowObservable<T>(Exception exception) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        observer.OnError(exception);
        return EmptyDisposable.Instance;
    }
}
