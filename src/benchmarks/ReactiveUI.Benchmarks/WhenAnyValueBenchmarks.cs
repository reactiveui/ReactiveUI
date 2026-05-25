// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks the <c>WhenAnyValue</c> sinks: cold subscribe cost and per-change value propagation across arity 1–3.
/// Each arity uses its own view model so an emission benchmark drives only the subscription under test.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class WhenAnyValueBenchmarks
{
    /// <summary>The number of property changes pushed per emission benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>Downstream sink for the arity-1 subscription.</summary>
    private readonly NoopObserver<string?> _arity1Sink = new();

    /// <summary>Downstream sink for the arity-2 subscription.</summary>
    private readonly NoopObserver<(string? First, string? Second)> _arity2Sink = new();

    /// <summary>Downstream sink for the arity-3 subscription.</summary>
    private readonly NoopObserver<(string? First, string? Second, int Count)> _arity3Sink = new();

    /// <summary>View model driving the arity-1 subscription.</summary>
    private BenchmarkViewModel _arity1ViewModel = null!;

    /// <summary>View model driving the arity-2 subscription.</summary>
    private BenchmarkViewModel _arity2ViewModel = null!;

    /// <summary>View model driving the arity-3 subscription.</summary>
    private BenchmarkViewModel _arity3ViewModel = null!;

    /// <summary>The live arity-1 subscription.</summary>
    private IDisposable _arity1Subscription = null!;

    /// <summary>The live arity-2 subscription.</summary>
    private IDisposable _arity2Subscription = null!;

    /// <summary>The live arity-3 subscription.</summary>
    private IDisposable _arity3Subscription = null!;

    /// <summary>Creates a dedicated view model and live subscription for each arity.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _arity1ViewModel = new BenchmarkViewModel();
        _arity2ViewModel = new BenchmarkViewModel();
        _arity3ViewModel = new BenchmarkViewModel();

        _arity1Subscription = _arity1ViewModel.WhenAnyValue(x => x.First).Subscribe(_arity1Sink);
        _arity2Subscription = _arity2ViewModel.WhenAnyValue(x => x.First, x => x.Second, (first, second) => (first, second)).Subscribe(_arity2Sink);
        _arity3Subscription = _arity3ViewModel.WhenAnyValue(x => x.First, x => x.Second, x => x.Count, (first, second, count) => (first, second, count)).Subscribe(_arity3Sink);
    }

    /// <summary>Disposes the live subscriptions.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _arity1Subscription.Dispose();
        _arity2Subscription.Dispose();
        _arity3Subscription.Dispose();
    }

    /// <summary>Measures cold subscribe + dispose of a single-property <c>WhenAnyValue</c>.</summary>
    [Benchmark]
    public void Subscribe_Arity1()
    {
        using var subscription = _arity1ViewModel.WhenAnyValue(x => x.First).Subscribe(_arity1Sink);
    }

    /// <summary>Measures cold subscribe + dispose of a two-property <c>WhenAnyValue</c>.</summary>
    [Benchmark]
    public void Subscribe_Arity2()
    {
        using var subscription = _arity2ViewModel.WhenAnyValue(x => x.First, x => x.Second, (first, second) => (first, second)).Subscribe(_arity2Sink);
    }

    /// <summary>Measures cold subscribe + dispose of a three-property <c>WhenAnyValue</c>.</summary>
    [Benchmark]
    public void Subscribe_Arity3()
    {
        using var subscription = _arity3ViewModel.WhenAnyValue(x => x.First, x => x.Second, x => x.Count, (first, second, count) => (first, second, count)).Subscribe(_arity3Sink);
    }

    /// <summary>Measures value propagation through a live single-property subscription.</summary>
    [Benchmark]
    public void Emit_Arity1()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _arity1ViewModel.First = (i & 1) == 0 ? "a" : "b";
        }
    }

    /// <summary>Measures value propagation through a live two-property subscription.</summary>
    [Benchmark]
    public void Emit_Arity2()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _arity2ViewModel.First = (i & 1) == 0 ? "a" : "b";
        }
    }

    /// <summary>Measures value propagation through a live three-property subscription.</summary>
    [Benchmark]
    public void Emit_Arity3()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _arity3ViewModel.Count = i;
        }
    }
}
