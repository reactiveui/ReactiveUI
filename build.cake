// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Cake.FileHelpers&version=1.0.4"
#addin "nuget:?package=Cake.Coveralls&version=0.4.0"
#addin "nuget:?package=Cake.PinNuGetDependency&version=0.1.0.1495792899"
#addin "nuget:?package=Cake.Powershell&version=0.3.5"

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=GitReleaseManager&version=0.6.0"
#tool "nuget:?package=GitVersion.CommandLine&version=3.6.5"
#tool "nuget:?package=coveralls.io&version=1.3.4"
#tool "nuget:?package=OpenCover&version=4.6.519"
#tool "nuget:?package=ReportGenerator&version=2.5.11"
#tool "nuget:?package=vswhere&version=2.1.4"

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
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var isRepository = StringComparer.OrdinalIgnoreCase.Equals("reactiveui/reactiveui", AppVeyor.Environment.Repository.Name);

var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", AppVeyor.Environment.Repository.Branch);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

var githubOwner = "reactiveui";
var githubRepository = "reactiveui";
var githubUrl = string.Format("https://github.com/{0}/{1}", githubOwner, githubRepository);

var msBuildPath = VSWhereLatest().CombineWithFilePath("./MSBuild/15.0/Bin/MSBuild.exe");

// Version
var gitVersion = GitVersion();
var majorMinorPatch = gitVersion.MajorMinorPatch;
var informationalVersion = gitVersion.InformationalVersion;
var nugetVersion = gitVersion.NuGetVersion;
var buildVersion = gitVersion.FullBuildMetaData;

// Artifacts
var artifactDirectory = "./artifacts/";
var testCoverageOutputFile = artifactDirectory + "OpenCover.xml";
var packageWhitelist = new[] { "ReactiveUI.Testing",
                               "ReactiveUI.Events",
                               "ReactiveUI.Events.WPF",
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

    Information("Building version {0} of ReactiveUI. (isTagged: {1})", informationalVersion, isTagged);

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
            ArgumentCustomization = args => args.Append("/bl:eventbuilder.binlog")
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
    var eventBuilder = "./src/EventBuilder/bin/Release/net452/EventBuilder.exe";
    var workingDirectory = "./src/EventBuilder/bin/Release/Net452";
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
});

Task("BuildReactiveUI")
    .IsDependentOn("GenerateEvents")
    .Does (() =>
{
    Action<string> build = (solution) =>
    {
        Information("Building {0} using {1}", solution, msBuildPath);

        MSBuild(solution, new MSBuildSettings() {
                ToolPath = msBuildPath,
                ArgumentCustomization = args => args.Append("/bl:reactiveui-build.binlog")
            }
            .WithTarget("build;pack") 
            .WithProperty("PackageOutputPath",  MakeAbsolute(Directory(artifactDirectory)).ToString())
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
            .SetConfiguration("Release")
            // Due to https://github.com/NuGet/Home/issues/4790 and https://github.com/NuGet/Home/issues/4337 we
            // have to pass a version explicitly
            .WithProperty("Version", nugetVersion.ToString())
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false));
    };

    // Restore must be a separate step
    MSBuild("./src/ReactiveUI.sln", new MSBuildSettings() {
                ToolPath = msBuildPath,
                ArgumentCustomization = args => args.Append("/bl:reactiveui-restore.binlog")
            }
            .WithTarget("restore")
            .WithProperty("Version", nugetVersion.ToString())
            .SetVerbosity(Verbosity.Minimal));
    
    build("./src/ReactiveUI.sln");
});

