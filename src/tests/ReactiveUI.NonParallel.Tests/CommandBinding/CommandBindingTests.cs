// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Tests.Infrastructure.StaticState;
using Splat;

namespace ReactiveUI.NonParallel.Tests
{
    /// <summary>
    /// Tests for command binding.
    /// </summary>
    /// <remarks>
    /// This test fixture is marked as NotInParallel because tests call
    /// Locator.CurrentMutable to register ICreatesCommandBinding implementations,
    /// which mutate global service locator state.
    /// </remarks>
    [NotInParallel]
    public class CommandBindingTests
    {
        private LocatorScope? _locatorScope;

        [Before(Test)]
        public void SetUp()
        {
            _locatorScope = new LocatorScope();

            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.RegisterConstant(new CreatesCommandBindingViaEvent(), typeof(ICreatesCommandBinding));

            // Register a custom binder to test binder resolution
            Locator.CurrentMutable.RegisterConstant(new FakeCustomBinder(), typeof(ICreatesCommandBinding));
        }

        [After(Test)]
        public void TearDown()
        {
            _locatorScope?.Dispose();
        }

        [Test]
        public async Task CommandBinderImplementation_Should_Bind_Command_To_Event()
        {
            var binder = new CommandBinderImplementation();
            var viewModel = new FakeViewModel();
            var view = new FakeView { ViewModel = viewModel };

            var disp = binder.BindCommand(
                viewModel,
                view,
                vm => vm.Command,
                v => v.Control,
                Observable.Return((object?)null),
                "Click");

            await Assert.That(disp).IsNotNull();

            bool executed = false;
            viewModel.Command.Subscribe(_ => executed = true);

            view.Control.RaiseClick();

            await Assert.That(executed).IsTrue();
        }

        [Test]
        public async Task CommandBinderImplementation_Should_Use_Custom_Binder()
        {
            var binder = new CommandBinderImplementation();
            var viewModel = new FakeViewModel();
            var view = new FakeView { ViewModel = viewModel };

            // FakeCustomControl has affinity with FakeCustomBinder
            var disp = binder.BindCommand(
                viewModel,
                view,
                vm => vm.Command,
                v => v.CustomControl,
                Observable.Return((object?)null));

            await Assert.That(disp).IsNotNull();
            await Assert.That(FakeCustomBinder.BindCalled).IsTrue();
        }

        private class FakeViewModel : ReactiveObject
        {
            public ReactiveCommand<Unit, Unit> Command { get; } = ReactiveCommand.Create(() => { });
        }

        private class FakeView : ReactiveObject, IViewFor<FakeViewModel>
        {
            private FakeViewModel? _viewModel;

            public FakeViewModel? ViewModel
            {
                get => _viewModel;
                set => this.RaiseAndSetIfChanged(ref _viewModel, value);
            }

            object? IViewFor.ViewModel
            {
                get => ViewModel;
                set => ViewModel = (FakeViewModel?)value;
            }

            public FakeControl Control { get; } = new FakeControl();

            public FakeCustomControl CustomControl { get; } = new FakeCustomControl();
        }

        private class FakeControl
        {
            public event EventHandler? Click;

            public void RaiseClick()
            {
                Click?.Invoke(this, EventArgs.Empty);
            }
        }

        private class FakeCustomControl
        {
        }

        private class FakeCustomBinder : ICreatesCommandBinding
        {
            public FakeCustomBinder()
            {
                BindCalled = false;
            }

            public static bool BindCalled { get; set; }

            public int GetAffinityForObject<T>(bool hasEventTarget)
            {
                if (typeof(T) == typeof(FakeCustomControl))
                {
                    return 100; // High affinity
                }

                return 0;
            }

            [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
            public IDisposable? BindCommandToObject<T>(ICommand? command, T? target, IObservable<object?> commandParameter)
                where T : class
            {
                BindCalled = true;
                return Disposable.Empty;
            }

            [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
            public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter, string eventName)
                where T : class
            {
                BindCalled = true;
                return Disposable.Empty;
            }

            public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter, Action<EventHandler<TEventArgs>> addHandler, Action<EventHandler<TEventArgs>> removeHandler)
                where T : class
                where TEventArgs : EventArgs
            {
                BindCalled = true;
                return Disposable.Empty;
            }
        }
    }
}