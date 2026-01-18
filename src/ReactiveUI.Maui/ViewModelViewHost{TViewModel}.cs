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
/// <typeparam name="TViewModel">The type of the view model. Must have a public parameterless constructor for AOT compatibility.</typeparam>
/// <remarks>
/// This is the AOT-compatible generic version of ViewModelViewHost. It uses compile-time type information
/// to resolve views without reflection, making it safe for Native AOT and trimming scenarios.
/// </remarks>
public partial class ViewModelViewHost<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel> : ContentView, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>
    /// Identifies the <see cref="ViewModel"/> property.
    /// </summary>
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
     nameof(ViewModel),
     typeof(TViewModel),
     typeof(ViewModelViewHost<TViewModel>));

    /// <summary>
    /// Identifies the <see cref="DefaultContent"/> property.
    /// </summary>
    public static readonly BindableProperty DefaultContentProperty = BindableProperty.Create(
     nameof(DefaultContent),
     typeof(View),
     typeof(ViewModelViewHost<TViewModel>),
     default(View));

    /// <summary>
    /// Identifies the <see cref="ViewContractObservable"/> property.
    /// </summary>
    public static readonly BindableProperty ViewContractObservableProperty = BindableProperty.Create(
     nameof(ViewContractObservable),
     typeof(IObservable<string>),
     typeof(ViewModelViewHost<TViewModel>),
     Observable<string>.Never);

    /// <summary>
    ///  The ContractFallbackByPass dependency property.
    /// </summary>
    public static readonly BindableProperty ContractFallbackByPassProperty = BindableProperty.Create(
        nameof(ContractFallbackByPass),
        typeof(bool),
        typeof(ViewModelViewHost<TViewModel>),
        false);

    private readonly CompositeDisposable _subscriptions = [];
    private string? _viewContract;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelViewHost{TViewModel}"/> class.
    /// </summary>
    public ViewModelViewHost()
    {
        // NB: InUnitTestRunner also returns true in Design Mode
        if (ModeDetector.InUnitTestRunner())
        {
            ViewContractObservable = Observable<string>.Never;
            return;
        }

        InitializeViewResolution();
    }

    /// <summary>
    /// Gets or sets the view model whose associated view is to be displayed.
    /// </summary>
    public TViewModel? ViewModel
    {
        get => (TViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the view model whose associated view is to be displayed.
    /// </summary>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
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
    /// Resolves and displays the view for the given view model with respect to the contract.
    /// This method uses the generic ResolveView method which is AOT-compatible.
    /// </summary>
    /// <param name="viewModel">The view model to resolve a view for.</param>
    /// <param name="contract">The contract to use when resolving the view.</param>
    /// <remarks>
    /// This method is excluded from code coverage because it is only called from <see cref="InitializeViewResolution"/>,
    /// which cannot be executed during unit tests due to the <see cref="ModeDetector.InUnitTestRunner"/> check.
    /// This code is exercised in integration tests and production runtime scenarios.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    protected virtual void ResolveViewForViewModel(TViewModel? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;

        // Use the generic ResolveView<TViewModel> method - this is AOT-safe!
        var viewInstance = viewLocator.ResolveView<TViewModel>(contract);
        if (viewInstance is null && !ContractFallbackByPass)
        {
            viewInstance = viewLocator.ResolveView<TViewModel>();
        }

        if (viewInstance is null)
        {
            throw new InvalidOperationException($"Couldn't find view for '{viewModel}'.");
        }

        if (viewInstance is not View castView)
        {
            throw new InvalidOperationException($"View '{viewInstance.GetType().FullName}' is not a subclass of '{typeof(View).FullName}'.");
        }

        viewInstance.ViewModel = viewModel;

        Content = castView;
    }

    /// <summary>
    /// Initializes the view resolution subscription for runtime (non-test) scenarios.
    /// </summary>
    /// <remarks>
    /// This method is excluded from code coverage because it cannot be executed during unit tests.
    /// The <see cref="ModeDetector.InUnitTestRunner"/> check in the constructor returns early in test mode,
    /// preventing this initialization code from running. This is by design - the automatic view resolution
    /// subscription would interfere with unit tests by resolving views asynchronously.
    /// This code is exercised in integration tests and production runtime scenarios.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    private void InitializeViewResolution()
    {
        ViewContractObservable = Observable<string>.Default;

        // Observe ViewModel property changes without expression trees (AOT-friendly)
        var viewModelChanged = MauiReactiveHelpers.CreatePropertyValueObservable(
            this,
            nameof(ViewModel),
            () => ViewModel);

        // Combine ViewModel and ViewContractObservable streams
        var vmAndContract = viewModelChanged.CombineLatest(
            ViewContractObservable,
            (vm, contract) => new { ViewModel = vm, Contract = contract });

        // Subscribe directly without WhenActivated
        vmAndContract
            .Subscribe(x =>
            {
                _viewContract = x.Contract;
                ResolveViewForViewModel(x.ViewModel, x.Contract);
            })
            .DisposeWith(_subscriptions);
    }
}
