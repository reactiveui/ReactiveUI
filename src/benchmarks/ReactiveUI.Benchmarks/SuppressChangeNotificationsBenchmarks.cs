// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks batched property changes under <c>SuppressChangeNotifications</c>: while suppressed, sets should not
/// flow to the live subscription, exercising the suppression gate on every change.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class SuppressChangeNotificationsBenchmarks
{
    /// <summary>The number of suppressed property changes per benchmark invocation.</summary>
    private const int EmissionCount = 10_000;

    /// <summary>Sink for the changed stream (should receive nothing while suppressed).</summary>
    private readonly NoopObserver<IReactivePropertyChangedEventArgs<IReactiveObject>> _sink = new();

    /// <summary>The view model under test.</summary>
    private BenchmarkViewModel _viewModel = null!;

    /// <summary>The live changed subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates the view model and a live changed subscription.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _viewModel = new BenchmarkViewModel();
        _subscription = _viewModel.Changed.Subscribe(_sink);
    }

    /// <summary>Disposes the live subscription.</summary>
    [GlobalCleanup]
    public void Cleanup() => _subscription.Dispose();

    /// <summary>Measures a batch of property changes inside a suppression scope.</summary>
    [Benchmark]
    public void SuppressedBatch()
    {
        using var suppress = _viewModel.SuppressChangeNotifications();
        for (var i = 0; i < EmissionCount; i++)
        {
            _viewModel.Count = i;
        }
    }
}
