using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
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
#if PORTABLE
            _TaskpoolScheduler = Scheduler.Default;
#else
            _TaskpoolScheduler = TaskPoolScheduler.Default;
#endif
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

            Locator.CurrentMutable.InitializeReactiveUI();

            if (ModeDetector.InUnitTestRunner()) {
                LogHost.Default.Warn("*** Detected Unit Test Runner, setting MainThreadScheduler to CurrentThread ***");
                LogHost.Default.Warn("If we are not actually in a test runner, please file a bug\n");
                _MainThreadScheduler = CurrentThreadScheduler.Instance;
                return;
            } else {
                LogHost.Default.Info("Initializing to normal mode");
            }

            if (_MainThreadScheduler == null) {
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
                var scheduler = _UnitTestMainThreadScheduler ?? _MainThreadScheduler;
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
                var scheduler = _UnitTestTaskpoolScheduler ?? _TaskpoolScheduler;
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
            get {
                return _DefaultExceptionHandler;
            }
            set {
                _DefaultExceptionHandler = value;
            }
        }

        /// <summary>
        /// This method will initialize your custom service locator with the 
        /// built-in RxUI types. Use this to help initialize containers that
        /// don't conform easily to IMutableDependencyResolver.
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

            fakeResolver.InitializeReactiveUI();
        }

        /// <summary>
        /// Acessing Splat's Default Locator this way ensures it is initialized
        /// by ReactiveUI. This method is primarily used by ReactiveUI itself,
        /// you can usually ignore it and should use Locator.Current.
        /// </summary>
        public static IDependencyResolver DependencyResolver
        {
            get { return Locator.Current; }
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