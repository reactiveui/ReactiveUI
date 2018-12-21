using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using ReactiveUI;

namespace ReactiveUI.Benchmarks
{
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class RoutableViewModelMixinsBenchmarks
    {
        private Func<MockViewModel> _mockViewModel;
        private RoutingState _router;

        [GlobalSetup]
        public void Setup()
        {
            _router = new RoutingState(ImmediateScheduler.Instance);
            _mockViewModel = () => new MockViewModel();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _router = null;
            _mockViewModel = null;
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _router.NavigationStack.Clear();
        }

        [Benchmark]
        public void WhenNavigatedToObservable()
        {
            using (_mockViewModel().WhenNavigatedToObservable().Subscribe(x => Console.WriteLine("Observed")))
            using (_router.Navigate.Execute(_mockViewModel()).Subscribe())
            {
            }
        }

        [Benchmark]
        public void WhenNavigatingFromObservable()
        {
           using (_router.Navigate.Execute(_mockViewModel()).Subscribe())
           using (_mockViewModel().WhenNavigatingFromObservable().Subscribe())
           using (_router.NavigateBack.Execute().Subscribe()) 
           {
           }
        }
    }
}
