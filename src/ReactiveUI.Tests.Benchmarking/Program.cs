using System;
using BenchmarkDotNet.Running;

namespace ReactiveUI.Tests.Benchmarking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<INPCObservableForPropertyBenchmarks>();
        }
    }
}