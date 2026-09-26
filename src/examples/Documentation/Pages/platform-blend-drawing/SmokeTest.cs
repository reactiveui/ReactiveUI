// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Threading;
using ReactiveUI.Blend;

namespace ReactiveUI.Documentation.PlatformBlendDrawing;

/// <summary>
/// Drives the weather dashboard without a person at the keyboard, so a headless run on a real Windows machine can
/// confirm the app actually works. Started with the command-line argument <c>--smoke</c>.
/// </summary>
public static class SmokeTest
{
    /// <summary>Opens the window, moves it through every weather state, raises an alert, then closes it and exits.</summary>
    public static void Run()
    {
        MainWindow window = new();
        window.Show();
        PumpDispatcher();

        foreach (string state in new[] { "Windy", "Stormy", "Calm" })
        {
            window.ViewModel.State = state;
            PumpDispatcher();
            Console.WriteLine($"State: {window.ViewModel.State}, panel background: {window.StatusBrush.Color}");
        }

        _ = window.ViewModel.RaiseAlert.Execute("Storm warning issued").Subscribe();
        PumpDispatcher();
        Console.WriteLine($"Alert label: {window.AlertText.Text}");

        ShowSchedulerOverrideForTesting();

        Console.WriteLine("Closing window.");
        window.Close();

        Environment.Exit(0);
    }

    /// <summary>
    /// Builds a behavior and a trigger the same way this window's XAML does, but with <c>SchedulerOverride</c> set
    /// to <see cref="Sequencer.Immediate"/> before their observable is assigned. A unit test for a view that uses
    /// either type sets <c>SchedulerOverride</c> the same way, so it can assert the effect right after raising it
    /// instead of pumping a dispatcher and hoping it caught up. Setting <c>SchedulerOverride</c> only changes
    /// delivery for the next time the observable property is assigned; it does nothing to a subscription that
    /// already exists.
    /// </summary>
    private static void ShowSchedulerOverrideForTesting()
    {
        Border probeElement = new();
        FollowObservableStateBehavior stateBehavior = new() { SchedulerOverride = Sequencer.Immediate };
        stateBehavior.Attach(probeElement);
        stateBehavior.StateObservable = Signal.Emit("Stormy");
        Console.WriteLine("FollowObservableStateBehavior.SchedulerOverride delivered without a dispatcher pump.");
        stateBehavior.Detach();

        TextBlock probeText = new();
        ObservableTrigger alertTrigger = new() { SchedulerOverride = Sequencer.Immediate };
        alertTrigger.Actions.Add(new ShowAlertAction { TargetText = probeText });
        alertTrigger.Attach(probeElement);
        alertTrigger.Observable = Signal.Emit<object>("Immediate alert");
        Console.WriteLine($"ObservableTrigger.SchedulerOverride delivered without a dispatcher pump: {probeText.Text}");
        alertTrigger.Detach();
    }

    /// <summary>
    /// Runs the dispatcher queue until it is idle, the way a person watching the window would let WPF catch up
    /// between actions. <see cref="Blend.FollowObservableStateBehavior"/> and
    /// <see cref="Blend.ObservableTrigger"/> deliver through <c>RxSchedulers.MainThreadScheduler</c>,
    /// which posts back to this same dispatcher.
    /// </summary>
    private static void PumpDispatcher()
    {
        DispatcherFrame frame = new();
        _ = Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new DispatcherOperationCallback(ExitFrame),
            frame);
        Dispatcher.PushFrame(frame);
    }

    /// <summary>Stops the dispatcher frame started by <see cref="PumpDispatcher"/>.</summary>
    /// <param name="frame">The frame to stop.</param>
    /// <returns>Always <see langword="null"/>; required by <see cref="DispatcherOperationCallback"/>.</returns>
    private static object? ExitFrame(object frame)
    {
        ((DispatcherFrame)frame).Continue = false;
        return null;
    }
}
