using System;
using System.Reactive;
using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace ReactiveUI.Benchmarks
{
    [CoreJob]
    [MarkdownExporterAttribute.GitHub]
    public class AutoPersistBenchmark
    {
        private ReactiveList<string> _collection;

        [GlobalSetup]
        public void Setup()
        {
            _collection = new ReactiveList<string>(new[]
            {
                "ReactiveUI",
                "ReactiveUI.XamForms",
                "ReactiveUI.WPF",
                "ReactiveUI.Events"
            });
        }

        [Benchmark]
        public void AutoPersistCollection() => _collection.AutoPersist(x => Observable.Return(Unit.Default), TimeSpan.FromMilliseconds(200));
    }
}
