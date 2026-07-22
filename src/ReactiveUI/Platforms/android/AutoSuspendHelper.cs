// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Internal;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Helps manage android application lifecycle events.</summary>
/// <remarks>
/// <para>
/// Register this helper inside your <see cref="Application"/> subclass to translate Activity lifecycle callbacks into
/// <see cref="RxSuspension.SuspensionHost"/> signals. The helper automatically distinguishes cold starts from restores by
/// inspecting <see cref="LatestBundle"/> and routes pause/save events to <see cref="ISuspensionDriver"/> via
/// <see cref="SuspensionHostExtensions.SetupDefaultSuspendResume(ISuspensionHost, ISuspensionDriver?)"/>.
/// </para>
/// <para>
/// Example usage:
/// <code language="csharp">
/// <![CDATA[
/// [Application]
/// public class App : Application
/// {
///     private AutoSuspendHelper? _autoSuspendHelper;
///     public App(IntPtr handle, JniHandleOwnership ownership)
///         : base(handle, ownership)
///     {
///     }
///     public override void OnCreate()
///     {
///         base.OnCreate();
///         _autoSuspendHelper = new AutoSuspendHelper(this);
///         RxSuspension.SuspensionHost.CreateNewAppState = () => new ShellState();
///         RxSuspension.SuspensionHost.SetupDefaultSuspendResume(new FileSuspensionDriver(FilesDir!.AbsolutePath));
///     }
/// }
/// ]]>
/// </code>
/// </para>
/// </remarks>
public class AutoSuspendHelper : IEnableLogger, IDisposable
{
    /// <summary>Relays Activity create callbacks along with the saved-state bundle.</summary>
    private readonly Signal<Bundle?> _onCreate = new();

    /// <summary>Relays Activity resume callbacks.</summary>
    private readonly Signal<RxVoid> _onRestart = new();

    /// <summary>Relays Activity pause callbacks.</summary>
    private readonly Signal<RxVoid> _onPause = new();

    /// <summary>Relays Activity save-instance-state callbacks along with the out bundle.</summary>
    private readonly Signal<Bundle?> _onSaveInstanceState = new();

    /// <summary>Tracks whether this instance has already been disposed.</summary>
    private bool _disposedValue;

    /// <summary>Initializes static members of the <see cref="AutoSuspendHelper"/> class.</summary>
    static AutoSuspendHelper() => AppDomain.CurrentDomain.UnhandledException +=
        static (_, _) => UntimelyDemise.OnNext(RxVoid.Default);

    /// <summary>Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.</summary>
    /// <param name="hostApplication">The host application.</param>
    public AutoSuspendHelper(Application hostApplication)
    {
        ArgumentExceptionHelper.ThrowIfNull(hostApplication);
        hostApplication.RegisterActivityLifecycleCallbacks(
            new ObservableLifecycle(_onCreate, _onRestart, _onPause, _onSaveInstanceState));

        // Both create and save-instance-state callbacks update the latest bundle (replaces a Merge + Subscribe).
        _ = _onCreate.Subscribe(new DelegateObserver<Bundle?>(static x => LatestBundle = x));
        _ = _onSaveInstanceState.Subscribe(new DelegateObserver<Bundle?>(static x => LatestBundle = x));

        RxSuspension.SuspensionHost.IsLaunchingNew = new CreateSignalObservable(_onCreate, emitWhenNull: true);
        RxSuspension.SuspensionHost.IsResuming = new CreateSignalObservable(_onCreate, emitWhenNull: false);
        RxSuspension.SuspensionHost.IsUnpausing = _onRestart;
        RxSuspension.SuspensionHost.ShouldPersistState = new PersistSignalObservable(_onPause);
        RxSuspension.SuspensionHost.ShouldInvalidateState = UntimelyDemise;
    }

    /// <summary>Gets a subject to indicate whether the application has untimely dismissed.</summary>
    public static ISignal<RxVoid> UntimelyDemise { get; } = new Signal<RxVoid>();

    /// <summary>Gets or sets the latest bundle.</summary>
    /// <remarks>
    /// Updated whenever <see cref="Activity.OnSaveInstanceState(Bundle)"/> runs so callers can detect whether
    /// <see cref="ObservableLifecycle.OnActivityCreated(Activity?, Bundle?)"/> represents a cold launch (<see langword="null"/>) or a
    /// recreation with persisted state.
    /// </remarks>
    public static Bundle? LatestBundle { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Disposes of the items inside the class.</summary>
    /// <param name="disposing">If we are disposing of managed objects or not.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _onCreate.Dispose();
            _onPause.Dispose();
            _onRestart.Dispose();
            _onSaveInstanceState.Dispose();
        }

