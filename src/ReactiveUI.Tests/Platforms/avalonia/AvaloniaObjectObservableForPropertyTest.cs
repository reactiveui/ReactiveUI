using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI.Avalonia;
using Xunit;

namespace ReactiveUI.Tests.Platforms.Avalonia
{
    public class AvaloniaObjectObservableForPropertyTest
    {
        public class AvaloniaObjectFixture : AvaloniaObject
        {
            public static readonly AvaloniaProperty<string> TextProperty = AvaloniaProperty
                .Register<AvaloniaObjectFixture, string>(nameof(Text), inherits: true);

            public string Text
            {
                get { return GetValue(TextProperty); }
                set { SetValue(TextProperty, value); }
            }
        }

        public class DerivedAvaloniaObjectFixture : AvaloniaObjectFixture
        {
            public static readonly AvaloniaProperty<string> AnotherTextProperty = AvaloniaProperty
                .Register<AvaloniaObjectFixture, string>(nameof(AnotherText), inherits: true);

            public string AnotherText
            {
                get { return GetValue(AnotherTextProperty); }
                set { SetValue(AnotherTextProperty, value); }
            }
        }

        [Fact]
        public void GetAffinityForObjectShouldReturnZeroOnlyIfPropertyDoesntExist()
        {
            var createsObservable = new AvaloniaObjectObservableForProperty();
            var objectType = typeof(AvaloniaObjectFixture);

            Assert.NotEqual(0, createsObservable.GetAffinityForObject(objectType, "Text"));
            Assert.Equal(0, createsObservable.GetAffinityForObject(objectType, "DoesntExist"));
        }

        [Fact]
        public void ObservableForPropertyShouldSendNotificationsWhenPropertyChanges()
        {
            var avaloniaObjectFixture = new AvaloniaObjectFixture();
            var propertyName = nameof(avaloniaObjectFixture.Text);
            Expression<Func<AvaloniaObjectFixture, string>> expression = x => x.Text;

            var notifications = new List<IObservedChange<object, object>>();
            var subscription = new AvaloniaObjectObservableForProperty()
                .GetNotificationForProperty(avaloniaObjectFixture, expression, propertyName)
                .Subscribe(notifications.Add);

            avaloniaObjectFixture.Text = "Hello!";
            Assert.Equal(1, notifications.Count);

            avaloniaObjectFixture.Text = "World!";
            Assert.Equal(2, notifications.Count);
            subscription.Dispose();
        }

        [Fact]
        public void GetAffinityForDerivedObjectShouldReturnZeroOnlyIfPropertyDoesntExist()
        {
            var createsObservable = new AvaloniaObjectObservableForProperty();
            var objectType = typeof(DerivedAvaloniaObjectFixture);

            Assert.NotEqual(0, createsObservable.GetAffinityForObject(objectType, "Text"));
            Assert.Equal(0, createsObservable.GetAffinityForObject(objectType, "DoesntExist"));
        }

        [Fact]
        public void ObservableForPropertyShouldSendNotificationsWhenDerivedPropertyChanges()
        {
            var avaloniaObjectFixture = new DerivedAvaloniaObjectFixture();
            var propertyName = nameof(avaloniaObjectFixture.Text);
            Expression<Func<DerivedAvaloniaObjectFixture, string>> expression = x => x.Text;

            var notifications = new List<IObservedChange<object, object>>();
            var subscription = new AvaloniaObjectObservableForProperty()
                .GetNotificationForProperty(avaloniaObjectFixture, expression, propertyName)
                .Subscribe(notifications.Add);

            avaloniaObjectFixture.Text = "Hello!";
            Assert.Equal(1, notifications.Count);

            avaloniaObjectFixture.Text = "World!";
            Assert.Equal(2, notifications.Count);
            subscription.Dispose();
        }
        
        [Fact]
        public void AvaloniaPropertyShouldSupportWhenAnyValue()
        {
            var inputs = new[] { "Foo", "Bar", "Baz" };
            var avaloniaObjectFixture = new DerivedAvaloniaObjectFixture();

            avaloniaObjectFixture
                .WhenAnyValue(x => x.Text)
                .ToObservableChangeSet()
                .Bind(out var propertyValues)
                .Subscribe();

            inputs.ForEach(x => avaloniaObjectFixture.Text = x);
            var allPropertiesEqual = inputs
                .Zip(propertyValues.Skip(1),
                    (expected, actual) => expected == actual)
                .All(x => x);

            Assert.Null(propertyValues.First());
            Assert.Equal(4, propertyValues.Count);
            Assert.True(allPropertiesEqual);
        }

        [Fact]
        public void AvaloniaObjectObservableForPropertyShouldSupportAvaloniaControls()
        {
            var dropDown = new DropDown
            {
                Items = new[] { "Foo", "Bar", "Baz" }
            };

            dropDown.WhenAnyValue(x => x.SelectedItem)
                .ToObservableChangeSet()
                .Bind(out var selectionChanges)
                .Subscribe();
            Assert.Equal(1, selectionChanges.Count);

            dropDown.SelectedIndex = 1;
            Assert.Equal(2, selectionChanges.Count);

            dropDown.SelectedIndex = 2;
            Assert.Equal(3, selectionChanges.Count);
        }
    }
}
