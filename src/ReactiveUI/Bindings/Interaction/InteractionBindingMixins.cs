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
    /// Binds an interaction from a view model to a view, allowing the view to handle interaction requests using the
    /// specified handler.
    /// </summary>
    /// <remarks>This method enables the view to respond to interaction requests initiated by the view model,
    /// such as displaying dialogs or requesting user input. The returned IDisposable should be disposed when the
    /// binding is no longer needed, such as when the view is unloaded, to avoid memory leaks. This method uses
    /// reflection and may not be compatible with trimming or AOT scenarios.</remarks>
    /// <typeparam name="TViewModel">The type of the view model that contains the interaction property.</typeparam>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <typeparam name="TInput">The type of the input parameter for the interaction.</typeparam>
    /// <typeparam name="TOutput">The type of the output parameter for the interaction.</typeparam>
    /// <param name="view">The view instance that will handle the interaction.</param>
    /// <param name="viewModel">The view model instance containing the interaction property. Can be null if the view is not currently bound to a
    /// view model.</param>
    /// <param name="propertyName">An expression identifying the interaction property on the view model to bind.</param>
    /// <param name="handler">A function that will be invoked to handle each interaction request. Receives the interaction context and returns
    /// a task representing the asynchronous operation.</param>
    /// <returns>An IDisposable that can be disposed to unbind the interaction and release associated resources.</returns>
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
    /// Binds an interaction from a view model to a view, allowing the view to handle interaction requests using the
    /// specified handler.
    /// </summary>
    /// <remarks>This method enables the view to respond to interaction requests initiated by the view model,
    /// typically for user-driven workflows such as dialogs or prompts. The handler is invoked each time the interaction
    /// is triggered. The returned IDisposable should be disposed when the binding is no longer needed, such as when the
    /// view is unloaded.</remarks>
    /// <typeparam name="TViewModel">The type of the view model containing the interaction property.</typeparam>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <typeparam name="TInput">The type of the input parameter for the interaction.</typeparam>
    /// <typeparam name="TOutput">The type of the output parameter for the interaction.</typeparam>
    /// <typeparam name="TDontCare">The type of the value produced by the handler observable. This value is ignored.</typeparam>
    /// <param name="view">The view instance that will handle the interaction.</param>
    /// <param name="viewModel">The view model instance containing the interaction property. Can be null if the view is not currently bound to a
    /// view model.</param>
    /// <param name="propertyName">An expression identifying the interaction property on the view model to bind.</param>
    /// <param name="handler">A function that handles the interaction by processing the interaction context and returning an observable
    /// sequence. The result of the observable is ignored.</param>
    /// <returns>An IDisposable that can be disposed to unbind the interaction and release associated resources.</returns>
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
