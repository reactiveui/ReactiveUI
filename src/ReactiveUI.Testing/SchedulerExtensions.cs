// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;

namespace ReactiveUI.Testing;

/// <summary>
/// Extension methods for the test based schedulers.
/// </summary>
public static class SchedulerExtensions
{
    private static readonly SemaphoreSlim _schedulerGate = new(1, 1);

    /// <summary>
    /// WithScheduler overrides the default Deferred and Taskpool schedulers
    /// with the given scheduler until the return value is disposed. This
    /// is useful in a unit test runner to force RxXaml objects to schedule
    /// via a TestScheduler object.
    /// </summary>
    /// <param name="scheduler">The scheduler to use.</param>
    /// <returns>An object that when disposed, restores the previous default
    /// schedulers.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithScheduler uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithScheduler uses methods that may require unreferenced code")]
#endif
    public static IDisposable WithScheduler(IScheduler scheduler)
    {
        _schedulerGate.Wait();
        var prevDef = RxApp.MainThreadScheduler;
        var prevTask = RxApp.TaskpoolScheduler;
        var prevRxDef = RxSchedulers.MainThreadScheduler;
        var prevRxTask = RxSchedulers.TaskpoolScheduler;

        RxApp.MainThreadScheduler = scheduler;
        RxApp.TaskpoolScheduler = scheduler;
        RxSchedulers.MainThreadScheduler = scheduler;
        RxSchedulers.TaskpoolScheduler = scheduler;

        return Disposable.Create(() =>
        {
            RxApp.MainThreadScheduler = prevDef;
            RxApp.TaskpoolScheduler = prevTask;
            RxSchedulers.MainThreadScheduler = prevRxDef;
            RxSchedulers.TaskpoolScheduler = prevRxTask;
            _schedulerGate.Release();
        });
    }

    /// <summary>
    /// With is an extension method that uses the given scheduler as the
    /// default Deferred and Taskpool schedulers for the given Func. Use
    /// this to initialize objects that store the default scheduler (most
    /// RxXaml objects).
    /// </summary>
    /// <typeparam name="T">The scheduler type.</typeparam>
    /// <typeparam name="TRet">The return type.</typeparam>
    /// <param name="scheduler">The scheduler to use.</param>
    /// <param name="block">The function to execute.</param>
    /// <returns>The return value of the function.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("With uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("With uses methods that may require unreferenced code")]
#endif
    public static TRet With<T, TRet>(this T scheduler, Func<T, TRet> block)
        where T : IScheduler
    {
        ArgumentExceptionHelper.ThrowIfNull(block);

        TRet ret;
        using (WithScheduler(scheduler))
        {
            ret = block(scheduler);
        }

        return ret;
    }

    /// <summary>
    /// With is an extension method that uses the given scheduler as the
    /// default Deferred and Taskpool schedulers for the given Func. Use
    /// this to initialize objects that store the default scheduler (most
    /// RxXaml objects).
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <typeparam name="TRet">The return type.</typeparam>
    /// <param name="scheduler">The scheduler to use.</param>
    /// <param name="block">The function to execute.</param>
    /// <returns>The return value of the function.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithAsync uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithAsync uses methods that may require unreferenced code")]
#endif
    public static async Task<TRet> WithAsync<T, TRet>(this T scheduler, Func<T, Task<TRet>> block)
        where T : IScheduler
    {
        ArgumentExceptionHelper.ThrowIfNull(block);

        TRet ret;
        using (WithScheduler(scheduler))
        {
            ret = await block(scheduler).ConfigureAwait(false);
        }

        return ret;
    }

    /// <summary>
    /// With is an extension method that uses the given scheduler as the
    /// default Deferred and Taskpool schedulers for the given Action.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="scheduler">The scheduler to use.</param>
    /// <param name="block">The action to execute.</param>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("With uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("With uses methods that may require unreferenced code")]
#endif
    public static void With<T>(this T scheduler, Action<T> block)
        where T : IScheduler =>
        scheduler.With(x =>
        {
            block(x);
            return 0;
        });

    /// <summary>
    /// With is an extension method that uses the given scheduler as the
    /// default Deferred and Taskpool schedulers for the given Action.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="scheduler">The scheduler to use.</param>
    /// <param name="block">The action to execute.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithAsync uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithAsync uses methods that may require unreferenced code")]
#endif
    public static Task WithAsync<T>(this T scheduler, Func<T, Task> block)
        where T : IScheduler =>
        scheduler.WithAsync(async x =>
        {
            await block(x).ConfigureAwait(false);
            return 0;
        });

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

// vim: tw=120 ts=4 sw=4 et :
