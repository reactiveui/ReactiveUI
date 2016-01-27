#addin "Cake.FileHelpers"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Get whether or not this is a local build.
var local = BuildSystem.IsLocalBuild;
var isRunningOnUnix = IsRunningOnUnix();
var isRunningOnWindows = IsRunningOnWindows();

//var isRunningOnBitrise = Bitrise.IsRunningOnBitrise;
//var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
//var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest ? Bitrise.Environment.PullRequst.IsPullRequest;
//var isMainReactiveUIRepo = StringComparer.OrdinalIgnoreCase.Equals("reactiveui/reactiveui", AppVeyor.Environment.Repository.Name) StringComparer.OrdinalIgnoreCase.Equals("reactiveui/reactiveui", Bitrise.Environment.Repository.Name);

// Parse release notes.
var releaseNotes = ParseReleaseNotes("RELEASENOTES.md");

// Get version.
//var gitSha = GitVersion().Sha;
var gitSha = "e3ff88554778db547537cc75dce32200e906e8c6";
var buildNumber = AppVeyor.Environment.Build.Number;
var version = releaseNotes.Version.ToString();
var semVersion = local ? version : (version + string.Concat("-sha-", gitSha));

// Define directories.

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(() =>
{
    Information("Building version {0} of ReactiveUI.", semVersion);
});

Teardown(() =>
{
    // Executed AFTER the last task.
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task ("BuildEventBuilder")
    .IsDependentOn("RestorePackages")
    .IsDependentOn("UpdateAssemblyInfo")
    .Does (() =>
    {
        if(isRunningOnUnix)
        {
            // run mdtool
        }
        else
        {
            MSBuild("./EventBuilder.sln", new MSBuildSettings()
                .SetConfiguration(configuration)
                .WithProperty("Windows", "True")
                .WithProperty("TreatWarningsAsErrors", "True")
                .UseToolVersion(MSBuildToolVersion.NET45)
                .SetVerbosity(Verbosity.Minimal)
                .SetNodeReuse(false));
        }
    }
);

Task ("BuildEvents")
    .IsDependentOn("RestorePackages")
    .IsDependentOn("UpdateAssemblyInfo")
    .IsDependentOn("BuildEventBuilder")
    .Does (() =>
    {
        if(isRunningOnUnix)
        {
            // run mdtool
        }
        else
        {
            // run msbuild
        }
    }
);

Task ("UpdateAssemblyInfo")
    .Does (() =>
{
    var file = "CommonAssemblySolutionInfo.cs";

    CreateAssemblyInfo(file, new AssemblyInfoSettings {
        Product = "ReactiveUI",
        Version = version,
        FileVersion = version,
        InformationalVersion = semVersion,
        Copyright = "Copyright (c) ReactiveUI and contributors"
    });
});

Task ("RestorePackages").Does (() =>
{
    NuGetRestore ("EventBuilder.sln");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget("BuildEventBuilder");
