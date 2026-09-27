// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>A window-wide reminder with no bindable state; only its activation lifecycle matters.</summary>
[System.Diagnostics.DebuggerDisplay("OfficeHoursBannerViewModel")]
public sealed class OfficeHoursBannerViewModel : IActivatableViewModel, IDisposable
{
    /// <inheritdoc/>
    public ViewModelActivator Activator { get; } = new();

    /// <inheritdoc/>
    public void Dispose() => Activator.Dispose();
}
