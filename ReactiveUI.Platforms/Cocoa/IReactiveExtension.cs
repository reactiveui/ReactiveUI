using System;
using System.Runtime.CompilerServices;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Threading;
using System.Reactive.Disposables;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using Splat;
using System.Collections.Generic;

namespace ReactiveUI.Cocoa {

    public interface IReactiveExtension : IEnableLogger {
        event PropertyChangingEventHandler PropertyChanging;
        event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanging(PropertyChangingEventArgs args);
        void RaisePropertyChanged(PropertyChangedEventArgs args);
    }

    public static class IReactiveExtensionExtensions {
        static ConditionalWeakTable<IReactiveExtension, ExtensionState> state = new ConditionalWeakTable<IReactiveExtension, ExtensionState>();

        internal static void setupReactiveExtension(this IReactiveExtension This) {
            state.GetOrCreateValue(This);
        }

        internal static IObservable<IObservedChange<object, object>> getChangedObservable(this IReactiveExtension This) {
            return state.GetOrCreateValue(This).ChangedSubject;
        }

        internal static IObservable<IObservedChange<object, object>> getChangingObservable(this IReactiveExtension This) {
            return state.GetOrCreateValue(This).ChangingSubject;
        }

        internal static IObservable<Exception> getThrownExceptionsObservable(this IReactiveExtension This) {
            return state.GetOrCreateValue(This).ThrownExceptions;
        }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        internal static IDisposable suppressChangeNotifications(this IReactiveExtension This)
        {
            var s = state.GetOrCreateValue(This);
            Interlocked.Increment(ref s.ChangeNotificationsSuppressed);
            return Disposable.Create(() => Interlocked.Decrement(ref s.ChangeNotificationsSuppressed));
        }

        internal static void raisePropertyChanging(this IReactiveExtension This, string propertyName)
        {
            Contract.Requires(propertyName != null);

            var s = state.GetOrCreateValue(This);

            if (!This.areChangeNotificationsEnabled() || s.ChangingSubject == null)
                return;

            This.RaisePropertyChanging(new PropertyChangingEventArgs(propertyName));

            This.notifyObservable(new ObservedChange<object, object>() {
                PropertyName = propertyName, Sender = This, Value = null
            }, s.ChangingSubject);
        }

        internal static void raisePropertyChanged(this IReactiveExtension This, string propertyName)
        {
            Contract.Requires(propertyName != null);

            var s = state.GetOrCreateValue(This);

            This.Log().Debug("{0:X}.{1} changed", This.GetHashCode(), propertyName);

            if (!This.areChangeNotificationsEnabled() || s.ChangedSubject == null) {
                This.Log().Debug("Suppressed change");
                return;
            }

            This.RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));

            This.notifyObservable(new ObservedChange<object, object>() {
                PropertyName = propertyName, Sender = This, Value = null
            }, s.ChangedSubject);
        }

        internal static bool areChangeNotificationsEnabled(this IReactiveExtension This) {
            var s = state.GetOrCreateValue(This);

            return (Interlocked.Read(ref s.ChangeNotificationsSuppressed) == 0);
        }

        internal static void notifyObservable<T>(this IReactiveExtension This, T item, Subject<T> subject)
        {
            var s = state.GetOrCreateValue(This);

            try {
                subject.OnNext(item);
            } catch (Exception ex) {
                This.Log().ErrorException("ReactiveObject Subscriber threw exception", ex);
                s.ThrownExceptions.OnNext(ex);
            }
        }

        /// <summary>
        /// RaiseAndSetIfChanged fully implements a Setter for a read-write
        /// property on a ReactiveObject, using CallerMemberName to raise the notification
        /// and the ref to the backing field to set the property.
        /// </summary>
        /// <typeparam name="TObj">The type of the This.</typeparam>
        /// <typeparam name="TRet">The type of the return value.</typeparam>
        /// <param name="This">The <see cref="ReactiveObject"/> raising the notification.</param>
        /// <param name="backingField">A Reference to the backing field for this
        /// property.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property, usually 
        /// automatically provided through the CallerMemberName attribute.</param>
        /// <returns>The newly set value, normally discarded.</returns>
        public static TRet RaiseAndSetIfChanged<TRet>(
            this IReactiveExtension This,
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string propertyName = null)
        {
            Contract.Requires(propertyName != null);

            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue)) {
                return newValue;
            }

            This.raisePropertyChanging(propertyName);
            backingField = newValue;
            This.raisePropertyChanged(propertyName);
            return newValue;
        }

        /// <summary>
        /// Use this method in your ReactiveObject classes when creating custom
        /// properties where raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="This">The instance of ReactiveObject on which the property has changed.</param>
        /// <param name="propertyName">
        /// A string representing the name of the property that has been changed.
        /// Leave <c>null</c> to let the runtime set to caller member name.
        /// </param>
        public static void RaisePropertyChanged(this IReactiveExtension This, [CallerMemberName] string propertyName = null)
        {
            This.raisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Use this method in your ReactiveObject classes when creating custom
        /// properties where raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="This">The instance of ReactiveObject on which the property has changed.</param>
        /// <param name="propertyName">
        /// A string representing the name of the property that has been changed.
        /// Leave <c>null</c> to let the runtime set to caller member name.
        /// </param>
        public static void RaisePropertyChanging(this IReactiveExtension This, [CallerMemberName] string propertyName = null)
        {
            This.raisePropertyChanging(propertyName);
        }

        class ExtensionState {
            public ExtensionState() {
                ChangingSubject = new Subject<IObservedChange<object, object>>();
                ChangedSubject = new Subject<IObservedChange<object, object>>();
                ThrownExceptions = new ScheduledSubject<Exception>(Scheduler.Immediate, RxApp.DefaultExceptionHandler);
            }

            public Subject<IObservedChange<object, object>> ChangingSubject { get; private set; }
            public Subject<IObservedChange<object, object>> ChangedSubject { get; private set; }
            public ScheduledSubject<Exception> ThrownExceptions { get; private set; }

            public long ChangeNotificationsSuppressed;
        }
    }
}

