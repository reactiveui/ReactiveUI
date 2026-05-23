// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows;
using ReactiveUI.Internal;

namespace ReactiveUI;

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
            return EmptyObservable<bool>.Instance;
        }

        var viewLoaded = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object sender, RoutedEventArgs e) => onNext(true);
            fe.Loaded += Handler;
            return new ActionDisposable(() => fe.Loaded -= Handler);
        });

        var viewUnloaded = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object sender, RoutedEventArgs e) => onNext(false);
            fe.Unloaded += Handler;
            return new ActionDisposable(() => fe.Unloaded -= Handler);
        });

        var hitTestVisible = new FromEventObservable<bool>(onNext =>
        {
            void Handler(object sender, DependencyPropertyChangedEventArgs e) => onNext((bool)e.NewValue);
            fe.IsHitTestVisibleChanged += Handler;
            return new ActionDisposable(() => fe.IsHitTestVisibleChanged -= Handler);
        });

        // Replaces viewLoaded.Merge(viewUnloaded).Merge(hitTestVisible).Merge(windowActivation).DistinctUntilChanged().
        return new MergedDistinctObservable<bool>(viewLoaded, viewUnloaded, hitTestVisible, GetActivationForWindow(view));
    }

    /// <summary>Gets the activation observable for a Window, signalling false when it closes.</summary>
    /// <param name="view">The view to observe.</param>
    /// <returns>An observable that emits activation state for the window.</returns>
    private static IObservable<bool> GetActivationForWindow(IActivatableView view)
    {
        if (view is not Window window)
        {
            return EmptyObservable<bool>.Instance;
        }

        return new FromEventObservable<bool>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(false);
            window.Closed += Handler;
            return new ActionDisposable(() => window.Closed -= Handler);
        });
    }
}
