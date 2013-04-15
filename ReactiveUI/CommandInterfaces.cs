using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveUI
{
    /// <summary>
    /// IReactiveCommand is an Rx-enabled version of ICommand that is also an
    /// Observable. Its Observable fires once for each invocation of
    /// ICommand.Execute and its value is the CommandParameter that was
    /// provided.
    /// </summary>
    public interface IReactiveCommand : ICommand, IObservable<object>, IHandleObservableErrors
    {
        /// <summary>
        /// Fires whenever the CanExecute of the ICommand changes. Note that
        /// this should not fire notifications unless the CanExecute changes
        /// (i.e. it should not fire 'true', 'true').
        /// </summary>
        IObservable<bool> CanExecuteObservable { get; }
    }

    /// <summary>
    /// IReactiveAsyncCommand represents commands that run an asynchronous
    /// operation in the background when invoked.
    /// </summary>
    public interface IReactiveAsyncCommand : IReactiveCommand
    {
        /// <summary>
        /// Fires whenever the number of asynchronous operations in-flight (i.e.
        /// currently running) changes and provides the new Count.
        /// </summary>
        IObservable<int> ItemsInflight { get; }

        /// <summary>
        /// Should be fired whenever an async operation starts.
        /// </summary>
        ISubject<Unit> AsyncStartedNotification { get; }

        /// <summary>
        /// Should be fired whenever an async operation completes.
        /// </summary>
        ISubject<Unit> AsyncCompletedNotification { get; }

        /// <summary>
        /// RegisterAsyncObservable registers an Rx-based async method whose
        /// results will be returned on the UI thread.
        /// </summary>
        /// <param name="calculationFunc">A calculation method that returns a
        /// future result, such as a method returned via
        /// Observable.FromAsyncPattern.</param>
        /// <returns>An Observable representing the items returned by the
        /// calculation result. Note that with this method it is possible with a
        /// calculationFunc to return multiple items per invocation of Execute.</returns>
        IObservable<TResult> RegisterAsyncObservable<TResult>(Func<object, IObservable<TResult>> calculationFunc);

        /// <summary>
        /// The maximum number of in-flight
        /// operations at a time - defaults to one.
        /// </summary>
        int MaximumConcurrent { get; }
    }

    public interface ICreatesCommandBinding
    {
        /// <summary>
        /// Returns a positive integer when this class supports 
        /// BindCommandToObject for this particular Type. If the method
        /// isn't supported at all, return a non-positive integer. When multiple
        /// implementations return a positive value, the host will use the one
        /// which returns the highest value. When in doubt, return '2' or '0'
        /// </summary>
        /// <param name="type">The type to query for.</param>
        /// <param name="hasEventTarget">If true, the host intends to use a custom
        /// event target.</param>
        /// <returns>A positive integer if BCTO is supported, zero or a negative
        /// value otherwise</returns>
        int GetAffinityForObject(Type type, bool hasEventTarget);

        /// <summary>
        /// Bind an ICommand to a UI object, in the "default" way. The meaning 
        /// of this is dependent on the implementation. Implement this if you
        /// have a new type of UI control that doesn't have 
        /// Command/CommandParameter like WPF or has a non-standard event name
        /// for "Invoke".
        /// </summary>
        /// <param name="command">The command to bind</param>
        /// <param name="target">The target object, usually a UI control of 
        /// some kind</param>
        /// <param name="commandParameter">An IObservable source whose latest 
        /// value will be passed as the command parameter to the command. Hosts
        /// will always pass a valid IObservable, but this may be 
        /// Observable.Empty</param>
        /// <returns>An IDisposable which will disconnect the binding when 
        /// disposed.</returns>
        IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter);

        /// <summary>
        /// Bind an ICommand to a UI object to a specific event. This event may
        /// be a standard .NET event, or it could be an event derived in another
        /// manner (i.e. in MonoTouch).
        /// </summary>
        /// <param name="command">The command to bind</param>
        /// <param name="target">The target object, usually a UI control of 
        /// some kind</param>
        /// <param name="commandParameter">An IObservable source whose latest 
        /// value will be passed as the command parameter to the command. Hosts
        /// will always pass a valid IObservable, but this may be 
        /// Observable.Empty</param>
        /// <param name="eventName">The event to bind to.</param>
        /// <returns></returns>
        /// <returns>An IDisposable which will disconnect the binding when 
        /// disposed.</returns>
        IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName);
    }
}
