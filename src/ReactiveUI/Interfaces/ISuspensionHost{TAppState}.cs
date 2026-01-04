// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Interfaces;

/// <summary>
/// Represents a standardized version of host operating system lifecycle signals with a strongly-typed application state.
/// </summary>
/// <typeparam name="TAppState">The application state type.</typeparam>
/// <remarks>
/// <para>
/// This interface is a strongly-typed companion to <see cref="ISuspensionHost"/>. It remains platform-agnostic and
/// retains the same lifecycle observables, while providing a typed <see cref="AppStateValue"/> surface.
/// </para>
/// <para>
/// Compatibility: this interface derives from <see cref="ISuspensionHost"/>. Implementations should typically
/// implement <see cref="ISuspensionHost.AppState"/> explicitly to project the typed state through the legacy object-based
/// contract.
/// </para>
/// </remarks>
public interface ISuspensionHost<TAppState> : ISuspensionHost
{
    /// <summary>
    /// Gets or sets a function that can be used to create a new application state instance.
    /// </summary>
    /// <remarks>
    /// This is the typed counterpart to <see cref="ISuspensionHost.CreateNewAppState"/> and is typically used when
    /// the application is launching fresh or recovering from an invalidated state.
    /// </remarks>
    Func<TAppState>? CreateNewAppStateTyped { get; set; }

    /// <summary>
    /// Gets or sets the current application state.
    /// </summary>
    /// <remarks>
    /// This is the typed counterpart to <see cref="ISuspensionHost.AppState"/>. Implementations should ensure that
    /// the legacy <see cref="ISuspensionHost.AppState"/> view remains consistent with this property.
    /// </remarks>
    TAppState? AppStateValue { get; set; }

    /// <summary>
    /// Gets an observable that signals when <see cref="AppStateValue"/> is assigned.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a trimming/AOT-friendly change signal for app state updates.
    /// Consumers can use this to observe state transitions without relying on ReactiveUI
    /// property-change expression pipelines.
    /// </para>
    /// <para>
    /// The observable does not guarantee replay; consumers that need the current value should combine this with
    /// <see cref="AppStateValue"/> (or use an extension that emits the current value first).
    /// </para>
    /// </remarks>
    IObservable<TAppState?> AppStateValueChanged { get; }
}
