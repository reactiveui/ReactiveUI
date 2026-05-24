// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

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
[RequiresUnreferencedCode(
    "This class uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
[RequiresDynamicCode(
    "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
public class ViewModelViewHost : ReactiveViewController
{
    /// <summary>
    /// Tracks the currently-adopted view controller and ensures it is disowned on replacement or disposal.
    /// </summary>
    private readonly SwapDisposable _currentView;

    /// <summary>
    /// Holds subscriptions created during initialization.
    /// </summary>
    private readonly DisposableBag _subscriptions;

    /// <summary>
    /// Holds the subscription to <see cref="ViewContractObservable"/> (the inner observable) and swaps it when the
    /// property changes.
    /// </summary>
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed by _subscriptions")]
    private readonly SwapDisposable _viewContractObservableSubscription;

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
        _currentView = new SwapDisposable();
        _subscriptions = new DisposableBag();
        _viewContractObservableSubscription = new SwapDisposable();

        // Drive ViewContract from ViewContractObservable without WhenAny*/expression trees (AOT-trimmer friendly).
        // We always publish an initial null contract to preserve the original StartWith(null) behavior.
        var contractStream = new ObserveOnObservableLocal<string?>(
            new ViewContractStreamObservable(this),
            RxSchedulers.MainThreadScheduler)
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
        set => this.RaiseAndSetIfChanged(ref _viewContract, value);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        _subscriptions.Dispose();
        _currentView.Dispose();
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
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    private void Initialize()
    {
        var viewModelChanges = new PropertyObservable<object?>(this, static x => x._viewModel, nameof(ViewModel));
        var defaultContentChanges = new PropertyObservable<NSViewController?>(this, static x => x._defaultContent, nameof(DefaultContent));
        var contractChanges = new PropertyObservable<string?>(this, static x => x._viewContract, nameof(ViewContract));

        // CombineLatest(viewModel, contract) filtered to non-null view models → fused ViewModelContractObservable.
        var viewChange = new ViewModelContractObservable(viewModelChanges, contractChanges);

        // CombineLatest(viewModel, defaultContent), filtered to null viewModel with non-null defaultContent,
        // then projected to the defaultContent value → fused DefaultViewObservable.
        var defaultViewChange = new DefaultViewObservable(viewModelChanges, defaultContentChanges);

        _subscriptions.Add(
            new ObserveOnObservableLocal<(object? ViewModel, string? Contract)>(viewChange, RxSchedulers.MainThreadScheduler)
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
                            throw new InvalidOperationException(message);
                        }

                        if (view is not NSViewController viewController)
                        {
                            // view?.GetType().FullName may be null when the runtime type name is unavailable; the message still identifies the expected type.
                            throw new InvalidOperationException($"Resolved view type '{view?.GetType().FullName}' is not a '{typeof(NSViewController).FullName}'.");
                        }

                        view.ViewModel = x.ViewModel;
                        Adopt(this, viewController);

                        _currentView.Disposable =
                            new DisposableBag(
                                viewController,
                                new ActionDisposable(() => Disown(viewController)));
                    }));

        _subscriptions.Add(
            new ObserveOnObservableLocal<NSViewController?>(defaultViewChange, RxSchedulers.MainThreadScheduler)
                .Subscribe(x => Adopt(this, x)));
    }

    /// <summary>
    /// Updates the <see cref="ViewContract"/> backing field and raises property changed notifications.
    /// </summary>
    /// <param name="contract">The new contract value.</param>
    private void SetViewContract(string? contract)
    {
        this.RaiseAndSetIfChanged(ref _viewContract, contract, nameof(ViewContract));
    }

    /// <summary>
    /// Observes a property on this <see cref="ViewModelViewHost"/> instance. Emits the current value immediately on
    /// subscription, then emits on every subsequent change. Replaces the <c>Observable.Create</c> body that was
    /// previously in <c>ObserveProperty</c>.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="host">The host instance whose property is observed.</param>
    /// <param name="getter">Returns the current property value from the host.</param>
    /// <param name="propertyName">The name of the property to observe.</param>
    private sealed class PropertyObservable<T>(
        ViewModelViewHost host,
        Func<ViewModelViewHost, T> getter,
        string propertyName) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            observer.OnNext(getter(host));

            // Changed.Where(e => e.PropertyName == propertyName).Select(_ => getter(host))
            // — two operators, fused inline here.
            return host.Changed.Subscribe(new Sink(observer, host, getter, propertyName));
        }

        /// <summary>
        /// Filters property-changed events to the target property name, then projects each event to the current
        /// property value.
        /// </summary>
        /// <param name="downstream">The downstream observer.</param>
        /// <param name="host">The host instance.</param>
        /// <param name="getter">Returns the current property value.</param>
        /// <param name="propertyName">The property name to filter on.</param>
        private sealed class Sink(
            IObserver<T> downstream,
            ViewModelViewHost host,
            Func<ViewModelViewHost, T> getter,
            string propertyName) : IObserver<IReactivePropertyChangedEventArgs<IReactiveObject>>
        {
            /// <inheritdoc/>
            public void OnNext(IReactivePropertyChangedEventArgs<IReactiveObject> value)
            {
                if (value.PropertyName != propertyName)
                {
                    return;
                }

                T result;
                try
                {
                    result = getter(host);
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                downstream.OnNext(result);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>
    /// Produces a stream that (1) emits an initial <see langword="null"/> contract, (2) subscribes to the current
    /// <see cref="ViewContractObservable"/>, and (3) swaps the inner subscription whenever
    /// <see cref="ViewContractObservable"/> changes. Replaces the <c>Observable.Create</c> body that was previously
    /// in <c>CreateViewContractStream</c>.
    /// </summary>
    /// <param name="host">The host instance.</param>
    private sealed class ViewContractStreamObservable(ViewModelViewHost host) : IObservable<string?>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<string?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            // Preserve the previous StartWith((string?)null) semantics.
            observer.OnNext(null);

            void SwapInner(IObservable<string?>? source)
            {
                host._viewContractObservableSubscription.Disposable =
                    source is null
                        ? EmptyDisposable.Instance
                        : source.Subscribe(observer);
            }

            // Subscribe to the initial observable (if any).
            SwapInner(host.ViewContractObservable);

            // Listen for property changes and rewire the inner subscription.
            // Single Where operator — no chain to fuse.
            return new WhereObservableLocal<IReactivePropertyChangedEventArgs<IReactiveObject>>(
                host.Changed,
                static e => e.PropertyName == nameof(ViewContractObservable))
                .Subscribe(new DelegateObserver<IReactivePropertyChangedEventArgs<IReactiveObject>>(
                    _ => SwapInner(host.ViewContractObservable)));
        }
    }

    /// <summary>
    /// Combines the latest view-model and contract values, forwarding only pairs where the view model is
    /// non-<see langword="null"/>. Fuses <c>CombineLatest + Where</c> into a single subscription hop.
    /// </summary>
    /// <param name="viewModelChanges">The view-model property stream.</param>
    /// <param name="contractChanges">The contract property stream.</param>
    private sealed class ViewModelContractObservable(
        IObservable<object?> viewModelChanges,
        IObservable<string?> contractChanges) : IObservable<(object? ViewModel, string? Contract)>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<(object? ViewModel, string? Contract)> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            var sink = new Sink(observer);
            sink.Run(viewModelChanges, contractChanges);
            return sink;
        }

        /// <summary>
        /// Tracks the latest view-model and contract values, emitting a combined pair only when both have
        /// produced a value and the view model is non-<see langword="null"/>.
        /// </summary>
        /// <param name="downstream">The downstream observer.</param>
        private sealed class Sink(IObserver<(object? ViewModel, string? Contract)> downstream) : IDisposable
        {
            /// <summary>Serializes value tracking across the two sources.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The latest view model value (valid once <see cref="_hasViewModel"/> is set).</summary>
            private object? _viewModel;

            /// <summary>The latest contract value (valid once <see cref="_hasContract"/> is set).</summary>
            private string? _contract;

            /// <summary>Whether the view-model source has produced a value.</summary>
            private bool _hasViewModel;

            /// <summary>Whether the contract source has produced a value.</summary>
            private bool _hasContract;

            /// <summary>The view-model source subscription.</summary>
            private IDisposable? _vmSubscription;

            /// <summary>The contract source subscription.</summary>
            private IDisposable? _contractSubscription;

            /// <summary>Subscribes to both sources.</summary>
            /// <param name="viewModelChanges">The view-model property stream.</param>
            /// <param name="contractChanges">The contract property stream.</param>
            public void Run(IObservable<object?> viewModelChanges, IObservable<string?> contractChanges)
            {
                _vmSubscription = viewModelChanges.Subscribe(new ViewModelObserver(this));
                _contractSubscription = contractChanges.Subscribe(new ContractObserver(this));
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _vmSubscription?.Dispose();
                _contractSubscription?.Dispose();
            }

            /// <summary>Records the latest view-model value and emits if conditions are satisfied.</summary>
            /// <param name="value">The new view-model value.</param>
            private void OnViewModel(object? value)
            {
                lock (_gate)
                {
                    _viewModel = value;
                    _hasViewModel = true;
                    Emit();
                }
            }

            /// <summary>Records the latest contract value and emits if conditions are satisfied.</summary>
            /// <param name="value">The new contract value.</param>
            private void OnContract(string? value)
            {
                lock (_gate)
                {
                    _contract = value;
                    _hasContract = true;
                    Emit();
                }
            }

            /// <summary>
            /// Emits the combined pair when both sources have produced a value and the view model is non-null.
            /// Caller must hold <see cref="_gate"/>.
            /// </summary>
            private void Emit()
            {
                if (!_hasViewModel || !_hasContract || _viewModel is null)
                {
                    return;
                }

                downstream.OnNext((_viewModel, _contract));
            }

            /// <summary>Forwards an error downstream.</summary>
            /// <param name="error">The error.</param>
            private void OnError(Exception error)
            {
                lock (_gate)
                {
                    downstream.OnError(error);
                }
            }

            /// <summary>Observes the view-model source.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class ViewModelObserver(Sink parent) : IObserver<object?>
            {
                /// <inheritdoc/>
                public void OnNext(object? value) => parent.OnViewModel(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnError(error);

                /// <inheritdoc/>
                public void OnCompleted()
                {
                }
            }

            /// <summary>Observes the contract source.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class ContractObserver(Sink parent) : IObserver<string?>
            {
                /// <inheritdoc/>
                public void OnNext(string? value) => parent.OnContract(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnError(error);

                /// <inheritdoc/>
                public void OnCompleted()
                {
                }
            }
        }
    }

    /// <summary>
    /// Combines the latest view-model and default-content values, forwarding only the default content when the view
    /// model is <see langword="null"/> and the default content is non-<see langword="null"/>. Fuses
    /// <c>CombineLatest + Where + Select</c> into a single subscription hop.
    /// </summary>
    /// <param name="viewModelChanges">The view-model property stream.</param>
    /// <param name="defaultContentChanges">The default-content property stream.</param>
    private sealed class DefaultViewObservable(
        IObservable<object?> viewModelChanges,
        IObservable<NSViewController?> defaultContentChanges) : IObservable<NSViewController?>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<NSViewController?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            var sink = new Sink(observer);
            sink.Run(viewModelChanges, defaultContentChanges);
            return sink;
        }

        /// <summary>
        /// Tracks the latest view-model and default-content values, emitting the default content only when the
        /// view model is <see langword="null"/> and the default content is non-<see langword="null"/>.
        /// </summary>
        /// <param name="downstream">The downstream observer.</param>
        private sealed class Sink(IObserver<NSViewController?> downstream) : IDisposable
        {
            /// <summary>Serializes value tracking across the two sources.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The latest view model value (valid once <see cref="_hasViewModel"/> is set).</summary>
            private object? _viewModel;

            /// <summary>The latest default content value (valid once <see cref="_hasDefaultContent"/> is set).</summary>
            private NSViewController? _defaultContent;

            /// <summary>Whether the view-model source has produced a value.</summary>
            private bool _hasViewModel;

            /// <summary>Whether the default-content source has produced a value.</summary>
            private bool _hasDefaultContent;

            /// <summary>The view-model source subscription.</summary>
            private IDisposable? _vmSubscription;

            /// <summary>The default-content source subscription.</summary>
            private IDisposable? _defaultContentSubscription;

            /// <summary>Subscribes to both sources.</summary>
            /// <param name="viewModelChanges">The view-model property stream.</param>
            /// <param name="defaultContentChanges">The default-content property stream.</param>
            public void Run(IObservable<object?> viewModelChanges, IObservable<NSViewController?> defaultContentChanges)
            {
                _vmSubscription = viewModelChanges.Subscribe(new ViewModelObserver(this));
                _defaultContentSubscription = defaultContentChanges.Subscribe(new DefaultContentObserver(this));
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _vmSubscription?.Dispose();
                _defaultContentSubscription?.Dispose();
            }

            /// <summary>Records the latest view-model value and emits if conditions are satisfied.</summary>
            /// <param name="value">The new view-model value.</param>
            private void OnViewModel(object? value)
            {
                lock (_gate)
                {
                    _viewModel = value;
                    _hasViewModel = true;
                    Emit();
                }
            }

            /// <summary>Records the latest default-content value and emits if conditions are satisfied.</summary>
            /// <param name="value">The new default-content value.</param>
            private void OnDefaultContent(NSViewController? value)
            {
                lock (_gate)
                {
                    _defaultContent = value;
                    _hasDefaultContent = true;
                    Emit();
                }
            }

            /// <summary>
            /// Emits the default content when both sources have produced a value, the view model is
            /// <see langword="null"/>, and the default content is non-<see langword="null"/>.
            /// Caller must hold <see cref="_gate"/>.
            /// </summary>
            private void Emit()
            {
                if (!_hasViewModel || !_hasDefaultContent || _viewModel is not null || _defaultContent is null)
                {
                    return;
                }

                downstream.OnNext(_defaultContent);
            }

            /// <summary>Forwards an error downstream.</summary>
            /// <param name="error">The error.</param>
            private void OnError(Exception error)
            {
                lock (_gate)
                {
                    downstream.OnError(error);
                }
            }

            /// <summary>Observes the view-model source.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class ViewModelObserver(Sink parent) : IObserver<object?>
            {
                /// <inheritdoc/>
                public void OnNext(object? value) => parent.OnViewModel(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnError(error);

                /// <inheritdoc/>
                public void OnCompleted()
                {
                }
            }

            /// <summary>Observes the default-content source.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class DefaultContentObserver(Sink parent) : IObserver<NSViewController?>
            {
                /// <inheritdoc/>
                public void OnNext(NSViewController? value) => parent.OnDefaultContent(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnError(error);

                /// <inheritdoc/>
                public void OnCompleted()
                {
                }
            }
        }
    }

    /// <summary>
    /// Delivers each notification to the downstream observer on a scheduler. Local copy of
    /// <c>ObserveOnObservable</c> for use within the platform-specific build that excludes <c>Shared/Platform</c>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="scheduler">The scheduler on which to deliver notifications.</param>
    private sealed class ObserveOnObservableLocal<T>(IObservable<T> source, IScheduler scheduler) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            return source.Subscribe(new Sink(observer, scheduler));
        }

        /// <summary>Reschedules each notification onto the scheduler.</summary>
        /// <param name="downstream">The downstream observer.</param>
        /// <param name="scheduler">The scheduler each notification is delivered on.</param>
        private sealed class Sink(IObserver<T> downstream, IScheduler scheduler) : IObserver<T>
        {
            /// <inheritdoc/>
            public void OnNext(T value) =>
                scheduler.Schedule((Downstream: downstream, Value: value), static (_, state) =>
                {
                    state.Downstream.OnNext(state.Value);
                    return EmptyDisposable.Instance;
                });

            /// <inheritdoc/>
            public void OnError(Exception error) =>
                scheduler.Schedule((Downstream: downstream, Error: error), static (_, state) =>
                {
                    state.Downstream.OnError(state.Error);
                    return EmptyDisposable.Instance;
                });

            /// <inheritdoc/>
            public void OnCompleted() =>
                scheduler.Schedule(downstream, static (_, observer) =>
                {
                    observer.OnCompleted();
                    return EmptyDisposable.Instance;
                });
        }
    }

    /// <summary>
    /// Forwards only values that satisfy a predicate. Local copy of <c>WhereObservable</c> for use within the
    /// platform-specific build that excludes <c>Shared/Platform</c>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source to filter.</param>
    /// <param name="predicate">Returns <see langword="true"/> for values that should be forwarded.</param>
    private sealed class WhereObservableLocal<T>(IObservable<T> source, Func<T, bool> predicate) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            return source.Subscribe(new Sink(observer, predicate));
        }

        /// <summary>Forwards values that satisfy the predicate.</summary>
        /// <param name="downstream">The downstream observer.</param>
        /// <param name="predicate">The filter predicate.</param>
        private sealed class Sink(IObserver<T> downstream, Func<T, bool> predicate) : IObserver<T>
        {
            /// <inheritdoc/>
            public void OnNext(T value)
            {
                if (!predicate(value))
                {
                    return;
                }

                downstream.OnNext(value);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }
}
