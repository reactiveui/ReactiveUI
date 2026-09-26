// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Routing;

/// <summary>The window of the to-do app. It owns the router, and the page on top of its stack is the one on screen.</summary>
[System.Diagnostics.DebuggerDisplay("Pages = {Router.NavigationStack.Count}")]
public sealed class AppShell : ReactiveObject, IScreen
{
    /// <summary>Initializes a new instance of the <see cref="AppShell"/> class with the default router.</summary>
    public AppShell()
        : this(new RoutingState())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AppShell"/> class with the given router.</summary>
    /// <param name="router">The router to use, such as one built with a specific <see cref="ISequencer"/>.</param>
    public AppShell(RoutingState router) => Router = router;

    /// <inheritdoc/>
    public RoutingState Router { get; }
}
