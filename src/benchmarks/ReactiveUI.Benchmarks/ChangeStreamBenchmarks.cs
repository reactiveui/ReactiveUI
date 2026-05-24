// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks the raw <see cref="ReactiveObject"/> change streams (<c>Changed</c> / <c>Changing</c>) that back the
/// notification pipeline, with a live subscription receiving every property change.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ChangeStreamBenchmarks
{
    /// <summary>The number of property changes pushed per benchmark invocation.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>Sink for the changed stream.</summary>
    private readonly NoopObserver<IReactivePropertyChangedEventArgs<IReactiveObject>> _changedSink = new();

    /// <summary>Sink for the changing stream.</summary>
    private readonly NoopObserver<IReactivePropertyChangedEventArgs<IReactiveObject>> _changingSink = new();

    /// <summary>View model driving the changed stream.</summary>
    private BenchmarkViewModel _changedViewModel = null!;

    /// <summary>View model driving the changing stream.</summary>
    private BenchmarkViewModel _changingViewModel = null!;

    /// <summary>Live subscription to the changed stream.</summary>
    private IDisposable _changedSubscription = null!;

    /// <summary>Live subscription to the changing stream.</summary>
    private IDisposable _changingSubscription = null!;

    /// <summary>Creates view models and live subscriptions to the changed and changing streams.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _changedViewModel = new BenchmarkViewModel();
        _changingViewModel = new BenchmarkViewModel();
        _changedSubscription = _changedViewModel.Changed.Subscribe(_changedSink);
        _changingSubscription = _changingViewModel.Changing.Subscribe(_changingSink);
    }

    /// <summary>Disposes the live subscriptions.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _changedSubscription.Dispose();
        _changingSubscription.Dispose();
    }

    /// <summary>Measures propagation through the <c>Changed</c> stream.</summary>
    [Benchmark]
    public void EmitChanged()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _changedViewModel.Count = i;
        }
    }

    /// <summary>Measures propagation through the <c>Changing</c> stream.</summary>
    [Benchmark]
    public void EmitChanging()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _changingViewModel.Count = i;
        }
    }
}
