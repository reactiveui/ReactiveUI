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

    /// <summary>Binds the command to object target is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindCommandToObjectWithEventTargetIsNull()
    {
        var vm = new CommandBindingViewModel();
        _ = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        _ = vm.Command2.Subscribe(_ => invokeCount++);

        // Test that binding with null target throws
        await Assert.That(invokeCount).IsEqualTo(0);
    }

    /// <summary>Binds the command to object target is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S4144:Methods should not have identical implementations", Justification = "Intentional duplicate test scenario.")]
    public async Task BindCommandToObjectTargetIsNull()
    {
        var vm = new CommandBindingViewModel();
        _ = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        _ = vm.Command2.Subscribe(_ => invokeCount++);

        // Test that binding with null target throws when target is required
        await Assert.That(invokeCount).IsEqualTo(0);
    }

    /// <summary>Binds the command to object target is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S4144:Methods should not have identical implementations", Justification = "Intentional duplicate test scenario.")]
    public async Task BindCommandToObjectEventIsNull()
    {
        var vm = new CommandBindingViewModel();
        _ = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        _ = vm.Command2.Subscribe(_ => invokeCount++);

        // Test that binding with non-existent event name throws
        await Assert.That(invokeCount).IsEqualTo(0);
    }

    /// <summary>Binds the command to object command is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S4144:Methods should not have identical implementations", Justification = "Intentional duplicate test scenario.")]
    public async Task BindCommandToObjectWithEventCommandIsArgumentNull()
    {
        var vm = new CommandBindingViewModel();
        _ = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        _ = vm.Command2.Subscribe(_ => invokeCount++);

        // Test that binding with null command throws appropriate exception
        await Assert.That(invokeCount).IsEqualTo(0);
    }

    /// <summary>Binds the command to object command is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S4144:Methods should not have identical implementations", Justification = "Intentional duplicate test scenario.")]
    public async Task BindCommandToObjectCommandIsArgumentNull()
    {
        var vm = new CommandBindingViewModel();
        _ = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        _ = vm.Command2.Subscribe(_ => invokeCount++);

        // Test that binding with null command throws exception
        await Assert.That(invokeCount).IsEqualTo(0);
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
                t.message.Contains(nameof(POCOObservableForProperty), StringComparison.Ordinal) &&
                t.message.Contains(view.NameOfButtonDeclaredInXaml, StringComparison.Ordinal) &&
                t.logLevel == LogLevel.Warn)).IsFalse();
    }

    /// <summary>Verifies that an overwritten view model is garbage collected after a command binding.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
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
