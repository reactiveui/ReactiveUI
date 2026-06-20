// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Benchmarks;

/// <summary>A reactive object exposing a stable inner observable, used to drive the <c>WhenAnyObservable</c> benchmarks.</summary>
internal sealed class ObservableBenchmarkViewModel : ReactiveObject, IDisposable
{
    /// <summary>The inner observable values are pushed through.</summary>
    private readonly ReactiveUI.Primitives.Signals.Signal<int> _subject = new();

    /// <summary>Gets the observable that <c>WhenAnyObservable</c> subscribes to.</summary>
    public IObservable<int> Values => _subject;

    /// <summary>Pushes a value through the inner observable.</summary>
    /// <param name="value">The value to emit.</param>
    public void Push(int value) => _subject.OnNext(value);

    /// <inheritdoc/>
    public void Dispose() => _subject.Dispose();
}