Task("RunUnitTests")
    .IsDependentOn("BuildReactiveUI")
    .Does(() =>
{
    Action<ICakeContext> testAction = tool => {

        tool.XUnit2("./src/ReactiveUI.Tests/bin/**/*.Tests.dll", new XUnit2Settings {
            OutputDirectory = artifactDirectory,
            XmlReportV1 = true,
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
    if (string.IsNullOrEmpty(token))
    {
        throw new Exception("The COVERALLS_TOKEN environment variable is not defined.");
    }

    CoverallsIo(testCoverageOutputFile, new CoverallsIoSettings()
    {
        RepoToken = token
    });
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
    // only pin whitelisted packages.
    foreach(var package in packageWhitelist)
    {
        // only pin the package which was created during this build run.
        var packagePath = artifactDirectory + File(string.Concat(package, ".", nugetVersion, ".nupkg"));

        // see https://github.com/cake-contrib/Cake.PinNuGetDependency
        PinNuGetDependency(packagePath, "reactiveui");
    }
});


Task("PublishPackages")
    .IsDependentOn("RunUnitTests")
    .IsDependentOn("Package")
    .IsDependentOn("SignPackages")
    .WithCriteria(() => !local)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isRepository)
    .WithCriteria(() => isDevelopBranch || isReleaseBranch)
    .Does (() =>
{

    if (isReleaseBranch && !isTagged)
    {
        Information("Packages will not be published as this release has not been tagged.");
        return;
    }

    // Resolve the API key.
    var apiKey = EnvironmentVariable("NUGET_APIKEY");
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new Exception("The NUGET_APIKEY environment variable is not defined.");
    }

    var source = EnvironmentVariable("NUGET_SOURCE");
    if (string.IsNullOrEmpty(source))
    {
        throw new Exception("The NUGET_SOURCE environment variable is not defined.");
    }

    // only push whitelisted packages.
    foreach(var package in packageWhitelist)
    {
        // only push the package which was created during this build run.
        var packagePath = artifactDirectory + File(string.Concat(package, ".", nugetVersion, ".nupkg"));

        // Push the package.
        NuGetPush(packagePath, new NuGetPushSettings {
            Source = source,
            ApiKey = apiKey
        });
    }
});

Task("CreateRelease")
    .IsDependentOn("Package")
    .WithCriteria(() => !local)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isRepository)
    .WithCriteria(() => isReleaseBranch)
    .WithCriteria(() => !isTagged)
    .Does (() =>
{
    var username = EnvironmentVariable("GITHUB_USERNAME");
    if (string.IsNullOrEmpty(username))
    {
        throw new Exception("The GITHUB_USERNAME environment variable is not defined.");
    }

    var token = EnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        throw new Exception("The GITHUB_TOKEN environment variable is not defined.");
    }

    GitReleaseManagerCreate(username, token, githubOwner, githubRepository, new GitReleaseManagerCreateSettings {
        Milestone         = majorMinorPatch,
        Name              = majorMinorPatch,
        Prerelease        = true,
        TargetCommitish   = "master"
    });
});

Task("PublishRelease")
    .IsDependentOn("RunUnitTests")
    .IsDependentOn("Package")
    .WithCriteria(() => !local)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isRepository)
    .WithCriteria(() => isReleaseBranch)
    .WithCriteria(() => isTagged)
    .Does (() =>
{
    var username = EnvironmentVariable("GITHUB_USERNAME");
    if (string.IsNullOrEmpty(username))
    {
        throw new Exception("The GITHUB_USERNAME environment variable is not defined.");
    }

    var token = EnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        throw new Exception("The GITHUB_TOKEN environment variable is not defined.");
    }

    // only push whitelisted packages.
    foreach(var package in packageWhitelist)
    {
        // only push the package which was created during this build run.
        var packagePath = artifactDirectory + File(string.Concat(package, ".", nugetVersion, ".nupkg"));

        GitReleaseManagerAddAssets(username, token, githubOwner, githubRepository, majorMinorPatch, packagePath);
    }

    GitReleaseManagerClose(username, token, githubOwner, githubRepository, majorMinorPatch);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("CreateRelease")
    .IsDependentOn("PublishPackages")
    .IsDependentOn("PublishRelease")
    .Does (() =>
{
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);