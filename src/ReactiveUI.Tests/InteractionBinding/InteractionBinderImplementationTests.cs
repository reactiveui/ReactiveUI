// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace ReactiveUI.Tests
{
    public class InteractionBinderImplementationTests
    {
        [Fact]
        public async Task ShouldReceiveOutputFromTaskHandler()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.Interaction1,
                input =>
                    {
                        input.SetOutput(true);
                        return Task.CompletedTask;
                    });

            bool isDeletionConfirmed = await vm.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldReceiveOutputFromObservableHandler()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Observable.Return(Unit.Default);
                });

            bool isDeletionConfirmed = await vm.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldReceiveOutputFromTaskHandlerWhenViewModelWasInitiallyNull()
        {
            InteractionBindViewModel? vm = null;
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Task.CompletedTask;
                });

            view.ViewModel = new InteractionBindViewModel();

            bool isDeletionConfirmed = await view.ViewModel.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldReceiveOutputFromObservableHandlerWhenViewModelWasInitiallyNull()
        {
            InteractionBindViewModel? vm = null;
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Observable.Return(Unit.Default);
                });

            view.ViewModel = new InteractionBindViewModel();

            bool isDeletionConfirmed = await view.ViewModel.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public void ShouldUnregisterTaskHandlerWhenViewModelIsSetToNull()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
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

        [Fact]
        public void ShouldUnregisterObservableHandlerWhenViewModelIsSetToNull()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
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

        [Fact]
        public void ShouldUnregisterTaskHandlerFromOverwrittenViewModel()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
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

        [Fact]
        public void ShouldUnregisterObservableHandlerFromOverwrittenViewModel()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
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

        [Fact]
        public async Task ShouldRegisterTaskHandlerToNewlyAssignedViewModel()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Task.CompletedTask;
                });

            view.ViewModel = new InteractionBindViewModel();

            bool isDeletionConfirmed = await view.ViewModel.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldRegisterObservableHandlerToNewlyAssignedViewModel()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Observable.Return(Unit.Default);
                });

            view.ViewModel = new InteractionBindViewModel();

            bool isDeletionConfirmed = await view.ViewModel.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task NestedInteractionShouldReceiveOutputFromTaskHandler()
        {
            var vm = new InteractionAncestorViewModel();
            var view = new InteractionAncestorView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.InteractionViewModel.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Task.CompletedTask;
                });

            bool isDeletionConfirmed = await vm.InteractionViewModel.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task NestedInteractionShouldReceiveOutputFromObservableHandler()
        {
            var vm = new InteractionAncestorViewModel();
            var view = new InteractionAncestorView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.InteractionViewModel.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Observable.Return(Unit.Default);
                });

            bool isDeletionConfirmed = await vm.InteractionViewModel.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public void ShouldUnregisterTaskHandlerFromOverwrittenNestedViewModel()
        {
            var firstInteractionVm = new InteractionBindViewModel();
            var vm = new InteractionAncestorViewModel();
            var view = new InteractionAncestorView { ViewModel = vm };

            var disp = view.BindInteraction(
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

        [Fact]
        public void ShouldUnregisterObservableHandlerFromOverwrittenNestedViewModel()
        {
            var firstInteractionVm = new InteractionBindViewModel();
            var vm = new InteractionAncestorViewModel();
            var view = new InteractionAncestorView { ViewModel = vm };

            var disp = view.BindInteraction(
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

        [Fact]
        public async Task ShouldRegisterTaskHandlerToNewlyAssignedNestedViewModel()
        {
            var vm = new InteractionAncestorViewModel()
            {
                InteractionViewModel = new InteractionBindViewModel()
            };
            var view = new InteractionAncestorView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.InteractionViewModel.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Observable.Return(Unit.Default);
                });

            vm.InteractionViewModel = new InteractionBindViewModel();

            bool isDeletionConfirmed = await vm.InteractionViewModel.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldRegisterObservableHandlerToNewlyAssignedNestedViewModel()
        {
            var vm = new InteractionAncestorViewModel()
            {
                InteractionViewModel = new InteractionBindViewModel()
            };
            var view = new InteractionAncestorView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.InteractionViewModel.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Observable.Return(Unit.Default);
                });

            vm.InteractionViewModel = new InteractionBindViewModel();

            bool isDeletionConfirmed = await vm.InteractionViewModel.Interaction1.Handle("123");

            isDeletionConfirmed.Should().BeTrue();
        }

        [Fact]
        public void ShouldUnregisterTaskHandlerWhenBindingIsDisposed()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Task.CompletedTask;
                });

            disp.Dispose();

            _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => vm.Interaction1.Handle("123").ToTask());
        }

        [Fact]
        public void ShouldUnregisterObservableHandlerWhenBindingIsDisposed()
        {
            var vm = new InteractionBindViewModel();
            var view = new InteractionBindView { ViewModel = vm };

            var disp = view.BindInteraction(
                vm,
                vm => vm.Interaction1,
                input =>
                {
                    input.SetOutput(true);
                    return Observable.Return(Unit.Default);
                });

            disp.Dispose();

            _ = Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => vm.Interaction1.Handle("123").ToTask());
        }

        [Fact]
        public void ViewModelShouldBeGarbageCollectedWhenOverwritten()
        {
            static (IDisposable, WeakReference) GetWeakReference()
            {
                var vm = new InteractionBindViewModel();
                var view = new InteractionBindView { ViewModel = vm };
                var weakRef = new WeakReference(vm);
                var disp = view.BindInteraction(
                    vm,
                    vm => vm.Interaction1,
                    input =>
                        {
                            input.SetOutput(true);
                            return Task.CompletedTask;
                        });
                view.ViewModel = new InteractionBindViewModel();

                return (disp, weakRef);
            }

            var (disp, weakRef) = GetWeakReference();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(weakRef.IsAlive);
        }

        [Fact]
        public void NestedViewModelShouldBeGarbageCollectedWhenOverwritten()
        {
            static (IDisposable, WeakReference) GetWeakReference()
            {
                var vm = new InteractionAncestorViewModel() { InteractionViewModel = new InteractionBindViewModel() };
                var view = new InteractionAncestorView { ViewModel = vm };
                var weakRef = new WeakReference(vm.InteractionViewModel);
                var disp = view.BindInteraction(
                    vm,
                    vm => vm.InteractionViewModel.Interaction1,
                    input =>
                    {
                        input.SetOutput(true);
                        return Observable.Return(Unit.Default);
                    });
                vm.InteractionViewModel = new InteractionBindViewModel();

                return (disp, weakRef);
            }

            var (disp, weakRef) = GetWeakReference();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(weakRef.IsAlive);
        }
    }
}
