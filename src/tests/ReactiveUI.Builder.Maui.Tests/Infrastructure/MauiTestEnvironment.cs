// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
#if WINDOWS
using Microsoft.UI.Dispatching;
using Microsoft.Windows.ApplicationModel.DynamicDependency;

using PackageVersion = Microsoft.Windows.ApplicationModel.DynamicDependency.PackageVersion;
#endif

namespace ReactiveUI.Builder.Maui.Tests;

/// <summary>
/// Provides global MAUI/WASDK initialization for unit tests.
/// </summary>
[SetUpFixture]
public sealed class MauiTestEnvironment
{
#if WINDOWS
    private const uint WindowsAppSdkMajorMinorVersion = 0x00010008; // 1.8
    private static bool _bootstrapInitialized;
    private static DispatcherQueueController? _dispatcherQueueController;
#endif
    private static bool _initialized;
    private static bool _initializationFailed;
    private static string? _skipReason;

    [OneTimeSetUp]
    public void Initialize() => EnsureInitialized();

    [OneTimeTearDown]
    public void Shutdown()
    {
#if WINDOWS
        if (_bootstrapInitialized)
        {
            Bootstrap.Shutdown();
            _bootstrapInitialized = false;
        }

        _dispatcherQueueController = null;
#endif
        _initialized = false;
        _initializationFailed = false;
        _skipReason = null;
    }

    private static void EnsureInitialized()
    {
        if (_initializationFailed)
        {
            Assert.Ignore(_skipReason ?? "MAUI test environment unavailable on this machine.");
        }

        if (_initialized)
        {
            return;
        }
#if WINDOWS
        try
        {
            EnsureWindowsAppSdk();
            EnsureDispatcherQueue();
        }
        catch (Exception ex)
        {
            HandleInitializationFailure(ex);
        }
#endif
        try
        {
            EnsureMauiApplication();
            DispatcherProvider.SetCurrent(new TestDispatcherProvider());
            _initialized = true;
        }
        catch (Exception ex)
        {
            HandleInitializationFailure(ex);
        }
    }

    private static void EnsureMauiApplication()
    {
        if (Application.Current is not null)
        {
            return;
        }

        _ = new TestApplication();
    }

#if WINDOWS
    private static void EnsureWindowsAppSdk()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var minVersion = default(PackageVersion);
        if (!Bootstrap.TryInitialize(WindowsAppSdkMajorMinorVersion, string.Empty, minVersion, Bootstrap.InitializeOptions.None, out var hresult))
        {
            throw new InvalidOperationException($"Failed to bootstrap Windows App SDK. HRESULT: 0x{hresult:X8}");
        }

        _bootstrapInitialized = true;
    }

    private static void EnsureDispatcherQueue()
    {
        if (DispatcherQueue.GetForCurrentThread() is not null)
        {
            return;
        }

        _dispatcherQueueController = DispatcherQueueController.CreateOnCurrentThread();
    }
#endif

    private static void HandleInitializationFailure(Exception exception)
    {
        _initializationFailed = true;
        _skipReason = $"MAUI test environment unavailable ({GetInnermostException(exception).GetType().Name}): {GetInnermostException(exception).Message}";
        Assert.Ignore(_skipReason);
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

    private sealed class TestApplication : Application;

    private sealed class TestDispatcherProvider : IDispatcherProvider
    {
        private readonly IDispatcher _dispatcher = new TestDispatcher();

        public IDispatcher GetForCurrentThread() => _dispatcher;

        public IDispatcher GetMainThreadDispatcher() => _dispatcher;
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

        public IDispatcherTimer CreateTimer() => new TestDispatcherTimer(this);
    }

    private sealed class TestDispatcherTimer(TestDispatcher dispatcher) : IDispatcherTimer
    {
        public event EventHandler? Tick;

        public TimeSpan Interval { get; set; }

        public bool IsRunning { get; private set; }

        public bool IsRepeating { get; set; }

        public void Start()
        {
            IsRunning = true;
            dispatcher.Dispatch(() => Tick?.Invoke(this, EventArgs.Empty));

            if (!IsRepeating)
            {
                Stop();
            }
        }

        public void Stop() => IsRunning = false;
    }
}
