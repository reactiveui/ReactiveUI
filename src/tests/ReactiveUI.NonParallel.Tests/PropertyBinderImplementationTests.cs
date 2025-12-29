// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for PropertyBinderImplementation covering edge cases and error paths.
/// These tests use Locator.CurrentMutable (static state) so must run in NonParallel test suite.
/// </summary>
public class PropertyBinderImplementationTests
{
    [Test]
    public async Task Bind_WithNullReturningConverter_HandlesNullTmpValue()
    {
        var viewModel = new TestViewModel { Name = "Test" };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var nullConverter = new NullReturningConverter();
        using var binding = fixture.Bind(
            viewModel,
            view,
            vm => vm.Name,
            v => v.NameText,
            (IObservable<Unit>?)null,
            null,
            nullConverter,
            nullConverter);

        // Should handle null tmp value from converter
        await Assert.That(view.NameText).IsNull();
    }

    [Test]
    public async Task OneWayBind_WithFailingConverter_DoesNotUpdateView()
    {
        var viewModel = new TestViewModel { Name = "Initial" };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var failingConverter = new FailingConverter();
        using var binding = fixture.OneWayBind(
            viewModel,
            view,
            vm => vm.Name,
            v => v.NameText,
            vmToViewConverterOverride: failingConverter);

        var initialText = view.NameText;

        viewModel.Name = "Changed";

        // View should not update when converter fails
        await Assert.That(view.NameText).IsEqualTo(initialText);
    }

    [Test]
    public async Task OneWayBind_WithBindingHookReturningFalse_ReturnsEmptyObservable()
    {
        var viewModel = new TestViewModel { Name = "Test" };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var hook = new RejectingBindingHook();
        var previousHooks = Locator.Current.GetServices<IPropertyBindingHook>().ToList();
        Locator.CurrentMutable.RegisterConstant<IPropertyBindingHook>(hook);
        try
        {
            using var binding = fixture.OneWayBind(viewModel, view, vm => vm.Name, v => v.NameText);

            // Binding should be created but produce no values
            viewModel.Name = "Changed";
            await Assert.That(view.NameText).IsNotEqualTo("Changed");
        }
        finally
        {
            Locator.CurrentMutable.UnregisterAll<IPropertyBindingHook>();
            foreach (var previousHook in previousHooks)
            {
                Locator.CurrentMutable.RegisterConstant(previousHook);
            }
        }
    }

    [Test]
    public async Task OneWayBind_WithSelector_AndBindingHookReturningFalse_ReturnsEmptyObservable()
    {
        var viewModel = new TestViewModel { Name = "Test" };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var hook = new RejectingBindingHook();
        var previousHooks = Locator.Current.GetServices<IPropertyBindingHook>().ToList();
        Locator.CurrentMutable.RegisterConstant<IPropertyBindingHook>(hook);
        try
        {
            using var binding = fixture.OneWayBind(
                viewModel,
                view,
                vm => vm.Name,
                v => v.NameText,
                x => x?.ToUpper() ?? string.Empty);

            // Binding should be created but produce no values
            viewModel.Name = "Changed";
            await Assert.That(view.NameText).IsNotEqualTo("CHANGED");
        }
        finally
        {
            Locator.CurrentMutable.UnregisterAll<IPropertyBindingHook>();
            foreach (var previousHook in previousHooks)
            {
                Locator.CurrentMutable.RegisterConstant(previousHook);
            }
        }
    }

    [Test]
    public async Task BindTo_WithBindingHookReturningFalse_ReturnsDisposableEmpty()
    {
        var view = new TestView { ViewModel = new TestViewModel() };
        var fixture = new PropertyBinderImplementation();
        var source = new Subject<string>();

        var hook = new RejectingBindingHook();
        var previousHooks = Locator.Current.GetServices<IPropertyBindingHook>().ToList();
        Locator.CurrentMutable.RegisterConstant<IPropertyBindingHook>(hook);
        try
        {
            var disposable = fixture.BindTo(source, view, v => v.NameText);

            source.OnNext("Test Value");

            // Value should not be set since binding was rejected
            await Assert.That(view.NameText).IsNotEqualTo("Test Value");

            disposable?.Dispose();
        }
        finally
        {
            Locator.CurrentMutable.UnregisterAll<IPropertyBindingHook>();
            foreach (var previousHook in previousHooks)
            {
                Locator.CurrentMutable.RegisterConstant(previousHook);
            }
        }

        source?.Dispose();
    }

