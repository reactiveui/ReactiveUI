// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using ReactiveUI.Builder;

namespace ReactiveUI.Tests.CommandBinding;

/// <summary>
///     Tests for command binding.
/// </summary>
/// <remarks>
///     This test fixture is marked as NotInParallel because tests call
///     Locator.CurrentMutable to register ICreatesCommandBinding implementations,
///     which mutate global service locator state.
/// </remarks>
[NotInParallel]
[TestExecutor<CommandBindingExecutorTests>]
public class CommandBindingTests
{
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

        var executed = false;
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

    /// <summary>
    /// Provides test execution support for command binding scenarios using the ReactiveUI framework.
    /// </summary>
    public class CommandBindingExecutorTests : ITestExecutor
    {
        /// <inheritdoc />
        public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(action);

            RxAppBuilder.ResetForTesting();
            RxAppBuilder
                .CreateReactiveUIBuilder()
                .WithRegistration(r => r.RegisterConstant<ICreatesCommandBinding>(new CreatesCommandBindingViaEvent()))
                .WithRegistration(r => r.RegisterConstant<ICreatesCommandBinding>(new FakeCustomBinder()))
                .WithCoreServices()
                .BuildApp();

            try
            {
                await action();
            }
            finally
            {
                RxAppBuilder.ResetForTesting();
            }
        }
    }

    private class FakeControl
    {
        public event EventHandler? Click;

        public void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);
    }

    private class FakeCustomBinder : ICreatesCommandBinding
    {
        public FakeCustomBinder() => BindCalled = false;

        public static bool BindCalled { get; set; }

        [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
        public IDisposable? BindCommandToObject<T>(ICommand? command, T? target, IObservable<object?> commandParameter)
            where T : class
        {
            BindCalled = true;
            return Disposable.Empty;
        }

        [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
        public IDisposable? BindCommandToObject<T, TEventArgs>(
            ICommand? command,
            T? target,
            IObservable<object?> commandParameter,
            string eventName)
            where T : class
        {
            BindCalled = true;
            return Disposable.Empty;
        }

        public IDisposable? BindCommandToObject<T, TEventArgs>(
            ICommand? command,
            T? target,
            IObservable<object?> commandParameter,
            Action<EventHandler<TEventArgs>> addHandler,
            Action<EventHandler<TEventArgs>> removeHandler)
            where T : class
            where TEventArgs : EventArgs
        {
            BindCalled = true;
            return Disposable.Empty;
        }

        public int GetAffinityForObject<T>(bool hasEventTarget)
        {
            if (typeof(T) == typeof(FakeCustomControl))
            {
                return 100; // High affinity
            }

            return 0;
        }
    }

    private class FakeCustomControl
    {
    }

    private class FakeView : ReactiveObject, IViewFor<FakeViewModel>
    {
        private FakeViewModel? _viewModel;

        public FakeControl Control { get; } = new();

        public FakeCustomControl CustomControl { get; } = new();

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
    }

    private class FakeViewModel : ReactiveObject
    {
        public ReactiveCommand<Unit, Unit> Command { get; } = ReactiveCommand.Create(() => { });
    }
}
