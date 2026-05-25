// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;
using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <see cref="CombinedReactiveCommand{TParam, TResult}"/> execution on the immediate scheduler: a combined
/// command that fans a single execute out to three child commands.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class CombinedReactiveCommandBenchmarks
{
    /// <summary>The number of executions per benchmark.</summary>
    private const int ExecuteCount = 10_000;

    /// <summary>The number of child commands composed into the combined command.</summary>
    private const int ChildCount = 3;

    /// <summary>Sink for the combined command's per-child result list.</summary>
    private readonly NoopObserver<IList<Unit>> _sink = new();

    /// <summary>The child commands composed into the combined command.</summary>
    private ReactiveCommand<Unit, Unit>[] _children = null!;

    /// <summary>The combined command under test.</summary>
    private CombinedReactiveCommand<Unit, Unit> _combined = null!;

    /// <summary>Creates the child commands and the combined command on the immediate scheduler.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _children = new ReactiveCommand<Unit, Unit>[ChildCount];
        for (var i = 0; i < ChildCount; i++)
        {
            _children[i] = ReactiveCommand.Create<Unit, Unit>(static parameter => parameter, outputScheduler: ImmediateScheduler.Instance);
        }

        _combined = ReactiveCommand.CreateCombined<Unit, Unit>(_children, null, ImmediateScheduler.Instance);
    }

    /// <summary>Disposes the combined and child commands.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _combined.Dispose();
        for (var i = 0; i < _children.Length; i++)
        {
            _children[i].Dispose();
        }
    }

    /// <summary>Measures repeated execution of the combined command through all child commands.</summary>
    [Benchmark]
    public void Execute()
    {
        for (var i = 0; i < ExecuteCount; i++)
        {
            using var subscription = _combined.Execute().Subscribe(_sink);
        }
    }
}
