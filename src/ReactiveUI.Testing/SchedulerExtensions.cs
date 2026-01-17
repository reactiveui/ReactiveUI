// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading;

namespace ReactiveUI.Testing;

/// <summary>
/// Extension methods for the test based schedulers.
/// </summary>
public static class SchedulerExtensions
{
    /// <summary>
    /// WithScheduler overrides the default Deferred and Taskpool schedulers
    /// with the given scheduler until the return value is disposed. This
    /// is useful in a unit test runner to force RxXaml objects to schedule
    /// via a TestScheduler object.
    /// </summary>
    /// <param name="scheduler">The scheduler to use.</param>
    /// <returns>An object that when disposed, restores the previous default
    /// schedulers.</returns>
    public static IDisposable WithScheduler(IScheduler scheduler)
    {
        var prevDef = RxSchedulers.MainThreadScheduler;
        var prevTask = RxSchedulers.TaskpoolScheduler;

        RxSchedulers.MainThreadScheduler = scheduler;
        RxSchedulers.TaskpoolScheduler = scheduler;

        return Disposable.Create(() =>
        {
            RxSchedulers.MainThreadScheduler = prevDef;
            RxSchedulers.TaskpoolScheduler = prevTask;
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
    public static Task WithAsync<T>(this T scheduler, Func<T, Task> block)
        where T : IScheduler =>
        scheduler.WithAsync(async x =>
        {
            await block(x).ConfigureAwait(false);
            return 0;
        });
}
