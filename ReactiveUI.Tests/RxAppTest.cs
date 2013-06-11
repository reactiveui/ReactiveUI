using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using ReactiveUI.Xaml;
using Xunit;

namespace ReactiveUI.Tests
{
    public class RxAppTest
    {
#if !MONO
        [Fact]
        public void DepPropNotifierShouldBeFound()
        {
            Assert.True(RxApp.DependencyResolver.GetServices<ICreatesObservableForProperty>()
                .Any(x => x is DependencyObjectObservableForProperty));
        }
#endif

        [Fact]
        public void SchedulerShouldBeCurrentThreadInTestRunner()
        {
            Console.WriteLine(RxApp.MainThreadScheduler.GetType().FullName);
            Assert.Equal(CurrentThreadScheduler.Instance, RxApp.MainThreadScheduler);
        }

        [Fact]
        public void UnitTestDetectorIdentifiesThisTestAsAnXUnitTest()
        {
            string[] testAssemblies = new[] {
                "CSUNIT",
                "NUNIT",
                "XUNIT",
                "MBUNIT",
                "TESTDRIVEN",
                "QUALITYTOOLS.TIPS.UNITTEST.ADAPTER",
                "QUALITYTOOLS.UNITTESTING.SILVERLIGHT",
                "PEX",
                "MSBUILD",
                "NBEHAVE",
                "TESTPLATFORM",
            };

            string[] designEnvironments = new[] {
                "BLEND.EXE",
                "MONODEVELOP",
                "SHARPDEVELOP.EXE",
            };

            var isInUnitTestRunner = PlatformUnitTestDetector.InUnitTestRunner(testAssemblies, designEnvironments);

            Assert.True(isInUnitTestRunner);
        }

        [Fact]
        public void UnitTestDetectorDoesNotIdentifyThisTestWhenXUnitAssemblyNotChecked()
        {
            // XUnit assembly name removed
            string[] testAssembliesWithoutNunit = new[] {
                "CSUNIT",
                "NUNIT",
                "MBUNIT",
                "TESTDRIVEN",
                "QUALITYTOOLS.TIPS.UNITTEST.ADAPTER",
                "QUALITYTOOLS.UNITTESTING.SILVERLIGHT",
                "PEX",
                "MSBUILD",
                "NBEHAVE",
                "TESTPLATFORM",
            };

            string[] designEnvironments = new[] {
                "BLEND.EXE",
                "MONODEVELOP",
                "SHARPDEVELOP.EXE",
            };

            var isInUnitTestRunner = PlatformUnitTestDetector.InUnitTestRunner(testAssembliesWithoutNunit, designEnvironments);

            Assert.False(isInUnitTestRunner);
        }
    }
}
