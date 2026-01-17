// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.PropertyBindings;

public class PropertyBindingMixinsTests
{
    [Test]
    public async Task Bind_AfterDispose_StopsUpdating()
    {
        var viewModel = new TestViewModel { Name = "Initial" };
        var view = new TestView { ViewModel = viewModel };

        using (var binding = view.Bind(viewModel, vm => vm.Name, v => v.NameText))
        {
            await Assert.That(view.NameText).IsEqualTo("Initial");
        }

        viewModel.Name = "Changed";
        await Assert.That(view.NameText).IsEqualTo("Initial");
    }

    [Test]
    public async Task Bind_WithBasicProperties_UpdatesBothDirections()
    {
        var viewModel = new TestViewModel { Name = "Initial" };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.Bind(viewModel, vm => vm.Name, v => v.NameText);

        // VM to View
        await Assert.That(view.NameText).IsEqualTo("Initial");

        // View to VM
        view.NameText = "Updated";
        await Assert.That(viewModel.Name).IsEqualTo("Updated");

        // VM to View again
        viewModel.Name = "Changed";
        await Assert.That(view.NameText).IsEqualTo("Changed");
    }

    [Test]
    public async Task Bind_WithConverters_ConvertsValues()
    {
        var viewModel = new TestViewModel { Count = 42 };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.Bind(
            viewModel,
            vm => vm.Count,
            v => v.NameText,
            count => $"Count: {count}",
            text => int.TryParse(text?.Replace("Count: ", string.Empty), out var n) ? n : 0);

        await Assert.That(view.NameText).IsEqualTo("Count: 42");

        view.NameText = "Count: 100";
        await Assert.That(viewModel.Count).IsEqualTo(100);
    }

    [Test]
    public async Task Bind_WithNullViewModel_DoesNotThrow()
    {
        var view = new TestView();

        using var binding = view.Bind<TestViewModel, TestView, string?, string?>(
            null,
            vm => vm.Name!,
            v => v.NameText!);

        // Should not throw, but won't update anything
        await Assert.That(view.NameText).IsNull();
    }

    [Test]
    public async Task Bind_WithSignalViewUpdate_UpdatesOnSignal()
    {
        var viewModel = new TestViewModel { Name = "Initial" };
        var view = new TestView { ViewModel = viewModel };
        var signal = new Subject<Unit>();

        using var binding = view.Bind(
            viewModel,
            vm => vm.Name,
            v => v.NameText,
            signal);

        await Assert.That(view.NameText).IsEqualTo("Initial");

        // Change view property but don't signal yet
        view.NameText = "Updated";
        await Assert.That(viewModel.Name).IsEqualTo("Initial");

        // Signal the update
        signal.OnNext(Unit.Default);
        await Assert.That(viewModel.Name).IsEqualTo("Updated");
    }

    [Test]
    public async Task Bind_WithTypeConverter_ConvertsTypes()
    {
        var viewModel = new TestViewModel { Count = 42 };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.Bind(
            viewModel,
            vm => vm.Count,
            v => v.NameText);

        // Should convert int to string automatically
        await Assert.That(view.NameText).IsEqualTo("42");

        view.NameText = "100";
        await Assert.That(viewModel.Count).IsEqualTo(100);
    }

    [Test]
    public async Task BindTo_AfterDispose_StopsUpdating()
    {
        var target = new TestViewModel();
        var source = new BehaviorSubject<string>("Initial");

        using (var binding = source.BindTo(target, t => t.Name))
        {
            await Assert.That(target.Name).IsEqualTo("Initial");
        }

        source.OnNext("Updated");
        await Assert.That(target.Name).IsEqualTo("Initial");
    }

    [Test]
    public async Task BindTo_UpdatesTargetProperty()
    {
        var target = new TestViewModel();
        var source = new BehaviorSubject<string>("Initial");

        using var binding = source.BindTo(target, t => t.Name);

        await Assert.That(target.Name).IsEqualTo("Initial");

        source.OnNext("Updated");
        await Assert.That(target.Name).IsEqualTo("Updated");

        source.OnNext("Final");
        await Assert.That(target.Name).IsEqualTo("Final");
    }

