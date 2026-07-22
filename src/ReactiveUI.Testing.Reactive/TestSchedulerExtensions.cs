// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using Microsoft.Reactive.Testing;

namespace ReactiveUI.Testing;

/// <summary>Extension methods for TestScheduler from Microsoft.Reactive.Testing.</summary>
public static class TestSchedulerExtensions
{
    /// <summary>Extension methods for the TestScheduler.</summary>
    /// <param name="scheduler">The scheduler to advance.</param>
    extension(TestScheduler scheduler)
    {
        /// <summary>Converts a timespan to a virtual time for testing.</summary>
        /// <param name="span">Timespan to convert.</param>
        /// <returns>Timespan for virtual scheduler to use.</returns>
        public static long FromTimeSpan(TimeSpan span) => span.Ticks;

        /// <summary>
        /// OnNextAt is a method to help create simulated input Observables in
        /// conjunction with CreateHotObservable or CreateColdObservable.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="milliseconds">The time offset to fire the notification
        /// on the recorded notification.</param>
        /// <param name="value">The value to produce.</param>
        /// <returns>A recorded notification that can be provided to
        /// TestScheduler.CreateHotObservable.</returns>
        public static Recorded<Notification<T>> OnNextAt<T>(double milliseconds, T value) =>
            new(
                TestScheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)),
                Notification.CreateOnNext(value));

        /// <summary>
        /// OnErrorAt is a method to help create simulated input Observables in
        /// conjunction with CreateHotObservable or CreateColdObservable.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="milliseconds">The time offset to fire the notification
        /// on the recorded notification.</param>
        /// <param name="ex">The exception to terminate the Observable
        /// with.</param>
        /// <returns>A recorded notification that can be provided to
        /// TestScheduler.CreateHotObservable.</returns>
        [SuppressMessage(
            "Design",
            "SST2307:A generic method's type parameter appears in no parameter, so no caller can infer it",
            Justification = "T is the notification element type; it appears only in the Recorded<Notification<T>> return and cannot be inferred.")]
        public static Recorded<Notification<T>> OnErrorAt<T>(
            double milliseconds,
            Exception ex) =>
            new(
                TestScheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)),
                Notification.CreateOnError<T>(ex));

        /// <summary>
        /// OnCompletedAt is a method to help create simulated input Observables in
        /// conjunction with CreateHotObservable or CreateColdObservable.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="milliseconds">The time offset to fire the notification
        /// on the recorded notification.</param>
        /// <returns>A recorded notification that can be provided to
        /// TestScheduler.CreateHotObservable.</returns>
        [SuppressMessage(
            "Design",
            "SST2307:A generic method's type parameter appears in no parameter, so no caller can infer it",
            Justification = "T is the notification element type; it appears only in the Recorded<Notification<T>> return and cannot be inferred.")]
        public static Recorded<Notification<T>> OnCompletedAt<T>(double milliseconds) =>
            new(
                TestScheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)),
                Notification.CreateOnCompleted<T>());

        /// <summary>AdvanceToMs moves the TestScheduler to the specified time in milliseconds.</summary>
        /// <param name="milliseconds">The time offset to set the TestScheduler
        /// to, in milliseconds. Note that this is *not* additive or
        /// incremental, it sets the time.</param>
        public void AdvanceToMs(double milliseconds)
        {
            ArgumentExceptionHelper.ThrowIfNull(scheduler);

            scheduler.AdvanceTo(TestScheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)));
        }

        /// <summary>AdvanceByMs moves the TestScheduler along by the specified time in milliseconds.</summary>
        /// <param name="milliseconds">The relative time to advance the TestScheduler
        /// by, in milliseconds.</param>
        public void AdvanceByMs(double milliseconds)
        {
            ArgumentExceptionHelper.ThrowIfNull(scheduler);

            scheduler.AdvanceBy(TestScheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)));
        }
    }
}
