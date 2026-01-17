// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// This class provides extension methods for the ReactiveUI view binding mechanism.
/// </summary>
/// <remarks>
/// <para>
/// Interaction bindings are usually established within a view's activation block to ensure registrations are disposed
/// when the view is no longer visible. The helpers resolve the <see cref="IInteraction{TInput,TOutput}"/> on the view
/// model via an expression and hook it to a handler that can await UI prompts such as dialogs.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// this.WhenActivated(disposables =>
/// {
///     this.BindInteraction(ViewModel, vm => vm.ShowDialog, HandleDialogAsync)
///         .DisposeWith(disposables);
/// });
/// ]]>
/// </code>
/// </example>
public static class InteractionBindingMixins
{
    private static readonly InteractionBinderImplementation _binderImplementation = new();

    /// <summary>
    /// Binds the <see cref="IInteraction{TInput, TOutput}"/> on a ViewModel to the specified handler.
    /// </summary>
    /// <param name="view">The view to bind to.</param>
    /// <param name="viewModel">The view model to bind to.</param>
    /// <param name="propertyName">The name of the property on the View Model.</param>
    /// <param name="handler">The handler.</param>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TInput">The interaction's input type.</typeparam>
    /// <typeparam name="TOutput">The interaction's output type.</typeparam>
    /// <returns>An object that when disposed, disconnects the binding.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public static IDisposable BindInteraction<TViewModel, TView, TInput, TOutput>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, Task> handler)
            where TViewModel : class
            where TView : class, IViewFor =>
        _binderImplementation.BindInteraction(
                viewModel,
                view,
                propertyName,
                handler);

    /// <summary>
    /// Binds the <see cref="IInteraction{TInput, TOutput}"/> on a ViewModel to the specified handler.
    /// </summary>
    /// <param name="view">The view to bind to.</param>
    /// <param name="viewModel">The view model to bind to.</param>
    /// <param name="propertyName">The name of the property on the View Model.</param>
    /// <param name="handler">The handler.</param>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TInput">The interaction's input type.</typeparam>
    /// <typeparam name="TOutput">The interaction's output type.</typeparam>
    /// <typeparam name="TDontCare">The interaction's signal type.</typeparam>
    /// <returns>An object that when disposed, disconnects the binding.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public static IDisposable BindInteraction<TViewModel, TView, TInput, TOutput, TDontCare>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
            where TViewModel : class
            where TView : class, IViewFor =>
        _binderImplementation.BindInteraction(
            viewModel,
            view,
            propertyName,
            handler);
}
