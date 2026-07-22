// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for PropertyBinderImplementation covering edge cases and error paths.
///     These tests use Locator.CurrentMutable (static state) so must run in NonParallel test suite.
/// </summary>
public class PropertyBinderImplementationTests
{
    /// <summary>The initial text value used by the binding tests.</summary>
    private const string TestText = "Test";

    /// <summary>The changed text value used to verify binding updates.</summary>
    private const string ChangedText = "Changed";

    /// <summary>Verifies that Bind with the default trigger update creates a binding.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Bind_WithDefaultTriggerUpdate_CreatesBinding()
    {
        // This test verifies the default ViewToViewModel trigger path
        const int CountValue = 5;
        var viewModel = new TestViewModel { Count = CountValue };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();
        var signal = new Signal<bool>();

        using var binding = fixture.Bind(
            viewModel,
            view,
            vm => vm.Count,
            v => v.CountText,
            signal,
            static count => count.ToString(),
            static text => int.TryParse(text, out var n) ? n : 0);

        // Just verify binding was created
        await Assert.That(binding).IsNotNull();

        signal.Dispose();
    }

    /// <summary>Verifies that Bind handles a converter that returns a null value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Bind_WithNullReturningConverter_HandlesNullTmpValue()
    {
        var viewModel = new TestViewModel { Name = TestText };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var nullConverter = new NullReturningConverter();
        using var binding = fixture.Bind(
            viewModel,
            view,
            vm => vm.Name,
            v => v.NameText,
            (IObservable<RxVoid>?)null,
            null,
            nullConverter,
            nullConverter);

        // Should handle null tmp value from converter
        await Assert.That(view.NameText).IsNull();
    }

    /// <summary>Verifies that Bind with TriggerUpdate.ViewModelToView creates a binding.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Bind_WithTriggerUpdateViewModelToView_ExercisesCodePath()
    {
        // This test exercises the TriggerUpdate.ViewModelToView code path
        const int CountValue = 10;
        var viewModel = new TestViewModel { Count = CountValue };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();
        var signal = new Signal<RxVoid>();

        // Using TriggerUpdate.ViewModelToView exercises the alternative binding implementation
        using var binding = fixture.Bind(
            viewModel,
            view,
            vm => vm.Count,
            v => v.CountText,
            signal,
            static count => count.ToString(),
            static text => int.TryParse(text, out var n) ? n : 0,
            TriggerUpdate.ViewModelToView);

        // Verify binding was created successfully
        await Assert.That(binding).IsNotNull();

        signal.Dispose();
    }

