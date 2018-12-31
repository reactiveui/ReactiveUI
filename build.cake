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
#tool "nuget:?package=coveralls.io&version=1.4.2"
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
var testCoverageOutputFile = MakeAbsolute(File(testsArtifactDirectory + "OpenCover.xml"));

// Whitelisted Packages
var packageWhitelist = new[] { "ReactiveUI.Testing",
                               "ReactiveUI.Events",
                               "ReactiveUI.Events.WPF",
                               "ReactiveUI.Events.Winforms",
                               "ReactiveUI.Events.XamForms",
                               "ReactiveUI",
                               "ReactiveUI.Fody",
                               "ReactiveUI.Fody.Helpers",
                               "ReactiveUI.AndroidSupport",
                               "ReactiveUI.Blend",
                               "ReactiveUI.WPF",
                               "ReactiveUI.Winforms",
                               "ReactiveUI.XamForms",
                                // TODO: seems the leak tests never worked as part of the CI, fix. For the moment just make sure it compiles.
                               "ReactiveUI.LeakTests" };

var packageTestWhitelist = new[] { "ReactiveUI.Tests", 
                                   "ReactiveUI.Fody.Tests" };

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
    CreateDirectory(testsArtifactDirectory);
});

Teardown(context =>
{
    // Executed AFTER the last task.
});


//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////
Action<string, string, bool, bool> Build = (solution, outputFolder, createPackage, forceUseFullDebugType) =>
{
    Information("Building {0} using {1}, createPackage = {2}, forceUseFullDebugType = {3}", solution, msBuildPath, createPackage, forceUseFullDebugType);

    var msBuildSettings = new MSBuildSettings() {
            ToolPath = msBuildPath,
            ArgumentCustomization = args => args.Append("/m /NoWarn:VSX1000")
        }
        .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
        .SetConfiguration("Release")                        
        .SetVerbosity(Verbosity.Minimal)
        .SetNodeReuse(false);

    if (forceUseFullDebugType)
    {
        msBuildSettings = msBuildSettings.WithProperty("DebugType",  "full");
    }

    if (createPackage)
    {
        msBuildSettings = msBuildSettings.WithProperty("PackageOutputPath",  MakeAbsolute(Directory(outputFolder)).ToString().Quote()).WithTarget("build;pack");
    }
    else
    {
        msBuildSettings = msBuildSettings.WithTarget("build");
        if (outputFolder != null)
        {
            msBuildSettings = msBuildSettings.WithProperty("OutputPath",  MakeAbsolute(Directory(outputFolder)).ToString().Quote());
        }
    }

    MSBuild(solution, msBuildSettings);
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("RestoreNuGet")
    .Does(() =>
{
    var settings = new NuGetRestoreSettings() {
        ArgumentCustomization = args => args.Append($"-MSBuildPath {MakeAbsolute(msBuildPath.GetDirectory()).ToString().Quote()}"),
        Verbosity = NuGetVerbosity.Quiet,
    };

    var solutions = GetFiles("./src/**/*.sln");
    NuGetRestore(solutions, settings);
});

Task("BuildEventBuilder")
    .IsDependentOn("RestoreNuGet")
    .Does(() =>
{
    Build("./src/EventBuilder.sln", artifactDirectory + "eventbuilder", false, false);
});

Task("GenerateEvents")
    .IsDependentOn("BuildEventBuilder")
    .Does (() =>
{
    var workingDirectory = artifactDirectory + "eventbuilder/";
    var eventBuilder = workingDirectory + "EventBuilder.dll";
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

    generate("android", "src/ReactiveUI.Events/");
    generate("ios", "src/ReactiveUI.Events/");
    generate("mac", "src/ReactiveUI.Events/");
    generate("uwp", "src/ReactiveUI.Events/");
    generate("tizen4", "src/ReactiveUI.Events/");
    generate("wpf", "src/ReactiveUI.Events.WPF/");
    generate("xamforms", "src/ReactiveUI.Events.XamForms/");
    generate("winforms", "src/ReactiveUI.Events.Winforms/");
    generate("essentials", "src/ReactiveUI.Events/");
    generate("tvos", "src/ReactiveUI.Events/");
});

Task("BuildReactiveUI")
    .IsDependentOn("GenerateEvents")
    .Does (() =>
{
    foreach(var package in packageWhitelist)
    {
        Build("./src/" + package + "/" + package + ".csproj", artifactDirectory, true, false);
    }
});

Task("RunUnitTests")
    .IsDependentOn("BuildReactiveUI")
    .Does(() =>
{
    // Clean the directories since we'll need to re-generate the debug type.
    CleanDirectories("./src/**/obj/Release");
    CleanDirectories("./src/**/bin/Release");

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
    };

    foreach (var projectName in packageTestWhitelist)
    {
        OpenCover(tool => 
        {
            Build("./src/" + projectName + "/" + projectName + ".csproj", null, false, true);

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
    .IsDependentOn("BuildReactiveUI")
    .IsDependentOn("RunUnitTests")
    .IsDependentOn("UploadTestCoverage")
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
