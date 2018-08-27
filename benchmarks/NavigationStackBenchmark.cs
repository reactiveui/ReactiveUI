using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ReactiveUI.Benchmarks
{
    [Config(typeof(Config))]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    //[MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class NavigationStackBenchmark
    {
        private Func<MockViewModel> _mockViewModel;
        private RoutingState _router;

        [GlobalCleanup]
        public void Cleanup()
        {
            _router.NavigationStack.Clear();
            _router = null;
            _mockViewModel = null;
        }

        [Benchmark]
        public void Navigate()
        {
            var disposable = _router.Navigate.Execute(_mockViewModel()).Subscribe();
            disposable.Dispose();
        }

        [Benchmark]
        public void NavigateAndReset()
        {
            var disposable = _router.NavigateAndReset.Execute(_mockViewModel()).Subscribe();
            disposable?.Dispose();
        }

        [Benchmark]
        public void NavigateBack()
        {
            var disposable = _router.NavigateAndReset.Execute(_mockViewModel()).Subscribe();
            _router.NavigateBack.Execute().Subscribe();
            disposable?.Dispose();
        }

        [Benchmark]
        public void NavigationStack()
        {
            var disposable = _router.NavigateAndReset.Execute(_mockViewModel()).Subscribe();
            _router.NavigationStack.ToList();
            disposable.Dispose();
        }

        [Benchmark]
        public void RoutingState() => new RoutingState();

        [GlobalSetup]
        public void Setup()
        {
            _router = new RoutingState();
            _mockViewModel = () => new MockViewModel();
        }
    }
}
