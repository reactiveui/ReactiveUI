// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks the <c>ToProperty</c> / <see cref="ObservableAsPropertyHelper{T}"/> pipeline: helper construction and
/// per-change value propagation into the derived property.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ToPropertyBenchmarks
{
    /// <summary>The number of source changes pushed per propagation benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>The multiplier applied to the source value in the derived projection.</summary>
    private const int Multiplier = 2;

    /// <summary>The name the derived OAPH raises change notifications for.</summary>
    private const string DerivedPropertyName = "Doubled";

    /// <summary>The source view model whose <see cref="BenchmarkViewModel.Count"/> drives the OAPH.</summary>
    private BenchmarkViewModel _viewModel = null!;

    /// <summary>The live OAPH under test.</summary>
    private ObservableAsPropertyHelper<int> _doubled = null!;

    /// <summary>Creates the source view model and wires a live OAPH derived from its count.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _viewModel = new BenchmarkViewModel();
        _doubled = _viewModel.WhenAnyValue(x => x.Count)
            .Select(static count => count * Multiplier)
            .ToProperty(_viewModel, DerivedPropertyName);
    }

    /// <summary>Disposes the live OAPH.</summary>
    [GlobalCleanup]
    public void Cleanup() => _doubled.Dispose();

    /// <summary>Measures wiring + disposing a fresh OAPH against the source view model.</summary>
    /// <returns>The derived value so the construction cannot be elided.</returns>
    [Benchmark]
    public int Create()
    {
        using var helper = _viewModel.WhenAnyValue(x => x.Count)
            .Select(static count => count * Multiplier)
            .ToProperty(_viewModel, DerivedPropertyName);
        return helper.Value;
    }

    /// <summary>Measures source-change propagation into the live OAPH.</summary>
    [Benchmark]
    public void Emit()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _viewModel.Count = i;
        }
    }
}
