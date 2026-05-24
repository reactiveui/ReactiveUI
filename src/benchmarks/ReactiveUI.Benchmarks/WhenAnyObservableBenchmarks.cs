// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks the <c>WhenAnyObservable</c> sink: per-value propagation through a live subscription and cold subscribe.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class WhenAnyObservableBenchmarks
{
    /// <summary>The number of values pushed through the inner observable per emission benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>Sink for the flattened observable values.</summary>
    private readonly NoopObserver<int> _sink = new();

    /// <summary>The view model exposing the inner observable.</summary>
    private ObservableBenchmarkViewModel _viewModel = null!;

    /// <summary>The standing <c>WhenAnyObservable</c> subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates the view model and a standing <c>WhenAnyObservable</c> subscription.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _viewModel = CreateViewModel();
        _subscription = _viewModel.WhenAnyObservable(x => x.Values).Subscribe(_sink);
    }

    /// <summary>Disposes the subscription and the view model.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _subscription.Dispose();
        _viewModel.Dispose();
    }

    /// <summary>Measures value propagation through the live subscription.</summary>
    [Benchmark]
    public void Emit()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _viewModel.Push(i);
        }
    }

    /// <summary>Measures a cold subscribe + dispose of a <c>WhenAnyObservable</c>.</summary>
    [Benchmark]
    public void Subscribe()
    {
        using var subscription = _viewModel.WhenAnyObservable(x => x.Values).Subscribe(_sink);
    }

    /// <summary>Creates a view model exposing the inner observable.</summary>
    /// <returns>The new view model.</returns>
    private static ObservableBenchmarkViewModel CreateViewModel() => new();
}
