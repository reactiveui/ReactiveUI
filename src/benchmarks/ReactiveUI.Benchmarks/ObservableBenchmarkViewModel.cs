// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Signals;

namespace ReactiveUI.Benchmarks;

/// <summary>A reactive object exposing a stable inner observable, used to drive the <c>WhenAnyObservable</c> benchmarks.</summary>
internal sealed class ObservableBenchmarkViewModel : ReactiveObject, IDisposable
{
    /// <summary>The inner observable values are pushed through.</summary>
    private readonly Signal<int> _subject = new();

    /// <summary>Gets the observable that <c>WhenAnyObservable</c> subscribes to.</summary>
    internal IObservable<int> Values => _subject;

    /// <inheritdoc/>
    public void Dispose() => _subject.Dispose();

    /// <summary>Pushes a value through the inner observable.</summary>
    /// <param name="value">The value to emit.</param>
    internal void Push(int value) => _subject.OnNext(value);
}
