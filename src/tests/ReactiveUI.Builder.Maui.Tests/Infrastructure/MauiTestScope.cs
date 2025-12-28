// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

using TUnit.Core.Exceptions;

#if WINDOWS
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
#endif

namespace ReactiveUI.Builder.Maui.Tests;

/// <summary>
/// Provides per-test (or per-class) MAUI/WASDK initialization for unit tests.
/// Intended to be used with TUnit's STA test execution support on Windows.
/// </summary>
public sealed class MauiTestScope : IDisposable
{
#if WINDOWS
    /// <summary>
    /// Version constant for Windows App SDK 1.7.
    /// </summary>
    private const uint WinAppSdk17 = 0x00010007;

    /// <summary>
    /// Version constant for Windows App SDK 1.8.
    /// </summary>
    private const uint WinAppSdk18 = 0x00010008;
#endif

    /// <summary>
    /// Lock object to synchronize initialization and scope tracking across threads.
    /// </summary>
    private static readonly Lock Gate = new();

    /// <summary>
    /// Tracks the number of currently active test scopes to coordinate cleanup.
    /// </summary>
    private static int _activeScopes;

    /// <summary>
    /// Indicates whether MAUI initialization has permanently failed for this test run.
    /// </summary>
    private static bool _initializationFailed;

    /// <summary>
    /// Contains the reason why initialization failed, used in skip messages.
    /// </summary>
    private static string? _skipReason;

    /// <summary>
    /// Stores the original dispatcher provider to restore after test scope cleanup.
    /// </summary>
    private static IDispatcherProvider? _originalDispatcherProvider;

    /// <summary>
    /// Indicates whether the original dispatcher provider has been captured.
    /// </summary>
    private static bool _originalDispatcherProviderCaptured;

#if WINDOWS
    /// <summary>
    /// Indicates whether the Windows App SDK bootstrap has been initialized.
    /// </summary>
    private static bool _bootstrapInitialized;

    /// <summary>
    /// The dispatcher queue controller for the Windows UI thread.
    /// </summary>
    private static DispatcherQueueController? _dispatcherQueueController;

    /// <summary>
    /// The Windows XAML manager instance for the current thread.
    /// </summary>
    private static WindowsXamlManager? _windowsXamlManager;
#endif

    /// <summary>
    /// Indicates whether this scope instance has been disposed.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MauiTestScope"/> class.
    /// </summary>
    /// <remarks>
    /// Private constructor prevents direct instantiation. Use <see cref="Enter"/> to create a scope.
    /// </remarks>
    private MauiTestScope()
    {
    }

    /// <summary>
    /// Enters a MAUI test scope for the current test thread.
    /// </summary>
    /// <returns>A disposable scope that manages MAUI infrastructure for the duration of the test.</returns>
    /// <exception cref="SkipTestException">
    /// Thrown if MAUI initialization fails, causing the test to be skipped.
    /// </exception>
    /// <remarks>
    /// On Windows, this initializes COM, Windows App SDK, DispatcherQueue, and Windows XAML infrastructure.
    /// On all platforms, it creates a MAUI Application and configures a test dispatcher.
    /// </remarks>
    public static MauiTestScope Enter()
    {
        EnsureInitializedForCurrentThread();
        return new MauiTestScope();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        lock (Gate)
        {
            if (_activeScopes > 0)
            {
                _activeScopes--;
            }

            if (_activeScopes != 0)
            {
                return;
            }

            TeardownWhenLastScopeExits();
        }
    }

