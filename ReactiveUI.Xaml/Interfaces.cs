using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows.Input;
using ReactiveUI;

namespace ReactiveUI.Xaml
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
}

// vim: tw=120 ts=4 sw=4 et :