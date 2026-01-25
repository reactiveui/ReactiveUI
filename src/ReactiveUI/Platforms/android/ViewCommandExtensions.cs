// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

using Android.Views;

using ReactiveUI.Helpers;

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for binding commands to view controls.
/// </summary>
public static class ViewCommandExtensions
{
    /// <summary>
    /// Binds the specified command to the click event of the given view, enabling or disabling the view based on the
    /// command's ability to execute.
    /// </summary>
    /// <remarks>The view's enabled state is automatically updated to reflect whether the command can execute.
    /// Disposing the returned object is required to avoid memory leaks and to properly detach event handlers.</remarks>
    /// <param name="command">The command to bind to the view. Cannot be null.</param>
    /// <param name="control">The view whose click event will trigger the command. Cannot be null.</param>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, detaches the event handlers and unbinds the command from the
    /// view.</returns>
    public static IDisposable BindToTarget(this ICommand command, View control) // TODO: Create Test
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(control);

        var ev = new EventHandler((o, e) =>
        {
            if (!command.CanExecute(null))
            {
                return;
            }

            command.Execute(null);
        });

        var cech = new EventHandler((o, e) => control.Enabled = command.CanExecute(null));

        command.CanExecuteChanged += cech;
        control.Click += ev;

        control.Enabled = command.CanExecute(null);

        return Disposable.Create(() =>
        {
            command.CanExecuteChanged -= cech;
            control.Click -= ev;
        });
    }
}
