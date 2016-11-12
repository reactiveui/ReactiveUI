using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using Splat;

namespace ReactiveUI
{
    /*
     * N.B. Why we have this evil global class
     *
     * In a WPF or Silverlight application, most commands must have the Dispatcher
     * scheduler set, because notifications will end up being run on another thread;
     * this happens most often in a CanExecute observable. Unfortunately, in a Unit
     * Test framework, while the MS Test Unit runner will *set* the Dispatcher (so
     * we can't even use the lack of its presence to determine whether we're in a
     * test runner or not), none of the items queued to it will ever be executed
     * during the unit test.
     *
     * Initially, I tried to plumb the ability to set the scheduler throughout the
     * classes, but when you start building applications on top of that, having to
     * have *every single* class have a default Scheduler property is really
     * irritating, with either default making life difficult.
     *
     * This class also initializes a whole bunch of other stuff, including the IoC container,
     * logging and error handling.
     */
    public static class RxApp
    {
        static RxApp()
        {
#if !PORTABLE
            _TaskpoolScheduler = TaskPoolScheduler.Default;
#endif

            // Initialize this to false as most platforms do not support
            // range notification for INotifyCollectionChanged
#if WP8 || NETFX_CORE
            SupportsRangeNotifications = false;
#else
            SupportsRangeNotifications = true;
#endif

            Locator.RegisterResolverCallbackChanged(() => {
                if (Locator.CurrentMutable == null) return;
                Locator.CurrentMutable.InitializeReactiveUI();
            });

            DefaultExceptionHandler = Observer.Create<Exception>(ex => {
                // NB: If you're seeing this, it means that an
                // ObservableAsPropertyHelper or the CanExecute of a
                // ReactiveCommand ended in an OnError. Instead of silently
                // breaking, ReactiveUI will halt here if a debugger is attached.
                if (Debugger.IsAttached) {
                    Debugger.Break();
                }

                RxApp.MainThreadScheduler.Schedule(() => {
                    throw new UnhandledErrorException(
                        "An object implementing IHandleObservableErrors (often a ReactiveCommand or ObservableAsPropertyHelper) has errored, thereby breaking its observable pipeline. To prevent this, ensure the pipeline does not error, or Subscribe to the ThrownExceptions property of the object in question to handle the erroneous case.",
                        ex);
                });
            });

            if (ModeDetector.InUnitTestRunner()) {
                LogHost.Default.Warn("*** Detected Unit Test Runner, setting MainThreadScheduler to CurrentThread ***");
                LogHost.Default.Warn("If we are not actually in a test runner, please file a bug\n\n");
                LogHost.Default.Warn("ReactiveUI acts differently under a test runner, see the docs\n");
                LogHost.Default.Warn("for more info about what to expect");

                _MainThreadScheduler = CurrentThreadScheduler.Instance;
                return;
            } else {
                LogHost.Default.Info("Initializing to normal mode");
            }

            if (_MainThreadScheduler == null) {
                _MainThreadScheduler = DefaultScheduler.Instance;
            }

            SuspensionHost = new SuspensionHost();
        }

        [ThreadStatic] static IScheduler _UnitTestMainThreadScheduler;
        static IScheduler _MainThreadScheduler;

        /// <summary>
        /// MainThreadScheduler is the scheduler used to schedule work items that
        /// should be run "on the UI thread". In normal mode, this will be
        /// DispatcherScheduler, and in Unit Test mode this will be Immediate,
        /// to simplify writing common unit tests.
        /// </summary>
        public static IScheduler MainThreadScheduler {
            get {
                return _UnitTestMainThreadScheduler ?? _MainThreadScheduler;
            }
            set {
                // N.B. The ThreadStatic dance here is for the unit test case -
                // often, each test will override MainThreadScheduler with their
                // own TestScheduler, and if this wasn't ThreadStatic, they would
                // stomp on each other, causing test cases to randomly fail,
                // then pass when you rerun them.
                if (ModeDetector.InUnitTestRunner()) {
                    _UnitTestMainThreadScheduler = value;
                    _MainThreadScheduler = _MainThreadScheduler ?? value;
                } else {
                    _MainThreadScheduler = value;
                }
            }
        }

        [ThreadStatic] static IScheduler _UnitTestTaskpoolScheduler;
        static IScheduler _TaskpoolScheduler;

        /// <summary>
        /// TaskpoolScheduler is the scheduler used to schedule work items to
        /// run in a background thread. In both modes, this will run on the TPL
        /// Task Pool (or the normal Threadpool on Silverlight).
        /// </summary>
        public static IScheduler TaskpoolScheduler {
            get {
                return _UnitTestTaskpoolScheduler ?? _TaskpoolScheduler;
            }
            set {
                if (ModeDetector.InUnitTestRunner()) {
                    _UnitTestTaskpoolScheduler = value;
                    _TaskpoolScheduler = _TaskpoolScheduler ?? value;
                } else {
                    _TaskpoolScheduler = value;
                }
            }
        }

        static IObserver<Exception> _DefaultExceptionHandler;

        /// <summary>
        /// This Observer is signalled whenever an object that has a
        /// ThrownExceptions property doesn't Subscribe to that Observable. Use
        /// Observer.Create to set up what will happen - the default is to crash
        /// the application with an error message.
        /// </summary>
        public static IObserver<Exception> DefaultExceptionHandler {
            get { return _DefaultExceptionHandler; }
            set { _DefaultExceptionHandler = value; }
        }

        [ThreadStatic] static ISuspensionHost _UnitTestSuspensionHost;
        static ISuspensionHost _SuspensionHost;

        /// <summary>
        /// This returns / allows you to override the current SuspensionHost, a
        /// class which provides events for process lifetime events, especially
        /// on mobile devices.
        /// </summary>
        public static ISuspensionHost SuspensionHost {
            get {
                var host = _UnitTestSuspensionHost ?? _SuspensionHost;
                return host;
            }
            set {
                if (ModeDetector.InUnitTestRunner()) {
                    _UnitTestSuspensionHost = value;
                    _SuspensionHost = _SuspensionHost ?? value;
                } else {
                    _SuspensionHost = value;
                }
            }
        }

        [ThreadStatic] static bool? _UnitTestSupportsRangeNotifications;
        static bool _SupportsRangeNotifications;

        /// <summary>
        /// Returns whether your UI framework is brain-dead or not and will blow
        /// up if a INotifyCollectionChanged object returns a ranged Add
        /// </summary>
        public static bool SupportsRangeNotifications  {
            get {
                return _UnitTestSupportsRangeNotifications ?? _SupportsRangeNotifications;
            }
            set {
                // N.B. The ThreadStatic dance here is for the unit test case -
                // often, each test will override MainThreadScheduler with their
                // own TestScheduler, and if this wasn't ThreadStatic, they would
                // stomp on each other, causing test cases to randomly fail,
                // then pass when you rerun them.
                if (ModeDetector.InUnitTestRunner()) {
                    _UnitTestSupportsRangeNotifications = value;
                    _SupportsRangeNotifications = value;
                } else {
                    _SupportsRangeNotifications = value;
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        internal static void EnsureInitialized()
        {
            // NB: This method only exists to invoke the static constructor
        }

#if ANDROID || SILVERLIGHT || IOS
        public const int SmallCacheLimit = 32;
        public const int BigCacheLimit = 64;
#else
        public const int SmallCacheLimit = 64;
        public const int BigCacheLimit = 256;
#endif
    }
}

// vim: tw=120 ts=4 sw=4 et :
