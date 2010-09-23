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

namespace ReactiveXaml
{
    public class ObservedChange<TSender, TValue>
    {
        public TSender Sender { get; set; }
        public string PropertyName { get; set; }
        public TValue Value { get; set; }
    }

    public interface IReactiveNotifyPropertyChanged : INotifyPropertyChanged, IObservable<PropertyChangedEventArgs> { }

    public static class ReactiveNotifyPropertyChangedMixin
    {
        [Obsolete("Use the Expression-based version instead!")]
        public static IObservable<ObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(this TSender This, string propertyName)
            where TSender : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(This != null); 

            return This.Where(x => x.PropertyName == propertyName)
                       .Select(x => new ObservedChange<TSender, TValue> { Sender = This, PropertyName = x.PropertyName });
        }

        public static IObservable<ObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(this TSender This, Expression<Func<TSender, TValue>> Property)
            where TSender : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(This != null);
            Contract.Requires(Property != null);

            string prop_name = RxApp.expressionToPropertyName(Property);
            var field_info = RxApp.getFieldInfoForProperty<TSender>(prop_name);
            return This
                .Where(x => x.PropertyName == prop_name)
                .Select(x => new ObservedChange<TSender, TValue>() { 
                    Sender = This, PropertyName = prop_name, Value = (TValue)field_info.GetValue(This)
                });
        }

        public static IObservable<TRet> ObservableForProperty<TSender, TValue, TRet>(this TSender This, Expression<Func<TSender, TValue>> Property, Func<TValue, TRet> Selector)
            where TSender : IReactiveNotifyPropertyChanged
        {
            return This.ObservableForProperty(Property).Select(x => Selector(x.Value));
        }
    }

    public interface IReactiveCollection<T> : IEnumerable<T>, IList<T>, INotifyCollectionChanged
    {
        IObservable<T> ItemsAdded { get; }
        IObservable<T> ItemsRemoved { get; }
        IObservable<int> CollectionCountChanged { get; }

        bool ChangeTrackingEnabled { get; set; }
        IObservable<ObservedChange<T, object>> ItemPropertyChanged { get; }
    }

    public interface IReactiveCommand : ICommand, IObservable<object> 
    {
        IObservable<bool> CanExecuteObservable {get;} 
    }

    public interface IReactiveAsyncCommand : IReactiveCommand
    {
        IObservable<int> ItemsInFlight { get; }
        IObservable<Unit> AsyncCompletedNotification { get; }
    }

    public interface IPromptUserForNewModel<T>
    {
        T Prompt(object Parameter);
    }

    public interface IViewForModel<T>
    {
        IDisposable Present(T Model, bool AsModal, Action OnClosed);
    }

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
            DeferredScheduler = Scheduler.ThreadPool;
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
        static MemoizingMRUCache<Tuple<Type, string>, FieldInfo> fieldInfoTypeCache = new MemoizingMRUCache<Tuple<Type,string>,FieldInfo>((x, _) => {
            var field_name = RxApp.GetFieldNameForProperty(x.Item2);
            return (x.Item1).GetField(field_name, BindingFlags.NonPublic | BindingFlags.Instance);
        }, 50);
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
                throw new ArgumentException("You must declare a backing field for this property named: " + prop_name);
            }
            return field;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :