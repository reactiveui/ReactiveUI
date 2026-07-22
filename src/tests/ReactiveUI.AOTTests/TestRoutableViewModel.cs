// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.AOT.Tests;

/// <summary>Test routable view model for AOT testing.</summary>
internal sealed class TestRoutableViewModel : ReactiveObject, IRoutableViewModel, IDisposable
{
    /// <inheritdoc/>
    public string? UrlPathSegment { get; } = "test";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = null!;

    /// <summary>Gets the view model activator.</summary>
    internal ViewModelActivator Activator { get; } = new();

    /// <inheritdoc/>
    public void Dispose() => Activator.Dispose();
}
