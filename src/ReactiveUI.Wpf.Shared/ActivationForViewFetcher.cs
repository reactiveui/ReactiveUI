// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// ActivationForViewFetcher is how ReactiveUI determine when a
/// View is activated or deactivated. This is usually only used when porting
/// ReactiveUI to a new UI framework.
/// </summary>
public class ActivationForViewFetcher : IActivationForViewFetcher
{
    /// <summary>The affinity returned when the view is not supported.</summary>
    private const int NoAffinity = 0;

    /// <inheritdoc/>
    public int GetAffinityForView(Type view) =>
        typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? BindingAffinity.ExactType : NoAffinity;

    /// <inheritdoc/>
    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        if (view is not FrameworkElement fe)
        {
            return Signal.None<bool>();
        }

        var viewLoaded = new FromEventObservable<bool>(onNext =>
        {
            RoutedEventHandler handler = (_, _) => onNext(true);
            fe.Loaded += handler;
            return new ActionDisposable(() => fe.Loaded -= handler);
        });

        var viewUnloaded = new FromEventObservable<bool>(onNext =>
        {
            RoutedEventHandler handler = (_, _) => onNext(false);
            fe.Unloaded += handler;
            return new ActionDisposable(() => fe.Unloaded -= handler);
        });

        var hitTestVisible = new FromEventObservable<bool>(onNext =>
        {
            DependencyPropertyChangedEventHandler handler = (_, e) => onNext((bool)e.NewValue);
            fe.IsHitTestVisibleChanged += handler;
            return new ActionDisposable(() => fe.IsHitTestVisibleChanged -= handler);
        });

        // Replaces viewLoaded.Merge(viewUnloaded).Merge(hitTestVisible).Merge(windowActivation).DistinctUntilChanged().
        return ReactiveUI.Primitives.LinqExtensions.BlendUnique(viewLoaded, viewUnloaded, hitTestVisible, GetActivationForWindow(view));
    }

    /// <summary>Gets the activation observable for a Window, signalling false when it closes.</summary>
    /// <param name="view">The view to observe.</param>
    /// <returns>An observable that emits activation state for the window.</returns>
    private static IObservable<bool> GetActivationForWindow(IActivatableView view) =>
        view is not Window window ? Signal.None<bool>() : new FromEventObservable<bool>(onNext =>
        {
            EventHandler handler = (_, _) => onNext(false);
            window.Closed += handler;
            return new ActionDisposable(() => window.Closed -= handler);
        });
}
