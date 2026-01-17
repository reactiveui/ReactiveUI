// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if UIKIT
using UIKit;

using NSViewController = UIKit.UIViewController;
#else
using AppKit;
#endif

namespace ReactiveUI;

/// <summary>
/// A controller that resolves an <see cref="IViewFor"/> implementation for the supplied <see cref="ViewModel"/> and
/// hosts it as a child view controller.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ViewModelViewHost"/> is useful when a view is responsible for projecting an arbitrary view model instance
/// determined at runtime. The host listens for <see cref="ViewModel"/> or contract changes, resolves a view via
/// <see cref="ViewLocator"/>, and swaps the child controller hierarchy accordingly.
/// </para>
/// <para>
/// Provide a <see cref="DefaultContent"/> controller to display placeholder UI while no view model is available, or set
/// <see cref="ViewContractObservable"/> to drive platform-specific view selection.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// var host = new ViewModelViewHost
/// {
///     ViewModel = screen.Router.CurrentViewModel.FirstAsync().Wait(),
///     ViewLocator = locator,
///     DefaultContent = new LoadingViewController()
/// };
///
/// host.ViewContractObservable = this.WhenAnyValue(x => x.SelectedTheme);
/// ]]>
/// </code>
/// </example>
[RequiresUnreferencedCode("This class uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
[RequiresDynamicCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
public class ViewModelViewHost : ReactiveViewController
{
    /// <summary>
    /// Tracks the currently-adopted view controller and ensures it is disowned on replacement or disposal.
    /// </summary>
    private readonly SerialDisposable _currentView;

    /// <summary>
    /// Holds subscriptions created during initialization.
    /// </summary>
    private readonly CompositeDisposable _subscriptions;

    /// <summary>
    /// Holds the subscription to <see cref="ViewContractObservable"/> (the inner observable) and swaps it when the
    /// property changes.
    /// </summary>
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed by _subscriptions")]
    private readonly SerialDisposable _viewContractObservableSubscription;

    /// <summary>
    /// Backing field for <see cref="ViewContract"/>. This is updated by observing <see cref="ViewContractObservable"/>
    /// and is raised as a property change for bindings.
    /// </summary>
    private string? _viewContract;

    /// <summary>
    /// Backing field for <see cref="ViewLocator"/>.
    /// </summary>
    private IViewLocator? _viewLocator;

    /// <summary>
    /// Backing field for <see cref="DefaultContent"/>.
    /// </summary>
    private NSViewController? _defaultContent;

    /// <summary>
    /// Backing field for <see cref="ViewModel"/>.
    /// </summary>
    private object? _viewModel;

    /// <summary>
    /// Backing field for <see cref="ViewContractObservable"/>.
    /// </summary>
    private IObservable<string?>? _viewContractObservable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelViewHost"/> class.
    /// </summary>
    public ViewModelViewHost()
    {
        _currentView = new SerialDisposable();
        _subscriptions = new CompositeDisposable();
        _viewContractObservableSubscription = new SerialDisposable();

        // Drive ViewContract from ViewContractObservable without WhenAny*/expression trees (AOT-trimmer friendly).
        // We always publish an initial null contract to preserve the original StartWith(null) behavior.
        var contractStream = CreateViewContractStream()
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(SetViewContract);

        _subscriptions.Add(contractStream);
        _subscriptions.Add(_viewContractObservableSubscription);

        Initialize();
    }

    /// <summary>
    /// Gets or sets the <see cref="IViewLocator"/> used to resolve views for the current <see cref="ViewModel"/>. Defaults
    /// to <see cref="ViewLocator.Current"/> if not provided.
    /// </summary>
    public IViewLocator? ViewLocator
    {
        get => _viewLocator;
        set => this.RaiseAndSetIfChanged(ref _viewLocator, value);
    }

    /// <summary>
    /// Gets or sets the controller displayed when <see cref="ViewModel"/> is <see langword="null"/>.
    /// </summary>
    public NSViewController? DefaultContent
    {
        get => _defaultContent;
        set => this.RaiseAndSetIfChanged(ref _defaultContent, value);
    }

    /// <summary>
    /// Gets or sets the view model whose view should be hosted.
    /// </summary>
    public object? ViewModel
    {
        get => _viewModel;
        set => this.RaiseAndSetIfChanged(ref _viewModel, value);
    }

    /// <summary>
    /// Gets or sets an observable producing view contracts. Contracts allow multiple views to be registered for the same
    /// view model but different display contexts.
    /// </summary>
    public IObservable<string?>? ViewContractObservable
    {
        get => _viewContractObservable;
        set => this.RaiseAndSetIfChanged(ref _viewContractObservable, value);
    }

    /// <summary>
    /// Gets or sets the view contract used when resolving views. Assigning a contract produces a singleton observable
    /// under the covers.
    /// </summary>
    public string? ViewContract
    {
        get => _viewContract;
        set => ViewContractObservable = Observable.Return(value);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _subscriptions.Dispose();
            _currentView.Dispose();
        }
    }

    /// <summary>
    /// Adds <paramref name="child"/> as a child controller of <paramref name="parent"/> and ensures its view fills
    /// the parent bounds.
    /// </summary>
    /// <param name="parent">The parent controller.</param>
    /// <param name="child">The child controller to adopt.</param>
    /// <exception cref="ArgumentException">Thrown when the parent's view is <see langword="null"/>.</exception>
    private static void Adopt(NSViewController parent, NSViewController? child)
    {
        ArgumentExceptionHelper.ThrowIfNull(parent);

        if (parent.View is null)
        {
            throw new ArgumentException("The View on the parent is null.", nameof(parent));
        }

        if (child?.View is null)
        {
            return;
        }

        // ensure the child view fills our entire frame
        child.View.Frame = parent.View.Bounds;
#if UIKIT
        child.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
#else
        child.View.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
#endif
        child.View.TranslatesAutoresizingMaskIntoConstraints = true;

        parent.AddChildViewController(child);

#if UIKIT
        var parentAlreadyVisible = parent.IsViewLoaded && parent.View.Window is not null;

        if (parentAlreadyVisible)
        {
            child.BeginAppearanceTransition(true, false);
        }
#endif

        parent.View.AddSubview(child.View);

#if UIKIT
        if (parentAlreadyVisible)
        {
            child.EndAppearanceTransition();
        }

        child.DidMoveToParentViewController(parent);
#endif
    }

    /// <summary>
    /// Removes <paramref name="child"/> from its parent controller and removes its view from the view hierarchy.
    /// </summary>
    /// <param name="child">The child controller to disown.</param>
    /// <exception cref="ArgumentException">Thrown when the child's view is <see langword="null"/>.</exception>
    private static void Disown(NSViewController child)
    {
        if (child.View is null)
        {
            throw new ArgumentException("The View on the child is null.", nameof(child));
        }

#if UIKIT
        child.WillMoveToParentViewController(null);
#endif
        child.View.RemoveFromSuperview();
        child.RemoveFromParentViewController();
    }

    /// <summary>
    /// Initializes reactive subscriptions that drive view resolution and controller swapping.
    /// </summary>
    [RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    private void Initialize()
    {
        var viewModelChanges = ObserveProperty(static x => x.ViewModel, nameof(ViewModel));
        var defaultContentChanges = ObserveProperty(static x => x.DefaultContent, nameof(DefaultContent));
        var contractChanges = ObserveProperty(static x => x.ViewContract, nameof(ViewContract));

        var viewChange =
            viewModelChanges
                .CombineLatest(
                    contractChanges,
                    static (vm, contract) => new { ViewModel = vm, Contract = contract })
                .Where(static x => x.ViewModel is not null);

        var defaultViewChange =
            viewModelChanges
                .CombineLatest(
                    defaultContentChanges,
                    static (vm, defaultContent) => new { ViewModel = vm, DefaultContent = defaultContent })
                .Where(static x => x.ViewModel is null && x.DefaultContent is not null)
                .Select(static x => x.DefaultContent);

        _subscriptions.Add(
            viewChange
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(
                    x =>
                    {
                        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                        var view = viewLocator.ResolveView(x.ViewModel, x.Contract);

                        if (view is null)
                        {
                            var message = $"Unable to resolve view for \"{x.ViewModel?.GetType()}\"";

                            if (x.Contract is not null)
                            {
                                message += $" and contract \"{x.Contract.GetType()}\"";
                            }

                            message += ".";
                            throw new Exception(message);
                        }

                        if (view is not NSViewController viewController)
                        {
                            //// TODO: As viewController may be NULL at this point this execution will never show the FullName, find fixed text to replace this with.
                            throw new Exception($"Resolved view type '{view?.GetType().FullName}' is not a '{typeof(NSViewController).FullName}'.");
                        }

                        view.ViewModel = x.ViewModel;
                        Adopt(this, viewController);

                        _currentView.Disposable =
                            new CompositeDisposable(
                                viewController,
                                Disposable.Create(() => Disown(viewController)));
                    }));

        _subscriptions.Add(
            defaultViewChange
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(x => Adopt(this, x)));
    }

    /// <summary>
    /// Creates a contract stream that (1) emits an initial <see langword="null"/> value, (2) subscribes to the current
    /// <see cref="ViewContractObservable"/>, and (3) swaps the inner subscription when the property changes.
    /// </summary>
    /// <returns>An observable of view contracts.</returns>
    private IObservable<string?> CreateViewContractStream()
    {
        return Observable.Create<string?>(
            observer =>
            {
                // Preserve the previous StartWith((string?)null) semantics.
                observer.OnNext(null);

                void SwapInner(IObservable<string?>? source)
                {
                    _viewContractObservableSubscription.Disposable =
                        source is null
                            ? Disposable.Empty
                            : source.Subscribe(observer);
                }

                // Subscribe to the initial observable (if any).
                SwapInner(ViewContractObservable);

                // Listen for property changes and rewire the inner subscription.
                var outerSubscription =
                    Changed
                        .Where(static e => e.PropertyName == nameof(ViewContractObservable))
                        .Subscribe(_ => SwapInner(ViewContractObservable));

                return new CompositeDisposable(outerSubscription);
            });
    }

    /// <summary>
    /// Observes changes to a property without using WhenAny* APIs (avoids RUC/RDC from expression-based pipelines).
    /// The observable emits the current value immediately and then emits on each subsequent property change.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="getter">A getter for the property value.</param>
    /// <param name="propertyName">The name of the property to observe.</param>
    /// <returns>An observable that emits the property value.</returns>
    private IObservable<T> ObserveProperty<T>(Func<ViewModelViewHost, T> getter, string propertyName)
    {
        return Observable.Create<T>(
            observer =>
            {
                observer.OnNext(getter(this));

                return Changed
                    .Where(e => e.PropertyName == propertyName)
                    .Select(_ => getter(this))
                    .Subscribe(observer);
            });
    }

    /// <summary>
    /// Updates the <see cref="ViewContract"/> backing field and raises property changed notifications.
    /// </summary>
    /// <param name="contract">The new contract value.</param>
    private void SetViewContract(string? contract)
    {
        this.RaiseAndSetIfChanged(ref _viewContract, contract, nameof(ViewContract));
    }
}
