// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks the activation lifecycle: repeated <see cref="ViewModelActivator.Activate"/> / deactivate cycles that run
/// a <c>WhenActivated</c> block and tear down its disposables.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ActivationBenchmarks
{
    /// <summary>The number of activate / deactivate cycles per benchmark invocation.</summary>
    private const int CycleCount = 10_000;

    /// <summary>The activatable view model under test.</summary>
    private ActivatableBenchmarkViewModel _viewModel = null!;

    /// <summary>Creates the activatable view model and wires its <c>WhenActivated</c> block.</summary>
    [GlobalSetup]
    public void Setup() => _viewModel = new();

    /// <summary>Measures repeated activate + deactivate cycles.</summary>
    [Benchmark]
    public void ActivateDeactivate()
    {
        for (var i = 0; i < CycleCount; i++)
        {
            using var activation = _viewModel.Activator.Activate();
        }
    }
}
