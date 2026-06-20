// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Default implementation of <see cref="IBindingHookEvaluator"/> that evaluates binding hooks.</summary>
/// <remarks>
/// This service evaluates registered IPropertyBindingHook instances for pre-binding validation.
/// Hooks can reject bindings based on custom logic, property names, types, or other criteria.
/// </remarks>
[RequiresUnreferencedCode("Uses Splat which may require dynamic type resolution for hook services.")]
internal class BindingHookEvaluator : IBindingHookEvaluator
{
    /// <inheritdoc/>
    public bool EvaluateBindingHooks<TViewModel, TView>(
        TViewModel? viewModel,
        TView view,
        Expression viewModelExpression,
        Expression viewExpression,
        BindingDirection direction)
        where TViewModel : class
        where TView : class, IViewFor
    {
        var hooks = AppLocator.Current.GetServices<IPropertyBindingHook>();
        ArgumentExceptionHelper.ThrowIfNull(view);

        var viewModelChain = viewModelExpression is not null
            ? new Reflection.CompiledPropertyChain<object?, object?>([.. viewModelExpression.GetExpressionChain()])
            : null;
        Reflection.CompiledPropertyChain<TView, object?> viewChainGetter = new([.. viewExpression.GetExpressionChain()]);

        Func<IObservedChange<object, object?>[]> viewModelFetcher = viewModelExpression is not null
            ? () =>
            {
                if (viewModel is null)
                {
                    return [];
                }

                viewModelChain!.TryGetAllValues(viewModel, out var fetchedValues);
                return fetchedValues;
            }
        : () => [new ObservedChange<object, object?>(null!, null, viewModel)];

        var viewFetcher = () =>
        {
            viewChainGetter.TryGetAllValues(view, out var fetchedValues);
            return fetchedValues;
        };

        var shouldBind = true;
        foreach (var hook in hooks)
        {
            if (hook is null)
            {
                continue;
            }

            if (hook.ExecuteHook(viewModel, view, viewModelFetcher!, viewFetcher!, direction))
            {
                continue;
            }

            shouldBind = false;
            break;
        }

        if (shouldBind)
        {
            return shouldBind;
        }

        var viewModelString = $"{typeof(TViewModel).Name}.{string.Join(".", viewModelExpression)}";
        var viewString = $"{typeof(TView).Name}.{string.Join(".", viewExpression)}";
        LogHost.Default.Warn($"Binding hook asked to disable binding {viewModelString} => {viewString}");

        return shouldBind;
    }
}
