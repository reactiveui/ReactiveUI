// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.PropertyBindings;

/// <summary>
/// Tests for the property binding mixin extension methods (Bind, OneWayBind, and BindTo).
/// </summary>
public class PropertyBindingMixinsTests
{
    private const string InitialText = "Initial";
    private const string ChangedText = "Changed";
    private const string UpdatedText = "Updated";

    /// <summary>
    /// Verifies that a two-way binding stops updating the view after it is disposed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Bind_AfterDispose_StopsUpdating()
    {
        var viewModel = new TestViewModel { Name = InitialText };
        var view = new TestView { ViewModel = viewModel };

        using (var binding = view.Bind(viewModel, vm => vm.Name, v => v.NameText))
        {
            await Assert.That(view.NameText).IsEqualTo(InitialText);
        }

        viewModel.Name = ChangedText;
        await Assert.That(view.NameText).IsEqualTo(InitialText);
    }

    /// <summary>
    /// Verifies that a two-way binding propagates changes in both directions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Bind_WithBasicProperties_UpdatesBothDirections()
    {
        var viewModel = new TestViewModel { Name = InitialText };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.Bind(viewModel, vm => vm.Name, v => v.NameText);

        // VM to View
        await Assert.That(view.NameText).IsEqualTo(InitialText);

        // View to VM
        view.NameText = UpdatedText;
        await Assert.That(viewModel.Name).IsEqualTo(UpdatedText);

        // VM to View again
        viewModel.Name = ChangedText;
        await Assert.That(view.NameText).IsEqualTo(ChangedText);
    }

    /// <summary>
    /// Verifies that a two-way binding applies the supplied conversion functions in each direction.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Bind_WithConverters_ConvertsValues()
    {
        const int InitialCount = 42;
        const int UpdatedCount = 100;
        var viewModel = new TestViewModel { Count = InitialCount };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.Bind(
            viewModel,
            vm => vm.Count,
            v => v.NameText,
            count => $"Count: {count}",
            text => int.TryParse(text?.Replace("Count: ", string.Empty, StringComparison.Ordinal), out var n) ? n : 0);

        await Assert.That(view.NameText).IsEqualTo("Count: 42");

        view.NameText = "Count: 100";
        await Assert.That(viewModel.Count).IsEqualTo(UpdatedCount);
    }

    /// <summary>
    /// Verifies that a two-way binding with a null view model does not throw.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
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

    /// <summary>
    /// Verifies that a two-way binding with a signal only pushes view changes back when the signal fires.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Bind_WithSignalViewUpdate_UpdatesOnSignal()
    {
        var viewModel = new TestViewModel { Name = InitialText };
        var view = new TestView { ViewModel = viewModel };
        var signal = new Subject<Unit>();

        using var binding = view.Bind(
            viewModel,
            vm => vm.Name,
            v => v.NameText,
            signal);

        await Assert.That(view.NameText).IsEqualTo(InitialText);

        // Change view property but don't signal yet
        view.NameText = UpdatedText;
        await Assert.That(viewModel.Name).IsEqualTo(InitialText);

        // Signal the update
        signal.OnNext(Unit.Default);
        await Assert.That(viewModel.Name).IsEqualTo(UpdatedText);
    }

    /// <summary>
    /// Verifies that a two-way binding automatically converts between differing property types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Bind_WithTypeConverter_ConvertsTypes()
    {
        const int InitialCount = 42;
        const int UpdatedCount = 100;
        var viewModel = new TestViewModel { Count = InitialCount };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.Bind(
            viewModel,
            vm => vm.Count,
            v => v.NameText);

        // Should convert int to string automatically
        await Assert.That(view.NameText).IsEqualTo("42");

        view.NameText = "100";
        await Assert.That(viewModel.Count).IsEqualTo(UpdatedCount);
    }

    /// <summary>
    /// Verifies that a BindTo binding stops updating the target after it is disposed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindTo_AfterDispose_StopsUpdating()
    {
        var target = new TestViewModel();
        var source = new BehaviorSubject<string>(InitialText);

        using (var binding = source.BindTo(target, t => t.Name))
        {
            await Assert.That(target.Name).IsEqualTo(InitialText);
        }

        source.OnNext(UpdatedText);
        await Assert.That(target.Name).IsEqualTo(InitialText);
    }

    /// <summary>
    /// Verifies that a BindTo binding updates the target property as the source observable emits.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindTo_UpdatesTargetProperty()
    {
        var target = new TestViewModel();
        var source = new BehaviorSubject<string>(InitialText);

        using var binding = source.BindTo(target, t => t.Name);

        await Assert.That(target.Name).IsEqualTo(InitialText);

        source.OnNext(UpdatedText);
        await Assert.That(target.Name).IsEqualTo(UpdatedText);

        source.OnNext("Final");
        await Assert.That(target.Name).IsEqualTo("Final");
    }

