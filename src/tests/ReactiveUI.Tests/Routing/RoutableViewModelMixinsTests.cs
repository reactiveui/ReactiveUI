// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Routing;

/// <summary>
/// Tests for <see cref="RoutableViewModelMixins"/> covering the navigation-focus sinks' re-entrancy behaviour,
/// their terminate-once latch, and the lifetime of the scope handed back by <c>WhenNavigatedTo</c>.
/// </summary>
/// <remarks>
/// Every navigation notification runs synchronously on the caller's thread, so a downstream handler that
/// navigates re-enters the sink underneath its own notification. The sink must advance its own bookkeeping before
/// it hands anything downstream, otherwise the nested notification is judged against stale state.
/// </remarks>
public class RoutableViewModelMixinsTests
{
    /// <summary>The number of concurrently disposed subscriptions used by the racing-disposal test.</summary>
    private const int ConcurrentSubscriptionCount = 200;

    /// <summary>
    /// A departure handler that navigates again must not make the sink report a second departure: the nested
    /// navigation moves focus between two other view models, which is not a departure of the watched one.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Catches state-after-emit ordering: recording the new current view model only after the downstream call
    /// leaves the nested notification comparing against the pre-navigation value, so the watched view model looks
    /// like it departed twice.
    /// </remarks>
    [Test]
    [Timeout(30_000)]
    public async Task WhenNavigatingFromObservable_HandlerNavigatesSynchronously_ReportsOneDeparture(CancellationToken cancellationToken)
    {
        using var router = new ScriptedRoutingState();
        var screen = new TestScreen(router);
        var watched = new RoutableViewModel(screen);
        var second = new RoutableViewModel(screen);
        var third = new RoutableViewModel(screen);

        var recorder = new FocusRecorder();
        using var subscription = watched.WhenNavigatingFromObservable().Subscribe(recorder);

        // Navigating from inside the departure handler re-enters the sink underneath its own notification.
        recorder.OnNextHandler = () =>
        {
            recorder.OnNextHandler = null;
            router.Push(third);
        };

        router.Push(watched);
        router.Push(second);

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(recorder.Emissions).IsEqualTo(1);
        await Assert.That(recorder.Completed).IsEqualTo(0);
    }

    /// <summary>An arrival handler that navigates again must not make the sink report a second arrival.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task WhenNavigatedToObservable_HandlerNavigatesSynchronously_ReportsOneArrival(CancellationToken cancellationToken)
    {
        using var router = new ScriptedRoutingState();
        var screen = new TestScreen(router);
        var watched = new RoutableViewModel(screen);
        var other = new RoutableViewModel(screen);

        var recorder = new FocusRecorder();
        using var subscription = watched.WhenNavigatedToObservable().Subscribe(recorder);

        recorder.OnNextHandler = () =>
        {
            recorder.OnNextHandler = null;
            router.Push(other);
        };

        router.Push(watched);

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(recorder.Emissions).IsEqualTo(1);
        await Assert.That(recorder.Completed).IsEqualTo(0);
    }

    /// <summary>A handler that removes the watched view model re-entrantly still terminates the sink exactly once.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task WhenNavigatedToObservable_HandlerRemovesWatchedViewModel_CompletesOnce(CancellationToken cancellationToken)
    {
        using var router = new ScriptedRoutingState();
        var screen = new TestScreen(router);
        var watched = new RoutableViewModel(screen);
        var other = new RoutableViewModel(screen);

        var recorder = new FocusRecorder();
        using var subscription = watched.WhenNavigatedToObservable().Subscribe(recorder);

        recorder.OnNextHandler = () =>
        {
            recorder.OnNextHandler = null;
            router.Remove(watched);
        };

        router.Push(watched);
        router.Push(other);
        router.Remove(other);

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(recorder.Emissions).IsEqualTo(1);
        await Assert.That(recorder.Completed).IsEqualTo(1);
    }

    /// <summary>Removal terminates the arrival observable once; later navigation neither emits nor terminates it again.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task WhenNavigatedToObservable_RemovedThenNavigatedAgain_CompletesOnce(CancellationToken cancellationToken)
    {
        var screen = new TestScreen();
        var watched = new RoutableViewModel(screen);
        var other = new RoutableViewModel(screen);
        var recorder = new FocusRecorder();

        using var subscription = watched.WhenNavigatedToObservable().Subscribe(recorder);

        _ = screen.Router.Navigate.Execute(watched).Subscribe();
        _ = screen.Router.NavigateBack.Execute().Subscribe();

        await Assert.That(recorder.Completed).IsEqualTo(1);

        var emissionsAtRemoval = recorder.Emissions;
        _ = screen.Router.Navigate.Execute(other).Subscribe();
        _ = screen.Router.Navigate.Execute(watched).Subscribe();

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(recorder.Completed).IsEqualTo(1);
        await Assert.That(recorder.Emissions).IsEqualTo(emissionsAtRemoval);
    }

