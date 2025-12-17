// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Represents any object capable of hosting its own navigation stack via <see cref="RoutingState"/>.
/// </summary>
/// <remarks>
/// <para>
/// Most applications expose a single implementation of <see cref="IScreen"/> (for example a shell or app view model)
/// that owns the global router. Individual view models can accept an <see cref="IScreen"/> via constructor
/// injection so they can request navigation without directly referencing UI types.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// public class AppViewModel : ReactiveObject, IScreen
/// {
///     public RoutingState Router { get; } = new();
///
///     public ReactiveCommand<Unit, IRoutableViewModel> ShowSettings { get; }
///
///     public AppViewModel()
///     {
///         ShowSettings = ReactiveCommand.CreateFromObservable(
///             () => Router.Navigate.Execute(new SettingsViewModel(this)));
///     }
/// }
/// ]]>
/// </code>
/// </example>
public interface IScreen
{
    /// <summary>
    /// Gets the router associated with this screen. The router coordinates navigation requests for
    /// all child view models attached to the screen.
    /// </summary>
    RoutingState Router { get; }
}
