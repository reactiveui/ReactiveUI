// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Winforms;

/// <summary>
/// A control host which will handling routing between different ViewModels and Views.
/// </summary>
[DefaultProperty("ViewModel")]
public partial class RoutedControlHost : UserControl, IReactiveObject
{
    private readonly CompositeDisposable _disposables = [];
#pragma warning disable IDE0032 // Use auto property
    private RoutingState? _router;
    private Control? _defaultContent;
    private IObservable<string>? _viewContractObservable;
#pragma warning restore IDE0032 // Use auto property

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedControlHost"/> class.
    /// </summary>
    public RoutedControlHost()
    {
        InitializeComponent();

        _disposables.Add(this.WhenAny(x => x.DefaultContent, x => x.Value).Subscribe(x =>
        {
            if (x is not null && Controls.Count == 0)
            {
                Controls.Add(InitView(x));
                components?.Add(DefaultContent);
            }
        }));

        ViewContractObservable = Observable<string>.Default;

        var vmAndContract =
            this.WhenAnyObservable(x => x.Router!.CurrentViewModel!)
                .CombineLatest(
                               this.WhenAnyObservable(x => x.ViewContractObservable!),
                               (vm, contract) => new { ViewModel = vm, Contract = contract });

        Control? viewLastAdded = null;
        _disposables.Add(vmAndContract.Subscribe(
                                                 x =>
                                                 {
                                                     // clear all hosted controls (view or default content)
                                                     SuspendLayout();
                                                     Controls.Clear();

                                                     viewLastAdded?.Dispose();

                                                     if (x.ViewModel is null)
                                                     {
                                                         if (DefaultContent is not null)
                                                         {
                                                             InitView(DefaultContent);
                                                             Controls.Add(DefaultContent);
                                                         }

                                                         ResumeLayout();
                                                         return;
                                                     }

                                                     var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                                                     var view = viewLocator.ResolveView(x.ViewModel, x.Contract);
                                                     if (view is not null)
                                                     {
                                                         view.ViewModel = x.ViewModel;

                                                         viewLastAdded = InitView((Control)view);
                                                     }

                                                     if (viewLastAdded is not null)
                                                     {
                                                         Controls.Add(viewLastAdded);
                                                     }

                                                     ResumeLayout();
                                                 },
                                                 RxApp.DefaultExceptionHandler!.OnNext));
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
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public RoutingState? Router
    {
        get => _router;
        set => this.RaiseAndSetIfChanged(ref _router, value);
    }

    /// <summary>
    /// Gets or sets the view contract observable.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public IObservable<string>? ViewContractObservable
    {
        get => _viewContractObservable;
        set => this.RaiseAndSetIfChanged(ref _viewContractObservable, value);
    }

    /// <summary>
    /// Gets or sets the view locator.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

    private static Control InitView(Control view)
    {
        view.Dock = DockStyle.Fill;
        return view;
    }
}
