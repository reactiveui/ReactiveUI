// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Conversion and change-projection helpers used by the two-way binding pipeline.</summary>
[SuppressMessage("Design", "SST1432:Mark the type as static", Justification = "Partial of a non-static type; the instance members live in the primary PropertyBinderImplementation partial.")]
public partial class PropertyBinderImplementation
{
    /// <summary>Dispatches a single conversion attempt to a converter, choosing the typed or fallback conversion path.</summary>
    /// <param name="converter">The converter to use.</param>
    /// <param name="sourceType">The type being converted from.</param>
    /// <param name="targetType">The type being converted to.</param>
    /// <param name="value">The value to convert.</param>
    /// <param name="conversionHint">An optional hint passed to the converter.</param>
    /// <param name="result">The converted value when conversion succeeds.</param>
    /// <returns><see langword="true"/> when the converter produced a value; otherwise <see langword="false"/>.</returns>
    private static bool TryConvertUsingConverter(
        object converter,
        Type sourceType,
        Type targetType,
        object? value,
        object? conversionHint,
        out object? result)
    {
        switch (converter)
        {
            case IBindingTypeConverter typedConverter:
                return typedConverter.TryConvertTyped(value, conversionHint, out result);
            case IBindingFallbackConverter when value is null:
                {
                    result = null;
                    return false;
                }

            case IBindingFallbackConverter fallbackConverter:
                return fallbackConverter.TryConvert(sourceType, value, targetType, conversionHint, out result);
            default:
                {
                    result = null;
                    return false;
                }
        }
    }

    /// <summary>
    /// Attempts a conversion using an explicitly supplied converter, falling back to a registry
    /// converter registered for the same type pair when the supplied converter does not apply.
    /// </summary>
    /// <param name="converter">The converter explicitly supplied by the caller.</param>
    /// <param name="sourceType">The type being converted from.</param>
    /// <param name="targetType">The type being converted to.</param>
    /// <param name="value">The value to convert.</param>
    /// <param name="conversionHint">An optional hint passed to the converter.</param>
    /// <param name="converted">The converted value when conversion succeeds.</param>
    /// <returns><see langword="true"/> when a value was produced; otherwise <see langword="false"/>.</returns>
    private static bool TryConvertWithOverride(
        object converter,
        Type sourceType,
        Type targetType,
        object? value,
        object? conversionHint,
        out object? converted)
    {
        if (TryConvertUsingConverter(converter, sourceType, targetType, value, conversionHint, out converted))
        {
            return true;
        }

        var fallbackConverter = GetConverterForTypes(sourceType, targetType);
        return fallbackConverter is not null
            && fallbackConverter != converter
            && BindingTypeConverterDispatch.TryConvertAny(fallbackConverter, sourceType, value, targetType, conversionHint, out converted);
    }

    /// <summary>Converts a view model property value into the corresponding view property value.</summary>
    /// <typeparam name="TViewModelPropertyType">The view model property type.</typeparam>
    /// <typeparam name="TViewPropertyType">The view property type.</typeparam>
    /// <param name="converter">The resolved converter, or <see langword="null"/> when no converter applies.</param>
    /// <param name="converterOverride">The converter explicitly supplied by the caller, if any.</param>
    /// <param name="conversionHint">An optional hint passed to the converter.</param>
    /// <param name="viewModelValue">The view model value to convert.</param>
    /// <param name="viewValue">The converted view value.</param>
    /// <returns><see langword="true"/> when a view value was produced; otherwise <see langword="false"/>.</returns>
    private static bool TryConvertViewModelToView<TViewModelPropertyType, TViewPropertyType>(
        object? converter,
        IBindingTypeConverter? converterOverride,
        object? conversionHint,
        TViewModelPropertyType? viewModelValue,
        out TViewPropertyType viewValue)
    {
        if (converter is null)
        {
            viewValue = viewModelValue is TViewPropertyType typedValue ? typedValue : default!;
            return true;
        }

        bool success;
        object? converted;

        if (converterOverride is not null)
        {
            success = TryConvertWithOverride(converter, typeof(TViewModelPropertyType), typeof(TViewPropertyType), viewModelValue, conversionHint, out converted);
        }
        else
        {
            success = BindingTypeConverterDispatch.TryConvertAny(
                converter,
                typeof(TViewModelPropertyType),
                viewModelValue,
                typeof(TViewPropertyType),
                conversionHint,
                out converted);

            if (!success && typeof(TViewPropertyType).IsAssignableFrom(typeof(TViewModelPropertyType)))
            {
                viewValue = viewModelValue is TViewPropertyType fallbackValue ? fallbackValue : default!;
                return true;
            }
        }

        viewValue = success ? (TViewPropertyType)converted! : default!;
        return success;
    }

