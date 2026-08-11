// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Input;

using ReactiveUI.Tests.Utilities.Logging;
using ReactiveUI.Tests.Wpf.Mocks;
using ReactiveUI.Tests.Xaml.Mocks;
using ReactiveUI.Tests.Xaml.Utilities;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for WPF command binding implementation.</summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because some tests call
/// Locator.CurrentMutable.RegisterConstant() to register test loggers, which mutates
/// global service locator state. This state must not be mutated concurrently by parallel tests.
/// </remarks>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class WpfCommandBindingImplementationTests
{
    /// <summary>The expected accumulated value after the command is invoked a second time.</summary>
    private const int ExpectedSecondInvocation = 2;

    /// <summary>The name of the mouse up routed event used for explicit event wiring.</summary>
    private const string MouseUpEventName = "MouseUp";

    /// <summary>The null-input command-binding cases exercised by the parameterized test.</summary>
    public enum NullBindingCase
    {
        /// <summary>An explicit-event binding with no target.</summary>
        ExplicitEventNullTarget = 0,

        /// <summary>A default-event binding with no target.</summary>
        DefaultEventNullTarget = 1,

        /// <summary>An explicit-event binding with no event name.</summary>
        NullEventName = 2,

        /// <summary>An explicit-event binding with no command.</summary>
        ExplicitEventNullCommand = 3,

        /// <summary>A default-event binding with no command.</summary>
        DefaultEventNullCommand = 4,
    }

    /// <summary>Commands the bind to explicit event wireup.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindToExplicitEventWireup()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        _ = vm.Command2.Subscribe(_ => invokeCount++);

        var disp = view.BindCommand(vm, x => x.Command2, x => x.Command2, MouseUpEventName);

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

        disp.Dispose();

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        await Assert.That(invokeCount).IsEqualTo(1);
    }

    /// <summary>Verifies command binding handles null targets, event names, and commands according to its contract.</summary>
    /// <param name="bindingCase">The null-input case to exercise.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(NullBindingCase.ExplicitEventNullTarget)]
    [Arguments(NullBindingCase.DefaultEventNullTarget)]
    [Arguments(NullBindingCase.NullEventName)]
    [Arguments(NullBindingCase.ExplicitEventNullCommand)]
    [Arguments(NullBindingCase.DefaultEventNullCommand)]
    public async Task BindCommandToObject_NullInput_HandlesAsDocumented(NullBindingCase bindingCase)
    {
        var command = ReactiveCommand.Create(static () => { }, outputScheduler: Sequencer.Immediate);
        var target = new System.Windows.Controls.Button();
        var parameter = Signal.Emit<object?>(null);

        IDisposable Bind() => bindingCase switch
        {
            NullBindingCase.ExplicitEventNullTarget => CreatesCommandBinding.BindCommandToObject<System.Windows.Controls.Button, RoutedEventArgs>(
                command,
                null,
                parameter,
                nameof(System.Windows.Controls.Button.Click)),
            NullBindingCase.DefaultEventNullTarget => CreatesCommandBinding.BindCommandToObject<System.Windows.Controls.Button>(command, null, parameter),
            NullBindingCase.NullEventName => CreatesCommandBinding.BindCommandToObject<System.Windows.Controls.Button, RoutedEventArgs>(command, target, parameter, null!),
            NullBindingCase.ExplicitEventNullCommand => CreatesCommandBinding.BindCommandToObject<System.Windows.Controls.Button, RoutedEventArgs>(
                null,
                target,
                parameter,
                nameof(System.Windows.Controls.Button.Click)),
            NullBindingCase.DefaultEventNullCommand => CreatesCommandBinding.BindCommandToObject(null, target, parameter),
            _ => throw new ArgumentOutOfRangeException(nameof(bindingCase), bindingCase, null)
        };

        if (bindingCase is NullBindingCase.ExplicitEventNullTarget or NullBindingCase.DefaultEventNullTarget or NullBindingCase.NullEventName)
        {
            _ = Assert.Throws<ArgumentNullException>(() => Bind());
            return;
        }

        using var binding = Bind();
        await Assert.That(binding).IsNotNull();
    }

    /// <summary>Commands the bind view model to view with observable.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindViewModelToViewWithObservable()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        // Create a paramenter feed
        _ = vm.Command2.Subscribe(_ => vm.Value++);
        _ = view.BindCommand(vm, x => x.Command2, x => x.Command2, MouseUpEventName);

        // Bind the command and the IObservable parameter.
        _ = new CommandBinderImplementation().BindCommand(vm, view, vm => vm.Command1, v => v.Command3, vm.WhenAnyValue(vm => vm.Value), MouseUpEventName);
        await Assert.That(vm.Value).IsEqualTo(0);

        // Confirm that the values update as expected.
        var parameter = 0;
        _ = vm.Command1.Subscribe(i => parameter = i);
        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.Multiple())
        {
            await Assert.That(vm.Value).IsEqualTo(1);
            await Assert.That(parameter).IsEqualTo(0);
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        await Assert.That(parameter).IsEqualTo(1);

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.Multiple())
        {
            await Assert.That(vm.Value).IsEqualTo(ExpectedSecondInvocation);
            await Assert.That(parameter).IsEqualTo(1);
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.Multiple())
        {
            await Assert.That(parameter).IsEqualTo(ExpectedSecondInvocation);
            await Assert.That(vm.Value).IsEqualTo(ExpectedSecondInvocation);
        }
    }

    /// <summary>Commands the bind view model to view with function.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindViewModelToViewWithFunc()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        // Create a paramenter feed
        _ = vm.Command2.Subscribe(_ => vm.Value++);
        _ = view.BindCommand(vm, x => x.Command2, x => x.Command2, MouseUpEventName);

        // Bind the command and the Func<T> parameter.
        _ = new CommandBinderImplementation().BindCommand(vm, view, vm => vm.Command1, v => v.Command3, vm => vm.Value, MouseUpEventName);
        await Assert.That(vm.Value).IsEqualTo(0);

        // Confirm that the values update as expected.
        var parameter = 0;
        _ = vm.Command1.Subscribe(i => parameter = i);
        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.Multiple())
        {
            await Assert.That(vm.Value).IsEqualTo(1);
            await Assert.That(parameter).IsEqualTo(0);
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        await Assert.That(parameter).IsEqualTo(1);

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.Multiple())
        {
            await Assert.That(vm.Value).IsEqualTo(ExpectedSecondInvocation);
            await Assert.That(parameter).IsEqualTo(1);
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.Multiple())
        {
            await Assert.That(parameter).IsEqualTo(ExpectedSecondInvocation);
            await Assert.That(vm.Value).IsEqualTo(ExpectedSecondInvocation);
        }
    }

    /// <summary>Verifies that binding a command to a XAML-declared field does not log a warning.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindCommandShouldNotWarnWhenBindingToFieldDeclaredInXaml()
    {
        var testLogger = new TestLogger();
        AppLocator.CurrentMutable.RegisterConstant<ILogger>(testLogger);

        var vm = new CommandBindingViewModel();
        var view = new FakeXamlCommandBindingView { ViewModel = vm };

        await Assert.That(testLogger.Messages.Exists(t =>
                t.Message.Contains(nameof(POCOObservableForProperty), StringComparison.Ordinal)
                && t.Message.Contains(view.NameOfButtonDeclaredInXaml, StringComparison.Ordinal)
                && t.LogLevel == LogLevel.Warn)).IsFalse();
    }

    /// <summary>Verifies that an overwritten view model is garbage collected after a command binding.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1021:Do not force garbage collection",
        Justification = "This test verifies the view model is finalized once overwritten, which requires forcing a collection.")]
    public async Task ViewModelShouldBeGarbageCollectedWhenOverwritten()
    {
        static (IDisposable, WeakReference) GetWeakReference()
        {
            var vm = new CommandBindingViewModel();
            var view = new CommandBindingView { ViewModel = vm };
            var weakRef = new WeakReference(vm);
            var disp = view.BindCommand(vm, static x => x.Command2, static x => x.Command2, MouseUpEventName);
            view.ViewModel = new();

            return (disp, weakRef);
        }

        var (_, weakRef) = GetWeakReference();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        await Assert.That(weakRef.IsAlive).IsFalse();
    }

    /// <summary>Verifies that the command and its parameter rebind when the view model instance is replaced.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandAndParameterRebindToNewViewModelInstance()
    {
        var vm = new CommandBindingViewModel { Value = 1 };
        var view = new CommandBindingView { ViewModel = vm };

        var received1 = 0;
        _ = view.ViewModel.Command1.Subscribe(i => received1 = i);

        _ = new CommandBinderImplementation().BindCommand(vm, view, vm => vm.Command1, v => v.Command1, vm => vm.Value, nameof(CustomClickButton.CustomClick));

        view.ViewModel = new() { Value = ExpectedSecondInvocation };

        var received2 = 0;
        _ = view.ViewModel.Command1.Subscribe(i => received2 = i);

        view.Command1.RaiseCustomClick();

        using (Assert.Multiple())
        {
            await Assert.That(received1).IsEqualTo(0);
            await Assert.That(received2).IsEqualTo(ExpectedSecondInvocation);
        }
    }

    /// <summary>Verifies that rebinding a command from a background thread does not touch the WPF control directly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandRebindingFromBackgroundThreadDoesNotTouchWpfControlDirectly()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };
        using var binding = view.BindCommand(vm, static x => x.Command2, static x => x.Command1);
        var replacement = ReactiveCommand.Create(static () => { }, outputScheduler: Sequencer.Immediate);

        Exception? thrown = null;
        await Task.Run(() =>
        {
            try
            {
                vm.Command2 = replacement;
            }
            catch (Exception ex)
            {
                thrown = ex;
            }
        });

        DispatcherUtilities.DoEvents();

        using (Assert.Multiple())
        {
            await Assert.That(thrown).IsNull();
            await Assert.That(view.Command1.Command).IsSameReferenceAs(replacement);
        }
    }
}
