// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.MessageBus;

/// <summary>Tests for <see cref="SkipFirstObserver{T}"/>.</summary>
public class SkipFirstObserverTests
{
    /// <summary>The first value is dropped and every subsequent value is forwarded.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task SkipsFirstValueThenForwards()
    {
        const int first = 1;
        const int second = 2;
        const int third = 3;
        var downstream = new CapturingObserver<int>();
        var observer = new SkipFirstObserver<int>(downstream);

        observer.OnNext(first);
        observer.OnNext(second);
        observer.OnNext(third);

        await Assert.That(downstream.Values).IsEquivalentTo([second, third]);
    }

    /// <summary>Errors and completion are forwarded to the downstream observer.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ForwardsErrorAndCompletion()
    {
        var error = new InvalidOperationException("boom");

        var erroring = new CapturingObserver<int>();
        new SkipFirstObserver<int>(erroring).OnError(error);
        await Assert.That(erroring.Error).IsSameReferenceAs(error);

        var completing = new CapturingObserver<int>();
        new SkipFirstObserver<int>(completing).OnCompleted();
        await Assert.That(completing.Completed).IsTrue();
    }

    /// <summary>An observer that records the values, error and completion it receives.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    private sealed class CapturingObserver<T> : IObserver<T>
    {
        /// <summary>Gets the values that were forwarded.</summary>
        public List<T> Values { get; } = [];

        /// <summary>Gets the error that was forwarded, if any.</summary>
        public Exception? Error { get; private set; }

        /// <summary>Gets a value indicating whether completion was forwarded.</summary>
        public bool Completed { get; private set; }

        /// <inheritdoc/>
        public void OnNext(T value) => Values.Add(value);

        /// <inheritdoc/>
        public void OnError(Exception error) => Error = error;

        /// <inheritdoc/>
        public void OnCompleted() => Completed = true;
    }
}
