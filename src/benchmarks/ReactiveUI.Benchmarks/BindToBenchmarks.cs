// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>Benchmarks <c>BindTo</c>: cold bind setup and per-value propagation of an observable into a target property.</summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class BindToBenchmarks
{
    /// <summary>The number of values pushed through the source observable per emission benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>The source view model exposing the inner observable.</summary>
    private ObservableBenchmarkViewModel _source = null!;

    /// <summary>The target whose property is bound to the source observable.</summary>
    private BenchmarkViewModel _target = null!;

    /// <summary>The standing binding.</summary>
    private IDisposable _binding = null!;

    /// <summary>Creates the source and target and a standing <c>BindTo</c>.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _source = CreateSource();
        _target = new();
        _binding = _source.Values.BindTo(_target, x => x.Count);
    }

    /// <summary>Disposes the binding and the source.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _binding.Dispose();
        _source.Dispose();
    }

    /// <summary>Measures value propagation through the live binding.</summary>
    [Benchmark]
    public void Emit()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _source.Push(i);
        }
    }

    /// <summary>Measures a cold <c>BindTo</c> setup + dispose.</summary>
    [Benchmark]
    public void Bind()
    {
        using var binding = _source.Values.BindTo(_target, x => x.Count);
    }

    /// <summary>Creates a source view model exposing the inner observable.</summary>
    /// <returns>The new source view model.</returns>
    private static ObservableBenchmarkViewModel CreateSource() => new();
}
