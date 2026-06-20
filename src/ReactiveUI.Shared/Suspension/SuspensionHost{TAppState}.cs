// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Default strongly-typed implementation of <see cref="ISuspensionHost{TAppState}"/>.</summary>
/// <typeparam name="TAppState">The application state type.</typeparam>
/// <remarks>
/// <para>
/// This implementation provides settable lifecycle observables and a strongly-typed <see cref="AppStateValue"/>.
/// </para>
/// <para>
/// The legacy <see cref="ISuspensionHost"/> members <see cref="ISuspensionHost.AppState"/> and
/// <see cref="ISuspensionHost.CreateNewAppState"/> are implemented explicitly to preserve compatibility with
/// existing infrastructure. The explicit implementations project to/from the typed properties.
/// </para>
/// <para>
/// Type safety: if a consumer sets <see cref="ISuspensionHost.AppState"/> to a value not assignable to
/// <typeparamref name="TAppState"/>, the implementation throws an <see cref="InvalidCastException"/>.
/// </para>
/// </remarks>
[System.Diagnostics.DebuggerDisplay("AppStateValue = {_appState}")]
public class SuspensionHost<TAppState> : ReactiveObject, ISuspensionHost<TAppState>, IDisposable
{
    /// <summary>Holds the observable that signals when the application is launching new.</summary>
    private readonly ReplaySignal<IObservable<RxVoid>> _isLaunchingNew = new(1);

    /// <summary>Holds the observable that signals when the application is resuming from a suspended state.</summary>
    private readonly ReplaySignal<IObservable<RxVoid>> _isResuming = new(1);

    /// <summary>Holds the observable that signals when the application is activated / unpausing.</summary>
    private readonly ReplaySignal<IObservable<RxVoid>> _isUnpausing = new(1);

    /// <summary>Holds the observable that signals when the application should persist its state.</summary>
    private readonly ReplaySignal<IObservable<IDisposable>> _shouldPersistState = new(1);

    /// <summary>Holds the observable that signals when persisted state should be invalidated.</summary>
    private readonly ReplaySignal<IObservable<RxVoid>> _shouldInvalidateState = new(1);

    /// <summary>Holds the observable that signals when the application is continuing from a temporarily paused state.</summary>
    private readonly ReplaySignal<IObservable<RxVoid>> _isContinuing = new(1);

    /// <summary>Publishes changes to <see cref="AppStateValue"/> when assigned.</summary>
    private readonly Signal<TAppState?> _appStateValueChanged = new();

    /// <summary>Stores the typed application state factory.</summary>
    private Func<TAppState>? _createNewAppStateTyped;

    /// <summary>Stores the current typed application state.</summary>
    private TAppState? _appState;

    /// <summary>Initializes a new instance of the <see cref="SuspensionHost{TAppState}"/> class.</summary>
    /// <remarks>
    /// The default values throw to indicate that platform-specific suspend/resume wiring has not been installed.
    /// Hosts should use <c>AutoSuspendHelper</c> (or an equivalent) to replace these streams.
    /// </remarks>
    public SuspensionHost()
    {
#if COCOA
        const string? Message = "Your AppDelegate class needs to use AutoSuspendHelper";
#elif ANDROID
        const string? Message = "You need to create an App class and use AutoSuspendHelper";
#else
        const string? Message = "Your App class needs to use AutoSuspendHelper";
#endif

        ShouldInvalidateState = Signal.Fail<RxVoid>(new InvalidOperationException(Message));
        IsContinuing = ShouldInvalidateState;
        IsUnpausing = IsContinuing;
        IsResuming = IsUnpausing;
        IsLaunchingNew = IsResuming;

        ShouldPersistState = Signal.Fail<IDisposable>(new InvalidOperationException(Message));
    }

    /// <summary>Gets or sets the observable which signals when the application is launching new.</summary>
    /// <remarks>
    /// Emits when the platform indicates a clean launch (for example, no saved state is available).
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the value is <see langword="null"/>.</exception>
    public IObservable<RxVoid> IsLaunchingNew
    {
        get => new WhenAnyObservableSwitchSink<RxVoid>(_isLaunchingNew);
        set
        {
            ArgumentExceptionHelper.ThrowIfNull(value);
            _isLaunchingNew.OnNext(value);
        }
    }

    /// <summary>Gets or sets the observable which signals when the application is resuming from a suspended state.</summary>
    /// <remarks>
    /// Raised when the host platform reports that the previous process image is being restored.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the value is <see langword="null"/>.</exception>
    public IObservable<RxVoid> IsResuming
    {
        get => new WhenAnyObservableSwitchSink<RxVoid>(_isResuming);
        set
        {
            ArgumentExceptionHelper.ThrowIfNull(value);
            _isResuming.OnNext(value);
        }
    }

    /// <summary>Gets or sets the observable which signals when the application is activated / unpausing.</summary>
    /// <remarks>
    /// Fired when the app returns to the foreground without being recreated.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the value is <see langword="null"/>.</exception>
    public IObservable<RxVoid> IsUnpausing
    {
        get => new WhenAnyObservableSwitchSink<RxVoid>(_isUnpausing);
        set
        {
            ArgumentExceptionHelper.ThrowIfNull(value);
            _isUnpausing.OnNext(value);
        }
    }

