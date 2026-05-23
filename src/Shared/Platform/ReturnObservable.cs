// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Emits a single value and then completes, synchronously on subscribe. The allocation-light replacement for
/// <c>Observable.Return(value)</c>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="value">The value to emit.</param>
internal sealed class ReturnObservable<T>(T value) : IObservable<T>
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
