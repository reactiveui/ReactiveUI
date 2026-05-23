// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;

namespace ReactiveUI.Internal;

/// <summary>
/// Maps each value of a source observable to <see cref="Unit.Default"/>, preserving completion and error.
/// Replaces a <c>.Select(_ =&gt; Unit.Default)</c> hop used to erase a handler's element type.
/// </summary>
/// <typeparam name="T">The source element type.</typeparam>
/// <param name="source">The source observable whose values are erased to <see cref="Unit"/>.</param>
internal sealed class ToUnitObservable<T>(IObservable<T> source) : IObservable<Unit>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<Unit> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        return source.Subscribe(new Sink(observer));
    }

    /// <summary>Forwards a unit per source value and relays completion and error.</summary>
    /// <param name="downstream">The observer receiving unit values.</param>
    private sealed class Sink(IObserver<Unit> downstream) : IObserver<T>
    {
        /// <inheritdoc/>
        public void OnNext(T value) => downstream.OnNext(Unit.Default);

        /// <inheritdoc/>
        public void OnError(Exception error) => downstream.OnError(error);

        /// <inheritdoc/>
        public void OnCompleted() => downstream.OnCompleted();
    }
}
