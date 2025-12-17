// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;
/* Nicked from http://caliburnmicro.codeplex.com/wikipage?title=Working%20with%20Windows%20Phone%207%20v1.1
 *
 * Launching - Occurs when a fresh instance of the application is launching.
 * Activated - Occurs when a previously paused/tombstoned app is resumed/resurrected.
 * Deactivated - Occurs when the application is being paused or tombstoned.
 * Closing - Occurs when the application is closing.
 * Continuing - Occurs when the app is continuing from a temporarily paused state.
 * Continued - Occurs after the app has continued from a temporarily paused state.
 * Resurrecting - Occurs when the app is "resurrecting" from a tombstoned state.
 * Resurrected - Occurs after the app has "resurrected" from a tombstoned state.
*/

/// <summary>
/// ISuspensionHost represents a standardized version of the events that the
/// host operating system publishes. Subscribe to these events in order to
/// handle app suspend / resume.
/// </summary>
/// <remarks>
/// <para>
/// These observables abstract platform terms such as "Launching", "Activated", and "Closing" into a
/// consistent API so shared code can persist state without branching on specific UI stacks. Most
/// applications call <c>RxApp.SuspensionHost.SetupDefaultSuspendResume()</c> during startup to wire
/// default handlers, but the properties are public so advanced hosts can plug in their own monitoring.
/// </para>
/// <para>
/// <see cref="AppState"/> represents the serialized model describing the last running session, while
/// <see cref="CreateNewAppState"/> can be configured to hydrate a fresh instance when a crash or first
/// launch occurs.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// var suspensionHost = RxApp.SuspensionHost;
/// suspensionHost.CreateNewAppState = () => new ShellState();
///
/// suspensionHost.IsLaunchingNew.Subscribe(_ =>
/// {
///     suspensionHost.AppState = suspensionHost.CreateNewAppState!();
/// });
///
/// suspensionHost.ShouldPersistState.Subscribe(disposable =>
/// {
///     storageService.Save((ShellState)suspensionHost.AppState!);
///     disposable.Dispose();
/// });
/// ]]>
/// </code>
/// </example>
public interface ISuspensionHost : IReactiveObject
{
    /// <summary>
    /// Gets or sets the observable which signals when the application is launching new. This can happen when
    /// an app has recently crashed, as well as the first time the app has
    /// been launched. Apps should create their state from scratch.
    /// </summary>
    IObservable<Unit> IsLaunchingNew { get; set; }

    /// <summary>
    /// Gets or sets the observable which signals when the application is resuming from suspended state (i.e.
    /// it was previously running but its process was destroyed).
    /// </summary>
    IObservable<Unit> IsResuming { get; set; }

    /// <summary>
    /// Gets or sets the observable which signals when the application is activated. Note that this may mean
    /// that your process was not actively running before this signal.
    /// </summary>
    IObservable<Unit> IsUnpausing { get; set; }

    /// <summary>
    /// Gets or sets the observable which signals when the application should persist its state to disk.
    /// </summary>
    /// <value>Returns an IDisposable that should be disposed once the
    /// application finishes persisting its state.</value>
    IObservable<IDisposable> ShouldPersistState { get; set; }

    /// <summary>
    /// Gets or sets the observable which signals that the saved application state should be deleted, this
    /// usually is called after an app has crashed.
    /// </summary>
    IObservable<Unit> ShouldInvalidateState { get; set; }

    /// <summary>
    /// Gets or sets a function that can be used to create a new application state - usually
    /// this method just calls 'new' on an object.
    /// </summary>
    Func<object>? CreateNewAppState { get; set; }

    /// <summary>
    /// Gets or sets the current application state - get a typed version of this via
    /// <see cref="SuspensionHostExtensions.GetAppState{T}(ISuspensionHost)"/>.
    /// The "application state" is a notion entirely defined
    /// via the client application - the framework places no restrictions on
    /// the object other than it can be serialized.
    /// </summary>
    object? AppState { get; set; }
}
