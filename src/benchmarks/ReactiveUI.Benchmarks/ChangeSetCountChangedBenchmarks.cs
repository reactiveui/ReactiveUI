// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using BenchmarkDotNet.Attributes;
using DynamicData;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <c>ChangeSetMixin.CountChanged</c> over the navigation change-set stream: each navigation changes the
/// count, so the count-changed filter forwards on every push.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ChangeSetCountChangedBenchmarks
{
    /// <summary>The number of navigations pushed per benchmark invocation.</summary>
    private const int NavigateCount = 1_000;

    /// <summary>Sink for the count-changed change-set stream.</summary>
    private readonly NoopObserver<IChangeSet<IRoutableViewModel>> _sink = new();

    /// <summary>The router producing the change-set stream.</summary>
    private RoutingState _router = null!;

    /// <summary>Reusable routable view model pushed during navigation.</summary>
    private NavigableViewModel _viewModel = null!;

    /// <summary>The count-changed subscription.</summary>
    private IDisposable _subscription = null!;

    /// <summary>Creates the router with an immediate scheduler and subscribes the count-changed filter.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _router = new RoutingState(ImmediateScheduler.Instance);
        _viewModel = new NavigableViewModel();
        _subscription = _router.NavigationChanged.CountChanged().Subscribe(_sink);
    }

    /// <summary>Disposes the subscription and clears the stack.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _subscription.Dispose();
        _router.NavigationStack.Clear();
    }

    /// <summary>Clears the navigation stack between invocations so it does not grow unbounded.</summary>
    [IterationCleanup]
    public void ResetStack() => _router.NavigationStack.Clear();

    /// <summary>Measures repeated pushes, exercising the count-changed filter once per navigation.</summary>
    [Benchmark]
    public void NavigateCounted()
    {
        for (var i = 0; i < NavigateCount; i++)
        {
            using var subscription = _router.Navigate.Execute(_viewModel).Subscribe();
        }
    }
}