    [Test]
    public async Task BindTo_WithConverter_ConvertsValue()
    {
        var target = new TestViewModel();
        var source = new BehaviorSubject<int>(42);

        using var binding = source.BindTo(
            target,
            t => t.Name,
            vmToViewConverterOverride: new FuncBindingTypeConverter<int, string>(i => $"Number: {i}"));

        await Assert.That(target.Name).IsEqualTo("Number: 42");

        source.OnNext(100);
        await Assert.That(target.Name).IsEqualTo("Number: 100");
    }

    [Test]
    public async Task OneWayBind_AfterDispose_StopsUpdating()
    {
        var viewModel = new TestViewModel { Name = "Initial" };
        var view = new TestView { ViewModel = viewModel };

        using (var binding = view.OneWayBind(viewModel, vm => vm.Name, v => v.NameText))
        {
            await Assert.That(view.NameText).IsEqualTo("Initial");
        }

        viewModel.Name = "Changed";
        await Assert.That(view.NameText).IsEqualTo("Initial");
    }

    [Test]
    public async Task OneWayBind_UpdatesViewOnly()
    {
        var viewModel = new TestViewModel { Name = "Initial" };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.OneWayBind(viewModel, vm => vm.Name, v => v.NameText);

        // VM to View
        await Assert.That(view.NameText).IsEqualTo("Initial");

        viewModel.Name = "Updated";
        await Assert.That(view.NameText).IsEqualTo("Updated");

        // View to VM should not work
        view.NameText = "ViewChange";
        await Assert.That(viewModel.Name).IsEqualTo("Updated");
    }

    [Test]
    public async Task OneWayBind_WithNullViewModel_DoesNotThrow()
    {
        var view = new TestView();

        using var binding = view.OneWayBind<TestViewModel, TestView, string?, string?>(
            null,
            vm => vm.Name!,
            v => v.NameText!);

        await Assert.That(view.NameText).IsNull();
    }

    [Test]
    public async Task OneWayBind_WithSelector_TransformsValue()
    {
        var viewModel = new TestViewModel { Count = 42 };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.OneWayBind(
            viewModel,
            vm => vm.Count,
            v => v.NameText,
            count => $"The count is: {count}");

        await Assert.That(view.NameText).IsEqualTo("The count is: 42");

        viewModel.Count = 100;
        await Assert.That(view.NameText).IsEqualTo("The count is: 100");
    }

    [Test]
    public async Task OneWayBind_WithTypeConverter_ConvertsTypes()
    {
        var viewModel = new TestViewModel { Count = 42 };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.OneWayBind(
            viewModel,
            vm => vm.Count,
            v => v.NameText);

        await Assert.That(view.NameText).IsEqualTo("42");

        viewModel.Count = 100;
        await Assert.That(view.NameText).IsEqualTo("100");
    }

    private class FuncBindingTypeConverter<TFrom, TTo> : IBindingTypeConverter
    {
        private readonly Func<TFrom, TTo> _converter;

        public FuncBindingTypeConverter(Func<TFrom, TTo> converter) => _converter = converter;

        public Type FromType => typeof(TFrom);

        public Type ToType => typeof(TTo);

        public int GetAffinityForObjects() => 100;

        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            if (from is TFrom typedFrom)
            {
                result = _converter(typedFrom)!;
                return true;
            }

            result = default(TTo);
            return false;
        }
    }

    private class TestView : ReactiveObject, IViewFor<TestViewModel>
    {
        private string? _nameText;
        private TestViewModel? _viewModel;

        public string? NameText
        {
            get => _nameText;
            set => this.RaiseAndSetIfChanged(ref _nameText, value);
        }

        public TestViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    private class TestViewModel : ReactiveObject
    {
        private int _count;
        private string? _name;

        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}