    /// <summary>Verifies that Bind returns null when a binding hook rejects the binding.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindImpl_WithBindingHookReturningFalse_ReturnsNull()
    {
        var viewModel = new TestViewModel();
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var hook = new RejectingBindingHook();
        var previousHooks = Splat.Locator.Current.GetServices<IPropertyBindingHook>().ToList();
        Splat.Locator.CurrentMutable.RegisterConstant<IPropertyBindingHook>(hook);
        try
        {
            var binding = fixture.Bind(
                viewModel,
                view,
                vm => vm.Name,
                v => v.NameText,
                (IObservable<RxVoid>?)null,
                null);

            // Should return null when binding is rejected
            await Assert.That(binding).IsNull();
        }
        finally
        {
            Splat.Locator.CurrentMutable.UnregisterAll<IPropertyBindingHook>();
            foreach (var previousHook in previousHooks)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousHook);
            }
        }
    }

    /// <summary>Verifies that BindTo returns an empty disposable and does not update when a hook rejects the binding.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindTo_WithBindingHookReturningFalse_ReturnsDisposableEmpty()
    {
        var view = new TestView { ViewModel = new() };
        var fixture = new PropertyBinderImplementation();
        var source = new Signal<string>();

        var hook = new RejectingBindingHook();
        var previousHooks = Splat.Locator.Current.GetServices<IPropertyBindingHook>().ToList();
        Splat.Locator.CurrentMutable.RegisterConstant<IPropertyBindingHook>(hook);
        try
        {
            var disposable = fixture.BindTo(source, view, v => v.NameText);

            source.OnNext("Test Value");

            // Value should not be set since binding was rejected
            await Assert.That(view.NameText).IsNotEqualTo("Test Value");

            disposable.Dispose();
        }
        finally
        {
            Splat.Locator.CurrentMutable.UnregisterAll<IPropertyBindingHook>();
            foreach (var previousHook in previousHooks)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousHook);
            }
        }

        source.Dispose();
    }

    /// <summary>Verifies that BindTo uses a type converter to convert the source value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindTo_WithTypeConversion_UsesConverter()
    {
        const int InitialValue = 42;
        const int UpdatedValue = 100;
        var target = new TestViewModel();
        var source = new BehaviorSignal<int>(InitialValue);
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Name);

        // Should convert int to string
        await Assert.That(target.Name).IsEqualTo("42");

        source.OnNext(UpdatedValue);
        await Assert.That(target.Name).IsEqualTo("100");

        source.Dispose();
    }

    /// <summary>Verifies that GetConverterForTypes handles a null registered converter gracefully.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetConverterForTypes_WithNullConverter_HandlesGracefully()
    {
        // Register a null converter to test null handling in type converter cache
        var previousConverters = Splat.Locator.Current.GetServices<IBindingTypeConverter>().ToList();
        Splat.Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(null!);
        try
        {
            var converter = PropertyBinderImplementation.GetConverterForTypes(typeof(string), typeof(int));

            // Should handle null converters gracefully - built-in converter should still work
            await Assert.That(converter).IsNotNull();
        }
        finally
        {
            Splat.Locator.CurrentMutable.UnregisterAll<IBindingTypeConverter>();
            foreach (var previousConverter in previousConverters)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousConverter);
            }
        }
    }

    /// <summary>Verifies that OneWayBind produces no values when a binding hook rejects the binding.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBind_WithBindingHookReturningFalse_ReturnsEmptyObservable()
    {
        var viewModel = new TestViewModel { Name = TestText };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var hook = new RejectingBindingHook();
        var previousHooks = Splat.Locator.Current.GetServices<IPropertyBindingHook>().ToList();
        Splat.Locator.CurrentMutable.RegisterConstant<IPropertyBindingHook>(hook);
        try
        {
            using var binding = fixture.OneWayBind(viewModel, view, vm => vm.Name, v => v.NameText);

            // Binding should be created but produce no values
            viewModel.Name = ChangedText;
            await Assert.That(view.NameText).IsNotEqualTo(ChangedText);
        }
        finally
        {
            Splat.Locator.CurrentMutable.UnregisterAll<IPropertyBindingHook>();
            foreach (var previousHook in previousHooks)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousHook);
            }
        }
    }

    /// <summary>Verifies that OneWayBind does not update the view when the converter fails.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
            viewModelToViewConverterOverride: failingConverter);

        var initialText = view.NameText;

        viewModel.Name = ChangedText;

        // View should not update when converter fails
        await Assert.That(view.NameText).IsEqualTo(initialText);
    }

    /// <summary>Verifies that OneWayBind with a selector produces no values when a binding hook rejects the binding.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBind_WithSelector_AndBindingHookReturningFalse_ReturnsEmptyObservable()
    {
        var viewModel = new TestViewModel { Name = TestText };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var hook = new RejectingBindingHook();
        var previousHooks = Splat.Locator.Current.GetServices<IPropertyBindingHook>().ToList();
        Splat.Locator.CurrentMutable.RegisterConstant<IPropertyBindingHook>(hook);
        try
        {
            using var binding = fixture.OneWayBind(
                viewModel,
                view,
                vm => vm.Name,
                v => v.NameText,
                static x => x?.ToUpper() ?? string.Empty);

            // Binding should be created but produce no values
            viewModel.Name = ChangedText;
            await Assert.That(view.NameText).IsNotEqualTo("CHANGED");
        }
        finally
        {
            Splat.Locator.CurrentMutable.UnregisterAll<IPropertyBindingHook>();
            foreach (var previousHook in previousHooks)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousHook);
            }
        }
    }

    /// <summary>A test converter that always fails to convert.</summary>
    private sealed class FailingConverter : IBindingTypeConverter
    {
        /// <summary>A high affinity so this converter is selected during binding.</summary>
        private const int HighAffinity = 100;

        /// <inheritdoc/>
        public Type FromType => typeof(object);

        /// <inheritdoc/>
        public Type ToType => typeof(object);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => HighAffinity;

        /// <inheritdoc/>
        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return false; // Always fails conversion
        }
    }

    /// <summary>A test converter that reports success but produces a null result.</summary>
    private sealed class NullReturningConverter : IBindingTypeConverter
    {
        /// <summary>A high affinity so this converter is selected during binding.</summary>
        private const int HighAffinity = 100;

        /// <inheritdoc/>
        public Type FromType => typeof(object);

        /// <inheritdoc/>
        public Type ToType => typeof(object);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => HighAffinity;

        /// <inheritdoc/>
        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return true; // Returns true but sets result to null
        }
    }

    /// <summary>A test binding hook that always rejects bindings.</summary>
    private sealed class RejectingBindingHook : IPropertyBindingHook
    {
        /// <inheritdoc/>
        public bool ExecuteHook(
            object? source,
            object target,
            Func<IObservedChange<object, object?>[]>? getCurrentViewModelProperties,
            Func<IObservedChange<object, object?>[]>? getCurrentViewProperties,
            BindingDirection direction) =>
            false; // Always rejects bindings
    }

    /// <summary>A test view used to exercise binding scenarios.</summary>
    private sealed class TestView : ReactiveObject, IViewFor<TestViewModel>
    {
        /// <summary>Gets or sets the count.</summary>
        public int Count
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the text representing a count.</summary>
        public string? CountText
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the text representing a name.</summary>
        public string? NameText
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <inheritdoc/>
        public TestViewModel? ViewModel
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>A test view model used to exercise binding scenarios.</summary>
    private sealed class TestViewModel : ReactiveObject
    {
        /// <summary>Gets or sets the count.</summary>
        public int Count
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the name.</summary>
        public string? Name
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }
}
