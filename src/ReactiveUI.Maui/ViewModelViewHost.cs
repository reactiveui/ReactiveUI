// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;

namespace ReactiveUI.Maui;

/// <summary>
/// This content view will automatically load and host the view for the given view model. The view model whose view is
/// to be displayed should be assigned to the <see cref="ViewModel"/> property. Optionally, the chosen view can be
/// customized by specifying a contract via <see cref="ViewContractObservable"/> or <see cref="ViewContract"/>.
/// </summary>
public class ViewModelViewHost : ContentView, IViewFor
{
    /// <summary>
    /// Identifies the <see cref="ViewModel"/> property.
    /// </summary>
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
     nameof(ViewModel),
     typeof(object),
     typeof(ViewModelViewHost));

    /// <summary>
    /// Identifies the <see cref="DefaultContent"/> property.
    /// </summary>
    public static readonly BindableProperty DefaultContentProperty = BindableProperty.Create(
     nameof(DefaultContent),
     typeof(View),
     typeof(ViewModelViewHost),
     default(View));

    /// <summary>
    /// Identifies the <see cref="ViewContractObservable"/> property.
    /// </summary>
    public static readonly BindableProperty ViewContractObservableProperty = BindableProperty.Create(
     nameof(ViewContractObservable),
     typeof(IObservable<string>),
     typeof(ViewModelViewHost),
     Observable<string>.Never);

    /// <summary>
    ///  The ContractFallbackByPass dependency property.
    /// </summary>
    public static readonly BindableProperty ContractFallbackByPassProperty = BindableProperty.Create(
        nameof(ContractFallbackByPass),
        typeof(bool),
        typeof(ViewModelViewHost),
        false);

    private string? _viewContract;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelViewHost"/> class.
    /// </summary>
    public ViewModelViewHost()
    {
        // NB: InUnitTestRunner also returns true in Design Mode
        if (ModeDetector.InUnitTestRunner())
        {
            ViewContractObservable = Observable<string>.Never;
            return;
        }

        ViewContractObservable = Observable<string>.Default;

        var vmAndContract = this.WhenAnyValue(x => x.ViewModel).CombineLatest(
                                                                              this.WhenAnyObservable(x => x.ViewContractObservable),
                                                                              (vm, contract) => new { ViewModel = vm, Contract = contract, });

        this.WhenActivated(() =>
            [
                vmAndContract.Subscribe(x =>
                {
                    _viewContract = x.Contract;

                    ResolveViewForViewModel(x.ViewModel, x.Contract);
                })
            ]);
    }

    /// <summary>
    /// Gets or sets the view model whose associated view is to be displayed.
    /// </summary>
    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the content to display when <see cref="ViewModel"/> is <see langword="null"/>.
    /// </summary>
    public View DefaultContent
    {
        get => (View)GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the observable which signals when the contract to use when resolving the view for the given view model has changed.
    /// </summary>
    public IObservable<string?> ViewContractObservable
    {
        get => (IObservable<string>)GetValue(ViewContractObservableProperty);
        set => SetValue(ViewContractObservableProperty, value);
    }

    /// <summary>
    /// Gets or sets the fixed contract to use when resolving the view for the given view model.
    /// </summary>
    /// <remarks>
    /// This property is a mere convenience so that a fixed contract can be assigned directly in XAML.
    /// </remarks>
    public string? ViewContract
    {
        get => _viewContract;
        set => ViewContractObservable = Observable.Return(value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether should bypass the default contract fallback behavior.
    /// </summary>
    public bool ContractFallbackByPass
    {
        get => (bool)GetValue(ContractFallbackByPassProperty);
        set => SetValue(ContractFallbackByPassProperty, value);
    }

    /// <summary>
    /// Gets or sets the override for the view locator to use when resolving the view. If unspecified, <see cref="ViewLocator.Current"/> will be used.
    /// </summary>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>
    /// resolve view for view model with respect to contract.
    /// </summary>
    /// <param name="viewModel">ViewModel.</param>
    /// <param name="contract">contract used by ViewLocator.</param>
    protected virtual void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;

        var viewInstance = viewLocator.ResolveView(viewModel, contract);
        if (viewInstance is null && !ContractFallbackByPass)
        {
            viewInstance = viewLocator.ResolveView(viewModel);
        }

        if (viewInstance is null)
        {
            throw new Exception($"Couldn't find view for '{viewModel}'.");
        }

        if (viewInstance is not View castView)
        {
            throw new Exception($"View '{viewInstance.GetType().FullName}' is not a subclass of '{typeof(View).FullName}'.");
        }

        viewInstance.ViewModel = viewModel;

        Content = castView;
    }
}
