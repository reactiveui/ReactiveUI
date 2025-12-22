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
#if NET6_0_OR_GREATER
[RequiresDynamicCode("ViewModelViewHost uses ReactiveUI extension methods and RxApp properties which require dynamic code generation")]
[RequiresUnreferencedCode("ViewModelViewHost uses ReactiveUI extension methods and RxApp properties which may require unreferenced code")]
#endif
public class ViewModelViewHost : ReactiveViewController
{
    private readonly SerialDisposable _currentView;
    private readonly ObservableAsPropertyHelper<string?> _viewContract;
    private IViewLocator? _viewLocator;
    private NSViewController? _defaultContent;
    private object? _viewModel;
    private IObservable<string?>? _viewContractObservable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelViewHost"/> class.
    /// </summary>
    public ViewModelViewHost()
    {
        _currentView = new SerialDisposable();
        _viewContract = this
            .WhenAnyObservable(static x => x.ViewContractObservable)
            .ToProperty(this, static x => x.ViewContract, initialValue: null, scheduler: RxSchedulers.MainThreadScheduler);

        Initialize();
    }

    /// <summary>
    /// Gets or sets the <see cref="IViewLocator"/> used to resolve views for the current <see cref="ViewModel"/>. Defaults
    /// to <see cref="ReactiveUI.ViewLocator.Current"/> if not provided.
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
        get => _viewContract.Value;
        set => ViewContractObservable = Observable.Return(value);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _currentView.Dispose();
            _viewContract.Dispose();
        }
    }

    private static void Adopt(NSViewController parent, NSViewController? child)
    {
        ArgumentNullException.ThrowIfNull(parent);

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

    private void Initialize()
    {
        var viewChange = this.WhenAnyValue<ViewModelViewHost, object?>(nameof(ViewModel))
            .CombineLatest(
                this.WhenAnyObservable(x => x.ViewContractObservable).StartWith((string?)null),
                (vm, contract) => new { ViewModel = vm, Contract = contract })
            .Where(x => x.ViewModel is not null);

        var defaultViewChange = this.WhenAnyValue<ViewModelViewHost, object?>(nameof(ViewModel))
            .CombineLatest(
                this.WhenAnyValue<ViewModelViewHost, NSViewController?>(nameof(DefaultContent)),
                (vm, defaultContent) => new { ViewModel = vm, DefaultContent = defaultContent })
            .Where(x => x.ViewModel is null && x.DefaultContent is not null)
            .Select(x => x.DefaultContent);

        viewChange
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(
                x =>
                {
                    var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                    var view = viewLocator.ResolveView<object?>(x.ViewModel, x.Contract);

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

                    _currentView.Disposable = (CompositeDisposable?)[viewController, Disposable.Create(() => Disown(viewController))];
                });

        defaultViewChange
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(x => Adopt(this, x));
    }
}