    /// <summary>Gets or sets an observable which signals when the application is continuing from a temporarily paused state.</summary>
    /// <remarks>
    /// This member exists to preserve behavior patterns where a host differentiates resume-from-tombstone vs
    /// resume-from-suspend; consumers may ignore it if not applicable.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the value is <see langword="null"/>.</exception>
    public IObservable<RxVoid> IsContinuing
    {
        get => new WhenAnyObservableSwitchSink<RxVoid>(_isContinuing);
        set
        {
            ArgumentExceptionHelper.ThrowIfNull(value);
            _isContinuing.OnNext(value);
        }
    }

    /// <summary>Gets or sets the observable which signals when the application should persist its state to disk.</summary>
    /// <remarks>
    /// The produced <see cref="IDisposable"/> should be disposed once the application finishes persisting its state.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the value is <see langword="null"/>.</exception>
    public IObservable<IDisposable> ShouldPersistState
    {
        get => new WhenAnyObservableSwitchSink<IDisposable>(_shouldPersistState);
        set
        {
            ArgumentExceptionHelper.ThrowIfNull(value);
            _shouldPersistState.OnNext(value);
        }
    }

    /// <summary>Gets or sets the observable which signals that the saved application state should be deleted.</summary>
    /// <remarks>
    /// Triggered when the host detects an unrecoverable failure; use it to delete corrupt state and log crash telemetry.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the value is <see langword="null"/>.</exception>
    public IObservable<RxVoid> ShouldInvalidateState
    {
        get => new WhenAnyObservableSwitchSink<RxVoid>(_shouldInvalidateState);
        set
        {
            ArgumentExceptionHelper.ThrowIfNull(value);
            _shouldInvalidateState.OnNext(value);
        }
    }

    /// <summary>Gets or sets a function that can be used to create a new application state instance.</summary>
    /// <remarks>
    /// This is the typed counterpart to <see cref="ISuspensionHost.CreateNewAppState"/> and is typically used when the
    /// application is launching fresh or recovering from an invalidated state.
    /// </remarks>
    public Func<TAppState>? CreateNewAppStateTyped
    {
        get => _createNewAppStateTyped;
        set => this.RaiseAndSetIfChanged(ref _createNewAppStateTyped, value);
    }

    /// <summary>Gets or sets the current application state.</summary>
    /// <remarks>
    /// This is the typed counterpart to <see cref="ISuspensionHost.AppState"/>.
    /// </remarks>
    public TAppState? AppStateValue
    {
        get => _appState;
        set
        {
            this.RaiseAndSetIfChanged(ref _appState, value);

            _appStateValueChanged.OnNext(value);
        }
    }

    /// <summary>Gets an observable that signals when <see cref="AppStateValue"/> is assigned.</summary>
    /// <remarks>
    /// This is a trimming/AOT-friendly change signal. It is independent of ReactiveUI's WhenAny APIs.
    /// </remarks>
    public IObservable<TAppState?> AppStateValueChanged => _appStateValueChanged;

    /// <summary>Gets or sets a function that can be used to create a new application state instance.</summary>
    /// <remarks>
    /// This is the legacy object-based API. It projects to/from <see cref="CreateNewAppStateTyped"/>.
    /// </remarks>
    /// <exception cref="InvalidCastException">
    /// Thrown when the factory returns a value that is not assignable to <typeparamref name="TAppState"/>.
    /// </exception>
    Func<object>? ISuspensionHost.CreateNewAppState
    {
        get
        {
            var typedFactory = _createNewAppStateTyped;
            return typedFactory is null ? null : () => typedFactory.Invoke()!;
        }

        set
        {
            if (value is null)
            {
                CreateNewAppStateTyped = null;
                return;
            }

            CreateNewAppStateTyped = () =>
            {
                var created = value();
                return created is TAppState typed
                    ? typed
                    : throw new InvalidCastException(
                        $"Created app state is not assignable to {typeof(TAppState).FullName}.");
            };
        }
    }

    /// <summary>Gets or sets the current application state.</summary>
    /// <remarks>
    /// This is the legacy object-based API. It projects to/from the typed <see cref="AppStateValue"/> property.
    /// </remarks>
    /// <exception cref="InvalidCastException">
    /// Thrown when the assigned value is not assignable to <typeparamref name="TAppState"/>.
    /// </exception>
    object? ISuspensionHost.AppState
    {
        get => _appState;
        set
        {
            if (value is null)
            {
                AppStateValue = default;
                return;
            }

            if (value is not TAppState typed)
            {
                throw new InvalidCastException($"AppState is not assignable to {typeof(TAppState).FullName}.");
            }

            AppStateValue = typed;
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases managed resources used by the instance.</summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release managed resources; <see langword="false"/> to release unmanaged resources only.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _isLaunchingNew.Dispose();
        _isResuming.Dispose();
        _isUnpausing.Dispose();
        _isContinuing.Dispose();
        _shouldPersistState.Dispose();
        _shouldInvalidateState.Dispose();

        _appStateValueChanged.Dispose();
    }
}
