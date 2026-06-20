// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>Benchmarks <see cref="ReactiveProperty{T}"/> on the immediate scheduler: value propagation through a live subscription and cold construction.</summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ReactivePropertyBenchmarks
{
    /// <summary>The number of value changes pushed per emission benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>Sink for the property's value stream.</summary>
    private readonly NoopObserver<int> _sink = new();

    /// <summary>The notification scheduler shared by setup and construction (held so the benchmarks use instance state).</summary>
    private readonly ISequencer _scheduler = Sequencer.Immediate;

    /// <summary>The reactive property under test.</summary>
    private ReactiveProperty<int> _property = null!;

    /// <summary>The standing value subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates a reactive property on the immediate scheduler with a standing subscription.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _property = CreateProperty(_scheduler);
        _subscription = _property.Subscribe(_sink);
    }

    /// <summary>Disposes the subscription and the property.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _subscription.Dispose();
        _property.Dispose();
    }

    /// <summary>Measures value propagation through the live subscription.</summary>
    [Benchmark]
    public void SetValue()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _property.Value = i;
        }
    }

    /// <summary>Measures cold construction + dispose of a reactive property.</summary>
    /// <returns>The constructed property's value, returned so the JIT cannot elide the work.</returns>
    [Benchmark]
    public int Create()
    {
        using var property = CreateProperty(_scheduler);
        return property.Value;
    }

    /// <summary>Creates a distinct, immediate-scheduled integer reactive property.</summary>
    /// <param name="scheduler">The notification scheduler.</param>
    /// <returns>The new reactive property.</returns>
    private static ReactiveProperty<int> CreateProperty(ISequencer scheduler) => new(0, scheduler, false, false);
}
