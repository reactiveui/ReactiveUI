// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Implementation logic for <see cref="Interaction{TInput, TOutput}"/> binding.
/// </summary>
public interface IInteractionBinderImplementation : IEnableLogger
{
    /// <summary>
    /// Binds the <see cref="IInteraction{TInput, TOutput}"/> on a ViewModel to the specified handler.
    /// </summary>
    /// <param name="viewModel">The view model to bind to.</param>
    /// <param name="view">The view to bind to.</param>
    /// <param name="propertyName">The name of the property on the View Model.</param>
    /// <param name="handler">The handler.</param>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TInput">The interaction's input type.</typeparam>
    /// <typeparam name="TOutput">The interaction's output type.</typeparam>
    /// <returns>An object that when disposed, disconnects the binding.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("BindInteraction uses expression evaluation which requires dynamic code generation")]
    [RequiresUnreferencedCode("BindInteraction uses expression evaluation which may require unreferenced code")]
#endif
    IDisposable BindInteraction<TViewModel, TView, TInput, TOutput>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, Task> handler)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Binds the <see cref="IInteraction{TInput, TOutput}"/> on a ViewModel to the specified handler.
    /// </summary>
    /// <param name="viewModel">The view model to bind to.</param>
    /// <param name="view">The view to bind to.</param>
    /// <param name="propertyName">The name of the property on the View Model.</param>
    /// <param name="handler">The handler.</param>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TInput">The interaction's input type.</typeparam>
    /// <typeparam name="TOutput">The interaction's output type.</typeparam>
    /// <typeparam name="TDontCare">The interaction's signal type.</typeparam>
    /// <returns>An object that when disposed, disconnects the binding.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("BindInteraction uses expression evaluation which requires dynamic code generation")]
    [RequiresUnreferencedCode("BindInteraction uses expression evaluation which may require unreferenced code")]
#endif
    IDisposable BindInteraction<TViewModel, TView, TInput, TOutput, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
        where TViewModel : class
        where TView : class, IViewFor;
}
