// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Cake.FileHelpers&version=2.0.0"
#addin "nuget:?package=Cake.Coveralls&version=0.8.0"
#addin "nuget:?package=Cake.PinNuGetDependency&version=3.0.1"
#addin "nuget:?package=Cake.Powershell&version=0.4.3"

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=GitReleaseManager&version=0.7.0"
#tool "nuget:?package=coveralls.io&version=1.4.2"
#tool "nuget:?package=OpenCover&version=4.6.519"
#tool "nuget:?package=ReportGenerator&version=3.1.2"
#tool "nuget:?package=vswhere&version=2.4.1"
#tool "nuget:?package=xunit.runner.console&version=2.4.0-beta.2.build3984"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Should MSBuild treat any errors as warnings?
var treatWarningsAsErrors = false;

// Build configuration
var local = BuildSystem.IsLocalBuild;
var isPullRequest = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SYSTEM_PULLREQUEST_PULLREQUESTNUMBER"));
var isRepository = StringComparer.OrdinalIgnoreCase.Equals("reactiveui/reactiveui", TFBuild.Environment.Repository.RepoName);

var msBuildPath = VSWhereLatest().CombineWithFilePath("./MSBuild/15.0/Bin/MSBuild.exe");

var informationalVersion = EnvironmentVariable("GitAssemblyInformationalVersion");

// Artifacts
var artifactDirectory = "./artifacts/";
var testCoverageOutputFile = artifactDirectory + "OpenCover.xml";
var packageWhitelist = new[] { "ReactiveUI.Testing",
                               "ReactiveUI.Events",
                               "ReactiveUI.Events.WPF",
                               "ReactiveUI.Events.Winforms",
                               "ReactiveUI.Events.XamForms",
                               "ReactiveUI",
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
});

Teardown(context =>
{
    // Executed AFTER the last task.
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("BuildEventBuilder")
    .Does (() =>
{
    var solution = "./src/EventBuilder.sln";
    Information("Building {0} using {1}", solution, msBuildPath);

    NuGetRestore(solution, new NuGetRestoreSettings() { ConfigFile = "./src/.nuget/NuGet.config" });

    MSBuild(solution, new MSBuildSettings() {
            ToolPath = msBuildPath,
            ArgumentCustomization = args => args.Append("/bl:eventbuilder.binlog /m")
        }
        .SetConfiguration("Release")
        .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
        .SetVerbosity(Verbosity.Minimal)
        .SetNodeReuse(false));
});

Task("GenerateEvents")
    .IsDependentOn("BuildEventBuilder")
    .Does (() =>
{
    var eventBuilder = "./src/EventBuilder/bin/Release/net461/EventBuilder.exe";
    var workingDirectory = "./src/EventBuilder/bin/Release/Net461";
    var referenceAssembliesPath = VSWhereLatest().CombineWithFilePath("./Common7/IDE/ReferenceAssemblies/Microsoft/Framework");

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
    generate("wpf", "src/ReactiveUI.Events.WPF/");
    generate("xamforms", "src/ReactiveUI.Events.XamForms/");
    generate("winforms", "src/ReactiveUI.Events.Winforms/");
});

Task("BuildReactiveUI")
    .IsDependentOn("GenerateEvents")
    .Does (() =>
{
    Action<string,string> build = (solution, name) =>
    {
        Information("Building {0} using {1}", solution, msBuildPath);

        MSBuild(solution, new MSBuildSettings() {
                ToolPath = msBuildPath,
                ArgumentCustomization = args => args.Append("/bl:reactiveui-build-" + name + ".binlog /m /restore")
            }
            .WithTarget("build;pack") 
            .WithProperty("PackageOutputPath",  MakeAbsolute(Directory(artifactDirectory)).ToString().Quote())
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
            .SetConfiguration("Release")                        
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false));
    };

    foreach(var package in packageWhitelist)
    {
        build("./src/" + package + "/" + package + ".csproj", package);
    }        
    build("./src/ReactiveUI.Tests/ReactiveUI.Tests.csproj", "ReactiveUI.Tests");
});

Task("RunUnitTests")
    .IsDependentOn("BuildReactiveUI")
    .Does(() =>
{
    Action<ICakeContext> testAction = tool => {

        tool.XUnit2("./src/ReactiveUI.Tests/bin/**/*.Tests.dll", new XUnit2Settings {
            OutputDirectory = artifactDirectory,
            XmlReport = true,
            NoAppDomain = true
        });
    };

    OpenCover(testAction,
        testCoverageOutputFile,
        new OpenCoverSettings {
            ReturnTargetCodeOffset = 0,
            ArgumentCustomization = args => args.Append("-mergeoutput")
        }
        .WithFilter("+[*]*")
        .WithFilter("-[*.Testing]*")
        .WithFilter("-[*.Tests*]*")
        .WithFilter("-[Playground*]*")
        .WithFilter("-[ReactiveUI.Events]*")
        .WithFilter("-[Splat*]*")
        .WithFilter("-[ApprovalTests*]*")
        .ExcludeByAttribute("*.ExcludeFromCodeCoverage*")
        .ExcludeByFile("*/*Designer.cs")
        .ExcludeByFile("*/*.g.cs")
        .ExcludeByFile("*/*.g.i.cs")
        .ExcludeByFile("*splat/splat*")
        .ExcludeByFile("*ApprovalTests*"));

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
