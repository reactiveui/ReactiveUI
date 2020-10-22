// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Input;

using FluentAssertions;

using Splat;

using Xunit;

using FactAttribute = Xunit.WpfFactAttribute;

namespace ReactiveUI.Tests.Wpf
{
    public class WpfCommandBindingImplementationTests
    {
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
