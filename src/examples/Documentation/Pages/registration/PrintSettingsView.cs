// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>A plain-text rendering of the settings screen, shown under the "print" contract.</summary>
[System.Diagnostics.DebuggerDisplay("PrintSettingsView ViewModel = {ViewModel}")]
public sealed class PrintSettingsView : IViewFor<SettingsViewModel>
{
    /// <inheritdoc/>
    public SettingsViewModel? ViewModel { get; set; }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (SettingsViewModel?)value;
    }
}
