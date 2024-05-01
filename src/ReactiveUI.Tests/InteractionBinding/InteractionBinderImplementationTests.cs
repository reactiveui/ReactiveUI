// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using FluentAssertions;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the Interaction bindings.
/// </summary>
public class InteractionBinderImplementationTests
{
    /// <summary>
    /// Tests that make sure that the we receive output from task handler.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task ReceiveOutputFromTaskHandler()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
                {
                    input.SetOutput(true);
                    return Task.CompletedTask;
                });

        var isDeletionConfirmed = await vm.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Test that we receive output from the observable handler.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task ReceiveOutputFromObservableHandler()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        var isDeletionConfirmed = await vm.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Test that checks that the receive output from task handler when view model was initially null.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task ReceiveOutputFromTaskHandlerWhenViewModelWasInitiallyNull()
    {
        InteractionBindViewModel? vm = null;
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Task.CompletedTask;
            });

        view.ViewModel = new InteractionBindViewModel();

        var isDeletionConfirmed = await view.ViewModel.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Test that checks that the receive output from observable handler when view model was initially null.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task ReceiveOutputFromObservableHandlerWhenViewModelWasInitiallyNull()
    {
        InteractionBindViewModel? vm = null;
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        view.ViewModel = new InteractionBindViewModel();

        var isDeletionConfirmed = await view.ViewModel.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Tests to make sure that it unregisters the task handler when view model is set to null.
    /// </summary>
    [Fact]
    public void UnregisterTaskHandlerWhenViewModelIsSetToNull()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Task.CompletedTask;
            });

        view.ViewModel = null;

        _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => vm.Interaction1.Handle("123").ToTask());
    }

    /// <summary>
    /// Tests to make sure that it unregisters the observable handler when view model is set to null.
    /// </summary>
    [Fact]
    public void UnregisterObservableHandlerWhenViewModelIsSetToNull()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        view.ViewModel = null;

        _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => vm.Interaction1.Handle("123").ToTask());
    }

    /// <summary>
    /// Tests to make sure that it unregisters the task handler from overwritten view model.
    /// </summary>
    [Fact]
    public void UnregisterTaskHandlerFromOverwrittenViewModel()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Task.CompletedTask;
            });

        view.ViewModel = new InteractionBindViewModel();

        _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => vm.Interaction1.Handle("123").ToTask());
    }

    /// <summary>
    /// Tests to make sure that it unregisters the observable handler from overwritten view model.
    /// </summary>
    [Fact]
    public void UnregisterObservableHandlerFromOverwrittenViewModel()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        view.ViewModel = new InteractionBindViewModel();

        _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => vm.Interaction1.Handle("123").ToTask());
    }

    /// <summary>
    /// Tests to make sure that it registers the task handler to newly assigned view model.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task RegisterTaskHandlerToNewlyAssignedViewModel()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Task.CompletedTask;
            });

        view.ViewModel = new InteractionBindViewModel();

        var isDeletionConfirmed = await view.ViewModel.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Tests to make sure that it registers the observable handler to newly assigned view model.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task RegisterObservableHandlerToNewlyAssignedViewModel()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        view.ViewModel = new InteractionBindViewModel();

        var isDeletionConfirmed = await view.ViewModel.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Tests to confirm nested interaction should receive output from task handler.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task NestedInteractionShouldReceiveOutputFromTaskHandler()
    {
        var vm = new InteractionAncestorViewModel();
        var view = new InteractionAncestorView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.InteractionViewModel.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Task.CompletedTask;
            });

        var isDeletionConfirmed = await vm.InteractionViewModel.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Tests to confirm nested interaction should receive output from observable handler.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task NestedInteractionShouldReceiveOutputFromObservableHandler()
    {
        var vm = new InteractionAncestorViewModel();
        var view = new InteractionAncestorView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.InteractionViewModel.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        var isDeletionConfirmed = await vm.InteractionViewModel.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Test to confirm that unregistering the task handler from overwritten nested view model.
    /// </summary>
    [Fact]
    public void UnregisterTaskHandlerFromOverwrittenNestedViewModel()
    {
        var firstInteractionVm = new InteractionBindViewModel();
        var vm = new InteractionAncestorViewModel();
        var view = new InteractionAncestorView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.InteractionViewModel.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Task.CompletedTask;
            });

        view.ViewModel.InteractionViewModel = new InteractionBindViewModel();

        _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => firstInteractionVm.Interaction1.Handle("123").ToTask());
    }

    /// <summary>
    /// Tests to make sure that it unregisters the observable handler from overwritten nested view model.
    /// </summary>
    [Fact]
    public void UnregisterObservableHandlerFromOverwrittenNestedViewModel()
    {
        var firstInteractionVm = new InteractionBindViewModel();
        var vm = new InteractionAncestorViewModel();
        var view = new InteractionAncestorView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.InteractionViewModel.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        view.ViewModel.InteractionViewModel = new InteractionBindViewModel();

        _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => firstInteractionVm.Interaction1.Handle("123").ToTask());
    }

    /// <summary>
    /// Tests to make sure that it registers the task handler to newly assigned nested view model.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task RegisterTaskHandlerToNewlyAssignedNestedViewModel()
    {
        var vm = new InteractionAncestorViewModel()
        {
            InteractionViewModel = new InteractionBindViewModel()
        };
        var view = new InteractionAncestorView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.InteractionViewModel.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        vm.InteractionViewModel = new InteractionBindViewModel();

        var isDeletionConfirmed = await vm.InteractionViewModel.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Tests to make sure that it registers the observable handler to newly assigned nested view model.
    /// </summary>
    /// <returns>A task to monitor the progress.</returns>
    [Fact]
    public async Task RegisterObservableHandlerToNewlyAssignedNestedViewModel()
    {
        var vm = new InteractionAncestorViewModel()
        {
            InteractionViewModel = new InteractionBindViewModel()
        };
        var view = new InteractionAncestorView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.InteractionViewModel.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        vm.InteractionViewModel = new InteractionBindViewModel();

        var isDeletionConfirmed = await vm.InteractionViewModel.Interaction1.Handle("123");

        isDeletionConfirmed.Should().BeTrue();
    }

    /// <summary>
    /// Tests to make sure that it unregisters the task handler when binding is disposed.
    /// </summary>
    [Fact]
    public void UnregisterTaskHandlerWhenBindingIsDisposed()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Task.CompletedTask;
            });

        disposable.Dispose();

        _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => vm.Interaction1.Handle("123").ToTask());
    }

    /// <summary>
    /// Tests to make sure that it unregisters the observable handler when binding is disposed.
    /// </summary>
    [Fact]
    public void UnregisterObservableHandlerWhenBindingIsDisposed()
    {
        var vm = new InteractionBindViewModel();
        var view = new InteractionBindView { ViewModel = vm };

        var disposable = view.BindInteraction(
            vm,
            vm => vm.Interaction1,
            input =>
            {
                input.SetOutput(true);
                return Observable.Return(Unit.Default);
            });

        disposable.Dispose();

        _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => vm.Interaction1.Handle("123").ToTask());
    }

    /// <summary>
    /// Test that confirms the view model should be garbage collected when overwritten.
    /// </summary>
    [Fact]
    public void ViewModelShouldBeGarbageCollectedWhenOverwritten()
    {
        static (IDisposable, WeakReference) GetWeakReference()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };
            var weakRef = new WeakReference(vm);
            var disposable = view.BindInteraction(
                vm,
                vm => vm.Interaction1,
                input =>
                    {
                        input.SetOutput(true);
                        return Task.CompletedTask;
                    });
            view.ViewModel = new InteractionBindViewModel();

            return (disposable, weakRef);
        }

        var (disposable, weakRef) = GetWeakReference();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Assert.False(weakRef.IsAlive);
    }

    /// <summary>
    /// Test that confirms nested view model should be garbage collected when overwritten.
    /// </summary>
    [Fact]
    public void NestedViewModelShouldBeGarbageCollectedWhenOverwritten()
    {
        static (IDisposable, WeakReference) GetWeakReference()
        {
            var vm = new InteractionAncestorViewModel() { InteractionViewModel = new InteractionBindViewModel() };
            var view = new InteractionAncestorView { ViewModel = vm };
            var weakRef = new WeakReference(vm.InteractionViewModel);
            var disposable = view.BindInteraction(
                vm,
                vm => vm.InteractionViewModel.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Observable.Return(Unit.Default);
                });
            vm.InteractionViewModel = new InteractionBindViewModel();

            return (disposable, weakRef);
        }

        var (disposable, weakRef) = GetWeakReference();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Assert.False(weakRef.IsAlive);
    }
}
