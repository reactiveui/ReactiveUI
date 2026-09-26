// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Enums;

namespace ReactiveUI.Tests.Routing;

/// <summary>
/// Tests for the navigation observables on <see cref="RoutingState"/>: <see cref="RoutingState.NavigationStackChanged"/>,
/// <see cref="RoutingState.CurrentViewModel"/> and <see cref="RoutingState.CanNavigateBack"/>.
/// </summary>
public class RoutingStateObservablesTests
{
    /// <summary>The number of view models in a stack that allows going back.</summary>
    private const int TwoViewModels = 2;

    /// <summary>The number of emissions expected after three navigation changes.</summary>
    private const int ThreeEmissions = 3;

    /// <summary>The URL path segment of the first page.</summary>
    private const string First = "first";

    /// <summary>The URL path segment of the second page.</summary>
    private const string Second = "second";

    /// <summary>The URL path segment of the third page.</summary>
    private const string Third = "third";

    /// <summary>Subscribing to the stack stream emits nothing until the stack changes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigationStackChanged_OnSubscribe_EmitsNothing()
    {
        var router = new RoutingState(Sequencer.Immediate);
        router.NavigationStack.Add(new PageViewModel(First));
        var stacks = new List<IReadOnlyList<IRoutableViewModel>>();

        using var subscription = router.NavigationStackChanged.Subscribe(stacks.Add);

        await Assert.That(stacks).IsEmpty();
    }

    /// <summary>Each navigation delivers the whole stack, oldest view model first.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigationStackChanged_Navigation_EmitsTheWholeStack()
    {
        var router = new RoutingState(Sequencer.Immediate);
        IRoutableViewModel first = new PageViewModel(First);
        IRoutableViewModel second = new PageViewModel(Second);
        var stacks = new List<IReadOnlyList<IRoutableViewModel>>();
        using var subscription = router.NavigationStackChanged.Subscribe(stacks.Add);

        _ = router.Navigate.Execute(first).Subscribe();
        _ = router.Navigate.Execute(second).Subscribe();
        _ = router.NavigateBack.Execute().Subscribe();

        await Assert.That(stacks).Count().IsEqualTo(ThreeEmissions);
        await Assert.That(stacks[0]).IsEquivalentTo([first], CollectionOrdering.Matching);
        await Assert.That(stacks[1]).IsEquivalentTo([first, second], CollectionOrdering.Matching);
        await Assert.That(stacks[2]).IsEquivalentTo([first], CollectionOrdering.Matching);
    }

    /// <summary>A reset delivers the emptied stack and then the stack holding the new view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigationStackChanged_NavigateAndReset_EmitsEmptyThenNewStack()
    {
        var router = new RoutingState(Sequencer.Immediate);
        IRoutableViewModel reset = new PageViewModel(Second);
        _ = router.Navigate.Execute(new PageViewModel(First)).Subscribe();
        var stacks = new List<IReadOnlyList<IRoutableViewModel>>();
        using var subscription = router.NavigationStackChanged.Subscribe(stacks.Add);

        _ = router.NavigateAndReset.Execute(reset).Subscribe();

        await Assert.That(stacks).Count().IsEqualTo(TwoViewModels);
        await Assert.That(stacks[0]).IsEmpty();
        await Assert.That(stacks[1]).IsEquivalentTo([reset], CollectionOrdering.Matching);
    }

    /// <summary>A snapshot is a copy: navigating again does not change a snapshot already delivered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigationStackChanged_Snapshot_IsNotChangedByLaterNavigation()
    {
        var router = new RoutingState(Sequencer.Immediate);
        IRoutableViewModel first = new PageViewModel(First);
        IReadOnlyList<IRoutableViewModel>? firstSnapshot = null;
        using var subscription = router.NavigationStackChanged.Subscribe(stack => firstSnapshot ??= stack);

        _ = router.Navigate.Execute(first).Subscribe();
        _ = router.Navigate.Execute(new PageViewModel(Second)).Subscribe();

        await Assert.That(firstSnapshot).IsNotNull();
        await Assert.That(firstSnapshot!).IsEquivalentTo([first], CollectionOrdering.Matching);
    }

