using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using DynamicData;

namespace ReactiveUI.Benchmarks
{
    [CoreJob]
    [MarkdownExporterAttribute.GitHub]
    public class CreateReactiveListBenchmark
    {
        [Benchmark(Baseline = true)]
        public object CreateList() => new List<string>();

        [Benchmark]
        public object CreateObservableCollection() => new ObservableCollection<string>();

        [Benchmark]
        public object CreateReactiveList() => new ReactiveList<string>();

        [Benchmark]
        public object CreateReactiveListFromList() => new ReactiveList<string>(new string[]
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
        public object CreateReadOnlyObservableList() =>
            new ReadOnlyObservableCollection<string>(new ObservableCollection<string>(Enumerable.Empty<string>()));

        [Benchmark]
        public object CreateSourceList() => new SourceList<string>().Connect().AsObservableList();
    }
}
