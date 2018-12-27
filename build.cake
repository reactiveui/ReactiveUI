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

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=GitReleaseManager&version=0.7.1"
#tool "nuget:?package=coveralls.io&version=1.4.2"
#tool "nuget:?package=OpenCover&version=4.6.519"
#tool "nuget:?package=ReportGenerator&version=4.0.4"
#tool "nuget:?package=vswhere&version=2.5.2"
#tool "nuget:?package=xunit.runner.console&version=2.4.1"

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
var testCoverageOutputFile = testsArtifactDirectory + "OpenCover.xml";
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
                               "ReactiveUI.XamForms" };

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
    Information("Building {0} using {1}, createPackage = {2}", solution, msBuildPath, createPackage);

    var msBuildSettings = new MSBuildSettings() {
            ToolPath = msBuildPath,
            ArgumentCustomization = args => args.Append("/m /restore")
        }
        .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
        .SetConfiguration("Release")                        
        .SetVerbosity(Verbosity.Minimal)
        .SetNodeReuse(false);

    if (createPackage)
    {
        msBuildSettings = msBuildSettings.WithProperty("PackageOutputPath",  MakeAbsolute(Directory(outputFolder)).ToString().Quote()).WithTarget("build;pack");
    }
    else
    {
        msBuildSettings = msBuildSettings.WithProperty("OutputPath",  MakeAbsolute(Directory(outputFolder)).ToString().Quote()).WithTarget("build");
    }

    if (forceUseFullDebugType)
    {
        msBuildSettings = msBuildSettings.WithProperty("DebugType",  "full");
    }

    MSBuild(solution, msBuildSettings);
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("BuildEventBuilder")
    .Does (() =>
{
    Build("./src/EventBuilder.sln", artifactDirectory + "eventbuilder", false, false);
});

Task("GenerateEvents")
    .IsDependentOn("BuildEventBuilder")
    .Does (() =>
{
    var workingDirectory = artifactDirectory + "eventbuilder/";
    var eventBuilder = workingDirectory + "EventBuilder.exe";
    var referenceAssembliesPath = vsLocation.CombineWithFilePath("./Common7/IDE/ReferenceAssemblies/Microsoft/Framework");

    Information(referenceAssembliesPath.ToString());

    Action<string, string> generate = (string platform, string directory) =>
    {
        using(var process = StartAndReturnProcess(eventBuilder,
            new ProcessSettings{
                Arguments = new ProcessArgumentBuilder()
                    .AppendSwitch("--platform","=", platform)
                    .AppendSwitchQuoted("--reference","=", referenceAssembliesPath.ToString()),
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true }))
        {
            // super important to ensure that the platform is always
            // uppercase so that the events are written to the write
            // filename as UNIX is case-sensitive - even though OSX
            // isn't by default.
            platform = platform.ToUpper();

            Information("Generating events for '{0}'", platform);
            
            int timeout = 10 * 60 * 1000;   // x Minute, y Second, z Millisecond
            process.WaitForExit(timeout);

            var stdout = process.GetStandardOutput();

            int success = 0;    // exit code aka %ERRORLEVEL% or $?
            if (process.GetExitCode() != success)
            {
                Error("Failed to generate events for '{0}'", platform);

                foreach (var line in stdout)
                {
                    Error(line);
                }

                Abort();
            }

            var filename = String.Format("Events_{0}.cs", platform);
            var output = System.IO.Path.Combine(directory, filename);

            FileWriteLines(output, stdout.ToArray());
            Information("The events have been written to '{0}'", output);
        }
    };

    generate("android", "src/ReactiveUI.Events/");
    generate("ios", "src/ReactiveUI.Events/");
    generate("mac", "src/ReactiveUI.Events/");
    generate("uwp", "src/ReactiveUI.Events/");
    generate("tizen", "src/ReactiveUI.Events/");
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
    Action<ICakeContext, string> RunTests = (tool, projectName) => {
        var testsArtifactProjectDirectory = testsArtifactDirectory + projectName + "/";
        Build("./src/" + projectName + "/" + projectName + ".csproj", testsArtifactProjectDirectory, false, true);

        var xunitSettings = new XUnit2Settings {
            OutputDirectory = testsArtifactProjectDirectory,
            XmlReport = true,
            NoAppDomain = true
        };

        tool.XUnit2(testsArtifactProjectDirectory + "**/*.Tests.dll", xunitSettings);
    };

    var openCoverSettings =  new OpenCoverSettings {
            ReturnTargetCodeOffset = 0,
            ArgumentCustomization = args => args.Append("-mergeoutput")
        }
        .WithFilter("+[*]*")
        .WithFilter("-[*.Testing]*")
        .WithFilter("-[*.Tests*]*")
        .WithFilter("-[ReactiveUI.Events]*")
        .WithFilter("-[Splat*]*")
        .WithFilter("-[ApprovalTests*]*")
        .ExcludeByAttribute("*.ExcludeFromCodeCoverage*")
        .ExcludeByFile("*/*Designer.cs")
        .ExcludeByFile("*/*.g.cs")
        .ExcludeByFile("*/*.g.i.cs")
        .ExcludeByFile("*splat/splat*")
        .ExcludeByFile("*ApprovalTests*");

    OpenCover(tool => RunTests(tool, "ReactiveUI.Tests"), testCoverageOutputFile, openCoverSettings);
    OpenCover(tool => RunTests(tool, "ReactiveUI.Fody.Tests"), testCoverageOutputFile, openCoverSettings);
    // TODO: seems the leak tests never worked as part of the CI, fix. For the moment just make sure it compiles.
    // OpenCover(tool => RunTests(tool, "ReactiveUI.LeakTests"), testCoverageOutputFile, openCoverSettings);
    Build("./src/ReactiveUI.LeakTests/ReactiveUI.LeakTests.csproj", testsArtifactDirectory + "LeakTests/", false, true);

    ReportGenerator(testCoverageOutputFile, artifactDirectory);
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
    var token = EnvironmentVariable("COVERALLS_TOKEN");
    if (!string.IsNullOrEmpty(token))
    {
        CoverallsIo(testCoverageOutputFile, new CoverallsIoSettings()
        {
            RepoToken = token
        });
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
