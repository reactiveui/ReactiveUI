// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Globalization;

using DynamicData.Binding;

#if NETFX_CORE
#else

using FactAttribute = Xunit.WpfFactAttribute;

#endif

namespace ReactiveUI.Tests.Xaml;

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

        var disp = fixture.Bind(vm, view, x => x.JustADecimal, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, d => d.ToString(), t => decimal.TryParse(t, out var res) ? res : decimal.Zero);

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

        Assert.Equal(vm.JustADecimal.ToString(CultureInfo.CurrentCulture), view.SomeTextBox.Text);
        Assert.Equal(17.2m, vm.JustADecimal);

        view.SomeTextBox.Text = 42.3m.ToString(CultureInfo.CurrentCulture);
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

        view.OneWayBind(view.ViewModel, x => x.Model!.AnotherThing, x => x.SomeTextBox.Text);
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

        view.OneWayBind(vm, x => x.Model!.AnotherThing, x => x.FakeControl.NullHatingString);
        Assert.Equal(string.Empty, view.FakeControl.NullHatingString);

        view.ViewModel = vm;
        Assert.Equal(vm.Model!.AnotherThing, view.FakeControl.NullHatingString);
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
             view.WhenAnyValue(x => x.FakeControl.NullHatingString)
                 .BindTo(view.ViewModel, x => x.Property1));
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

        vm.JustADecimal = 123.45m;
        Assert.NotEqual(vm.JustADecimal.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        view.Bind(vm, x => x.JustADecimal, x => x.SomeTextBox.Text, d => d.ToString(CultureInfo.InvariantCulture), t => decimal.TryParse(t, out var res) ? res : 0m);

        Assert.Equal(view.SomeTextBox.Text, "123.45");

        vm.JustADecimal = 1.0M;
        Assert.Equal(view.SomeTextBox.Text, "1.0");

        vm.JustADecimal = 2.0M;
        Assert.Equal(view.SomeTextBox.Text, "2.0");

        view.SomeTextBox.Text = "3.0";
        Assert.Equal(vm.JustADecimal, 3.0M);
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

    [Fact]
    public void OneWayBindWithHintTest()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView() { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        fixture.OneWayBind(vm, view, vm => vm.JustABoolean, v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis);
        Assert.Equal(view.SomeTextBox.Visibility, System.Windows.Visibility.Visible);

        vm.JustABoolean = true;
        Assert.Equal(view.SomeTextBox.Visibility, System.Windows.Visibility.Collapsed);

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void OneWayBindWithHintTestDisposeWithFailure()
    {
        CompositeDisposable? dis = null;
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView() { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        Assert.Throws<ArgumentNullException>(() => fixture.OneWayBind(vm, view, vm => vm.JustABoolean, v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis!));
    }

    [Fact]
    public void BindToWithHintTest()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var obs = vm.WhenAnyValue(x => x.JustABoolean);
        var a = new PropertyBinderImplementation().BindTo(obs, view, v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis);
        Assert.Equal(view.SomeTextBox.Visibility, System.Windows.Visibility.Visible);

        vm.JustABoolean = true;
        Assert.Equal(view.SomeTextBox.Visibility, System.Windows.Visibility.Collapsed);

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToView()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADecimal = 123.45m;
        Assert.NotEqual(vm.JustADecimal.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        view.Bind(vm, x => x.JustADecimal, x => x.SomeTextBox.Text, update.AsObservable(), d => d.ToString(CultureInfo.InvariantCulture), t => decimal.TryParse(t, out var res) ? res : decimal.Zero, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADecimal = 1.0M;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123.45");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1.0");

        vm.JustADecimal = 2.0M;
        Assert.Equal(view.SomeTextBox.Text, "1.0");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.0");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.0";
        Assert.Equal(vm.JustADecimal, 3.0M);

        view.SomeTextBox.Text = "4.0";
        Assert.Equal(vm.JustADecimal, 4.0M);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADecimal = 2.0M;
        Assert.Equal(view.SomeTextBox.Text, "4.0");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.0");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithDecimalConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADecimal = 123.45m;
        Assert.NotEqual(vm.JustADecimal.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var decimalToStringTypeConverter = new DecimalToStringTypeConverter();

        view.Bind(vm, x => x.JustADecimal, x => x.SomeTextBox.Text, update.AsObservable(), 2, decimalToStringTypeConverter, decimalToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADecimal = 1.0M;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123.45");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        vm.JustADecimal = 2.0M;
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.Equal(vm.JustADecimal, 3.0M);

        view.SomeTextBox.Text = "4.00";
        Assert.Equal(vm.JustADecimal, 4.0M);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADecimal = 2.0M;
        Assert.Equal(view.SomeTextBox.Text, "4.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableDecimalConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullDecimal = 123.45m;
        Assert.NotEqual(vm.JustANullDecimal.Value.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var decimalToStringTypeConverter = new NullableDecimalToStringTypeConverter();

        view.Bind(vm, x => x.JustANullDecimal, x => x.SomeTextBox.Text, update.AsObservable(), 2, decimalToStringTypeConverter, decimalToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullDecimal = 1.0M;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123.45");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        vm.JustANullDecimal = 2.0M;
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.Equal(vm.JustANullDecimal, 3.0M);

        // test non numerical
        view.SomeTextBox.Text = "ad3";
        Assert.Equal(vm.JustANullDecimal, 3.0M);

        view.SomeTextBox.Text = "4.00";
        Assert.Equal(vm.JustANullDecimal, 4.0M);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullDecimal = 2.0M;
        Assert.Equal(view.SomeTextBox.Text, "4.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewToViewModel()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADecimal = 123.45m;
        Assert.NotEqual(vm.JustADecimal.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        view.Bind(vm, x => x.JustADecimal, x => x.SomeTextBox.Text, update.AsObservable(), d => d.ToString(CultureInfo.InvariantCulture), t => decimal.TryParse(t, out var res) ? res : decimal.Zero, TriggerUpdate.ViewToViewModel).DisposeWith(dis);

        view.SomeTextBox.Text = "1.0";

        // value should have pre bind value
        Assert.Equal(vm.JustADecimal, 123.45m);

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(vm.JustADecimal, 1.0m);

        view.SomeTextBox.Text = "2.0";
        Assert.Equal(vm.JustADecimal, 1.0m);

        update.OnNext(true);
        Assert.Equal(vm.JustADecimal, 2.0m);

        // test reverse bind no trigger required
        vm.JustADecimal = 3.0m;
        Assert.Equal(view.SomeTextBox.Text, "3.0");

        vm.JustADecimal = 4.0m;
        Assert.Equal(view.SomeTextBox.Text, "4.0");

        // test forward bind to ensure trigger is still honoured.
        view.SomeTextBox.Text = "2.0";
        Assert.Equal(vm.JustADecimal, 4.0m);

        update.OnNext(true);
        Assert.Equal(vm.JustADecimal, 2.0m);

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithDoubleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADouble = 123.45;
        Assert.NotEqual(vm.JustADouble.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new DoubleToStringTypeConverter();

        view.Bind(vm, x => x.JustADouble, x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADouble = 1.0;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123.45");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        vm.JustADouble = 2.0;
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.Equal(vm.JustADouble, 3.0);

        view.SomeTextBox.Text = "4.00";
        Assert.Equal(vm.JustADouble, 4.0);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADouble = 2.0;
        Assert.Equal(view.SomeTextBox.Text, "4.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableDoubleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullDouble = 123.45;
        Assert.NotEqual(vm.JustANullDouble.Value.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new NullableDoubleToStringTypeConverter();

        view.Bind(vm, x => x.JustANullDouble, x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullDouble = 1.0;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123.45");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        vm.JustANullDouble = 2.0;
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.Equal(vm.JustANullDouble, 3.0);

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        Assert.Equal(vm.JustANullDouble, 3.0);

        view.SomeTextBox.Text = "4.00";
        Assert.Equal(vm.JustANullDouble, 4.0);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullDouble = 2.0;
        Assert.Equal(view.SomeTextBox.Text, "4.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithDoubleConverterNoRound()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADouble = 123.45;
        Assert.NotEqual(vm.JustADouble.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        view.Bind(vm, x => x.JustADouble, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADouble = 1.0;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123.45");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1");

        vm.JustADouble = 2.0;
        Assert.Equal(view.SomeTextBox.Text, "1");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.Equal(vm.JustADouble, 3.0);

        view.SomeTextBox.Text = "4";
        Assert.Equal(vm.JustADouble, 4.0);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADouble = 2.0;
        Assert.Equal(view.SomeTextBox.Text, "4");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithSingleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustASingle = 123.45f;
        Assert.NotEqual(vm.JustASingle.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new SingleToStringTypeConverter();

        view.Bind(vm, x => x.JustASingle, x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustASingle = 1.0f;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123.45");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        vm.JustASingle = 2.0f;
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.Equal(vm.JustASingle, 3.0f);

        view.SomeTextBox.Text = "4.00";
        Assert.Equal(vm.JustASingle, 4.0f);

        // test forward bind to ensure trigger is still honoured.
        vm.JustASingle = 2.0f;
        Assert.Equal(view.SomeTextBox.Text, "4.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableSingleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullSingle = 123.45f;
        Assert.NotEqual(vm.JustANullSingle.Value.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new NullableSingleToStringTypeConverter();

        view.Bind(vm, x => x.JustANullSingle, x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullSingle = 1.0f;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123.45");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        vm.JustANullSingle = 2.0f;
        Assert.Equal(view.SomeTextBox.Text, "1.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.Equal(vm.JustANullSingle, 3.0f);

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        Assert.Equal(vm.JustANullSingle, 3.0f);

        view.SomeTextBox.Text = "4.00";
        Assert.Equal(vm.JustANullSingle, 4.0f);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullSingle = 2.0f;
        Assert.Equal(view.SomeTextBox.Text, "4.00");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2.00");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithSingleConverterNoRound()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustASingle = 123.45f;
        Assert.NotEqual(vm.JustASingle.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        view.Bind(vm, x => x.JustASingle, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustASingle = 1.0f;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123.45");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1");

        vm.JustASingle = 2.0f;
        Assert.Equal(view.SomeTextBox.Text, "1");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.Equal(vm.JustASingle, 3.0f);

        view.SomeTextBox.Text = "4";
        Assert.Equal(vm.JustASingle, 4.0f);

        // test forward bind to ensure trigger is still honoured.
        vm.JustASingle = 2.0f;
        Assert.Equal(view.SomeTextBox.Text, "4");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithByteConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAByte = 123;
        Assert.NotEqual(vm.JustAByte.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new ByteToStringTypeConverter();

        view.Bind(vm, x => x.JustAByte, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAByte = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "001");

        vm.JustAByte = 2;
        Assert.Equal(view.SomeTextBox.Text, "001");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.Equal(vm.JustAByte, 3);

        view.SomeTextBox.Text = "004";
        Assert.Equal(vm.JustAByte, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAByte = 2;
        Assert.Equal(view.SomeTextBox.Text, "004");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableByteConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullByte = 123;
        Assert.NotEqual(vm.JustANullByte.Value.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new NullableByteToStringTypeConverter();

        view.Bind(vm, x => x.JustANullByte, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullByte = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "001");

        vm.JustANullByte = 2;
        Assert.Equal(view.SomeTextBox.Text, "001");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.Equal(vm.JustANullByte.Value, 3);

        // test non numerical value
        view.SomeTextBox.Text = "ad4";
        Assert.Equal(vm.JustANullByte.Value, 3);

        view.SomeTextBox.Text = "004";
        Assert.Equal(vm.JustANullByte.Value, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullByte = 2;
        Assert.Equal(view.SomeTextBox.Text, "004");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithByteConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAByte = 123;
        Assert.NotEqual(vm.JustAByte.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        view.Bind(vm, x => x.JustAByte, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAByte = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1");

        vm.JustAByte = 2;
        Assert.Equal(view.SomeTextBox.Text, "1");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.Equal(vm.JustAByte, 3);

        view.SomeTextBox.Text = "4";
        Assert.Equal(vm.JustAByte, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAByte = 2;
        Assert.Equal(view.SomeTextBox.Text, "4");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithShortConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt16 = 123;
        Assert.NotEqual(vm.JustAInt16.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new ShortToStringTypeConverter();

        view.Bind(vm, x => x.JustAInt16, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt16 = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "001");

        vm.JustAInt16 = 2;
        Assert.Equal(view.SomeTextBox.Text, "001");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.Equal(vm.JustAInt16, 3);

        view.SomeTextBox.Text = "004";
        Assert.Equal(vm.JustAInt16, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt16 = 2;
        Assert.Equal(view.SomeTextBox.Text, "004");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableShortConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullInt16 = 123;
        Assert.NotEqual(vm.JustANullInt16.Value.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new NullableShortToStringTypeConverter();

        view.Bind(vm, x => x.JustANullInt16, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullInt16 = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "001");

        vm.JustANullInt16 = 2;
        Assert.Equal(view.SomeTextBox.Text, "001");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.Equal(vm.JustANullInt16.Value, 3);

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        Assert.Equal(vm.JustANullInt16.Value, 3);

        view.SomeTextBox.Text = "004";
        Assert.Equal(vm.JustANullInt16.Value, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullInt16 = 2;
        Assert.Equal(view.SomeTextBox.Text, "004");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithShortConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt16 = 123;
        Assert.NotEqual(vm.JustAInt16.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        view.Bind(vm, x => x.JustAInt16, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt16 = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1");

        vm.JustAInt16 = 2;
        Assert.Equal(view.SomeTextBox.Text, "1");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.Equal(vm.JustAInt16, 3);

        view.SomeTextBox.Text = "4";
        Assert.Equal(vm.JustAInt16, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt16 = 2;
        Assert.Equal(view.SomeTextBox.Text, "4");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithIntegerConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt32 = 123;
        Assert.NotEqual(vm.JustAInt32.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new IntegerToStringTypeConverter();

        view.Bind(vm, x => x.JustAInt32, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt32 = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "001");

        vm.JustAInt32 = 2;
        Assert.Equal(view.SomeTextBox.Text, "001");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.Equal(vm.JustAInt32, 3);

        view.SomeTextBox.Text = "004";
        Assert.Equal(vm.JustAInt32, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt32 = 2;
        Assert.Equal(view.SomeTextBox.Text, "004");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableIntegerConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullInt32 = 123;
        Assert.NotEqual(vm.JustANullInt32!.Value.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new NullableIntegerToStringTypeConverter();

        view.Bind(vm, x => x.JustANullInt32, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullInt32 = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "001");

        vm.JustANullInt32 = 2;
        Assert.Equal(view.SomeTextBox.Text, "001");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.Equal(vm.JustANullInt32, 3);

        // test if the binding handles a non number
        view.SomeTextBox.Text = "3a4";
        Assert.Equal(vm.JustANullInt32, 3);

        view.SomeTextBox.Text = "004";
        Assert.Equal(vm.JustANullInt32, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullInt32 = 2;
        Assert.Equal(view.SomeTextBox.Text, "004");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithIntegerConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt32 = 123;
        Assert.NotEqual(vm.JustAInt32.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        view.Bind(vm, x => x.JustAInt32, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt32 = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1");

        vm.JustAInt32 = 2;
        Assert.Equal(view.SomeTextBox.Text, "1");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.Equal(vm.JustAInt32, 3);

        view.SomeTextBox.Text = "4";
        Assert.Equal(vm.JustAInt32, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt32 = 2;
        Assert.Equal(view.SomeTextBox.Text, "4");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithLongConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt64 = 123;
        Assert.NotEqual(vm.JustAInt64.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        var xToStringTypeConverter = new LongToStringTypeConverter();

        view.Bind(vm, x => x.JustAInt64, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt64 = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "001");

        vm.JustAInt64 = 2;
        Assert.Equal(view.SomeTextBox.Text, "001");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.Equal(vm.JustAInt64, 3);

        view.SomeTextBox.Text = "004";
        Assert.Equal(vm.JustAInt64, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt64 = 2;
        Assert.Equal(view.SomeTextBox.Text, "004");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "002");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }

    [Fact]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithLongConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt64 = 123;
        Assert.NotEqual(vm.JustAInt64.ToString(CultureInfo.InvariantCulture), view.SomeTextBox.Text);

        view.Bind(vm, x => x.JustAInt64, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt64 = 1;

        // value should have pre bind value
        Assert.Equal(view.SomeTextBox.Text, "123");

        // trigger UI update
        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "1");

        vm.JustAInt64 = 2;
        Assert.Equal(view.SomeTextBox.Text, "1");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.Equal(vm.JustAInt64, 3);

        view.SomeTextBox.Text = "4";
        Assert.Equal(vm.JustAInt64, 4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt64 = 2;
        Assert.Equal(view.SomeTextBox.Text, "4");

        update.OnNext(true);
        Assert.Equal(view.SomeTextBox.Text, "2");

        dis.Dispose();
        Assert.True(dis.IsDisposed);
    }
}
