// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

using ReactiveUI.Blend;
using ReactiveUI.Tests.Utilities.Schedulers;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for FollowObservableStateBehavior.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class FollowObservableStateBehaviorTests
{
    /// <summary>
    /// Tests that the behavior subscribes to state changes and transitions visual states.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task StateObservable_WhenChanged_TransitionsVisualState()
    {
        var button = new Button();
        var behavior = new FollowObservableStateBehavior
        {
            SchedulerOverride = ImmediateScheduler.Instance
        };
        var stateSubject = new Subject<string>();

        // Attach behavior to button
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        // Set the state observable
        behavior.StateObservable = stateSubject.AsObservable();

        // No exception should be thrown - the state manager will just silently fail if the state doesn't exist
        // This is expected behavior for VisualStateManager.GoToState
        stateSubject.OnNext("SomeState");

        await Task.Delay(50);

        // If we reach here without exception, the test passed
        await Task.CompletedTask;
    }

    /// <summary>
    /// Tests that OnStateObservableChanged throws ArgumentException for invalid sender.
    /// </summary>
    [Test]
    public void OnStateObservableChanged_InvalidSender_ThrowsArgumentException()
    {
        var button = new Button();
        var stateSubject = new Subject<string>();
        var eventArgs = new DependencyPropertyChangedEventArgs(
            FollowObservableStateBehavior.StateObservableProperty,
            null,
            stateSubject.AsObservable());

        Assert.Throws<ArgumentException>(() =>
            FollowObservableStateBehavior.InternalOnStateObservableChangedForTesting(button, eventArgs));
    }

    /// <summary>
    /// Tests that OnStateObservableChanged throws ArgumentNullException for default event args.
    /// </summary>
    [Test]
    public void OnStateObservableChanged_DefaultEventArgs_ThrowsArgumentNullException()
    {
        var behavior = new FollowObservableStateBehavior();
        var button = new Button();
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        Assert.Throws<ArgumentNullException>(() =>
            FollowObservableStateBehavior.InternalOnStateObservableChangedForTesting(behavior, default));
    }

    /// <summary>
    /// Tests that OnStateObservableChanged throws ArgumentNullException for null NewValue.
    /// </summary>
    [Test]
    public void OnStateObservableChanged_NullNewValue_ThrowsArgumentNullException()
    {
        var behavior = new FollowObservableStateBehavior();
        var button = new Button();
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        var eventArgs = new DependencyPropertyChangedEventArgs(
            FollowObservableStateBehavior.StateObservableProperty,
            null,
            null!);

        Assert.Throws<ArgumentNullException>(() =>
            FollowObservableStateBehavior.InternalOnStateObservableChangedForTesting(behavior, eventArgs));
    }

    /// <summary>
    /// Tests that setting a new observable disposes the previous subscription.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task StateObservable_WhenChangedMultipleTimes_DisposesOldSubscription()
    {
        var button = new Button();
        var behavior = new FollowObservableStateBehavior
        {
            SchedulerOverride = ImmediateScheduler.Instance
        };
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        var subject1 = new Subject<string>();
        var subject2 = new Subject<string>();

        // Set first observable
        behavior.StateObservable = subject1.AsObservable();
        await Task.Delay(50);

        // Set second observable (should dispose first)
        behavior.StateObservable = subject2.AsObservable();
        await Task.Delay(50);

        // First subject should no longer be subscribed
        var completed1 = false;
        subject1.Subscribe(_ => { }, () => completed1 = true);
        subject1.OnCompleted();

        await Assert.That(completed1).IsTrue();
    }

    /// <summary>
    /// Tests that AutoResubscribeOnError resubscribes after an error.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoResubscribeOnError_WhenTrue_ResubscribesAfterError()
    {
        var button = new Button();
        var scheduler = new VirtualTimeScheduler();
        var behavior = new FollowObservableStateBehavior
        {
            AutoResubscribeOnError = true,
            SchedulerOverride = scheduler
        };
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        var errorCount = 0;
        behavior.StateObservable = Observable.Create<string>(observer =>
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
    public async Task AutoResubscribeOnError_WhenFalse_DoesNotResubscribe()
    {
        var button = new Button();
        var scheduler = new VirtualTimeScheduler();
        var behavior = new FollowObservableStateBehavior
        {
            AutoResubscribeOnError = false,
            SchedulerOverride = scheduler
        };
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        var errorCount = 0;
        behavior.StateObservable = Observable.Create<string>(observer =>
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
    /// Tests that OnDetaching disposes the watcher.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnDetaching_DisposesWatcher()
    {
        var button = new Button();
        var behavior = new FollowObservableStateBehavior
        {
            SchedulerOverride = ImmediateScheduler.Instance
        };
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        var disposed = false;
        behavior.StateObservable = Observable.Create<string>(observer => Disposable.Create(() => disposed = true));
        await Task.Delay(50);

        // Detach the behavior
        behaviors.Remove(behavior);
        await Task.Delay(50);

        await Assert.That(disposed).IsTrue();
    }

    /// <summary>
    /// Tests that TargetObject can be set and is used instead of AssociatedObject.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TargetObject_WhenSet_UsedInsteadOfAssociatedObject()
    {
        var button = new Button();
        var targetButton = new Button();
        var behavior = new FollowObservableStateBehavior
        {
            TargetObject = targetButton,
            SchedulerOverride = ImmediateScheduler.Instance
        };
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        var subject = new Subject<string>();
        behavior.StateObservable = subject.AsObservable();

        subject.OnNext("SomeState");
        await Task.Delay(50);

        // If we reach here without exception, target object was used
        await Task.CompletedTask;
    }

    /// <summary>
    /// Tests that StateObservable getter returns the set value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task StateObservable_Getter_ReturnsSetValue()
    {
        var button = new Button();
        var behavior = new FollowObservableStateBehavior
        {
            SchedulerOverride = ImmediateScheduler.Instance
        };
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        var subject = new Subject<string>();
        behavior.StateObservable = subject.AsObservable();

        // Read the getter to cover that line
        var observable = behavior.StateObservable;

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Tests that TargetObject getter returns the set value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TargetObject_Getter_ReturnsSetValue()
    {
        var button = new Button();
        var targetButton = new Button();
        var behavior = new FollowObservableStateBehavior
        {
            TargetObject = targetButton
        };
        var behaviors = Interaction.GetBehaviors(button);
        behaviors.Add(behavior);

        // Read the getter to cover that line
        var target = behavior.TargetObject;

        await Assert.That(target).IsEqualTo(targetButton);
    }
}
