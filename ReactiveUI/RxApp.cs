using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;

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
#if DEBUG
            LoggerFactory = (prefix) => new StdErrLogger(prefix) { CurrentLogLevel = LogLevel.Info };
#else
            LoggerFactory = (prefix) => new NullLogger(prefix);
#endif

            // Default name for the field backing the "Foo" property => "_Foo"
            // This is used for ReactiveObject's RaiseAndSetIfChanged mixin
            GetFieldNameForPropertyNameFunc = new Func<string,string>(x => "_" + x);

            if (InUnitTestRunner())
            {
                Console.Error.WriteLine("*** Detected Unit Test Runner, setting Scheduler to Immediate ***");
                Console.Error.WriteLine("If we are not actually in a test runner, please file a bug\n");
                DeferredScheduler = Scheduler.Immediate;

#if DEBUG
                LoggerFactory = (prefix) => new StdErrLogger(prefix) { CurrentLogLevel = LogLevel.Debug };
#else
                LoggerFactory = (prefix) => new StdErrLogger(prefix) { CurrentLogLevel = LogLevel.Info };
#endif

#if FALSE
                DefaultUnitTestRecoveryResult = RecoveryOptionResult.FailOperation;

                CustomErrorPresentationFunc = new Func<UserException, RecoveryOptionResult>(e => {
                    Console.Error.WriteLine("Presenting Error: '{0}'", e.LocalizedFailureReason);
                    Console.Error.WriteLine("Returning default result: {0}", DefaultUnitTestRecoveryResult);
                    return DefaultUnitTestRecoveryResult;
                });
#endif
            } else {
                Console.Error.WriteLine("Initializing to normal mode");
#if IOS
                // XXX: This should be an instance of NSRunloopScheduler
                DeferredScheduler = new EventLoopScheduler();
#else
                DeferredScheduler = Scheduler.Dispatcher;
#endif
            }

#if SILVERLIGHT
            TaskpoolScheduler = Scheduler.ThreadPool;
#else
            TaskpoolScheduler = Scheduler.TaskPool;
#endif

            MessageBus = new MessageBus();
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

        /// <summary>
        /// Set this property to implement a custom logger provider - the
        /// string parameter is the 'prefix' (usually the class name of the log
        /// entry)
        /// </summary>
        public static Func<string, ILog> LoggerFactory { get; set; }

        /// <summary>
        /// Set this property to implement a custom MessageBus for
        /// MessageBus.Current.
        /// </summary>
        public static IMessageBus MessageBus { get; set; }

#if FALSE
        public static Func<UserException, RecoveryOptionResult> CustomErrorPresentationFunc { get; set; }
        public static RecoveryOptionResult DefaultUnitTestRecoveryResult { get; set; }
#endif

        /// <summary>
        /// Set this property to override the default field naming convention
        /// of "_PropertyName" with a custom one.
        /// </summary>
        public static Func<string, string> GetFieldNameForPropertyNameFunc { get; set; }

        /// <summary>
        /// InUnitTestRunner attempts to determine heuristically if the current
        /// application is running in a unit test framework.
        /// </summary>
        /// <returns>True if we have determined that a unit test framework is
        /// currently running.</returns>
        public static bool InUnitTestRunner()
        {
            // XXX: This is hacky and evil, but I can't think of any better way
            // to do this
            string[] test_assemblies = new[] {
                "CSUNIT",
                "NUNIT",
                "XUNIT",
                "MBUNIT",
                "TESTDRIVEN",
                "QUALITYTOOLS.TIPS.UNITTEST.ADAPTER",
                "QUALITYTOOLS.UNITTESTING.SILVERLIGHT",
                "PEX",
            };

#if SILVERLIGHT
            return Deployment.Current.Parts.Any(x => 
                test_assemblies.Any(name => x.Source.ToUpperInvariant().Contains(name)));
#else
            return AppDomain.CurrentDomain.GetAssemblies().Any(x =>
                test_assemblies.Any(name => x.FullName.ToUpperInvariant().Contains(name)));
#endif
        }

        /// <summary>
        ///
        /// </summary>
        public static void EnableDebugMode()
        {
            LoggerFactory = (x => new StdErrLogger());

#if !SILVERLIGHT && !IOS
            // NOTE: This is a handy feature for writing desktop applications;
            // it crashes the app whenever a dispatcher item would have hung the
            // UI.
            // XXX: This error message wording sucks
            DeferredScheduler = new StopwatchScheduler(TimeSpan.FromMilliseconds(400), 
                "The code that has just executed has prevented the UI from redrawing",
                Scheduler.Dispatcher);
#endif
        }

