// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Avalonia.Threading;

namespace ReactiveUI.Avalonia
{
    /// <summary>
    /// A reactive scheduler that uses Avalonia's <see cref="Dispatcher"/>.
    /// </summary>
    public class AvaloniaScheduler : LocalScheduler
    {
        /// <summary>
        /// The instance of the <see cref="AvaloniaScheduler"/>.
        /// </summary>
        public static readonly AvaloniaScheduler Instance = new();

        /// <summary>
        /// Users can schedule actions on the dispatcher thread while being on the correct thread already.
        /// We are optimizing this case by invoking user callback immediately which can lead to stack overflows in certain cases.
        /// To prevent this we are limiting amount of reentrant calls to <see cref="Schedule{TState}"/> before we will
        /// schedule on a dispatcher anyway.
        /// </summary>
        private const int MaxReentrantSchedules = 32;

        private int _reentrancyGuard;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaScheduler"/> class.
        /// </summary>
        private AvaloniaScheduler()
        {
        }

        /// <inheritdoc/>
        public override IDisposable Schedule<TState>(
            TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            IDisposable PostOnDispatcher()
            {
                var composite = new CompositeDisposable(2);

                var cancellation = new CancellationDisposable();

                Dispatcher.UIThread.Post(
                                         () =>
                                         {
                                             if (!cancellation.Token.IsCancellationRequested)
                                             {
                                                 composite.Add(action(this, state));
                                             }
                                         },
                                         DispatcherPriority.Background);

                composite.Add(cancellation);

                return composite;
            }

            if (dueTime == TimeSpan.Zero)
            {
                if (!Dispatcher.UIThread.CheckAccess())
                {
                    return PostOnDispatcher();
                }

                if (_reentrancyGuard >= MaxReentrantSchedules)
                {
                    return PostOnDispatcher();
                }

                try
                {
                    _reentrancyGuard++;

                    return action(this, state);
                }
                finally
                {
                    _reentrancyGuard--;
                }
            }

            {
                var composite = new CompositeDisposable(2);

                composite.Add(DispatcherTimer.RunOnce(() => composite.Add(action(this, state)), dueTime));

                return composite;
            }
        }
    }
}
