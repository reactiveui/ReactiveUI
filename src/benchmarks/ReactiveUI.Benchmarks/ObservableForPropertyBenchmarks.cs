// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <c>ObservableForProperty</c>: cold subscribe and per-change observed-change propagation for a single
/// property (the observed-change form behind <c>WhenAnyValue</c>).
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ObservableForPropertyBenchmarks
{
    /// <summary>The number of property changes pushed per emission benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>Sink for the observed-change stream.</summary>
    private readonly NoopObserver<IObservedChange<BenchmarkViewModel, string?>> _sink = new();

    /// <summary>The view model under observation.</summary>
    private BenchmarkViewModel _viewModel = null!;

    /// <summary>The standing observed-change subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates the view model and a standing observed-change subscription.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _viewModel = new BenchmarkViewModel();
        _subscription = _viewModel.ObservableForProperty(x => x.First).Subscribe(_sink);
    }

    /// <summary>Disposes the standing subscription.</summary>
    [GlobalCleanup]
    public void Cleanup() => _subscription.Dispose();

    /// <summary>Measures observed-change propagation through the live subscription.</summary>
    [Benchmark]
    public void Emit()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _viewModel.First = (i & 1) == 0 ? "a" : "b";
        }
    }

    /// <summary>Measures a cold subscribe + dispose of an observed-change stream.</summary>
    [Benchmark]
    public void Subscribe()
    {
        using var subscription = _viewModel.ObservableForProperty(x => x.First).Subscribe(_sink);
    }
}
