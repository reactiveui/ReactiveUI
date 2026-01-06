// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.Reactive.Testing;
using Microsoft.Xaml.Behaviors;
using ReactiveUI.Blend;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for ObservableTrigger.
/// </summary>
[NotInParallel]
public class ObservableTriggerTests
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
    /// Tests that the trigger invokes actions when the observable emits.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Observable_WhenEmits_InvokesActions()
    {
        var button = new Button();
        var trigger = new ObservableTrigger
        {
            SchedulerOverride = ImmediateScheduler.Instance
        };
        var subject = new Subject<object>();
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

        await Task.Delay(100);

        await Assert.That(actionInvoked).IsTrue();
    }

    /// <summary>
    /// Tests that OnObservableChanged throws ArgumentException for invalid sender.
    /// </summary>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void OnObservableChanged_InvalidSender_ThrowsArgumentException()
    {
        var button = new Button();
        var subject = new Subject<object>();
        var eventArgs = new DependencyPropertyChangedEventArgs(
            ObservableTrigger.ObservableProperty,
            null,
            subject.AsObservable());

        Assert.Throws<ArgumentException>(() =>
            ObservableTrigger.InternalOnObservableChangedForTesting(button, eventArgs));
    }

    /// <summary>
    /// Tests that OnObservableChanged throws ArgumentNullException for default event args.
    /// </summary>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void OnObservableChanged_DefaultEventArgs_ThrowsArgumentNullException()
    {
        var trigger = new ObservableTrigger();
        var button = new Button();
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        Assert.Throws<ArgumentNullException>(() =>
            ObservableTrigger.InternalOnObservableChangedForTesting(trigger, default));
    }

    /// <summary>
    /// Tests that OnObservableChanged throws ArgumentNullException for null NewValue.
    /// </summary>
    [Test]
    [TestExecutor<STAThreadExecutor>]
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

        Assert.Throws<ArgumentNullException>(() =>
            ObservableTrigger.InternalOnObservableChangedForTesting(trigger, eventArgs));
    }

    /// <summary>
    /// Tests that Observable getter returns the set value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Observable_Getter_ReturnsSetValue()
    {
        var button = new Button();
        var trigger = new ObservableTrigger
        {
            SchedulerOverride = ImmediateScheduler.Instance
        };
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        var subject = new Subject<object>();
        trigger.Observable = subject.AsObservable();

        // Read the getter to cover that line
        var observable = trigger.Observable;

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Tests that setting a new observable disposes the previous subscription.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Observable_WhenChangedMultipleTimes_DisposesOldSubscription()
    {
        var button = new Button();
        var trigger = new ObservableTrigger
        {
            SchedulerOverride = ImmediateScheduler.Instance
        };
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        var disposed1 = false;
        var observable1 = Observable.Create<object>(observer => Disposable.Create(() => disposed1 = true));

        var observable2 = Observable.Never<object>();

        // Set first observable
        trigger.Observable = observable1;
        await Task.Delay(50);

        // Set second observable (should dispose first)
        trigger.Observable = observable2;
        await Task.Delay(50);

        await Assert.That(disposed1).IsTrue();
    }

    /// <summary>
    /// Tests that AutoResubscribeOnError resubscribes after an error.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task AutoResubscribeOnError_WhenTrue_ResubscribesAfterError()
    {
        var button = new Button();
        var scheduler = new TestScheduler();
        var trigger = new ObservableTrigger
        {
            AutoResubscribeOnError = true,
            SchedulerOverride = scheduler
        };
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        var errorCount = 0;
        trigger.Observable = Observable.Create<object>(observer =>
        {
            errorCount++;
            if (errorCount <= 3)
            {
                observer.OnError(new InvalidOperationException("Test error"));
            }
            else
            {
                observer.OnCompleted();
            }

            return Disposable.Empty;
        });

        scheduler.Start();

        // Should have resubscribed 3 times (errored 3 times, then completed on 4th)
        await Assert.That(errorCount).IsEqualTo(4);
    }

    /// <summary>
    /// Tests that AutoResubscribeOnError false does not resubscribe after error.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task AutoResubscribeOnError_WhenFalse_DoesNotResubscribe()
    {
        var button = new Button();
        var scheduler = new TestScheduler();
        var trigger = new ObservableTrigger
        {
            AutoResubscribeOnError = false,
            SchedulerOverride = scheduler
        };
        var triggers = Interaction.GetTriggers(button);
        triggers.Add(trigger);

        var errorCount = 0;
        trigger.Observable = Observable.Create<object>(observer =>
        {
            errorCount++;
            observer.OnError(new InvalidOperationException("Test error"));
            return Disposable.Empty;
        });

        scheduler.Start();

        // Should only subscribe once
        await Assert.That(errorCount).IsEqualTo(1);
    }

    /// <summary>
    /// Tests that multiple emissions invoke actions multiple times.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Observable_MultipleEmissions_InvokesActionsMultipleTimes()
    {
        var button = new Button();
        var trigger = new ObservableTrigger
        {
            SchedulerOverride = ImmediateScheduler.Instance
        };
        var subject = new Subject<object>();
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

        await Task.Delay(150);

        await Assert.That(invokeCount).IsEqualTo(3);
    }

    /// <summary>
    /// Test action that can be customized.
    /// </summary>
    private class TestAction : TriggerAction<DependencyObject>
    {
        /// <summary>
        /// Gets or sets the action to invoke.
        /// </summary>
        public Action<object>? OnInvoke { get; set; }

        /// <inheritdoc/>
        protected override void Invoke(object parameter)
        {
            OnInvoke?.Invoke(parameter);
        }
    }
}
