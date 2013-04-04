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
            Assert.True(RxApp.GetAllServices<ICreatesObservableForProperty>()
                .Any(x => x is DependencyObjectObservableForProperty));
        }
#endif

        [Fact]
        public void SchedulerShouldBeImmediateInTestRunner()
        {
            Console.WriteLine(RxApp.DeferredScheduler.GetType().FullName);
            Assert.Equal(Scheduler.Immediate, RxApp.DeferredScheduler);
        }

        [Fact]
        public void UnitTestDetectorIdentifiesThisTestAsAnXUnitTest()
        {
            var isInUnitTestRunner = UnitTestDetector.IsInUnitTestRunner();

            Assert.True(isInUnitTestRunner);
        }

        [Fact]
        public void UnitTestDetectorDoesNotIdentifyThisTestWhenXUnitAssemblyNotChecked()
        {
            string vsUnitTest = "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute, Microsoft.VisualStudio.QualityTools.UnitTestFramework";

            var isInUnitTestRunner = UnitTestDetector.IsInUnitTestRunner(vsUnitTest);

            Assert.False(isInUnitTestRunner);
        }
    }
}
