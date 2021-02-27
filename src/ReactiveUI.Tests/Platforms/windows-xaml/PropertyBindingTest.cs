﻿// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reactive;
using DynamicData.Binding;
using Xunit;

#if NETFX_CORE
#else
using FactAttribute = Xunit.WpfFactAttribute;
#endif

namespace ReactiveUI.Tests.Xaml
{
    /// <summary>
    /// Tests property bindings.
    /// </summary>
    public class PropertyBindingTest
    {
        /// <summary>
        /// Performs a smoke test with two way binding with func converter.
        /// </summary>
        [Fact]
        [UseInvariantCulture]
        public void TwoWayBindWithFuncConvertersSmokeTest()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            var fixture = new PropertyBinderImplementation();

            vm.JustADecimal = 123.45m;
            Assert.NotEqual(vm.JustADecimal.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

            var disp = fixture.Bind(vm, view, x => x.JustADecimal, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, d => d.ToString(), decimal.Parse);

            Assert.Equal(vm.JustADecimal.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);
            Assert.Equal(123.45m, vm.JustADecimal);

            view.SomeTextBox.Text = "567.89";
            Assert.Equal(567.89m, vm.JustADecimal);

            disp?.Dispose();
            vm.JustADecimal = 0;

            Assert.Equal(0, vm.JustADecimal);
            Assert.Equal("567.89", view.SomeTextBox.Text);
        }

        /// <summary>
        /// Performs a smoke test with two way binding.
        /// </summary>
        [Fact]
        public void TwoWayBindSmokeTest()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            var fixture = new PropertyBinderImplementation();

            vm.Property1 = "Foo";
            Assert.NotEqual(vm.Property1, view.SomeTextBox.Text);

            var disp = fixture.Bind(vm, view, x => x.Property1, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

            Assert.Equal(vm.Property1, view.SomeTextBox.Text);
            Assert.Equal("Foo", vm.Property1);

            view.SomeTextBox.Text = "Bar";
            Assert.Equal(vm.Property1, "Bar");

            disp.Dispose();
            vm.Property1 = "Baz";

            Assert.Equal("Baz", vm.Property1);
            Assert.NotEqual(vm.Property1, view.SomeTextBox.Text);
        }

        /// <summary>
        /// Performs a smoke test with two way binding with a type converter.
        /// </summary>
        [Fact]
        public void TypeConvertedTwoWayBindSmokeTest()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            var fixture = new PropertyBinderImplementation();

            vm.Property2 = 17;
            Assert.NotEqual(vm.Property2.ToString(), view.SomeTextBox.Text);

            var disp = fixture.Bind(vm, view, x => x.Property2, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

            Assert.Equal(vm.Property2.ToString(), view.SomeTextBox.Text);
            Assert.Equal(17, vm.Property2);

            view.SomeTextBox.Text = "42";
            Assert.Equal(42, vm.Property2);

            // Bad formatting error
            view.SomeTextBox.Text = "--";
            Assert.Equal(42, vm.Property2);

            disp.Dispose();
            vm.Property2 = 0;

            Assert.Equal(0, vm.Property2);
            Assert.NotEqual("0", view.SomeTextBox.Text);

            vm.JustADecimal = 17.2m;
            var disp1 = fixture.Bind(vm, view, x => x.JustADecimal, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

            Assert.Equal(vm.JustADecimal.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);
            Assert.Equal(17.2m, vm.JustADecimal);

            view.SomeTextBox.Text = 42.3m.ToString(CultureInfo.InvariantCulture);
            Assert.Equal(42.3m, vm.JustADecimal);

            // Bad formatting.
            view.SomeTextBox.Text = "--";
            Assert.Equal(42.3m, vm.JustADecimal);

            disp1.Dispose();

            vm.JustADecimal = 0;

            Assert.Equal(0, vm.JustADecimal);
            Assert.NotEqual("0", view.SomeTextBox.Text);

            // Empty test
            vm.JustAInt32 = 12;
            var disp2 = fixture.Bind(vm, view, x => x.JustAInt32, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

            view.SomeTextBox.Text = string.Empty;
            Assert.Equal(12, vm.JustAInt32);

            view.SomeTextBox.Text = "1.2";

            Assert.Equal(12, vm.JustAInt32);

            view.SomeTextBox.Text = "13";
            Assert.Equal(13, vm.JustAInt32);
        }

        /// <summary>
        /// Tests binding into model objects.
        /// </summary>
        [Fact]
        public void BindingIntoModelObjects()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.OneWayBind(view.ViewModel, x => x!.Model!.AnotherThing, x => x.SomeTextBox.Text);
            Assert.Equal("Baz", view.SomeTextBox.Text);
        }

        /// <summary>
        /// Tests the view model nullable to view non nullable.
        /// </summary>
        [Fact]
        public void ViewModelNullableToViewNonNullable()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.Bind(view.ViewModel, x => x.NullableDouble, x => x.FakeControl.JustADouble);
            Assert.Equal(0.0, view.FakeControl.JustADouble);

            vm.NullableDouble = 4.0;
            Assert.Equal(4.0, view.FakeControl.JustADouble);

            vm.NullableDouble = null;
            Assert.Equal(4.0, view.FakeControl.JustADouble);

            vm.NullableDouble = 0.0;
            Assert.Equal(0.0, view.FakeControl.JustADouble);
        }

