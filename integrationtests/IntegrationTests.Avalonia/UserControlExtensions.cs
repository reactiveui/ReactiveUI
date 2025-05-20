// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Threading.Tasks;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace IntegrationTests.Avalonia;

/// <summary>
/// Extension methods associated with the UserControl class.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class UserControlExtensions
{
    /// <summary>
    /// Shows a message to the user, and have the results wrapped in a observable.
    /// </summary>
    /// <param name="this">The user control that hosts the message box.</param>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to show to the user.</param>
    /// <returns>An observable of the result from the message box.</returns>
    public static IObservable<ButtonResult> ShowMessage(
        this UserControl @this,
        string title,
        string message)
    {
        var window = (Window)TopLevel.GetTopLevel(@this)!;
        var box = MessageBoxManager
            .GetMessageBoxStandard(title, message);
        return box.ShowWindowDialogAsync(window).ToObservable();
    }
}
