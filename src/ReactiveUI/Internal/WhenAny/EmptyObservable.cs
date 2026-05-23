// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// An observable that completes immediately on subscription without producing a value.
/// Used by the WhenAnyObservable mixins to stand in for a null inner observable.
/// </summary>
/// <typeparam name="T">The element type the observable would have produced.</typeparam>
internal sealed class EmptyObservable<T> : IObservable<T>
{
    /// <summary>
    /// The shared singleton instance.
    /// </summary>
    public static readonly EmptyObservable<T> Instance = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyObservable{T}"/> class.
    /// </summary>
    private EmptyObservable()
    {
    }

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        observer.OnCompleted();
        return EmptyDisposable.Instance;
    }
}
