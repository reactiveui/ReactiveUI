﻿using System;
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

            var isInUnitTestRunner = RealUnitTestDetector.InUnitTestRunner(testAssemblies, designEnvironments);

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

            var isInUnitTestRunner = RealUnitTestDetector.InUnitTestRunner(testAssembliesWithoutNunit, designEnvironments);

            Assert.False(isInUnitTestRunner);
        }
        
        [Fact]
        public void ReferenceDetectorReturnsReferencesForAssembliesLoadedIntoAppdomain()
        {
            var expectedAssembly = typeof(RxApp).Assembly.GetName().FullName;
            var assemblyReferences = RxApp.buildListOfReferencedAssemblies();

            Assert.True(assemblyReferences.Any(x => x.FullName.Equals(expectedAssembly, StringComparison.Ordinal)));
        }

        [Fact]
        public void ReferenceDetectorCanFilterReferences()
        {
            var expectedAssembly = typeof(RxApp).Assembly.GetName().FullName;
            var assemblyReferences = RxApp.buildListOfReferencedAssemblies(name => name.FullName.Equals(expectedAssembly));

            Assert.Equal(assemblyReferences.Count(), 1);
        }
    }
}
