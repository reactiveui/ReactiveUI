// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// An observable that never produces any notification (no value, completion, or error). Replaces <c>Observable.Never</c>.
/// </summary>
/// <typeparam name="T">The element type the observable would have produced.</typeparam>
internal sealed class NeverObservable<T> : IObservable<T>
{
    /// <summary>The shared singleton instance.</summary>
    public static readonly NeverObservable<T> Instance = new();

    /// <summary>Prevents a default instance of the <see cref="NeverObservable{T}"/> class from being created externally.</summary>
    private NeverObservable()
    {
    }

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        return EmptyDisposable.Instance;
    }
}
