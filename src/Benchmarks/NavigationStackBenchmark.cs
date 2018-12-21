using System;
using System.Linq;
using System.Reactive.Concurrency;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ReactiveUI.Benchmarks
{
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class NavigationStackBenchmark
    {
        private static readonly Func<MockViewModel> _mockViewModel;
        private RoutingState _router;

        static NavigationStackBenchmark()
        {
            _mockViewModel = () => new MockViewModel();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _router.NavigationStack.Clear();
            _router = null;
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _router.NavigationStack.Clear();
        }

        [Benchmark]
        public void Navigate()
        {
            using (_router.Navigate.Execute(_mockViewModel()).Subscribe())
            {
                _router.NavigationStack.ToList();
            }
        }

        [Benchmark]
        public void NavigateAndReset()
        {
            using (_router.NavigateAndReset.Execute(_mockViewModel()).Subscribe())
            {
                _router.NavigationStack.ToList();
            }
        }

        [Benchmark]
        public void NavigateBack()
        {
            using (_router.NavigateAndReset.Execute(_mockViewModel()).Subscribe())
            using (_router.Navigate.Execute(_mockViewModel()).Subscribe())
            using (_router.NavigateBack.Execute().Subscribe())
            {
            }
        }

        [Benchmark]
        public void NavigationStack()
        {
            using (_router.NavigateAndReset.Execute(_mockViewModel()).Subscribe())
            {
                _router.NavigationStack.ToList();
            }
        }

        [Benchmark]
        public void RoutingState() => new RoutingState();

        [GlobalSetup]
        public void Setup()
        {
            _router = new RoutingState(ImmediateScheduler.Instance);
        }
    }
}
