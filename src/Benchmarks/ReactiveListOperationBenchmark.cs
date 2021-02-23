// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DynamicData;

using ReactiveUI.Legacy;

#pragma warning disable CS0618 // Item is obsolete warning

namespace ReactiveUI.Benchmarks.Legacy
{
    /// <summary>
    /// Benchmarks associated with the ReactiveList object.
    /// </summary>
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ReactiveListOperationBenchmark
    {
        private ReactiveList<string> _reactiveList;

        /// <summary>
        /// Setup for all benchmark instances being run.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _reactiveList = new ReactiveList<string>();
        }

        /// <summary>
        /// Setup for each iteration of a benchmark.
        /// </summary>
        [IterationSetup]
        public void SetupIteration()
        {
            _reactiveList.Clear();
        }

        /// <summary>
        /// Cleanup after all benchmarks have completed.
        /// </summary>
        [GlobalCleanup]
        public void Teardown()
        {
            _reactiveList = null;
        }

        /// <summary>
        /// Benchmark for when adding a item to a ReactiveList.
        /// </summary>
        [Benchmark]
        public void Add() => _reactiveList.Add("ReactiveUI.Fody");

        /// <summary>
        /// Benchmark for when adding a range of items to a ReactiveList.
        /// </summary>
        [Benchmark]
        public void AddRange() => _reactiveList.AddRange(
            new[]
            {
                "ReactiveUI",
                "ReactiveUI.XamForms",
                "ReactiveUI.WPF",
                "ReactiveUI.Events",
                "ReactiveUI",
                "ReactiveUI.XamForms",
                "ReactiveUI.WPF",
                "ReactiveUI.Events"
            });

        /// <summary>
        /// Benchmark for when adding a range of items or inserting.
        /// </summary>
        [Benchmark]
        public void AddOrInsertRange() => _reactiveList.AddOrInsertRange(
            new[]
            {
                "ReactiveUI",
                "ReactiveUI.XamForms",
                "ReactiveUI.WPF",
                "ReactiveUI.Events",
                "ReactiveUI",
                "ReactiveUI.XamForms",
                "ReactiveUI.WPF",
                "ReactiveUI.Events"
            }, -1);

        /// <summary>
        /// Benchmark for inserting a item at the start.
        /// </summary>
        [Benchmark]
        public void Insert() => _reactiveList.Insert(0, "ReactiveUI.Benchmarks");

        /// <summary>
        /// Benchmark for when removing a item from the list.
        /// </summary>
        [Benchmark]
        public void RemoveItem() => _reactiveList.Remove("ReactiveUI");
    }
}
