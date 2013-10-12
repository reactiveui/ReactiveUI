// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reactive;

namespace Microsoft.Reactive.Testing
{
    /// <summary>
    /// Base class to write unit tests for applications and libraries built using Reactive Extensions.
    /// </summary>
    public class ReactiveTest
    {
        /// <summary>
        /// Default virtual time used for creation of observable sequences in <see cref="ReactiveTest"/>-based unit tests.
        /// </summary>
        public const long Created = 100;
        
        /// <summary>
        /// Default virtual time used to subscribe to observable sequences in <see cref="ReactiveTest"/>-based unit tests.
        /// </summary>
        public const long Subscribed = 200;

        /// <summary>
        /// Default virtual time used to dispose subscriptions in <see cref="ReactiveTest"/>-based unit tests.
        /// </summary>
        public const long Disposed = 1000;

        /// <summary>
        /// Factory method for an OnNext notification record at a given time with a given value.
        /// </summary>
        /// <typeparam name="T">The element type for the resulting notification object.</typeparam>
        /// <param name="ticks">Recorded virtual time the OnNext notification occurs.</param>
        /// <param name="value">Recorded value stored in the OnNext notification.</param>
        /// <returns>Recorded OnNext notification.</returns>
        public static Recorded<Notification<T>> OnNext<T>(long ticks, T value)
        {
            return new Recorded<Notification<T>>(ticks, Notification.CreateOnNext<T>(value));
        }

        /// <summary>
        /// Factory method for writing an assert that checks for an OnNext notification record at a given time, using the specified predicate to check the value.
        /// </summary>
        /// <typeparam name="T">The element type for the resulting notification object.</typeparam>
        /// <param name="ticks">Recorded virtual time the OnNext notification occurs.</param>
        /// <param name="predicate">Predicate function to check the OnNext notification value against an expected value.</param>
        /// <returns>Recorded OnNext notification with a predicate to assert a given value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is null.</exception>
        public static Recorded<Notification<T>> OnNext<T>(long ticks, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return new Recorded<Notification<T>>(ticks, new OnNextPredicate<T>(predicate));
        }

        /// <summary>
        /// Factory method for an OnCompleted notification record at a given time.
        /// </summary>
        /// <typeparam name="T">The element type for the resulting notification object.</typeparam>
        /// <param name="ticks">Recorded virtual time the OnCompleted notification occurs.</param>
        /// <returns>Recorded OnCompleted notification.</returns>
        public static Recorded<Notification<T>> OnCompleted<T>(long ticks)
        {
            return new Recorded<Notification<T>>(ticks, Notification.CreateOnCompleted<T>());
        }

        /// <summary>
        /// Factory method for an OnCompleted notification record at a given time.
        /// </summary>
        /// <typeparam name="T">The element type for the resulting notification object.</typeparam>
        /// <param name="dummy">An unused instance of type T, to force the compiler to infer that T as part of the method's return value.</param>
        /// <param name="ticks">Recorded virtual time the OnCompleted notification occurs.</param>
        /// <returns>Recorded OnCompleted notification.</returns>
        /// <remarks>This overload is used for anonymous types - by passing in an instance of the type, the compiler can infer the 
        /// anonymous type without you having to try naming the type.</remarks>
        public static Recorded<Notification<T>> OnCompleted<T>(T dummy, long ticks)
        {
            return new Recorded<Notification<T>>(ticks, Notification.CreateOnCompleted<T>());
        }

        /// <summary>
        /// Factory method for an OnError notification record at a given time with a given error.
        /// </summary>
        /// <typeparam name="T">The element type for the resulting notification object.</typeparam>
        /// <param name="ticks">Recorded virtual time the OnError notification occurs.</param>
        /// <param name="exception">Recorded exception stored in the OnError notification.</param>
        /// <returns>Recorded OnError notification.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> is null.</exception>
        public static Recorded<Notification<T>> OnError<T>(long ticks, Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            return new Recorded<Notification<T>>(ticks, Notification.CreateOnError<T>(exception));
        }

        /// <summary>
        /// Factory method for writing an assert that checks for an OnError notification record at a given time, using the specified predicate to check the exception.
        /// </summary>
        /// <typeparam name="T">The element type for the resulting notification object.</typeparam>
        /// <param name="ticks">Recorded virtual time the OnError notification occurs.</param>
        /// <param name="predicate">Predicate function to check the OnError notification value against an expected exception.</param>
        /// <returns>Recorded OnError notification with a predicate to assert a given exception.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is null.</exception>
        public static Recorded<Notification<T>> OnError<T>(long ticks, Func<Exception, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return new Recorded<Notification<T>>(ticks, new OnErrorPredicate<T>(predicate));
        }