        _disposedValue = true;
    }

    /// <summary>
    /// Emits <see cref="RxVoid.Default"/> for each create callback whose saved-state bundle matches the requested
    /// null-ness — replacing <c>_onCreate.Where(x =&gt; x is null/not null).Select(_ =&gt; RxVoid.Default)</c>.
    /// </summary>
    /// <param name="source">The create-callback stream carrying the saved-state bundle.</param>
    /// <param name="emitWhenNull">When true, emits for null bundles (cold launch); when false, for non-null bundles (resume).</param>
    private sealed class CreateSignalObservable(IObservable<Bundle?> source, bool emitWhenNull) : IObservable<RxVoid>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<RxVoid> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer, emitWhenNull));
        }

        /// <summary>Emits <see cref="RxVoid.Default"/> for each create callback whose saved-state bundle matches the requested null-ness.</summary>
        /// <param name="downstream">The observer to receive the <see cref="RxVoid.Default"/> notifications.</param>
        /// <param name="emitWhenNull">When true, emits for null bundles (cold launch); when false, for non-null bundles (resume).</param>
        private sealed class Sink(IObserver<RxVoid> downstream, bool emitWhenNull) : IObserver<Bundle?>
        {
            /// <inheritdoc/>
            public void OnNext(Bundle? value)
            {
                if ((value is null) != emitWhenNull)
                {
                    return;
                }

                downstream.OnNext(RxVoid.Default);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>Emits an empty disposable for each pause callback — replacing <c>_onPause.Select(_ =&gt; Disposable.Empty)</c> to feed <see cref="ISuspensionHost.ShouldPersistState"/>.</summary>
    /// <param name="source">The pause-callback stream.</param>
    private sealed class PersistSignalObservable(IObservable<RxVoid> source) : IObservable<IDisposable>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IDisposable> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer));
        }

        /// <summary>Emits an empty disposable for each pause callback.</summary>
        /// <param name="downstream">The downstream observer.</param>
        private sealed class Sink(IObserver<IDisposable> downstream) : IObserver<RxVoid>
        {
            /// <inheritdoc/>
            public void OnNext(RxVoid value) => downstream.OnNext(EmptyDisposable.Instance);

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>
    /// Handles Android activity lifecycle events and forwards them to the associated AutoSuspendHelper instance for
    /// reactive processing.
    /// </summary>
    /// <remarks>This class implements the Application.IActivityLifecycleCallbacks interface to observe
    /// activity lifecycle changes. It is intended for internal use to bridge Android lifecycle events to reactive
    /// streams managed by AutoSuspendHelper.</remarks>
    /// <param name="onCreate">Relays Activity create callbacks along with the saved-state bundle.</param>
    /// <param name="onRestart">Relays Activity resume callbacks.</param>
    /// <param name="onPause">Relays Activity pause callbacks.</param>
    /// <param name="onSaveInstanceState">Relays Activity save-instance-state callbacks along with the out bundle.</param>
    private sealed class ObservableLifecycle(
        Signal<Bundle?> onCreate,
        Signal<RxVoid> onRestart,
        Signal<RxVoid> onPause,
        Signal<Bundle?> onSaveInstanceState)
        : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        /// <inheritdoc/>
        public void OnActivityCreated(Activity? activity, Bundle? savedInstanceState) =>
            onCreate.OnNext(savedInstanceState);

        /// <inheritdoc/>
        public void OnActivityResumed(Activity? activity) => onRestart.OnNext(RxVoid.Default);

        /// <inheritdoc/>
        public void OnActivitySaveInstanceState(Activity? activity, Bundle? outState)
        {
            outState?.PutString("___dummy_value_please_create_a_bundle", "VeryYes");

            onSaveInstanceState.OnNext(outState);
        }

        /// <inheritdoc/>
        public void OnActivityPaused(Activity? activity) => onPause.OnNext(RxVoid.Default);

        /// <inheritdoc/>
        public void OnActivityDestroyed(Activity? activity)
        {
        }

        /// <inheritdoc/>
        public void OnActivityStarted(Activity? activity)
        {
        }

        /// <inheritdoc/>
        public void OnActivityStopped(Activity? activity)
        {
        }
    }
}
