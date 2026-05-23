// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// A cold observable that emits a single value and then completes on each subscription.
/// Replaces <c>Observable.Return</c> for internal use.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="value">The value emitted to each subscriber.</param>
internal sealed class SingleValueObservable<T>(T value) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        observer.OnNext(value);
        observer.OnCompleted();
        return EmptyDisposable.Instance;
    }
}
