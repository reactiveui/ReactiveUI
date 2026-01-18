// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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
    [Test]
    public async Task BindTo_WithDirectPropertyAccess_UsesDirectSetObservable()
    {
        var target = new TestViewModel();
        var source = new BehaviorSubject<string?>("Initial");
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Name);

        await Assert.That(target.Name).IsEqualTo("Initial");

        source.OnNext("Updated");
        await Assert.That(target.Name).IsEqualTo("Updated");

        source?.Dispose();
    }

    [Test]
    public async Task BindTo_WithChainedPropertyAccess_UsesChainedSetObservable()
    {
        var target = new ParentViewModel { Child = new TestViewModel() };
        var source = new BehaviorSubject<string?>("ChainedValue");
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Child!.Name);

        await Assert.That(target.Child!.Name).IsEqualTo("ChainedValue");

        source.OnNext("UpdatedChainedValue");
        await Assert.That(target.Child!.Name).IsEqualTo("UpdatedChainedValue");

        source?.Dispose();
    }

    [Test]
    public async Task BindTo_WithChainedPropertyAndHostChange_ReplaysValue()
    {
        var target = new ParentViewModel { Child = new TestViewModel() };
        var source = new BehaviorSubject<string?>("OriginalValue");
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Child!.Name);

        await Assert.That(target.Child!.Name).IsEqualTo("OriginalValue");

        // Change the host (Child) to a new instance with default Name
        var newChild = new TestViewModel();
        target.Child = newChild;

        // The binding should replay the last value to the new host
        await Task.Delay(50); // Give time for host change to propagate
        await Assert.That(newChild.Name).IsEqualTo("OriginalValue");

        source?.Dispose();
    }

    [Test]
    public async Task BindTo_WithNullHost_SkipsUpdate()
    {
        var target = new ParentViewModel { Child = new TestViewModel() };
        var source = new BehaviorSubject<string?>("InitialValue");
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.BindTo(source, target, t => t.Child!.Name);

        await Assert.That(target.Child!.Name).IsEqualTo("InitialValue");

        // Set host to null
        target.Child = null;

        // Try to push a new value - should be skipped
        source.OnNext("ShouldNotApply");

        // Restore host - should have the new value applied
        target.Child = new TestViewModel();
        await Task.Delay(50);
        await Assert.That(target.Child.Name).IsEqualTo("ShouldNotApply");

        source?.Dispose();
    }

    [Test]
    public async Task BindTo_WithViewModelPropertyChain_DoesNotReplay()
    {
        var viewModel = new TestViewModel { Name = "VMName" };
        var view = new TestView { ViewModel = viewModel };
        var source = new BehaviorSubject<string?>("SourceValue");
        var fixture = new PropertyBinderImplementation();

        // Binding to view.ViewModel.Name should NOT replay on ViewModel changes
        using var binding = fixture.BindTo(source, view, v => ((TestViewModel)v.ViewModel!).Name);

        await Assert.That(viewModel.Name).IsEqualTo("SourceValue");

        // Change ViewModel to a new instance
        var newViewModel = new TestViewModel { Name = "NewVMName" };
        view.ViewModel = newViewModel;

        // The binding should NOT replay the last value because it's through IViewFor.ViewModel
        await Task.Delay(50);
        await Assert.That(newViewModel.Name).IsEqualTo("NewVMName"); // Should keep its original value

        source?.Dispose();
    }

    [Test]
    public async Task Bind_WithNullViewModel_HandlesGracefully()
    {
        var viewModel = new TestViewModel { Name = "Initial" };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        using var binding = fixture.Bind<TestViewModel, TestView, string?, string?, Unit>(
            viewModel,
            view,
            vm => vm.Name,
            v => v.NameText,
            null,
            null);

        await Assert.That(view.NameText).IsEqualTo("Initial");
    }

    [Test]
    public async Task Bind_WithExplicitConverterThatFails_TriesFallbackFromRegistry()
    {
        var viewModel = new TestViewModel { Count = 42 };
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
                (IObservable<Unit>?)null,
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

    [Test]
    public async Task Bind_WithExplicitConverterThatSucceeds_UsesExplicitConverter()
    {
        var viewModel = new TestViewModel { Count = 100 };
        var view = new TestView { ViewModel = viewModel };
        var fixture = new PropertyBinderImplementation();

        var customConverter = new IntToStringConverter();
        using var binding = fixture.Bind(
            viewModel,
            view,
            vm => vm.Count,
            v => v.CountText,
            (IObservable<Unit>?)null,
            null,
            customConverter,
            customConverter);

        await Assert.That(view.CountText).IsEqualTo("100");
    }

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

    [Test]
    public async Task BindTo_WithAutoDiscoveredConverterThatFails_TriesDirectAssignment()
    {
        var target = new TestViewModel { Data = "Original" };
        var source = new BehaviorSubject<string?>("NewValue");
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

        source?.Dispose();
    }

    [Test]
    public async Task ResolveBestConverter_WithMultipleTypedConverters_SelectsHighestAffinity()
    {
        var previousConverters = Splat.Locator.Current.GetServices<IBindingTypeConverter>().ToList();
        var lowAffinityConverter = new CustomTypeConverter(affinity: 10);
        var highAffinityConverter = new CustomTypeConverter(affinity: 100);

        Splat.Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(lowAffinityConverter);
        Splat.Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(highAffinityConverter);

        try
        {
            var converter = PropertyBinderImplementation.GetConverterForTypes(typeof(CustomSource), typeof(CustomTarget));

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

    [Test]
    public async Task ResolveBestConverter_WithMultipleFallbackConverters_SelectsHighestAffinity()
    {
        var previousConverters = Splat.Locator.Current.GetServices<IBindingFallbackConverter>().ToList();
        var lowAffinityConverter = new TestFallbackConverter(affinity: 10);
        var highAffinityConverter = new TestFallbackConverter(affinity: 100);

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

    [Test]
    public async Task ResolveBestConverter_WithZeroAffinityConverter_IgnoresConverter()
    {
        var previousConverters = Splat.Locator.Current.GetServices<IBindingTypeConverter>().ToList();
        var zeroAffinityConverter = new IntToStringConverter(affinity: 0);

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
            (IObservable<Unit>?)null,
            null,
            unknownConverter,
            unknownConverter);

        // Should handle unknown converter type gracefully by trying direct assignment
        await Assert.That(view.NameText).IsEqualTo("Test");
    }

    private class TestFallbackConverter : IBindingFallbackConverter
    {
        private readonly int _affinity;

        public TestFallbackConverter(int affinity = 50)
        {
            _affinity = affinity;
        }

        public int GetAffinityForObjects(Type from, Type to) => _affinity;

        public bool TryConvert(Type fromType, object from, Type toType, object? conversionHint, [NotNullWhen(true)] out object? result)
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

    private class IntToStringConverter : IBindingTypeConverter
    {
        private readonly int _affinity;

        public IntToStringConverter(int affinity = 100)
        {
            _affinity = affinity;
        }

        public Type FromType => typeof(int);

        public Type ToType => typeof(string);

        public int GetAffinityForObjects() => _affinity;

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

    private class FailingConverter : IBindingTypeConverter
    {
        public Type FromType => typeof(object);

        public Type ToType => typeof(object);

        public int GetAffinityForObjects() => 10; // Low affinity so fallback can override

        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return false;
        }
    }

    private class FailingStringConverter : IBindingTypeConverter
    {
        public Type FromType => typeof(string);

        public Type ToType => typeof(string);

        public int GetAffinityForObjects() => 100;

        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return false;
        }
    }

    private class UnknownConverterType : IBindingTypeConverter, IBindingFallbackConverter
    {
        public Type FromType => typeof(object);

        public Type ToType => typeof(object);

        public int GetAffinityForObjects() => 100;

        public int GetAffinityForObjects(Type from, Type to) => 100;

        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return false;
        }

        public bool TryConvert(Type fromType, object from, Type toType, object? conversionHint, [NotNullWhen(true)] out object? result)
        {
            result = null;
            return false;
        }
    }

    private class TestView : ReactiveObject, IViewFor<TestViewModel>
    {
        private string? _countText;
        private string? _nameText;
        private TestViewModel? _viewModel;

        public string? CountText
        {
            get => _countText;
            set => this.RaiseAndSetIfChanged(ref _countText, value);
        }

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
        private string? _data;
        private string? _name;

        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }

        public string? Data
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, value);
        }

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }

    private class ParentViewModel : ReactiveObject
    {
        private TestViewModel? _child;

        public TestViewModel? Child
        {
            get => _child;
            set => this.RaiseAndSetIfChanged(ref _child, value);
        }
    }

    private class CustomSource
    {
        public string? Value { get; set; }
    }

    private class CustomTarget
    {
        public string? Value { get; set; }
    }

    private class CustomTypeConverter : IBindingTypeConverter
    {
        private readonly int _affinity;

        public CustomTypeConverter(int affinity = 100)
        {
            _affinity = affinity;
        }

        public Type FromType => typeof(CustomSource);

        public Type ToType => typeof(CustomTarget);

        public int GetAffinityForObjects() => _affinity;

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
