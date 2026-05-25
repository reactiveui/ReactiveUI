// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Reactive.Disposables;
using ReactiveUI.Internal;

namespace ReactiveUI.Winforms;

/// <summary>
/// A control host which will handling routing between different ViewModels and Views.
/// </summary>
[DefaultProperty("ViewModel")]
public partial class RoutedControlHost : UserControl, IReactiveObject
{
    /// <summary>Holds the subscriptions created during construction so they can be disposed together.</summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>Backing field for the routing state.</summary>
    private RoutingState? _router;

    /// <summary>Backing field for the default content shown when no view model is set.</summary>
    private Control? _defaultContent;

    /// <summary>Backing field for the view contract observable.</summary>
    private IObservable<string>? _viewContractObservable;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedControlHost"/> class.
    /// </summary>
    public RoutedControlHost()
    {
        InitializeComponent();
        _disposables.Add(new BindingSink(this));
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the default content.
    /// </summary>
    /// <value>
    /// The default content.
    /// </value>
    [Category("ReactiveUI")]
    [Description("The default control when no viewmodel is specified")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Control? DefaultContent
    {
        get => _defaultContent;
        set => this.RaiseAndSetIfChanged(ref _defaultContent, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="RoutingState"/> of the view model stack.
    /// </summary>
    [Category("ReactiveUI")]
    [Description("The router.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RoutingState? Router
    {
        get => _router;
        set => this.RaiseAndSetIfChanged(ref _router, value);
    }

    /// <summary>
    /// Gets or sets the view contract observable.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IObservable<string>? ViewContractObservable
    {
        get => _viewContractObservable;
        set => this.RaiseAndSetIfChanged(ref _viewContractObservable, value);
    }

    /// <summary>
    /// Gets or sets the view locator.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IViewLocator? ViewLocator { get; set; }

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
            _disposables.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Prepares a control for hosting by docking it to fill the host.
    /// </summary>
    /// <param name="view">The control to initialize.</param>
    /// <returns>The same control, docked to fill.</returns>
    private static Control InitView(Control view)
    {
        view.Dock = DockStyle.Fill;
        return view;
    }

    /// <summary>
    /// A single fused sink that owns every subscription the host needs and drives routing directly. It folds the
    /// <see cref="DefaultContent"/> source and the <see cref="RoutingState.CurrentViewModel"/>/
    /// <see cref="ViewContractObservable"/> combine into one object, resolving and hosting the matching view inline
    /// instead of allocating a separate combine operator and terminal observer.
    /// </summary>
    private sealed class BindingSink : IDisposable
    {
        /// <summary>The host whose routing this sink drives.</summary>
        private readonly RoutedControlHost _host;

        /// <summary>The <see cref="DefaultContent"/> subscription.</summary>
        private readonly IDisposable _defaultContentSubscription;

        /// <summary>The current view-model/contract combine that drives view resolution.</summary>
        private readonly CombineLatestSink<IRoutableViewModel?, string> _viewModelContract;

        /// <summary>Initializes a new instance of the <see cref="BindingSink"/> class and wires every source.</summary>
        /// <param name="host">The host to drive.</param>
        public BindingSink(RoutedControlHost host)
        {
            _host = host;

            _defaultContentSubscription = host.WhenAnyValue<RoutedControlHost, Control?>(nameof(DefaultContent))
                .Subscribe(new DelegateObserver<Control?>(OnDefaultContentChanged));

            host.ViewContractObservable = new ReturnObservable<string>(null!);

            // Router.CurrentViewModel + ViewContractObservable -> resolve and host the matching view.
            _viewModelContract = new(
                host.WhenAnyObservable(x => x.Router!.CurrentViewModel),
                host.WhenAnyObservable(x => x.ViewContractObservable!),
                OnViewModelContract,
                RxState.DefaultExceptionHandler.OnNext);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _defaultContentSubscription.Dispose();
            _viewModelContract.Dispose();
        }

        /// <summary>Hosts the default content the first time it is assigned while nothing else is shown.</summary>
        /// <param name="defaultContent">The new default content.</param>
        private void OnDefaultContentChanged(Control? defaultContent)
        {
            if (defaultContent is null || _host.Controls.Count != 0)
            {
                return;
            }

            _host.Controls.Add(InitView(defaultContent));
            _host.components?.Add(_host.DefaultContent);
        }

        /// <summary>Resolves and hosts the view for the current view model/contract pair.</summary>
        /// <param name="viewModel">The current view model, or null.</param>
        /// <param name="contract">The view contract.</param>
        private void OnViewModelContract(IRoutableViewModel? viewModel, string contract)
        {
            _host.SuspendLayout();

            // Remove every hosted control, disposing the resolved views but preserving DefaultContent for reuse.
            ClearHostedViews();

            if (viewModel is null)
            {
                if (_host.DefaultContent is not null)
                {
                    InitView(_host.DefaultContent);
                    _host.Controls.Add(_host.DefaultContent);
                }

                _host.ResumeLayout();
                return;
            }

            var viewLocator = _host.ViewLocator ?? ReactiveUI.ViewLocator.Current;
            var view = viewLocator.ResolveView(viewModel, contract);
            if (view is not null)
            {
                view.ViewModel = viewModel;

                _host.Controls.Add(InitView((Control)view));
            }

            _host.ResumeLayout();
        }

        /// <summary>
        /// Removes every hosted control, disposing resolved views as they are removed while leaving the reusable
        /// <see cref="DefaultContent"/> intact.
        /// </summary>
        private void ClearHostedViews()
        {
            // Iterate in reverse: disposing a control detaches it from the Controls collection.
            for (var i = _host.Controls.Count - 1; i >= 0; i--)
            {
                var control = _host.Controls[i];
                if (ReferenceEquals(control, _host.DefaultContent))
                {
                    _host.Controls.RemoveAt(i);
                }
                else
                {
                    control.Dispose();
                }
            }
        }
    }
}
