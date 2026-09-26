// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ReactiveUI.Internal;

#if REACTIVE_SHIM
using static ReactiveUI.Binding.Reactive.ViewLocator;
#else
using static ReactiveUI.Binding.ViewLocator;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Winforms;
#else
namespace ReactiveUI.Winforms;
#endif

/// <summary>A view model control host which will find and host the View for a ViewModel.</summary>
/// <remarks>
/// The host asks the view locator for the view by the view model's run-time type, without building any type at run
/// time. The default locator checks the view lookup the source generator writes, then the views the app added with
/// <c>Map</c>. A view registered only with the service locator needs <see cref="ViewModelControlHostUnsafe"/>, which
/// also asks the service locator for <see cref="IViewFor{T}"/> closed over the view model's run-time type.
/// </remarks>
[DefaultProperty("ViewModel")]
[DebuggerDisplay("{CurrentView}, {DefaultContent}")]
public partial class ViewModelControlHost : UserControl, IReactiveObject, IViewFor
{
    /// <summary>Holds the subscriptions created during setup so they can be disposed together.</summary>
    private readonly MultipleDisposable _disposables = [];

    /// <summary>Asks a view locator for the view of a view model under a contract.</summary>
    private readonly Func<IViewLocator, object, string?, IViewFor?> _resolveView;

    /// <summary>Backing field for the currently displayed content.</summary>
    private object? _content;

    /// <summary>Backing field indicating whether resolved views are cached and reused.</summary>
    private bool _cacheViews;

    /// <summary>The <see cref="INotifyPropertyChanging.PropertyChanging"/> handlers; subscribing enables the classic event.</summary>
    private PropertyChangingEventHandler? _propertyChanging;

    /// <summary>The <see cref="INotifyPropertyChanged.PropertyChanged"/> handlers; subscribing enables the classic event.</summary>
    private PropertyChangedEventHandler? _propertyChanged;

