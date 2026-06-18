// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Defines the minimum contract for view models that participate in <see cref="RoutingState"/> navigation.</summary>
/// <remarks>
/// <para>
/// Routable view models expose a user-readable <see cref="UrlPathSegment"/> used for diagnostics / navigation breadcrumbs
/// and keep a reference to the owning <see cref="IScreen"/> so that downstream navigation commands can be issued.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// public class SettingsViewModel : ReactiveObject, IRoutableViewModel
/// {
///     public SettingsViewModel(IScreen hostScreen) => HostScreen = hostScreen;
///
///     public string? UrlPathSegment => "settings";
///
///     public IScreen HostScreen { get; }
/// }
/// ]]>
/// </code>
/// </example>
public interface IRoutableViewModel : IReactiveObject
{
    /// <summary>Gets a string token representing the current view model, such as "login" or "user".</summary>
    string? UrlPathSegment { get; }

    /// <summary>
    /// Gets the <see cref="IScreen"/> instance that hosts this view model. Use this reference to access the
    /// shared <see cref="RoutingState"/> when chaining navigation from child view models.
    /// </summary>
    IScreen HostScreen { get; }
}