    /// <summary>Converts a view property value back into the corresponding view model property value.</summary>
    /// <typeparam name="TViewModelPropertyType">The view model property type.</typeparam>
    /// <typeparam name="TViewPropertyType">The view property type.</typeparam>
    /// <param name="converter">The resolved converter, or <see langword="null"/> when no converter applies.</param>
    /// <param name="converterOverride">The converter explicitly supplied by the caller, if any.</param>
    /// <param name="conversionHint">An optional hint passed to the converter.</param>
    /// <param name="viewValue">The view value to convert.</param>
    /// <param name="viewModelValue">The converted view model value.</param>
    /// <returns><see langword="true"/> when a view model value was produced; otherwise <see langword="false"/>.</returns>
    private static bool TryConvertViewToViewModel<TViewModelPropertyType, TViewPropertyType>(
        object? converter,
        IBindingTypeConverter? converterOverride,
        object? conversionHint,
        TViewPropertyType viewValue,
        out TViewModelPropertyType? viewModelValue)
    {
        if (converter is null)
        {
            viewModelValue = viewValue is TViewModelPropertyType typedValue ? typedValue : default;
            return true;
        }

        bool success;
        object? converted;

        if (converterOverride is not null)
        {
            success = TryConvertWithOverride(converter, typeof(TViewPropertyType), typeof(TViewModelPropertyType?), viewValue, conversionHint, out converted);
        }
        else
        {
            success = BindingTypeConverterDispatch.TryConvertAny(
                converter,
                typeof(TViewPropertyType),
                viewValue,
                typeof(TViewModelPropertyType?),
                conversionHint,
                out converted);

            if (!success && typeof(TViewModelPropertyType).IsAssignableFrom(typeof(TViewPropertyType)))
            {
                viewModelValue = viewValue is TViewModelPropertyType fallbackValue ? fallbackValue : default;
                return true;
            }
        }

        viewModelValue = success ? (TViewModelPropertyType?)converted : default;
        return success;
    }

    /// <summary>Projects a change signal into the resolved value to apply, in the correct direction.</summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the view model property.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the view property.</typeparam>
    /// <param name="isViewModelChange"><see langword="true"/> when the view model side changed; otherwise the view side changed.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="viewModelChainGetter">The compiled getter for the view model property chain.</param>
    /// <param name="viewChainGetter">The compiled getter for the view property chain.</param>
    /// <param name="viewModelToViewConverter">Converter from view model value to view value.</param>
    /// <param name="viewToViewModelConverter">Converter from view value to view model value.</param>
    /// <returns>Whether a value should be applied, the value to apply, and the direction to apply it.</returns>
    private static (bool isValid, object? view, bool isViewModel) ProjectChange<TView, TViewModelPropertyType, TViewPropertyType>(
        bool isViewModelChange,
        TView view,
        Reflection.CompiledPropertyChain<object?, TViewModelPropertyType> viewModelChainGetter,
        Reflection.CompiledPropertyChain<TView, TViewPropertyType> viewChainGetter,
        OutFunc<TViewModelPropertyType?, TViewPropertyType> viewModelToViewConverter,
        OutFunc<TViewPropertyType, TViewModelPropertyType?> viewToViewModelConverter)
        where TView : class, IViewFor
    {
        if (!viewModelChainGetter.TryGetValue(view.ViewModel, out var viewModelValue) ||
            !viewChainGetter.TryGetValue(view, out var viewValue))
        {
            return (false, null, false);
        }

        if (isViewModelChange)
        {
            return !viewModelToViewConverter(viewModelValue, out var viewModelAsView) ||
                EqualityComparer<TViewPropertyType>.Default.Equals(viewValue, viewModelAsView) ? (false, null, false) : (true, viewModelAsView, true);
        }

        return !viewToViewModelConverter(viewValue, out var viewAsViewModel) ||
            EqualityComparer<TViewModelPropertyType?>.Default.Equals(viewModelValue, viewAsViewModel) ? (false, null, false) : (true, viewAsViewModel, false);
    }

