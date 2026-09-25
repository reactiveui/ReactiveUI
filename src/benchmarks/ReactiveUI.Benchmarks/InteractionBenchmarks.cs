// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <see cref="Interaction{TInput, TOutput}"/> task-based handler dispatch: repeated
/// <c>Handle</c> through a registered handler, and a register + handle + dispose cycle.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[DebuggerDisplay("InteractionBenchmarks")]
public class InteractionBenchmarks
{
    /// <summary>The number of <c>Handle</c> calls per benchmark invocation.</summary>
    private const int HandleCount = 10_000;

    /// <summary>The interaction under test.</summary>
    private Interaction<int, int> _interaction = null!;

    /// <summary>The registration handle for the standing handler.</summary>
    private IDisposable _handler = null!;

    /// <summary>Creates an interaction and registers a synchronous handler.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _interaction = new();
        _handler = _interaction.RegisterHandler(static context => context.SetOutput(context.Input));
    }

    /// <summary>Disposes the standing handler registration.</summary>
    [GlobalCleanup]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Cleanup() => _handler.Dispose();

    /// <summary>Measures repeated handling through the standing handler.</summary>
    /// <returns>A task that completes after all interaction outputs have been received.</returns>
    [Benchmark]
    public async Task Handle()
    {
        for (var i = 0; i < HandleCount; i++)
        {
            _ = await _interaction.Handle(i).ConfigureAwait(false);
        }
    }

    /// <summary>Measures a cold register-handler + handle + dispose cycle.</summary>
    /// <returns>A task that completes after the registered handler responds.</returns>
    [Benchmark]
    public async Task RegisterAndHandle()
    {
        using var handler = _interaction.RegisterHandler(static context => context.SetOutput(context.Input));
        _ = await _interaction.Handle(1).ConfigureAwait(false);
    }
}
