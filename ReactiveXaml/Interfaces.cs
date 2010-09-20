using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Concurrency;
using System.Diagnostics.Contracts;

namespace ReactiveXaml
{
    public class ObservedChange<TSender, TValue>
    {
        public TSender Sender { get; set; }
        public string PropertyName { get; set; }
    }

    public interface IReactiveNotifyPropertyChanged : INotifyPropertyChanged, IObservable<PropertyChangedEventArgs> { }

    public static class ReactiveNotifyPropertyChangedMixin
    {
        public static IObservable<ObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(this TSender This, string propertyName)
            where TSender : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(This != null); 

            return This.Where(x => x.PropertyName == propertyName)
                       .Select(x => new ObservedChange<TSender, TValue> { Sender = This, PropertyName = x.PropertyName });
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
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :