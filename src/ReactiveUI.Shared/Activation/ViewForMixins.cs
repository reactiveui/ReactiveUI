// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// Provides extension methods for registering activation logic on views and view models that support activation. These
/// methods enable the execution of custom code when a view or view model is activated or deactivated, facilitating
/// resource management and lifecycle handling in reactive UI scenarios.
/// </summary>
/// <remarks>The methods in this class are typically used to register disposables or cleanup actions that should
/// be tied to the activation lifecycle of a view or view model. This helps ensure that resources such as subscriptions
/// are properly disposed of when the view is deactivated. Some methods accept an optional view parameter for advanced
/// scenarios where the view and view model are not hosted together. Use these methods to simplify activation-aware
/// resource management in MVVM architectures. Thread safety and correct disposal are managed internally. For unit
/// testing purposes, the cache used to optimize activation fetcher lookups can be reset using the provided internal
/// method; this should not be used in production code.</remarks>
public static class ViewForMixins
{
    /// <summary>Cache mapping view types to their resolved activation fetcher, to avoid repeated service locator lookups.</summary>
    private static readonly MemoizingMRUCache<Type, IActivationForViewFetcher?> _activationFetcherCache =
        new((t, _) => ResolveActivationFetcher(t), RxCacheSize.SmallCacheLimit);

    /// <summary>Provides activation lifecycle extension members for <see cref="IActivatableView"/>.</summary>
    /// <param name="item">The view whose activation lifecycle will manage the disposables.</param>
    extension(IActivatableView item)
    {
        /// <summary>Activates the specified view and registers a block of disposables to be disposed when the view is deactivated.</summary>
        /// <param name="block">A function that returns a collection of <see cref="IDisposable"/> objects to be disposed when the view is
        /// deactivated. Cannot be null.</param>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes the registered disposables when disposed.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable
            WhenActivated(Func<IEnumerable<IDisposable>> block)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            return item.WhenActivated(block, (IViewFor?)null);
        }

