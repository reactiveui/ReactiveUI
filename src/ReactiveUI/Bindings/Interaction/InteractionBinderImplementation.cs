// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;
using Splat;

namespace ReactiveUI;

/// <summary>
/// Provides methods to bind <see cref="Interaction{TInput, TOutput}"/>s to handlers.
/// </summary>
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
        ArgumentExceptionHelper.ThrowIfNull(propertyName);
        ArgumentExceptionHelper.ThrowIfNull(handler);

        var vmExpression = Reflection.Rewrite(propertyName.Body);

        var vmNulls = new ChooseObservable<object?, IInteraction<TInput, TOutput>?>(
            view.WhenAnyValue(x => x.ViewModel),
            static x => x is null ? (true, (IInteraction<TInput, TOutput>?)null) : (false, null));
        var source = new MergeObservable<IInteraction<TInput, TOutput>?>(
        [
            new SelectObservable<object, IInteraction<TInput, TOutput>?>(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression),
                static x => (IInteraction<TInput, TOutput>?)x),
            vmNulls,
        ]);

        var registration = new SwapDisposable();
        var subscription = source.Subscribe(new InteractionRegistrationObserver<TInput, TOutput>(
            registration,
            x => x is null ? EmptyDisposable.Instance : x.RegisterHandler(handler),
            this,
            $"{vmExpression} Interaction Binding received an Exception!"));
        return new CompositeDisposable(subscription, registration);
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
        ArgumentExceptionHelper.ThrowIfNull(propertyName);
        ArgumentExceptionHelper.ThrowIfNull(handler);

        var vmExpression = Reflection.Rewrite(propertyName.Body);

        var vmNulls = new ChooseObservable<object?, IInteraction<TInput, TOutput>?>(
            view.WhenAnyValue(x => x.ViewModel),
            static x => x is null ? (true, (IInteraction<TInput, TOutput>?)null) : (false, null));
        var source = new MergeObservable<IInteraction<TInput, TOutput>?>(
        [
            new SelectObservable<object, IInteraction<TInput, TOutput>?>(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression),
                static x => (IInteraction<TInput, TOutput>?)x),
            vmNulls,
        ]);

        var registration = new SwapDisposable();
        var subscription = source.Subscribe(new InteractionRegistrationObserver<TInput, TOutput>(
            registration,
            x => x is null ? EmptyDisposable.Instance : x.RegisterHandler(handler),
            this,
            $"{vmExpression} Interaction Binding received an Exception!"));
        return new CompositeDisposable(subscription, registration);
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
