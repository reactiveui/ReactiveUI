// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI.Tests;

public class INPCObservableForPropertyTests
{
    [Test]
    public async Task CheckGetAffinityForObjectValues()
    {
        var instance = new INPCObservableForProperty();

        using (Assert.Multiple())
        {
            await Assert.That(instance.GetAffinityForObject(typeof(TestClassChanged), string.Empty, false)).IsEqualTo(5);
            await Assert.That(instance.GetAffinityForObject(typeof(TestClassChanged), string.Empty, true)).IsEqualTo(0);
            await Assert.That(instance.GetAffinityForObject(typeof(object), string.Empty, false)).IsEqualTo(0);

            await Assert.That(instance.GetAffinityForObject(typeof(TestClassChanging), string.Empty, true)).IsEqualTo(5);
            await Assert.That(instance.GetAffinityForObject(typeof(TestClassChanging), string.Empty, false)).IsEqualTo(0);
        }

        await Assert.That(instance.GetAffinityForObject(typeof(object), string.Empty, false)).IsEqualTo(0);
    }

    [Test]
    public async Task NotificationOnPropertyChanged()
    {
        var instance = new INPCObservableForProperty();

        var testClass = new TestClassChanged();

        Expression<Func<TestClassChanged, string?>> expr = x => x!.Property1;
        var exp = Reflection.Rewrite(expr.Body);

        var changes = new List<IObservedChange<object?, object?>>();

        var propertyName = exp.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null");
        instance.GetNotificationForProperty(testClass, exp, propertyName).WhereNotNull().Subscribe(c => changes.Add(c));

        testClass.Property1 = "test1";
        testClass.Property1 = "test2";

        await Assert.That(changes).Count().IsEqualTo(2);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    [Test]
    public async Task NotificationOnPropertyChanging()
    {
        var instance = new INPCObservableForProperty();

        var testClass = new TestClassChanging();

        Expression<Func<TestClassChanged, string?>> expr = x => x.Property1;
        var exp = Reflection.Rewrite(expr.Body);

        var changes = new List<IObservedChange<object?, object?>>();

        var propertyName = exp.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null");
        instance.GetNotificationForProperty(testClass, exp, propertyName, true).WhereNotNull().Subscribe(c => changes.Add(c));

        testClass.Property1 = "test1";
        testClass.Property1 = "test2";

        await Assert.That(changes).Count().IsEqualTo(2);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    [Test]
    public async Task NotificationOnWholeObjectChanged()
    {
        var instance = new INPCObservableForProperty();

        var testClass = new TestClassChanged();

        Expression<Func<TestClassChanged, string?>> expr = x => x.Property1;
        var exp = Reflection.Rewrite(expr.Body);

        var changes = new List<IObservedChange<object?, object?>>();

        var propertyName = exp.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null");
        instance.GetNotificationForProperty(testClass, exp, propertyName, false).WhereNotNull().Subscribe(c => changes.Add(c));

        testClass.OnPropertyChanged(null);
        testClass.OnPropertyChanged(string.Empty);

        await Assert.That(changes).Count().IsEqualTo(2);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    [Test]
    public async Task NotificationOnWholeObjectChanging()
    {
        var instance = new INPCObservableForProperty();

        var testClass = new TestClassChanging();

        Expression<Func<TestClassChanged, string?>> expr = x => x.Property1;
        var exp = Reflection.Rewrite(expr.Body);

        var changes = new List<IObservedChange<object?, object?>>();

        var propertyName = exp.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null");
        instance.GetNotificationForProperty(testClass, exp, propertyName, true).WhereNotNull().Subscribe(c => changes.Add(c));

        testClass.OnPropertyChanging(null);
        testClass.OnPropertyChanging(string.Empty);

        await Assert.That(changes).Count().IsEqualTo(2);

        using (Assert.Multiple())
        {
            await Assert.That(changes[0].Sender).IsEqualTo(testClass);
            await Assert.That(changes[1].Sender).IsEqualTo(testClass);
        }
    }

    private class TestClassChanged : INotifyPropertyChanged
    {
        private string? _property;

        private string? _property2;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Property1
        {
            get => _property;
            set
            {
                _property = value;
                OnPropertyChanged();
            }
        }

        public string? Property2
        {
            get => _property2;
            set
            {
                _property2 = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private class TestClassChanging : INotifyPropertyChanging
    {
        private string? _property1;

        private string? _property2;

        public event PropertyChangingEventHandler? PropertyChanging;

        public string? Property1
        {
            get => _property1;
            set
            {
                _property1 = value;
                OnPropertyChanging();
            }
        }

        public string? Property2
        {
            get => _property2;
            set
            {
                _property2 = value;
                OnPropertyChanging();
            }
        }

        public void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            var handler = PropertyChanging;
            handler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}
