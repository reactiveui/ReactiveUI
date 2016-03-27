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


// Define global marcos.
Action Abort = () => { throw new Exception("a non-recoverable fatal error occurred."); };

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
        throw new NotImplementedException("Building events on OSX is not implemented yet.");
        // run mdtool
    }
    else
    {
        MSBuild("./EventBuilder.sln", new MSBuildSettings()
            .SetConfiguration(configuration)
            .WithProperty("TreatWarningsAsErrors", "True")
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false));
    }
});

Task ("GenerateEvents")
    .IsDependentOn("BuildEventBuilder")
    .Does (() =>
{
    if(isRunningOnUnix)
    {
        throw new NotImplementedException("Building events on OSX is not implemented yet.");
    }
    else
    {
        var eventBuilder = "EventBuilder/bin/Release/EventBuilder.exe";
        var workingDirectory = "EventBuilder/bin/Release";

        Action<string> generate = (string platform) =>
        {
            using(var process = StartAndReturnProcess(eventBuilder,
                new ProcessSettings{
                    Arguments = "--platform=" + platform,
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
                    Abort();
                }

                var directory = "ReactiveUI.Events/";
                var filename = String.Format("Events_{0}.cs", platform);
                var output = System.IO.Path.Combine(directory, filename);

                FileWriteLines(output, stdout.ToArray());
                Information("The events have been written to '{0}'", output);
            }
        };

        generate("android");
        generate("ios");

        //Warning("Generating events for '{0}' is not implemented on Windows yet.", "MAC");
        //generate("mac");

        generate("net45");

        generate("wp81");
        generate("wpa81");
        generate("uwp");

        generate("xamforms");
    }
});

Task ("BuildEvents")
    .IsDependentOn("GenerateEvents")
    .Does (() =>
{
    if(isRunningOnUnix)
    {
        throw new NotImplementedException("Building events on OSX is not implemented yet.");
    }
    else
    {
        // WP81 is the only platform that needs to specify the MSBUILD platform
        // when the platform is retired remove the platform signature,
        // remove the .SetMSBuildPlatform method and simply the invoking methods.
        Action<string, MSBuildPlatform> build = (filename, platform) =>
        {
            var solution = System.IO.Path.Combine("./ReactiveUI.Events", filename);

            // UWP (project.json) needs to be restored before it will build.
            NuGetRestore (solution);

            Information("Building {0} with MSBuild {1} ", solution, platform);

            MSBuild(solution, new MSBuildSettings()
                .SetConfiguration(configuration)
                .SetMSBuildPlatform(platform)
                .WithProperty("NoWarn", "1591")
                .WithProperty("TreatWarningsAsErrors", "False")
                .SetVerbosity(Verbosity.Minimal)
                .SetNodeReuse(false));
        };

        build("ReactiveUI.Events_Android.sln", MSBuildPlatform.Automatic);
        build("ReactiveUI.Events_iOS.sln", MSBuildPlatform.Automatic);

        Warning("Building events for '{0}' is not implemented on Windows yet.", "MAC");
        //build("ReactiveUI.Events_MAC.sln");

        build("ReactiveUI.Events_NET45.sln", MSBuildPlatform.Automatic);

        build("ReactiveUI.Events_WP81.sln", MSBuildPlatform.x86);
        build("ReactiveUI.Events_WPA81.sln", MSBuildPlatform.Automatic);
        build("ReactiveUI.Events_UWP.sln", MSBuildPlatform.Automatic);

        build("ReactiveUI.Events_XamForms.sln", MSBuildPlatform.Automatic);
    }
});


Task ("PackageEvents")
    .IsDependentOn("BuildEvents")
    .Does (() =>
{

});

Task ("UpdateAssemblyInfo")
    .Does (() =>
{
    var file = "./CommonAssemblyInfo.cs";

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
    NuGetRestore ("./EventBuilder.sln");
    NuGetRestore ("./ReactiveUI.sln");
});

Task ("Package").Does (() =>
{
    if(isRunningOnUnix)
    {
        // Abort abort, packaging only works on Windows!
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget("PackageEvents");
