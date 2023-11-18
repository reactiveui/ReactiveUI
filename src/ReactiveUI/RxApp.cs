// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// The main registration point for common class instances throughout a ReactiveUI application.
/// </summary>
/// <remarks>
/// N.B. Why we have this evil global class
/// In a WPF or UWP application, most commands must have the Dispatcher
/// scheduler set, because notifications will end up being run on another thread;
/// this happens most often in a CanExecute observable.Unfortunately, in a Unit
/// Test framework, while the MS Test Unit runner will *set* the Dispatcher (so
/// we can't even use the lack of its presence to determine whether we're in a
/// test runner or not), none of the items queued to it will ever be executed
/// during the unit test.
/// Initially, I tried to plumb the ability to set the scheduler throughout the
/// classes, but when you start building applications on top of that, having to
/// have *every single * class have a default Scheduler property is really
/// irritating, with either default making life difficult.
/// This class also initializes a whole bunch of other stuff, including the IoC container,
/// logging and error handling.
/// </remarks>
public static class RxApp
{
#if ANDROID || IOS
    /// <summary>
    /// The size of a small cache of items. Often used for the MemoizingMRUCache class.
    /// </summary>
    public const int SmallCacheLimit = 32;

    /// <summary>
    /// The size of a large cache of items. Often used for the MemoizingMRUCache class.
    /// </summary>
    public const int BigCacheLimit = 64;
#else
    /// <summary>
    /// The size of a small cache of items. Often used for the MemoizingMRUCache class.
    /// </summary>
    public const int SmallCacheLimit = 64;

    /// <summary>
    /// The size of a large cache of items. Often used for the MemoizingMRUCache class.
    /// </summary>
    public const int BigCacheLimit = 256;
#endif

    [ThreadStatic]
    private static IScheduler _unitTestTaskpoolScheduler = null!;

    private static IScheduler _taskpoolScheduler = null!;

    [ThreadStatic]
    private static IScheduler _unitTestMainThreadScheduler = null!;

    private static IScheduler _mainThreadScheduler = null!;

    [ThreadStatic]
    private static ISuspensionHost _unitTestSuspensionHost = null!;

    private static ISuspensionHost _suspensionHost = null!;
    private static bool _hasSchedulerBeenChecked;

    /// <summary>
    /// Initializes static members of the <see cref="RxApp"/> class.
    /// </summary>
    /// <exception cref="UnhandledErrorException">Default exception when we have unhandled exception in RxUI.</exception>
    static RxApp()
    {
#if !PORTABLE
        _taskpoolScheduler = TaskPoolScheduler.Default;
#endif

        Locator.RegisterResolverCallbackChanged(() =>
        {
            if (Locator.CurrentMutable is null)
            {
                return;
            }

            Locator.CurrentMutable.InitializeReactiveUI(PlatformRegistrationManager.NamespacesToRegister);
        });

        DefaultExceptionHandler = Observer.Create<Exception>(ex =>
        {
            // NB: If you're seeing this, it means that an
            // ObservableAsPropertyHelper or the CanExecute of a
            // ReactiveCommand ended in an OnError. Instead of silently
            // breaking, ReactiveUI will halt here if a debugger is attached.
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            MainThreadScheduler.Schedule(() =>
            {
#pragma warning disable CA1065 // Avoid exceptions in constructors -- In scheduler.
                throw new UnhandledErrorException(
                    "An object implementing IHandleObservableErrors (often a ReactiveCommand or ObservableAsPropertyHelper) has errored, thereby breaking its observable pipeline. To prevent this, ensure the pipeline does not error, or Subscribe to the ThrownExceptions property of the object in question to handle the erroneous case.",
                    ex);
#pragma warning restore CA1065
            });
        });

        _suspensionHost = new SuspensionHost();
        if (ModeDetector.InUnitTestRunner())
        {
            LogHost.Default.Warn("*** Detected Unit Test Runner, setting MainThreadScheduler to CurrentThread ***");
            LogHost.Default.Warn("If we are not actually in a test runner, please file a bug\n\n");
            LogHost.Default.Warn("ReactiveUI acts differently under a test runner, see the docs\n");
            LogHost.Default.Warn("for more info about what to expect");

            UnitTestMainThreadScheduler = CurrentThreadScheduler.Instance;
            return;
        }

        LogHost.Default.Info("Initializing to normal mode");

        _mainThreadScheduler ??= DefaultScheduler.Instance;
    }

