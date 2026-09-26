// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
using System.Reactive.Concurrency;
#else
using ReactiveUI.Primitives.Concurrency;
#endif

namespace ReactiveUI.Tests.Routing;

/// <summary>Tests for <see cref="RoutingState"/>.</summary>
public class RoutingStateTests
{
    /// <summary>
    /// Navigate delivers the navigated view model before it completes, even when the navigation scheduler runs
    /// queued work out of order (as a concurrent scheduler such as the task pool can).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigateDeliversViewModelBeforeCompletionOnReorderingScheduler()
    {
        var scheduler = new ReorderingScheduler();
        var router = new RoutingState(scheduler);
        var viewModel = new TestViewModel();
        var notifications = new List<string>();

        using var subscription = router.Navigate.Execute(viewModel).Subscribe(
            vm => notifications.Add(ReferenceEquals(vm, viewModel) ? "next" : "next-other"),
            () => notifications.Add("completed"));
        scheduler.RunNewestFirst();

        await Assert.That(notifications).IsEquivalentTo(["next", "completed"], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    /// <summary>A routable view model used as the navigation target.</summary>
    private sealed class TestViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment => "test";

        /// <inheritdoc/>
        public IScreen HostScreen => null!;
    }

#if REACTIVE_SHIM
    /// <summary>
    /// A scheduler that holds work until <see cref="RunNewestFirst"/>, then runs it newest first. It stands in for a
    /// concurrent scheduler, which gives no ordering guarantee between separately scheduled work items.
    /// </summary>
    private sealed class ReorderingScheduler : IScheduler
    {
        /// <summary>The pending work, newest last.</summary>
        private readonly List<Action> _pending = [];

        /// <inheritdoc/>
        public DateTimeOffset Now => default;

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            _pending.Add(() => action(this, state));
            return EmptyDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action) =>
            Schedule(state, action);

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) =>
            Schedule(state, action);

        /// <summary>Runs pending work, newest first, until none remains.</summary>
        public void RunNewestFirst()
        {
            while (_pending.Count > 0)
            {
                var work = _pending[^1];
                _pending.RemoveAt(_pending.Count - 1);
                work();
            }
        }
    }
#else
    /// <summary>
    /// A scheduler that holds work until <see cref="RunNewestFirst"/>, then runs it newest first. It stands in for a
    /// concurrent scheduler, which gives no ordering guarantee between separately scheduled work items.
    /// </summary>
    private sealed class ReorderingScheduler : ISequencer
    {
        /// <summary>The pending work, newest last.</summary>
        private readonly List<IWorkItem> _pending = [];

        /// <inheritdoc/>
        public DateTimeOffset Now => default;

        /// <inheritdoc/>
        public long Timestamp => 0;

        /// <inheritdoc/>
        public void Schedule(IWorkItem item) => _pending.Add(item);

        /// <inheritdoc/>
        public void Schedule(IWorkItem item, long dueTimestamp) => _pending.Add(item);

        /// <summary>Runs pending work, newest first, until none remains.</summary>
        public void RunNewestFirst()
        {
            while (_pending.Count > 0)
            {
                var work = _pending[^1];
                _pending.RemoveAt(_pending.Count - 1);
                work.Execute();
            }
        }
    }
#endif
}
