// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace ReactiveUI.Tests
{
    public class CountingTestScheduler : IScheduler
    {
        public CountingTestScheduler(IScheduler innerScheduler)
        {
            InnerScheduler = innerScheduler;
            ScheduledItems = new List<(Action action, TimeSpan? dueTime)>();
        }

        public IScheduler InnerScheduler { get; }

        public List<(Action action, TimeSpan? dueTime)> ScheduledItems { get; }

        /// <inheritdoc/>
        public DateTimeOffset Now => InnerScheduler.Now;

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            ScheduledItems.Add((() => action(this, state), null));
            return InnerScheduler.Schedule(state, dueTime, action);
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            ScheduledItems.Add((() => action(this, state), dueTime));
            return InnerScheduler.Schedule(state, dueTime, action);
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            ScheduledItems.Add((() => action(this, state), null));
            return InnerScheduler.Schedule(state, action);
        }
    }
}
