// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif
/// <summary>
/// Maps each value of a source observable to <see cref="RxVoid.Default"/>, preserving completion and error.
/// Replaces a <c>.Select(_ =&gt; RxVoid.Default)</c> hop used to erase a handler's element type.
/// </summary>
/// <typeparam name="T">The source element type.</typeparam>
/// <param name="source">The source observable whose values are erased to <see cref="RxVoid"/>.</param>
internal sealed class ToUnitObservable<T>(IObservable<T> source) : IObservable<RxVoid>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<RxVoid> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        return source.Subscribe(new Sink(observer));
    }

    /// <summary>Forwards a unit per source value and relays completion and error.</summary>
    /// <param name="downstream">The observer receiving unit values.</param>
    private sealed class Sink(IObserver<RxVoid> downstream) : IObserver<T>
    {
        /// <inheritdoc/>
        public void OnNext(T value) => downstream.OnNext(RxVoid.Default);

        /// <inheritdoc/>
        public void OnError(Exception error) => downstream.OnError(error);

        /// <inheritdoc/>
        public void OnCompleted() => downstream.OnCompleted();
    }
}
