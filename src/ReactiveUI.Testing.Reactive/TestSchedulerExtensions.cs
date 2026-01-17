// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using Microsoft.Reactive.Testing;

namespace ReactiveUI.Testing.Reactive;

/// <summary>
/// Extension methods for TestScheduler from Microsoft.Reactive.Testing.
/// </summary>
public static class TestSchedulerExtensions
{
    /// <summary>
    /// AdvanceToMs moves the TestScheduler to the specified time in
    /// milliseconds.
    /// </summary>
    /// <param name="scheduler">The scheduler to advance.</param>
    /// <param name="milliseconds">The time offset to set the TestScheduler
    /// to, in milliseconds. Note that this is *not* additive or
    /// incremental, it sets the time.</param>
    public static void AdvanceToMs(this TestScheduler scheduler, double milliseconds)
    {
        ArgumentExceptionHelper.ThrowIfNull(scheduler);

        scheduler.AdvanceTo(scheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)));
    }

    /// <summary>
    /// AdvanceByMs moves the TestScheduler along by the specified time in
    /// milliseconds.
    /// </summary>
    /// <param name="scheduler">The scheduler to advance.</param>
    /// <param name="milliseconds">The relative time to advance the TestScheduler
    /// by, in milliseconds.</param>
    public static void AdvanceByMs(this TestScheduler scheduler, double milliseconds)
    {
        ArgumentExceptionHelper.ThrowIfNull(scheduler);

        scheduler.AdvanceBy(scheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)));
    }

    /// <summary>
    /// OnNextAt is a method to help create simulated input Observables in
    /// conjunction with CreateHotObservable or CreateColdObservable.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="scheduler">The scheduler to fire from.</param>
    /// <param name="milliseconds">The time offset to fire the notification
    /// on the recorded notification.</param>
    /// <param name="value">The value to produce.</param>
    /// <returns>A recorded notification that can be provided to
    /// TestScheduler.CreateHotObservable.</returns>
    public static Recorded<Notification<T>> OnNextAt<T>(this TestScheduler scheduler, double milliseconds, T value) =>
        new(
            scheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)),
            Notification.CreateOnNext(value));

    /// <summary>
    /// OnErrorAt is a method to help create simulated input Observables in
    /// conjunction with CreateHotObservable or CreateColdObservable.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="scheduler">The scheduler to fire from.</param>
    /// <param name="milliseconds">The time offset to fire the notification
    /// on the recorded notification.</param>
    /// <param name="ex">The exception to terminate the Observable
    /// with.</param>
    /// <returns>A recorded notification that can be provided to
    /// TestScheduler.CreateHotObservable.</returns>
    public static Recorded<Notification<T>> OnErrorAt<T>(this TestScheduler scheduler, double milliseconds, Exception ex) =>
        new(
            scheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)),
            Notification.CreateOnError<T>(ex));

    /// <summary>
    /// OnCompletedAt is a method to help create simulated input Observables in
    /// conjunction with CreateHotObservable or CreateColdObservable.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="scheduler">The scheduler to fire from.</param>
    /// <param name="milliseconds">The time offset to fire the notification
    /// on the recorded notification.</param>
    /// <returns>A recorded notification that can be provided to
    /// TestScheduler.CreateHotObservable.</returns>
    public static Recorded<Notification<T>> OnCompletedAt<T>(this TestScheduler scheduler, double milliseconds) =>
        new(
            scheduler.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)),
            Notification.CreateOnCompleted<T>());

    /// <summary>
    /// Converts a timespan to a virtual time for testing.
    /// </summary>
    /// <param name="scheduler">The scheduler.</param>
    /// <param name="span">Timespan to convert.</param>
    /// <returns>Timespan for virtual scheduler to use.</returns>
    public static long FromTimeSpan(this TestScheduler scheduler, TimeSpan span) => span.Ticks;
}
