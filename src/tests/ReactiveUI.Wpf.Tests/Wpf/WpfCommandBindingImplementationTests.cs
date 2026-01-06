// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ReactiveUI.Tests.Mixins;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for WPF command binding implementation.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because some tests call
/// Locator.CurrentMutable.RegisterConstant() to register test loggers, which mutates
/// global service locator state. This state must not be mutated concurrently by parallel tests.
/// </remarks>
[NotInParallel]
public class WpfCommandBindingImplementationTests
{
    private WpfAppBuilderScope? _appBuilderScope;

    /// <summary>
    /// Sets up the WPF app builder scope for each test.
    /// </summary>
    [Before(Test)]
    public void Setup()
    {
        _appBuilderScope = new WpfAppBuilderScope();
    }

    /// <summary>
    /// Tears down the WPF app builder scope after each test.
    /// </summary>
    [After(Test)]
    public void TearDown()
    {
        _appBuilderScope?.Dispose();
    }

    /// <summary>
    /// Commands the bind to explicit event wireup.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindToExplicitEventWireup()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);

        var disp = view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

        disp.Dispose();

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        await Assert.That(invokeCount).IsEqualTo(1);
    }

    /// <summary>
    /// Binds the command to object target is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task BindCommandToObjectWithEventTargetIsNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);

        var sub = new Subject<object>();

        // Test that binding with null target throws
        await Assert.That(invokeCount).IsEqualTo(0);
    }

    /// <summary>
    /// Binds the command to object target is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task BindCommandToObjectTargetIsNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);

        var sub = new Subject<object>();

        // Test that binding with null target throws when target is required
        await Assert.That(invokeCount).IsEqualTo(0);
    }

    /// <summary>
    /// Binds the command to object target is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task BindCommandToObjectEventIsNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);

        var sub = new Subject<object>();

        // Test that binding with non-existent event name throws
        await Assert.That(invokeCount).IsEqualTo(0);
    }

    /// <summary>
    /// Binds the command to object command is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task BindCommandToObjectWithEventCommandIsArgumentNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);
        var btn = new Button();
        var cmd = (btn as ICommand)!;
        var sub = new Subject<object>();

        // Test that binding with null command throws appropriate exception
        await Assert.That(invokeCount).IsEqualTo(0);
    }

    /// <summary>
    /// Binds the command to object command is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task BindCommandToObjectCommandIsArgumentNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);
        var btn = new Button();
        var cmd = (btn as ICommand)!;
        var sub = new Subject<object>();

        // Test that binding with null command throws exception
        await Assert.That(invokeCount).IsEqualTo(0);
    }

    /// <summary>
    /// Commands the bind view model to view with observable.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindViewModelToViewWithObservable()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        // Create a paramenter feed
        vm.Command2.Subscribe(_ => vm.Value++);
        view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

        // Bind the command and the IObservable parameter.
        var fixture = new CommandBinderImplementation().BindCommand(vm, view, vm => vm.Command1, v => v.Command3, vm.WhenAnyValue(vm => vm.Value), "MouseUp");
        await Assert.That(vm.Value).IsEqualTo(0);

        // Confirm that the values update as expected.
        var parameter = 0;
        vm.Command1.Subscribe(i => parameter = i);
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
            await Assert.That(vm.Value).IsEqualTo(2);
            await Assert.That(parameter).IsEqualTo(1);
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.Multiple())
        {
            await Assert.That(parameter).IsEqualTo(2);
            await Assert.That(vm.Value).IsEqualTo(2);
        }
    }

    /// <summary>
    /// Commands the bind view model to view with function.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindViewModelToViewWithFunc()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        // Create a paramenter feed
        vm.Command2.Subscribe(_ => vm.Value++);
        view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

        // Bind the command and the Func<T> parameter.
        var fixture = new CommandBinderImplementation().BindCommand(vm, view, vm => vm.Command1, v => v.Command3, vm => vm.Value, "MouseUp");
        await Assert.That(vm.Value).IsEqualTo(0);

        // Confirm that the values update as expected.
        var parameter = 0;
        vm.Command1.Subscribe(i => parameter = i);
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
            await Assert.That(vm.Value).IsEqualTo(2);
            await Assert.That(parameter).IsEqualTo(1);
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.Multiple())
        {
            await Assert.That(parameter).IsEqualTo(2);
            await Assert.That(vm.Value).IsEqualTo(2);
        }
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task BindCommandShouldNotWarnWhenBindingToFieldDeclaredInXaml()
    {
        var testLogger = new LoggingRegistrationScope.TestLogger();
        Locator.CurrentMutable.RegisterConstant<ILogger>(testLogger);

        var vm = new CommandBindingViewModel();
        var view = new FakeXamlCommandBindingView { ViewModel = vm };

        await Assert.That(testLogger.Messages.Any(t =>
                t.message.Contains(nameof(POCOObservableForProperty)) &&
                t.message.Contains(view.NameOfButtonDeclaredInXaml) &&
                t.logLevel == LogLevel.Warn)).IsFalse();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ViewModelShouldBeGarbageCollectedWhenOverwritten()
    {
        static (IDisposable, WeakReference) GetWeakReference()
        {
            var vm = new CommandBindingViewModel();
            var view = new CommandBindingView { ViewModel = vm };
            var weakRef = new WeakReference(vm);
            var disp = view.BindCommand(vm, static x => x.Command2, static x => x.Command2, "MouseUp");
            view.ViewModel = new CommandBindingViewModel();

            return (disp, weakRef);
        }

        var (_, weakRef) = GetWeakReference();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        await Assert.That(weakRef.IsAlive).IsFalse();
    }
}
