﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Xunit;

namespace ReactiveUI.Tests
{
    public class INPCObservableForPropertyTests
    {
        [Fact]
        public void CheckGetAffinityForObjectValues()
        {
            var instance = new INPCObservableForProperty();

            Assert.Equal(5, instance.GetAffinityForObject(typeof(TestClassChanged), null, false));
            Assert.Equal(0, instance.GetAffinityForObject(typeof(TestClassChanged), null, true));
            Assert.Equal(0, instance.GetAffinityForObject(typeof(object), null, false));

            Assert.Equal(5, instance.GetAffinityForObject(typeof(TestClassChanging), null, true));
            Assert.Equal(0, instance.GetAffinityForObject(typeof(TestClassChanging), null, false));
            Assert.Equal(0, instance.GetAffinityForObject(typeof(object), null, false));
        }

        [Fact]
        public void NotificationOnPropertyChanged()
        {
            var instance = new INPCObservableForProperty();

            var testClass = new TestClassChanged();

            Expression<Func<TestClassChanged, string>> expr = x => x.Property1;
            var exp = Reflection.Rewrite(expr.Body);

            var changes = new List<IObservedChange<object, object>>();
            instance.GetNotificationForProperty(testClass, exp, exp.GetMemberInfo().Name, false).Subscribe(c => changes.Add(c));

            testClass.Property1 = "test1";
            testClass.Property1 = "test2";

            Assert.Equal(2, changes.Count);

            Assert.Equal(testClass, changes[0].Sender);
            Assert.Equal(testClass, changes[1].Sender);
        }

        [Fact]
        public void NotificationOnPropertyChanging()
        {
            var instance = new INPCObservableForProperty();

            var testClass = new TestClassChanging();

            Expression<Func<TestClassChanged, string>> expr = x => x.Property1;
            var exp = Reflection.Rewrite(expr.Body);

            var changes = new List<IObservedChange<object, object>>();
            instance.GetNotificationForProperty(testClass, exp, exp.GetMemberInfo().Name, true).Subscribe(c => changes.Add(c));

            testClass.Property1 = "test1";
            testClass.Property1 = "test2";

            Assert.Equal(2, changes.Count);

            Assert.Equal(testClass, changes[0].Sender);
            Assert.Equal(testClass, changes[1].Sender);
        }

        [Fact]
        public void NotificationOnWholeObjectChanged()
        {
            var instance = new INPCObservableForProperty();

            var testClass = new TestClassChanged();

            Expression<Func<TestClassChanged, string>> expr = x => x.Property1;
            var exp = Reflection.Rewrite(expr.Body);

            var changes = new List<IObservedChange<object, object>>();
            instance.GetNotificationForProperty(testClass, exp, exp.GetMemberInfo().Name, false).Subscribe(c => changes.Add(c));

            testClass.OnPropertyChanged(null);
            testClass.OnPropertyChanged(string.Empty);

            Assert.Equal(2, changes.Count);

            Assert.Equal(testClass, changes[0].Sender);
            Assert.Equal(testClass, changes[1].Sender);
        }

        [Fact]
        public void NotificationOnWholeObjectChanging()
        {
            var instance = new INPCObservableForProperty();

            var testClass = new TestClassChanging();

            Expression<Func<TestClassChanged, string>> expr = x => x.Property1;
            var exp = Reflection.Rewrite(expr.Body);

            var changes = new List<IObservedChange<object, object>>();
            instance.GetNotificationForProperty(testClass, exp, exp.GetMemberInfo().Name, true).Subscribe(c => changes.Add(c));

            testClass.OnPropertyChanging(null);
            testClass.OnPropertyChanging(string.Empty);

            Assert.Equal(2, changes.Count);

            Assert.Equal(testClass, changes[0].Sender);
            Assert.Equal(testClass, changes[1].Sender);
        }

        private class TestClassChanged : INotifyPropertyChanged
        {
            private string _property;

            private string _property2;

            public event PropertyChangedEventHandler PropertyChanged;

            public string Property1
            {
                get => _property;
                set
                {
                    _property = value;
                    OnPropertyChanged();
                }
            }

            public string Property2
            {
                get => _property2;
                set
                {
                    _property2 = value;
                    OnPropertyChanged();
                }
            }

            public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private class TestClassChanging : INotifyPropertyChanging
        {
            private string _property1;

            private string _property2;

            public event PropertyChangingEventHandler PropertyChanging;

            public string Property1
            {
                get => _property1;
                set
                {
                    _property1 = value;
                    OnPropertyChanging();
                }
            }

            public string Property2
            {
                get => _property2;
                set
                {
                    _property2 = value;
                    OnPropertyChanging();
                }
            }

            public void OnPropertyChanging([CallerMemberName] string propertyName = null)
            {
                var handler = PropertyChanging;
                handler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }
}
