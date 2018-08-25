using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.InProcess;

namespace ReactiveUI.Benchmarks
{
    public class Config : ManualConfig
    {
        public Config()
        {
            Add(Job.MediumRun
                .WithLaunchCount(1)
                .With(runtime: Runtime.Clr)
                .WithId("Clr"));

            Add(Job.MediumRun
                .With(Runtime.Core)
                .WithLaunchCount(1)
                .WithId("Core"));

            Add(Job.MediumRun
                .WithLaunchCount(1)
                .With(InProcessToolchain.Instance)
                .WithId("InProcess"));
        }
    }
}
