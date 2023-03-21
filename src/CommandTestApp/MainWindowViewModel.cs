// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;

namespace CommandTestApp
{
    /// <summary>
    /// MainWindowViewModel.
    /// </summary>
    public class MainWindowViewModel : ReactiveObject
    {
        private static CancellationToken _cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            NormalCommand = ReactiveCommand.Create(NormalMethod);
            NormalAsyncCommand = ReactiveCommand.Create(NormalMethod, runAsync: true);
            ObservableCommand = ReactiveCommand.CreateFromObservable(ObservableMethod);
            TaskCommand = ReactiveCommand.CreateFromTask(() => TaskMethod());
            TaskTokenCommand = ReactiveCommand.CreateFromTask(token => TaskMethod(token));
        }

        /// <summary>
        /// Gets the normal command.
        /// </summary>
        /// <value>
        /// The normal command.
        /// </value>
        public ReactiveCommand<Unit, Unit> NormalCommand { get; }

        /// <summary>
        /// Gets the normal command.
        /// </summary>
        /// <value>
        /// The normal command.
        /// </value>
        public ReactiveCommand<Unit, Unit> NormalAsyncCommand { get; }

        /// <summary>
        /// Gets the observable command.
        /// </summary>
        /// <value>
        /// The observable command.
        /// </value>
        public ReactiveCommand<Unit, Unit> ObservableCommand { get; }

        /// <summary>
        /// Gets the task command.
        /// </summary>
        /// <value>
        /// The task command.
        /// </value>
        public ReactiveCommand<Unit, Unit> TaskCommand { get; }

        /// <summary>
        /// Gets the task token command.
        /// </summary>
        /// <value>
        /// The task token command.
        /// </value>
        public ReactiveCommand<Unit, Unit> TaskTokenCommand { get; }

        private static async Task<Unit> TaskMethod()
        {
            await Task.Delay(10000);
            return Unit.Default;
        }

        private static async Task<Unit> TaskMethod(CancellationToken token)
        {
            await Task.Delay(10000, token);
            return Unit.Default;
        }

        private static IObservable<Unit> ObservableMethod()
        {
            return Task.Delay(10000).ToObservable();
        }

        private void NormalMethod()
        {
            Task.Delay(10000).Wait();
        }
    }
}
