using System;
using BenchmarkDotNet.Running;
using Xunit;

namespace ReactiveUI.Benchmarks
{
    public class Harness
    {
        [Fact]
        public void Auto_Persist_Benchmark()
        {
            BenchmarkRunner.Run<AutoPersistBenchmark>();
        }

        [Fact]
        public void Create_Reactive_List_Benchmark()
        {
            BenchmarkRunner.Run<CreateReactiveListBenchmark>();
        }

        [Fact]
        public void Reactive_List_Operation_Benchmark()
        {
            BenchmarkRunner.Run<ReactiveListOperationBenchmark>();
        }

        [Fact]
        public void Navigation_Reactive_List_Benchmark()
        {
            BenchmarkRunner.Run<NavigationStackBenchmark>();
        }

        [Fact]
        public void Routable_View_Model_Mixin_Benchmark()
        {
            BenchmarkRunner.Run<RoutableViewModelMixinsBenchmarks>();
        }

        [Fact]
        public void INPC_Observable_For_Property_Benchmarks()
        {
            BenchmarkRunner.Run<INPCObservableForPropertyBenchmarks>();
        }
    }
}
