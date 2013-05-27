using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Diagnostics.Contracts;
using System.Linq;      
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

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
        public static void Initialize()
        {
            _TaskpoolScheduler = Scheduler.TaskPool;
                
            DefaultExceptionHandler = Observer.Create<Exception>(ex => {
                // NB: If you're seeing this, it means that an 
                // ObservableAsPropertyHelper or the CanExecute of a 
                // ReactiveCommand ended in an OnError. Instead of silently 
                // breaking, ReactiveUI will halt here if a debugger is attached.
                if (Debugger.IsAttached) {
                    Debugger.Break();
                }

                RxApp.MainThreadScheduler.Schedule(() => {
                    throw new Exception(
                        "An OnError occurred on an object (usually ObservableAsPropertyHelper) that would break a binding or command. To prevent this, Subscribe to the ThrownExceptions property of your objects",
                        ex);
                });
            });

            initializeDependencyResolver();

            if (InUnitTestRunner()) {
                LogHost.Default.Warn("*** Detected Unit Test Runner, setting MainThreadScheduler to CurrentThread ***");
                LogHost.Default.Warn("If we are not actually in a test runner, please file a bug\n");
                _MainThreadScheduler = CurrentThreadScheduler.Instance;
            } else {
                LogHost.Default.Info("Initializing to normal mode");
            }

            if (MainThreadScheduler == null) {
#if !ANDROID
                // NB: We can't initialize a scheduler automatically on Android
                // because it is intrinsically tied to the current Activity, 
                // so devs have to set it up by hand :-/
                LogHost.Default.Error("*** ReactiveUI Platform DLL reference not added - using Default scheduler *** ");
                LogHost.Default.Error("Add a reference to ReactiveUI.{Xaml / Cocoa / etc}.");
                LogHost.Default.Error("or consider explicitly setting RxApp.MainThreadScheduler if not");
#endif
                _MainThreadScheduler = DefaultScheduler.Instance;
            }
        }

        [ThreadStatic] static IDependencyResolver _UnitTestDependencyResolver;
        static IDependencyResolver _DependencyResolver;

        public static IDependencyResolver DependencyResolver {
            get {
                IDependencyResolver resolver = _UnitTestDependencyResolver ?? _DependencyResolver;
                if (resolver == null) {
                    //if we haven't initialized yet, do this once
                    Initialize();
                }

                return resolver;
            }
            set {
                if (InUnitTestRunner()) {
                    _UnitTestDependencyResolver = value;
                    _DependencyResolver = _DependencyResolver ?? value;
                } else {
                    _DependencyResolver = value;
                }
            }
        }

        public static IMutableDependencyResolver MutableResolver {
            get { return DependencyResolver as IMutableDependencyResolver; }
            set { DependencyResolver = value; }
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
                IScheduler scheduler = _UnitTestMainThreadScheduler ?? _MainThreadScheduler;
                if (scheduler == null) {
                    //if we haven't initialized yet, do this once
                    Initialize();
                }

                return scheduler;
            }
            set {
                // N.B. The ThreadStatic dance here is for the unit test case -
                // often, each test will override MainThreadScheduler with their
                // own TestScheduler, and if this wasn't ThreadStatic, they would
                // stomp on each other, causing test cases to randomly fail,
                // then pass when you rerun them.
                if (InUnitTestRunner()) {
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
                IScheduler scheduler = _UnitTestTaskpoolScheduler ?? _TaskpoolScheduler;
                if (scheduler == null) {
                    // If we haven't initialized yet, do this once
                    Initialize();
                }

                return scheduler;
            }
            set {
                if (InUnitTestRunner()) {
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
            get {
                if (_DefaultExceptionHandler == null) {
                    // If we haven't initialized yet, do this once
                    Initialize();
                }

                return _DefaultExceptionHandler;
            }
            set {
                _DefaultExceptionHandler = value;
            }
        }

        /// <summary>
        /// This method allows you to override the return value of 
        /// RxApp.InUnitTestRunner - a null value means that InUnitTestRunner
        /// will determine this using its normal logic.
        /// </summary>
        public static bool? InUnitTestRunnerOverride { get; set; }

        static bool? _InUnitTestRunner;

        /// <summary>
        /// InUnitTestRunner attempts to determine heuristically if the current
        /// application is running in a unit test framework by checking 
        /// if the Rx TestScheduler is present.
        /// </summary>
        /// <returns>True if we have determined that a unit test framework is
        /// currently running.</returns>
        public static bool InUnitTestRunner()
        {
            if (InUnitTestRunnerOverride.HasValue) {
                return InUnitTestRunnerOverride.Value;
            }

            if (!_InUnitTestRunner.HasValue) {
                // NB: This is in a separate static ctor to avoid a deadlock on 
                // the static ctor lock when blocking on async methods 
                _InUnitTestRunner = UnitTestDetector.IsInUnitTestRunner() || DesignModeDetector.IsInDesignMode();
            }

            return _InUnitTestRunner.Value;
        }

        /// <summary>
        /// This method will initialize your custom service locator with the 
        /// built-in RxUI types.
        /// </summary>
        /// <param name="registerMethod">Create a method here that will 
        /// register a constant. For example, the NInject version of
        /// this method might look like:
        /// 
        /// (obj, type) => kernel.Bind(type).ToConstant(obj)
        /// </param>
        public static void InitializeCustomResolver(Action<object, Type> registerMethod)
        {
            var fakeResolver = new FuncDependencyResolver(null,
                (fac, type, str) => registerMethod(fac(), type));
            fakeResolver.InitializeResolver();
        }

        static internal bool suppressLogging { get; set; }

        static void initializeDependencyResolver()
        {
            var resolver = new ModernDependencyResolver();

            // NB: The reason that we need to do this is that logging itself
            // is set up via dependency resolution - if we try to log while
            // setting up the logger, we will end up StackOverflowException'ing
            resolver.InitializeResolver();

            _DependencyResolver = resolver;
        }
    }    
}

// vim: tw=120 ts=4 sw=4 et :
