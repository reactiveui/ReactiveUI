// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for ObservableTrigger.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class ObservableTriggerTests
{
    /// <summary>The delay, in milliseconds, allowed for an invoked action to settle.</summary>
    private const int InvokeSettleDelayMs = 100;

    /// <summary>The delay, in milliseconds, allowed for disposal to settle.</summary>
    private const int DisposeSettleDelayMs = 50;

    /// <summary>The delay, in milliseconds, allowed for multiple emissions to settle.</summary>
    private const int MultiEmitSettleDelayMs = 150;

    /// <summary>The number of errors observed before the test sequence is considered complete.</summary>
    private const int MaxErrorsBeforeComplete = 3;

    /// <summary>The expected number of subscriptions established by the trigger.</summary>
    private const int ExpectedSubscriptionCount = 4;

    /// <summary>The expected number of times the trigger invokes its actions.</summary>
    private const int ExpectedInvokeCount = 3;

    /// <summary>Tests that the trigger invokes actions when the observable emits.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Observable_WhenEmits_InvokesActions()
    {
        var button = new Button();
        var trigger = new ObservableTrigger
        {
            SchedulerOverride = Sequencer.Immediate
        };
        var subject = new Signal<object>();
        var actionInvoked = false;

        // Create a test action
        var action = new TestAction
        {
            OnInvoke = _ => actionInvoked = true
        };

        // Attach trigger to button
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);
        trigger.Actions.Add(action);

        // Set the observable
        trigger.Observable = subject.AsObservable();

        // Emit a value
        subject.OnNext(new object());

        await Task.Delay(InvokeSettleDelayMs);

        await Assert.That(actionInvoked).IsTrue();
    }

    /// <summary>Tests that OnObservableChanged throws ArgumentException for invalid sender.</summary>
    [Test]
    public void OnObservableChanged_InvalidSender_ThrowsArgumentException()
    {
        var button = new Button();
        var subject = new Signal<object>();
        var eventArgs = new DependencyPropertyChangedEventArgs(
            ObservableTrigger.ObservableProperty,
            null,
            subject.AsObservable());

        _ = Assert.Throws<ArgumentException>(() =>
            ObservableTrigger.InternalOnObservableChangedForTesting(button, eventArgs));
    }

    /// <summary>Tests that OnObservableChanged throws ArgumentNullException for default event args.</summary>
    [Test]
    public void OnObservableChanged_DefaultEventArgs_ThrowsArgumentNullException()
    {
        var trigger = new ObservableTrigger();
        var button = new Button();
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        _ = Assert.Throws<ArgumentNullException>(() =>
            ObservableTrigger.InternalOnObservableChangedForTesting(trigger, default));
    }

    /// <summary>Tests that OnObservableChanged throws ArgumentNullException for null NewValue.</summary>
    [Test]
    public void OnObservableChanged_NullNewValue_ThrowsArgumentNullException()
    {
        var trigger = new ObservableTrigger();
        var button = new Button();
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        var eventArgs = new DependencyPropertyChangedEventArgs(
            ObservableTrigger.ObservableProperty,
            null,
            null!);

        _ = Assert.Throws<ArgumentNullException>(() =>
            ObservableTrigger.InternalOnObservableChangedForTesting(trigger, eventArgs));
    }

    /// <summary>Tests that Observable getter returns the set value.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Observable_Getter_ReturnsSetValue()
    {
        var button = new Button();
        var trigger = new ObservableTrigger
        {
            SchedulerOverride = Sequencer.Immediate
        };
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        var subject = new Signal<object>();
        trigger.Observable = subject.AsObservable();

        // Read the getter to cover that line
        var observable = trigger.Observable;

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Tests that setting a new observable disposes the previous subscription.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Observable_WhenChangedMultipleTimes_DisposesOldSubscription()
    {
        var button = new Button();
        var trigger = new ObservableTrigger
        {
            SchedulerOverride = Sequencer.Immediate
        };
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        var disposed1 = false;
        var observable1 = Signal.Create<object>(observer => Scope.Create(() => disposed1 = true));

        var observable2 = Signal.Silent<object>();

        // Set first observable
        trigger.Observable = observable1;
        await Task.Delay(DisposeSettleDelayMs);

        // Set second observable (should dispose first)
        trigger.Observable = observable2;
        await Task.Delay(DisposeSettleDelayMs);

        await Assert.That(disposed1).IsTrue();
    }

    /// <summary>Tests that AutoResubscribeOnError resubscribes after an error.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoResubscribeOnError_WhenTrue_ResubscribesAfterError()
    {
        var button = new Button();
        var scheduler = new VirtualTimeScheduler();
        var trigger = new ObservableTrigger
        {
            AutoResubscribeOnError = true,
            SchedulerOverride = scheduler
        };
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        var errorCount = 0;
        trigger.Observable = Signal.Create<object>(observer =>
        {
            errorCount++;
            if (errorCount <= MaxErrorsBeforeComplete)
            {
                observer.OnError(new InvalidOperationException("Test error"));
            }
            else
            {
                observer.OnCompleted();
            }

            return Scope.Empty;
        });

        scheduler.Start();

        // Should have resubscribed 3 times (errored 3 times, then completed on 4th)
        await Assert.That(errorCount).IsEqualTo(ExpectedSubscriptionCount);
    }

    /// <summary>Tests that AutoResubscribeOnError false does not resubscribe after error.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoResubscribeOnError_WhenFalse_DoesNotResubscribe()
    {
        var button = new Button();
        var scheduler = new VirtualTimeScheduler();
        var trigger = new ObservableTrigger
        {
            AutoResubscribeOnError = false,
            SchedulerOverride = scheduler
        };
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        var errorCount = 0;
        trigger.Observable = Signal.Create<object>(observer =>
        {
            errorCount++;
            observer.OnError(new InvalidOperationException("Test error"));
            return Scope.Empty;
        });

        scheduler.Start();

        // Should only subscribe once
        await Assert.That(errorCount).IsEqualTo(1);
    }

    /// <summary>Tests that multiple emissions invoke actions multiple times.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Observable_MultipleEmissions_InvokesActionsMultipleTimes()
    {
        var button = new Button();
        var trigger = new ObservableTrigger
        {
            SchedulerOverride = Sequencer.Immediate
        };
        var subject = new Signal<object>();
        var invokeCount = 0;

        var action = new TestAction
        {
            OnInvoke = _ => invokeCount++
        };

        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);
        trigger.Actions.Add(action);

        trigger.Observable = subject.AsObservable();

        // Emit multiple values
        subject.OnNext(new object());
        subject.OnNext(new object());
        subject.OnNext(new object());

        await Task.Delay(MultiEmitSettleDelayMs);

        await Assert.That(invokeCount).IsEqualTo(ExpectedInvokeCount);
    }

    /// <summary>Test action that can be customized.</summary>
    private sealed class TestAction : TriggerAction<DependencyObject>
    {
        /// <summary>Gets or sets the action to invoke.</summary>
        public Action<object>? OnInvoke { get; set; }

        /// <inheritdoc/>
        protected override void Invoke(object parameter)
        {
            OnInvoke?.Invoke(parameter);
        }
    }
}
