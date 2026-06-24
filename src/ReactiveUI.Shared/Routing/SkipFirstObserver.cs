// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Forwards every value except the first to the downstream observer.</summary>
/// <typeparam name="T">The message type.</typeparam>
/// <param name="downstream">The observer receiving values after the first.</param>
internal sealed class SkipFirstObserver<T>(IObserver<T> downstream) : IObserver<T>
{
    /// <summary>Whether the first value has been skipped.</summary>
    private bool _skipped;

    /// <inheritdoc/>
    public void OnNext(T value)
    {
        if (!_skipped)
        {
            _skipped = true;
            return;
        }

        downstream.OnNext(value);
    }

    /// <inheritdoc/>
    public void OnError(Exception error) => downstream.OnError(error);

    /// <inheritdoc/>
    public void OnCompleted() => downstream.OnCompleted();
}
