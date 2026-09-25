// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>Benchmarks generated <c>WhenChanged</c> subscription and per-change value delivery.</summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[DebuggerDisplay("WhenChangedBenchmarks")]
public class WhenChangedBenchmarks
{
    /// <summary>The number of property changes pushed per emission benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>Sink for the observed value stream.</summary>
    private readonly NoopObserver<string?> _sink = new();

    /// <summary>The view model under observation.</summary>
    private BenchmarkViewModel _viewModel = null!;

    /// <summary>The standing observed-change subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates the view model and a standing generated observation.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _viewModel = new();
        _subscription = _viewModel.WhenChanged(static x => x.First).Subscribe(_sink);
    }

    /// <summary>Disposes the standing subscription.</summary>
    [GlobalCleanup]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Cleanup() => _subscription.Dispose();

    /// <summary>Measures generated value delivery through the live subscription.</summary>
    [Benchmark]
    public void Emit()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _viewModel.First = (i & 1) == 0 ? "a" : "b";
        }
    }

    /// <summary>Measures a cold subscribe and dispose of a generated observation.</summary>
    [Benchmark]
    public void Subscribe()
    {
        using var subscription = _viewModel.WhenChanged(static x => x.First).Subscribe(_sink);
    }
}