    /// <summary>Initializes a new instance of the <see cref="ViewModelControlHost"/> class.</summary>
    public ViewModelControlHost()
        : this(ResolveViewWithoutReflection)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ViewModelControlHost"/> class with its view lookup.</summary>
    /// <param name="resolveView">Asks a view locator for the view of a view model under a contract.</param>
    [SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "'this' is passed to BindingSink; the WinForms designer-mandated constructor wires this control's bindings; the single-threaded control is never published elsewhere.")]
    private protected ViewModelControlHost(Func<IViewLocator, object, string?, IViewFor?> resolveView)
    {
        _resolveView = resolveView;
        InitializeComponent();
        _cacheViews = DefaultCacheViewsEnabled;
        _disposables.Add(new BindingSink(this));
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging
    {
        add
        {
            this.SubscribePropertyChangingEvents();
            _propertyChanging += value;
        }

        remove => _propertyChanging -= value;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            this.SubscribePropertyChangedEvents();
            _propertyChanged += value;
        }

        remove => _propertyChanged -= value;
    }

    /// <summary>Gets or sets a value indicating whether [default cache views enabled].</summary>
    public static bool DefaultCacheViewsEnabled { get; set; }

    /// <summary>Gets the current view.</summary>
    public Control? CurrentView => _content as Control;

    /// <summary>Gets or sets the default content.</summary>
    [Category("ReactiveUI")]
    [Description("The default control when no viewmodel is specified")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Control? DefaultContent
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the view contract observable.</summary>
    /// <value>
    /// The view contract observable.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IObservable<string>? ViewContractObservable
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the view locator.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IViewLocator? ViewLocator { get; set; }

    /// <inheritdoc/>
    [Category("ReactiveUI")]
    [Description("The viewmodel to host.")]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the content.</summary>
    [Category("ReactiveUI")]
    [Description("The Current View")]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? Content
    {
        get => _content;
        protected set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    /// <summary>Gets or sets a value indicating whether to cache views.</summary>
    [Category("ReactiveUI")]
    [Description("Cache Views")]
    [Bindable(true)]
    [DefaultValue(true)]
    public bool CacheViews
    {
        get => _cacheViews;
        set => this.RaiseAndSetIfChanged(ref _cacheViews, value);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => _propertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => _propertyChanged?.Invoke(this, args);

    /// <summary>Clean up any resources being used.</summary>
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

    /// <summary>Finds a view by the view model's run-time type without building any type at run time.</summary>
    /// <param name="viewLocator">The view locator to ask.</param>
    /// <param name="viewModel">The view model to find a view for.</param>
    /// <param name="contract">The contract to resolve under, or <see langword="null"/> for the default view.</param>
    /// <returns>The view, or <see langword="null"/> when neither the generated lookup nor a <c>Map</c> registration has one.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IViewFor? ResolveViewWithoutReflection(IViewLocator viewLocator, object viewModel, string? contract) =>
        viewLocator.ResolveView(viewModel, contract);

    /// <summary>Replaces the currently visible control with the supplied view, docked to fill the host.</summary>
    /// <param name="view">The control to display.</param>
    private void SwapHostedView(Control view)
    {
        // change the view in the ui
        SuspendLayout();

        // clear out existing visible control view
        foreach (Control? c in Controls)
        {
            c?.Dispose();
            Controls.Remove(c);
        }

        view.Dock = DockStyle.Fill;
        Controls.Add(view);
        ResumeLayout();
    }

    /// <summary>Resolves and displays the content for the supplied view model and contract.</summary>
    /// <param name="viewModel">The current view model, or null.</param>
    /// <param name="contract">The view contract.</param>
    private void UpdateContentForViewModel(object? viewModel, string contract)
    {
        // set content to default when viewmodel is null
        if (viewModel is null)
        {
            if (DefaultContent is not null)
            {
                Content = DefaultContent;
            }

            return;
        }

        if (TryReuseCachedView(viewModel))
        {
            return;
        }

        var view = _resolveView(ViewLocator ?? GetCurrent(), viewModel, contract);
        if (view is null)
        {
            return;
        }

        view.ViewModel = viewModel;
        Content = view;
    }

    /// <summary>Reuses the cached view for the supplied view model when caching is enabled and the types match.</summary>
    /// <param name="viewModel">The current view model.</param>
    /// <returns>true if the cached view was reused; otherwise, false.</returns>
    private bool TryReuseCachedView(object viewModel)
    {
        if (!CacheViews)
        {
            return false;
        }

        // when caching views, check the current viewmodel and type
        var c = _content as IViewFor;

        if (c?.ViewModel is null || c.ViewModel.GetType() != viewModel.GetType())
        {
            return false;
        }

        // setting the viewmodel allows the view to update its bindings
        c.ViewModel = viewModel;
        return true;
    }

    /// <summary>
    /// A single fused sink that owns every subscription the host needs and drives view presentation directly. It
    /// folds the four sources — <see cref="Content"/>, <see cref="DefaultContent"/>, <see cref="ViewModel"/> and the
    /// <see cref="ViewContractObservable"/> — into one object, performing the view-model/contract <c>CombineLatest</c>
    /// inline instead of allocating a separate combine operator and per-source observers.
    /// </summary>
    private sealed class BindingSink : IDisposable
    {
        /// <summary>The host whose presentation this sink drives.</summary>
        private readonly ViewModelControlHost _host;

        /// <summary>The <see cref="Content"/> subscription.</summary>
        private readonly IDisposable _contentSubscription;

        /// <summary>The <see cref="DefaultContent"/> subscription.</summary>
        private readonly IDisposable _defaultContentSubscription;

        /// <summary>The view-model/contract combine that drives content resolution.</summary>
        private readonly CombineLatestSink<object?, string> _viewModelContract;

        /// <summary>Initializes a new instance of the <see cref="BindingSink"/> class and wires every source.</summary>
        /// <param name="host">The host to drive.</param>
        public BindingSink(ViewModelControlHost host)
        {
            _host = host;

            // Content -> swap the hosted control (the type check replaces WhereNotNull().OfType<Control>()).
            _contentSubscription = host.WhenAnyValue(static x => x.Content)
                .Subscribe(new DelegateObserver<object?>(OnContentChanged));

            // DefaultContent -> show it as the current content once it is set.
            _defaultContentSubscription = host.WhenAnyValue(static x => x.DefaultContent)
                .Subscribe(new DelegateObserver<Control?>(OnDefaultContentChanged));

            host.ViewContractObservable = Signal.Emit(string.Empty);

            // ViewModel + ViewContractObservable -> resolve and show the matching view.
            _viewModelContract = new(
                host.WhenAnyValue(static x => x.ViewModel),
                host.WhenAnyObservable(x => x.ViewContractObservable!),
                host.UpdateContentForViewModel,
                RxState.DefaultExceptionHandler.OnNext);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _contentSubscription.Dispose();
            _defaultContentSubscription.Dispose();
            _viewModelContract.Dispose();
        }

        /// <summary>Swaps the hosted view when the content is a control.</summary>
        /// <param name="content">The new content.</param>
        private void OnContentChanged(object? content)
        {
            if (content is not Control control)
            {
                return;
            }

            _host.SwapHostedView(control);
        }

        /// <summary>Shows the default content once it is assigned.</summary>
        /// <param name="defaultContent">The new default content.</param>
        private void OnDefaultContentChanged(Control? defaultContent)
        {
            if (defaultContent is null)
            {
                return;
            }

            _host.Content = _host.DefaultContent;
        }
    }
}
