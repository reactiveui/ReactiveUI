// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Legacy;

#pragma warning disable CS0618 // Item is obsolete warning

namespace ReactiveUI.Benchmarks.Legacy
{
    /// <summary>
    /// Creates bench marks to the deprecated ReactiveList. This allows us to compare to the DynamicData approach.
    /// </summary>
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class CreateReactiveListBenchmark
    {
        private static readonly string[] _testData =
        {
            "ReactiveUI",
            "ReactiveUI.XamForms",
            "ReactiveUI.WPF",
            "ReactiveUI.Events",
            "ReactiveUI",
            "ReactiveUI.XamForms",
            "ReactiveUI.WPF",
            "ReactiveUI.Events"
        };

        /// <summary>
        /// Benchmarks using a System.Collections.Generic.List class as our base benchmark.
        /// </summary>
        /// <returns>The created list.</returns>
        [Benchmark(Baseline = true)]
        public object CreateList() => new List<string>();

        /// <summary>
        /// Benchmark using the ObservableCollection class to compare against.
        /// </summary>
        /// <returns>The created list.</returns>
        [Benchmark]
        public object CreateObservableCollection() => new ObservableCollection<string>(_testData);

        /// <summary>
        /// Benchmark using the legacy ReactiveList interface.
        /// </summary>
        /// <returns>The cretaed list.</returns>
        [Benchmark]
        public object CreateReactiveList() => new ReactiveList<string>(_testData);

        /// <summary>
        /// Benchmark using the ObservableCollection class wrapped in a ReadOnlyObservableCollection.
        /// </summary>
        /// <returns>The created list.</returns>
        [Benchmark]
        public object CreateReadOnlyObservableList() =>
            new ReadOnlyObservableCollection<string>(new ObservableCollection<string>(_testData));

        /// <summary>
        /// Benchmark using the ObservableCollection class wrapped in a ReadOnlyObservableCollection.
        /// </summary>
        /// <returns>The created list.</returns>
        [Benchmark]
        public object CreateObservableCollectionExtended() => new ObservableCollectionExtended<string>(_testData);

        /// <summary>
        /// Benchmark using the source list class from DynamicData.
        /// </summary>
        /// <returns>The created list.</returns>
        [Benchmark]
        public object CreateSourceList() => new SourceList<string>().Connect().AsObservableList();
    }
}
