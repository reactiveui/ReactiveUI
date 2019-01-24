// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Tests
{
    public class CountingTestScheduler : IScheduler
    {
        public CountingTestScheduler(IScheduler innerScheduler)
        {
            InnerScheduler = innerScheduler;
            ScheduledItems = new List<Tuple<Action, TimeSpan?>>();
        }

        public IScheduler InnerScheduler { get; }

        public List<Tuple<Action, TimeSpan?>> ScheduledItems { get; }

        public DateTimeOffset Now => InnerScheduler.Now;

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            ScheduledItems.Add(new Tuple<Action, TimeSpan?>(() => action(this, state), null));
            return InnerScheduler.Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            ScheduledItems.Add(new Tuple<Action, TimeSpan?>(() => action(this, state), dueTime));
            return InnerScheduler.Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            ScheduledItems.Add(new Tuple<Action, TimeSpan?>(() => action(this, state), null));
            return InnerScheduler.Schedule(state, action);
        }
    }
}
