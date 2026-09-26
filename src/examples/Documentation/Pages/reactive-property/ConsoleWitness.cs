// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>An <see cref="IObserver{T}"/> that prints every notification it receives.</summary>
/// <typeparam name="T">The type of value the witness observes.</typeparam>
/// <param name="name">The name printed before each notification.</param>
public sealed class ConsoleWitness<T>(string name) : IObserver<T>
{
    /// <inheritdoc/>
    public void OnNext(T value) => Console.WriteLine($"{name}: {value}");

    /// <inheritdoc/>
    public void OnError(Exception error) => Console.WriteLine($"{name} failed: {error.Message}");

    /// <inheritdoc/>
    public void OnCompleted() => Console.WriteLine($"{name} completed");
}
