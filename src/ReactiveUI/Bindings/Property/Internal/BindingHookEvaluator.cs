// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Default implementation of <see cref="IBindingHookEvaluator"/> that evaluates binding hooks.
/// </summary>
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
        Expression vmExpression,
        Expression viewExpression,
        BindingDirection direction)
        where TViewModel : class
        where TView : class, IViewFor
    {
        var hooks = AppLocator.Current.GetServices<IPropertyBindingHook>();
        ArgumentExceptionHelper.ThrowIfNull(view);

        // Compile chains once for hook evaluation.
        var vmChainGetter = vmExpression != null
            ? new Reflection.CompiledPropertyChain<object?, object?>([.. vmExpression.GetExpressionChain()])
            : null;
        var viewChainGetter = new Reflection.CompiledPropertyChain<TView, object?>([.. viewExpression.GetExpressionChain()]);

        Func<IObservedChange<object, object?>[]> vmFetcher = vmExpression is not null
            ? (() =>
            {
                if (viewModel is null)
                {
                    return [];
                }

                vmChainGetter!.TryGetAllValues(viewModel, out var fetchedValues);
                return fetchedValues;
            })
            : (() => [new ObservedChange<object, object?>(null!, null, viewModel)]);

        Func<IObservedChange<object, object?>[]> vFetcher = () =>
        {
            viewChainGetter.TryGetAllValues(view, out var fetchedValues);
            return fetchedValues;
        };

        // Replace Aggregate with a loop to avoid enumerator overhead and closures.
        var shouldBind = true;
        foreach (var hook in hooks)
        {
            if (hook is null)
            {
                continue;
            }

            if (!hook.ExecuteHook(viewModel, view!, vmFetcher!, vFetcher!, direction))
            {
                shouldBind = false;
                break;
            }
        }

        if (!shouldBind)
        {
            var vmString = $"{typeof(TViewModel).Name}.{string.Join(".", vmExpression)}";
            var vString = $"{typeof(TView).Name}.{string.Join(".", viewExpression)}";
            LogHost.Default.Warn($"Binding hook asked to disable binding {vmString} => {vString}");
        }

        return shouldBind;
    }
}