#if FALSE
        public static RecoveryOptionResult PresentUserException(UserException ex)
        {
            if (CustomErrorPresentationFunc != null)
                return CustomErrorPresentationFunc(ex);

            // TODO: Pop the WPF dialog here if we're not in Silverlight
            throw new NotImplementedException();
        }
#endif

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
        // Internal utility functions
        //

        internal static string simpleExpressionToPropertyName<TObj, TRet>(Expression<Func<TObj, TRet>> Property) 
        {
            Contract.Requires(Property != null);
            Contract.Ensures(Contract.Result<string>() != null);

            string prop_name = null;

            try {
                var prop_expr = Property.Body as MemberExpression;
                if (prop_expr.Expression.NodeType != ExpressionType.Parameter) {
                    throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
                }

                prop_name = prop_expr.Member.Name;
            } catch (NullReferenceException) {
                throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
            }
            return prop_name;
        }

        internal static string[] expressionToPropertyNames<TObj, TRet>(Expression<Func<TObj, TRet>> Property)
        {
            var ret = new List<string>();

            var current = Property.Body;
            while(current.NodeType != ExpressionType.Parameter) {

                // This happens when a value type gets boxed
                if (current.NodeType == ExpressionType.Convert || current.NodeType == ExpressionType.ConvertChecked) {
                    var ue = (UnaryExpression) current;
                    current = ue.Operand;
                    continue;
                }

                if (current.NodeType != ExpressionType.MemberAccess) {
                    throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty.SomeOtherProperty'");
                }

                var me = (MemberExpression)current;
                ret.Insert(0, me.Member.Name);
                current = me.Expression;
            }

            return ret.ToArray();
        }

#if SILVERLIGHT
        static MemoizingMRUCache<Tuple<Type, string>, FieldInfo> fieldInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, FieldInfo>(
                (x, _) => (x.Item1).GetField(RxApp.GetFieldNameForProperty(x.Item2)), 
                15 /*items*/);
        static MemoizingMRUCache<Tuple<Type, string>, PropertyInfo> propInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, PropertyInfo>(
                (x, _) => (x.Item1).GetProperty(x.Item2), 
                15 /*items*/);
#else
        static MemoizingMRUCache<Tuple<Type, string>, FieldInfo> fieldInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, FieldInfo>((x, _) => {
                var field_name = RxApp.GetFieldNameForProperty(x.Item2);
                var ret = (x.Item1).GetField(field_name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                return ret;
            }, 50 /*items*/);
        static MemoizingMRUCache<Tuple<Type, string>, PropertyInfo> propInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, PropertyInfo>((x,_) => {
                var ret = (x.Item1).GetProperty(x.Item2, BindingFlags.Public | BindingFlags.Instance);
                return ret;
            }, 50/*items*/);
#endif

        internal static FieldInfo getFieldInfoForProperty<TObj>(string prop_name) 
            where TObj : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(prop_name != null);
            FieldInfo field;

            lock(fieldInfoTypeCache) {
                field = fieldInfoTypeCache.Get(new Tuple<Type,string>(typeof(TObj), prop_name));
            }

            if (field == null) {
                throw new ArgumentException("You must declare a backing field for this property named: " + 
                    RxApp.GetFieldNameForProperty(prop_name));
            }
            return field;
        }

        internal static PropertyInfo getPropertyInfoForProperty<TObj>(string prop_name)
        {
            return getPropertyInfoForProperty(typeof (TObj), prop_name);
        }

        internal static PropertyInfo getPropertyInfoForProperty(Type type, string prop_name)
        {
            Contract.Requires(prop_name != null);
            PropertyInfo pi;

#if SILVERLIGHT
            lock(propInfoTypeCache) {
                pi = propInfoTypeCache.Get(new Tuple<Type,string>(type, prop_name));
            }
#else
            pi = propInfoTypeCache.Get(new Tuple<Type,string>(type, prop_name));
#endif 

            if (pi == null) {
                throw new ArgumentException("You must declare a property named: " + prop_name);
            }

            return pi;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
