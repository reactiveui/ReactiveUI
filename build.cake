// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Cake.FileHelpers&version=3.1.0"
#addin "nuget:?package=Cake.Coveralls&version=0.9.0"
#addin "nuget:?package=Cake.PinNuGetDependency&loaddependencies=true&version=3.2.3"
#addin "nuget:?package=Cake.Powershell&version=0.4.7"
#addin "nuget:?package=Cake.Codecov&version=0.5.0"

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=GitReleaseManager&version=0.7.1"
#tool "nuget:?package=OpenCover&version=4.6.519"
#tool "nuget:?package=ReportGenerator&version=4.0.4"
#tool "nuget:?package=vswhere&version=2.5.2"
#tool "nuget:?package=xunit.runner.console&version=2.4.1"
#tool "nuget:?package=Codecov&version=1.1.0"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

var includePrerelease = Argument("includePrerelease", false);
var vsLocationString = Argument("vsLocation", string.Empty);
var msBuildPathString = Argument("msBuildPath", string.Empty);

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Should MSBuild treat any errors as warnings?
var treatWarningsAsErrors = false;

// Build configuration
var local = BuildSystem.IsLocalBuild;
var isPullRequest = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SYSTEM_PULLREQUEST_PULLREQUESTNUMBER"));
var isRepository = StringComparer.OrdinalIgnoreCase.Equals("reactiveui/reactiveui", TFBuild.Environment.Repository.RepoName);

var vsWhereSettings = new VSWhereLatestSettings() { IncludePrerelease = includePrerelease };
var vsLocation = string.IsNullOrWhiteSpace(vsLocationString) ? VSWhereLatest(vsWhereSettings) : new DirectoryPath(vsLocationString);
var msBuildPath = string.IsNullOrWhiteSpace(msBuildPathString) ? vsLocation.CombineWithFilePath("./MSBuild/15.0/Bin/MSBuild.exe") : new FilePath(msBuildPathString);

var informationalVersion = EnvironmentVariable("GitAssemblyInformationalVersion");

// Artifacts
var artifactDirectory = "./artifacts/";
var testsArtifactDirectory = artifactDirectory + "tests/";
var eventsArtifactDirectory = artifactDirectory + "Events/";
var binariesArtifactDirectory = artifactDirectory + "binaries/";
var packagesArtifactDirectory = artifactDirectory + "packages/";

// OpenCover file location
var testCoverageOutputFile = MakeAbsolute(File(testsArtifactDirectory + "OpenCover.xml"));

// Whitelisted Packages
var packageWhitelist = new[] 
{ 
    "ReactiveUI",
    "ReactiveUI.Testing",
    "ReactiveUI.Events",
    "ReactiveUI.Events.WPF",
    "ReactiveUI.Events.Winforms",
    "ReactiveUI.Events.XamEssentials",
    "ReactiveUI.Events.XamForms",
    "ReactiveUI.Fody",
    "ReactiveUI.Fody.Helpers",
    "ReactiveUI.AndroidSupport",
    "ReactiveUI.Blend",
    "ReactiveUI.WPF",
    "ReactiveUI.Winforms",
    "ReactiveUI.XamForms",
    // TODO: seems the leak tests never worked as part of the CI, fix. For the moment just make sure it compiles.
    "ReactiveUI.LeakTests"
};

var packageTestWhitelist = new[]
{
    "ReactiveUI.Tests", 
    "ReactiveUI.Fody.Tests"
};

(string targetName, string destination)[] eventGenerators = new[]
{
    ("android", "src/ReactiveUI.Events/"),
    ("ios", "src/ReactiveUI.Events/"),
    ("mac", "src/ReactiveUI.Events/"),
    ("uwp", "src/ReactiveUI.Events/"),
    ("tizen4", "src/ReactiveUI.Events/"),
    ("wpf", "src/ReactiveUI.Events.WPF/"),
    ("xamforms", "src/ReactiveUI.Events.XamForms/"),
    ("winforms", "src/ReactiveUI.Events.Winforms/"),
    ("essentials", "src/ReactiveUI.Events.XamEssentials/"),
    ("tvos", "src/ReactiveUI.Events/"),
};

