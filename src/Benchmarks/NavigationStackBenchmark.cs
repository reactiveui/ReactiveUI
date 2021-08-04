// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Concurrency;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ReactiveUI.Benchmarks
{
    /// <summary>
    /// Benchmarks for the NavigationStack and the RoutingState objects.
    /// </summary>
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class NavigationStackBenchmark
    {
        private static readonly Func<MockViewModel> _mockViewModel;
        private RoutingState? _router;

        /// <summary>
        /// Initializes static members of the <see cref="NavigationStackBenchmark"/> class.
        /// </summary>
        static NavigationStackBenchmark()
        {
            _mockViewModel = () => new MockViewModel();
        }

        /// <summary>
        /// Setup method for when running all bench marks.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _router = new RoutingState(ImmediateScheduler.Instance);
        }

        /// <summary>
        /// Performs the cleanup after all the benchmarks have been completed.
        /// </summary>
        [GlobalCleanup]
        public void Cleanup()
        {
            _router!.NavigationStack.Clear();
            _router = null;
        }

        /// <summary>
        /// Setup for each run of a benchmark.
        /// </summary>
        [IterationSetup]
        public void IterationSetup()
        {
            _router!.NavigationStack.Clear();
        }

        /// <summary>
        /// Benchmark for when navigating to a new view model.
        /// </summary>
        [Benchmark]
        public void Navigate()
        {
            using (_router!.Navigate.Execute(_mockViewModel()).Subscribe())
            {
                _router.NavigationStack.ToList();
            }
        }

        /// <summary>
        /// Benchmark for when navigating and resetting to a new view model.
        /// </summary>
        [Benchmark]
        public void NavigateAndReset()
        {
            using (_router!.NavigateAndReset.Execute(_mockViewModel()).Subscribe())
            {
                _router.NavigationStack.ToList();
            }
        }

        /// <summary>
        /// Benchmark for when navigating back from a view model.
        /// </summary>
        [Benchmark]
        public void NavigateBack()
        {
            using (_router!.NavigateAndReset.Execute(_mockViewModel()).Subscribe())
            using (_router.Navigate.Execute(_mockViewModel()).Subscribe())
            using (_router.NavigateBack.Execute().Subscribe())
            {
            }
        }

        /// <summary>
        /// Benchmark for when navigating and resetting the navigation stack.
        /// </summary>
        [Benchmark]
        public void NavigationStack()
        {
            using (_router!.NavigateAndReset.Execute(_mockViewModel()).Subscribe())
            {
                _router.NavigationStack.ToList();
            }
        }

        /// <summary>
        /// Benchmark creating our routing state.
        /// </summary>
        [SuppressMessage("usage", "CA1806: unused variable", Justification = "Used in the benchmark.")]
        [Benchmark]
        public void RoutingState() => new RoutingState();
    }
}
