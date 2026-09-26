// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>
/// A stand-in for an older platform control that predates <see cref="ICanActivate"/>: it only raises plain events
/// when it is shown and hidden, the way some toolkits still do. <see cref="LegacyPanelActivationFetcher"/> adapts it.
/// </summary>
public sealed class LegacyScorePanel : IActivatableView
{
    /// <summary>Raised when the panel is shown.</summary>
    public event EventHandler? Shown;

    /// <summary>Raised when the panel is hidden.</summary>
    public event EventHandler? Hidden;

    /// <summary>Shows the panel, raising <see cref="Shown"/>.</summary>
    public void Show() => Shown?.Invoke(this, EventArgs.Empty);

    /// <summary>Hides the panel, raising <see cref="Hidden"/>.</summary>
    public void Hide() => Hidden?.Invoke(this, EventArgs.Empty);
}
