// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Terminal no-op observer used as the downstream sink in benchmarks. It records callback counts so the JIT cannot
/// elide the subscription and so no allocations leak in from a closure-based sink.
/// </summary>
/// <typeparam name="T">The element type the sink consumes.</typeparam>
internal sealed class NoopObserver<T> : IObserver<T>
{
    /// <summary>Gets the number of <see cref="OnNext"/> callbacks received.</summary>
    internal long NextCount { get; private set; }

    /// <summary>Gets the number of <see cref="OnError"/> callbacks received.</summary>
    internal long ErrorCount { get; private set; }

    /// <summary>Gets the number of <see cref="OnCompleted"/> callbacks received.</summary>
    internal long CompletedCount { get; private set; }

    /// <inheritdoc/>
    public void OnNext(T value) => NextCount++;

    /// <inheritdoc/>
    public void OnError(Exception error) => ErrorCount++;

    /// <inheritdoc/>
    public void OnCompleted() => CompletedCount++;
}
