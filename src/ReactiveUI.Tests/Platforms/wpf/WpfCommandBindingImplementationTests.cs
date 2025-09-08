// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReactiveUI.Tests.Wpf;

[TestFixture]
[Apartment(ApartmentState.STA)]
public class WpfCommandBindingImplementationTests
{
    /// <summary>
    /// Commands the bind to explicit event wireup.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void CommandBindToExplicitEventWireup()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);

        var disp = view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

        disp.Dispose();

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        Assert.That(invokeCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Binds the command to object target is null.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void BindCommandToObjectWithEventTargetIsNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);

        var sub = new Subject<object>();
        Assert.Throws<Exception>(() =>
        {
            var disp = CreatesCommandBinding.BindCommandToObject(vm.Command2, true, sub, "MouseUp");

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        });

        Assert.That(invokeCount, Is.Zero);
    }

    /// <summary>
    /// Binds the command to object target is null.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void BindCommandToObjectTargetIsNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);

        var sub = new Subject<object>();
        Assert.Throws<Exception>(() =>
        {
            var disp = CreatesCommandBinding.BindCommandToObject(vm.Command2, true, sub);

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        });

        Assert.That(invokeCount, Is.Zero);
    }

    /// <summary>
    /// Binds the command to object target is null.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void BindCommandToObjectEventIsNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);

        var sub = new Subject<object>();
        Assert.Throws<Exception>(() =>
        {
            var disp = CreatesCommandBinding.BindCommandToObject(vm.Command2, vm, sub, "HappyMouseEvent");

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        });

        Assert.That(invokeCount, Is.Zero);
    }

    /// <summary>
    /// Binds the command to object command is null.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void BindCommandToObjectWithEventCommandIsArgumentNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);
        var btn = new Button();
        var cmd = (btn as ICommand)!;
        var sub = new Subject<object>();
        Assert.Throws<TargetInvocationException>(() =>
        {
            var disp = CreatesCommandBinding.BindCommandToObject(cmd, view, sub, "PropertyChanged");

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        });

        Assert.That(invokeCount, Is.Zero);
    }

    /// <summary>
    /// Binds the command to object command is null.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void BindCommandToObjectCommandIsArgumentNull()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        var invokeCount = 0;
        vm.Command2.Subscribe(_ => invokeCount++);
        var btn = new Button();
        var cmd = (btn as ICommand)!;
        var sub = new Subject<object>();
        Assert.Throws<Exception>(() =>
        {
            var disp = CreatesCommandBinding.BindCommandToObject(cmd, view, sub);

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        });

        Assert.That(invokeCount, Is.Zero);
    }

    /// <summary>
    /// Commands the bind view model to view with observable.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void CommandBindViewModelToViewWithObservable()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        // Create a paramenter feed
        vm.Command2.Subscribe(_ => vm.Value++);
        view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

        // Bind the command and the IObservable parameter.
        var fixture = new CommandBinderImplementation().BindCommand(vm, view, vm => vm.Command1, v => v.Command3, vm.WhenAnyValue(vm => vm.Value), "MouseUp");
        Assert.That(vm.Value, Is.Zero);

        // Confirm that the values update as expected.
        var parameter = 0;
        vm.Command1.Subscribe(i => parameter = i);
        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.Value, Is.EqualTo(1));
            Assert.That(parameter, Is.Zero);
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        Assert.That(parameter, Is.EqualTo(1));

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.Value, Is.EqualTo(2));
            Assert.That(parameter, Is.EqualTo(1));
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parameter, Is.EqualTo(2));
            Assert.That(vm.Value, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// Commands the bind view model to view with function.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void CommandBindViewModelToViewWithFunc()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };

        // Create a paramenter feed
        vm.Command2.Subscribe(_ => vm.Value++);
        view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

        // Bind the command and the Func<T> parameter.
        var fixture = new CommandBinderImplementation().BindCommand(vm, view, vm => vm.Command1, v => v.Command3, vm => vm.Value, "MouseUp");
        Assert.That(vm.Value, Is.Zero);

        // Confirm that the values update as expected.
        var parameter = 0;
        vm.Command1.Subscribe(i => parameter = i);
        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.Value, Is.EqualTo(1));
            Assert.That(parameter, Is.Zero);
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        Assert.That(parameter, Is.EqualTo(1));

        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.Value, Is.EqualTo(2));
            Assert.That(parameter, Is.EqualTo(1));
        }

        view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parameter, Is.EqualTo(2));
            Assert.That(vm.Value, Is.EqualTo(2));
        }
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public void BindCommandShouldNotWarnWhenBindingToFieldDeclaredInXaml()
    {
        var testLogger = new TestLogger();
        Locator.CurrentMutable.RegisterConstant<ILogger>(testLogger);

        var vm = new CommandBindingViewModel();
        var view = new FakeXamlCommandBindingView { ViewModel = vm };

        Assert.That(
            testLogger.Messages.Any(t =>
                t.message.Contains(nameof(POCOObservableForProperty)) &&
                t.message.Contains(view.NameOfButtonDeclaredInXaml) &&
                t.logLevel == LogLevel.Warn),
            Is.False);
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public void ViewModelShouldBeGarbageCollectedWhenOverwritten()
    {
        static (IDisposable, WeakReference) GetWeakReference()
        {
            var vm = new CommandBindingViewModel();
            var view = new CommandBindingView { ViewModel = vm };
            var weakRef = new WeakReference(vm);
            var disp = view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");
            view.ViewModel = new CommandBindingViewModel();

            return (disp, weakRef);
        }

        var (_, weakRef) = GetWeakReference();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Assert.That(weakRef.IsAlive, Is.False);
    }
}
