using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Concurrency;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
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
            LoggerFactory = (prefix) => new StdErrLogger(prefix);
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
                LoggerFactory = (prefix) => new StdErrLogger(prefix);

                DefaultUnitTestRecoveryResult = RecoveryOptionResult.FailOperation;

                CustomErrorPresentationFunc = new Func<UserException, RecoveryOptionResult>(e => {
                    Console.Error.WriteLine("Presenting Error: '{0}'", e.LocalizedFailureReason);
                    Console.Error.WriteLine("Returning default result: {0}", DefaultUnitTestRecoveryResult);
                    return DefaultUnitTestRecoveryResult;
                });
            }
            else
            {
                DeferredScheduler = Scheduler.Dispatcher;
            }

#if SILVERLIGHT
            TaskpoolScheduler = Scheduler.ThreadPool;
#else
            TaskpoolScheduler = Scheduler.TaskPool;
#endif
        }

        public static IScheduler DeferredScheduler { get; set; }
        public static IScheduler TaskpoolScheduler { get; set; }

        public static Func<string, ILog> LoggerFactory { get; set; }

        public static Func<UserException, RecoveryOptionResult> CustomErrorPresentationFunc { get; set; }
        public static Func<string, string> GetFieldNameForPropertyNameFunc { get; set; }
        public static RecoveryOptionResult DefaultUnitTestRecoveryResult { get; set; }

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
                "PEX",
            };

#if SILVERLIGHT
            return false;
#else
            return AppDomain.CurrentDomain.GetAssemblies().Any(x =>
                test_assemblies.Any(name => x.FullName.ToUpperInvariant().Contains(name)));
#endif
        }

        public static void EnableDebugMode()
        {
            LoggerFactory = (x => new StdErrLogger());

#if !SILVERLIGHT
            // NOTE: This is a handy feature for writing desktop applications;
            // it crashes the app whenever a dispatcher item would have hung the
            // UI.
            // XXX: This error message wording sucks
            DeferredScheduler = new StopwatchScheduler(TimeSpan.FromMilliseconds(400), 
                "The code that has just executed has prevented the UI from redrawing",
                Scheduler.Dispatcher);
#endif
        }

        public static RecoveryOptionResult PresentUserException(UserException ex)
        {
            if (CustomErrorPresentationFunc != null)
                return CustomErrorPresentationFunc(ex);

            // TODO: Pop the WPF dialog here if we're not in Silverlight
            throw new NotImplementedException();
        }

        public static string GetFieldNameForProperty(string PropertyName)
        {
            return GetFieldNameForPropertyNameFunc(PropertyName);
        }


        //
        // Internal utility functions
        //

        internal static string expressionToPropertyName<TObj, TRet>(Expression<Func<TObj, TRet>> Property) 
            where TObj : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(Property != null);
            Contract.Ensures(Contract.Result<string>() != null);

            string prop_name = null;

            try {
                var prop_expr = ((LambdaExpression)Property).Body as MemberExpression;
                prop_name = prop_expr.Member.Name;
            } catch (NullReferenceException) {
                throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
            }
            return prop_name;
        }

#if SILVERLIGHT
        static MemoizingMRUCache<Tuple<Type, string>, FieldInfo> fieldInfoTypeCache = new MemoizingMRUCache<Tuple<Type,string>,FieldInfo>((x, _) =>
            (x.Item1).GetField(RxApp.GetFieldNameForProperty(x.Item2)), 
            15 /*items*/);
#else
        static QueuedAsyncMRUCache<Tuple<Type, string>, FieldInfo> fieldInfoTypeCache = new QueuedAsyncMRUCache<Tuple<Type,string>,FieldInfo>(x => {
            var field_name = RxApp.GetFieldNameForProperty(x.Item2);
            return (x.Item1).GetField(field_name, BindingFlags.NonPublic | BindingFlags.Instance);
        }, 50);
#endif

        internal static FieldInfo getFieldInfoForProperty<TObj>(string prop_name) 
            where TObj : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(prop_name != null);
            FieldInfo field;

#if SILVERLIGHT
            lock(fieldInfoTypeCache) {
                field = fieldInfoTypeCache.Get(new Tuple<Type,string>(typeof(TObj), prop_name));
            }
#else
            field = fieldInfoTypeCache.Get(new Tuple<Type,string>(typeof(TObj), prop_name));
#endif 

            if (field == null) {
                throw new ArgumentException("You must declare a backing field for this property named: " + RxApp.GetFieldNameForProperty(prop_name));
            }
            return field;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :
