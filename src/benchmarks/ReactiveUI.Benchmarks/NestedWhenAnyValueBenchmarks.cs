// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks a multi-level <c>WhenAnyValue</c> chain (<c>x =&gt; x.Child.First</c>), exercising the per-link
/// <c>ExpressionChainSink</c>: leaf-value propagation and intermediate (parent) re-subscription.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class NestedWhenAnyValueBenchmarks
{
    /// <summary>The number of changes pushed per emission benchmark.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>Sink for the leaf value of the chain.</summary>
    private readonly NoopObserver<string?> _sink = new();

    /// <summary>The root view model whose <c>Child.First</c> is observed.</summary>
    private NestedBenchmarkViewModel _viewModel = null!;

    /// <summary>The standing chain subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates the nested view model with a child and a standing two-level chain subscription.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _viewModel = new NestedBenchmarkViewModel { Child = new() };
        _subscription = _viewModel.WhenAnyValue(x => x.Child!.First).Subscribe(_sink);
    }

    /// <summary>Disposes the standing subscription.</summary>
    [GlobalCleanup]
    public void Cleanup() => _subscription.Dispose();

    /// <summary>Measures leaf-value propagation (changing <c>Child.First</c>).</summary>
    [Benchmark]
    public void EmitLeaf()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _viewModel.Child!.First = (i & 1) == 0 ? "a" : "b";
        }
    }

    /// <summary>Measures intermediate re-subscription (swapping <c>Child</c>, which re-establishes the leaf watcher).</summary>
    [Benchmark]
    public void SwapParent()
    {
        for (var i = 0; i < EmissionCount; i++)
        {
            _viewModel.Child = new();
        }
    }
}