    /// <summary>
    /// Gets or sets a scheduler used to schedule work items that
    /// should be run "on the UI thread". In normal mode, this will be
    /// DispatcherScheduler, and in Unit Test mode this will be Immediate,
    /// to simplify writing common unit tests.
    /// </summary>
    public static IScheduler MainThreadScheduler
    {
        get
        {
            if (ModeDetector.InUnitTestRunner())
            {
                return UnitTestMainThreadScheduler;
            }

            // If Scheduler is DefaultScheduler, user is likely using .NET Standard
            if (!_hasSchedulerBeenChecked && _mainThreadScheduler == Scheduler.Default)
            {
                _hasSchedulerBeenChecked = true;
                LogHost.Default.Warn("It seems you are running .NET Standard, but there is no host package installed!\n");
                LogHost.Default.Warn("You will need to install the specific host package for your platform (ReactiveUI.WPF, ReactiveUI.Blazor, ...)\n");
                LogHost.Default.Warn("You can install the needed package via NuGet, see https://reactiveui.net/docs/getting-started/installation/");
            }

            return _mainThreadScheduler!;
        }

        set
        {
            // N.B. The ThreadStatic dance here is for the unit test case -
            // often, each test will override MainThreadScheduler with their
            // own TestScheduler, and if this wasn't ThreadStatic, they would
            // stomp on each other, causing test cases to randomly fail,
            // then pass when you rerun them.
            if (ModeDetector.InUnitTestRunner())
            {
                UnitTestMainThreadScheduler = value;
                _mainThreadScheduler ??= value;
            }
            else
            {
                _mainThreadScheduler = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the a the scheduler used to schedule work items to
    /// run in a background thread. In both modes, this will run on the TPL
    /// Task Pool.
    /// </summary>
    public static IScheduler TaskpoolScheduler
    {
        get => _unitTestTaskpoolScheduler ?? _taskpoolScheduler;
        set
        {
            if (ModeDetector.InUnitTestRunner())
            {
                _unitTestTaskpoolScheduler = value;
                _taskpoolScheduler ??= value;
            }
            else
            {
                _taskpoolScheduler = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether log messages should be suppressed for command bindings in the view.
    /// </summary>
    public static bool SuppressViewCommandBindingMessage { get; set; }

    /// <summary>
    /// Gets or sets the Observer which signaled whenever an object that has a
    /// ThrownExceptions property doesn't Subscribe to that Observable. Use
    /// Observer.Create to set up what will happen - the default is to crash
    /// the application with an error message.
    /// </summary>
    public static IObserver<Exception> DefaultExceptionHandler { get; set; } = null!;

    /// <summary>
    /// Gets or sets the current SuspensionHost, a
    /// class which provides events for process lifetime events, especially
    /// on mobile devices.
    /// </summary>
    public static ISuspensionHost SuspensionHost
    {
        get => _unitTestSuspensionHost ?? _suspensionHost;
        set
        {
            if (ModeDetector.InUnitTestRunner())
            {
                _unitTestSuspensionHost = value;
                _suspensionHost ??= value;
            }
            else
            {
                _suspensionHost = value;
            }
        }
    }

    private static IScheduler UnitTestMainThreadScheduler
    {
        get => _unitTestMainThreadScheduler ??= CurrentThreadScheduler.Instance;

        set => _unitTestMainThreadScheduler = value;
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    internal static void EnsureInitialized()
    {
        // NB: This method only exists to invoke the static constructor
    }
}

// vim: tw=120 ts=4 sw=4 et :
