using System;
using Avalonia;
using Avalonia.Controls;
using DynamicData;
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
        public void AvaloniaObjectShouldSupportWhenAnyValue()
        {
            var avaloniaObjectFixture = new AvaloniaObjectFixture();
            avaloniaObjectFixture.WhenAnyValue(x => x.Text)
                .ToObservableChangeSet()
                .Bind(out var changes)
                .Subscribe();
            Assert.Equal(1, changes.Count);

            avaloniaObjectFixture.Text = "Foo";
            Assert.Equal(2, changes.Count);

            avaloniaObjectFixture.Text = "Bar";
            Assert.Equal(3, changes.Count);
        }

        [Fact]
        public void AvaloniaDerivedObjectShouldSupportWhenAnyValue()
        {
            var avaloniaObjectFixture = new DerivedAvaloniaObjectFixture();
            avaloniaObjectFixture.WhenAnyValue(x => x.Text)
                .ToObservableChangeSet()
                .Bind(out var changes)
                .Subscribe();
            Assert.Equal(1, changes.Count);

            avaloniaObjectFixture.Text = "Foo";
            Assert.Equal(2, changes.Count);

            avaloniaObjectFixture.Text = "Bar";
            Assert.Equal(3, changes.Count);
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
