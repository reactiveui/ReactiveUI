// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace ReactiveUI.Benchmarks
{
    /// <summary>
    /// Benchmarks associated with the ReactiveCommand object.
    /// </summary>
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ReactiveCommandCreateBenchmark
    {
        private readonly IObservable<bool> _canExecute = new Subject<bool>().AsObservable();

        /// <summary>
        /// Gets or sets from observable.
        /// </summary>
        public Subject<MockViewModel> FromObservable { get; set; }

        /// <summary>
        /// Gets or sets from task.
        /// </summary>
        public Task FromTask { get; set; }

        /// <summary>
        /// Setup for all benchmark instances being run.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            FromObservable = new Subject<MockViewModel>();

            FromTask = Task.CompletedTask;
        }

        /// <summary>
        /// Benchmark for creating a ReactiveCommand.
        /// </summary>
        /// <returns>The command.</returns>
        [Benchmark(Baseline = true)]
        public object CreateReactiveCommand() => ReactiveCommand.Create(() => { });

        /// <summary>
        /// Benchmark for creating a ReactiveCommand from an observable.
        /// </summary>
        /// <returns>The command.</returns>
        [Benchmark]
        public object CreateReactiveCommandFromObservable() => ReactiveCommand.CreateFromObservable(() => FromObservable.AsObservable());

        /// <summary>
        /// Benchmark for creating a ReactiveCommand a task.
        /// </summary>
        /// <returns>The command.</returns>
        [Benchmark]
        public object CreateReactiveCommandFromTask() => ReactiveCommand.CreateFromTask(async () => await FromTask);

        /// <summary>
        /// Benchmark for creating a ReactiveCommand with a can execute observable.
        /// </summary>
        /// <returns>The command.</returns>
        [Benchmark]
        public object CreateWithCanExecute() => ReactiveCommand.Create(() => { }, _canExecute);

        /// <summary>
        /// Benchmark for creating a ReactiveCommand from an observable with a can execute observable.
        /// </summary>
        /// <returns>The command.</returns>
        [Benchmark]
        public object CreateFromObservableWithCanExecute() => ReactiveCommand.CreateFromObservable(() => FromObservable.AsObservable(), _canExecute);

        /// <summary>
        /// Benchmark for creating a ReactiveCommand from a task with a can execute observable.
        /// </summary>
        /// <returns>The command.</returns>
        [Benchmark]
        public object CreateFromTaskWithCanExecute() => ReactiveCommand.CreateFromTask(async () => await FromTask, _canExecute);

        /// <summary>
        /// Benchmark for creating a ReactiveCommand with a can execute observable.
        /// </summary>
        /// <returns>The command.</returns>
        [Benchmark]
        public object CreateWithCanExecuteAndScheduler() => ReactiveCommand.Create(() => { }, _canExecute, RxApp.MainThreadScheduler);

        /// <summary>
        /// Benchmark for creating a ReactiveCommand from an observable with a can execute observable.
        /// </summary>
        /// <returns>The command.</returns>
        [Benchmark]
        public object CreateFromObservableWithCanExecuteAndScheduler() => ReactiveCommand.CreateFromObservable(() => FromObservable.AsObservable(), _canExecute, RxApp.MainThreadScheduler);

        /// <summary>
        /// Benchmark for creating a ReactiveCommand from a task with a can execute observable.
        /// </summary>
        /// <returns>The command.</returns>
        [Benchmark]
        public object CreateFromTaskWithCanExecuteAndScheduler() => ReactiveCommand.CreateFromTask(async () => await FromTask, _canExecute, RxApp.MainThreadScheduler);
    }
}