        /// <summary>
        /// Registers a block of disposables to be activated and disposed in sync with the activation lifecycle of the
        /// specified view or view model.
        /// </summary>
        /// <remarks>This method is typically used to manage subscriptions or other resources that should only be
        /// active while the view or view model is active. The activation lifecycle is determined by the implementation of
        /// <see cref="IActivatableView"/> and any registered activation fetchers.</remarks>
        /// <param name="block">A function that returns the set of <see cref="IDisposable"/> resources to activate when the view or view model
        /// is activated. The returned disposables will be disposed when the view or view model is deactivated.</param>
        /// <param name="view">An optional <see cref="IViewFor"/> instance to use for activation. If null, <paramref name="item"/> is used.
        /// This parameter allows specifying a different view context for activation.</param>
        /// <returns>An <see cref="IDisposable"/> that deactivates and disposes the registered resources when disposed. Disposing
        /// this object will also unsubscribe from the activation lifecycle.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="item"/> is null or if activation cannot be determined for the specified type.</exception>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(
            Func<IEnumerable<IDisposable>> block,
            IViewFor? view)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            var activationEvents = ResolveActivationEvents(item);

            IDisposable viewModelDisposable = EmptyDisposable.Instance;
            if ((view ?? item) is IViewFor v)
            {
                viewModelDisposable = HandleViewModelActivation(v, activationEvents);
            }

            var viewDisposable = HandleViewActivation(block, activationEvents);
            return new MultipleDisposable(viewModelDisposable, viewDisposable);
        }

        /// <summary>
        /// Registers a block of activation logic to be executed when the specified view is activated, and disposes of
        /// resources when the view is deactivated.
        /// </summary>
        /// <remarks>Use this method to manage resources or subscriptions that should only be active while the
        /// view is active. The provided block is invoked each time the view is activated, and any disposables registered
        /// within the block are disposed when the view is deactivated.</remarks>
        /// <param name="block">An action that receives a disposable registration callback. Use this callback to register disposables that
        /// should be disposed when the view is deactivated.</param>
        /// <returns>An <see cref="IDisposable"/> that, when disposed, unregisters the activation logic and disposes any registered
        /// resources.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(Action<Action<IDisposable>> block) =>
            item.WhenActivated(block, (IViewFor)null!);

        /// <summary>Activates the specified view and manages the provided disposables for the duration of the activation lifecycle.</summary>
        /// <remarks>This method is typically used to manage subscriptions or other resources that should be tied
        /// to the view's activation lifecycle. All disposables registered via the provided callback will be disposed when
        /// the returned <see cref="IDisposable"/> is disposed. Reflection is used to evaluate expression-based member
        /// chains, which may be affected by trimming in some deployment scenarios.</remarks>
        /// <param name="block">An action that receives a callback for registering <see cref="IDisposable"/> resources to be disposed when the
        /// view is deactivated. Cannot be null.</param>
        /// <param name="view">The view instance associated with the activation. Cannot be null.</param>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes all registered resources when disposed.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(
            Action<Action<IDisposable>> block,
            IViewFor view) =>
            item.WhenActivated(
                () =>
                {
                    List<IDisposable> ret = [];
                    block(ret.Add);
                    return ret;
                },
                view);

        /// <summary>
        /// Activates the specified view and executes the provided block when the view is activated, managing disposables
        /// for the activation lifecycle.
        /// </summary>
        /// <param name="block">An action that receives a MultipleDisposable to which activation-related disposables should be added.</param>
        /// <returns>An IDisposable that deactivates the view and disposes of all registered disposables when disposed.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(
            Action<MultipleDisposable> block) =>
            item.WhenActivated(block, (IViewFor?)null);

        /// <summary>
        /// Activates the specified view and executes the provided block when the view is activated, managing disposables
        /// for the activation lifecycle.
        /// </summary>
        /// <param name="block">An action that receives a MultipleDisposable to which activation-related disposables should be added.</param>
        /// <param name="view">An optional IViewFor instance representing the view context. If null, the item itself is used as the view.</param>
        /// <returns>An IDisposable that deactivates the view and disposes of all registered disposables when disposed.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(
            Action<MultipleDisposable> block,
            IViewFor? view) =>
            item.WhenActivated(
                () =>
                {
                    MultipleDisposable d = [];
                    block(d);
                    return [d];
                },
                view);

        /// <summary>Activates the view for its activation lifecycle without registering any activation-scoped disposables.</summary>
        /// <remarks>Use this no-op overload purely to trigger <see cref="IActivatableViewModel"/> activation when the
        /// view itself has no resources to manage — it avoids the empty <c>WhenActivated(_ =&gt; { })</c> boilerplate.</remarks>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes registered resources when disposed.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated() =>
            item.WhenActivated(static () => (IEnumerable<IDisposable>)[], (IViewFor?)null);

        /// <summary>
        /// Registers a block of disposables to be activated and disposed in sync with the view's activation lifecycle,
        /// forwarding <see cref="IActivatableViewModel"/> activation using a caller-supplied ViewModel-change signal.
        /// </summary>
        /// <remarks>
        /// This is the trim- and AOT-safe counterpart to the reflected <c>WhenActivated(block, IViewFor)</c> overloads:
        /// they discover the view's ViewModel with an expression/string-based <c>WhenAnyValue</c>, which requires
        /// unreferenced code. Here the caller passes <paramref name="viewModelChanged"/> directly, so no reflection is
        /// used and the member carries no <c>[RequiresUnreferencedCode]</c> annotation. Emit the current ViewModel (and
        /// every subsequent value, e.g. <see langword="null"/> on clear) on <paramref name="viewModelChanged"/> using a
        /// reflection-free source such as the source-generated <c>WhenAnyValue</c> from <c>ReactiveUI.SourceGenerators</c>
        /// or a hand-written <see cref="System.ComponentModel.INotifyPropertyChanged"/> subscription.
        /// </remarks>
        /// <param name="block">A function that returns the set of <see cref="IDisposable"/> resources to activate when the view is activated.
        /// The returned disposables are disposed when the view is deactivated.</param>
        /// <param name="viewModelChanged">An observable that emits the view's ViewModel whenever it changes (including its current value). When an emitted
        /// value is an <see cref="IActivatableViewModel"/>, its activator is activated for the duration of the view's activation.</param>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes the registered resources when disposed.</returns>
        public IDisposable WhenActivated(
            Func<IEnumerable<IDisposable>> block,
            IObservable<object?> viewModelChanged)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);
            ArgumentExceptionHelper.ThrowIfNull(viewModelChanged);

            var activationEvents = ResolveActivationEvents(item);

            var viewModelDisposable = HandleViewModelActivation(viewModelChanged, activationEvents);
            var viewDisposable = HandleViewActivation(block, activationEvents);
            return new MultipleDisposable(viewModelDisposable, viewDisposable);
        }

        /// <summary>
        /// Registers a block of activation logic to be executed when the view is activated, forwarding
        /// <see cref="IActivatableViewModel"/> activation using a caller-supplied ViewModel-change signal.
        /// </summary>
        /// <remarks>This is the trim- and AOT-safe counterpart to the reflected <c>WhenActivated(Action&lt;Action&lt;IDisposable&gt;&gt;, IViewFor)</c>
        /// overload; the <paramref name="viewModelChanged"/> observable replaces reflection-based ViewModel discovery. Produce it with a
        /// reflection-free source such as the source-generated <c>WhenAnyValue</c> or a hand-written
        /// <see cref="System.ComponentModel.INotifyPropertyChanged"/> subscription.</remarks>
        /// <param name="block">An action that receives a callback for registering disposables that should be disposed when the view is deactivated.</param>
        /// <param name="viewModelChanged">An observable that emits the view's ViewModel whenever it changes (including its current value).</param>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes the registered resources when disposed.</returns>
        public IDisposable WhenActivated(
            Action<Action<IDisposable>> block,
            IObservable<object?> viewModelChanged) =>
            item.WhenActivated(
                () =>
                {
                    List<IDisposable> ret = [];
                    block(ret.Add);
                    return ret;
                },
                viewModelChanged);

        /// <summary>
        /// Registers a block of activation logic to be executed when the view is activated, forwarding
        /// <see cref="IActivatableViewModel"/> activation using a caller-supplied ViewModel-change signal.
        /// </summary>
        /// <remarks>This is the trim- and AOT-safe counterpart to the reflected <c>WhenActivated(Action&lt;MultipleDisposable&gt;, IViewFor)</c>
        /// overload; the <paramref name="viewModelChanged"/> observable replaces reflection-based ViewModel discovery. Produce it with a
        /// reflection-free source such as the source-generated <c>WhenAnyValue</c> or a hand-written
        /// <see cref="System.ComponentModel.INotifyPropertyChanged"/> subscription.</remarks>
        /// <param name="block">An action that receives a <see cref="MultipleDisposable"/> to which activation-related disposables can be added.</param>
        /// <param name="viewModelChanged">An observable that emits the view's ViewModel whenever it changes (including its current value).</param>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes the registered resources when disposed.</returns>
        public IDisposable WhenActivated(
            Action<MultipleDisposable> block,
            IObservable<object?> viewModelChanged) =>
            item.WhenActivated(
                () =>
                {
                    MultipleDisposable d = [];
                    block(d);
                    return [d];
                },
                viewModelChanged);

        /// <summary>
        /// Activates the view for its activation lifecycle without registering any activation-scoped disposables,
        /// forwarding <see cref="IActivatableViewModel"/> activation using a caller-supplied ViewModel-change signal.
        /// </summary>
        /// <remarks>This is the trim- and AOT-safe counterpart to the reflected parameterless <c>WhenActivated()</c> overload;
        /// the <paramref name="viewModelChanged"/> observable replaces reflection-based ViewModel discovery. Produce it with a
        /// reflection-free source such as the source-generated <c>WhenAnyValue</c> or a hand-written
        /// <see cref="System.ComponentModel.INotifyPropertyChanged"/> subscription.</remarks>
        /// <param name="viewModelChanged">An observable that emits the view's ViewModel whenever it changes (including its current value).</param>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes registered resources when disposed.</returns>
        public IDisposable WhenActivated(IObservable<object?> viewModelChanged) =>
            item.WhenActivated(static () => (IEnumerable<IDisposable>)[], viewModelChanged);

        /// <summary>Gets a value indicating whether the view is currently being loaded by a designer surface.</summary>
        /// <returns><see langword="false"/> by default. Platform packages can provide more specific overloads for their view types.</returns>
        public bool GetIsDesignMode()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            return false;
        }
    }

    /// <summary>Provides activation lifecycle extension members for <see cref="IActivatableViewModel"/>.</summary>
    /// <param name="item">The view model whose activation lifecycle will manage the disposables.</param>
    extension(IActivatableViewModel item)
    {
        /// <summary>Registers a block of disposables to be created and disposed with the activation lifecycle of the specified view model.</summary>
        /// <remarks>Use this method to associate resources with the activation and deactivation of a view model.
        /// The disposables returned by the block will be disposed automatically when the view model is
        /// deactivated.</remarks>
        /// <param name="block">A function that returns the disposables to be created when the view model is activated. Cannot be null.</param>
        public void
            WhenActivated(Func<IEnumerable<IDisposable>> block)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            item.Activator.AddActivationBlock(block);
        }

        /// <summary>
        /// Registers a block of code to be executed when the specified view model is activated, allowing disposable
        /// resources to be managed for the activation period.
        /// </summary>
        /// <remarks>Use this method to associate resources or subscriptions with the activation lifecycle of a
        /// view model. All disposables registered within the block will be automatically disposed when the view model is
        /// deactivated, helping to prevent resource leaks.</remarks>
        /// <param name="block">An action that receives a callback for registering disposables. Disposables added via this callback will be
        /// disposed when the activation ends.</param>
        public void WhenActivated(Action<Action<IDisposable>> block)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            item.Activator.AddActivationBlock(() =>
            {
                List<IDisposable> ret = [];
                block(ret.Add);
                return ret;
            });
        }

        /// <summary>
        /// Registers a block of code to be executed when the specified view model is activated, and ensures that any
        /// disposables created within the block are disposed when the view model is deactivated.
        /// </summary>
        /// <remarks>Use this method to manage subscriptions or other resources that should be tied to the
        /// activation lifecycle of the view model. All disposables added to the provided <see cref="MultipleDisposable"/>
        /// will be disposed automatically when the view model is deactivated.</remarks>
        /// <param name="block">An action that receives a <see cref="MultipleDisposable"/> to which disposables can be added for automatic
        /// cleanup upon deactivation. Cannot be null.</param>
        public void
            WhenActivated(Action<MultipleDisposable> block)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            item.Activator.AddActivationBlock(() =>
            {
                MultipleDisposable d = [];
                block(d);
                return [d];
            });
        }
    }

    /// <summary>
    /// Clears the activation fetcher cache. This method is intended for use by unit tests
    /// to ensure the cache is invalidated when the service locator is reset.
    /// </summary>
    /// <remarks>
    /// WARNING: This method should ONLY be used in unit tests to reset cache state between test runs.
    /// Never call this in production code as it will force re-querying of activation fetchers
    /// from the service locator on the next access.
    /// </remarks>
    internal static void ResetActivationFetcherCacheForTesting() => _activationFetcherCache.InvalidateAll();

    /// <summary>Selects the registered activation fetcher with the highest affinity for the given view type.</summary>
    /// <param name="viewType">The view type to resolve an activation fetcher for.</param>
    /// <returns>The highest-affinity fetcher, or <see langword="null"/> when none have positive affinity.</returns>
    private static IActivationForViewFetcher? ResolveActivationFetcher(Type viewType)
    {
        var bestScore = 0;
        IActivationForViewFetcher? best = null;
        foreach (var fetcher in AppLocator.Current.GetServices<IActivationForViewFetcher?>())
        {
            var score = fetcher?.GetAffinityForView(viewType) ?? 0;
            if (score > bestScore)
            {
                bestScore = score;
                best = fetcher;
            }
        }

        return best;
    }

    /// <summary>Resolves the activation event stream for the view via its highest-affinity activation fetcher.</summary>
    /// <param name="item">The view to resolve activation events for.</param>
    /// <returns>An observable that emits <see langword="true"/> on activation and <see langword="false"/> on deactivation.</returns>
    /// <exception cref="ArgumentException">Thrown when no registered <see cref="IActivationForViewFetcher"/> can determine activation for the view type.</exception>
    private static IObservable<bool> ResolveActivationEvents(IActivatableView item)
    {
        var activationFetcher = _activationFetcherCache.Get(item.GetType());
        if (activationFetcher is null)
        {
            const string Msg =
                "Don't know how to detect when {0} is activated/deactivated, you may need to implement IActivationForViewFetcher";
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Msg, item.GetType().FullName));
        }

        return activationFetcher.GetActivationForView(item);
    }

    /// <summary>
    /// Manages the activation and deactivation lifecycle of a view by subscribing to an activation observable and
    /// invoking a resource allocation block when activated.
    /// </summary>
    /// <remarks>The block is invoked each time the activation observable emits <see langword="true"/>. Any
    /// disposables created by a previous activation are disposed before the block is invoked again. This method is
    /// typically used to manage resources that should only be active while the view is active.</remarks>
    /// <param name="block">A delegate that returns a collection of disposables to be created when the view is activated. The returned
    /// disposables are disposed when the view is deactivated or reactivated.</param>
    /// <param name="activation">An observable sequence that signals activation state changes. Emits <see langword="true"/> to indicate
    /// activation and <see langword="false"/> to indicate deactivation.</param>
    /// <returns>A <see cref="MultipleDisposable"/> that manages the subscription to the activation observable and the
    /// disposables created by the block. Disposing this object cleans up all associated resources.</returns>
    private static MultipleDisposable HandleViewActivation(
        Func<IEnumerable<IDisposable>> block,
        IObservable<bool> activation)
    {
        SwapDisposable viewDisposable = new();

        return new(
            activation.Subscribe(new DelegateObserver<bool>(activated =>
            {
                viewDisposable.Disposable = EmptyDisposable.Instance;
                if (!activated)
                {
                    return;
                }

                viewDisposable.Disposable = new MultipleDisposable([.. block()]);
            })),
            viewDisposable);
    }

    /// <summary>Manages the activation and deactivation lifecycle of a view's ViewModel in response to an activation observable.</summary>
    /// <remarks>This bridge discovers the view's ViewModel with an expression/string-based <c>WhenAnyValue</c> — the only
    /// reflection in the activation path — and forwards to the reflection-free <see cref="HandleViewModelActivation(IObservable{object}, IObservable{bool})"/>.
    /// Callers that already have a ViewModel-change observable should use that overload to stay trim- and AOT-safe.</remarks>
    /// <param name="view">The view implementing the IViewFor interface whose ViewModel activation lifecycle will be managed. Cannot be
    /// null.</param>
    /// <param name="activation">An observable sequence that signals when the view is activated or deactivated. Emits <see langword="true"/> to
    /// indicate activation and <see langword="false"/> for deactivation.</param>
    /// <returns>A MultipleDisposable that manages all subscriptions and resources related to the activation lifecycle.
    /// Disposing this object will clean up all associated subscriptions.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    private static MultipleDisposable HandleViewModelActivation(IViewFor view, IObservable<bool> activation) =>
        HandleViewModelActivation(view.WhenAnyValue<IViewFor, object?>(nameof(view.ViewModel)), activation);

    /// <summary>Manages the activation and deactivation lifecycle of a view's ViewModel in response to an activation observable, without reflection.</summary>
    /// <remarks>This method subscribes to a caller-supplied stream of ViewModel values and manages the activation
    /// state of any <see cref="IActivatableViewModel"/> assigned to the view. Because the ViewModel signal is provided
    /// directly rather than discovered by reflection, this path is safe under trimming and AOT.</remarks>
    /// <param name="viewModelChanged">An observable that emits the view's ViewModel whenever it changes (including its current value).</param>
    /// <param name="activation">An observable sequence that signals when the view is activated or deactivated. Emits <see langword="true"/> to
    /// indicate activation and <see langword="false"/> for deactivation.</param>
    /// <returns>A MultipleDisposable that manages all subscriptions and resources related to the activation lifecycle.
    /// Disposing this object will clean up all associated subscriptions.</returns>
    private static MultipleDisposable HandleViewModelActivation(IObservable<object?> viewModelChanged, IObservable<bool> activation)
    {
        SwapDisposable viewModelDisposable = new();
        SwapDisposable viewViewModelDisposable = new();

        return new(
            activation.Subscribe(new DelegateObserver<bool>(activated =>
            {
                if (activated)
                {
                    viewViewModelDisposable.Disposable = viewModelChanged
                        .Subscribe(new DelegateObserver<object?>(value =>
                        {
                            viewModelDisposable.Disposable = EmptyDisposable.Instance;
                            if (value is not IActivatableViewModel activatable)
                            {
                                return;
                            }

                            viewModelDisposable.Disposable = activatable.Activator.Activate();
                        }));
                }
                else
                {
                    viewViewModelDisposable.Disposable = EmptyDisposable.Instance;
                    viewModelDisposable.Disposable = EmptyDisposable.Instance;
                }
            })),
            viewModelDisposable,
            viewViewModelDisposable);
    }
}
