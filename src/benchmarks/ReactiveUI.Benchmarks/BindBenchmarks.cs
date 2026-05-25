// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks the property-binding sinks: cold one-way/two-way binding setup and view-model to view propagation.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class BindBenchmarks
{
    /// <summary>The number of source changes pushed per propagation benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>View model for the one-way binding.</summary>
    private BenchmarkViewModel _oneWayViewModel = null!;

    /// <summary>The live one-way binding (which keeps its view alive).</summary>
    private IDisposable _oneWayBinding = null!;

    /// <summary>View model for the two-way binding.</summary>
    private BenchmarkViewModel _twoWayViewModel = null!;

    /// <summary>The live two-way binding (which keeps its view alive).</summary>
    private IDisposable _twoWayBinding = null!;

    /// <summary>Creates the views, view models, and live bindings for the propagation benchmarks.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _oneWayViewModel = new BenchmarkViewModel();
        var oneWayView = new BenchmarkView { ViewModel = _oneWayViewModel };
        _oneWayBinding = oneWayView.OneWayBind(_oneWayViewModel, x => x.First, x => x.FirstText);

        _twoWayViewModel = new BenchmarkViewModel();
        var twoWayView = new BenchmarkView { ViewModel = _twoWayViewModel };
        _twoWayBinding = twoWayView.Bind(_twoWayViewModel, x => x.First, x => x.FirstText);
    }

    /// <summary>Disposes the live bindings.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _oneWayBinding.Dispose();
        _twoWayBinding.Dispose();
    }

    /// <summary>Measures cold setup + dispose of a one-way binding.</summary>
    [Benchmark]
    public void OneWayBind_Setup()
    {
        var view = new BenchmarkView { ViewModel = _oneWayViewModel };
        using var binding = view.OneWayBind(_oneWayViewModel, x => x.First, x => x.FirstText);
    }

    /// <summary>Measures cold setup + dispose of a two-way binding.</summary>
    [Benchmark]
    public void Bind_Setup()
    {
        var view = new BenchmarkView { ViewModel = _twoWayViewModel };
        using var binding = view.Bind(_twoWayViewModel, x => x.First, x => x.FirstText);
    }

    /// <summary>Measures view-model to view propagation through a live one-way binding.</summary>
    [Benchmark]
    public void OneWayBind_Propagate()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _oneWayViewModel.First = (i & 1) == 0 ? "a" : "b";
        }
    }

    /// <summary>Measures view-model to view propagation through a live two-way binding.</summary>
    [Benchmark]
    public void Bind_Propagate()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _twoWayViewModel.First = (i & 1) == 0 ? "a" : "b";
        }
    }
}
