// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>Delegate-backed observer used internally where a one-shot anonymous subscriber is convenient.</summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="onNext">Per-value callback.</param>
/// <param name="onError">Error callback.</param>
/// <param name="onCompleted">Completion callback.</param>
internal sealed class DelegateObserver<T>(
    Action<T> onNext,
    Action<Exception>? onError = null,
    Action? onCompleted = null) : IObserver<T>
{
    /// <inheritdoc/>
    public void OnNext(T value) => onNext(value);

    /// <inheritdoc/>
    public void OnError(Exception error) => onError?.Invoke(error);

    /// <inheritdoc/>
    public void OnCompleted() => onCompleted?.Invoke();
}
