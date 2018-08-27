using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DynamicData;

#pragma warning disable CS0618 // Item is obsolete warning

namespace ReactiveUI.Benchmarks.Legacy
{
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ReactiveListOperationBenchmark
    {
        private ReactiveList<string> _reactiveList;

        [GlobalSetup]
        public void Setup()
        {
            _reactiveList = new ReactiveList<string>();
        }

        [IterationSetup]
        public void SetupIteration()
        {
            _reactiveList.Clear();
        }

        [GlobalCleanup]
        public void Teardown()
        {
            _reactiveList = null;
        }

        [Benchmark]
        public void Add() => _reactiveList.Add("ReactiveUI.Fody");

        [Benchmark]
        public void AddRange() => _reactiveList.AddRange(new[]
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

        [Benchmark]
        public void AddOrInsertRange() => _reactiveList.AddOrInsertRange(new[]
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

        [Benchmark]
        public void Insert() => _reactiveList.Insert(0, "ReactiveUI.Benchmarks");

        [Benchmark]
        public void RemoveItem() => _reactiveList.Remove("ReactiveUI");
    }
}
