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
        [Fact]
        public void DepPropNotifierShouldBeFound()
        {
            Assert.True(RxApp.GetAllServices<ICreatesObservableForProperty>()
                .Any(x => x is DependencyObjectObservableForProperty));
        }

        [Fact]
        public void SchedulerShouldBeImmediateInTestRunner()
        {
            Console.WriteLine(RxApp.DeferredScheduler.GetType().FullName);
            Assert.Equal(Scheduler.Immediate, RxApp.DeferredScheduler);
        }
    }
}
