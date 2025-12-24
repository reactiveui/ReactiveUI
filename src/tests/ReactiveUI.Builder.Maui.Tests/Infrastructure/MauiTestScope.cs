// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Threading;

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
    private const uint WinAppSdk17 = 0x00010007;
    private const uint WinAppSdk18 = 0x00010008;
#endif

    private static readonly Lock Gate = new();

    private static int _activeScopes;
    private static bool _initializationFailed;
    private static string? _skipReason;

    private static IDispatcherProvider? _originalDispatcherProvider;
    private static bool _originalDispatcherProviderCaptured;

#if WINDOWS
    private static bool _bootstrapInitialized;
    private static DispatcherQueueController? _dispatcherQueueController;
    private static WindowsXamlManager? _windowsXamlManager;
#endif

    private bool _disposed;

    private MauiTestScope()
    {
    }

    /// <summary>
    /// Enters a MAUI test scope for the current test thread.
    /// </summary>
    /// <returns>A disposable scope.</returns>
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

    private static void EnsureMauiApplication()
    {
        if (Application.Current is not null)
        {
            return;
        }

        _ = new TestApplication();
    }

    private static void HandleInitializationFailure(Exception exception)
    {
        _initializationFailed = true;

        var innermost = GetInnermostException(exception);
        _skipReason =
            $"MAUI test environment unavailable ({innermost.GetType().Name}, ProcArch={RuntimeInformation.ProcessArchitecture}): {innermost}";

        throw new SkipTestException(_skipReason);
    }

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
    private static void EnsureStaThread()
    {
        // This is critical for WinUI/XAML event hookup paths that otherwise throw 0x8001010E.
        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        {
            throw new InvalidOperationException(
                "MAUI/WinUI initialization requires an STA test thread. Use TUnit STA test executor for this test/class.");
        }
    }

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

    private static void EnsureDispatcherQueue()
    {
        if (DispatcherQueue.GetForCurrentThread() is not null)
        {
            return;
        }

        _dispatcherQueueController = DispatcherQueueController.CreateOnCurrentThread();
    }

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

    private static class NativeMethods
    {
        [DllImport("ole32.dll")]
        internal static extern int CoInitializeEx(nint pvReserved, uint dwCoInit);
    }
#endif

    private sealed class TestApplication : Application
    {
    }

    private sealed class TestDispatcherProvider : IDispatcherProvider
    {
        private readonly IDispatcher _dispatcher = new TestDispatcher();

        public IDispatcher GetForCurrentThread()
        {
            return _dispatcher;
        }

        public IDispatcher GetMainThreadDispatcher()
        {
            return _dispatcher;
        }
    }

    private sealed class TestDispatcher : IDispatcher
    {
        public bool IsDispatchRequired => false;

        public bool Dispatch(Action action)
        {
            action();
            return true;
        }

        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            action();
            return true;
        }

        public IDispatcherTimer CreateTimer()
        {
            return new TestDispatcherTimer(this);
        }
    }

    private sealed class TestDispatcherTimer : IDispatcherTimer
    {
        private readonly TestDispatcher _dispatcher;

        public TestDispatcherTimer(TestDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public event EventHandler? Tick;

        public TimeSpan Interval { get; set; }

        public bool IsRunning { get; private set; }

        public bool IsRepeating { get; set; }

        public void Start()
        {
            IsRunning = true;
            _dispatcher.Dispatch(() => Tick?.Invoke(this, EventArgs.Empty));

            if (!IsRepeating)
            {
                Stop();
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
