// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Emits a seed value on subscribe and then forwards the source — a fused replacement for <c>StartWith(value)</c>
/// with no intermediate operator subscription.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="source">The source observable.</param>
/// <param name="value">The seed value emitted before the source.</param>
internal sealed class StartWithObservable<T>(IObservable<T> source, T value) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        observer.OnNext(value);
        return source.Subscribe(observer);
    }
}
