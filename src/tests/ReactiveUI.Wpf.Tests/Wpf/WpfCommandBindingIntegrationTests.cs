// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Input;
using ReactiveUI.Tests.Wpf.Mocks;
using ReactiveUI.Tests.Xaml.Utilities;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Exercises generated Binding command calls against ReactiveUI commands and WPF controls.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class WpfCommandBindingIntegrationTests
{
    /// <summary>The routed event name used by the explicit-event binding.</summary>
    private const string MouseUpEventName = "MouseUp";

    /// <summary>Verifies an explicit routed event executes the command until the binding is disposed.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task BindCommand_ExplicitEvent_StopsAfterDisposal()
    {
        var vm = new CommandBindingViewModel();
        var view = new CommandBindingView { ViewModel = vm };
        var invokeCount = 0;
        using var commandSubscription = vm.Command2.Subscribe(_ => invokeCount++);

        var binding = view.BindCommand(vm, static x => x.Command2, static x => x.Command2, MouseUpEventName);
        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

        binding.Dispose();
        view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

        await Assert.That(invokeCount).IsEqualTo(1);
    }

    /// <summary>Verifies a background command replacement is written to its WPF target on the dispatcher.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task BindCommand_BackgroundReplacement_UsesDispatcher()
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

        await Assert.That(thrown).IsNull();
        await Assert.That(view.Command1.Command).IsSameReferenceAs(replacement);
    }
}
