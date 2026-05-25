// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <see cref="Interaction{TInput, TOutput}"/> handler dispatch on the immediate scheduler: repeated
/// <c>Handle</c> through a registered handler, and a cold register + handle + dispose cycle.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class InteractionBenchmarks
{
    /// <summary>The number of <c>Handle</c> calls per benchmark invocation.</summary>
    private const int HandleCount = 10_000;

    /// <summary>Sink for the handled interaction output.</summary>
    private readonly NoopObserver<int> _sink = new();

    /// <summary>The interaction under test.</summary>
    private Interaction<int, int> _interaction = null!;

    /// <summary>The registration handle for the standing handler.</summary>
    private IDisposable _handler = null!;

    /// <summary>Creates an interaction on the immediate scheduler and registers a synchronous handler.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _interaction = new Interaction<int, int>(ImmediateScheduler.Instance);
        _handler = _interaction.RegisterHandler(static context => context.SetOutput(context.Input));
    }

    /// <summary>Disposes the standing handler registration.</summary>
    [GlobalCleanup]
    public void Cleanup() => _handler.Dispose();

    /// <summary>Measures repeated handling through the standing handler.</summary>
    [Benchmark]
    public void Handle()
    {
        for (var i = 0; i < HandleCount; i++)
        {
            using var subscription = _interaction.Handle(i).Subscribe(_sink);
        }
    }

    /// <summary>Measures a cold register-handler + handle + dispose cycle.</summary>
    [Benchmark]
    public void RegisterAndHandle()
    {
        using var handler = _interaction.RegisterHandler(static context => context.SetOutput(context.Input));
        using var subscription = _interaction.Handle(1).Subscribe(_sink);
    }
}