    /// <summary>Disposing the subscription stops further snapshots.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigationStackChanged_Disposed_StopsEmitting()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var stacks = new List<IReadOnlyList<IRoutableViewModel>>();
        var subscription = router.NavigationStackChanged.Subscribe(stacks.Add);

        _ = router.Navigate.Execute(new PageViewModel(First)).Subscribe();
        subscription.Dispose();
        _ = router.Navigate.Execute(new PageViewModel(Second)).Subscribe();

        await Assert.That(stacks).Count().IsEqualTo(1);
    }

    /// <summary>The stack stream observes the stack assigned when the observer subscribes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigationStackChanged_StackReplaced_ObservesTheStackAtSubscribe()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var replacement = new ObservableCollection<IRoutableViewModel>();
        router.NavigationStack = replacement;
        var stacks = new List<IReadOnlyList<IRoutableViewModel>>();
        using var subscription = router.NavigationStackChanged.Subscribe(stacks.Add);

        IRoutableViewModel page = new PageViewModel(First);
        replacement.Add(page);

        await Assert.That(stacks).Count().IsEqualTo(1);
        await Assert.That(stacks[0]).IsEquivalentTo([page], CollectionOrdering.Matching);
    }

    /// <summary>The current view model is emitted on subscribe, then after every change, and is null while the stack is empty.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CurrentViewModel_EmitsOnSubscribeAndAfterEveryChange()
    {
        var router = new RoutingState(Sequencer.Immediate);
        IRoutableViewModel first = new PageViewModel(First);
        IRoutableViewModel second = new PageViewModel(Second);
        var current = new List<IRoutableViewModel?>();
        using var subscription = router.CurrentViewModel.Subscribe(current.Add);

        _ = router.Navigate.Execute(first).Subscribe();
        _ = router.Navigate.Execute(second).Subscribe();
        _ = router.NavigateAndReset.Execute(first).Subscribe();

        await Assert.That(current).IsEquivalentTo([null, first, second, null, first], CollectionOrdering.Matching);
    }

    /// <summary>The current view model is the top of a stack that already holds view models.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CurrentViewModel_NonEmptyStack_EmitsTopOnSubscribe()
    {
        var router = new RoutingState(Sequencer.Immediate);
        IRoutableViewModel second = new PageViewModel(Second);
        router.NavigationStack.Add(new PageViewModel(First));
        router.NavigationStack.Add(second);
        IRoutableViewModel? current = null;

        using var subscription = router.CurrentViewModel.Subscribe(viewModel => current = viewModel);

        await Assert.That(current).IsSameReferenceAs(second);
    }

    /// <summary>Whether you can go back is emitted on subscribe, then only when the answer changes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanNavigateBack_EmitsOnSubscribeThenOnlyChanges()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var answers = new List<bool>();
        using var subscription = router.CanNavigateBack.Subscribe(answers.Add);

        _ = router.Navigate.Execute(new PageViewModel(First)).Subscribe();
        _ = router.Navigate.Execute(new PageViewModel(Second)).Subscribe();
        _ = router.Navigate.Execute(new PageViewModel(Third)).Subscribe();
        _ = router.NavigateBack.Execute().Subscribe();
        _ = router.NavigateBack.Execute().Subscribe();

        await Assert.That(answers).IsEquivalentTo([false, true, false], CollectionOrdering.Matching);
    }

    /// <summary>A stack that already allows going back reports true on subscribe.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanNavigateBack_TwoViewModels_EmitsTrueOnSubscribe()
    {
        var router = new RoutingState(Sequencer.Immediate);
        router.NavigationStack.Add(new PageViewModel(First));
        router.NavigationStack.Add(new PageViewModel(Second));
        var answers = new List<bool>();

        using var subscription = router.CanNavigateBack.Subscribe(answers.Add);

        await Assert.That(answers).IsEquivalentTo([true], CollectionOrdering.Matching);
    }

    /// <summary><see cref="RoutingState.NavigateBack"/> can execute exactly when <see cref="RoutingState.CanNavigateBack"/> is true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigateBack_CanExecute_FollowsCanNavigateBack()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var canExecute = new List<bool>();
        using var subscription = router.NavigateBack.CanExecute.Subscribe(canExecute.Add);

        _ = router.Navigate.Execute(new PageViewModel(First)).Subscribe();
        _ = router.Navigate.Execute(new PageViewModel(Second)).Subscribe();
        _ = router.NavigateBack.Execute().Subscribe();

        await Assert.That(canExecute[^1]).IsFalse();
        await Assert.That(canExecute).Contains(true);
    }

    /// <summary>The observables derived from the stack stream forward its error and completion.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DerivedObservables_ForwardErrorAndCompletion()
    {
        using var failing = new ScriptedRoutingState();
        using var completing = new ScriptedRoutingState();
        var error = new InvalidOperationException("stack failed");
        Exception? currentError = null;
        Exception? canGoBackError = null;
        var currentCompleted = false;
        var canGoBackCompleted = false;

        using var currentFailing = failing.CurrentViewModel.Subscribe(static _ => { }, ex => currentError = ex);
        using var canGoBackFailing = failing.CanNavigateBack.Subscribe(static _ => { }, ex => canGoBackError = ex);
        using var currentCompleting = completing.CurrentViewModel.Subscribe(static _ => { }, static _ => { }, () => currentCompleted = true);
        using var canGoBackCompleting = completing.CanNavigateBack.Subscribe(static _ => { }, static _ => { }, () => canGoBackCompleted = true);

        failing.Changes.OnError(error);
        completing.Changes.OnCompleted();

        await Assert.That(currentError).IsSameReferenceAs(error);
        await Assert.That(canGoBackError).IsSameReferenceAs(error);
        await Assert.That(currentCompleted).IsTrue();
        await Assert.That(canGoBackCompleted).IsTrue();
    }

    /// <summary>The derived observables read snapshots from the stack stream rather than the stack itself.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DerivedObservables_FollowTheStackStream()
    {
        using var router = new ScriptedRoutingState();
        IRoutableViewModel first = new PageViewModel(First);
        IRoutableViewModel second = new PageViewModel(Second);
        var current = new List<IRoutableViewModel?>();
        var answers = new List<bool>();
        using var currentSubscription = router.CurrentViewModel.Subscribe(current.Add);
        using var answerSubscription = router.CanNavigateBack.Subscribe(answers.Add);

        router.Changes.OnNext([first]);
        router.Changes.OnNext([first, second]);
        router.Changes.OnNext([first, second]);
        router.Changes.OnNext([]);

        await Assert.That(current).IsEquivalentTo([null, first, second, second, null], CollectionOrdering.Matching);
        await Assert.That(answers).IsEquivalentTo([false, true, false], CollectionOrdering.Matching);
    }

    /// <summary>Each observable rejects a null observer.</summary>
    [Test]
    public void Observables_NullObserver_Throw()
    {
        var router = new RoutingState(Sequencer.Immediate);

        _ = Assert.Throws<ArgumentNullException>(() => router.NavigationStackChanged.Subscribe((IObserver<IReadOnlyList<IRoutableViewModel>>)null!));
        _ = Assert.Throws<ArgumentNullException>(() => router.CurrentViewModel.Subscribe((IObserver<IRoutableViewModel?>)null!));
        _ = Assert.Throws<ArgumentNullException>(() => router.CanNavigateBack.Subscribe((IObserver<bool>)null!));
    }

    /// <summary>A router whose stack stream is pushed by the test, so errors and completion can be delivered.</summary>
    private sealed class ScriptedRoutingState : RoutingState, IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="ScriptedRoutingState"/> class.</summary>
        public ScriptedRoutingState()
            : base(Sequencer.Immediate) => NavigationStackChanged = Changes;

        /// <summary>Gets the stack stream the test pushes into.</summary>
        public Signal<IReadOnlyList<IRoutableViewModel>> Changes { get; } = new();

        /// <inheritdoc/>
        public void Dispose() => Changes.Dispose();
    }

    /// <summary>A routable view model used to populate the navigation stack.</summary>
    /// <param name="segment">The URL path segment.</param>
    private sealed class PageViewModel(string segment) : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment => segment;

        /// <inheritdoc/>
        public IScreen HostScreen => null!;
    }
}
