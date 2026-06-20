// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing;

/// <summary>Extension methods for the test based schedulers.</summary>
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
    public static IDisposable WithScheduler(ISequencer scheduler)
    {
        var prevDef = RxSchedulers.MainThreadScheduler;
        var prevTask = RxSchedulers.TaskpoolScheduler;

        RxSchedulers.MainThreadScheduler = scheduler;
        RxSchedulers.TaskpoolScheduler = scheduler;

        return new ActionDisposable(() =>
        {
            RxSchedulers.MainThreadScheduler = prevDef;
            RxSchedulers.TaskpoolScheduler = prevTask;
        });
    }

    /// <summary>Provides testing extension members for schedulers.</summary>
    /// <typeparam name="T">The scheduler type.</typeparam>
    /// <param name="scheduler">The scheduler to use.</param>
    extension<T>(T scheduler)
        where T : ISequencer
    {
        /// <summary>
        /// With is an extension method that uses the given scheduler as the
        /// default Deferred and Taskpool schedulers for the given Func. Use
        /// this to initialize objects that store the default scheduler (most
        /// RxXaml objects).
        /// </summary>
        /// <typeparam name="TRet">The return type.</typeparam>
        /// <param name="block">The function to execute.</param>
        /// <returns>The return value of the function.</returns>
        public TRet With<TRet>(Func<T, TRet> block)
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
        /// default Deferred and Taskpool schedulers for the given Action.
        /// </summary>
        /// <param name="block">The action to execute.</param>
        public void With(Action<T> block) =>
            scheduler.With(x =>
            {
                block(x);
                return 0;
            });

        /// <summary>
        /// With is an extension method that uses the given scheduler as the
        /// default Deferred and Taskpool schedulers for the given Func. Use
        /// this to initialize objects that store the default scheduler (most
        /// RxXaml objects).
        /// </summary>
        /// <typeparam name="TRet">The return type.</typeparam>
        /// <param name="block">The function to execute.</param>
        /// <returns>The return value of the function.</returns>
        public async Task<TRet> WithAsync<TRet>(Func<T, Task<TRet>> block)
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
        /// <param name="block">The action to execute.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task WithAsync(Func<T, Task> block) =>
            scheduler.WithAsync(async x =>
            {
                await block(x).ConfigureAwait(false);
                return 0;
            });
    }
}
