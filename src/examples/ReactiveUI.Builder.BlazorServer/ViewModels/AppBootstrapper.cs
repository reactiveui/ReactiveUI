// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.BlazorServer.ViewModels;

/// <summary>The root screen that hosts the router and bootstraps navigation for a Blazor circuit.</summary>
/// <seealso cref="ReactiveObject" />
/// <seealso cref="IScreen" />
public class AppBootstrapper : ReactiveObject, IScreen
{
    /// <summary>Initializes a new instance of the <see cref="AppBootstrapper"/> class.</summary>
    public AppBootstrapper()
    {
        CircuitId = Guid.NewGuid();

        Router = new();

        _ = Router.Navigate.Execute(new LobbyViewModel(this)).Subscribe(Witness.Create<IRoutableViewModel>(static _ => { }));
    }

    /// <summary>Gets the unique identifier for the Blazor circuit tab associated with this instance.</summary>
    public Guid CircuitId { get; }

    /// <summary>Gets the Router associated with this Screen.</summary>
    public RoutingState Router { get; }
}
