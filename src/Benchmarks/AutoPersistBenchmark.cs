using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ReactiveUI.Benchmarks
{
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class AutoPersistBenchmark
    {
        private ObservableCollection<MockViewModel> _collection;

        [GlobalSetup]
        public void Setup()
        {
            _collection = new ObservableCollection<MockViewModel>(new[]
            {
                new MockViewModel(),
                new MockViewModel(),
                new MockViewModel(),
                new MockViewModel(),
                new MockViewModel(),
            });
        }

        [Benchmark]
        public void AutoPersistCollection()
        {
            var disposable = _collection.AutoPersistCollection(x => Observable.Create<Unit>(_ => 
                {
                    Console.WriteLine("Done stuff");
                    return Disposable.Empty;
                }).Select(_ => Unit.Default), TimeSpan.FromMilliseconds(200));

            for (int i = 0; i < 5; ++i)
            {
                _collection.Add(new MockViewModel());
            }

            _collection.Clear();

            disposable.Dispose();
        }
    }
}
