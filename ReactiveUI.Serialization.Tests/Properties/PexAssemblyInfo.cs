// <copyright file="PexAssemblyInfo.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Moles;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;
using Microsoft.Pex.Linq;
using Microsoft.Pex.Framework.Using;
using ReactiveUI;

// Microsoft.Pex.Framework.Settings
//[assembly: PexAssemblySettings(TestFramework = "Xunit")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("ReactiveUI.Serialization")]
[assembly: PexInstrumentAssembly("Newtonsoft.Json")]
[assembly: PexInstrumentAssembly("System.CoreEx")]
[assembly: PexInstrumentAssembly("System.Reactive")]
[assembly: PexInstrumentAssembly("System.Core")]
[assembly: PexInstrumentAssembly("ReactiveUI")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.CoreEx")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Reactive")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Runtime.Serialization")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "ReactiveUI")]

// Microsoft.Pex.Framework.Moles
[assembly: PexAssumeContractEnsuresFailureAtBehavedSurface]
[assembly: PexChooseAsBehavedCurrentBehavior]

// Microsoft.Pex.Linq
[assembly: PexLinqPackage]

[assembly: PexInstrumentAssembly("ReactiveUI.Tests")]
//[assembly: PexUseType(typeof(StdErrLogger))]
