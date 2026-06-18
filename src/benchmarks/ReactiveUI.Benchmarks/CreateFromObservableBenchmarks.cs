// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <c>ReactiveCommand.CreateFromObservable</c> execution on the immediate scheduler with a synchronously
/// completing execution observable.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class CreateFromObservableBenchmarks
{
    /// <summary>The number of executions per benchmark.</summary>
    private const int ExecuteCount = 10_000;

    /// <summary>Sink for the command results.</summary>
    private readonly NoopObserver<RxVoid> _sink = new();

    /// <summary>The observable-backed command under test.</summary>
    private ReactiveCommand<RxVoid, RxVoid> _command = null!;

    /// <summary>Creates an observable-backed command on the immediate scheduler.</summary>
    [GlobalSetup]
    public void Setup() =>
        _command = ReactiveCommand.CreateFromObservable(
            static () => Signal.Emit(RxVoid.Default),
            outputScheduler: Sequencer.Immediate);

    /// <summary>Disposes the command.</summary>
    [GlobalCleanup]
    public void Cleanup() => _command.Dispose();

    /// <summary>Measures repeated execution of the observable-backed command.</summary>
    [Benchmark]
    public void Execute()
    {
        for (var i = 0; i < ExecuteCount; i++)
        {
            using var subscription = _command.Execute().Subscribe(_sink);
        }
    }
}
