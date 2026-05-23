// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#elif HAS_UNO
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#else

using System.Windows;
#endif
using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Internal;
using Splat;

#if HAS_UNO
namespace ReactiveUI.Uno
#else

namespace ReactiveUI;
#endif

/// <summary>
/// This content control will automatically load the View associated with
/// the ViewModel property and display it. This control is very useful
/// inside a DataTemplate to display the View associated with a ViewModel.
/// </summary>
public
#if HAS_UNO
    partial
#endif
    class ViewModelViewHost : TransitioningContentControl, IViewFor, IEnableLogger
{
    /// <summary>
    /// The default content dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost), new(null));

    /// <summary>
    /// The view model dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(ViewModelViewHost), new(null));

    /// <summary>
    /// The view contract observable dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(
            nameof(ViewContractObservable),
            typeof(IObservable<string>),
            typeof(ViewModelViewHost),
            new(new ReturnObservable<string>(default!)));

    /// <summary>
    ///  The ContractFallbackByPass dependency property.
    /// </summary>
    public static readonly DependencyProperty ContractFallbackByPassProperty =
        DependencyProperty.Register("ContractFallbackByPass", typeof(bool), typeof(ViewModelViewHost), new(false));

    /// <summary>
    /// Stores the most recently observed view contract.
    /// </summary>
    private string? _viewContract;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelViewHost"/> class.
    /// </summary>
    public ViewModelViewHost()
    {
        var platform = AppLocator.Current.GetService<IPlatformOperations>();
        Func<string?> platformGetter = () => null;

        if (platform is null)
        {
            // NB: This used to be an error but WPF design mode can't read
            // good or do other stuff good.
            this.Log().Error(
                "Couldn't find an IPlatformOperations implementation. Please make sure you have installed " +
                "the latest version of the ReactiveUI packages for your platform. " +
                "See https://reactiveui.net/docs/getting-started/installation for guidance.");
        }
        else
        {
            platformGetter = platform.GetOrientation;
        }

        ViewContractObservable = ModeDetector.InUnitTestRunner()
            ? NeverObservable<string?>.Instance
            : new DistinctUntilChangedObservable<string?>(
                new StartWithObservable<string?>(
                    new FromEventObservable<string?>(onNext =>
                    {
                        void Handler(object? sender, SizeChangedEventArgs e) => onNext(platformGetter());
                        SizeChanged += Handler;
                        return new ActionDisposable(() => SizeChanged -= Handler);
                    }),
                    platformGetter()));

        var contractChanged = new StartWithObservable<string?>(
            new DoObservable<string?>(this.WhenAnyObservable(x => x.ViewContractObservable), x => _viewContract = x),
            ViewContract);
        var viewModelChanged = new StartWithObservable<object?>(
            this.WhenAnyValue<ViewModelViewHost, object?>(nameof(ViewModel)),
            ViewModel);
        var vmAndContract = new CombineLatestObservable<string?, object?, (object? ViewModel, string? Contract)>(
            contractChanged,
            viewModelChanged,
            (contract, vm) => (ViewModel: vm, Contract: contract));

        this.WhenActivated(d =>
        {
            d(new ObserveOnObservable<string?>(contractChanged, RxSchedulers.MainThreadScheduler)
                .Subscribe(new DelegateObserver<string?>(x => _viewContract = x ?? string.Empty)));

            d(new DistinctUntilChangedObservable<(object? ViewModel, string? Contract)>(vmAndContract)
                .Subscribe(new DelegateObserver<(object? ViewModel, string? Contract)>(x => ResolveViewForViewModel(x.ViewModel, x.Contract))));
        });
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
    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the view contract.
    /// </summary>
    [SuppressMessage(
        "Critical Bug",
        "S4275:Getters and setters should access the expected fields",
        Justification = "Setter intentionally routes through ViewContractObservable rather than the field.")]
    public string? ViewContract
    {
        get => _viewContract;
        set => ViewContractObservable = new ReturnObservable<string?>(value);
    }

    /// <summary>
    /// Gets or sets the view locator.
    /// </summary>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether should bypass the default contract fallback behavior.
    /// </summary>
    public bool ContractFallbackByPass
    {
        get => (bool)GetValue(ContractFallbackByPassProperty);
        set => SetValue(ContractFallbackByPassProperty, value);
    }

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
            Content = DefaultContent;
            this.Log().Warn(
                $"The {nameof(ViewModelViewHost)} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}.");
            return;
        }

        viewInstance.ViewModel = viewModel;

        Content = viewInstance;
    }
}