    /// <summary>
    /// Ensures MAUI infrastructure is initialized for the current test thread.
    /// </summary>
    /// <exception cref="SkipTestException">
    /// Thrown if initialization has previously failed or fails during this call.
    /// </exception>
    /// <remarks>
    /// This method is thread-safe and tracks active scopes. On first initialization,
    /// it sets up platform-specific infrastructure and creates a MAUI Application.
    /// Subsequent calls while scopes are active simply increment the scope counter.
    /// </remarks>
    private static void EnsureInitializedForCurrentThread()
    {
        lock (Gate)
        {
            if (_initializationFailed)
            {
                throw new SkipTestException(_skipReason ?? "MAUI test environment unavailable on this machine.");
            }

            if (_activeScopes > 0)
            {
                _activeScopes++;
                return;
            }

            if (!_originalDispatcherProviderCaptured)
            {
                _originalDispatcherProvider = DispatcherProvider.Current;
                _originalDispatcherProviderCaptured = true;
            }

            try
            {
#if WINDOWS
                ////EnsureStaThread();
                EnsureComInitializedForCurrentThread();
                EnsureWindowsAppSdk();
                EnsureDispatcherQueue();
                EnsureWindowsXaml();
#endif

                EnsureMauiApplication();
                DispatcherProvider.SetCurrent(new TestDispatcherProvider());

                _activeScopes = 1;
            }
            catch (Exception ex)
            {
                HandleInitializationFailure(ex);
            }
        }
    }

    /// <summary>
    /// Cleans up test infrastructure when the last active scope is disposed.
    /// </summary>
    /// <remarks>
    /// Restores the original dispatcher provider but deliberately avoids shutting down
    /// Windows App SDK, DispatcherQueue, or WindowsXamlManager to prevent flakiness
    /// when tests run sequentially in the same process.
    /// </remarks>
    private static void TeardownWhenLastScopeExits()
    {
        // Restore MAUI dispatcher provider (this is test-owned global state).
        if (_originalDispatcherProviderCaptured && _originalDispatcherProvider is not null)
        {
            DispatcherProvider.SetCurrent(_originalDispatcherProvider);
        }

        // Do NOT shutdown WinAppSDK bootstrap or dispatcher queue infrastructure here.
        // Those are process-/thread-affine and shutting them down causes flakiness across
        // reruns within the same testhost process.
        //
        // Also do NOT dispose WindowsXamlManager here. It is thread-affine to the UI thread.
        // Disposing on a different thread can be harmful.
        _initializationFailed = false;
        _skipReason = null;
    }

    /// <summary>
    /// Creates a MAUI Application instance if one doesn't already exist.
    /// </summary>
    /// <remarks>
    /// Uses a minimal <see cref="TestApplication"/> implementation that provides
    /// the bare minimum MAUI application infrastructure needed for tests.
    /// </remarks>
    private static void EnsureMauiApplication()
    {
        if (Application.Current is not null)
        {
            return;
        }

        _ = new TestApplication();
    }

    /// <summary>
    /// Marks initialization as failed and throws a test skip exception with diagnostic information.
    /// </summary>
    /// <param name="exception">The exception that caused initialization to fail.</param>
    /// <exception cref="SkipTestException">Always thrown with details about the failure.</exception>
    private static void HandleInitializationFailure(Exception exception)
    {
        _initializationFailed = true;

        var innermost = GetInnermostException(exception);
        _skipReason =
            $"MAUI test environment unavailable ({innermost.GetType().Name}, ProcArch={RuntimeInformation.ProcessArchitecture}): {innermost}";

        throw new SkipTestException(_skipReason);
    }

    /// <summary>
    /// Extracts the innermost exception from an exception chain.
    /// </summary>
    /// <param name="exception">The exception to unwrap.</param>
    /// <returns>The innermost exception in the chain.</returns>
    private static Exception GetInnermostException(Exception exception)
    {
        var current = exception;
        while (current.InnerException is not null)
        {
            current = current.InnerException;
        }

        return current;
    }

#if WINDOWS
    /// <summary>
    /// Verifies that the current thread is configured as a Single-Threaded Apartment (STA).
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the current thread is not in STA mode.
    /// </exception>
    /// <remarks>
    /// STA is required for WinUI/XAML event hookup paths. Without it, COM calls fail with 0x8001010E.
    /// </remarks>
    private static void EnsureStaThread()
    {
        // This is critical for WinUI/XAML event hookup paths that otherwise throw 0x8001010E.
        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        {
            throw new InvalidOperationException(
                "MAUI/WinUI initialization requires an STA test thread. Use TUnit STA test executor for this test/class.");
        }
    }