// Define global marcos.
Action Abort = () => { throw new Exception("a non-recoverable fatal error occurred."); };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    if (!IsRunningOnWindows())
    {
        throw new NotImplementedException("ReactiveUI will only build on Windows (w/Xamarin installed) because it's not possible to target UWP, WPF and Windows Forms from UNIX.");
    }

    Information("Building version {0} of ReactiveUI.", informationalVersion);

    CreateDirectory(artifactDirectory);
    CleanDirectories(artifactDirectory);
    CreateDirectory(testsArtifactDirectory);
    CreateDirectory(eventsArtifactDirectory);
    CreateDirectory(binariesArtifactDirectory);
    CreateDirectory(packagesArtifactDirectory);
});

Teardown(context =>
{
    // Executed AFTER the last task.
});


//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////
Action<string, string, bool, bool> Build = (solution, packageOutputPath, createPackage, forceUseFullDebugType) =>
{
    Information("Building {0} using {1}, createPackage = {2}, forceUseFullDebugType = {3}", solution, msBuildPath, createPackage, forceUseFullDebugType);

    var msBuildSettings = new MSBuildSettings() {
            ToolPath = msBuildPath,
            ArgumentCustomization = args => args.Append("/m /NoWarn:VSX1000"),
            NodeReuse = false,
            Restore = true
        }
        .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
        .SetConfiguration("Release")                        
        .SetVerbosity(Verbosity.Minimal);

    if (forceUseFullDebugType)
    {
        msBuildSettings = msBuildSettings.WithProperty("DebugType",  "full");
    }

    if (createPackage)
    {
        if (!string.IsNullOrWhiteSpace(packageOutputPath))
        {
            msBuildSettings = msBuildSettings.WithProperty("PackageOutputPath",  MakeAbsolute(Directory(packageOutputPath)).ToString().Quote());
        }
        
        msBuildSettings = msBuildSettings.WithTarget("build;pack");
    }
    else
    {
        msBuildSettings = msBuildSettings.WithTarget("build");
    }

    MSBuild(solution, msBuildSettings);
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("BuildEventBuilder")
    .Does(() =>
{
    Build("./src/EventBuilder.sln", artifactDirectory + "eventbuilder", false, false);
});

Task("GenerateEvents")
    .IsDependentOn("BuildEventBuilder")
    .Does (() =>
{
    var workingDirectory = MakeAbsolute(Directory("./src/EventBuilder/bin/Release/netcoreapp2.1"));
    var eventBuilder = workingDirectory + "/EventBuilder.dll";
    var referenceAssembliesPath = vsLocation.CombineWithFilePath("./Common7/IDE/ReferenceAssemblies/Microsoft/Framework");

    Information(referenceAssembliesPath.ToString());

    Action<string, string> generate = (string platform, string directory) =>
    {
        var settings = new DotNetCoreExecuteSettings
        {
            WorkingDirectory = workingDirectory,
        };

        var filename = String.Format("Events_{0}.cs", platform);
        var output = MakeAbsolute(File(System.IO.Path.Combine(directory, filename))).ToString().Quote();

        Information("Generating events for '{0}'", platform);
        DotNetCoreExecute(
                    eventBuilder,
                    new ProcessArgumentBuilder()
                        .AppendSwitch("--platform","=", platform)
                        .AppendSwitchQuoted("--reference","=", referenceAssembliesPath.ToString())
                        .AppendSwitchQuoted("--output-path", "=", output),
                    settings);

        Information("The events have been written to '{0}'", output);
    };

    Parallel.ForEach(eventGenerators, arg => generate(arg.targetName, arg.destination));

    CopyFiles(GetFiles("./src/ReactiveUI.**/Events_*.cs"), Directory(eventsArtifactDirectory));
});

Task("BuildReactiveUIPackages")
    .IsDependentOn("GenerateEvents")
    .Does (() =>
{
    // Clean the directories since we'll need to re-generate the debug type.
    CleanDirectories("./src/**/obj/Release");
    CleanDirectories("./src/**/bin/Release");

    foreach(var package in packageWhitelist)
    {
        Build("./src/" + package + "/" + package + ".csproj", packagesArtifactDirectory, true, false);
    }

    CopyFiles(GetFiles("./src/**/bin/Release/**/*"), Directory(binariesArtifactDirectory), true);
});

Task("RunUnitTests")
    .IsDependentOn("GenerateEvents")
    .Does(() =>
{
    var fodyPackages = new string[]
    {
        "ReactiveUI.Fody",
        "ReactiveUI.Fody.Helpers",
    };       

    // Clean the directories since we'll need to re-generate the debug type.
    CleanDirectories("./src/**/obj/Release");
    CleanDirectories("./src/**/bin/Release");

    foreach (var package in fodyPackages)
    {
        Build("./src/" + package + "/" + package + ".csproj", null, true, true);
    }

    var openCoverSettings =  new OpenCoverSettings {
            ReturnTargetCodeOffset = 0,
            MergeOutput = true,
        }
        .WithFilter("+[*]*")
        .WithFilter("-[*.Testing]*")
        .WithFilter("-[*.Tests*]*")
        .WithFilter("-[ReactiveUI.Events]*")
        .WithFilter("-[Splat*]*")
        .ExcludeByAttribute("*.ExcludeFromCodeCoverage*")
        .ExcludeByFile("*/*Designer.cs")
        .ExcludeByFile("*/*.g.cs")
        .ExcludeByFile("*/*.g.i.cs")
        .ExcludeByFile("*splat/splat*")
        .ExcludeByFile("*ApprovalTests*");

    var xunitSettings = new XUnit2Settings {
        HtmlReport = true,
        OutputDirectory = testsArtifactDirectory,
        NoAppDomain = true
    };

    foreach (var projectName in packageTestWhitelist)
    {
        OpenCover(tool => 
        {
            Build("./src/" + projectName + "/" + projectName + ".csproj", null, true, true);

            tool.XUnit2("./src/" + projectName + "/bin/" + "**/*.Tests.dll", xunitSettings);
        },
        testCoverageOutputFile,
        openCoverSettings);
    }

    ReportGenerator(testCoverageOutputFile, testsArtifactDirectory + "Report/");
}).ReportError(exception =>
{
    var apiApprovals = GetFiles("./**/ApiApprovalTests.*");
    CopyFiles(apiApprovals, artifactDirectory);
});

Task("UploadTestCoverage")
    .WithCriteria(() => !local)
    .WithCriteria(() => isRepository)
    .IsDependentOn("RunUnitTests")
    .Does(() =>
{
    // Resolve the API key.
    var token = EnvironmentVariable("CODECOV_TOKEN");
    if (!string.IsNullOrEmpty(token))
    {
        Information("Upload {0} to Codecov server", testCoverageOutputFile);

        // Upload a coverage report.
        Codecov(testCoverageOutputFile.ToString(), token);
    }
});

Task("SignPackages")
    .WithCriteria(() => !local)
    .WithCriteria(() => !isPullRequest)
    .Does(() =>
{
    StartPowershellFile("./SignPackages.ps1", args =>
    {
    });
});

Task("Package")
    .IsDependentOn("BuildReactiveUIPackages")
    .IsDependentOn("PinNuGetDependencies")
    .IsDependentOn("SignPackages")
    .Does (() =>
{
});

Task("PinNuGetDependencies")
    .Does (() =>
{
    var packages = GetFiles(artifactDirectory + "*.nupkg");
    foreach(var package in packages)
    {
        // only pin whitelisted packages.
        if(packageWhitelist.Any(p => package.GetFilename().ToString().StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            // see https://github.com/cake-contrib/Cake.PinNuGetDependency
            PinNuGetDependency(package, "ReactiveUI");
        }
    }
});



//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package")
    .Does (() =>
{
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
