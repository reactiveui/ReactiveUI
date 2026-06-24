// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ReactiveUI.Tests.Resolvers;

/// <summary>Tests for <see cref="INPCObservableForProperty"/>.</summary>
public class InpcObservableForPropertyTests
{
    /// <summary>The expected exception message when the property name is null.</summary>
    private const string PropertyNameNullMessage = "propertyName should not be null";

    /// <summary>Verifies that the affinity values returned for changed and changing types are correct.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task CheckGetAffinityForObjectValues()
    {
        var instance = new INPCObservableForProperty();

        using (Assert.Multiple())
        {
            await Assert.That(instance.GetAffinityForObject(typeof(TestClassChanged), string.Empty, false)).IsEqualTo(BindingAffinity.Explicit);
            await Assert.That(instance.GetAffinityForObject(typeof(TestClassChanged), string.Empty, true)).IsEqualTo(0);
            await Assert.That(instance.GetAffinityForObject(typeof(object), string.Empty, false)).IsEqualTo(0);

            await Assert.That(instance.GetAffinityForObject(typeof(TestClassChanging), string.Empty, true)).IsEqualTo(BindingAffinity.Explicit);
            await Assert.That(instance.GetAffinityForObject(typeof(TestClassChanging), string.Empty, false)).IsEqualTo(0);
        }

        await Assert.That(instance.GetAffinityForObject(typeof(object), string.Empty, false)).IsEqualTo(0);
    }

