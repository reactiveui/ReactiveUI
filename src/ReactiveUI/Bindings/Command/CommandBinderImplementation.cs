// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// Used by the CommandBinder extension methods to handle binding View controls and ViewModel commands.
/// </summary>
public class CommandBinderImplementation : ICommandBinderImplementation
{
    /// <summary>
    /// Bind a command from the ViewModel to an explicitly specified control
    /// on the View.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TProp">The property type.</typeparam>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="viewModel">The View model.</param>
    /// <param name="view">The View.</param>
    /// <param name="vmProperty">The ViewModel command to bind.</param>
    /// <param name="controlProperty">The name of the control on the view.</param>
    /// <param name="withParameter">The ViewModel property to pass as the
    /// param of the ICommand.</param>
    /// <param name="toEvent">If specified, bind to the specific event
    /// instead of the default.
    /// NOTE: If this parameter is used inside WhenActivated, it's
    /// important to dispose the binding when the view is deactivated.</param>
    /// <returns>
    /// A class representing the binding. Dispose it to disconnect
    /// the binding.
    /// </returns>
    /// <exception cref="ArgumentNullException">nameof(vmProperty)
    /// or
    /// nameof(vmProperty).</exception>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> vmProperty,
        Expression<Func<TView, TControl>> controlProperty,
        Expression<Func<TViewModel, TParam?>> withParameter,
        string? toEvent = null)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand
    {
        vmProperty.ArgumentNullExceptionThrowIfNull(nameof(vmProperty));

        controlProperty.ArgumentNullExceptionThrowIfNull(nameof(controlProperty));

        var vmExpression = Reflection.Rewrite(vmProperty.Body);
        var controlExpression = Reflection.Rewrite(controlProperty.Body);
        var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<TProp>();

        var bindingDisposable = BindCommandInternal(source, view, controlExpression, withParameter.ToObservable(viewModel), toEvent);

        return new ReactiveBinding<TView, TProp>(
                                                 view,
                                                 controlExpression,
                                                 vmExpression,
                                                 source,
                                                 BindingDirection.OneWay,
                                                 bindingDisposable);
    }

    /// <summary>
    /// Bind a command from the ViewModel to an explicitly specified control
    /// on the View.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TProp">The property type.</typeparam>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <returns>A class representing the binding. Dispose it to disconnect
    /// the binding.</returns>
    /// <param name="viewModel">The View model.</param>
    /// <param name="view">The View.</param>
    /// <param name="vmProperty">The ViewModel command to bind.</param>
    /// <param name="controlProperty">The name of the control on the view.</param>
    /// <param name="withParameter">The ViewModel property to pass as the
    /// param of the ICommand.</param>
    /// <param name="toEvent">If specified, bind to the specific event
    /// instead of the default.
    /// NOTE: If this parameter is used inside WhenActivated, it's
    /// important to dispose the binding when the view is deactivated.</param>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> vmProperty,
        Expression<Func<TView, TControl>> controlProperty,
        IObservable<TParam?> withParameter,
        string? toEvent = null)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand
    {
        vmProperty.ArgumentNullExceptionThrowIfNull(nameof(vmProperty));

        controlProperty.ArgumentNullExceptionThrowIfNull(nameof(controlProperty));

        var vmExpression = Reflection.Rewrite(vmProperty.Body);
        var controlExpression = Reflection.Rewrite(controlProperty.Body);
        var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<TProp>();

        var bindingDisposable = BindCommandInternal(source, view, controlExpression, withParameter, toEvent);

        return new ReactiveBinding<TView, TProp>(
                                                 view,
                                                 controlExpression,
                                                 vmExpression,
                                                 source,
                                                 BindingDirection.OneWay,
                                                 bindingDisposable);
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    private static IDisposable BindCommandInternal<TView, TProp, TParam>(
        IObservable<TProp> source,
        TView view,
        Expression controlExpression,
        IObservable<TParam> withParameter,
        string? toEvent)
        where TView : class, IViewFor
        where TProp : ICommand
    {
        var disposable = Disposable.Empty;

        var bindInfo = source.CombineLatest(
                                            view.SubscribeToExpressionChain<TView, object?>(controlExpression, false, false, RxApp.SuppressViewCommandBindingMessage).Select(x => x.GetValue()),
                                            (val, host) => new { val, host });

        var propSub = bindInfo
            .Subscribe(x =>
            {
                if (x.host is null)
                {
                    return;
                }

                disposable.Dispose();
                if (x is null)
                {
                    disposable = Disposable.Empty;
                    return;
                }

                disposable = !string.IsNullOrEmpty(toEvent) ?
                                 CreatesCommandBinding.BindCommandToObject(x.val, x.host, withParameter.Select(y => (object)y!), toEvent) :
                                 CreatesCommandBinding.BindCommandToObject(x.val, x.host, withParameter.Select(y => (object)y!));
            });

        return Disposable.Create(() =>
        {
            propSub.Dispose();
            disposable.Dispose();
        });
    }
}
