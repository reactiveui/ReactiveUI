// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Globalization;

using DynamicData.Binding;

using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using ReactiveUI.Tests.Xaml.Utilities;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests property bindings.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class PropertyBindingTest
{
    /// <summary>
    /// Performs a smoke test with two way binding with func converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TwoWayBindWithFuncConvertersSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        vm.JustADecimal = 123.45m;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        var disp = fixture.Bind(vm, view, static x => x.JustADecimal, static x => x.SomeTextBox.Text, (IObservable<Unit>?)null, static d => d.ToString(), static t => decimal.TryParse(t, out var res) ? res : decimal.Zero);

        using (Assert.Multiple())
        {
            await Assert.That(view.SomeTextBox.Text).IsEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));
            await Assert.That(vm.JustADecimal).IsEqualTo(123.45m);
        }

        view.SomeTextBox.Text = "567.89";
        await Assert.That(vm.JustADecimal).IsEqualTo(567.89m);

        disp?.Dispose();
        vm.JustADecimal = 0;

        using (Assert.Multiple())
        {
            await Assert.That(vm.JustADecimal).IsEqualTo(0);
            await Assert.That(view.SomeTextBox.Text).IsEqualTo("567.89");
        }
    }

    /// <summary>
    /// Performs a smoke test with two way binding.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TwoWayBindSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        vm.Property1 = "Foo";
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.Property1);

        var disp = fixture.Bind(vm, view, static x => x.Property1, static x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        using (Assert.Multiple())
        {
            await Assert.That(view.SomeTextBox.Text).IsEqualTo(vm.Property1);
            await Assert.That(vm.Property1).IsEqualTo("Foo");
        }

        view.SomeTextBox.Text = "Bar";
        await Assert.That(vm.Property1).IsEqualTo("Bar");

        disp.Dispose();
        vm.Property1 = "Baz";

        using (Assert.Multiple())
        {
            await Assert.That(vm.Property1).IsEqualTo("Baz");
            await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.Property1);
        }
    }

    /// <summary>
    /// Performs a smoke test with two way binding with a type converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TypeConvertedTwoWayBindSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        vm.Property2 = 17;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.Property2.ToString());

        var disp = fixture.Bind(vm, view, static x => x.Property2, static x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        using (Assert.Multiple())
        {
            await Assert.That(view.SomeTextBox.Text).IsEqualTo(vm.Property2.ToString());
            await Assert.That(vm.Property2).IsEqualTo(17);
        }

        view.SomeTextBox.Text = "42";
        await Assert.That(vm.Property2).IsEqualTo(42);

        // Bad formatting error
        view.SomeTextBox.Text = "--";
        await Assert.That(vm.Property2).IsEqualTo(42);

        disp.Dispose();
        vm.Property2 = 0;

        using (Assert.Multiple())
        {
            await Assert.That(vm.Property2).IsEqualTo(0);
            await Assert.That(view.SomeTextBox.Text).IsNotEqualTo("0");
        }

        vm.JustADecimal = 17.2m;
        var disp1 = fixture.Bind(vm, view, static x => x.JustADecimal, static x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        using (Assert.Multiple())
        {
            await Assert.That(view.SomeTextBox.Text).IsEqualTo(vm.JustADecimal.ToString(CultureInfo.CurrentCulture));
            await Assert.That(vm.JustADecimal).IsEqualTo(17.2m);
        }

        view.SomeTextBox.Text = 42.3m.ToString(CultureInfo.CurrentCulture);
        await Assert.That(vm.JustADecimal).IsEqualTo(42.3m);

        // Bad formatting.
        view.SomeTextBox.Text = "--";
        await Assert.That(vm.JustADecimal).IsEqualTo(42.3m);

        disp1.Dispose();

        vm.JustADecimal = 0;

        using (Assert.Multiple())
        {
            await Assert.That(vm.JustADecimal).IsEqualTo(0);
            await Assert.That(view.SomeTextBox.Text).IsNotEqualTo("0");
        }

        // Empty test
        vm.JustAInt32 = 12;
        var disp2 = fixture.Bind(vm, view, static x => x.JustAInt32, static x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        view.SomeTextBox.Text = string.Empty;
        await Assert.That(vm.JustAInt32).IsEqualTo(12);

        view.SomeTextBox.Text = "1.2";
        await Assert.That(vm.JustAInt32).IsEqualTo(12);

        view.SomeTextBox.Text = "13";
        await Assert.That(vm.JustAInt32).IsEqualTo(13);
    }

    /// <summary>
    /// Tests binding into model objects.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindingIntoModelObjects()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, static x => x.Model!.AnotherThing, static x => x.SomeTextBox.Text);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("Baz");
    }

    /// <summary>
    /// Tests the view model nullable to view non nullable.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNullableToViewNonNullable()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.Bind(view.ViewModel, static x => x.NullableDouble, static x => x.FakeControl.JustADouble);
        await Assert.That(view.FakeControl.JustADouble).IsEqualTo(0);

        vm.NullableDouble = 4.0;
        await Assert.That(view.FakeControl.JustADouble).IsEqualTo(4.0);

        vm.NullableDouble = null;
        await Assert.That(view.FakeControl.JustADouble).IsEqualTo(4.0);

        vm.NullableDouble = 0.0;
        await Assert.That(view.FakeControl.JustADouble).IsEqualTo(0);
    }

    /// <summary>
    /// Tests the view model non-nullable to view nullable.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNonNullableToViewNullable()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.Bind(view.ViewModel, static x => x.JustADouble, static x => x.FakeControl.NullableDouble);
        await Assert.That(vm.JustADouble).IsEqualTo(0);

        view.FakeControl.NullableDouble = 4.0;
        await Assert.That(vm.JustADouble).IsEqualTo(4.0);

        view.FakeControl.NullableDouble = null;
        await Assert.That(vm.JustADouble).IsEqualTo(4.0);

        view.FakeControl.NullableDouble = 0.0;
        await Assert.That(vm.JustADouble).IsEqualTo(0);
    }

    /// <summary>
    /// Tests the view model nullable to view nullable.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNullableToViewNullable()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.Bind(view.ViewModel, static x => x.NullableDouble, static x => x.FakeControl.NullableDouble);
        await Assert.That(vm.NullableDouble).IsNull();

        view.FakeControl.NullableDouble = 4.0;
        await Assert.That(vm.NullableDouble).IsEqualTo(4.0);

        view.FakeControl.NullableDouble = null;
        await Assert.That(vm.NullableDouble).IsNull();

        view.FakeControl.NullableDouble = 0.0;
        await Assert.That(vm.NullableDouble).IsEqualTo(0);
    }

    /// <summary>
    /// Tests the view model indexer to view.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelIndexerToView()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, static x => x.SomeCollectionOfStrings[0], static x => x.SomeTextBox.Text);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("Foo");
    }

    /// <summary>
    /// Tests the view model indexer to view changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelIndexerToViewChanges()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, static x => x.SomeCollectionOfStrings[0], static x => x.SomeTextBox.Text);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("Foo");

        vm.SomeCollectionOfStrings[0] = "Bar";

        await Assert.That(view.SomeTextBox.Text).IsEqualTo("Bar");
    }

    /// <summary>
    /// Tests view model indexer property to view.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelIndexerPropertyToView()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, static x => x.SomeCollectionOfStrings[0].Length, static x => x.SomeTextBox.Text);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("3");
    }

    /// <summary>
    /// Tests when OneWayBind shouldn't initially be set to null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBindShouldntInitiallySetToNull()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = null };

        view.OneWayBind(vm, static x => x.Model!.AnotherThing, static x => x.FakeControl.NullHatingString);
        await Assert.That(view.FakeControl.NullHatingString).IsEqualTo(string.Empty);

        view.ViewModel = vm;
        await Assert.That(view.FakeControl.NullHatingString).IsEqualTo(vm.Model!.AnotherThing);
    }

    /// <summary>
    /// Perform a BindTo type conversion smoke test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindToTypeConversionSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = null };

        await Assert.That(view.FakeControl.NullHatingString).IsEqualTo(string.Empty);

        view.WhenAnyValue(static x => x.ViewModel!.JustADouble)
            .BindTo(view, static x => x.FakeControl.NullHatingString);

        view.ViewModel = vm;
        await Assert.That(view.FakeControl.NullHatingString).IsEqualTo(vm.JustADouble.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests that BindTo null should throw a helpful error.
    /// </summary>
    [Test]
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TwoWayBindToSelectedItemOfItemsControl()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        view.FakeItemsControl.ItemsSource = new ObservableCollectionExtended<string>(new[] { "aaa", "bbb", "ccc" });

        view.Bind(view.ViewModel, static x => x.Property1, static x => x.FakeItemsControl.SelectedItem);

        using (Assert.Multiple())
        {
            await Assert.That(view.FakeItemsControl.SelectedItem).IsNull();
            await Assert.That(vm.Property1).IsNull();
        }

        view.FakeItemsControl.SelectedItem = "aaa";
        await Assert.That(vm.Property1).IsEqualTo("aaa"); // fail

        vm.Property1 = "bbb";
        await Assert.That(view.FakeItemsControl.SelectedItem).IsEqualTo("bbb");
    }

    /// <summary>
    /// Tests that ItemControl get a DataTemplate if none is set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ItemsControlShouldGetADataTemplate()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        await Assert.That(view.FakeItemsControl.ItemTemplate).IsNull();
        view.OneWayBind(vm, static x => x.SomeCollectionOfStrings, static x => x.FakeItemsControl.ItemsSource);

        await Assert.That(view.FakeItemsControl.ItemTemplate).IsNotNull();
    }

    /// <summary>
    /// Tests that ItemControl display member path doesn't set a DataTemplate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ItemsControlWithDisplayMemberPathSetShouldNotGetADataTemplate()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        view.FakeItemsControl.DisplayMemberPath = "Bla";

        await Assert.That(view.FakeItemsControl.ItemTemplate).IsNull();
        view.OneWayBind(vm, static x => x.SomeCollectionOfStrings, static x => x.FakeItemsControl.ItemsSource);

        await Assert.That(view.FakeItemsControl.ItemTemplate).IsNull();
    }

    /// <summary>
    /// Tests that ItemControl get a DataTemplate if none is set with BindTo.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ItemsControlShouldGetADataTemplateInBindTo()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        await Assert.That(view.FakeItemsControl.ItemTemplate).IsNull();
        vm.WhenAnyValue(static x => x.SomeCollectionOfStrings)
            .BindTo(view, static v => v.FakeItemsControl.ItemsSource);

        await Assert.That(view.FakeItemsControl.ItemTemplate).IsNotNull();

        view.WhenAnyValue(static x => x.FakeItemsControl.SelectedItem)
            .BindTo(vm, static x => x.Property1);
    }

    /// <summary>
    /// Tests that ItemControl OneWayBind.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindingToItemsControl()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, static x => x.SomeCollectionOfStrings, static x => x.FakeItemsControl.ItemsSource);

        var itemsSourceValue = (IList)view.FakeItemsControl.ItemsSource;
        await Assert.That(itemsSourceValue.OfType<string>().Count()).IsGreaterThan(1);
    }

    /// <summary>
    /// Tests OneWayBind and a converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBindConverter()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();
        fixture.OneWayBind(vm, view, static x => x.JustABoolean, static x => x.SomeTextBox.IsEnabled, static s => s);
        await Assert.That(view.SomeTextBox.IsEnabled).IsFalse();
    }

    /// <summary>
    /// Tests OneWayBind and a converter with a null starting value, and tests it against a non-null value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBindWithNullStartingValueToNonNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(vm, static x => x.Property1, static x => x.SomeTextBox.Text);

        vm.Property1 = "Baz";

        await Assert.That(view.SomeTextBox.Text).IsEqualTo("Baz");
    }

    /// <summary>
    /// Tests OneWayBind and a converter with a non-null starting value, and tests it against a null value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBindWithNonNullStartingValueToNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        vm.Property1 = "Baz";

        view.OneWayBind(vm, static x => x.Property1, static x => x.SomeTextBox.Text);

        vm.Property1 = null;

        await Assert.That(string.IsNullOrEmpty(view.SomeTextBox.Text)).IsTrue();
    }

    /// <summary>
    /// Tests OneWayBind and a converter with a non-null starting value, and tests it against a non-null value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBindWithSelectorAndNonNullStartingValueToNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(vm, static x => x.Model, static x => x.SomeTextBox.Text, static x => x?.AnotherThing);

        vm.Model = null;

        await Assert.That(string.IsNullOrEmpty(view.SomeTextBox.Text)).IsTrue();
    }

    /// <summary>
    /// Tests OneWayBind initial view model should be garbage collected when overwritten.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBindInitialViewModelShouldBeGarbageCollectedWhenOverwritten()
    {
        static (IDisposable?, WeakReference) GetWeakReference()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            var weakRef = new WeakReference(vm);
            var disp = view.OneWayBind(vm, static x => x.Property1, static x => x.SomeTextBox.Text);
            view.ViewModel = new PropertyBindViewModel();

            return (disp, weakRef);
        }

        var (disp, weakRef) = GetWeakReference();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        await Assert.That(weakRef.IsAlive).IsFalse();
    }

    /// <summary>
    /// Tests BindTo  with a null starting value, and tests it against a non-null value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindToWithNullStartingValueToNonNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.WhenAnyValue(static x => x.ViewModel!.Property1)
            .BindTo(view, static x => x.SomeTextBox.Text);

        vm.Property1 = "Baz";

        await Assert.That(view.SomeTextBox.Text).IsEqualTo("Baz");
    }

    /// <summary>
    /// Tests BindTo  with a non-null starting value, and tests it against a null value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindToWithNonNullStartingValueToNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        vm.Property1 = "Baz";

        view.WhenAnyValue(static x => x.ViewModel!.Property1)
            .BindTo(view, static x => x.SomeTextBox.Text);

        vm.Property1 = null;

        await Assert.That(string.IsNullOrEmpty(view.SomeTextBox.Text)).IsTrue();
    }

    /// <summary>
    /// Tests BindTo with a converter is not null.
    /// </summary>
    [Test]
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncShouldWorkAsExtensionMethodSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        vm.JustADecimal = 123.45m;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustADecimal, static x => x.SomeTextBox.Text, static d => d.ToString(CultureInfo.InvariantCulture), static t => decimal.TryParse(t, out var res) ? res : 0m);

        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        vm.JustADecimal = 1.0M;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.0");

        vm.JustADecimal = 2.0M;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.0");

        view.SomeTextBox.Text = "3.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(3.0M);
    }

    /// <summary>
    /// Tests that bind initial view model should be garbage collected when overwritten.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindInitialViewModelShouldBeGarbageCollectedWhenOverwritten()
    {
        static (IDisposable?, WeakReference) GetWeakReference()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            var weakRef = new WeakReference(vm);
            var disp = view.Bind(vm, static x => x.Property1, static x => x.SomeTextBox.Text);
            view.ViewModel = new PropertyBindViewModel();

            return (disp, weakRef);
        }

        var (disp, weakRef) = GetWeakReference();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        await Assert.That(weakRef.IsAlive).IsFalse();
    }

    [Test]
    public async Task OneWayBindWithHintTest()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView() { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        fixture.OneWayBind(vm, view, static vm => vm.JustABoolean, static v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis);
        await Assert.That(view.SomeTextBox.Visibility).IsEqualTo(System.Windows.Visibility.Visible);

        vm.JustABoolean = true;
        await Assert.That(view.SomeTextBox.Visibility).IsEqualTo(System.Windows.Visibility.Collapsed);

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public void OneWayBindWithHintTestDisposeWithFailure()
    {
        CompositeDisposable? dis = null;
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView() { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        Assert.Throws<ArgumentNullException>(() => fixture.OneWayBind(vm, view, vm => vm.JustABoolean, v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis!));
    }

    [Test]
    public async Task BindToWithHintTest()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var obs = vm.WhenAnyValue(static x => x.JustABoolean);
        var a = new PropertyBinderImplementation().BindTo(obs, view, static v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis);
        await Assert.That(view.SomeTextBox.Visibility).IsEqualTo(System.Windows.Visibility.Visible);

        vm.JustABoolean = true;
        await Assert.That(view.SomeTextBox.Visibility).IsEqualTo(System.Windows.Visibility.Collapsed);

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToView()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADecimal = 123.45m;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustADecimal, static x => x.SomeTextBox.Text, update.AsObservable(), static d => d.ToString(CultureInfo.InvariantCulture), static t => decimal.TryParse(t, out var res) ? res : decimal.Zero, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADecimal = 1.0M;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.0");

        vm.JustADecimal = 2.0M;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.0");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.0");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(3.0M);

        view.SomeTextBox.Text = "4.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(4.0M);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADecimal = 2.0M;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.0");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.0");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithDecimalConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADecimal = 123.45m;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        var decimalToStringTypeConverter = new DecimalToStringTypeConverter();

        view.Bind(vm, static x => x.JustADecimal, static x => x.SomeTextBox.Text, update.AsObservable(), 2, decimalToStringTypeConverter, decimalToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADecimal = 1.0M;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustADecimal = 2.0M;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustADecimal).IsEqualTo(3.0M);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustADecimal).IsEqualTo(4.0M);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADecimal = 2.0M;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableDecimalConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullDecimal = 123.45m;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullDecimal.Value.ToString(CultureInfo.InvariantCulture));

        var decimalToStringTypeConverter = new NullableDecimalToStringTypeConverter();

        view.Bind(vm, static x => x.JustANullDecimal, static x => x.SomeTextBox.Text, update.AsObservable(), 2, decimalToStringTypeConverter, decimalToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullDecimal = 1.0M;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustANullDecimal = 2.0M;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustANullDecimal).IsEqualTo(3.0M);

        // test non numerical
        view.SomeTextBox.Text = "ad3";
        await Assert.That(vm.JustANullDecimal).IsEqualTo(3.0M);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustANullDecimal).IsEqualTo(4.0M);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullDecimal = 2.0M;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewToViewModel()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADecimal = 123.45m;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustADecimal, static x => x.SomeTextBox.Text, update.AsObservable(), static d => d.ToString(CultureInfo.InvariantCulture), static t => decimal.TryParse(t, out var res) ? res : decimal.Zero, TriggerUpdate.ViewToViewModel).DisposeWith(dis);

        view.SomeTextBox.Text = "1.0";

        // value should have pre bind value
        await Assert.That(vm.JustADecimal).IsEqualTo(123.45m);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(vm.JustADecimal).IsEqualTo(1.0m);

        view.SomeTextBox.Text = "2.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(1.0m);

        update.OnNext(true);
        await Assert.That(vm.JustADecimal).IsEqualTo(2.0m);

        // test reverse bind no trigger required
        vm.JustADecimal = 3.0m;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("3.0");

        vm.JustADecimal = 4.0m;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.0");

        // test forward bind to ensure trigger is still honoured.
        view.SomeTextBox.Text = "2.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(4.0m);

        update.OnNext(true);
        await Assert.That(vm.JustADecimal).IsEqualTo(2.0m);

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithDoubleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADouble = 123.45;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADouble.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new DoubleToStringTypeConverter();

        view.Bind(vm, static x => x.JustADouble, static x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADouble = 1.0;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustADouble = 2.0;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustADouble).IsEqualTo(3.0);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustADouble).IsEqualTo(4.0);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADouble = 2.0;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableDoubleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullDouble = 123.45;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullDouble.Value.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new NullableDoubleToStringTypeConverter();

        view.Bind(vm, static x => x.JustANullDouble, static x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullDouble = 1.0;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustANullDouble = 2.0;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustANullDouble).IsEqualTo(3.0);

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        await Assert.That(vm.JustANullDouble).IsEqualTo(3.0);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustANullDouble).IsEqualTo(4.0);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullDouble = 2.0;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithDoubleConverterNoRound()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADouble = 123.45;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADouble.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustADouble, static x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADouble = 1.0;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustADouble = 2.0;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That(vm.JustADouble).IsEqualTo(3.0);

        view.SomeTextBox.Text = "4";
        await Assert.That(vm.JustADouble).IsEqualTo(4.0);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADouble = 2.0;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithSingleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustASingle = 123.45f;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustASingle.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new SingleToStringTypeConverter();

        view.Bind(vm, static x => x.JustASingle, static x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustASingle = 1.0f;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustASingle = 2.0f;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustASingle).IsEqualTo(3.0f);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustASingle).IsEqualTo(4.0f);

        // test forward bind to ensure trigger is still honoured.
        vm.JustASingle = 2.0f;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableSingleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullSingle = 123.45f;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullSingle.Value.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new NullableSingleToStringTypeConverter();

        view.Bind(vm, static x => x.JustANullSingle, static x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullSingle = 1.0f;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustANullSingle = 2.0f;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustANullSingle).IsEqualTo(3.0f);

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        await Assert.That(vm.JustANullSingle).IsEqualTo(3.0f);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustANullSingle).IsEqualTo(4.0f);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullSingle = 2.0f;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithSingleConverterNoRound()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustASingle = 123.45f;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustASingle.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustASingle, static x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustASingle = 1.0f;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123.45");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustASingle = 2.0f;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That(vm.JustASingle).IsEqualTo(3.0f);

        view.SomeTextBox.Text = "4";
        await Assert.That(vm.JustASingle).IsEqualTo(4.0f);

        // test forward bind to ensure trigger is still honoured.
        vm.JustASingle = 2.0f;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithByteConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAByte = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAByte.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new ByteToStringTypeConverter();

        view.Bind(vm, static x => x.JustAByte, static x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAByte = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustAByte = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That((int)vm.JustAByte).IsEqualTo(3);

        view.SomeTextBox.Text = "004";
        await Assert.That((int)vm.JustAByte).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAByte = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableByteConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullByte = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullByte.Value.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new NullableByteToStringTypeConverter();

        view.Bind(vm, static x => x.JustANullByte, static x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullByte = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustANullByte = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That((int)vm.JustANullByte!.Value).IsEqualTo(3);

        // test non numerical value
        view.SomeTextBox.Text = "ad4";
        await Assert.That((int)vm.JustANullByte!.Value).IsEqualTo(3);

        view.SomeTextBox.Text = "004";
        await Assert.That((int)vm.JustANullByte!.Value).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullByte = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithByteConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAByte = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAByte.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustAByte, static x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAByte = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustAByte = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That((int)vm.JustAByte).IsEqualTo(3);

        view.SomeTextBox.Text = "4";
        await Assert.That((int)vm.JustAByte).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAByte = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithShortConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt16 = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt16.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new ShortToStringTypeConverter();

        view.Bind(vm, static x => x.JustAInt16, static x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt16 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustAInt16 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That((int)vm.JustAInt16).IsEqualTo(3);

        view.SomeTextBox.Text = "004";
        await Assert.That((int)vm.JustAInt16).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt16 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableShortConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullInt16 = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullInt16.Value.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new NullableShortToStringTypeConverter();

        view.Bind(vm, static x => x.JustANullInt16, static x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullInt16 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustANullInt16 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That((int)vm.JustANullInt16!.Value).IsEqualTo(3);

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        await Assert.That((int)vm.JustANullInt16!.Value).IsEqualTo(3);

        view.SomeTextBox.Text = "004";
        await Assert.That((int)vm.JustANullInt16!.Value).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullInt16 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithShortConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt16 = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt16.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustAInt16, static x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt16 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustAInt16 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That((int)vm.JustAInt16).IsEqualTo(3);

        view.SomeTextBox.Text = "4";
        await Assert.That((int)vm.JustAInt16).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt16 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithIntegerConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt32 = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt32.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new IntegerToStringTypeConverter();

        view.Bind(vm, static x => x.JustAInt32, static x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt32 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustAInt32 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That(vm.JustAInt32).IsEqualTo(3);

        view.SomeTextBox.Text = "004";
        await Assert.That(vm.JustAInt32).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt32 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableIntegerConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullInt32 = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullInt32!.Value.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new NullableIntegerToStringTypeConverter();

        view.Bind(vm, static x => x.JustANullInt32, static x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullInt32 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustANullInt32 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That(vm.JustANullInt32).IsEqualTo(3);

        // test if the binding handles a non number
        view.SomeTextBox.Text = "3a4";
        await Assert.That(vm.JustANullInt32).IsEqualTo(3);

        view.SomeTextBox.Text = "004";
        await Assert.That(vm.JustANullInt32).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullInt32 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithIntegerConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt32 = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt32.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustAInt32, static x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt32 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustAInt32 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That(vm.JustAInt32).IsEqualTo(3);

        view.SomeTextBox.Text = "4";
        await Assert.That(vm.JustAInt32).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt32 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithLongConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt64 = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt64.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new LongToStringTypeConverter();

        view.Bind(vm, static x => x.JustAInt64, static x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt64 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustAInt64 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That(vm.JustAInt64).IsEqualTo(3);

        view.SomeTextBox.Text = "004";
        await Assert.That(vm.JustAInt64).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt64 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithLongConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt64 = 123;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt64.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustAInt64, static x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt64 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustAInt64 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That(vm.JustAInt64).IsEqualTo(3);

        view.SomeTextBox.Text = "4";
        await Assert.That(vm.JustAInt64).IsEqualTo(4);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt64 = 2;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// BindTo should only invoke the nested setter once per source value on the same host.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindToSetsNestedPropertyOncePerValueOnSameHost()
    {
        var view = new TrackingHostView { ViewModel = new() };

        using var source = new Subject<string>();
        using var subscription = source.BindTo(view, static x => x.ViewModel!.Nested.SomeText);

        source.OnNext("Alpha");
        source.OnNext("Alpha");
        source.OnNext("Alpha");
        source.OnNext("Beta");
        source.OnNext("Beta");
        source.OnNext("Beta");
        source.OnNext("Gamma");
        source.OnNext("Gamma");
        source.OnNext("Gamma");

        var nested = view.ViewModel!.Nested;

        using (Assert.Multiple())
        {
            await Assert.That(nested.SetCallCount).IsEqualTo(3);
            await Assert.That(nested.SomeText).IsEqualTo("Gamma");
        }
    }

    /// <summary>
    /// BindTo should not reapply stale values after replacing the nested host.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindToSetsNestedPropertyOncePerValueAfterHostReplacement()
    {
        var view = new TrackingHostView { ViewModel = new() };

        using var source = new Subject<string>();
        using var subscription = source.BindTo(view, static x => x.ViewModel!.Nested.SomeText);

        foreach (var value in new[] { "Delta", "Epsilon", "Zeta" })
        {
            var replacement = new TrackingNestedValue();
            view.ViewModel!.Nested = replacement;

            source.OnNext(value);

            using (Assert.Multiple())
            {
                await Assert.That(replacement.SetCallCount).IsEqualTo(1);
                await Assert.That(replacement.SomeText).IsEqualTo(value);
            }
        }
    }

    private sealed class TrackingHostView : ReactiveObject, IViewFor<TrackingHostViewModel>
    {
        private TrackingHostViewModel? _viewModel;

        public TrackingHostViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TrackingHostViewModel?)value;
        }
    }

    private sealed class TrackingHostViewModel : ReactiveObject
    {
        private TrackingNestedValue _nested = new();

        public TrackingNestedValue Nested
        {
            get => _nested;
            set => this.RaiseAndSetIfChanged(ref _nested, value);
        }
    }

    private sealed class TrackingNestedValue : ReactiveObject
    {
        public int SetCallCount { get; private set; }

        public string? SomeText
        {
            get => field;
            set
            {
                if (value != field)
                {
                    this.RaisePropertyChanging();
                    field = value;
                    this.RaisePropertyChanged();
                    SetCallCount++;
                }
            }
        }
    }
}
