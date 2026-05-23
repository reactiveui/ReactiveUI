// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Globalization;

using DynamicData.Binding;

using ReactiveUI.Builder;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using ReactiveUI.Tests.Xaml.Utilities;
using Splat;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests property bindings.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public partial class PropertyBindingTest
{
    private const decimal InitialDecimal = 123.45m;
    private const decimal SecondDecimal = 567.89m;
    private const decimal DecimalOne = 1.0M;
    private const decimal DecimalTwo = 2.0M;
    private const decimal DecimalThree = 3.0M;
    private const decimal DecimalFour = 4.0M;
    private const double InitialDouble = 123.45;
    private const double DoubleOne = 1.0;
    private const double DoubleTwo = 2.0;
    private const double DoubleThree = 3.0;
    private const double DoubleFour = 4.0;
    private const float InitialSingle = 123.45f;
    private const float SingleOne = 1.0f;
    private const float SingleTwo = 2.0f;
    private const float SingleThree = 3.0f;
    private const float SingleFour = 4.0f;
    private const int InitialIntegral = 123;
    private const int IntegralTwo = 2;
    private const int IntegralThree = 3;
    private const int IntegralFour = 4;
    private const int FormatHint = 3;
    private const int RoundingHint = 2;

    private const string InitialNumericText = "123.45";
    private const string AlphaValue = "Alpha";
    private const string GammaValue = "Gamma";

    private static readonly string[] _itemsControlSource = ["aaa", "bbb", "ccc"];

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

        vm.JustADecimal = InitialDecimal;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        var disp = fixture.Bind(
            vm,
            view,
            static x => x.JustADecimal,
            static x => x.SomeTextBox.Text,
            (IObservable<Unit>?)null,
            static d => d.ToString(CultureInfo.CurrentCulture),
            static t => decimal.TryParse(t, out var res) ? res : decimal.Zero);

        using (Assert.Multiple())
        {
            await Assert.That(view.SomeTextBox.Text).IsEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));
            await Assert.That(vm.JustADecimal).IsEqualTo(InitialDecimal);
        }

        view.SomeTextBox.Text = "567.89";
        await Assert.That(vm.JustADecimal).IsEqualTo(SecondDecimal);

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
        const int initialProperty2 = 17;
        const int parsedProperty2 = 42;
        const decimal initialJustADecimal = 17.2m;
        const decimal parsedJustADecimal = 42.3m;
        const int initialJustAInt32 = 12;
        const int parsedJustAInt32 = 13;

        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        vm.Property2 = initialProperty2;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.Property2.ToString());

        var disp = fixture.Bind(vm, view, static x => x.Property2, static x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        using (Assert.Multiple())
        {
            await Assert.That(view.SomeTextBox.Text).IsEqualTo(vm.Property2.ToString());
            await Assert.That(vm.Property2).IsEqualTo(initialProperty2);
        }

        view.SomeTextBox.Text = "42";
        await Assert.That(vm.Property2).IsEqualTo(parsedProperty2);

        // Bad formatting error
        view.SomeTextBox.Text = "--";
        await Assert.That(vm.Property2).IsEqualTo(parsedProperty2);

        disp.Dispose();
        vm.Property2 = 0;

        using (Assert.Multiple())
        {
            await Assert.That(vm.Property2).IsEqualTo(0);
            await Assert.That(view.SomeTextBox.Text).IsNotEqualTo("0");
        }

        vm.JustADecimal = initialJustADecimal;
        var disp1 = fixture.Bind(vm, view, static x => x.JustADecimal, static x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        using (Assert.Multiple())
        {
            await Assert.That(view.SomeTextBox.Text).IsEqualTo(vm.JustADecimal.ToString(CultureInfo.CurrentCulture));
            await Assert.That(vm.JustADecimal).IsEqualTo(initialJustADecimal);
        }

        view.SomeTextBox.Text = parsedJustADecimal.ToString(CultureInfo.CurrentCulture);
        await Assert.That(vm.JustADecimal).IsEqualTo(parsedJustADecimal);

        // Bad formatting.
        view.SomeTextBox.Text = "--";
        await Assert.That(vm.JustADecimal).IsEqualTo(parsedJustADecimal);

        disp1.Dispose();

        vm.JustADecimal = 0;

        using (Assert.Multiple())
        {
            await Assert.That(vm.JustADecimal).IsEqualTo(0);
            await Assert.That(view.SomeTextBox.Text).IsNotEqualTo("0");
        }

        // Empty test
        vm.JustAInt32 = initialJustAInt32;
        _ = fixture.Bind(vm, view, static x => x.JustAInt32, static x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        view.SomeTextBox.Text = string.Empty;
        await Assert.That(vm.JustAInt32).IsEqualTo(initialJustAInt32);

        view.SomeTextBox.Text = "1.2";
        await Assert.That(vm.JustAInt32).IsEqualTo(initialJustAInt32);

        view.SomeTextBox.Text = "13";
        await Assert.That(vm.JustAInt32).IsEqualTo(parsedJustAInt32);
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

        vm.NullableDouble = DoubleFour;
        await Assert.That(view.FakeControl.JustADouble).IsEqualTo(DoubleFour);

        vm.NullableDouble = null;
        await Assert.That(view.FakeControl.JustADouble).IsEqualTo(DoubleFour);

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

        view.FakeControl.NullableDouble = DoubleFour;
        await Assert.That(vm.JustADouble).IsEqualTo(DoubleFour);

        view.FakeControl.NullableDouble = null;
        await Assert.That(vm.JustADouble).IsEqualTo(DoubleFour);

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

        view.FakeControl.NullableDouble = DoubleFour;
        await Assert.That(vm.NullableDouble).IsEqualTo(DoubleFour);

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
        view.FakeItemsControl.ItemsSource = new ObservableCollectionExtended<string>(_itemsControlSource);

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
    /// Tests that Bind two-way selected item of ComboBox updates in both directions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TwoWayBindToSelectedItemOfComboBox()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        view.ComboBoxSelection.ItemsSource = new ObservableCollectionExtended<string>(new[] { "aaa", "bbb", "ccc" });

        view.Bind(view.ViewModel, static x => x.Property1, static x => x.ComboBoxSelection.SelectedItem);

        view.ComboBoxSelection.SelectedItem = "aaa";
        await Assert.That(vm.Property1).IsEqualTo("aaa");

        vm.Property1 = "bbb";
        await Assert.That(view.ComboBoxSelection.SelectedItem).IsEqualTo("bbb");
    }

    /// <summary>
    /// Tests that view model updates from a background thread are marshalled before setting WPF controls.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelToViewBindingFromBackgroundThreadDoesNotTouchWpfControlDirectly()
    {
        using var locator = new ModernDependencyResolver();
        locator.CreateReactiveUIBuilder().WithWpf().Build();

        using (locator.WithResolver())
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView { ViewModel = vm };
            using var binding = view.Bind(view.ViewModel, static x => x.Property1, static x => x.SomeTextBox.Text);

            Exception? thrown = null;
            await Task.Run(() =>
            {
                try
                {
                    vm.Property1 = "background update";
                }
                catch (Exception ex)
                {
                    thrown = ex;
                }
            });

            DispatcherUtilities.DoEvents();

            using (Assert.Multiple())
            {
                await Assert.That(thrown).IsNull();
                await Assert.That(view.SomeTextBox.Text).IsEqualTo("background update");
            }
        }
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

        var (_, weakRef) = GetWeakReference();

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

        const Func<string?, string?> nullFunc = null!;

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

        vm.JustADecimal = InitialDecimal;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustADecimal, static x => x.SomeTextBox.Text, static d => d.ToString(CultureInfo.InvariantCulture), static t => decimal.TryParse(t, out var res) ? res : 0m);

        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        vm.JustADecimal = DecimalOne;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.0");

        vm.JustADecimal = DecimalTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.0");

        view.SomeTextBox.Text = "3.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalThree);
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

        var (_, weakRef) = GetWeakReference();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        await Assert.That(weakRef.IsAlive).IsFalse();
    }

    /// <summary>
    /// Verifies one way bind with hint test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBindWithHintTest()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        fixture.OneWayBind(vm, view, static vm => vm.JustABoolean, static v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis);
        await Assert.That(view.SomeTextBox.Visibility).IsEqualTo(System.Windows.Visibility.Visible);

        vm.JustABoolean = true;
        await Assert.That(view.SomeTextBox.Visibility).IsEqualTo(System.Windows.Visibility.Collapsed);

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies one way bind with hint test dispose with failure.
    /// </summary>
    [Test]
    public void OneWayBindWithHintTestDisposeWithFailure()
    {
        CompositeDisposable? dis = null;
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        Assert.Throws<ArgumentNullException>(() => fixture.OneWayBind(vm, view, vm => vm.JustABoolean, v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis!));
    }

    /// <summary>
    /// Verifies bind to with hint test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindToWithHintTest()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var obs = vm.WhenAnyValue(static x => x.JustABoolean);
        _ = new PropertyBinderImplementation().BindTo(obs, view, static v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis);
        await Assert.That(view.SomeTextBox.Visibility).IsEqualTo(System.Windows.Visibility.Visible);

        vm.JustABoolean = true;
        await Assert.That(view.SomeTextBox.Visibility).IsEqualTo(System.Windows.Visibility.Collapsed);

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
        const int expectedDistinctSetCount = 3;

        var view = new TrackingHostView { ViewModel = new() };

        using var source = new Subject<string>();
        using var subscription = source.BindTo(view, static x => x.ViewModel!.Nested.SomeText);

        source.OnNext(AlphaValue);
        source.OnNext(AlphaValue);
        source.OnNext(AlphaValue);
        source.OnNext("Beta");
        source.OnNext("Beta");
        source.OnNext("Beta");
        source.OnNext(GammaValue);
        source.OnNext(GammaValue);
        source.OnNext(GammaValue);

        var nested = view.ViewModel!.Nested;

        using (Assert.Multiple())
        {
            await Assert.That(nested.SetCallCount).IsEqualTo(expectedDistinctSetCount);
            await Assert.That(nested.SomeText).IsEqualTo(GammaValue);
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

    /// <summary>
    /// A host view used to verify nested binding update tracking.
    /// </summary>
    private sealed class TrackingHostView : ReactiveObject, IViewFor<TrackingHostViewModel>
    {
        /// <summary>
        /// Backing field for the <see cref="ViewModel"/> property.
        /// </summary>
        private TrackingHostViewModel? _viewModel;

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public TrackingHostViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TrackingHostViewModel?)value;
        }
    }

    /// <summary>
    /// A host view model that exposes a nested reactive value.
    /// </summary>
    private sealed class TrackingHostViewModel : ReactiveObject
    {
        /// <summary>
        /// Backing field for the <see cref="Nested"/> property.
        /// </summary>
        private TrackingNestedValue _nested = new();

        /// <summary>
        /// Gets or sets the nested value.
        /// </summary>
        public TrackingNestedValue Nested
        {
            get => _nested;
            set => this.RaiseAndSetIfChanged(ref _nested, value);
        }
    }

    /// <summary>
    /// A nested reactive value that counts how many times its text is set.
    /// </summary>
    private sealed class TrackingNestedValue : ReactiveObject
    {
        /// <summary>
        /// Gets the number of times <see cref="SomeText"/> has been set.
        /// </summary>
        public int SetCallCount { get; private set; }

        /// <summary>
        /// Gets or sets some text whose set count is tracked.
        /// </summary>
        public string? SomeText
        {
            get => field;
            set
            {
                if (value == field)
                {
                    return;
                }

                this.RaisePropertyChanging();
                field = value;
                this.RaisePropertyChanged();
                SetCallCount++;
            }
        }
    }
}
