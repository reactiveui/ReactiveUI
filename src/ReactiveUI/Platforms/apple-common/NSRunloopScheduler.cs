// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using CoreFoundation;
using Foundation;
using NSAction = System.Action;

namespace ReactiveUI
{
    /// <summary>
    /// Provides a scheduler which will use the Cocoa main loop to schedule
    /// work on. This is the Cocoa equivalent of DispatcherScheduler.
    /// </summary>
    public class NSRunloopScheduler : IScheduler
    {
        /// <inheritdoc/>
        public DateTimeOffset Now => DateTimeOffset.Now;

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            var innerDisp = new SingleAssignmentDisposable();

            DispatchQueue.MainQueue.DispatchAsync(new NSAction(() =>
            {
                if (!innerDisp.IsDisposed)
                {
                    innerDisp.Disposable = action(this, state);
                }
            }));

            return innerDisp;
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (dueTime <= Now)
            {
                return Schedule(state, action);
            }

            return Schedule(state, dueTime - Now, action);
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var innerDisp = Disposable.Empty;
            bool isCancelled = false;

            var timer = NSTimer.CreateScheduledTimer(dueTime, _ =>
            {
                if (!isCancelled)
                {
                    innerDisp = action(this, state);
                }
            });

            return Disposable.Create(() =>
            {
                isCancelled = true;
                timer.Invalidate();
                innerDisp.Dispose();
            });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
