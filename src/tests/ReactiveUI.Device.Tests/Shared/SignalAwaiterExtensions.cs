// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>Awaits values from a stream with a timeout, so a missing platform callback fails the test instead of hanging it.</summary>
internal static class SignalAwaiterExtensions
{
    /// <summary>The time a device test waits for a platform callback.</summary>
    internal static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    /// <summary>Provides first-value helpers for streams.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The stream to watch.</param>
    extension<T>(IObservable<T> source)
    {
        /// <summary>Subscribes to the stream and returns a task for its first value.</summary>
        /// <param name="subscription">The subscription, which the caller disposes.</param>
        /// <returns>A task that completes with the first value or faults with the stream's error.</returns>
        internal Task<T> FirstValueAsync(out IDisposable subscription)
        {
            ArgumentNullException.ThrowIfNull(source);

            var witness = new FirstWitness<T>();
            subscription = source.Subscribe(witness);
            return witness.Task;
        }
    }

    /// <summary>Provides timeout helpers for tasks that wait on a platform callback.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="task">The task to wait for.</param>
    extension<T>(Task<T> task)
    {
        /// <summary>Waits for the task and fails with a clear message when the time runs out.</summary>
        /// <param name="what">What the test waits for, used in the failure message.</param>
        /// <returns>The task's result.</returns>
        /// <exception cref="TimeoutException">Thrown when the task does not finish within <see cref="DefaultTimeout"/>.</exception>
        internal async Task<T> WithTimeout(string what)
        {
            ArgumentNullException.ThrowIfNull(task);

            if (await Task.WhenAny(task, Task.Delay(DefaultTimeout)).ConfigureAwait(false) != task)
            {
                throw new TimeoutException($"Timed out after {DefaultTimeout.TotalSeconds:0}s waiting for {what}.");
            }

            return await task.ConfigureAwait(false);
        }
    }

    /// <summary>Completes a task with the first value, error or completion it sees.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    private sealed class FirstWitness<T> : IObserver<T>
    {
        /// <summary>The task completion for the first notification.</summary>
        private readonly TaskCompletionSource<T> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>Gets the task that completes with the first value.</summary>
        public Task<T> Task => _completion.Task;

        /// <inheritdoc/>
        public void OnNext(T value) => _completion.TrySetResult(value);

        /// <inheritdoc/>
        public void OnError(Exception error) => _completion.TrySetException(error);

        /// <inheritdoc/>
        public void OnCompleted() => _completion.TrySetException(new InvalidOperationException("The stream completed without a value."));
    }
}