        /// <summary>
        /// Tests the view model non-nullable to view nullable.
        /// </summary>
        [Fact]
        public void ViewModelNonNullableToViewNullable()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.Bind(view.ViewModel, x => x.JustADouble, x => x.FakeControl.NullableDouble);
            Assert.Equal(0.0, vm.JustADouble);

            view.FakeControl.NullableDouble = 4.0;
            Assert.Equal(4.0, vm.JustADouble);

            view.FakeControl.NullableDouble = null;
            Assert.Equal(4.0, vm.JustADouble);

            view.FakeControl.NullableDouble = 0.0;
            Assert.Equal(0.0, vm.JustADouble);
        }

        /// <summary>
        /// Tests the view model nullable to view nullable.
        /// </summary>
        [Fact]
        public void ViewModelNullableToViewNullable()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.Bind(view.ViewModel, x => x.NullableDouble, x => x.FakeControl.NullableDouble);
            Assert.Equal(null, vm.NullableDouble);

            view.FakeControl.NullableDouble = 4.0;
            Assert.Equal(4.0, vm.NullableDouble);

            view.FakeControl.NullableDouble = null;
            Assert.Equal(null, vm.NullableDouble);

            view.FakeControl.NullableDouble = 0.0;
            Assert.Equal(0.0, vm.NullableDouble);
        }

        /// <summary>
        /// Tests the view model indexer to view.
        /// </summary>
        [Fact]
        public void ViewModelIndexerToView()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.OneWayBind(view.ViewModel, x => x.SomeCollectionOfStrings[0], x => x.SomeTextBox.Text);
            Assert.Equal("Foo", view.SomeTextBox.Text);
        }

        /// <summary>
        /// Tests the view model indexer to view changes.
        /// </summary>
        [Fact]
        public void ViewModelIndexerToViewChanges()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.OneWayBind(view.ViewModel, x => x.SomeCollectionOfStrings[0], x => x.SomeTextBox.Text);
            Assert.Equal("Foo", view.SomeTextBox.Text);

            vm.SomeCollectionOfStrings[0] = "Bar";

            Assert.Equal("Bar", view.SomeTextBox.Text);
        }

        /// <summary>
        /// Tests view model indexer property to view.
        /// </summary>
        [Fact]
        public void ViewModelIndexerPropertyToView()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.OneWayBind(view.ViewModel, x => x.SomeCollectionOfStrings[0].Length, x => x.SomeTextBox.Text);
            Assert.Equal("3", view.SomeTextBox.Text);
        }

        /// <summary>
        /// Tests when OneWayBind shouldn't initially be set to null.
        /// </summary>
        [Fact]
        public void OneWayBindShouldntInitiallySetToNull()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = null };

            view.OneWayBind(vm, x => x!.Model!.AnotherThing, x => x.FakeControl.NullHatingString);
            Assert.Equal(string.Empty, view.FakeControl.NullHatingString);

            view.ViewModel = vm;
            Assert.Equal(vm!.Model!.AnotherThing, view.FakeControl.NullHatingString);
        }

        /// <summary>
        /// Perform a BindTo type conversion smoke test.
        /// </summary>
        [Fact]
        public void BindToTypeConversionSmokeTest()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = null };

            Assert.Equal(string.Empty, view.FakeControl.NullHatingString);

            view.WhenAnyValue(x => x.ViewModel!.JustADouble)
                .BindTo(view, x => x.FakeControl.NullHatingString);

            view.ViewModel = vm;
            Assert.Equal(vm.JustADouble.ToString(CultureInfo.InvariantCulture), view.FakeControl.NullHatingString);
        }

        /// <summary>
        /// Tests that BindTo null should throw a helpful error.
        /// </summary>
        [Fact]
        public void BindToNullShouldThrowHelpfulError()
        {
            var view = new PropertyBindView { ViewModel = null };

            Assert.Throws<ArgumentNullException>(() =>
                 view.WhenAnyValue(x => x.FakeControl!.NullHatingString!)
                     .BindTo(view!.ViewModel!, x => x!.Property1));
        }

        /// <summary>
        /// Tests that BindTo two-way selected item of ItemControl.
        /// </summary>
        [Fact]
        public void TwoWayBindToSelectedItemOfItemsControl()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            view.FakeItemsControl.ItemsSource = new ObservableCollectionExtended<string>(new[] { "aaa", "bbb", "ccc" });

            view.Bind(view.ViewModel, x => x.Property1, x => x.FakeItemsControl.SelectedItem);

            Assert.Null(view.FakeItemsControl.SelectedItem);
            Assert.Null(vm.Property1);

            view.FakeItemsControl.SelectedItem = "aaa";
            Assert.Equal("aaa", vm.Property1); // fail

            vm.Property1 = "bbb";
            Assert.Equal("bbb", view.FakeItemsControl.SelectedItem);
        }

        /// <summary>
        /// Tests that ItemControl get a DataTemplate if none is set.
        /// </summary>
        [Fact]
        public void ItemsControlShouldGetADataTemplate()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            Assert.Null(view.FakeItemsControl.ItemTemplate);
            view.OneWayBind(vm, x => x.SomeCollectionOfStrings, x => x.FakeItemsControl.ItemsSource);

            Assert.NotNull(view.FakeItemsControl.ItemTemplate);
        }

        /// <summary>
        /// Tests that ItemControl display member path doesn't set a DataTemplate.
        /// </summary>
        [Fact]
        public void ItemsControlWithDisplayMemberPathSetShouldNotGetADataTemplate()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            view.FakeItemsControl.DisplayMemberPath = "Bla";

            Assert.Null(view.FakeItemsControl.ItemTemplate);
            view.OneWayBind(vm, x => x.SomeCollectionOfStrings, x => x.FakeItemsControl.ItemsSource);

            Assert.Null(view.FakeItemsControl.ItemTemplate);
        }

        /// <summary>
        /// Tests that ItemControl get a DataTemplate if none is set with BindTo.
        /// </summary>
        [Fact]
        public void ItemsControlShouldGetADataTemplateInBindTo()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            Assert.Null(view.FakeItemsControl.ItemTemplate);
            vm.WhenAnyValue(x => x.SomeCollectionOfStrings)
                .BindTo(view, v => v.FakeItemsControl.ItemsSource);

            Assert.NotNull(view.FakeItemsControl.ItemTemplate);

            view.WhenAnyValue(x => x.FakeItemsControl.SelectedItem)
                .BindTo(vm, x => x.Property1);
        }

        /// <summary>
        /// Tests that ItemControl OneWayBind.
        /// </summary>
        [Fact]
        public void BindingToItemsControl()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.OneWayBind(view.ViewModel, x => x.SomeCollectionOfStrings, x => x.FakeItemsControl.ItemsSource);

            var itemsSourceValue = (IList)view.FakeItemsControl.ItemsSource;
            Assert.True(itemsSourceValue.OfType<string>().Count() > 1);
        }

        /// <summary>
        /// Tests OneWayBind and a converter.
        /// </summary>
        [Fact]
        public void OneWayBindConverter()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            var fixture = new PropertyBinderImplementation();
            fixture.OneWayBind(vm, view, x => x.JustABoolean, x => x.SomeTextBox.IsEnabled, s => s);
            Assert.False(view.SomeTextBox.IsEnabled);
        }

        /// <summary>
        /// Tests OneWayBind and a converter with a null starting value, and tests it against a non-null value.
        /// </summary>
        [Fact]
        public void OneWayBindWithNullStartingValueToNonNullValue()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.OneWayBind(vm, x => x.Property1, x => x.SomeTextBox.Text);

            vm.Property1 = "Baz";

            Assert.Equal("Baz", view.SomeTextBox.Text);
        }

        /// <summary>
        /// Tests OneWayBind and a converter with a non-null starting value, and tests it against a null value.
        /// </summary>
        [Fact]
        public void OneWayBindWithNonNullStartingValueToNullValue()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            vm.Property1 = "Baz";

            view.OneWayBind(vm, x => x.Property1, x => x.SomeTextBox.Text);

            vm.Property1 = null;

            Assert.True(string.IsNullOrEmpty(view.SomeTextBox.Text));
        }

        /// <summary>
        /// Tests OneWayBind and a converter with a non-null starting value, and tests it against a non-null value.
        /// </summary>
        [Fact]
        public void OneWayBindWithSelectorAndNonNullStartingValueToNullValue()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.OneWayBind(vm, x => x.Model, x => x.SomeTextBox.Text, x => x?.AnotherThing);

            vm.Model = null;

            Assert.True(string.IsNullOrEmpty(view.SomeTextBox.Text));
        }

        /// <summary>
        /// Tests OneWayBind initial view model should be garbage collected when overwritten.
        /// </summary>
        [Fact]
        public void OneWayBindInitialViewModelShouldBeGarbageCollectedWhenOverwritten()
        {
            static (IDisposable?, WeakReference) GetWeakReference()
            {
                var vm = new PropertyBindViewModel();
                var view = new PropertyBindView { ViewModel = vm };
                var weakRef = new WeakReference(vm);
                var disp = view.OneWayBind(vm, x => x.Property1, x => x.SomeTextBox.Text);
                view.ViewModel = new PropertyBindViewModel();

                return (disp, weakRef);
            }

            var (disp, weakRef) = GetWeakReference();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(weakRef.IsAlive);
        }

        /// <summary>
        /// Tests BindTo  with a null starting value, and tests it against a non-null value.
        /// </summary>
        [Fact]
        public void BindToWithNullStartingValueToNonNullValue()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.WhenAnyValue(x => x.ViewModel!.Property1)
                .BindTo(view, x => x.SomeTextBox.Text);

            vm.Property1 = "Baz";

            Assert.Equal("Baz", view.SomeTextBox.Text);
        }

        /// <summary>
        /// Tests BindTo  with a non-null starting value, and tests it against a null value.
        /// </summary>
        [Fact]
        public void BindToWithNonNullStartingValueToNullValue()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            vm.Property1 = "Baz";

            view.WhenAnyValue(x => x.ViewModel!.Property1)
                .BindTo(view, x => x.SomeTextBox.Text);

            vm.Property1 = null;

            Assert.True(string.IsNullOrEmpty(view.SomeTextBox.Text));
        }

        /// <summary>
        /// Tests BindTo with a converter is not null.
        /// </summary>
        [Fact]
        public void BindExpectsConverterFuncsToNotBeNull()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            var fixture = new PropertyBinderImplementation();

            Func<string?, string?> nullFunc = null!;

            Assert.Throws<ArgumentNullException>(() => fixture.Bind(vm, view, x => x.Property1, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, nullFunc, s => s));
            Assert.Throws<ArgumentNullException>(() => fixture.Bind(vm, view, x => x.Property1, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, s => s, nullFunc));
        }

        /// <summary>
        /// Tests the BindWith func's should work as extension methods.
        /// </summary>
        [Fact]
        public void BindWithFuncShouldWorkAsExtensionMethodSmokeTest()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };

            view.Bind(vm, x => x.JustADecimal, x => x.SomeTextBox.Text, d => d.ToString(CultureInfo.InvariantCulture), decimal.Parse);
        }

        /// <summary>
        /// Tests that bind initial view model should be garbage collected when overwritten.
        /// </summary>
        [Fact]
        public void BindInitialViewModelShouldBeGarbageCollectedWhenOverwritten()
        {
            static (IDisposable?, WeakReference) GetWeakReference()
            {
                var vm = new PropertyBindViewModel();
                var view = new PropertyBindView { ViewModel = vm };
                var weakRef = new WeakReference(vm);
                var disp = view.Bind(vm, x => x.Property1, x => x.SomeTextBox.Text);
                view.ViewModel = new PropertyBindViewModel();

                return (disp, weakRef);
            }

            var (disp, weakRef) = GetWeakReference();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(weakRef.IsAlive);
        }
    }
}
