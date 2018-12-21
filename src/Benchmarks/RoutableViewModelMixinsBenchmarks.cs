using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using ReactiveUI;

namespace ReactiveUI.Benchmarks
{
    /// <summary>
    /// Benchmarking the Mixin class for the RoutableViewModel classes.
    /// </summary>
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class RoutableViewModelMixinsBenchmarks
    {
        private Func<MockViewModel> _mockViewModel;
        private RoutingState _router;

        /// <summary>
        /// Setup before all benchmarks are run.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _router = new RoutingState(ImmediateScheduler.Instance);
            _mockViewModel = () => new MockViewModel();
        }

        /// <summary>
        /// Cleanup after all benchmark iterations have been completed.
        /// </summary>
        [GlobalCleanup]
        public void Cleanup()
        {
            _router = null;
            _mockViewModel = null;
        }

        /// <summary>
        /// Setup before each benchmark iteration starts.
        /// </summary>
        [IterationSetup]
        public void IterationSetup()
        {
            _router.NavigationStack.Clear();
        }

        /// <summary>
        /// Benchmark for getting and using the NavigatedTo observable.
        /// </summary>
        [Benchmark]
        public void WhenNavigatedToObservable()
        {
            using (_mockViewModel().WhenNavigatedToObservable().Subscribe(x => Console.WriteLine("Observed")))
            using (_router.Navigate.Execute(_mockViewModel()).Subscribe())
            {
            }
        }

        /// <summary>
        /// Benchmark for getting and using the NavigateFrom observable.
        /// </summary>
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
