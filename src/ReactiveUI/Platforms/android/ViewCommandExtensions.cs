// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Android.Views;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides extension methods for binding commands to view controls.</summary>
public static class ViewCommandExtensions
{
    /// <summary>Extends an <see cref="ICommand"/> with helpers for binding it to Android view controls.</summary>
    /// <param name="command">The command being bound to a view. Cannot be null.</param>
    extension(ICommand command)
    {
        /// <summary>
        /// Binds the specified command to the click event of the given view, enabling or disabling the view based on the
        /// command's ability to execute.
        /// </summary>
        /// <remarks>The view's enabled state is automatically updated to reflect whether the command can execute.
        /// Disposing the returned object is required to avoid memory leaks and to properly detach event handlers.</remarks>
        /// <param name="control">The view whose click event will trigger the command. Cannot be null.</param>
        /// <returns>An <see cref="IDisposable"/> that, when disposed, detaches the event handlers and unbinds the command from the
        /// view.</returns>
        public IDisposable BindToTarget(View control)
        {
            ArgumentExceptionHelper.ThrowIfNull(command);
            ArgumentExceptionHelper.ThrowIfNull(control);

            EventHandler ev = (_, _) =>
            {
                if (!command.CanExecute(null))
                {
                    return;
                }

                command.Execute(null);
            };

            EventHandler cech = (_, _) => control.Enabled = command.CanExecute(null);

            command.CanExecuteChanged += cech;
            control.Click += ev;

            control.Enabled = command.CanExecute(null);

            return new ActionDisposable(() =>
            {
                command.CanExecuteChanged -= cech;
                control.Click -= ev;
            });
        }
    }
}
