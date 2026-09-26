// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Benchmarks <see cref="RoutingState"/> navigation with every navigation observable subscribed:
/// <see cref="RoutingState.NavigationStackChanged"/>, <see cref="RoutingState.CurrentViewModel"/> and
/// <see cref="RoutingState.CanNavigateBack"/>.
/// </summary>
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[DebuggerDisplay("NavigationBenchmarks")]
public class NavigationBenchmarks
{
    /// <summary>The number of navigations pushed per benchmark invocation.</summary>
    private const int NavigateCount = 1_000;

    /// <summary>Sink for each navigate command execution.</summary>
    private readonly NoopObserver<IRoutableViewModel> _navigateSink = new();

    /// <summary>Sink observing the navigation-stack snapshots so they cannot be elided.</summary>
    private readonly NoopObserver<IReadOnlyList<IRoutableViewModel>> _stackSink = new();

    /// <summary>Sink observing the current view model.</summary>
    private readonly NoopObserver<IRoutableViewModel?> _currentSink = new();

    /// <summary>Sink observing whether the router can navigate back.</summary>
    private readonly NoopObserver<bool> _canNavigateBackSink = new();

    /// <summary>The router under test.</summary>
    private RoutingState _router = null!;

    /// <summary>Reusable routable view model pushed during navigation.</summary>
    private NavigableViewModel _viewModel = null!;

    /// <summary>The subscriptions on the navigation observables.</summary>
    private MultipleDisposable _subscriptions = null!;

    /// <summary>Creates the router with an immediate scheduler and subscribes every navigation observable.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _router = new(Sequencer.Immediate);
        _viewModel = new();
        _subscriptions =
        [
            _router.NavigationStackChanged.Subscribe(_stackSink),
            _router.CurrentViewModel.Subscribe(_currentSink),
            _router.CanNavigateBack.Subscribe(_canNavigateBackSink),
        ];
    }

    /// <summary>Disposes the subscriptions and clears the stack.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _subscriptions.Dispose();
        _router.NavigationStack.Clear();
    }

    /// <summary>Clears the navigation stack between invocations so it does not grow unbounded.</summary>
    [IterationCleanup]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetStack() => _router.NavigationStack.Clear();

    /// <summary>Measures repeated pushes, delivering to every navigation observable once per navigation.</summary>
    [Benchmark]
    public void Navigate()
    {
        for (var i = 0; i < NavigateCount; i++)
        {
            using var subscription = _router.Navigate.Execute(_viewModel).Subscribe(_navigateSink);
        }
    }
}
