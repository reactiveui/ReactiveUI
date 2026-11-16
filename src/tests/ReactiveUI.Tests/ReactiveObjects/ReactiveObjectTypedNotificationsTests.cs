// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.ReactiveObjects;

/// <summary>Typed observers share notification state with interface-typed binding consumers.</summary>
public class ReactiveObjectTypedNotificationsTests
{
    /// <summary>Both observation directions retain their sender and property after interface-first initialization.</summary>
    /// <param name="interfaceFirst">Whether the interface initializes the shared state.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task TypedObservers_MixedSenderTypes_ReceiveChangingAndChanged(bool interfaceFirst)
    {
        var fixture = new TestFixture();
        if (interfaceFirst)
        {
            _ = ((IReactiveObject)fixture).GetChangedObservable();
        }
        else
        {
            _ = fixture.GetChangedObservable();
        }

        var changing = new List<IReactivePropertyChangedEventArgs<TestFixture>>();
        var changed = new List<IReactivePropertyChangedEventArgs<TestFixture>>();
        var interfaceChanges = new List<IReactivePropertyChangedEventArgs<IReactiveObject>>();
        using var changingSubscription = fixture.GetChangingObservable().Subscribe(changing.Add);
        using var changedSubscription = fixture.GetChangedObservable().Subscribe(changed.Add);
        using var interfaceSubscription = ((IReactiveObject)fixture).GetChangedObservable().Subscribe(interfaceChanges.Add);

        fixture.IsOnlyOneWord = nameof(fixture);

        await Assert.That(changing).Count().IsEqualTo(1);
        await Assert.That(changed).Count().IsEqualTo(1);
        await Assert.That(interfaceChanges).Count().IsEqualTo(1);
        await Assert.That(changing[0].Sender).IsSameReferenceAs(fixture);
        await Assert.That(changed[0].Sender).IsSameReferenceAs(fixture);
        await Assert.That(changing[0].PropertyName).IsEqualTo(nameof(TestFixture.IsOnlyOneWord));
        await Assert.That(changed[0].PropertyName).IsEqualTo(nameof(TestFixture.IsOnlyOneWord));
    }

    /// <summary>Explicitly null property names cannot emit notifications.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RaiseNotifications_NullPropertyName_DoesNotEmit()
    {
        var fixture = new TestFixture();
        var notifications = new List<string?>();
        using var changing = fixture.GetChangingObservable().Subscribe(args => notifications.Add(args.PropertyName));
        using var changed = fixture.GetChangedObservable().Subscribe(args => notifications.Add(args.PropertyName));

        fixture.RaisePropertyChanging(null);
        fixture.RaisePropertyChanged(null);

        await Assert.That(notifications).IsEmpty();
    }

    /// <summary>Retyping notifications forwards their terminal signal without replacing it.</summary>
    /// <param name="fault">Whether the source fails rather than completing.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task TypedObservable_TerminalSignal_ReachesObserver(bool fault)
    {
        using var source = new Signal<IReactivePropertyChangedEventArgs<IReactiveObject>>();
        var observable = new IReactiveObjectExtensions.ChangeArgsCastObservable<TestFixture>(source);
        var expected = new InvalidOperationException(nameof(fault));
        Exception? received = null;
        var completed = false;
        using var subscription = observable.Subscribe(static _ => { }, error => received = error, () => completed = true);

        if (fault)
        {
            source.OnError(expected);
        }
        else
        {
            source.OnCompleted();
        }

        await Assert.That(received).IsSameReferenceAs(fault ? expected : null);
        await Assert.That(completed).IsEqualTo(!fault);
    }
}