    [Test]
    public async Task BindImpl_WithBindingHookReturningFalse_ReturnsNull()
    {
        var viewModel = new TestViewModel();
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var hook = new RejectingBindingHook();
        var previousHooks = Locator.Current.GetServices<IPropertyBindingHook>().ToList();
        Locator.CurrentMutable.RegisterConstant<IPropertyBindingHook>(hook);
        try
        {
            var binding = fixture.Bind(
                viewModel,
                view,
                vm => vm.Name,
                v => v.NameText,
                (IObservable<Unit>?)null,
                null);

            // Should return null when binding is rejected
            await Assert.That(binding).IsNull();
        }
        finally
        {
            Locator.CurrentMutable.UnregisterAll<IPropertyBindingHook>();
            foreach (var previousHook in previousHooks)
            {
                Locator.CurrentMutable.RegisterConstant(previousHook);
            }
        }
    }

    [Test]
    public async Task Bind_WithTriggerUpdateViewModelToView_ExercisesCodePath()
    {
        // This test exercises the TriggerUpdate.ViewModelToView code path
        var viewModel = new TestViewModel { Count = 10 };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();
        var signal = new Subject<Unit>();

        // Using TriggerUpdate.ViewModelToView exercises the alternative binding implementation
        using var binding = fixture.Bind(
            viewModel,
            view,
            vm => vm.Count,
            v => v.CountText,
            signal,
            count => count.ToString(),
            text => int.TryParse(text, out var n) ? n : 0,
            TriggerUpdate.ViewModelToView);

        // Verify binding was created successfully
        await Assert.That(binding).IsNotNull();

        signal?.Dispose();
    }

    [Test]
    public async Task GetConverterForTypes_WithNullConverter_HandlesGracefully()
    {
        // Register a null converter to test null handling in type converter cache
        var previousConverters = Locator.Current.GetServices<IBindingTypeConverter>().ToList();
        Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(null!);
        try
        {
            var converter = PropertyBinderImplementation.GetConverterForTypes(typeof(string), typeof(int));

            // Should handle null converters gracefully - built-in converter should still work
            await Assert.That(converter).IsNotNull();
        }
        finally
        {
            Locator.CurrentMutable.UnregisterAll<IBindingTypeConverter>();
            foreach (var previousConverter in previousConverters)
            {
                Locator.CurrentMutable.RegisterConstant(previousConverter);
            }
        }
    }

    [Test]
    public async Task BindTo_WithTypeConversion_UsesConverter()
    {
        var target = new TestViewModel();
        var source = new BehaviorSubject<int>(42);
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Name);

        // Should convert int to string
        await Assert.That(target.Name).IsEqualTo("42");

        source.OnNext(100);
        await Assert.That(target.Name).IsEqualTo("100");

        source?.Dispose();
    }

    [Test]
    public async Task Bind_WithDefaultTriggerUpdate_CreatesBinding()
    {
        // This test verifies the default ViewToViewModel trigger path
        var viewModel = new TestViewModel { Count = 5 };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();
        var signal = new Subject<bool>();

        using var binding = fixture.Bind(
            viewModel,
            view,
            vm => vm.Count,
            v => v.CountText,
            signal,
            count => count.ToString(),
            text => int.TryParse(text, out var n) ? n : 0,
            TriggerUpdate.ViewToViewModel);

        // Just verify binding was created
        await Assert.That(binding).IsNotNull();

        signal?.Dispose();
    }

    private class TestViewModel : ReactiveObject
    {
        private string? _name;
        private int _count;

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }
    }

    private class TestView : ReactiveObject, IViewFor<TestViewModel>
    {
        private TestViewModel? _viewModel;
        private string? _nameText;
        private string? _countText;
        private int _count;

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }

        public TestViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        public string? NameText
        {
            get => _nameText;
            set => this.RaiseAndSetIfChanged(ref _nameText, value);
        }

        public string? CountText
        {
            get => _countText;
            set => this.RaiseAndSetIfChanged(ref _countText, value);
        }

        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }
    }

    private class NullReturningConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType) => 100;

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            result = null;
            return true; // Returns true but sets result to null
        }
    }

    private class FailingConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType) => 100;

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            result = null;
            return false; // Always fails conversion
        }
    }

    private class RejectingBindingHook : IPropertyBindingHook
    {
        public bool ExecuteHook(
            object? source,
            object target,
            Func<IObservedChange<object, object?>[]>? getCurrentViewModelProperties,
            Func<IObservedChange<object, object?>[]>? getCurrentViewProperties,
            BindingDirection direction)
        {
            return false; // Always rejects bindings
        }
    }
}
