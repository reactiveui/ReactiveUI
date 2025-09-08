// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;
using DynamicData.Binding;
using NUnit.Framework;
using ReactiveUI.Testing;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests property bindings.
/// </summary>
[TestFixture]
[Apartment(ApartmentState.STA)]
public class PropertyBindingTest
{
    /// <summary>
    /// Performs a smoke test with two way binding with func converter.
    /// </summary>
    [Test]
    [UseInvariantCulture]
    public void TwoWayBindWithFuncConvertersSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        vm.JustADecimal = 123.45m;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture)));

        var disp = fixture.Bind(vm, view, x => x.JustADecimal, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, d => d.ToString(), t => decimal.TryParse(t, out var res) ? res : decimal.Zero);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.SomeTextBox.Text, Is.EqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture)));
            Assert.That(vm.JustADecimal, Is.EqualTo(123.45m));
        }

        view.SomeTextBox.Text = "567.89";
        Assert.That(vm.JustADecimal, Is.EqualTo(567.89m));

        disp?.Dispose();
        vm.JustADecimal = 0;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.JustADecimal, Is.Zero);
            Assert.That(view.SomeTextBox.Text, Is.EqualTo("567.89"));
        }
    }

    /// <summary>
    /// Performs a smoke test with two way binding.
    /// </summary>
    [Test]
    public void TwoWayBindSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        vm.Property1 = "Foo";
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.Property1));

        var disp = fixture.Bind(vm, view, x => x.Property1, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.SomeTextBox.Text, Is.EqualTo(vm.Property1));
            Assert.That(vm.Property1, Is.EqualTo("Foo"));
        }

        view.SomeTextBox.Text = "Bar";
        Assert.That(vm.Property1, Is.EqualTo("Bar"));

        disp.Dispose();
        vm.Property1 = "Baz";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.Property1, Is.EqualTo("Baz"));
            Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.Property1));
        }
    }

    /// <summary>
    /// Performs a smoke test with two way binding with a type converter.
    /// </summary>
    [Test]
    public void TypeConvertedTwoWayBindSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        vm.Property2 = 17;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.Property2.ToString()));

        var disp = fixture.Bind(vm, view, x => x.Property2, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.SomeTextBox.Text, Is.EqualTo(vm.Property2.ToString()));
            Assert.That(vm.Property2, Is.EqualTo(17));
        }

        view.SomeTextBox.Text = "42";
        Assert.That(vm.Property2, Is.EqualTo(42));

        // Bad formatting error
        view.SomeTextBox.Text = "--";
        Assert.That(vm.Property2, Is.EqualTo(42));

        disp.Dispose();
        vm.Property2 = 0;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.Property2, Is.Zero);
            Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo("0"));
        }

        vm.JustADecimal = 17.2m;
        var disp1 = fixture.Bind(vm, view, x => x.JustADecimal, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.SomeTextBox.Text, Is.EqualTo(vm.JustADecimal.ToString(CultureInfo.CurrentCulture)));
            Assert.That(vm.JustADecimal, Is.EqualTo(17.2m));
        }

        view.SomeTextBox.Text = 42.3m.ToString(CultureInfo.CurrentCulture);
        Assert.That(vm.JustADecimal, Is.EqualTo(42.3m));

        // Bad formatting.
        view.SomeTextBox.Text = "--";
        Assert.That(vm.JustADecimal, Is.EqualTo(42.3m));

        disp1.Dispose();

        vm.JustADecimal = 0;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.JustADecimal, Is.Zero);
            Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo("0"));
        }

        // Empty test
        vm.JustAInt32 = 12;
        var disp2 = fixture.Bind(vm, view, x => x.JustAInt32, x => x.SomeTextBox.Text, (IObservable<Unit>?)null, null);

        view.SomeTextBox.Text = string.Empty;
        Assert.That(vm.JustAInt32, Is.EqualTo(12));

        view.SomeTextBox.Text = "1.2";
        Assert.That(vm.JustAInt32, Is.EqualTo(12));

        view.SomeTextBox.Text = "13";
        Assert.That(vm.JustAInt32, Is.EqualTo(13));
    }

    /// <summary>
    /// Tests binding into model objects.
    /// </summary>
    [Test]
    public void BindingIntoModelObjects()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, x => x.Model!.AnotherThing, x => x.SomeTextBox.Text);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("Baz"));
    }

    /// <summary>
    /// Tests the view model nullable to view non nullable.
    /// </summary>
    [Test]
    public void ViewModelNullableToViewNonNullable()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.Bind(view.ViewModel, x => x.NullableDouble, x => x.FakeControl.JustADouble);
        Assert.That(view.FakeControl.JustADouble, Is.Zero);

        vm.NullableDouble = 4.0;
        Assert.That(view.FakeControl.JustADouble, Is.EqualTo(4.0));

        vm.NullableDouble = null;
        Assert.That(view.FakeControl.JustADouble, Is.EqualTo(4.0));

        vm.NullableDouble = 0.0;
        Assert.That(view.FakeControl.JustADouble, Is.Zero);
    }

    /// <summary>
    /// Tests the view model non-nullable to view nullable.
    /// </summary>
    [Test]
    public void ViewModelNonNullableToViewNullable()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.Bind(view.ViewModel, x => x.JustADouble, x => x.FakeControl.NullableDouble);
        Assert.That(vm.JustADouble, Is.Zero);

        view.FakeControl.NullableDouble = 4.0;
        Assert.That(vm.JustADouble, Is.EqualTo(4.0));

        view.FakeControl.NullableDouble = null;
        Assert.That(vm.JustADouble, Is.EqualTo(4.0));

        view.FakeControl.NullableDouble = 0.0;
        Assert.That(vm.JustADouble, Is.Zero);
    }

    /// <summary>
    /// Tests the view model nullable to view nullable.
    /// </summary>
    [Test]
    public void ViewModelNullableToViewNullable()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.Bind(view.ViewModel, x => x.NullableDouble, x => x.FakeControl.NullableDouble);
        Assert.That(vm.NullableDouble, Is.Null);

        view.FakeControl.NullableDouble = 4.0;
        Assert.That(vm.NullableDouble, Is.EqualTo(4.0));

        view.FakeControl.NullableDouble = null;
        Assert.That(vm.NullableDouble, Is.Null);

        view.FakeControl.NullableDouble = 0.0;
        Assert.That(vm.NullableDouble, Is.Zero);
    }

    /// <summary>
    /// Tests the view model indexer to view.
    /// </summary>
    [Test]
    public void ViewModelIndexerToView()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, x => x.SomeCollectionOfStrings[0], x => x.SomeTextBox.Text);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("Foo"));
    }

    /// <summary>
    /// Tests the view model indexer to view changes.
    /// </summary>
    [Test]
    public void ViewModelIndexerToViewChanges()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, x => x.SomeCollectionOfStrings[0], x => x.SomeTextBox.Text);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("Foo"));

        vm.SomeCollectionOfStrings[0] = "Bar";

        Assert.That(view.SomeTextBox.Text, Is.EqualTo("Bar"));
    }

    /// <summary>
    /// Tests view model indexer property to view.
    /// </summary>
    [Test]
    public void ViewModelIndexerPropertyToView()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, x => x.SomeCollectionOfStrings[0].Length, x => x.SomeTextBox.Text);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("3"));
    }

    /// <summary>
    /// Tests when OneWayBind shouldn't initially be set to null.
    /// </summary>
    [Test]
    public void OneWayBindShouldntInitiallySetToNull()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = null };

        view.OneWayBind(vm, x => x.Model!.AnotherThing, x => x.FakeControl.NullHatingString);
        Assert.That(view.FakeControl.NullHatingString, Is.EqualTo(string.Empty));

        view.ViewModel = vm;
        Assert.That(view.FakeControl.NullHatingString, Is.EqualTo(vm.Model!.AnotherThing));
    }

    /// <summary>
    /// Perform a BindTo type conversion smoke test.
    /// </summary>
    [Test]
    public void BindToTypeConversionSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = null };

        Assert.That(view.FakeControl.NullHatingString, Is.EqualTo(string.Empty));

        view.WhenAnyValue(x => x.ViewModel!.JustADouble)
            .BindTo(view, x => x.FakeControl.NullHatingString);

        view.ViewModel = vm;
        Assert.That(view.FakeControl.NullHatingString, Is.EqualTo(vm.JustADouble.ToString(CultureInfo.InvariantCulture)));
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
    [Test]
    public void TwoWayBindToSelectedItemOfItemsControl()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        view.FakeItemsControl.ItemsSource = new ObservableCollectionExtended<string>(new[] { "aaa", "bbb", "ccc" });

        view.Bind(view.ViewModel, x => x.Property1, x => x.FakeItemsControl.SelectedItem);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.FakeItemsControl.SelectedItem, Is.Null);
            Assert.That(vm.Property1, Is.Null);
        }

        view.FakeItemsControl.SelectedItem = "aaa";
        Assert.That(vm.Property1, Is.EqualTo("aaa")); // fail

        vm.Property1 = "bbb";
        Assert.That(view.FakeItemsControl.SelectedItem, Is.EqualTo("bbb"));
    }

    /// <summary>
    /// Tests that ItemControl get a DataTemplate if none is set.
    /// </summary>
    [Test]
    public void ItemsControlShouldGetADataTemplate()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        Assert.That(view.FakeItemsControl.ItemTemplate, Is.Null);
        view.OneWayBind(vm, x => x.SomeCollectionOfStrings, x => x.FakeItemsControl.ItemsSource);

        Assert.That(view.FakeItemsControl.ItemTemplate, Is.Not.Null);
    }

    /// <summary>
    /// Tests that ItemControl display member path doesn't set a DataTemplate.
    /// </summary>
    [Test]
    public void ItemsControlWithDisplayMemberPathSetShouldNotGetADataTemplate()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        view.FakeItemsControl.DisplayMemberPath = "Bla";

        Assert.That(view.FakeItemsControl.ItemTemplate, Is.Null);
        view.OneWayBind(vm, x => x.SomeCollectionOfStrings, x => x.FakeItemsControl.ItemsSource);

        Assert.That(view.FakeItemsControl.ItemTemplate, Is.Null);
    }

    /// <summary>
    /// Tests that ItemControl get a DataTemplate if none is set with BindTo.
    /// </summary>
    [Test]
    public void ItemsControlShouldGetADataTemplateInBindTo()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        Assert.That(view.FakeItemsControl.ItemTemplate, Is.Null);
        vm.WhenAnyValue(x => x.SomeCollectionOfStrings)
            .BindTo(view, v => v.FakeItemsControl.ItemsSource);

        Assert.That(view.FakeItemsControl.ItemTemplate, Is.Not.Null);

        view.WhenAnyValue(x => x.FakeItemsControl.SelectedItem)
            .BindTo(vm, x => x.Property1);
    }

    /// <summary>
    /// Tests that ItemControl OneWayBind.
    /// </summary>
    [Test]
    public void BindingToItemsControl()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(view.ViewModel, x => x.SomeCollectionOfStrings, x => x.FakeItemsControl.ItemsSource);

        var itemsSourceValue = (IList)view.FakeItemsControl.ItemsSource;
        Assert.That(itemsSourceValue.OfType<string>().Count() > 1, Is.True);
    }

    /// <summary>
    /// Tests OneWayBind and a converter.
    /// </summary>
    [Test]
    public void OneWayBindConverter()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();
        fixture.OneWayBind(vm, view, x => x.JustABoolean, x => x.SomeTextBox.IsEnabled, s => s);
        Assert.That(view.SomeTextBox.IsEnabled, Is.False);
    }

    /// <summary>
    /// Tests OneWayBind and a converter with a null starting value, and tests it against a non-null value.
    /// </summary>
    [Test]
    public void OneWayBindWithNullStartingValueToNonNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(vm, x => x.Property1, x => x.SomeTextBox.Text);

        vm.Property1 = "Baz";

        Assert.That(view.SomeTextBox.Text, Is.EqualTo("Baz"));
    }

    /// <summary>
    /// Tests OneWayBind and a converter with a non-null starting value, and tests it against a null value.
    /// </summary>
    [Test]
    public void OneWayBindWithNonNullStartingValueToNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        vm.Property1 = "Baz";

        view.OneWayBind(vm, x => x.Property1, x => x.SomeTextBox.Text);

        vm.Property1 = null;

        Assert.That(string.IsNullOrEmpty(view.SomeTextBox.Text), Is.True);
    }

    /// <summary>
    /// Tests OneWayBind and a converter with a non-null starting value, and tests it against a non-null value.
    /// </summary>
    [Test]
    public void OneWayBindWithSelectorAndNonNullStartingValueToNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.OneWayBind(vm, x => x.Model, x => x.SomeTextBox.Text, x => x?.AnotherThing);

        vm.Model = null;

        Assert.That(string.IsNullOrEmpty(view.SomeTextBox.Text), Is.True);
    }

    /// <summary>
    /// Tests OneWayBind initial view model should be garbage collected when overwritten.
    /// </summary>
    [Test]
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

        Assert.That(weakRef.IsAlive, Is.False);
    }

    /// <summary>
    /// Tests BindTo  with a null starting value, and tests it against a non-null value.
    /// </summary>
    [Test]
    public void BindToWithNullStartingValueToNonNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        view.WhenAnyValue(x => x.ViewModel!.Property1)
            .BindTo(view, x => x.SomeTextBox.Text);

        vm.Property1 = "Baz";

        Assert.That(view.SomeTextBox.Text, Is.EqualTo("Baz"));
    }

    /// <summary>
    /// Tests BindTo  with a non-null starting value, and tests it against a null value.
    /// </summary>
    [Test]
    public void BindToWithNonNullStartingValueToNullValue()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        vm.Property1 = "Baz";

        view.WhenAnyValue(x => x.ViewModel!.Property1)
            .BindTo(view, x => x.SomeTextBox.Text);

        vm.Property1 = null;

        Assert.That(string.IsNullOrEmpty(view.SomeTextBox.Text), Is.True);
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
    [Test]
    public void BindWithFuncShouldWorkAsExtensionMethodSmokeTest()
    {
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };

        vm.JustADecimal = 123.45m;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture)));

        view.Bind(vm, x => x.JustADecimal, x => x.SomeTextBox.Text, d => d.ToString(CultureInfo.InvariantCulture), t => decimal.TryParse(t, out var res) ? res : 0m);

        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        vm.JustADecimal = 1.0M;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.0"));

        vm.JustADecimal = 2.0M;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.0"));

        view.SomeTextBox.Text = "3.0";
        Assert.That(vm.JustADecimal, Is.EqualTo(3.0M));
    }

    /// <summary>
    /// Tests that bind initial view model should be garbage collected when overwritten.
    /// </summary>
    [Test]
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

        Assert.That(weakRef.IsAlive, Is.False);
    }

    [Test]
    public void OneWayBindWithHintTest()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView() { ViewModel = vm };
        var fixture = new PropertyBinderImplementation();

        fixture.OneWayBind(vm, view, vm => vm.JustABoolean, v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis);
        Assert.That(view.SomeTextBox.Visibility, Is.EqualTo(System.Windows.Visibility.Visible));

        vm.JustABoolean = true;
        Assert.That(view.SomeTextBox.Visibility, Is.EqualTo(System.Windows.Visibility.Collapsed));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
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
    public void BindToWithHintTest()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var obs = vm.WhenAnyValue(x => x.JustABoolean);
        var a = new PropertyBinderImplementation().BindTo(obs, view, v => v.SomeTextBox.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(dis);
        Assert.That(view.SomeTextBox.Visibility, Is.EqualTo(System.Windows.Visibility.Visible));

        vm.JustABoolean = true;
        Assert.That(view.SomeTextBox.Visibility, Is.EqualTo(System.Windows.Visibility.Collapsed));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToView()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADecimal = 123.45m;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture)));

        view.Bind(vm, x => x.JustADecimal, x => x.SomeTextBox.Text, update.AsObservable(), d => d.ToString(CultureInfo.InvariantCulture), t => decimal.TryParse(t, out var res) ? res : decimal.Zero, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADecimal = 1.0M;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.0"));

        vm.JustADecimal = 2.0M;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.0"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.0"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.0";
        Assert.That(vm.JustADecimal, Is.EqualTo(3.0M));

        view.SomeTextBox.Text = "4.0";
        Assert.That(vm.JustADecimal, Is.EqualTo(4.0M));

        // test forward bind to ensure trigger is still honoured.
        vm.JustADecimal = 2.0M;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4.0"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.0"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithDecimalConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADecimal = 123.45m;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture)));

        var decimalToStringTypeConverter = new DecimalToStringTypeConverter();

        view.Bind(vm, x => x.JustADecimal, x => x.SomeTextBox.Text, update.AsObservable(), 2, decimalToStringTypeConverter, decimalToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADecimal = 1.0M;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        vm.JustADecimal = 2.0M;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.That(vm.JustADecimal, Is.EqualTo(3.0M));

        view.SomeTextBox.Text = "4.00";
        Assert.That(vm.JustADecimal, Is.EqualTo(4.0M));

        // test forward bind to ensure trigger is still honoured.
        vm.JustADecimal = 2.0M;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableDecimalConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullDecimal = 123.45m;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustANullDecimal.Value.ToString(CultureInfo.InvariantCulture)));

        var decimalToStringTypeConverter = new NullableDecimalToStringTypeConverter();

        view.Bind(vm, x => x.JustANullDecimal, x => x.SomeTextBox.Text, update.AsObservable(), 2, decimalToStringTypeConverter, decimalToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullDecimal = 1.0M;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        vm.JustANullDecimal = 2.0M;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.That(vm.JustANullDecimal, Is.EqualTo(3.0M));

        // test non numerical
        view.SomeTextBox.Text = "ad3";
        Assert.That(vm.JustANullDecimal, Is.EqualTo(3.0M));

        view.SomeTextBox.Text = "4.00";
        Assert.That(vm.JustANullDecimal, Is.EqualTo(4.0M));

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullDecimal = 2.0M;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewToViewModel()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADecimal = 123.45m;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture)));

        view.Bind(vm, x => x.JustADecimal, x => x.SomeTextBox.Text, update.AsObservable(), d => d.ToString(CultureInfo.InvariantCulture), t => decimal.TryParse(t, out var res) ? res : decimal.Zero, TriggerUpdate.ViewToViewModel).DisposeWith(dis);

        view.SomeTextBox.Text = "1.0";

        // value should have pre bind value
        Assert.That(vm.JustADecimal, Is.EqualTo(123.45m));

        // trigger UI update
        update.OnNext(true);
        Assert.That(vm.JustADecimal, Is.EqualTo(1.0m));

        view.SomeTextBox.Text = "2.0";
        Assert.That(vm.JustADecimal, Is.EqualTo(1.0m));

        update.OnNext(true);
        Assert.That(vm.JustADecimal, Is.EqualTo(2.0m));

        // test reverse bind no trigger required
        vm.JustADecimal = 3.0m;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("3.0"));

        vm.JustADecimal = 4.0m;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4.0"));

        // test forward bind to ensure trigger is still honoured.
        view.SomeTextBox.Text = "2.0";
        Assert.That(vm.JustADecimal, Is.EqualTo(4.0m));

        update.OnNext(true);
        Assert.That(vm.JustADecimal, Is.EqualTo(2.0m));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithDoubleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADouble = 123.45;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustADouble.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new DoubleToStringTypeConverter();

        view.Bind(vm, x => x.JustADouble, x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADouble = 1.0;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        vm.JustADouble = 2.0;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.That(vm.JustADouble, Is.EqualTo(3.0));

        view.SomeTextBox.Text = "4.00";
        Assert.That(vm.JustADouble, Is.EqualTo(4.0));

        // test forward bind to ensure trigger is still honoured.
        vm.JustADouble = 2.0;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableDoubleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullDouble = 123.45;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustANullDouble.Value.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new NullableDoubleToStringTypeConverter();

        view.Bind(vm, x => x.JustANullDouble, x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullDouble = 1.0;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        vm.JustANullDouble = 2.0;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.That(vm.JustANullDouble, Is.EqualTo(3.0));

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        Assert.That(vm.JustANullDouble, Is.EqualTo(3.0));

        view.SomeTextBox.Text = "4.00";
        Assert.That(vm.JustANullDouble, Is.EqualTo(4.0));

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullDouble = 2.0;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithDoubleConverterNoRound()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustADouble = 123.45;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustADouble.ToString(CultureInfo.InvariantCulture)));

        view.Bind(vm, x => x.JustADouble, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADouble = 1.0;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        vm.JustADouble = 2.0;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.That(vm.JustADouble, Is.EqualTo(3.0));

        view.SomeTextBox.Text = "4";
        Assert.That(vm.JustADouble, Is.EqualTo(4.0));

        // test forward bind to ensure trigger is still honoured.
        vm.JustADouble = 2.0;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithSingleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustASingle = 123.45f;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustASingle.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new SingleToStringTypeConverter();

        view.Bind(vm, x => x.JustASingle, x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustASingle = 1.0f;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        vm.JustASingle = 2.0f;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.That(vm.JustASingle, Is.EqualTo(3.0f));

        view.SomeTextBox.Text = "4.00";
        Assert.That(vm.JustASingle, Is.EqualTo(4.0f));

        // test forward bind to ensure trigger is still honoured.
        vm.JustASingle = 2.0f;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableSingleConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullSingle = 123.45f;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustANullSingle.Value.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new NullableSingleToStringTypeConverter();

        view.Bind(vm, x => x.JustANullSingle, x => x.SomeTextBox.Text, update.AsObservable(), 2, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullSingle = 1.0f;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        vm.JustANullSingle = 2.0f;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        Assert.That(vm.JustANullSingle, Is.EqualTo(3.0f));

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        Assert.That(vm.JustANullSingle, Is.EqualTo(3.0f));

        view.SomeTextBox.Text = "4.00";
        Assert.That(vm.JustANullSingle, Is.EqualTo(4.0f));

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullSingle = 2.0f;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4.00"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2.00"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithSingleConverterNoRound()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustASingle = 123.45f;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustASingle.ToString(CultureInfo.InvariantCulture)));

        view.Bind(vm, x => x.JustASingle, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustASingle = 1.0f;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123.45"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        vm.JustASingle = 2.0f;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.That(vm.JustASingle, Is.EqualTo(3.0f));

        view.SomeTextBox.Text = "4";
        Assert.That(vm.JustASingle, Is.EqualTo(4.0f));

        // test forward bind to ensure trigger is still honoured.
        vm.JustASingle = 2.0f;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithByteConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAByte = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustAByte.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new ByteToStringTypeConverter();

        view.Bind(vm, x => x.JustAByte, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAByte = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        vm.JustAByte = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.That(vm.JustAByte, Is.EqualTo(3));

        view.SomeTextBox.Text = "004";
        Assert.That(vm.JustAByte, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustAByte = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("004"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableByteConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullByte = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustANullByte.Value.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new NullableByteToStringTypeConverter();

        view.Bind(vm, x => x.JustANullByte, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullByte = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        vm.JustANullByte = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.That(vm.JustANullByte!.Value, Is.EqualTo(3));

        // test non numerical value
        view.SomeTextBox.Text = "ad4";
        Assert.That(vm.JustANullByte!.Value, Is.EqualTo(3));

        view.SomeTextBox.Text = "004";
        Assert.That(vm.JustANullByte!.Value, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullByte = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("004"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithByteConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAByte = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustAByte.ToString(CultureInfo.InvariantCulture)));

        view.Bind(vm, x => x.JustAByte, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAByte = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        vm.JustAByte = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.That(vm.JustAByte, Is.EqualTo(3));

        view.SomeTextBox.Text = "4";
        Assert.That(vm.JustAByte, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustAByte = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithShortConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt16 = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustAInt16.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new ShortToStringTypeConverter();

        view.Bind(vm, x => x.JustAInt16, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt16 = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        vm.JustAInt16 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.That(vm.JustAInt16, Is.EqualTo(3));

        view.SomeTextBox.Text = "004";
        Assert.That(vm.JustAInt16, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt16 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("004"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableShortConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullInt16 = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustANullInt16.Value.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new NullableShortToStringTypeConverter();

        view.Bind(vm, x => x.JustANullInt16, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullInt16 = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        vm.JustANullInt16 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.That(vm.JustANullInt16!.Value, Is.EqualTo(3));

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        Assert.That(vm.JustANullInt16!.Value, Is.EqualTo(3));

        view.SomeTextBox.Text = "004";
        Assert.That(vm.JustANullInt16!.Value, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullInt16 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("004"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithShortConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt16 = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustAInt16.ToString(CultureInfo.InvariantCulture)));

        view.Bind(vm, x => x.JustAInt16, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt16 = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        vm.JustAInt16 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.That(vm.JustAInt16, Is.EqualTo(3));

        view.SomeTextBox.Text = "4";
        Assert.That(vm.JustAInt16, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt16 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithIntegerConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt32 = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustAInt32.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new IntegerToStringTypeConverter();

        view.Bind(vm, x => x.JustAInt32, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt32 = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        vm.JustAInt32 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.That(vm.JustAInt32, Is.EqualTo(3));

        view.SomeTextBox.Text = "004";
        Assert.That(vm.JustAInt32, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt32 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("004"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableIntegerConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullInt32 = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustANullInt32!.Value.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new NullableIntegerToStringTypeConverter();

        view.Bind(vm, x => x.JustANullInt32, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullInt32 = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        vm.JustANullInt32 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.That(vm.JustANullInt32, Is.EqualTo(3));

        // test if the binding handles a non number
        view.SomeTextBox.Text = "3a4";
        Assert.That(vm.JustANullInt32, Is.EqualTo(3));

        view.SomeTextBox.Text = "004";
        Assert.That(vm.JustANullInt32, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullInt32 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("004"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithIntegerConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt32 = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustAInt32.ToString(CultureInfo.InvariantCulture)));

        view.Bind(vm, x => x.JustAInt32, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt32 = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        vm.JustAInt32 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.That(vm.JustAInt32, Is.EqualTo(3));

        view.SomeTextBox.Text = "4";
        Assert.That(vm.JustAInt32, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt32 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithLongConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt64 = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustAInt64.ToString(CultureInfo.InvariantCulture)));

        var xToStringTypeConverter = new LongToStringTypeConverter();

        view.Bind(vm, x => x.JustAInt64, x => x.SomeTextBox.Text, update.AsObservable(), 3, xToStringTypeConverter, xToStringTypeConverter, TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt64 = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        vm.JustAInt64 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("001"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        Assert.That(vm.JustAInt64, Is.EqualTo(3));

        view.SomeTextBox.Text = "004";
        Assert.That(vm.JustAInt64, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt64 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("004"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("002"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }

    [Test]
    public void BindWithFuncToTriggerUpdateTestViewModelToViewWithLongConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt64 = 123;
        Assert.That(view.SomeTextBox.Text, Is.Not.EqualTo(vm.JustAInt64.ToString(CultureInfo.InvariantCulture)));

        view.Bind(vm, x => x.JustAInt64, x => x.SomeTextBox.Text, update.AsObservable(), null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt64 = 1;

        // value should have pre bind value
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("123"));

        // trigger UI update
        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        vm.JustAInt64 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("1"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        Assert.That(vm.JustAInt64, Is.EqualTo(3));

        view.SomeTextBox.Text = "4";
        Assert.That(vm.JustAInt64, Is.EqualTo(4));

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt64 = 2;
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("4"));

        update.OnNext(true);
        Assert.That(view.SomeTextBox.Text, Is.EqualTo("2"));

        dis.Dispose();
        Assert.That(dis.IsDisposed, Is.True);
    }
}
