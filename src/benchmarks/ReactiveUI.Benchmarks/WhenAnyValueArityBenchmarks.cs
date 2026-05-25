// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks a higher-arity <c>WhenAnyValue</c> (arity 5), exercising the arity-5 sink: cold subscribe and per-change
/// propagation.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class WhenAnyValueArityBenchmarks
{
    /// <summary>The number of property changes pushed per emission benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>Sink for the combined arity-5 projection.</summary>
    private readonly NoopObserver<int> _sink = new();

    /// <summary>The view model driving the arity-5 subscription.</summary>
    private BenchmarkViewModel _viewModel = null!;

    /// <summary>The live arity-5 subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates the view model and a live arity-5 subscription.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _viewModel = new BenchmarkViewModel();
        _subscription = Observe(_viewModel).Subscribe(_sink);
    }

    /// <summary>Disposes the live subscription.</summary>
    [GlobalCleanup]
    public void Cleanup() => _subscription.Dispose();

    /// <summary>Measures cold subscribe + dispose of a five-property <c>WhenAnyValue</c>.</summary>
    [Benchmark]
    public void Subscribe_Arity5()
    {
        using var subscription = Observe(_viewModel).Subscribe(_sink);
    }

    /// <summary>Measures propagation through the live arity-5 subscription.</summary>
    [Benchmark]
    public void Emit_Arity5()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _viewModel.Count = i;
        }
    }

    /// <summary>Builds the arity-5 projection observed by the benchmarks.</summary>
    /// <param name="viewModel">The view model to observe.</param>
    /// <returns>An observable of the combined projection.</returns>
    private static IObservable<int> Observe(BenchmarkViewModel viewModel) =>
        viewModel.WhenAnyValue(
            x => x.First,
            x => x.Second,
            x => x.Third,
            x => x.Fourth,
            x => x.Count,
            static (first, second, third, fourth, count) => (first?.Length ?? 0) + (second?.Length ?? 0) + (third?.Length ?? 0) + (fourth?.Length ?? 0) + count);
}
