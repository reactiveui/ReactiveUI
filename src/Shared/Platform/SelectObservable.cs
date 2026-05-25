// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Projects each value of a source through a selector — a fused, allocation-light replacement for <c>Select</c>. It
/// adds a single observer layer with no operator pipeline, forwarding errors and completion unchanged.
/// </summary>
/// <typeparam name="TSource">The source element type.</typeparam>
/// <typeparam name="TResult">The projected element type.</typeparam>
/// <param name="source">The source to project.</param>
/// <param name="selector">Projects each source value into a result.</param>
internal sealed class SelectObservable<TSource, TResult>(IObservable<TSource> source, Func<TSource, TResult> selector)
    : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        return source.Subscribe(new Sink(observer, selector));
    }

    /// <summary>Projects each source value downstream.</summary>
    /// <param name="downstream">The downstream observer.</param>
    /// <param name="selector">Projects each source value into a result.</param>
    private sealed class Sink(IObserver<TResult> downstream, Func<TSource, TResult> selector) : IObserver<TSource>
    {
        /// <inheritdoc/>
        public void OnNext(TSource value) => downstream.OnNext(selector(value));

        /// <inheritdoc/>
        public void OnError(Exception error) => downstream.OnError(error);

        /// <inheritdoc/>
        public void OnCompleted() => downstream.OnCompleted();
    }
}
