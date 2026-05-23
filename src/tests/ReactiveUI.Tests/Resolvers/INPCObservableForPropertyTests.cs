// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI.Tests.Resolvers;

/// <summary>
/// Tests for <see cref="INPCObservableForProperty"/>.
/// </summary>
public class InpcObservableForPropertyTests
{
    private const string PropertyNameNullMessage = "propertyName should not be null";

    /// <summary>
    /// Verifies that the affinity values returned for changed and changing types are correct.
    /// </summary>
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

    /// <summary>
    /// Verifies that property changed notifications are raised for an individual property.
    /// </summary>
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
        instance.GetNotificationForProperty(testClass, exp, propertyName).WhereNotNull().Subscribe(changes.Add);

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

    /// <summary>
    /// Verifies that property changing notifications are raised for an individual property.
    /// </summary>
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
        instance.GetNotificationForProperty(testClass, exp, propertyName, true).WhereNotNull().Subscribe(changes.Add);

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

    /// <summary>
    /// Verifies that notifications are raised when the whole object signals a change.
    /// </summary>
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
        instance.GetNotificationForProperty(testClass, exp, propertyName).WhereNotNull().Subscribe(changes.Add);

        const int ExpectedChangeCount = 2;
        testClass.OnPropertyChanged();
        testClass.OnPropertyChanged(string.Empty);

        await Assert.That(changes).Count().IsEqualTo(ExpectedChangeCount);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    /// <summary>
    /// Verifies that notifications are raised when the whole object signals a changing event.
    /// </summary>
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
        instance.GetNotificationForProperty(testClass, exp, propertyName, true).WhereNotNull().Subscribe(changes.Add);

        const int ExpectedChangeCount = 2;
        testClass.OnPropertyChanging();
        testClass.OnPropertyChanging(string.Empty);

        await Assert.That(changes).Count().IsEqualTo(ExpectedChangeCount);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    /// <summary>
    /// A test fixture implementing <see cref="INotifyPropertyChanged"/> to drive change notifications.
    /// </summary>
    private sealed class TestClassChanged : INotifyPropertyChanged
    {
        private string? _property;

        private string? _property2;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the first test property.
        /// </summary>
        public string? Property1
        {
            get => _property;
            set
            {
                _property = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the second test property.
        /// </summary>
        public string? Property2
        {
            get => _property2;
            set
            {
                _property2 = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new(propertyName));
    }

    /// <summary>
    /// A test fixture implementing <see cref="INotifyPropertyChanging"/> to drive changing notifications.
    /// </summary>
    private sealed class TestClassChanging : INotifyPropertyChanging
    {
        private string? _property1;

        private string? _property2;

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <summary>
        /// Gets or sets the first test property.
        /// </summary>
        public string? Property1
        {
            get => _property1;
            set
            {
                _property1 = value;
                OnPropertyChanging();
            }
        }

        /// <summary>
        /// Gets or sets the second test property.
        /// </summary>
        public string? Property2
        {
            get => _property2;
            set
            {
                _property2 = value;
                OnPropertyChanging();
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that is changing.</param>
        public void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            var handler = PropertyChanging;
            handler?.Invoke(this, new(propertyName));
        }
    }
}
