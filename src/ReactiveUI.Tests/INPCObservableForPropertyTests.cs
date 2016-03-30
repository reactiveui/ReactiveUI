using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
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
            instance.GetNotificationForProperty(testClass, exp, false).Subscribe(c => changes.Add(c));

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
            instance.GetNotificationForProperty(testClass, exp, true).Subscribe(c => changes.Add(c));

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
            instance.GetNotificationForProperty(testClass, exp, false).Subscribe(c => changes.Add(c));

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
            instance.GetNotificationForProperty(testClass, exp, true).Subscribe(c => changes.Add(c));

            testClass.OnPropertyChanging(null);
            testClass.OnPropertyChanging(string.Empty);

            Assert.Equal(2, changes.Count);

            Assert.Equal(testClass, changes[0].Sender);
            Assert.Equal(testClass, changes[1].Sender);
        }

        class TestClassChanged : INotifyPropertyChanged
        {
            string property;

            string property2;

            public string Property1
            {
                get { return property; }
                set
                {
                    property = value;
                    OnPropertyChanged();
                }
            }

            public string Property2
            {
                get { return property2; }
                set
                {
                    property2 = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                var handler = PropertyChanged;
                if (handler != null) {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        class TestClassChanging : INotifyPropertyChanging
        {
            string property1;

            string property2;

            public string Property1
            {
                get { return property1; }
                set
                {
                    property1 = value;
                    OnPropertyChanging();
                }
            }

            public string Property2
            {
                get { return property2; }
                set
                {
                    property2 = value;
                    OnPropertyChanging();
                }
            }

            public event PropertyChangingEventHandler PropertyChanging;

            public void OnPropertyChanging([CallerMemberName] string propertyName = null)
            {
                var handler = PropertyChanging;
                if (handler != null) {
                    handler(this, new PropertyChangingEventArgs(propertyName));
                }
            }
        }
    }
}