    /// <summary>
    /// Initializes COM for the current thread using the STA apartment model.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if COM initialization fails with an unexpected HRESULT.
    /// </exception>
    /// <remarks>
    /// This method tolerates situations where COM is already initialized, even with a different
    /// apartment model, as some test runners initialize COM before our code runs.
    /// </remarks>
    private static void EnsureComInitializedForCurrentThread()
    {
        // The runner may have already initialized COM on this thread (possibly with a different apartment model).
        // CoInitializeEx will return:
        //  - S_OK (0) / S_FALSE (1): COM is initialized for this thread.
        //  - RPC_E_CHANGED_MODE (0x80010106): COM is initialized, but with a different apartment model.
        //    This is still "initialized", and we should continue.
        const uint coinitApartmentThreaded = 0x2; // STA
        const int sOk = 0;
        const int sFalse = 1;
        const int rpcEChangedMode = unchecked((int)0x80010106);

        var hr = NativeMethods.CoInitializeEx(0, coinitApartmentThreaded);

        if (hr == sOk || hr == sFalse || hr == rpcEChangedMode)
        {
            return;
        }

        throw new InvalidOperationException($"Failed to initialize COM on test thread. HRESULT: 0x{hr:X8}");
    }

    /// <summary>
    /// Initializes the Windows App SDK bootstrap for the test process.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if neither version 1.8 nor 1.7 of the Windows App SDK can be initialized.
    /// </exception>
    /// <remarks>
    /// Attempts to initialize version 1.8 first, falling back to 1.7 if unavailable.
    /// This initialization is process-wide and only performed once.
    /// </remarks>
    private static void EnsureWindowsAppSdk()
    {
        if (_bootstrapInitialized)
        {
            return;
        }

        // Prefer 1.8; fall back to 1.7.
        if (Bootstrap.TryInitialize(WinAppSdk18, out _))
        {
            _bootstrapInitialized = true;
            return;
        }

        if (Bootstrap.TryInitialize(WinAppSdk17, out var hresult))
        {
            _bootstrapInitialized = true;
            return;
        }

        throw new InvalidOperationException(
            $"Failed to bootstrap Windows App SDK (1.8 and 1.7). HRESULT: 0x{hresult:X8}");
    }

    /// <summary>
    /// Creates a DispatcherQueue for the current thread if one doesn't already exist.
    /// </summary>
    /// <remarks>
    /// The DispatcherQueue is required for MAUI's Windows scheduler implementation
    /// and must be created on the same thread that will process UI messages.
    /// </remarks>
    private static void EnsureDispatcherQueue()
    {
        if (DispatcherQueue.GetForCurrentThread() is not null)
        {
            return;
        }

        _dispatcherQueueController = DispatcherQueueController.CreateOnCurrentThread();
    }

    /// <summary>
    /// Initializes the Windows XAML framework for the current thread.
    /// </summary>
    /// <remarks>
    /// This prevents COMException 0x8001010E when MAUI's ViewHandler static constructor
    /// subscribes to FocusManager events. Failures are silently ignored because some
    /// environments may already have XAML initialized or may not support it.
    /// </remarks>
    private static void EnsureWindowsXaml()
    {
        // This is the practical mitigation for the FocusManager event subscription path
        // throwing COMException 0x8001010E when MAUI's ViewHandler static ctor runs.
        // If XAML is already initialized, InitializeForCurrentThread() will throw; ignore that.
        if (_windowsXamlManager is not null)
        {
            return;
        }

        try
        {
            _windowsXamlManager = WindowsXamlManager.InitializeForCurrentThread();
        }
        catch
        {
            // Intentionally ignore. Some environments may already have XAML initialized or
            // may not support initializing XAML here; MAUI may still work, and if it doesn't,
            // we will surface the real failure and skip the test.
        }
    }

    /// <summary>
    /// Contains P/Invoke declarations for native Windows APIs.
    /// </summary>
    private static class NativeMethods
    {
        /// <summary>
        /// Initializes the COM library for use by the calling thread.
        /// </summary>
        /// <param name="pvReserved">Reserved; must be zero.</param>
        /// <param name="dwCoInit">The concurrency model and initialization options for the thread.</param>
        /// <returns>An HRESULT indicating success or the type of failure.</returns>
        [DllImport("ole32.dll")]
        internal static extern int CoInitializeEx(nint pvReserved, uint dwCoInit);
    }
#endif

