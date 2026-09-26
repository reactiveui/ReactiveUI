// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// A notice from the school office. It comes from the office's shared library, which registers its view,
/// <see cref="OfficeNoticeView"/>, with the service locator only.
/// </summary>
/// <param name="hostScreen">The notice board the notice is pinned to.</param>
/// <param name="message">The notice text.</param>
[System.Diagnostics.DebuggerDisplay("{Message}")]
public sealed class OfficeNoticeViewModel(IScreen hostScreen, string message) : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string UrlPathSegment => "notice";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = hostScreen;

    /// <summary>Gets the notice text.</summary>
    public string Message { get; } = message;
}
