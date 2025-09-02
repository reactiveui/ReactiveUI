// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI;

/// <summary>
/// A set of extension methods to help wire up View and ViewModel activation.
/// </summary>
public static class ViewForMixins
{
    private static readonly MemoizingMRUCache<Type, IActivationForViewFetcher?> _activationFetcherCache =
        new(
            (t, _) =>
                Locator.Current
                       .GetServices<IActivationForViewFetcher?>()
                       .Aggregate((count: 0, viewFetcher: default(IActivationForViewFetcher?)), (acc, x) =>
                       {
                           var score = x?.GetAffinityForView(t) ?? 0;
                           return score > acc.count ? (score, x) : acc;
                       }).viewFetcher,
            RxApp.SmallCacheLimit);

    static ViewForMixins() => RxApp.EnsureInitialized();

    /// <summary>
    /// WhenActivated allows you to register a Func to be called when a
    /// ViewModel's View is Activated.
    /// </summary>
    /// <param name="item">Object that supports activation.</param>
    /// <param name="block">
    /// The method to be called when the corresponding
    /// View is activated. It returns a list of Disposables that will be
    /// cleaned up when the View is deactivated.
    /// </param>
    public static void WhenActivated(this IActivatableViewModel item, Func<IEnumerable<IDisposable>> block) // TODO: Create Test
    {
        item.ArgumentNullExceptionThrowIfNull(nameof(item));

        item.Activator.AddActivationBlock(block);
    }

    /// <summary>
    /// WhenActivated allows you to register a Func to be called when a
    /// ViewModel's View is Activated.
    /// </summary>
    /// <param name="item">Object that supports activation.</param>
    /// <param name="block">
    /// The method to be called when the corresponding
    /// View is activated. The Action parameter (usually called 'd') allows
    /// you to register Disposables to be cleaned up when the View is
    /// deactivated (i.e. "d(someObservable.Subscribe());").
    /// </param>
    public static void WhenActivated(this IActivatableViewModel item, Action<Action<IDisposable>> block)
    {
        item.ArgumentNullExceptionThrowIfNull(nameof(item));

        item.Activator.AddActivationBlock(() =>
        {
            var ret = new List<IDisposable>();
            block(ret.Add);
            return ret;
        });
    }

    /// <summary>
    /// WhenActivated allows you to register a Func to be called when a
    /// ViewModel's View is Activated.
    /// </summary>
    /// <param name="item">Object that supports activation.</param>
    /// <param name="block">
    /// The method to be called when the corresponding
    /// View is activated. The Action parameter (usually called 'disposables') allows
    /// you to collate all the disposables to be cleaned up during deactivation.
    /// </param>
    public static void WhenActivated(this IActivatableViewModel item, Action<CompositeDisposable> block) // TODO: Create Test
    {
        item.ArgumentNullExceptionThrowIfNull(nameof(item));

        item.Activator.AddActivationBlock(() =>
        {
            var d = new CompositeDisposable();
            block(d);
            return [d];
        });
    }

    /// <summary>
    /// WhenActivated allows you to register a Func to be called when a
    /// View is Activated.
    /// </summary>
    /// <param name="item">Object that supports activation.</param>
    /// <param name="block">
    /// The method to be called when the corresponding
    /// View is activated. It returns a list of Disposables that will be
    /// cleaned up when the View is deactivated.
    /// </param>
    /// <returns>A Disposable that deactivates this registration.</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("WhenActivated may reference types that could be trimmed")]
    [RequiresDynamicCode("WhenActivated uses reflection which requires dynamic code generation")]
#endif
    public static IDisposable WhenActivated(this IActivatableView item, Func<IEnumerable<IDisposable>> block) // TODO: Create Test
    {
        item.ArgumentNullExceptionThrowIfNull(nameof(item));

        return item.WhenActivated(block, null);
    }

    /// <summary>
    /// WhenActivated allows you to register a Func to be called when a
    /// View is Activated.
    /// </summary>
    /// <param name="item">Object that supports activation.</param>
    /// <param name="block">
    /// The method to be called when the corresponding
    /// View is activated. It returns a list of Disposables that will be
    /// cleaned up when the View is deactivated.
    /// </param>
    /// <param name="view">
    /// The IActivatableView will ordinarily also host the View
    /// Model, but in the event it is not, a class implementing <see cref="IViewFor" />
    /// can be supplied here.
    /// </param>
    /// <returns>A Disposable that deactivates this registration.</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("WhenActivated may reference types that could be trimmed")]
    [RequiresDynamicCode("WhenActivated uses reflection which requires dynamic code generation")]
#endif
    public static IDisposable WhenActivated(this IActivatableView item, Func<IEnumerable<IDisposable>> block, IViewFor? view) // TODO: Create Test
    {
        item.ArgumentNullExceptionThrowIfNull(nameof(item));

        var activationFetcher = _activationFetcherCache.Get(item.GetType());
        if (activationFetcher is null)
        {
            const string msg = "Don't know how to detect when {0} is activated/deactivated, you may need to implement IActivationForViewFetcher";
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, msg, item.GetType().FullName));
        }

        var activationEvents = activationFetcher.GetActivationForView(item);

        var vmDisposable = Disposable.Empty;
        if ((view ?? item) is IViewFor v)
        {
            vmDisposable = HandleViewModelActivation(v, activationEvents);
        }

        var viewDisposable = HandleViewActivation(block, activationEvents);
        return new CompositeDisposable(vmDisposable, viewDisposable);
    }

    /// <summary>
    /// WhenActivated allows you to register a Func to be called when a
    /// View is Activated.
    /// </summary>
    /// <param name="item">Object that supports activation.</param>
    /// <param name="block">
    /// The method to be called when the corresponding
    /// View is activated. The Action parameter (usually called 'd') allows
    /// you to register Disposables to be cleaned up when the View is
    /// deactivated (i.e. "d(someObservable.Subscribe());").
    /// </param>
    /// <returns>A Disposable that deactivates this registration.</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("WhenActivated may reference types that could be trimmed")]
    [RequiresDynamicCode("WhenActivated uses reflection which requires dynamic code generation")]
