// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI.Tests;

[TestFixture]
public class INPCObservableForPropertyTests
{
    [Test]
    public void CheckGetAffinityForObjectValues()
    {
        var instance = new INPCObservableForProperty();

        Assert.That(instance.GetAffinityForObject(typeof(TestClassChanged, Is.EqualTo(5)), string.Empty, false));
        Assert.That(instance.GetAffinityForObject(typeof(TestClassChanged, Is.EqualTo(0)), string.Empty, true));
        Assert.That(instance.GetAffinityForObject(typeof(object, Is.EqualTo(0)), string.Empty, false));

        Assert.That(instance.GetAffinityForObject(typeof(TestClassChanging, Is.EqualTo(5)), string.Empty, true));
        Assert.That(instance.GetAffinityForObject(typeof(TestClassChanging, Is.EqualTo(0)), string.Empty, false));
        Assert.That(instance.GetAffinityForObject(typeof(object, Is.EqualTo(0)), string.Empty, false));
    }

    [Test]
    public void NotificationOnPropertyChanged()
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

        Assert.That(changes.Count, Is.EqualTo(2));

        Assert.That(changes[0].Sender, Is.EqualTo(testClass));
        Assert.That(changes[1].Sender, Is.EqualTo(testClass));
    }

    [Test]
    public void NotificationOnPropertyChanging()
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

        Assert.That(changes.Count, Is.EqualTo(2));

        Assert.That(changes[0].Sender, Is.EqualTo(testClass));
        Assert.That(changes[1].Sender, Is.EqualTo(testClass));
    }

    [Test]
    public void NotificationOnWholeObjectChanged()
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

        Assert.That(changes.Count, Is.EqualTo(2));

        Assert.That(changes[0].Sender, Is.EqualTo(testClass));
        Assert.That(changes[1].Sender, Is.EqualTo(testClass));
    }

    [Test]
    public void NotificationOnWholeObjectChanging()
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

        Assert.That(changes.Count, Is.EqualTo(2));

        Assert.That(changes[0].Sender, Is.EqualTo(testClass));
        Assert.That(changes[1].Sender, Is.EqualTo(testClass));
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
