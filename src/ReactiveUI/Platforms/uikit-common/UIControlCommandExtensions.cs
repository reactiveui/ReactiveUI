// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Extension methods for binding <see cref="ICommand"/> to a <see cref="UIControl"/>.</summary>
public static class UIControlCommandExtensions
{
    /// <summary>Provides command-binding extension members for <see cref="ICommand"/>.</summary>
    /// <param name="item">The command to bind to.</param>
    extension(ICommand item)
    {
        /// <summary>Binds the <see cref="ICommand"/> to target <see cref="UIControl"/>.</summary>
        /// <param name="control">The control.</param>
        /// <param name="events">The events.</param>
        /// <returns>A disposable.</returns>
        public IDisposable BindToTarget(UIControl control, UIControlEvent events)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);
            ArgumentExceptionHelper.ThrowIfNull(control);

            var ev = new EventHandler((_, _) =>
            {
                if (!item.CanExecute(null))
                {
                    return;
                }

                item.Execute(null);
            });

            var cech = new EventHandler((_, _) => control.Enabled = item.CanExecute(null));

            item.CanExecuteChanged += cech;
            control.AddTarget(ev, events);

            control.Enabled = item.CanExecute(null);

            return Scope.Create(
                (control, ev, events, cech, item),
                static state =>
                {
                    state.control.RemoveTarget(state.ev, state.events);
                    state.item.CanExecuteChanged -= state.cech;
                });
        }
    }
}