#endif
    public static IDisposable WhenActivated(this IActivatableView item, Action<Action<IDisposable>> block) =>
        item.WhenActivated(block, null!);

    /// <summary>
    /// WhenActivated allows you to register a Func to be called when a
    /// View is Activated.
    /// </summary>
    /// <param name="item">Object that supports activation.</param>
    /// <param name="block">
    /// The method to be called when the corresponding
    /// View is activated. The Action parameter (usually called 'd') allows
    /// you to register Disposables to be cleaned up when the View is
    /// deactivated (i.e. "d(someObservable.Subscribe());").
    /// </param>
    /// <param name="view">
    /// The IActivatableView will ordinarily also host the View
    /// Model, but in the event it is not, a class implementing <see cref="IViewFor" />
    /// can be supplied here.
    /// </param>
    /// <returns>A Disposable that deactivates this registration.</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("WhenActivated may reference types that could be trimmed")]
    [RequiresDynamicCode("WhenActivated uses reflection which requires dynamic code generation")]
#endif
    public static IDisposable WhenActivated(this IActivatableView item, Action<Action<IDisposable>> block, IViewFor view) => // TODO: Create Test
        item.WhenActivated(
                           () =>
                           {
                               var ret = new List<IDisposable>();
                               block(ret.Add);
                               return ret;
                           },
                           view);

    /// <summary>
    /// WhenActivated allows you to register a Func to be called when a
    /// View is Activated.
    /// </summary>
    /// <param name="item">Object that supports activation.</param>
    /// <param name="block">
    /// The method to be called when the corresponding
    /// View is activated. The Action parameter (usually called 'disposables') allows
    /// you to collate all disposables that should be cleaned up during deactivation.
    /// </param>
    /// <param name="view">
    /// The IActivatableView will ordinarily also host the View
    /// Model, but in the event it is not, a class implementing <see cref="IViewFor" />
    /// can be supplied here.
    /// </param>
    /// <returns>A Disposable that deactivates this registration.</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("WhenActivated may reference types that could be trimmed")]
    [RequiresDynamicCode("WhenActivated uses reflection which requires dynamic code generation")]
#endif
    public static IDisposable WhenActivated(this IActivatableView item, Action<CompositeDisposable> block, IViewFor? view = null) =>
            item.WhenActivated(
                               () =>
                               {
                                   var d = new CompositeDisposable();
                                   block(d);
                                   return [d];
                               },
                               view);

    private static CompositeDisposable HandleViewActivation(Func<IEnumerable<IDisposable>> block, IObservable<bool> activation)
    {
        var viewDisposable = new SerialDisposable();

        return new CompositeDisposable(
                                       activation.Subscribe(activated =>
                                       {
                                           // NB: We need to make sure to respect ordering so that the clean up
                                           // happens before we invoke block again
                                           viewDisposable.Disposable = Disposable.Empty;
                                           if (activated)
                                           {
                                               viewDisposable.Disposable = new CompositeDisposable(block());
                                           }
                                       }),
                                       viewDisposable);
    }

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("ViewModel activation may reference types that could be trimmed")]
    [RequiresDynamicCode("ViewModel activation uses reflection which requires dynamic code generation")]
#endif
    private static CompositeDisposable HandleViewModelActivation(IViewFor view, IObservable<bool> activation)
    {
        var vmDisposable = new SerialDisposable();
        var viewVmDisposable = new SerialDisposable();

        return new CompositeDisposable(
                                       activation.Subscribe(activated =>
                                       {
                                           if (activated)
                                           {
                                               viewVmDisposable.Disposable = view.WhenAnyValue<IViewFor, object?>(nameof(view.ViewModel))
                                                   .Select(x => x as IActivatableViewModel)
                                                   .Subscribe(x =>
                                                   {
                                                       // NB: We need to make sure to respect ordering so that the clean up
                                                       // happens before we activate again
                                                       vmDisposable.Disposable = Disposable.Empty;
                                                       if (x is not null)
                                                       {
                                                           vmDisposable.Disposable = x.Activator.Activate();
                                                       }
                                                   });
                                           }
                                           else
                                           {
                                               viewVmDisposable.Disposable = Disposable.Empty;
                                               vmDisposable.Disposable = Disposable.Empty;
                                           }
                                       }),
                                       vmDisposable,
                                       viewVmDisposable);
    }
}
