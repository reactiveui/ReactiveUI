using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using ReactiveUI;

namespace ReactiveUI.Benchmarks
{
    [CoreJob]
    [MarkdownExporterAttribute.GitHub]
    public class RoutableViewModelMixinsBenchmarks
    {
        private Func<MockViewModel> _mockViewModel;
        private RoutingState _router;

        [GlobalSetup]
        public void Setup()
        {
            _router = new RoutingState();
            _mockViewModel = () => new MockViewModel();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _router = null;
            _mockViewModel = null;
        }

        [Benchmark]
        public void WhenNavigatedToObservable()
        {
            _mockViewModel()
                .WhenNavigatedToObservable()
                .Subscribe(x =>
                {
                    Console.WriteLine("Observed");
                });

            _router.Navigate
                .Execute(_mockViewModel()).Subscribe();
        }

        [Benchmark]
        public void WhenNavigatingFromObservable()
        {
            _router.Navigate.Execute(_mockViewModel()).Subscribe();
            _mockViewModel().WhenNavigatingFromObservable().Subscribe();
            _router.NavigateBack.Execute().Subscribe();
        }
    }
}
