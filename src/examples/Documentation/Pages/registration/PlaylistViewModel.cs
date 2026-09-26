// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>The playlist screen's view model.</summary>
[System.Diagnostics.DebuggerDisplay("PlaylistViewModel Title = {Title}")]
public sealed class PlaylistViewModel
{
    /// <summary>Gets the title the screen shows in its header.</summary>
    public string Title => "Now Playing";
}
