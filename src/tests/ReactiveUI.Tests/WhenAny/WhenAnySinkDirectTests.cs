// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>
/// Direct tests for the internal <c>WhenAnyValueSink</c> and <c>WhenAnyChangeSink</c> combinators, exercising the
/// per-source emit branches, error forwarding, source-completion, and selector-exception paths that the public
/// <c>WhenAnyValue</c> API cannot reach (property-change observables never error or complete). Each arity lives in
/// its own partial-class file.
/// </summary>
public partial class WhenAnySinkDirectTests
{
    /// <summary>Wraps a string as an <see cref="IObservedChange{TSender, TValue}"/> with a null sender.</summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>An observed-change carrying the value.</returns>
    private static ObservedChange<object?, string> Ch(string value) => new(null!, null, value);

    /// <summary>Records the notifications delivered to an observer for assertion.</summary>
    /// <typeparam name="T">The notification value type.</typeparam>
    private sealed class Recorder<T> : IObserver<T>
    {
        /// <summary>Gets the values delivered via <see cref="OnNext"/>.</summary>
        public List<T> Values { get; } = [];

        /// <summary>Gets the errors delivered via <see cref="OnError"/>.</summary>
        public List<Exception> Errors { get; } = [];

        /// <summary>Gets the number of times <see cref="OnCompleted"/> was called.</summary>
        public int Completed { get; private set; }

        /// <inheritdoc/>
        public void OnNext(T value) => Values.Add(value);

        /// <inheritdoc/>
        public void OnError(Exception error) => Errors.Add(error);

        /// <inheritdoc/>
        public void OnCompleted() => Completed++;
    }
}