    /// <summary>Removal terminates the departure observable once; later navigation neither emits nor terminates it again.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task WhenNavigatingFromObservable_RemovedThenNavigatedAgain_CompletesOnce(CancellationToken cancellationToken)
    {
        var screen = new TestScreen();
        var watched = new RoutableViewModel(screen);
        var other = new RoutableViewModel(screen);
        var recorder = new FocusRecorder();

        using var subscription = watched.WhenNavigatingFromObservable().Subscribe(recorder);

        _ = screen.Router.Navigate.Execute(watched).Subscribe();
        _ = screen.Router.NavigateBack.Execute().Subscribe();

        await Assert.That(recorder.Completed).IsEqualTo(1);

        var emissionsAtRemoval = recorder.Emissions;
        _ = screen.Router.Navigate.Execute(other).Subscribe();
        _ = screen.Router.Navigate.Execute(watched).Subscribe();

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(recorder.Completed).IsEqualTo(1);
        await Assert.That(recorder.Emissions).IsEqualTo(emissionsAtRemoval);
    }

    /// <summary>Disposing the handle returned by <c>WhenNavigatedTo</c> also disposes the scope currently in force.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Catches the leak in which the handle only unsubscribes from the navigation stream: the scope built for the
    /// focused view model then stays alive for the life of the process, holding every resource the caller opened.
    /// </remarks>
    [Test]
    [Timeout(30_000)]
    public async Task WhenNavigatedTo_HandleDisposed_DisposesTheActiveScope(CancellationToken cancellationToken)
    {
        var screen = new TestScreen();
        var watched = new RoutableViewModel(screen);
        var scopeDisposals = 0;

        var handle = watched.WhenNavigatedTo(() => new ActionDisposable(() => scopeDisposals++));

        _ = screen.Router.Navigate.Execute(watched).Subscribe();

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(scopeDisposals).IsEqualTo(0);

        handle.Dispose();

        await Assert.That(scopeDisposals).IsEqualTo(1);
    }

    /// <summary>Disposing the handle stops any further scope from being built when navigation continues.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task WhenNavigatedTo_HandleDisposed_StopsBuildingScopes(CancellationToken cancellationToken)
    {
        var screen = new TestScreen();
        var watched = new RoutableViewModel(screen);
        var other = new RoutableViewModel(screen);
        var scopesBuilt = 0;

        var handle = watched.WhenNavigatedTo(() =>
        {
            scopesBuilt++;
            return Scope.Empty;
        });

        _ = screen.Router.Navigate.Execute(watched).Subscribe();
        handle.Dispose();

        _ = screen.Router.Navigate.Execute(other).Subscribe();
        _ = screen.Router.Navigate.Execute(watched).Subscribe();

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(scopesBuilt).IsEqualTo(1);
    }

    /// <summary>
    /// Subscriptions being disposed on another thread while the watched view model is removed must never see two
    /// terminal notifications.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Disposal and the removal-driven completion race for the same latch; losing that race either delivers
    /// <c>OnCompleted</c> twice or wedges the sink so the terminal notification is never delivered at all.
    /// </remarks>
    [Test]
    [Timeout(120_000)]
    public async Task WhenNavigatedToObservable_DisposalRacesRemoval_CompletesAtMostOnce(CancellationToken cancellationToken)
    {
        var screen = new TestScreen();
        var watched = new RoutableViewModel(screen);
        _ = screen.Router.Navigate.Execute(watched).Subscribe();

        var recorders = new FocusRecorder[ConcurrentSubscriptionCount];
        var handles = new IDisposable[ConcurrentSubscriptionCount];
        for (var i = 0; i < ConcurrentSubscriptionCount; i++)
        {
            recorders[i] = new();
            handles[i] = watched.WhenNavigatedToObservable().Subscribe(recorders[i]);
        }

        using var start = new ManualResetEventSlim(false);
        var disposer = new Thread(() =>
        {
            start.Wait(cancellationToken);
            for (var i = 0; i < ConcurrentSubscriptionCount; i++)
            {
                handles[i].Dispose();
            }
        })
        { IsBackground = true };

        disposer.Start();
        start.Set();
        _ = screen.Router.NavigateBack.Execute().Subscribe();
        disposer.Join();

        var duplicated = 0;
        var errored = 0;
        for (var i = 0; i < ConcurrentSubscriptionCount; i++)
        {
            if (recorders[i].Completed > 1)
            {
                duplicated++;
            }

            errored += recorders[i].Errors;
        }

        await Assert.That(duplicated).IsEqualTo(0);
        await Assert.That(errored).IsEqualTo(0);
    }

