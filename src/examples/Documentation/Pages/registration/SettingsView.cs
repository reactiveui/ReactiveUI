// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>The settings screen, shown once at a time and so registered as a singleton view.</summary>
[System.Diagnostics.DebuggerDisplay("SettingsView ViewModel = {ViewModel}")]
public sealed class SettingsView : IViewFor<SettingsViewModel>
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
