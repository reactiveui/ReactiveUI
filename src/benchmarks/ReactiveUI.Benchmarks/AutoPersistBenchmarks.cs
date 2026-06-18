// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <c>AutoPersist</c>'s change-watching pipeline: each persistable property change is detected, filtered
/// and fed through the throttle that drives persistence.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class AutoPersistBenchmarks
{
    /// <summary>The number of persistable property changes pushed per benchmark invocation.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>The view model whose changes drive persistence.</summary>
    private BenchmarkViewModel _viewModel = null!;

    /// <summary>The auto-persist registration.</summary>
    private IDisposable _persistence = null!;

    /// <summary>Creates the view model and registers auto-persistence on its integer property.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _viewModel = new();
        _persistence = _viewModel.AutoPersist(
            static _ => Signal.Emit(RxVoid.Default),
            new(true, new HashSet<string> { nameof(BenchmarkViewModel.Count) }),
            TimeSpan.FromMilliseconds(1));
    }

    /// <summary>Disposes the auto-persist registration.</summary>
    [GlobalCleanup]
    public void Cleanup() => _persistence.Dispose();

    /// <summary>Measures the change-detection / filter / throttle pipeline as a persistable property changes.</summary>
    [Benchmark]
    public void WatchChanges()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _viewModel.Count = i;
        }
    }
}
