// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Reactive.Disposables;
using System.Reactive.Linq;

#if NUNIT
using NUnit.Framework;
#elif WINDOWS8
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Reactive.Testing
{
    /// <summary>
    /// Helper class to write asserts in unit tests for applications and libraries built using Reactive Extensions.
    /// </summary>
    public static class ReactiveAssert
    {
        static string Message<T>(IEnumerable<T> actual, IEnumerable<T> expected)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("Expected: [");
            sb.Append(string.Join(", ", expected.Select(x => x.ToString()).ToArray()));
            sb.Append("]");
            sb.AppendLine();
            sb.Append("Actual..: [");
            sb.Append(string.Join(", ", actual.Select(x => x.ToString()).ToArray()));
            sb.Append("]");
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// Asserts that both enumerable sequences have equal length and equal elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="expected">Expected sequence.</param>
        /// <param name="actual">Actual sequence to compare against the expected one.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (actual == null)
                throw new ArgumentNullException("actual");

            if (!expected.SequenceEqual(actual))
                Assert.Fail(Message(actual, expected));
        }

        /// <summary>
        /// Asserts that both enumerable sequences have equal length and equal elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="expected">Expected sequence.</param>
        /// <param name="actual">Actual sequence to compare against the expected one.</param>
        /// <param name="message">Error message for assert failure.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (actual == null)
                throw new ArgumentNullException("actual");

            if (!expected.SequenceEqual(actual))
                Assert.Fail(message);
        }

        /// <summary>
        /// Asserts that both observable sequences have equal length and equal notifications.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="expected">Expected sequence.</param>
        /// <param name="actual">Actual sequence to compare against the expected one.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public static void AreElementsEqual<T>(IObservable<T> expected, IObservable<T> actual)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (actual == null)
                throw new ArgumentNullException("actual");

            AreElementsEqual(expected.Materialize().ToEnumerable(), actual.Materialize().ToEnumerable());
        }

        /// <summary>
        /// Asserts that both observable sequences have equal length and equal elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="expected">Expected sequence.</param>
        /// <param name="actual">Actual sequence to compare against the expected one.</param>
        /// <param name="message">Error message for assert failure.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public static void AreElementsEqual<T>(IObservable<T> expected, IObservable<T> actual, string message)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (actual == null)
                throw new ArgumentNullException("actual");

            AreElementsEqual(expected.Materialize().ToEnumerable(), actual.Materialize().ToEnumerable(), message);
        }

        /// <summary>
        /// Asserts that the given action throws an exception of the type specified in the generic parameter, or a subtype thereof.
        /// </summary>
        /// <typeparam name="TException">Type of the exception to check for.</typeparam>
        /// <param name="action">Action to run.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Throws<TException>(Action action) where TException : Exception
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var failed = false;
            try
            {
                action();
                failed = true;
            }
            catch (TException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format(CultureInfo.CurrentCulture, "Expected {0} threw {1}.\r\n\r\nStack trace:\r\n{2}", typeof(TException).Name, ex.GetType().Name, ex.StackTrace));
            }

            if (failed)
                Assert.Fail(string.Format(CultureInfo.CurrentCulture, "Expected {0}.", typeof(TException).Name));
        }

        /// <summary>
        /// Asserts that the given action throws an exception of the type specified in the generic parameter, or a subtype thereof.
        /// </summary>
        /// <typeparam name="TException">Type of the exception to check for.</typeparam>
        /// <param name="action">Action to run.</param>
        /// <param name="message">Error message for assert failure.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Throws<TException>(Action action, string message) where TException : Exception
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var failed = false;
            try
            {
                action();
                failed = true;
            }
            catch (TException)
            {
            }
            catch
            {
                Assert.Fail(message);
            }

            if (failed)
                Assert.Fail(message);
        }

        /// <summary>
        /// Asserts that the given action throws the specified exception.
        /// </summary>
        /// <typeparam name="TException">Type of the exception to check for.</typeparam>
        /// <param name="exception">Exception to assert being thrown.</param>
        /// <param name="action">Action to run.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Throws<TException>(TException exception, Action action) where TException : Exception
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var failed = false;
            try
            {
                action();
                failed = true;
            }
            catch (TException ex)
            {
                Assert.AreSame(exception, ex);
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format(CultureInfo.CurrentCulture, "Expected {0} threw {1}.\r\n\r\nStack trace:\r\n{2}", typeof(TException).Name, ex.GetType().Name, ex.StackTrace));
            }

            if (failed)
                Assert.Fail(string.Format(CultureInfo.CurrentCulture, "Expected {0}.", typeof(TException).Name));
        }

        /// <summary>
        /// Asserts that the given action throws the specified exception.
        /// </summary>
        /// <typeparam name="TException">Type of the exception to check for.</typeparam>
        /// <param name="exception">Exception to assert being thrown.</param>
        /// <param name="action">Action to run.</param>
        /// <param name="message">Error message for assert failure.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static void Throws<TException>(TException exception, Action action, string message) where TException : Exception
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var failed = false;
            try
            {
                action();
                failed = true;
            }
            catch (TException ex)
            {
                Assert.AreSame(exception, ex);
            }
            catch
            {
                Assert.Fail(message);
            }

            if (failed)
                Assert.Fail(message);
        }

        /// <summary>
        /// Asserts that both enumerable sequences have equal length and equal elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="actual">Actual sequence to compare against the expected one.</param>
        /// <param name="expected">Expected sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public static void AssertEqual<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            if (actual == null)
                throw new ArgumentNullException("actual");
            if (expected == null)
                throw new ArgumentNullException("expected");

            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        /// <summary>
        /// Asserts the enumerable sequence has the expected elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="actual">Actual sequence to compare against the expected elements.</param>
        /// <param name="expected">Expected elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public static void AssertEqual<T>(this IEnumerable<T> actual, params T[] expected)
        {
            if (actual == null)
                throw new ArgumentNullException("actual");
            if (expected == null)
                throw new ArgumentNullException("expected");

            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        /// <summary>
        /// Asserts that both observable sequences have equal length and equal notifications.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="actual">Actual sequence to compare against the expected one.</param>
        /// <param name="expected">Expected sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public static void AssertEqual<T>(this IObservable<T> actual, IObservable<T> expected)
        {
            if (actual == null)
                throw new ArgumentNullException("actual");
            if (expected == null)
                throw new ArgumentNullException("expected");

            ReactiveAssert.AreElementsEqual(expected, actual);
        }
    }
}
