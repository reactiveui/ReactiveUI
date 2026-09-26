// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>The settings screen's view model.</summary>
[System.Diagnostics.DebuggerDisplay("SettingsViewModel Title = {Title}")]
public sealed class SettingsViewModel
{
    /// <summary>Gets the title the screen shows in its header.</summary>
    public string Title => "Settings";
}