        /// <summary>
        /// Factory method for an OnError notification record at a given time with a given error.
        /// </summary>
        /// <typeparam name="T">The element type for the resulting notification object.</typeparam>
        /// <param name="dummy">An unused instance of type T, to force the compiler to infer that T as part of the method's return value.</param>
        /// <param name="ticks">Recorded virtual time the OnError notification occurs.</param>
        /// <param name="exception">Recorded exception stored in the OnError notification.</param>
        /// <returns>Recorded OnError notification.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> is null.</exception>
        /// <remarks>This overload is used for anonymous types - by passing in an instance of the type, the compiler can infer the 
        /// anonymous type without you having to try naming the type.</remarks>
        public static Recorded<Notification<T>> OnError<T>(T dummy, long ticks, Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            return new Recorded<Notification<T>>(ticks, Notification.CreateOnError<T>(exception));
        }

        /// <summary>
        /// Factory method for writing an assert that checks for an OnError notification record at a given time, using the specified predicate to check the exception.
        /// </summary>
        /// <typeparam name="T">The element type for the resulting notification object.</typeparam>
        /// <param name="dummy">An unused instance of type T, to force the compiler to infer that T as part of the method's return value.</param>
        /// <param name="ticks">Recorded virtual time the OnError notification occurs.</param>
        /// <param name="predicate">Predicate function to check the OnError notification value against an expected exception.</param>
        /// <returns>Recorded OnError notification with a predicate to assert a given exception.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is null.</exception>
        /// <remarks>This overload is used for anonymous types - by passing in an instance of the type, the compiler can infer the 
        /// anonymous type without you having to try naming the type.</remarks>
        public static Recorded<Notification<T>> OnError<T>(T dummy, long ticks, Func<Exception, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return new Recorded<Notification<T>>(ticks, new OnErrorPredicate<T>(predicate));
        }

        /// <summary>
        /// Factory method for a subscription record based on a given subscription and disposal time.
        /// </summary>
        /// <param name="start">Virtual time indicating when the subscription was created.</param>
        /// <param name="end">Virtual time indicating when the subscription was disposed.</param>
        /// <returns>Subscription object.</returns>
        public static Subscription Subscribe(long start, long end)
        {
            return new Subscription(start, end);
        }

        /// <summary>
        /// Factory method for a subscription record based on a given subscription time.
        /// </summary>
        /// <param name="start">Virtual time indicating when the subscription was created.</param>
        /// <returns>Subscription object.</returns>
        public static Subscription Subscribe(long start)
        {
            return new Subscription(start);
        }

        #region Predicate-based notification assert helper classes

        class OnNextPredicate<T> : PredicateNotification<T>
        {
            private readonly Func<T, bool> _predicate;

            public OnNextPredicate(Func<T, bool> predicate)
            {
                _predicate = predicate;
            }

            public override bool Equals(Notification<T> other)
            {
                if (Object.ReferenceEquals(this, other))
                    return true;
                if (Object.ReferenceEquals(other, null))
                    return false;
                if (other.Kind != NotificationKind.OnNext)
                    return false;

                return _predicate(other.Value);
            }
        }

        class OnErrorPredicate<T> : PredicateNotification<T>
        {
            private readonly Func<Exception, bool> _predicate;

            public OnErrorPredicate(Func<Exception, bool> predicate)
            {
                _predicate = predicate;
            }

            public override bool Equals(Notification<T> other)
            {
                if (Object.ReferenceEquals(this, other))
                    return true;
                if (Object.ReferenceEquals(other, null))
                    return false;
                if (other.Kind != NotificationKind.OnError)
                    return false;

                return _predicate(other.Exception);
            }
        }

        abstract class PredicateNotification<T> : Notification<T>
        {
            #region Non-implemented members (by design)

            public override T Value
            {
                get { throw new NotSupportedException(); }
            }

            public override bool HasValue
            {
                get { throw new NotSupportedException(); }
            }

            public override Exception Exception
            {
                get { throw new NotSupportedException(); }
            }

            public override NotificationKind Kind
            {
                get { throw new NotSupportedException(); }
            }

            public override void Accept(IObserver<T> observer)
            {
                throw new NotSupportedException();
            }

            public override TResult Accept<TResult>(IObserver<T, TResult> observer)
            {
                throw new NotSupportedException();
            }

            public override void Accept(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                throw new NotSupportedException();
            }

            public override TResult Accept<TResult>(Func<T, TResult> onNext, Func<Exception, TResult> onError, Func<TResult> onCompleted)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion
    }
}