    /// <summary>
    /// Minimal MAUI application implementation for test scenarios.
    /// </summary>
    /// <remarks>
    /// Provides the bare minimum infrastructure required by MAUI's Application.Current singleton.
    /// </remarks>
    private sealed class TestApplication : Application
    {
    }

    /// <summary>
    /// Test implementation of <see cref="IDispatcherProvider"/> that returns a synchronous dispatcher.
    /// </summary>
    /// <remarks>
    /// Used to override MAUI's default dispatcher provider during tests, ensuring all dispatcher
    /// calls execute synchronously on the current thread for predictable test behavior.
    /// </remarks>
    private sealed class TestDispatcherProvider : IDispatcherProvider
    {
        private readonly IDispatcher _dispatcher = new TestDispatcher();

        /// <summary>
        /// Gets the dispatcher for the current thread.
        /// </summary>
        /// <returns>A synchronous test dispatcher.</returns>
        public IDispatcher GetForCurrentThread()
        {
            return _dispatcher;
        }

        /// <summary>
        /// Gets the main thread dispatcher.
        /// </summary>
        /// <returns>A synchronous test dispatcher.</returns>
        public IDispatcher GetMainThreadDispatcher()
        {
            return _dispatcher;
        }
    }

    /// <summary>
    /// Test implementation of <see cref="IDispatcher"/> that executes all work synchronously.
    /// </summary>
    /// <remarks>
    /// Never requires dispatching and executes all actions immediately on the calling thread,
    /// making tests deterministic and avoiding threading complexities.
    /// </remarks>
    private sealed class TestDispatcher : IDispatcher
    {
        /// <summary>
        /// Gets a value indicating whether dispatching is required.
        /// </summary>
        /// <remarks>
        /// Always returns <see langword="false"/> since this dispatcher executes synchronously.
        /// </remarks>
        public bool IsDispatchRequired => false;

        /// <summary>
        /// Dispatches an action to execute immediately on the current thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public bool Dispatch(Action action)
        {
            action();
            return true;
        }

        /// <summary>
        /// Dispatches an action to execute immediately, ignoring the specified delay.
        /// </summary>
        /// <param name="delay">The delay to ignore (executed immediately).</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            action();
            return true;
        }

        /// <summary>
        /// Creates a test dispatcher timer.
        /// </summary>
        /// <returns>A new <see cref="TestDispatcherTimer"/> instance.</returns>
        public IDispatcherTimer CreateTimer()
        {
            return new TestDispatcherTimer(this);
        }
    }

    /// <summary>
    /// Test implementation of <see cref="IDispatcherTimer"/> that fires immediately when started.
    /// </summary>
    /// <remarks>
    /// Ignores the interval and fires the Tick event synchronously when <see cref="Start"/> is called,
    /// making timer-based tests execute immediately without waiting.
    /// </remarks>
    private sealed class TestDispatcherTimer : IDispatcherTimer
    {
        private readonly TestDispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDispatcherTimer"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that will execute the timer callback.</param>
        public TestDispatcherTimer(TestDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Occurs when the timer interval has elapsed.
        /// </summary>
        public event EventHandler? Tick;

        /// <summary>
        /// Gets or sets the interval between timer ticks.
        /// </summary>
        /// <remarks>
        /// This value is ignored; the timer fires immediately when started.
        /// </remarks>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Gets a value indicating whether the timer is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer should fire repeatedly.
        /// </summary>
        /// <remarks>
        /// If <see langword="false"/>, the timer stops automatically after firing once.
        /// </remarks>
        public bool IsRepeating { get; set; }

        /// <summary>
        /// Starts the timer and immediately fires the Tick event.
        /// </summary>
        /// <remarks>
        /// If <see cref="IsRepeating"/> is <see langword="false"/>, the timer stops after firing.
        /// </remarks>
        public void Start()
        {
            IsRunning = true;
            _dispatcher.Dispatch(() => Tick?.Invoke(this, EventArgs.Empty));

            if (!IsRepeating)
            {
                Stop();
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
        }
    }
}
