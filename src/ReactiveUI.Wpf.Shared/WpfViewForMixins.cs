// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>WPF-specific activation helpers.</summary>
public static class WpfViewForMixins
{
    /// <summary>Provides WPF activation extension methods for activatable framework element views.</summary>
    /// <param name="item">The view.</param>
    /// <typeparam name="TView">The type of the view.</typeparam>
    extension<TView>(TView item)
        where TView : FrameworkElement, IActivatableView
    {
        /// <summary>Gets a value indicating whether the WPF view is currently being loaded by a designer surface.</summary>
        /// <returns><see langword="true"/> when the WPF designer is loading the view; otherwise, <see langword="false"/>.</returns>
        public bool GetIsDesignMode()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            return DesignerProperties.GetIsInDesignMode(item);
        }

        /// <summary>Activates the WPF view for its activation lifecycle without registering any activation-scoped disposables.</summary>
        /// <remarks>Use this no-op overload purely to trigger <see cref="IActivatableViewModel"/> activation when the
        /// view itself has no resources to manage — it avoids the empty <c>WhenActivated(_ =&gt; { })</c> boilerplate.</remarks>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view when disposed.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated() =>
            item.GetIsDesignMode()
                ? EmptyDisposable.Instance
                : ((IActivatableView)item).WhenActivated(static () => (IEnumerable<IDisposable>)[], (IViewFor?)null);

        /// <summary>Activates the specified WPF view and registers a block of disposables to be disposed when the view is deactivated.</summary>
        /// <param name="block">A function that returns disposables to be disposed when the view is deactivated.</param>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes the registered disposables when disposed.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(Func<IEnumerable<IDisposable>> block)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);
            ArgumentExceptionHelper.ThrowIfNull(block);

            return item.GetIsDesignMode() ? EmptyDisposable.Instance : ((IActivatableView)item).WhenActivated(block);
        }

        /// <summary>Registers a block of disposables to be activated and disposed in sync with the WPF view lifecycle.</summary>
        /// <param name="block">A function that returns disposables to be disposed when the view is deactivated.</param>
        /// <param name="view">An optional view instance to use for view model activation.</param>
        /// <returns>An <see cref="IDisposable"/> that deactivates the view and disposes the registered disposables when disposed.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(Func<IEnumerable<IDisposable>> block, IViewFor? view)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);
            ArgumentExceptionHelper.ThrowIfNull(block);

            return item.GetIsDesignMode() ? EmptyDisposable.Instance : ((IActivatableView)item).WhenActivated(block, view);
        }

        /// <summary>Registers a block of code to be executed when the specified WPF view is activated.</summary>
        /// <param name="block">An action that receives a callback for registering disposables.</param>
        /// <returns>An <see cref="IDisposable"/> that unregisters the activation logic.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(Action<Action<IDisposable>> block)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);
            ArgumentExceptionHelper.ThrowIfNull(block);

            return item.GetIsDesignMode() ? EmptyDisposable.Instance : ((IActivatableView)item).WhenActivated(block);
        }

        /// <summary>Registers a block of code to be executed when the specified WPF view is activated.</summary>
        /// <param name="block">An action that receives a callback for registering disposables.</param>
        /// <param name="view">The view instance to use for view model activation.</param>
        /// <returns>An <see cref="IDisposable"/> that unregisters the activation logic.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(Action<Action<IDisposable>> block, IViewFor view)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);
            ArgumentExceptionHelper.ThrowIfNull(block);

            return item.GetIsDesignMode() ? EmptyDisposable.Instance : ((IActivatableView)item).WhenActivated(block, view);
        }

        /// <summary>Activates the specified WPF view and manages the provided disposables for the duration of the activation lifecycle.</summary>
        /// <param name="block">An action that receives a composite disposable for activation-related resources.</param>
        /// <returns>An <see cref="IDisposable"/> that unregisters the activation logic.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(Action<MultipleDisposable> block) => item.WhenActivated(block, null);

        /// <summary>Activates the specified WPF view and manages the provided disposables for the duration of the activation lifecycle.</summary>
        /// <param name="block">An action that receives a composite disposable for activation-related resources.</param>
        /// <param name="view">An optional view instance to use for view model activation.</param>
        /// <returns>An <see cref="IDisposable"/> that unregisters the activation logic.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable WhenActivated(Action<MultipleDisposable> block, IViewFor? view)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);
            ArgumentExceptionHelper.ThrowIfNull(block);

            return item.GetIsDesignMode()
                ? EmptyDisposable.Instance
                : ((IActivatableView)item).WhenActivated(
                    () =>
                    {
                        MultipleDisposable d = [];
                        block(d);
                        return [d];
                    },
                    view);
        }
    }
}
