// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.RoutingDynamicData;

/// <summary>The window of a small shop app. It owns the router, and the page on top of its stack is the one on screen.</summary>
[System.Diagnostics.DebuggerDisplay("Pages = {Router.NavigationStack.Count}")]
public sealed class AppShell : ReactiveObject, IScreen
{
    /// <summary>Initializes a new instance of the <see cref="AppShell"/> class.</summary>
    public AppShell() => Router = new RoutingState();

    /// <inheritdoc/>
    public RoutingState Router { get; }
}
