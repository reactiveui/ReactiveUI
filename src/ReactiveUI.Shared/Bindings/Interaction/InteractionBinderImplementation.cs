// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides methods to bind <see cref="Interaction{TInput, TOutput}"/>s to handlers.</summary>
public class InteractionBinderImplementation : IInteractionBinderImplementation
{
    /// <inheritdoc />
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public IDisposable BindInteraction<TViewModel, TView, TInput, TOutput>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, Task> handler)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(handler);

        return BindInteractionCore<TViewModel, TView, TInput, TOutput>(
            viewModel,
            view,
            propertyName,
            interaction => interaction is null ? EmptyDisposable.Instance : interaction.RegisterHandler(handler));
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public IDisposable BindInteraction<TViewModel, TView, TInput, TOutput, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(handler);

        return BindInteractionCore<TViewModel, TView, TInput, TOutput>(
            viewModel,
            view,
            propertyName,
            interaction => interaction is null ? EmptyDisposable.Instance : interaction.RegisterHandler(handler));
    }

    /// <summary>
    /// Builds the interaction-binding pipeline shared by the <c>BindInteraction</c> overloads: observes the current and
    /// reassigned view models, registers the supplied handler against the latest interaction (swapping out the previous
    /// registration), and tears everything down when disposed.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TInput">The interaction input type.</typeparam>
    /// <typeparam name="TOutput">The interaction output type.</typeparam>
    /// <param name="viewModel">The view model containing the interaction.</param>
    /// <param name="view">The view.</param>
    /// <param name="propertyName">An expression selecting the interaction on the view model.</param>
    /// <param name="register">Registers the supplied handler against an interaction (or a no-op for a null interaction).</param>
    /// <returns>A disposable that tears down the binding.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    private DisposableBag BindInteractionCore<TViewModel, TView, TInput, TOutput>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteraction<TInput, TOutput>?, IDisposable> register)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        var viewModelExpression = Reflection.Rewrite(propertyName.Body);

        var viewModelNulls = view.WhenAnyValue(x => x.ViewModel)
            .Choose(static x => x is null ? (true, (IInteraction<TInput, TOutput>?)null) : (false, null));
        var source = Signal.Blend<IInteraction<TInput, TOutput>?>(
            new MapSignal<object, IInteraction<TInput, TOutput>?>(
                Reflection.ViewModelWhenAnyValue(viewModel, view, viewModelExpression),
                static x => (IInteraction<TInput, TOutput>?)x),
            viewModelNulls);

        var registration = new SwapDisposable();
        var subscription = source.Subscribe(new InteractionRegistrationObserver<TInput, TOutput>(
            registration,
            register,
            this,
            $"{viewModelExpression} Interaction Binding received an Exception!"));
        return new(subscription, registration);
    }

    /// <summary>
    /// Registers the latest interaction's handler (swapping out the previous registration), logs binding errors, and
    /// disposes the registration when the source terminates. Fuses the prior <c>Do</c> + <c>Finally</c> + <c>Subscribe</c>.
    /// </summary>
    /// <typeparam name="TInput">The interaction input type.</typeparam>
    /// <typeparam name="TOutput">The interaction output type.</typeparam>
    /// <param name="registration">Holds the current handler registration, disposing the previous on assignment.</param>
    /// <param name="register">Registers a handler for the supplied interaction (or a no-op for null).</param>
    /// <param name="logHost">The object used for logging.</param>
    /// <param name="errorMessage">Logged when the binding errors.</param>
    private sealed class InteractionRegistrationObserver<TInput, TOutput>(
        SwapDisposable registration,
        Func<IInteraction<TInput, TOutput>?, IDisposable> register,
        IEnableLogger logHost,
        string errorMessage) : IObserver<IInteraction<TInput, TOutput>?>
    {
        /// <inheritdoc/>
        public void OnNext(IInteraction<TInput, TOutput>? value) => registration.Disposable = register(value);

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            logHost.Log().Error(error, errorMessage);
            registration.Dispose();
        }

        /// <inheritdoc/>
        public void OnCompleted() => registration.Dispose();
    }
}