    /// <summary>Verifies that property changed notifications are raised for an individual property.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NotificationOnPropertyChanged()
    {
        var instance = new INPCObservableForProperty();

        var testClass = new TestClassChanged();

        Expression<Func<TestClassChanged, string?>> expr = x => x.Property1;
        var exp = Reflection.Rewrite(expr.Body);

        var changes = new List<IObservedChange<object?, object?>>();

        var propertyName = exp.GetMemberInfo()?.Name ??
                           throw new InvalidOperationException(PropertyNameNullMessage);
        _ = ObservableMixins.WhereNotNull(instance.GetNotificationForProperty(testClass, exp, propertyName)).Subscribe(changes.Add);

        const int ExpectedChangeCount = 2;
        testClass.Property1 = "test1";
        testClass.Property1 = "test2";

        await Assert.That(changes).Count().IsEqualTo(ExpectedChangeCount);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    /// <summary>Verifies that property changing notifications are raised for an individual property.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NotificationOnPropertyChanging()
    {
        var instance = new INPCObservableForProperty();

        var testClass = new TestClassChanging();

        Expression<Func<TestClassChanged, string?>> expr = x => x.Property1;
        var exp = Reflection.Rewrite(expr.Body);

        var changes = new List<IObservedChange<object?, object?>>();

        var propertyName = exp.GetMemberInfo()?.Name ??
                           throw new InvalidOperationException(PropertyNameNullMessage);
        _ = ObservableMixins.WhereNotNull(instance.GetNotificationForProperty(testClass, exp, propertyName, true)).Subscribe(changes.Add);

        const int ExpectedChangeCount = 2;
        testClass.Property1 = "test1";
        testClass.Property1 = "test2";

        await Assert.That(changes).Count().IsEqualTo(ExpectedChangeCount);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    /// <summary>Verifies that notifications are raised when the whole object signals a change.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NotificationOnWholeObjectChanged()
    {
        var instance = new INPCObservableForProperty();

        var testClass = new TestClassChanged();

        Expression<Func<TestClassChanged, string?>> expr = x => x.Property1;
        var exp = Reflection.Rewrite(expr.Body);

        var changes = new List<IObservedChange<object?, object?>>();

        var propertyName = exp.GetMemberInfo()?.Name ??
                           throw new InvalidOperationException(PropertyNameNullMessage);
        _ = ObservableMixins.WhereNotNull(instance.GetNotificationForProperty(testClass, exp, propertyName)).Subscribe(changes.Add);

        const int ExpectedChangeCount = 2;

        // Raise genuine whole-object notifications (null/empty name). A bare OnPropertyChanged() would capture the
        // caller member name via [CallerMemberName] (the test method), which is not a whole-object change.
        testClass.RaiseChanged(null);
        testClass.RaiseChanged(string.Empty);

        await Assert.That(changes).Count().IsEqualTo(ExpectedChangeCount);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    /// <summary>Verifies that notifications are raised when the whole object signals a changing event.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NotificationOnWholeObjectChanging()
    {
        var instance = new INPCObservableForProperty();

        var testClass = new TestClassChanging();

        Expression<Func<TestClassChanged, string?>> expr = x => x.Property1;
        var exp = Reflection.Rewrite(expr.Body);

        var changes = new List<IObservedChange<object?, object?>>();

        var propertyName = exp.GetMemberInfo()?.Name ??
                           throw new InvalidOperationException(PropertyNameNullMessage);
        _ = ObservableMixins.WhereNotNull(instance.GetNotificationForProperty(testClass, exp, propertyName, true)).Subscribe(changes.Add);

        const int ExpectedChangeCount = 2;

        // Raise genuine whole-object notifications (null/empty name). A bare OnPropertyChanging() would capture the
        // caller member name via [CallerMemberName] (the test method), which is not a whole-object change.
        testClass.RaiseChanging(null);
        testClass.RaiseChanging(string.Empty);

        await Assert.That(changes).Count().IsEqualTo(ExpectedChangeCount);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    /// <summary>The two-argument affinity overload defers to changed notifications, and a null type has no affinity.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjectTwoArgOverloadAndNullType()
    {
        var instance = new INPCObservableForProperty();

        using (Assert.Multiple())
        {
            await Assert.That(instance.GetAffinityForObject(typeof(TestClassChanged), "Property1")).IsEqualTo(BindingAffinity.Explicit);
            await Assert.That(instance.GetAffinityForObject(null, "Property1", false)).IsEqualTo(0);
        }
    }

    /// <summary>A changing notification ignores other properties' events and unsubscribes on dispose.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ChangingNotificationIgnoresOtherPropertiesAndDisposes()
    {
        var instance = new INPCObservableForProperty();
        var testClass = new TestClassChanging();

        Expression<Func<TestClassChanging, string?>> expr = x => x.Property1;
        var exp = Reflection.Rewrite(expr.Body);
        var propertyName = exp.GetMemberInfo()?.Name ??
                           throw new InvalidOperationException(PropertyNameNullMessage);

        var changes = new List<IObservedChange<object?, object?>>();
        var subscription = instance.GetNotificationForProperty(testClass, exp, propertyName, true).Subscribe(changes.Add);

        // A non-matching property's changing event is filtered out.
        testClass.Property2 = "ignored";
        await Assert.That(changes).IsEmpty();

        // The observed property's changing event is forwarded.
        testClass.Property1 = "matched";
        await Assert.That(changes).IsNotEmpty();

        subscription.Dispose();
    }

    /// <summary>A test fixture implementing <see cref="INotifyPropertyChanged"/> to drive change notifications.</summary>
    private sealed class TestClassChanged : INotifyPropertyChanged
    {
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Gets or sets the first test property.</summary>
        public string? Property1
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the second test property.</summary>
        public string? Property2
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Raises the <see cref="PropertyChanged"/> event.</summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new(propertyName));

        /// <summary>Raises <see cref="PropertyChanged"/> with an explicit name (null/empty means whole-object).</summary>
        /// <param name="propertyName">The property name, or null/empty for a whole-object change.</param>
        public void RaiseChanged(string? propertyName) =>
            PropertyChanged?.Invoke(this, new(propertyName));
    }

    /// <summary>A test fixture implementing <see cref="INotifyPropertyChanging"/> to drive changing notifications.</summary>
    private sealed class TestClassChanging : INotifyPropertyChanging
    {
        /// <summary>Occurs when a property value is changing.</summary>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <summary>Gets or sets the first test property.</summary>
        public string? Property1
        {
            get;
            set
            {
                field = value;
                OnPropertyChanging();
            }
        }

        /// <summary>Gets or sets the second test property.</summary>
        public string? Property2
        {
            get;
            set
            {
                field = value;
                OnPropertyChanging();
            }
        }

        /// <summary>Raises the <see cref="PropertyChanging"/> event.</summary>
        /// <param name="propertyName">The name of the property that is changing.</param>
        public void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            var handler = PropertyChanging;
            handler?.Invoke(this, new(propertyName));
        }

        /// <summary>Raises <see cref="PropertyChanging"/> with an explicit name (null/empty means whole-object).</summary>
        /// <param name="propertyName">The property name, or null/empty for a whole-object change.</param>
        public void RaiseChanging(string? propertyName) =>
            PropertyChanging?.Invoke(this, new(propertyName));
    }
}
