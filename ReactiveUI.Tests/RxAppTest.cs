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
        public void DispatcherSchedulerAssemblyStringMustBeCorrect()
        {
            Assert.Equal(typeof (DispatcherScheduler).AssemblyQualifiedName, RxApp.dispatcherSchedulerQualifiedName);
        }

        [Fact]
        public void DepPropNotifierShouldBeFound()
        {
            Assert.True(RxApp.GetAllServices<ICreatesObservableForProperty>()
                .Any(x => x is DependencyObjectObservableForProperty));
        }
    }
}
