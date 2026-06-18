// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>
/// Advanced tests for PropertyBinderImplementation focusing on uncovered code paths.
/// Tests internal methods using reflection and complex binding scenarios.
/// Uses AppBuilderTestExecutor to ensure RxConverters state is reset between tests.
/// </summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public class PropertyBinderImplementationAdvancedTests
{
    /// <summary>The delay, in milliseconds, allowed for a host change to propagate.</summary>
    private const int HostChangePropagationDelayMs = 50;

    /// <summary>A binding source value reused across host-change scenarios.</summary>
    private const string OriginalValueText = "OriginalValue";

    /// <summary>The initial binding value reused across tests.</summary>
    private const string InitialText = "Initial";

    /// <summary>Verifies that BindTo with a direct property access uses the direct set observable path.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindTo_WithDirectPropertyAccess_UsesDirectSetObservable()
    {
        var target = new TestViewModel();
        var source = new BehaviorSignal<string?>(InitialText);
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Name);

        await Assert.That(target.Name).IsEqualTo(InitialText);

        source.OnNext("Updated");
        await Assert.That(target.Name).IsEqualTo("Updated");

        source.Dispose();
    }

    /// <summary>Verifies that BindTo with a chained property access uses the chained set observable path.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindTo_WithChainedPropertyAccess_UsesChainedSetObservable()
    {
        var target = new ParentViewModel { Child = new() };
        var source = new BehaviorSignal<string?>("ChainedValue");
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Child!.Name);

        await Assert.That(target.Child!.Name).IsEqualTo("ChainedValue");

        source.OnNext("UpdatedChainedValue");
        await Assert.That(target.Child!.Name).IsEqualTo("UpdatedChainedValue");

        source.Dispose();
    }

    /// <summary>Verifies that BindTo replays the last value when the chained host changes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindTo_WithChainedPropertyAndHostChange_ReplaysValue()
    {
        var target = new ParentViewModel { Child = new() };
        var source = new BehaviorSignal<string?>(OriginalValueText);
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Child!.Name);

        await Assert.That(target.Child!.Name).IsEqualTo(OriginalValueText);

        // Change the host (Child) to a new instance with default Name
        var newChild = new TestViewModel();
        target.Child = newChild;

        // The binding should replay the last value to the new host
        // Give time for host change to propagate
        await Task.Delay(HostChangePropagationDelayMs);
        await Assert.That(newChild.Name).IsEqualTo(OriginalValueText);

        source.Dispose();
    }

    /// <summary>Verifies that BindTo skips updates while the host is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindTo_WithNullHost_SkipsUpdate()
    {
        var target = new ParentViewModel { Child = new() };
        var source = new BehaviorSignal<string?>("InitialValue");
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Child!.Name);

        await Assert.That(target.Child!.Name).IsEqualTo("InitialValue");

        // Set host to null
        target.Child = null;

        // Try to push a new value - should be skipped
        source.OnNext("ShouldNotApply");

        // Restore host - should have the new value applied
        target.Child = new();
        await Task.Delay(HostChangePropagationDelayMs);
        await Assert.That(target.Child.Name).IsEqualTo("ShouldNotApply");

        source.Dispose();
    }

    /// <summary>Verifies that BindTo does not replay the last value when the chain goes through IViewFor.ViewModel.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindTo_WithViewModelPropertyChain_DoesNotReplay()
    {
        var viewModel = new TestViewModel { Name = "VMName" };
        var view = new TestView { ViewModel = viewModel };
        var source = new BehaviorSignal<string?>("SourceValue");
        var fixture = new PropertyBinderImplementation();

        // Binding to view.ViewModel.Name should NOT replay on ViewModel changes
        using var binding = fixture.BindTo(source, view, v => v.ViewModel!.Name);

        await Assert.That(viewModel.Name).IsEqualTo("SourceValue");

        // Change ViewModel to a new instance
        var newViewModel = new TestViewModel { Name = "NewVMName" };
        view.ViewModel = newViewModel;

        // The binding should NOT replay the last value because it's through IViewFor.ViewModel
        await Task.Delay(HostChangePropagationDelayMs);
        await Assert.That(newViewModel.Name).IsEqualTo("NewVMName"); // Should keep its original value

        source.Dispose();
    }

    /// <summary>Verifies that Bind handles a null type converter argument gracefully.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Bind_WithNullViewModel_HandlesGracefully()
    {
        var viewModel = new TestViewModel { Name = InitialText };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.Bind<TestViewModel, TestView, string?, string?, RxVoid>(
            viewModel,
            view,
            vm => vm.Name,
            v => v.NameText,
            null,
            null);

        await Assert.That(view.NameText).IsEqualTo(InitialText);
    }

    /// <summary>Verifies that Bind falls back to a registry converter when the explicit converter fails.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Bind_WithExplicitConverterThatFails_TriesFallbackFromRegistry()
    {
        const int CountValue = 42;
        var viewModel = new TestViewModel { Count = CountValue };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        // Register a better converter in the registry
        var previousConverters = Splat.Locator.Current.GetServices<IBindingTypeConverter>().ToList();
        var betterConverter = new IntToStringConverter();
        Splat.Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(betterConverter);

        try
        {
            var failingConverter = new FailingConverter();
            using var binding = fixture.Bind(
                viewModel,
                view,
                vm => vm.Count,
                v => v.CountText,
                (IObservable<RxVoid>?)null,
                null,
                failingConverter, // Explicit converter that fails
                failingConverter);

            // Should fall back to registry converter
            await Assert.That(view.CountText).IsEqualTo("42");
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

    /// <summary>Verifies that Bind uses the explicit converter when it succeeds.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Bind_WithExplicitConverterThatSucceeds_UsesExplicitConverter()
    {
        const int CountValue = 100;
        var viewModel = new TestViewModel { Count = CountValue };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var customConverter = new IntToStringConverter();
        using var binding = fixture.Bind(
            viewModel,
            view,
            vm => vm.Count,
            v => v.CountText,
            (IObservable<RxVoid>?)null,
            null,
            customConverter,
            customConverter);

        await Assert.That(view.CountText).IsEqualTo("100");
    }

    /// <summary>Verifies that OneWayBind falls back to direct assignment when the auto-discovered converter fails.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OneWayBind_WithAutoDiscoveredConverterThatFails_TriesDirectAssignment()
    {
        var viewModel = new TestViewModel { Data = "DirectValue" };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        // Register a failing converter for string->string
        var previousConverters = Splat.Locator.Current.GetServices<IBindingTypeConverter>().ToList();
        var failingConverter = new FailingStringConverter();
        Splat.Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(failingConverter);

        try
        {
            using var binding = fixture.OneWayBind(
                viewModel,
                view,
                vm => vm.Data,
                v => v.NameText);

            // Should fall back to direct assignment since types are assignable
            await Assert.That(view.NameText).IsEqualTo("DirectValue");
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

    /// <summary>Verifies that BindTo falls back to direct assignment when the auto-discovered converter fails.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindTo_WithAutoDiscoveredConverterThatFails_TriesDirectAssignment()
    {
        var target = new TestViewModel { Data = "Original" };
        var source = new BehaviorSignal<string?>("NewValue");
        var fixture = new PropertyBinderImplementation();

        // Register a failing converter for string->string
        var previousConverters = Splat.Locator.Current.GetServices<IBindingTypeConverter>().ToList();
        var failingConverter = new FailingStringConverter();
        Splat.Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(failingConverter);

        try
        {
            using var binding = fixture.BindTo(source, target, t => t.Data);

            // Should fall back to direct assignment
            await Assert.That(target.Data).IsEqualTo("NewValue");
        }
        finally
        {
            Splat.Locator.CurrentMutable.UnregisterAll<IBindingTypeConverter>();
            foreach (var previousConverter in previousConverters)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousConverter);
            }
        }

        source.Dispose();
    }

    /// <summary>Verifies that the converter resolution selects the highest-affinity typed converter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResolveBestConverter_WithMultipleTypedConverters_SelectsHighestAffinity()
    {
        const int LowAffinity = 10;
        var previousConverters = Splat.Locator.Current.GetServices<IBindingTypeConverter>().ToList();
        var lowAffinityConverter = new CustomTypeConverter(LowAffinity);
        var highAffinityConverter = new CustomTypeConverter();

        Splat.Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(lowAffinityConverter);
        Splat.Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(highAffinityConverter);

        try
        {
            var converter =
                PropertyBinderImplementation.GetConverterForTypes(typeof(CustomSource), typeof(CustomTarget));

            // Should select the high affinity converter (RxConverters.Current won't have this custom type)
            await Assert.That(converter).IsSameReferenceAs(highAffinityConverter);
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

    /// <summary>Verifies that the converter resolution selects the highest-affinity fallback converter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResolveBestConverter_WithMultipleFallbackConverters_SelectsHighestAffinity()
    {
        const int LowAffinity = 10;
        const int HighAffinity = 100;
        var previousConverters = Splat.Locator.Current.GetServices<IBindingFallbackConverter>().ToList();
        var lowAffinityConverter = new TestFallbackConverter(LowAffinity);
        var highAffinityConverter = new TestFallbackConverter(HighAffinity);

        Splat.Locator.CurrentMutable.RegisterConstant<IBindingFallbackConverter>(lowAffinityConverter);
        Splat.Locator.CurrentMutable.RegisterConstant<IBindingFallbackConverter>(highAffinityConverter);

        try
        {
            var converter = PropertyBinderImplementation.GetConverterForTypes(typeof(object), typeof(object));

            // Should select the high affinity fallback converter
            await Assert.That(converter).IsSameReferenceAs(highAffinityConverter);
        }
        finally
        {
            Splat.Locator.CurrentMutable.UnregisterAll<IBindingFallbackConverter>();
            foreach (var previousConverter in previousConverters)
            {
                Splat.Locator.CurrentMutable.RegisterConstant(previousConverter);
            }
        }
    }

    /// <summary>Verifies that the converter resolution ignores converters with zero affinity.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResolveBestConverter_WithZeroAffinityConverter_IgnoresConverter()
    {
        var previousConverters = Splat.Locator.Current.GetServices<IBindingTypeConverter>().ToList();
        var zeroAffinityConverter = new IntToStringConverter(0);

        Splat.Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(zeroAffinityConverter);

        try
        {
            var converter = PropertyBinderImplementation.GetConverterForTypes(typeof(int), typeof(string));

            // Should not select zero-affinity converter, should find built-in converter instead
            await Assert.That(converter).IsNotSameReferenceAs(zeroAffinityConverter);
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

    /// <summary>Verifies that Bind handles an unknown converter type by falling back to direct assignment.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Bind_WithUnknownConverterType_HandlesFallbackGracefully()
    {
        var viewModel = new TestViewModel { Name = "Test" };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var unknownConverter = new UnknownConverterType();
        using var binding = fixture.Bind(
            viewModel,
            view,
            vm => vm.Name,
            v => v.NameText,
            (IObservable<RxVoid>?)null,
            null,
            unknownConverter,
            unknownConverter);

        // Should handle unknown converter type gracefully by trying direct assignment
        await Assert.That(view.NameText).IsEqualTo("Test");
    }

    /// <summary>A test fallback converter with a configurable affinity.</summary>
    private sealed class TestFallbackConverter : IBindingFallbackConverter
    {
        /// <summary>The default affinity reported when none is supplied to the constructor.</summary>
        private const int DefaultAffinity = 50;

        /// <summary>The affinity returned by this converter.</summary>
        private readonly int _affinity;

        /// <summary>Initializes a new instance of the <see cref="TestFallbackConverter" /> class.</summary>
        /// <param name="affinity">The affinity to report.</param>
        public TestFallbackConverter(int affinity = DefaultAffinity) => _affinity = affinity;

        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType) => _affinity;

        /// <inheritdoc/>
        public bool TryConvert(
            Type fromType,
            object from,
            Type toType,
            object? conversionHint,
            [NotNullWhen(true)] out object? result)
        {
            if (from is null)
            {
                result = null;
                return false;
            }

            result = from.ToString() ?? string.Empty;
            return true;
        }
    }

    /// <summary>A test converter that converts an integer to its string representation.</summary>
    private sealed class IntToStringConverter : IBindingTypeConverter
    {
        /// <summary>The default affinity reported when none is supplied to the constructor.</summary>
        private const int DefaultAffinity = 100;

        /// <summary>The affinity returned by this converter.</summary>
        private readonly int _affinity;

        /// <summary>Initializes a new instance of the <see cref="IntToStringConverter" /> class.</summary>
        /// <param name="affinity">The affinity to report.</param>
        public IntToStringConverter(int affinity = DefaultAffinity) => _affinity = affinity;

        /// <inheritdoc/>
        public Type FromType => typeof(int);

        /// <inheritdoc/>
        public Type ToType => typeof(string);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => _affinity;

        /// <inheritdoc/>
        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            if (from is int intValue)
            {
                result = intValue.ToString();
                return true;
            }

            result = null;
            return false;
        }
    }

    /// <summary>A test converter that always fails to convert.</summary>
    private sealed class FailingConverter : IBindingTypeConverter
    {
        /// <summary>A low affinity so a fallback converter can override this one.</summary>
        private const int LowAffinity = 10;

        /// <inheritdoc/>
        public Type FromType => typeof(object);

        /// <inheritdoc/>
        public Type ToType => typeof(object);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => LowAffinity; // Low affinity so fallback can override

        /// <inheritdoc/>
        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return false;
        }
    }

    /// <summary>A test string converter that always fails to convert.</summary>
    private sealed class FailingStringConverter : IBindingTypeConverter
    {
        /// <summary>A high affinity so this converter is preferred during selection.</summary>
        private const int HighAffinity = 100;

        /// <inheritdoc/>
        public Type FromType => typeof(string);

        /// <inheritdoc/>
        public Type ToType => typeof(string);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => HighAffinity;

        /// <inheritdoc/>
        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return false;
        }
    }

    /// <summary>A test converter that implements both converter interfaces but always fails to convert.</summary>
    private sealed class UnknownConverterType : IBindingTypeConverter, IBindingFallbackConverter
    {
        /// <summary>A high affinity so this converter is preferred during selection.</summary>
        private const int HighAffinity = 100;

        /// <inheritdoc/>
        public Type FromType => typeof(object);

        /// <inheritdoc/>
        public Type ToType => typeof(object);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => HighAffinity;

        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType) => HighAffinity;

        /// <inheritdoc/>
        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return false;
        }

        /// <inheritdoc/>
        public bool TryConvert(
            Type fromType,
            object from,
            Type toType,
            object? conversionHint,
            [NotNullWhen(true)] out object? result)
        {
            result = null;
            return false;
        }
    }

    /// <summary>A test view used to exercise binding scenarios.</summary>
    private sealed class TestView : ReactiveObject, IViewFor<TestViewModel>
    {
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

        /// <summary>Gets or sets the data.</summary>
        public string? Data
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

    /// <summary>A test view model that contains a child view model for chained binding tests.</summary>
    private sealed class ParentViewModel : ReactiveObject
    {
        /// <summary>Gets or sets the child view model.</summary>
        public TestViewModel? Child
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>A custom source type used for converter resolution tests.</summary>
    private sealed class CustomSource
    {
        /// <summary>Gets or sets the value.</summary>
        public string? Value { get; set; } = null!;
    }

    /// <summary>A custom target type used for converter resolution tests.</summary>
    private sealed class CustomTarget
    {
        /// <summary>Gets or sets the value.</summary>
        public string? Value { get; set; }
    }

    /// <summary>A test converter that converts a <see cref="CustomSource" /> to a <see cref="CustomTarget" />.</summary>
    private sealed class CustomTypeConverter : IBindingTypeConverter
    {
        /// <summary>The default affinity reported when none is supplied to the constructor.</summary>
        private const int DefaultAffinity = 100;

        /// <summary>The affinity returned by this converter.</summary>
        private readonly int _affinity;

        /// <summary>Initializes a new instance of the <see cref="CustomTypeConverter" /> class.</summary>
        /// <param name="affinity">The affinity to report.</param>
        public CustomTypeConverter(int affinity = DefaultAffinity) => _affinity = affinity;

        /// <inheritdoc/>
        public Type FromType => typeof(CustomSource);

        /// <inheritdoc/>
        public Type ToType => typeof(CustomTarget);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => _affinity;

        /// <inheritdoc/>
        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            if (from is CustomSource source)
            {
                result = new CustomTarget { Value = source.Value };
                return true;
            }

            result = null;
            return false;
        }
    }
}
