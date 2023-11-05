// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using FluentAssertions;

using FactAttribute = Xunit.WpfFactAttribute;

namespace ReactiveUI.Tests.Wpf
{
    public class WpfCommandBindingImplementationTests
    {
        /// <summary>
        /// Commands the bind to explicit event wireup.
        /// </summary>
        [Fact]
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
            Assert.Equal(1, invokeCount);
        }

        /// <summary>
        /// Binds the command to object target is null.
        /// </summary>
        [Fact]
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

            Assert.Equal(0, invokeCount);
        }

        /// <summary>
        /// Binds the command to object target is null.
        /// </summary>
        [Fact]
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

            Assert.Equal(0, invokeCount);
        }

        /// <summary>
        /// Binds the command to object target is null.
        /// </summary>
        [Fact]
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

            Assert.Equal(0, invokeCount);
        }

        /// <summary>
        /// Binds the command to object command is null.
        /// </summary>
        [Fact]
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

            Assert.Equal(0, invokeCount);
        }

        /// <summary>
        /// Binds the command to object command is null.
        /// </summary>
        [Fact]
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

            Assert.Equal(0, invokeCount);
        }

        /// <summary>
        /// Commands the bind view model to view with observable.
        /// </summary>
        [Fact]
        public void CommandBindViewModelToViewWithObservable()
        {
            var vm = new CommandBindingViewModel();
            var view = new CommandBindingView { ViewModel = vm };

            // Create a paramenter feed
            vm.Command2.Subscribe(_ => vm.Value++);
            view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

            // Bind the command and the IObservable parameter.
            var fixture = new CommandBinderImplementation().BindCommand(vm, view, vm => vm.Command1, v => v.Command3, vm.WhenAnyValue(vm => vm.Value), "MouseUp");
            Assert.Equal(0, vm.Value);

            // Confirm that the values update as expected.
            var parameter = 0;
            vm.Command1.Subscribe(i => parameter = i);
            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(1, vm.Value);
            Assert.Equal(0, parameter);

            view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(1, parameter);

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(2, vm.Value);
            Assert.Equal(1, parameter);

            view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(2, parameter);
            Assert.Equal(2, vm.Value);
        }

        /// <summary>
        /// Commands the bind view model to view with function.
        /// </summary>
        [Fact]
        public void CommandBindViewModelToViewWithFunc()
        {
            var vm = new CommandBindingViewModel();
            var view = new CommandBindingView { ViewModel = vm };

            // Create a paramenter feed
            vm.Command2.Subscribe(_ => vm.Value++);
            view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

            // Bind the command and the Func<T> parameter.
            var fixture = new CommandBinderImplementation().BindCommand(vm, view, vm => vm.Command1, v => v.Command3, vm => vm.Value, "MouseUp");
            Assert.Equal(0, vm.Value);

            // Confirm that the values update as expected.
            var parameter = 0;
            vm.Command1.Subscribe(i => parameter = i);
            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(1, vm.Value);
            Assert.Equal(0, parameter);

            view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(1, parameter);

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(2, vm.Value);
            Assert.Equal(1, parameter);

            view.Command3.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(2, parameter);
            Assert.Equal(2, vm.Value);
        }

        [Fact]
        public void BindCommandShouldNotWarnWhenBindingToFieldDeclaredInXaml()
        {
            var testLogger = new TestLogger();
            Locator.CurrentMutable.RegisterConstant<ILogger>(testLogger);

            var vm = new CommandBindingViewModel();
            var view = new FakeXamlCommandBindingView { ViewModel = vm };

            testLogger.Messages.Should().NotContain(t => t.message.Contains(nameof(POCOObservableForProperty)) && t.message.Contains(view.NameOfButtonDeclaredInXaml) && t.logLevel == LogLevel.Warn);
        }

        [Fact]
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

            var (disp, weakRef) = GetWeakReference();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(weakRef.IsAlive);
        }
    }
}
