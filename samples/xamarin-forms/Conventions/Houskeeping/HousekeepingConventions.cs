// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Conventional;
using App;
using Services;
using Services.Connected;
using Services.Disconnected;
using Tests;
using Utility;
using Features;
using Xunit;
using Utility;
using Services;
using Tests.Utility;

namespace Conventions.Houskeeping
{
    // When we want to contribute some code to an existing codebase, we should do so in a way that
    // aligns with the codebases existing conventions. Nothing makes navigating anything beyond
    // trivial codebases harder than poor organisation, imposing a large cognitive overhead and makes
    // contributing code an exercise in guesswork and frustration. Here we instruct the codebase to
    // enforce it's own housekeeping rules to ensure new contributors can get up to speed easily and
    // extend the codebase with minimal brainpower expended on trivialities like where to put X or
    // what to name Y.
    public class HousekeepingConventions : IClassFixture<BaseFixture>
    {
        private readonly BaseFixture _baseFixture;

        public HousekeepingConventions(BaseFixture baseFixture)
        {
            _baseFixture = baseFixture;
        }

        [Fact]
        public void AppAssemblyMustNotTakeADependancyOn()
        {
            typeof(AppAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(FeaturesAssembly), "TODO"));
            typeof(AppAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsAssembly), "TODO"));
            typeof(AppAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsUtilityAssembly), "TODO"));
        }

        [Fact]
        public void AssembliesMustNotReferenceDllsFromBinOrObjDirectories()
        {
            _baseFixture.ApplicationAssemblies.Select(
                x => x.Assembly.MustConformTo(Convention.MustNotReferenceDllsFromBinOrObjDirectories));
        }


        [Fact]
        public void ServicesAssemblyMustNotTakeADependancyOn()
        {
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(AppAssembly), "TODO"));
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(ServicesConnectedAssembly), "TODO"));
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(ServicesDisconnectedAssembly), "TODO"));
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(FeaturesAssembly), "TODO"));
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsAssembly), "TODO"));
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsUtilityAssembly), "TODO"));
        }

        [Fact]
        public void ServicesConnectedAssemblyMustNotTakeADependancyOn()
        {
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(AppAssembly), "TODO"));
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(ServicesDisconnectedAssembly), "TODO"));
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(FeaturesAssembly), "TODO"));
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsAssembly), "TODO"));
            typeof(ServicesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsUtilityAssembly), "TODO"));
        }

        [Fact]
        public void ServicesDisconnectedAssemblyMustNotTakeADependancyOn()
        {
            typeof(ServicesDisconnectedAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(AppAssembly), "TODO"));
            typeof(ServicesDisconnectedAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(ServicesConnectedAssembly), "TODO"));
            typeof(ServicesDisconnectedAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(FeaturesAssembly), "TODO"));
            typeof(ServicesDisconnectedAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsAssembly), "TODO"));
            typeof(ServicesDisconnectedAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsUtilityAssembly), "TODO"));
        }

        [Fact]
        public void UtilityAssemblyMustNotTakeADependancyOn()
        {
            typeof(UtilityAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(AppAssembly), "TODO"));
            typeof(UtilityAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(ServicesAssembly), "TODO"));
            typeof(UtilityAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(ServicesConnectedAssembly), "TODO"));
            typeof(UtilityAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(ServicesDisconnectedAssembly), "TODO"));
            typeof(UtilityAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(FeaturesAssembly), "TODO"));
            typeof(UtilityAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsAssembly), "TODO"));
            typeof(UtilityAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsUtilityAssembly), "TODO"));
        }

        [Fact]
        public void ViewModelsAssemblyMustNotTakeADependancyOn()
        {
            typeof(FeaturesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(AppAssembly), "TODO"));
            typeof(FeaturesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(ServicesConnectedAssembly), "TODO"));
            typeof(FeaturesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(ServicesDisconnectedAssembly), "TODO"));
            typeof(FeaturesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsAssembly), "TODO"));
            typeof(FeaturesAssembly).MustConformTo(Convention.MustNotTakeADependencyOn(typeof(TestsUtilityAssembly), "TODO"));
        }
    }
}