    /// <summary>
    /// Verifies that a BindTo binding applies the supplied converter override before setting the target.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindTo_WithConverter_ConvertsValue()
    {
        const int InitialValue = 42;
        const int UpdatedValue = 100;
        var target = new TestViewModel();
        var source = new BehaviorSubject<int>(InitialValue);

        using var binding = source.BindTo(
            target,
            t => t.Name,
            vmToViewConverterOverride: new FuncBindingTypeConverter<int, string>(i => $"Number: {i}"));

        await Assert.That(target.Name).IsEqualTo("Number: 42");

        source.OnNext(UpdatedValue);
        await Assert.That(target.Name).IsEqualTo("Number: 100");
    }

    /// <summary>
    /// Verifies that a one-way binding stops updating the view after it is disposed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task OneWayBind_AfterDispose_StopsUpdating()
    {
        var viewModel = new TestViewModel { Name = InitialText };
        var view = new TestView { ViewModel = viewModel };

        using (var binding = view.OneWayBind(viewModel, vm => vm.Name, v => v.NameText))
        {
            await Assert.That(view.NameText).IsEqualTo(InitialText);
        }

        viewModel.Name = ChangedText;
        await Assert.That(view.NameText).IsEqualTo(InitialText);
    }

    /// <summary>
    /// Verifies that a one-way binding updates the view but does not propagate view changes to the view model.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task OneWayBind_UpdatesViewOnly()
    {
        var viewModel = new TestViewModel { Name = InitialText };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.OneWayBind(viewModel, vm => vm.Name, v => v.NameText);

        // VM to View
        await Assert.That(view.NameText).IsEqualTo(InitialText);

        viewModel.Name = UpdatedText;
        await Assert.That(view.NameText).IsEqualTo(UpdatedText);

        // View to VM should not work
        view.NameText = "ViewChange";
        await Assert.That(viewModel.Name).IsEqualTo(UpdatedText);
    }

    /// <summary>
    /// Verifies that a one-way binding with a null view model does not throw.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
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

    /// <summary>
    /// Verifies that a one-way binding applies the supplied selector to transform the value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task OneWayBind_WithSelector_TransformsValue()
    {
        const int InitialCount = 42;
        const int UpdatedCount = 100;
        var viewModel = new TestViewModel { Count = InitialCount };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.OneWayBind(
            viewModel,
            vm => vm.Count,
            v => v.NameText,
            count => $"The count is: {count}");

        await Assert.That(view.NameText).IsEqualTo("The count is: 42");

        viewModel.Count = UpdatedCount;
        await Assert.That(view.NameText).IsEqualTo("The count is: 100");
    }

    /// <summary>
    /// Verifies that a one-way binding automatically converts between differing property types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task OneWayBind_WithTypeConverter_ConvertsTypes()
    {
        const int InitialCount = 42;
        const int UpdatedCount = 100;
        var viewModel = new TestViewModel { Count = InitialCount };
        var view = new TestView { ViewModel = viewModel };

        using var binding = view.OneWayBind(
            viewModel,
            vm => vm.Count,
            v => v.NameText);

        await Assert.That(view.NameText).IsEqualTo("42");

        viewModel.Count = UpdatedCount;
        await Assert.That(view.NameText).IsEqualTo("100");
    }

    /// <summary>
    /// Test binding type converter that delegates conversion to a supplied function.
    /// </summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The target type.</typeparam>
    private sealed class FuncBindingTypeConverter<TFrom, TTo> : IBindingTypeConverter
    {
        private const int HighAffinity = 100;
        private readonly Func<TFrom, TTo> _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncBindingTypeConverter{TFrom, TTo}"/> class.
        /// </summary>
        /// <param name="converter">The conversion function to apply.</param>
        public FuncBindingTypeConverter(Func<TFrom, TTo> converter) => _converter = converter;

        /// <inheritdoc/>
        public Type FromType => typeof(TFrom);

        /// <inheritdoc/>
        public Type ToType => typeof(TTo);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => HighAffinity;

        /// <inheritdoc/>
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

    /// <summary>
    /// Test helper view class.
    /// </summary>
    private sealed class TestView : ReactiveObject, IViewFor<TestViewModel>
    {
        private string? _nameText;
        private TestViewModel? _viewModel;

        /// <summary>
        /// Gets or sets the text displayed for the bound name.
        /// </summary>
        public string? NameText
        {
            get => _nameText;
            set => this.RaiseAndSetIfChanged(ref _nameText, value);
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public TestViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>
    /// Test helper view model class.
    /// </summary>
    private sealed class TestViewModel : ReactiveObject
    {
        private int _count;
        private string? _name;

        /// <summary>
        /// Gets or sets the count value.
        /// </summary>
        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }

        /// <summary>
        /// Gets or sets the name value.
        /// </summary>
        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}
