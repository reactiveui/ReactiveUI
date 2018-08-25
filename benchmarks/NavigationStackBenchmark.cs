using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ReactiveUI.Benchmarks
{
    [Config(typeof(Config))]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class NavigationStackBenchmark
    {
        private Func<MockViewModel> _mockViewModel;
        private RoutingState _router;

        [GlobalCleanup]
        public void Cleanup()
        {
            _router = null;
            _mockViewModel = null;
        }

        [Benchmark]
        public object Navigate() => _router.Navigate.Execute(_mockViewModel()).Subscribe();

        [Benchmark]
        public object NavigateAndReset() => _router.NavigateAndReset.Execute(_mockViewModel()).Subscribe();

        [Benchmark]
        public object NavigateBack()
        {
            _router.NavigateAndReset.Execute(_mockViewModel()).Subscribe();
            return _router.NavigateBack.Execute().Subscribe();
        }

        [Benchmark]
        public object NavigationStack()
        {
            _router.NavigateAndReset.Execute(_mockViewModel()).Subscribe();
            return _router.NavigationStack.ToList();
        }

        [Benchmark]
        public object RoutingState() => new RoutingState();

        [GlobalSetup]
        public void Setup()
        {
            _router = new RoutingState();
            _mockViewModel = () => new MockViewModel();
        }
    }
}