    /// <summary>
    /// Every live subscription is completed exactly once when the watched view model is removed, no matter how
    /// many observers are attached.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(120_000)]
    public async Task WhenNavigatedToObservable_ManySubscriptions_EachCompletesExactlyOnce(CancellationToken cancellationToken)
    {
        var screen = new TestScreen();
        var watched = new RoutableViewModel(screen);
        _ = screen.Router.Navigate.Execute(watched).Subscribe();

        var recorders = new FocusRecorder[ConcurrentSubscriptionCount];
        var handles = new IDisposable[ConcurrentSubscriptionCount];
        for (var i = 0; i < ConcurrentSubscriptionCount; i++)
        {
            recorders[i] = new();
            handles[i] = watched.WhenNavigatedToObservable().Subscribe(recorders[i]);
        }

        _ = screen.Router.NavigateBack.Execute().Subscribe();

        cancellationToken.ThrowIfCancellationRequested();

        var completedExactlyOnce = 0;
        for (var i = 0; i < ConcurrentSubscriptionCount; i++)
        {
            if (recorders[i].Completed == 1)
            {
                completedExactlyOnce++;
            }

            handles[i].Dispose();
        }

        await Assert.That(completedExactlyOnce).IsEqualTo(ConcurrentSubscriptionCount);
    }

    /// <summary>Counts the notifications a navigation-focus observable delivers.</summary>
    private sealed class FocusRecorder : IObserver<RxVoid>
    {
        /// <summary>The number of emissions delivered.</summary>
        private int _emissions;

        /// <summary>The number of completions delivered.</summary>
        private int _completed;

        /// <summary>The number of errors delivered.</summary>
        private int _errors;

        /// <summary>Gets the number of emissions delivered.</summary>
        public int Emissions => Volatile.Read(ref _emissions);

        /// <summary>Gets the number of completions delivered.</summary>
        public int Completed => Volatile.Read(ref _completed);

        /// <summary>Gets the number of errors delivered.</summary>
        public int Errors => Volatile.Read(ref _errors);

        /// <summary>Gets or sets a callback run inside <see cref="OnNext"/>, used to re-enter the router.</summary>
        public Action? OnNextHandler { get; set; }

        /// <inheritdoc/>
        public void OnNext(RxVoid value)
        {
            _ = Interlocked.Increment(ref _emissions);
            OnNextHandler?.Invoke();
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => Interlocked.Increment(ref _errors);

        /// <inheritdoc/>
        public void OnCompleted() => Interlocked.Increment(ref _completed);
    }

    /// <summary>A host screen whose router runs every navigation notification inline.</summary>
    /// <param name="router">The router to host, or <see langword="null"/> for a stock inline router.</param>
    private sealed class TestScreen(RoutingState? router = null) : IScreen
    {
        /// <inheritdoc/>
        public RoutingState Router { get; } = router ?? new(Sequencer.Immediate);
    }

    /// <summary>
    /// A router whose navigation-change stream is pushed by the test instead of being derived from the navigation
    /// stack, so a handler can drive further navigation from inside a notification.
    /// </summary>
    /// <remarks>
    /// <see cref="ObservableCollection{T}"/> forbids mutation from inside its own <c>CollectionChanged</c> event, so
    /// the stock router cannot deliver a nested navigation change at all. Splitting the announcement from the stack
    /// mutation is the only way to put the focus sinks under genuine downstream re-entrancy.
    /// </remarks>
    private sealed class ScriptedRoutingState : RoutingState, IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="ScriptedRoutingState"/> class.</summary>
        public ScriptedRoutingState()
            : base(Sequencer.Immediate) => NavigationChanges = Changes;

        /// <summary>Gets the change stream the test pushes navigation announcements into.</summary>
        public Signal<IReactiveChangeSet<IRoutableViewModel>> Changes { get; } = new();

        /// <summary>Pushes a view model onto the stack and announces the addition.</summary>
        /// <param name="viewModel">The view model that became current.</param>
        public void Push(IRoutableViewModel viewModel)
        {
            NavigationStack.Add(viewModel);
            Changes.OnNext(ChangeSet(ReactiveChangeReason.Add, viewModel));
        }

        /// <summary>Removes a view model from the stack and announces the removal.</summary>
        /// <param name="viewModel">The view model that left the stack.</param>
        public void Remove(IRoutableViewModel viewModel)
        {
            _ = NavigationStack.Remove(viewModel);
            Changes.OnNext(ChangeSet(ReactiveChangeReason.Remove, viewModel));
        }

        /// <inheritdoc/>
        public void Dispose() => Changes.Dispose();

        /// <summary>Builds a batch holding exactly one change.</summary>
        /// <param name="reason">The reason for the change.</param>
        /// <param name="viewModel">The view model the change carries.</param>
        /// <returns>A batch holding exactly that one change.</returns>
        private static ReactiveChangeSet<IRoutableViewModel> ChangeSet(ReactiveChangeReason reason, IRoutableViewModel viewModel) =>
            new([new(reason, viewModel, null, -1, -1)]);
    }

    /// <summary>A routable view model used to populate the navigation stack.</summary>
    /// <param name="screen">The host screen for the view model.</param>
    private sealed class RoutableViewModel(IScreen screen) : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string UrlPathSegment => "test";

        /// <inheritdoc/>
        public IScreen HostScreen { get; } = screen;
    }
}
