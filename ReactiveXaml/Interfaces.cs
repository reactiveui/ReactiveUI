using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Concurrency;

namespace ReactiveXaml
{
    public class ObservedChange<TSender, TValue>
    {
        public TSender Sender { get; set; }
        public TValue Value { get; set; }
    }

    public interface IReactiveNotifyPropertyChanged : INotifyPropertyChanged, IObservable<PropertyChangedEventArgs> { }

    public static class ReactiveNotifyPropertyChangedMixin
    {
        public static IObservable<ObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(this TSender This, string propertyName)
            where TSender : IReactiveNotifyPropertyChanged
        {
            var prop = This.GetType().GetProperty(propertyName);

            return This.Where(x => x.PropertyName == propertyName)
                       .Select(x => new ObservedChange<TSender, TValue> { Sender = This, Value = (TValue)prop.GetValue(This, null) });
        }
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
    public static class ReactiveXaml
    {
        static ReactiveXaml()
        {
#if DEBUG
            LoggerFactory = (prefix) => new StdErrLogger(prefix);
#else
            LoggerFactory = (prefix) => new NullLogger(prefix);
#endif

            if (InUnitTestRunner()) {
                Console.Error.WriteLine("*** Detected Unit Test Runner, setting Scheduler to Immediate ***");
                Console.Error.WriteLine("If we are not actually in a test runner, please file a bug\n");
                DefaultScheduler = Scheduler.Immediate;
                LoggerFactory = (prefix) => new StdErrLogger(prefix);
            } else {
                DefaultScheduler = Scheduler.Dispatcher;
            }
        }

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

        public static IScheduler DefaultScheduler { get; set; }
        public static Func<string, ILog> LoggerFactory { get; set; }
    }
}