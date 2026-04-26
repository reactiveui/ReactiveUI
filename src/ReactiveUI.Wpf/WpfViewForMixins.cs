// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace ReactiveUI;

/// <summary>
/// WPF-specific activation helpers.
/// </summary>
public static class WpfViewForMixins
{
    /// <summary>
    /// Gets a value indicating whether the WPF view is currently being loaded by a designer surface.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="item">The view being activated.</param>
    /// <returns><see langword="true"/> when the WPF designer is loading the view; otherwise, <see langword="false"/>.</returns>
    public static bool GetIsDesignMode<TView>(this TView item)
        where TView : FrameworkElement, IActivatableView
    {
        ArgumentExceptionHelper.ThrowIfNull(item);

        return DesignerProperties.GetIsInDesignMode(item);
    }

    /// <summary>
    /// Activates the specified WPF view and registers a block of disposables to be disposed when the view is deactivated.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="item">The view to activate.</param>
    /// <param name="block">A function that returns disposables to be disposed when the view is deactivated.</param>
    /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes the registered disposables when disposed.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IDisposable WhenActivated<TView>(this TView item, Func<IEnumerable<IDisposable>> block)
        where TView : FrameworkElement, IActivatableView
    {
        ArgumentExceptionHelper.ThrowIfNull(item);
        ArgumentExceptionHelper.ThrowIfNull(block);

        return item.GetIsDesignMode() ? Disposable.Empty : ViewForMixins.WhenActivated(item, block);
    }

    /// <summary>
    /// Registers a block of disposables to be activated and disposed in sync with the WPF view lifecycle.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="item">The view to activate.</param>
    /// <param name="block">A function that returns disposables to be disposed when the view is deactivated.</param>
    /// <param name="view">An optional view instance to use for view model activation.</param>
    /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes the registered disposables when disposed.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IDisposable WhenActivated<TView>(this TView item, Func<IEnumerable<IDisposable>> block, IViewFor? view)
        where TView : FrameworkElement, IActivatableView
    {
        ArgumentExceptionHelper.ThrowIfNull(item);
        ArgumentExceptionHelper.ThrowIfNull(block);

        return item.GetIsDesignMode() ? Disposable.Empty : ViewForMixins.WhenActivated(item, block, view);
    }

    /// <summary>
    /// Registers a block of code to be executed when the specified WPF view is activated.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="item">The view to activate.</param>
    /// <param name="block">An action that receives a callback for registering disposables.</param>
    /// <returns>An <see cref="IDisposable"/> that unregisters the activation logic.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IDisposable WhenActivated<TView>(this TView item, Action<Action<IDisposable>> block)
        where TView : FrameworkElement, IActivatableView
    {
        ArgumentExceptionHelper.ThrowIfNull(item);
        ArgumentExceptionHelper.ThrowIfNull(block);

        return item.GetIsDesignMode() ? Disposable.Empty : ViewForMixins.WhenActivated(item, block);
    }

    /// <summary>
    /// Registers a block of code to be executed when the specified WPF view is activated.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="item">The view to activate.</param>
    /// <param name="block">An action that receives a callback for registering disposables.</param>
    /// <param name="view">The view instance to use for view model activation.</param>
    /// <returns>An <see cref="IDisposable"/> that unregisters the activation logic.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IDisposable WhenActivated<TView>(this TView item, Action<Action<IDisposable>> block, IViewFor view)
        where TView : FrameworkElement, IActivatableView
    {
        ArgumentExceptionHelper.ThrowIfNull(item);
        ArgumentExceptionHelper.ThrowIfNull(block);

        return item.GetIsDesignMode() ? Disposable.Empty : ViewForMixins.WhenActivated(item, block, view);
    }

    /// <summary>
    /// Activates the specified WPF view and manages the provided disposables for the duration of the activation lifecycle.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="item">The view to activate.</param>
    /// <param name="block">An action that receives a composite disposable for activation-related resources.</param>
    /// <param name="view">An optional view instance to use for view model activation.</param>
    /// <returns>An <see cref="IDisposable"/> that unregisters the activation logic.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IDisposable WhenActivated<TView>(this TView item, Action<CompositeDisposable> block, IViewFor? view = null)
        where TView : FrameworkElement, IActivatableView
    {
        ArgumentExceptionHelper.ThrowIfNull(item);
        ArgumentExceptionHelper.ThrowIfNull(block);

        return item.GetIsDesignMode() ? Disposable.Empty : ViewForMixins.WhenActivated(item, block, view);
    }
}
