// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// An observable that delivers a single error to every subscriber on subscription. Replaces <c>Observable.Throw</c>.
/// </summary>
/// <typeparam name="T">The (never-produced) element type.</typeparam>
/// <param name="error">The error delivered to subscribers.</param>
internal sealed class ThrowObservable<T>(Exception error) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        observer.OnError(error);
        return EmptyDisposable.Instance;
    }
}
