﻿using System;
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

#if SILVERLIGHT
using System.Windows;
#endif

#if WINRT
using Windows.ApplicationModel;
using System.Reactive.Windows.Foundation;
using System.Reactive.Threading.Tasks;
using Windows.ApplicationModel.Store;

#endif

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
     */
    public static class RxApp
    {
        static RxApp()
        {
            // Default name for the field backing the "Foo" property => "_Foo"
            // This is used for ReactiveObject's RaiseAndSetIfChanged mixin
            GetFieldNameForPropertyNameFunc = new Func<string,string>(x => "_" + x);

#if WP7
            TaskpoolScheduler = new EventLoopScheduler();
#elif WP8
            //TaskpoolScheduler = Scheduler.TaskPool;
            TaskpoolScheduler = Scheduler.ThreadPool;
#elif SILVERLIGHT || DOTNETISOLDANDSAD
            TaskpoolScheduler = Scheduler.ThreadPool;
#elif WINRT            
            TaskpoolScheduler = System.Reactive.Concurrency.ThreadPoolScheduler.Default;
#else
            TaskpoolScheduler = Scheduler.TaskPool;
#endif

            DefaultExceptionHandler = Observer.Create<Exception>(ex => {
                // NB: If you're seeing this, it means that an 
                // ObservableAsPropertyHelper or the CanExecute of a 
                // ReactiveCommand ended in an OnError. Instead of silently 
                // breaking, ReactiveUI will halt here if a debugger is attached.
                if (Debugger.IsAttached) {
                    Debugger.Break();
                }

                RxApp.DeferredScheduler.Schedule(() => {
                    throw new Exception(
                        "An OnError occurred on an object (usually ObservableAsPropertyHelper) that would break a binding or command. To prevent this, Subscribe to the ThrownExceptions property of your objects",
                        ex);
                });
            });

            MessageBus = new MessageBus();

            LoggerFactory = t => new DebugLogger();

            RxApp.Register(typeof(INPCObservableForProperty), typeof(ICreatesObservableForProperty));
            RxApp.Register(typeof(IRNPCObservableForProperty), typeof(ICreatesObservableForProperty));
            RxApp.Register(typeof(POCOObservableForProperty), typeof(ICreatesObservableForProperty));
            RxApp.Register(typeof(NullDefaultPropertyBindingProvider), typeof(IDefaultPropertyBindingProvider));
            RxApp.Register(typeof(EqualityTypeConverter), typeof(IBindingTypeConverter));
            RxApp.Register(typeof(StringConverter), typeof(IBindingTypeConverter));

            RxApp.Register(typeof(BindingTrackerBindingHook), typeof(IPropertyBindingHook));
            RxApp.Register(typeof(BindingTrackerBindingHook), typeof(IBindingRegistry));

#if NET_45 || SILVERLIGHT5
            RxApp.Register(typeof(NotifyDataErrorInfoBindingProvider), typeof(IBindingErrorProvider));
#endif

#if !WINRT
            RxApp.Register(typeof(DataErrorInfoBindingProvider), typeof(IBindingErrorProvider));
#endif

#if !SILVERLIGHT && !WINRT
            RxApp.Register(typeof(ComponentModelTypeConverter), typeof(IBindingTypeConverter));
#endif

            var namespaces = attemptToEarlyLoadReactiveUIDLLs();

            namespaces.ForEach(ns => {
#if WINRT
                var assm = typeof (RxApp).GetTypeInfo().Assembly;
#else
                var assm = Assembly.GetExecutingAssembly();
#endif
                var fullName = typeof (RxApp).AssemblyQualifiedName;
                var targetType = ns + ".ServiceLocationRegistration";
                fullName = fullName.Replace("ReactiveUI.RxApp", targetType);
                fullName = fullName.Replace(assm.FullName, assm.FullName.Replace("ReactiveUI", ns));

                var registerTypeClass = Reflection.ReallyFindType(fullName, false);
                if (registerTypeClass != null) {
                    var registerer = (IWantsToRegisterStuff) Activator.CreateInstance(registerTypeClass);
                    registerer.Register();
                }
            });

            if (InUnitTestRunner()) {
                LogHost.Default.Warn("*** Detected Unit Test Runner, setting Scheduler to Immediate ***");
                LogHost.Default.Warn("If we are not actually in a test runner, please file a bug\n");
                DeferredScheduler = Scheduler.Immediate;
            } else {
                LogHost.Default.Info("Initializing to normal mode");
            }

            if (DeferredScheduler == null) {
                LogHost.Default.Error("*** ReactiveUI.Xaml DLL reference not added - using Event Loop *** ");
                LogHost.Default.Error("Add a reference to ReactiveUI.Xaml if you're using WPF / SL5 / WP7 / WinRT");
                LogHost.Default.Error("or consider explicitly setting RxApp.DeferredScheduler if not");
                RxApp.DeferredScheduler = new EventLoopScheduler();
            }
        }

        [ThreadStatic] static IScheduler _UnitTestDeferredScheduler;
        static IScheduler _DeferredScheduler;

        /// <summary>
        /// DeferredScheduler is the scheduler used to schedule work items that
        /// should be run "on the UI thread". In normal mode, this will be
        /// DispatcherScheduler, and in Unit Test mode this will be Immediate,
        /// to simplify writing common unit tests.
        /// </summary>
        public static IScheduler DeferredScheduler {
            get { return _UnitTestDeferredScheduler ?? _DeferredScheduler; }
            set {
                // N.B. The ThreadStatic dance here is for the unit test case -
                // often, each test will override DeferredScheduler with their
                // own TestScheduler, and if this wasn't ThreadStatic, they would
                // stomp on each other, causing test cases to randomly fail,
                // then pass when you rerun them.
                if (InUnitTestRunner()) {
                    _UnitTestDeferredScheduler = value;
                    _DeferredScheduler = _DeferredScheduler ?? value;
                } else {
                    _DeferredScheduler = value;
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
            get { return _UnitTestTaskpoolScheduler ?? _TaskpoolScheduler; }
            set {
                if (InUnitTestRunner()) {
                    _UnitTestTaskpoolScheduler = value;
                    _TaskpoolScheduler = _TaskpoolScheduler ?? value;
                } else {
                    _TaskpoolScheduler = value;
                }
            }
        }

        static Func<Type, IRxUILogger> _LoggerFactory;
        static internal readonly Subject<Unit> _LoggerFactoryChanged = new Subject<Unit>();

        /// <summary>
        /// Set this property to implement a custom logger provider - the
        /// string parameter is the 'prefix' (usually the class name of the log
        /// entry)
        /// </summary>
        public static Func<Type, IRxUILogger> LoggerFactory {
            get { return _LoggerFactory; }
            set { _LoggerFactory = value; _LoggerFactoryChanged.OnNext(Unit.Default); }
        }

        [ThreadStatic] static IMessageBus _UnitTestMessageBus;
        static IMessageBus _MessageBus;

        /// <summary>
        /// Set this property to implement a custom MessageBus for
        /// MessageBus.Current.
        /// </summary>
        public static IMessageBus MessageBus {
            get { return _UnitTestMessageBus ?? _MessageBus; }
            set {
                if (InUnitTestRunner()) {
                    _UnitTestMessageBus = value;
                    _MessageBus = _MessageBus ?? value;
                } else {
                    _MessageBus = value;
                }
            }
        }

        /// <summary>
        /// Set this property to override the default field naming convention
        /// of "_PropertyName" with a custom one.
        /// </summary>
        public static Func<string, string> GetFieldNameForPropertyNameFunc { get; set; }

        /// <summary>
        /// This method allows you to override the return value of 
        /// RxApp.InUnitTestRunner - a null value means that InUnitTestRunner
        /// will determine this using its normal logic.
        /// </summary>
        public static bool? InUnitTestRunnerOverride { get; set; }

        /// <summary>
        /// This Observer is signalled whenever an object that has a 
        /// ThrownExceptions property doesn't Subscribe to that Observable. Use
        /// Observer.Create to set up what will happen - the default is to crash
        /// the application with an error message.
        /// </summary>
        public static IObserver<Exception> DefaultExceptionHandler { get; set; }

        static bool? _inUnitTestRunner;

        /// <summary>
        /// InUnitTestRunner attempts to determine heuristically if the current
        /// application is running in a unit test framework.
        /// </summary>
        /// <returns>True if we have determined that a unit test framework is
        /// currently running.</returns>
        public static bool InUnitTestRunner()
        {
            if (InUnitTestRunnerOverride.HasValue) {
                return InUnitTestRunnerOverride.Value;
            }

            if (_inUnitTestRunner.HasValue) return _inUnitTestRunner.Value;

            // NB: This is in a separate static ctor to avoid a deadlock on 
            // the static ctor lock when blocking on async methods 
            _inUnitTestRunner = RealUnitTestDetector.InUnitTestRunner();
            return _inUnitTestRunner.Value;
        }


        /// <summary>
        /// GetFieldNameForProperty returns the corresponding backing field name
        /// for a given property name, using the convention specified in
        /// GetFieldNameForPropertyNameFunc.
        /// </summary>
        /// <param name="propertyName">The name of the property whose backing
        /// field needs to be found.</param>
        /// <returns>The backing field name.</returns>
        public static string GetFieldNameForProperty(string propertyName)
        {
            return GetFieldNameForPropertyNameFunc(propertyName);
        }


        // 
        // Service Location
        //

        static Func<Type, string, object> _getService;
        static Func<Type, string, IEnumerable<object>> _getAllServices;
        static Action<Type, Type, string> _register;

        public static T GetService<T>(string key = null)
        {
            return (T)GetService(typeof(T), key);
        }

        public static object GetService(Type type, string key = null)
        {
            if (_getService != null) goto callSl;

            lock (_preregisteredTypes) {
                if (_preregisteredTypes.Count == 0) goto callSl;

                var k = Tuple.Create(type, key);
                if (!_preregisteredTypes.ContainsKey(k)) goto callSl;
                return Activator.CreateInstance(_preregisteredTypes[k].First());
            }
            
        callSl:
            var getService = _getService ??
                ((_, __) => { throw new Exception("You need to call RxApp.ConfigureServiceLocator to set up service location"); });
            return getService(type, key);
        }

        public static IEnumerable<T> GetAllServices<T>(string key = null)
        {
            return GetAllServices(typeof(T), key).Cast<T>().ToArray();
        }

        public static IEnumerable<object> GetAllServices(Type type, string key = null)
        {
            if (_getAllServices != null) goto callSl;

            lock (_preregisteredTypes) {
                if (_preregisteredTypes.Count == 0) goto callSl;

                var k = Tuple.Create(type, key);
                if (!_preregisteredTypes.ContainsKey(k)) goto callSl;
                return _preregisteredTypes[k].Select(Activator.CreateInstance).ToArray();
            }
            
        callSl:
            var getAllServices = _getAllServices ??
                ((_,__) => { throw new Exception("You need to call RxApp.ConfigureServiceLocator to set up service location"); });
            return (getAllServices(type, key) ?? Enumerable.Empty<object>()).ToArray();
        }

        static readonly Dictionary<Tuple<Type, string>, List<Type>> _preregisteredTypes = new Dictionary<Tuple<Type, string>, List<Type>>();
        public static void Register(Type concreteType, Type interfaceType, string key = null)
        {
            // NB: This allows ReactiveUI itself (as well as other libraries) 
            // to register types before the actual service locator is set up,
            // or to serve as an ultra-crappy service locator if the app doesn't
            // use service location
            lock (_preregisteredTypes) {
                if (_register == null) {
                    var k = Tuple.Create(interfaceType, key);
                    if (!_preregisteredTypes.ContainsKey(k)) _preregisteredTypes[k] = new List<Type>();
                    _preregisteredTypes[k].Add(concreteType);
                } else {
                    _register(concreteType, interfaceType, key);
                }
            }
        }

        public static void ConfigureServiceLocator(
            Func<Type, string, object> getService, 
            Func<Type, string, IEnumerable<object>> getAllServices,
            Action<Type, Type, string> register)
        {
            if (getService == null || getAllServices == null || register == null) {
                throw new ArgumentException("Both getService and getAllServices must be implemented");
            }

            _getService = getService;
            _getAllServices = getAllServices;
            _register = register;

            // Empty out the types that were registered before service location
            // was set up.
            lock (_preregisteredTypes) {
                _preregisteredTypes.Keys
                    .SelectMany(x => _preregisteredTypes[x]
                        .Select(v => Tuple.Create(v, x.Item1, x.Item2)))
                    .ForEach(x => _register(x.Item1, x.Item2, x.Item3));
            }
        }

        public static bool IsServiceLocationConfigured()
        {
            return _getService != null && _getAllServices != null;
        }

        static IEnumerable<string> attemptToEarlyLoadReactiveUIDLLs()
        {
            var guiLibs = new[] {
                "ReactiveUI.Xaml",
                "ReactiveUI.Routing",
                "ReactiveUI.Gtk",
                "ReactiveUI.Cocoa",
                "ReactiveUI.Android",
                "ReactiveUI.NLog",
                "ReactiveUI.Mobile",
            };

#if WINRT || WP8
            // NB: WinRT hates your Freedom
            return new[] {"ReactiveUI.Xaml", "ReactiveUI.Routing", "ReactiveUI.Mobile", };
#elif SILVERLIGHT
            return new[] {"ReactiveUI.Xaml", "ReactiveUI.Routing"};
#else
            var name = Assembly.GetExecutingAssembly().GetName();
            var suffix = getArchSuffixForPath(Assembly.GetExecutingAssembly().Location);

            return guiLibs.SelectMany(x => {
                var fullName = String.Format("{0}{1}, Version={2}, Culture=neutral, PublicKeyToken=null", x, suffix, name.Version.ToString());

                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                if (String.IsNullOrEmpty(assemblyLocation))
                    return Enumerable.Empty<string>();

                var path = Path.Combine(Path.GetDirectoryName(assemblyLocation), x + suffix + ".dll");
                if (!File.Exists(path) && !RxApp.InUnitTestRunner()) {
                    LogHost.Default.Debug("Couldn't find {0}", path);
                    return Enumerable.Empty<string>();
                }

                try {
                    Assembly.Load(fullName);
                    return new[] {x};
                } catch (Exception ex) {
                    LogHost.Default.DebugException("Couldn't load " + x, ex);
                    return Enumerable.Empty<string>();
                }
            });
#endif
        }

        static string getArchSuffixForPath(string path)
        {
            var re = new Regex(@"(_[A-Za-z0-9]+)\.");
            var m = re.Match(Path.GetFileName(path));
            return m.Success ? m.Groups[1].Value : "";
        }
    }

    public class NullDefaultPropertyBindingProvider : IDefaultPropertyBindingProvider
    {
        public Tuple<string, int> GetPropertyForControl(object control)
        {
            return null;
        }
    }

    internal static class RealUnitTestDetector
    {
        public static bool InUnitTestRunner(string[] testAssemblies, string[] designEnvironments)
        {
#if SILVERLIGHT
            // NB: Deployment.Current.Parts throws an exception when accessed in Blend
            try {
                var ret = Deployment.Current.Parts.Any(x =>
                    testAssemblies.Any(name => x.Source.ToUpperInvariant().Contains(name)));

                if (ret) {
                    return ret;
                }
            } catch(Exception) {
                return true;
            }

            try {
                if (Application.Current.RootVisual != null && System.ComponentModel.DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual)) {
                    return false;
                }
            } catch {
                return true;
            }

            return false;
#elif WINRT
            if (DesignMode.DesignModeEnabled) return true;

            var depPackages = Package.Current.Dependencies.Select(x => x.Id.FullName);
            if (depPackages.Any(x => testAssemblies.Any(name => x.ToUpperInvariant().Contains(name)))) return true;


            var fileTask = Task.Factory.StartNew(async () =>
            {
                var files = await Package.Current.InstalledLocation.GetFilesAsync();
                return files.Select(x => x.Path).ToArray();
            }, TaskCreationOptions.HideScheduler).Unwrap();

            return fileTask.Result.Any(x => testAssemblies.Any(name => x.ToUpperInvariant().Contains(name)));
#else
            // Try to detect whether we're in design mode - bonus points, 
            // without access to any WPF references :-/
            var entry = Assembly.GetEntryAssembly();
            if (entry != null) {
                var exeName = (new FileInfo(entry.Location)).Name.ToUpperInvariant(); 

                if (designEnvironments.Any(x => x.Contains(exeName))) {
                    return true;
                }
            }

            return AppDomain.CurrentDomain.GetAssemblies().Any(x =>
                testAssemblies.Any(name => x.FullName.ToUpperInvariant().Contains(name)));
#endif
        }

        public static bool InUnitTestRunner()
        {
            // XXX: This is hacky and evil, but I can't think of any better way
            // to do this
            string[] testAssemblies = new[] {
                "CSUNIT",
                "NUNIT",
                "XUNIT",
                "MBUNIT",
                "TESTDRIVEN",
                "QUALITYTOOLS.TIPS.UNITTEST.ADAPTER",
                "QUALITYTOOLS.UNITTESTING.SILVERLIGHT",
                "PEX",
                "MSBUILD",
                "NBEHAVE",
                "TESTPLATFORM",
            };

            string[] designEnvironments = new[] {
                "BLEND.EXE",
                "MONODEVELOP",
                "SHARPDEVELOP.EXE",
                "XDESPROC.EXE",
            };

            return InUnitTestRunner(testAssemblies, designEnvironments);
        }

    }
}

// vim: tw=120 ts=4 sw=4 et :