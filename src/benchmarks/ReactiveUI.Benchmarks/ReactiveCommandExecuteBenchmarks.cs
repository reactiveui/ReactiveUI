// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;
using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <see cref="ReactiveCommand"/> execution on the immediate scheduler, for both the synchronous and
/// task-based pipelines.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ReactiveCommandExecuteBenchmarks
{
    /// <summary>The number of executions per benchmark.</summary>
    private const int ExecuteCount = 10_000;

    /// <summary>Sink for synchronous command results.</summary>
    private readonly NoopObserver<Unit> _syncSink = new();

    /// <summary>Sink for task-based command results.</summary>
    private readonly NoopObserver<int> _taskSink = new();

    /// <summary>The synchronous command under test.</summary>
    private ReactiveCommand<Unit, Unit> _syncCommand = null!;

    /// <summary>The task-based command under test.</summary>
    private ReactiveCommand<int, int> _taskCommand = null!;

    /// <summary>Creates both commands with the immediate output scheduler for deterministic, synchronous execution.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _syncCommand = ReactiveCommand.Create<Unit, Unit>(static parameter => parameter, outputScheduler: ImmediateScheduler.Instance);
        _taskCommand = ReactiveCommand.CreateFromTask<int, int>(static parameter => Task.FromResult(parameter), outputScheduler: ImmediateScheduler.Instance);
    }

    /// <summary>Disposes both commands.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _syncCommand.Dispose();
        _taskCommand.Dispose();
    }

    /// <summary>Measures repeated synchronous command execution.</summary>
    [Benchmark]
    public void ExecuteSync()
    {
        for (var i = 0; i < ExecuteCount; i++)
        {
            using var subscription = _syncCommand.Execute().Subscribe(_syncSink);
        }
    }

    /// <summary>Measures repeated task-based command execution (synchronously completed task).</summary>
    [Benchmark]
    public void ExecuteTask()
    {
        for (var i = 0; i < ExecuteCount; i++)
        {
            using var subscription = _taskCommand.Execute(i).Subscribe(_taskSink);
        }
    }
}