    /// <summary>Builds the merged observable that signals when either the view model or the view side changed.</summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TDontCare">The element type of the optional view update signal.</typeparam>
    /// <param name="triggerUpdate">Specifies which direction triggers the initial update.</param>
    /// <param name="signalViewUpdate">An optional observable that signals view property changes.</param>
    /// <param name="viewModel">The view model instance.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="viewModelExpression">The rewritten view model property expression.</param>
    /// <param name="viewChanges">An observable that emits when the view property changes.</param>
    /// <param name="signalInitialUpdate">A subject used to push the initial update through the pipeline.</param>
    /// <returns>An observable emitting <see langword="true"/> for view model changes and <see langword="false"/> for view changes.</returns>
    private static IObservable<bool> BuildChangeSource<TViewModel, TView, TDontCare>(
        TriggerUpdate triggerUpdate,
        IObservable<TDontCare>? signalViewUpdate,
        TViewModel? viewModel,
        TView view,
        Expression viewModelExpression,
        IObservable<bool> viewChanges,
        Signal<bool> signalInitialUpdate)
        where TViewModel : class
        where TView : class, IViewFor
    {
        var initialUpdateSignal = new MapSignal<bool, bool>(signalInitialUpdate, static _ => true);
        var viewModelSignal = new MapSignal<object, bool>(
            Reflection.ViewModelWhenAnyValue(viewModel, view, viewModelExpression),
            static _ => true);

        switch (triggerUpdate)
        {
            case TriggerUpdate.ViewToViewModel:
                {
                    var signalObservable = signalViewUpdate is not null
                        ? new MapSignal<TDontCare, bool>(signalViewUpdate, static _ => false)
                        : viewChanges;

                    return Signal.Blend<bool>(viewModelSignal, initialUpdateSignal, signalObservable);
                }

            default:
                {
                    var primary = signalViewUpdate is null
                        ? (IObservable<bool>)viewModelSignal
                        : Signal.Blend<bool>(
                            new MapSignal<TDontCare, bool>(signalViewUpdate, static _ => true),
                            new Take1Observable<bool>(viewModelSignal));

                    return Signal.Blend<bool>(primary, initialUpdateSignal, viewChanges);
                }
        }
    }

    /// <summary>Bundles the inputs required to create a two-way binding so they can be threaded through the binding pipeline.</summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the view model property.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the view property.</typeparam>
    /// <typeparam name="TDontCare">The element type of the optional view update signal.</typeparam>
    private readonly record struct TwoWayBindRequest<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>
        where TViewModel : class
        where TView : class, IViewFor
    {
        /// <summary>Gets the view model instance.</summary>
        public TViewModel? ViewModel { get; init; }

        /// <summary>Gets the view instance.</summary>
        public TView View { get; init; }

        /// <summary>Gets the expression for the view model property.</summary>
        public Expression<Func<TViewModel, TViewModelPropertyType?>> ViewModelProperty { get; init; }

        /// <summary>Gets the expression for the view property.</summary>
        public Expression<Func<TView, TViewPropertyType>> ViewProperty { get; init; }

        /// <summary>Gets an optional observable that signals view property changes.</summary>
        public IObservable<TDontCare>? SignalViewUpdate { get; init; }

        /// <summary>Gets the converter from a view model value to a view value.</summary>
        public OutFunc<TViewModelPropertyType?, TViewPropertyType> ViewModelToViewConverter { get; init; }

        /// <summary>Gets the converter from a view value to a view model value.</summary>
        public OutFunc<TViewPropertyType, TViewModelPropertyType?> ViewToViewModelConverter { get; init; }

        /// <summary>Gets the direction that triggers the initial update.</summary>
        public TriggerUpdate TriggerUpdate { get; init; }
    }
}
