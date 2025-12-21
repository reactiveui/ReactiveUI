// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// A internal state setup by other classes for the different suspension state of a application.
/// The user does not implement themselves but is often setup via the AutoSuspendHelper class.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="SuspensionHost"/> backs <see cref="RxApp.SuspensionHost"/> and provides concrete observables that are wired up
/// by helpers such as <see cref="AutoSuspendHelper"/>. Platform hosts push their lifecycle notifications into the
/// <c>ReplaySubject</c> instances exposed here and view models subscribe through <see cref="ISuspensionHost"/>.
/// </para>
/// <para>
/// Consumers rarely instantiate this type directly; instead call <c>RxApp.SuspensionHost</c> to access the singleton. The
/// object is intentionally thread-safe via <see cref="ReplaySubject{T}"/> so events raised prior to subscription are
/// replayed to late subscribers.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// var suspensionHost = new SuspensionHost();
/// suspensionHost.IsLaunchingNew = Observable.Return(Unit.Default);
/// suspensionHost.CreateNewAppState = () => new ShellState();
/// suspensionHost.AppState = suspensionHost.CreateNewAppState();
/// ]]>
/// </code>
/// </example>
internal class SuspensionHost : ReactiveObject, ISuspensionHost, IDisposable
{
    private readonly ReplaySubject<IObservable<Unit>> _isLaunchingNew = new(1);

    private readonly ReplaySubject<IObservable<Unit>> _isResuming = new(1);

    private readonly ReplaySubject<IObservable<Unit>> _isUnpausing = new(1);

    private readonly ReplaySubject<IObservable<IDisposable>> _shouldPersistState = new(1);

    private readonly ReplaySubject<IObservable<Unit>> _shouldInvalidateState = new(1);

    private object? _appState;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuspensionHost"/> class.
    /// </summary>
    public SuspensionHost()
    {
#if COCOA
        const string? message = "Your AppDelegate class needs to use AutoSuspendHelper";
#elif ANDROID
        const string? message = "You need to create an App class and use AutoSuspendHelper";
#else
        const string? message = "Your App class needs to use AutoSuspendHelper";
#endif

        IsLaunchingNew = IsResuming = IsUnpausing = ShouldInvalidateState =
            Observable.Throw<Unit>(new Exception(message));

        ShouldPersistState = Observable.Throw<IDisposable>(new Exception(message));
    }

    /// <summary>
    /// Gets or sets a observable which notifies when the application is resuming.
    /// </summary>
    /// <remarks>
    /// Raised when the host platform reports that the previous process image is being restored (for example, when an
    /// Android Activity is recreated with a saved bundle). Use this signal to reload persisted state before showing UI.
    /// </remarks>
    public IObservable<Unit> IsResuming // TODO: Create Test
    {
        get => _isResuming.Switch();
        set => _isResuming.OnNext(value);
    }

    /// <summary>
    /// Gets or sets a observable which notifies when the application is un-pausing.
    /// </summary>
    /// <remarks>
    /// Fired when the app returns to the foreground without being recreated (for example, when an Activity is resumed
    /// after being paused). React to this stream to refresh transient UI state that should not be serialized.
    /// </remarks>
    public IObservable<Unit> IsUnpausing // TODO: Create Test
    {
        get => _isUnpausing.Switch();
        set => _isUnpausing.OnNext(value);
    }

    /// <summary>
    /// Gets or sets a observable which notifies when the application should persist its state.
    /// </summary>
    /// <remarks>
    /// Subscribers should write <see cref="AppState"/> to durable storage and dispose the provided token once the
    /// operation completes so platform helpers can release any background execution grants.
    /// </remarks>
    public IObservable<IDisposable> ShouldPersistState // TODO: Create Test
    {
        get => _shouldPersistState.Switch();
        set => _shouldPersistState.OnNext(value);
    }

    /// <summary>
    /// Gets or sets a observable which notifies when a application is launching new.
    /// </summary>
    /// <remarks>
    /// Emits when the platform indicates a clean launch (for example, no saved Android bundle). Use this to create
    /// default state via <see cref="CreateNewAppState"/> or to initialize services only needed on cold start.
    /// </remarks>
    public IObservable<Unit> IsLaunchingNew // TODO: Create Test
    {
        get => _isLaunchingNew.Switch();
        set => _isLaunchingNew.OnNext(value);
    }

    /// <summary>
    /// Gets or sets a observable which notifies when the application state should be invalidated.
    /// </summary>
    /// <remarks>
    /// Triggered when the host detects an unrecoverable failure (for example, AppDomain unhandled exceptions). Use it to
    /// delete corrupt state and log crash telemetry before the process terminates.
    /// </remarks>
    public IObservable<Unit> ShouldInvalidateState // TODO: Create Test
    {
        get => _shouldInvalidateState.Switch();
        set => _shouldInvalidateState.OnNext(value);
    }

    /// <summary>
    /// Gets or sets a Func which will generate a fresh application state.
    /// </summary>
    /// <remarks>
    /// Invoked whenever persisted state cannot be loaded. Provide a factory that creates the root object backing
    /// <see cref="AppState"/> so cold launches and crash recoveries produce consistent defaults.
    /// </remarks>
    public Func<object>? CreateNewAppState { get; set; }

    /// <summary>
    /// Gets or sets the application state that will be used when suspending and resuming the class.
    /// </summary>
    /// <remarks>
    /// The value should be a serializable aggregate that represents the shell of your application. It is populated via
    /// <see cref="ISuspensionDriver.LoadState"/> during resume and saved through <see cref="ISuspensionDriver.SaveState"/>
    /// when <see cref="ShouldPersistState"/> fires.
    /// </remarks>
    public object? AppState
    {
        get => _appState;
        set => this.RaiseAndSetIfChanged(ref _appState, value);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _isLaunchingNew.Dispose();
            _isResuming.Dispose();
            _isUnpausing.Dispose();
            _shouldPersistState.Dispose();
            _shouldInvalidateState.Dispose();
        }
    }
}
