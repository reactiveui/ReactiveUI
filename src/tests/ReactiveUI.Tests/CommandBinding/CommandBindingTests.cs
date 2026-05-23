// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI.Builder;
using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Executors;

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
    /// <summary>
    /// Verifies that the command binder binds a command to a control event so the command executes when the event is raised.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Verifies that the command binder uses a custom <see cref="ICreatesCommandBinding" /> when the target has affinity for it.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
    public class CommandBindingExecutorTests : BaseAppBuilderTestExecutor
    {
        /// <inheritdoc />
        protected override void ConfigureAppBuilder(IReactiveUIBuilder builder, TestContext context)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(context);

            var scheduler = ImmediateScheduler.Instance;

            builder
                .WithMainThreadScheduler(scheduler)
                .WithTaskPoolScheduler(scheduler)
                .WithRegistration(r => r.RegisterConstant<ICreatesCommandBinding>(new CreatesCommandBindingViaEvent()))
                .WithRegistration(r => r.RegisterConstant<ICreatesCommandBinding>(new FakeCustomBinder()))
                .WithCoreServices();
        }
    }

    /// <summary>
    /// A fake control exposing an event used to test event-based command binding.
    /// </summary>
    private sealed class FakeControl
    {
        /// <summary>
        /// Occurs when the control is clicked.
        /// </summary>
        public event EventHandler? Click;

        /// <summary>
        /// Raises the <see cref="Click" /> event.
        /// </summary>
        public void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// A fake <see cref="ICreatesCommandBinding" /> used to verify custom binder selection.
    /// </summary>
    [SuppressMessage("Major Code Smell", "S4018:Generic methods should provide type parameters", Justification = "Type parameter cannot be inferred.")]
    private sealed class FakeCustomBinder : ICreatesCommandBinding
    {
        /// <summary>
        /// The high affinity returned for <see cref="FakeCustomControl" />.
        /// </summary>
        private const int HighAffinity = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCustomBinder" /> class.
        /// </summary>
        public FakeCustomBinder() => BindCalled = false;

        /// <summary>
        /// Gets or sets a value indicating whether a bind method was invoked.
        /// </summary>
        public static bool BindCalled { get; set; }

        /// <inheritdoc />
        [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
        public IDisposable BindCommandToObject<T>(ICommand? command, T? target, IObservable<object?> commandParameter)
            where T : class
        {
            BindCalled = true;
            return Disposable.Empty;
        }

        /// <inheritdoc />
        [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
        public IDisposable BindCommandToObject<T, TEventArgs>(
            ICommand? command,
            T? target,
            IObservable<object?> commandParameter,
            string eventName)
            where T : class
        {
            BindCalled = true;
            return Disposable.Empty;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public int GetAffinityForObject<T>(bool hasEventTarget) =>
            typeof(T) == typeof(FakeCustomControl) ? HighAffinity : 0;
    }

    /// <summary>
    /// A fake control type for which <see cref="FakeCustomBinder" /> has affinity.
    /// </summary>
    [SuppressMessage(
        "Minor Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class FakeCustomControl;

    /// <summary>
    /// A fake view exposing controls used in command binding tests.
    /// </summary>
    private sealed class FakeView : ReactiveObject, IViewFor<FakeViewModel>
    {
        /// <summary>
        /// The backing field for the <see cref="ViewModel" /> property.
        /// </summary>
        private FakeViewModel? _viewModel;

        /// <summary>
        /// Gets the standard control under test.
        /// </summary>
        public FakeControl Control { get; } = new();

        /// <summary>
        /// Gets the custom control under test.
        /// </summary>
        public FakeCustomControl CustomControl { get; } = new();

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public FakeViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        /// <inheritdoc />
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (FakeViewModel?)value;
        }
    }

    /// <summary>
    /// A fake view model exposing a command used in command binding tests.
    /// </summary>
    private sealed class FakeViewModel : ReactiveObject
    {
        /// <summary>
        /// Gets the command under test.
        /// </summary>
        public ReactiveCommand<Unit, Unit> Command { get; } = ReactiveCommand.Create(() => { });
    }
}
