// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

#if REACTIVE_SHIM
using ReactiveUI.Binding.Reactive.Fallback;
#else
using ReactiveUI.Binding.Fallback;
#endif

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Drives view-first two-way bindings from an update signal. Binding offers <c>signalViewUpdate</c> and
/// <see cref="TriggerUpdate"/> only on the runtime <c>BindUnsafe</c> overloads, which take conversion delegates, so
/// these helpers adapt the converter and hint arguments the scenarios are written with.
/// </summary>
internal static class TriggeredBindingMixins
{
    /// <summary>Provides the triggered bindings for a view.</summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="view">The view.</param>
    extension<TView>(TView view)
        where TView : class, IViewFor
    {
        /// <summary>Binds two properties, converting with delegates, and updates the side the trigger names on each signal.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TVMProp">The view model property type.</typeparam>
        /// <typeparam name="TVProp">The view property type.</typeparam>
        /// <typeparam name="TDontCare">The signal payload, which is ignored.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewModelProperty">The view model property.</param>
        /// <param name="viewProperty">The view property.</param>
        /// <param name="signalViewUpdate">The update signal.</param>
        /// <param name="viewModelToViewConverter">Converts view model values for the view.</param>
        /// <param name="viewToViewModelConverter">Converts view values for the view model.</param>
        /// <param name="triggerUpdate">The direction the signal drives.</param>
        /// <returns>The binding.</returns>
        internal IReactiveBinding<TView, BindingChange> BindTriggered<TViewModel, TVMProp, TVProp, TDontCare>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TVMProp>> viewModelProperty,
            Expression<Func<TView, TVProp>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            Func<TVMProp, TVProp> viewModelToViewConverter,
            Func<TVProp, TVMProp> viewToViewModelConverter,
            TriggerUpdate triggerUpdate)
            where TViewModel : class =>
            view.BindUnsafe(viewModel, viewModelProperty, viewProperty, viewModelToViewConverter, viewToViewModelConverter, signalViewUpdate, triggerUpdate);

        /// <summary>Binds two properties through binding converters, and updates the side the trigger names on each signal.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TVMProp">The view model property type.</typeparam>
        /// <typeparam name="TVProp">The view property type.</typeparam>
        /// <typeparam name="TDontCare">The signal payload, which is ignored.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewModelProperty">The view model property.</param>
        /// <param name="viewProperty">The view property.</param>
        /// <param name="signalViewUpdate">The update signal.</param>
        /// <param name="converters">The converters for each direction and the hint handed to them.</param>
        /// <param name="triggerUpdate">The direction the signal drives.</param>
        /// <returns>The binding.</returns>
        internal IReactiveBinding<TView, BindingChange> BindTriggered<TViewModel, TVMProp, TVProp, TDontCare>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TVMProp>> viewModelProperty,
            Expression<Func<TView, TVProp>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            TriggeredConverters converters,
            TriggerUpdate triggerUpdate)
            where TViewModel : class
        {
            var readViewModel = viewModelProperty.Compile();
            var readView = viewProperty.Compile();
            return view.BindUnsafe(
                viewModel,
                viewModelProperty,
                viewProperty,
                value => Convert(value, converters.Hint, converters.ToView, () => readView(view)),
                value => Convert(value, converters.Hint, converters.ToViewModel, () => viewModel is null ? default! : readViewModel(viewModel)),
                signalViewUpdate,
                triggerUpdate);
        }
    }

    /// <summary>Converts a value through a converter, falling back to the registered converters.</summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The target type.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="conversionHint">The hint handed to the converter.</param>
    /// <param name="converter">The converter, or null for the registered converters.</param>
    /// <param name="current">Reads the destination's current value.</param>
    /// <returns>The converted value, or the destination's current value when no converter accepts it, so the binding skips the write.</returns>
    private static TTo Convert<TFrom, TTo>(TFrom value, object? conversionHint, IBindingTypeConverter? converter, Func<TTo> current) =>
        RuntimeBindingConverter.TryConvert<TFrom, TTo>(value, conversionHint, converter, out var result)
        || RuntimeBindingConverter.TryConvert(value, conversionHint, null, out result)
            ? result!
            : current();
}
