// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reflection;
using ReactiveUI.Internal;

#if IS_WINUI
using Microsoft.UI.Xaml;
using ReactiveUI.Maui.Internal;

namespace ReactiveUI.WinUI;
#endif
#if IS_MAUI
using Microsoft.Maui.Controls;

namespace ReactiveUI.Maui;
#endif

/// <summary>
/// This class is the default implementation that determines when views are Activated and Deactivated.
/// </summary>
/// <seealso cref="IActivationForViewFetcher" />
public class ActivationForViewFetcher : IActivationForViewFetcher
{
    /// <inheritdoc/>
    public int GetAffinityForView(Type view) =>
#if IS_WINUI
       typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())
#endif
#if IS_MAUI
        typeof(Page).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ||
        typeof(View).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ||
        typeof(Cell).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())
#endif
            ? BindingAffinity.ExactType
            : 0;

    /// <inheritdoc/>
    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        // ?? is right-associative, so casting the terminal operand unifies the differently-typed concrete sinks the
        // helpers return under IObservable<bool>.
        var activation =
            GetActivationFor(view as ICanActivate) ??
#if IS_WINUI
            GetActivationFor(view as FrameworkElement) ??
#endif
#if IS_MAUI
            GetActivationFor(view as Page) ??
            GetActivationFor(view as View) ??
            GetActivationFor(view as Cell) ??
#endif
            (IObservable<bool>)NeverObservable<bool>.Instance;

        return new DistinctUntilChangedObservable<bool>(activation);
    }

    /// <summary>Gets the activation stream for an <see cref="ICanActivate"/>, or null when not applicable.</summary>
    /// <param name="canActivate">The view to observe, or null.</param>
    /// <returns>The activation stream, or null when <paramref name="canActivate"/> is null.</returns>
    private static MergedObservable<bool>? GetActivationFor(ICanActivate? canActivate)
    {
        if (canActivate is null)
        {
            return null;
        }

        // Replaces Activated.Select(_ => true).Merge(Deactivated.Select(_ => false)).
        return new MergedObservable<bool>(
            new SelectObservable<Unit, bool>(canActivate.Activated, static _ => true),
            new SelectObservable<Unit, bool>(canActivate.Deactivated, static _ => false));
    }

#if IS_MAUI
    /// <summary>Gets the activation stream for a <see cref="Page"/>, or null when not applicable.</summary>
    /// <param name="page">The page to observe, or null.</param>
    /// <returns>The activation stream, or null when <paramref name="page"/> is null.</returns>
    private static MergedObservable<bool>? GetActivationFor(Page? page)
    {
        if (page is null)
        {
            return null;
        }

        var appearing = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(true);
            page.Appearing += Handler;
            return new ActionDisposable(() => page.Appearing -= Handler);
        });

        var disappearing = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(false);
            page.Disappearing += Handler;
            return new ActionDisposable(() => page.Disappearing -= Handler);
        });

        return new MergedObservable<bool>(appearing, disappearing);
    }
#endif

#if IS_MAUI
    /// <summary>Gets the activation stream for a <see cref="View"/>, or null when not applicable.</summary>
    /// <param name="view">The view to observe, or null.</param>
    /// <returns>The activation stream, or null when <paramref name="view"/> is null.</returns>
    private static DistinctUntilChangedObservable<bool>? GetActivationFor(View? view)
    {
        if (view is null)
        {
            return null;
        }

        var loaded = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(true);
            view.Loaded += Handler;
            return new ActionDisposable(() => view.Loaded -= Handler);
        });

        var unloaded = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(false);
            view.Unloaded += Handler;
            return new ActionDisposable(() => view.Unloaded -= Handler);
        });

        // Replaces loaded.Merge(unloaded).StartWith(view.IsLoaded).DistinctUntilChanged().
        return new DistinctUntilChangedObservable<bool>(
            new StartWithObservable<bool>(new MergedObservable<bool>(loaded, unloaded), view.IsLoaded));
    }

    /// <summary>Gets the activation stream for a <see cref="Cell"/>, or null when not applicable.</summary>
    /// <param name="cell">The cell to observe, or null.</param>
    /// <returns>The activation stream, or null when <paramref name="cell"/> is null.</returns>
    private static MergedObservable<bool>? GetActivationFor(Cell? cell)
    {
        if (cell is null)
        {
            return null;
        }

        var appearing = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(true);
            cell.Appearing += Handler;
            return new ActionDisposable(() => cell.Appearing -= Handler);
        });

        var disappearing = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(false);
            cell.Disappearing += Handler;
            return new ActionDisposable(() => cell.Disappearing -= Handler);
        });

        return new MergedObservable<bool>(appearing, disappearing);
    }
#else
    /// <summary>Gets the activation stream for a <see cref="FrameworkElement"/>, or null when not applicable.</summary>
    /// <param name="view">The framework element to observe, or null.</param>
    /// <returns>The activation stream, or null when <paramref name="view"/> is null.</returns>
    private static DistinctUntilChangedObservable<bool>? GetActivationFor(FrameworkElement? view)
    {
        if (view is null)
        {
            return null;
        }

        var viewLoaded = new FromEventObservable<bool>(onNext =>
        {
            void Handler(FrameworkElement sender, object args) => onNext(true);
            view.Loading += Handler;
            return new ActionDisposable(() => view.Loading -= Handler);
        });

        var viewUnloaded = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object sender, RoutedEventArgs args) => onNext(false);
            view.Unloaded += Handler;
            return new ActionDisposable(() => view.Unloaded -= Handler);
        });

        // Observe IsHitTestVisible property changes using DependencyProperty (AOT-safe)
        var isHitTestVisible = MauiReactiveHelpers.CreatePropertyValueObservable(
            view,
            nameof(view.IsHitTestVisible),
            FrameworkElement.IsHitTestVisibleProperty,
            () => view.IsHitTestVisible);

        // Replaces Merge(...).Select(b => b ? hitTest.SkipWhile(!x) : false).Switch().DistinctUntilChanged().
        return new DistinctUntilChangedObservable<bool>(
            new SwitchObservable<bool>(
                new SelectObservable<bool, IObservable<bool>>(
                    new MergedObservable<bool>(viewLoaded, viewUnloaded),
                    b => b ? new SkipWhileObservable<bool>(isHitTestVisible, static x => !x) : new ReturnObservable<bool>(false))));
    }
#endif
}
