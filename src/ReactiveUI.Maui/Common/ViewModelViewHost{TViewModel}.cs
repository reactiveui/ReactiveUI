// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;

namespace ReactiveUI;

/// <summary>
/// This content control will automatically load the View associated with
/// the ViewModel property and display it. This control is very useful
/// inside a DataTemplate to display the View associated with a ViewModel.
/// This generic version provides AOT-compatibility by using compile-time type information.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model. Must have a public parameterless constructor.</typeparam>
public partial class ViewModelViewHost<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel> : TransitioningContentControl, IViewFor<TViewModel>, IEnableLogger
    where TViewModel : class
{
    /// <summary>
    /// The default content dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost<TViewModel>), new PropertyMetadata(null));

    /// <summary>
    /// The view model dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(TViewModel), typeof(ViewModelViewHost<TViewModel>), new PropertyMetadata(null));

    /// <summary>
    /// The view contract observable dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string>), typeof(ViewModelViewHost<TViewModel>), new PropertyMetadata(Observable<string>.Default));

    /// <summary>
    ///  The ContractFallbackByPass dependency property.
    /// </summary>
    public static readonly DependencyProperty ContractFallbackByPassProperty =
        DependencyProperty.Register("ContractFallbackByPass", typeof(bool), typeof(ViewModelViewHost<TViewModel>), new PropertyMetadata(false));

    private readonly CompositeDisposable _subscriptions = [];
    private string? _viewContract;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelViewHost{TViewModel}"/> class.
    /// </summary>
    public ViewModelViewHost()
    {
        var platform = AppLocator.Current.GetService<IPlatformOperations>();
        Func<string?> platformGetter = () => default;

        if (platform is null)
        {
            // NB: This used to be an error but WPF design mode can't read
            // good or do other stuff good.
            this.Log().Error("Couldn't find an IPlatformOperations implementation. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance.");
        }
        else
        {
            platformGetter = () => platform.GetOrientation();
        }

        ViewContractObservable = ModeDetector.InUnitTestRunner()
            ? Observable<string?>.Never
            : Observable.FromEvent<SizeChangedEventHandler, string?>(
              eventHandler =>
              {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
                  void Handler(object? _, SizeChangedEventArgs __) => eventHandler(platformGetter());
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
                  return Handler;
              },
              x => SizeChanged += x,
              x => SizeChanged -= x)
              .StartWith(platformGetter())
              .DistinctUntilChanged();

        // Observe ViewModel property changes without expression trees (AOT-friendly)
        var viewModelChanged = MauiReactiveHelpers.CreatePropertyValueObservable(
            this,
            nameof(ViewModel),
            ViewModelProperty,
            () => ViewModel);

        // Combine contract observable with ViewModel changes
        var vmAndContract = ViewContractObservable
            .Do(x => _viewContract = x)
            .CombineLatest(viewModelChanged, (contract, vm) => (ViewModel: vm, Contract: contract));

        // Subscribe directly without WhenActivated
        ViewContractObservable
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(x => _viewContract = x ?? string.Empty)
            .DisposeWith(_subscriptions);

        vmAndContract
            .DistinctUntilChanged()
            .Subscribe(x => ResolveViewForViewModel(x.ViewModel, x.Contract))
            .DisposeWith(_subscriptions);
    }

    /// <summary>
    /// Gets or sets the view contract observable.
    /// </summary>
    public IObservable<string?> ViewContractObservable
    {
        get => (IObservable<string>)GetValue(ViewContractObservableProperty);
        set => SetValue(ViewContractObservableProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed by default when no content is set.
    /// </summary>
    public object DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the ViewModel to display.
    /// </summary>
    public TViewModel? ViewModel
    {
        get => (TViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the ViewModel to display (non-generic interface implementation).
    /// </summary>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as TViewModel;
    }

    /// <summary>
    /// Gets or sets the view contract.
    /// </summary>
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
    /// Gets or sets the view locator.
    /// </summary>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>
    /// resolve view for view model with respect to contract.
    /// </summary>
    /// <param name="viewModel">ViewModel.</param>
    /// <param name="contract">contract used by ViewLocator.</param>
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
            Content = DefaultContent;
            this.Log().Warn($"The {nameof(ViewModelViewHost)} could not find a valid view for the view model of type {typeof(TViewModel)} and value {viewModel}.");
            return;
        }

        viewInstance.ViewModel = viewModel;

        Content = viewInstance;
    }
}
