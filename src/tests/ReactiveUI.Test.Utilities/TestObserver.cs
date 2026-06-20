// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Creates lightweight <see cref="IObserver{T}"/> instances from delegates. Neutral replacement for the
/// System.Reactive <c>Observer.Create</c> factory so test source compiles against both the Primitives and
/// System.Reactive operator surfaces.
/// </summary>
public static class TestObserver
{
    /// <summary>Creates an observer that forwards each value to <paramref name="onNext"/> and rethrows on error.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="onNext">Invoked for each value the observer receives.</param>
    /// <returns>An observer that invokes <paramref name="onNext"/> for every value.</returns>
    public static IObserver<T> Create<T>(Action<T> onNext) => new DelegateObserver<T>(onNext);

    /// <summary>An observer that forwards values to a delegate.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="onNext">The delegate invoked for each value.</param>
    private sealed class DelegateObserver<T>(Action<T> onNext) : IObserver<T>
    {
        /// <inheritdoc/>
        public void OnNext(T value) => onNext(value);

        /// <inheritdoc/>
        public void OnError(Exception error) => throw error;

        /// <inheritdoc/>
        public void OnCompleted()
        {
            // No completion handling needed for a test observer.
        }
    }
}
