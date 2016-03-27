//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin "Cake.FileHelpers"

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool GitVersion.CommandLine
#tool GitLink

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// should MSBuild & GitLink treat any errors as warnings.
var treatWarningsAsErrors = false;

// Get whether or not this is a local build.
var local = BuildSystem.IsLocalBuild;
Information("local={0}", local);

var isRunningOnUnix = IsRunningOnUnix();
Information("isRunningOnUnix={0}", isRunningOnUnix);

var isRunningOnWindows = IsRunningOnWindows();
Information("isRunningOnWindows={0}", isRunningOnWindows);

//var isRunningOnBitrise = Bitrise.IsRunningOnBitrise;
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
Information("isRunningOnAppVeyor={0}", isRunningOnAppVeyor);

var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
Information("isPullRequest={0}", isPullRequest);

var isMainReactiveUIRepo = StringComparer.OrdinalIgnoreCase.Equals("reactiveui/reactiveui", AppVeyor.Environment.Repository.Name);
Information("isMainReactiveUIRepo={0}", isMainReactiveUIRepo);

// Parse release notes.
var releaseNotes = ParseReleaseNotes("RELEASENOTES.md");

// Get version.
var version = releaseNotes.Version.ToString();
Information("version={0}", version);

var epoch = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
Information("epoch={0}", epoch);

var gitSha = GitVersion().Sha;
Information("gitSha={0}", gitSha);

var semVersion = local ? string.Format("{0}.{1}", version, epoch) : string.Format("{0}.{1}", version, epoch);
Information("semVersion={0}", semVersion);

// Define directories.
var artifactDirectory = "./artifacts/";

// Define global marcos.
Action Abort = () => { throw new Exception("a non-recoverable fatal error occurred."); };

Action<string, string> Package = (nuspec, basePath) =>
{
    CreateDirectory(artifactDirectory);

    Information("Packaging {0} using {1} as the BasePath.", nuspec, basePath);

    NuGetPack(nuspec, new NuGetPackSettings {
        Authors                  = new [] {"ReactiveUI contributors"},
        Owners                   = new [] {"xpaulbettsx", "flagbug", "ghuntley", "haacked", "kent.boogaart", "mteper", "moswald", "niik", "onovotny", "rdavisau", "shiftkey"},

        ProjectUrl               = new Uri("http://www.reactiveui.net"),
        IconUrl                  = new Uri("https://i.imgur.com/7WDbqSy.png"),
        LicenseUrl               = new Uri("https://opensource.org/licenses/ms-pl.html"),
        Copyright                = "Copyright (c) ReactiveUI and contributors",
        RequireLicenseAcceptance = false,

        Version                  = semVersion,
        Tags                     = new [] {"mvvm", "reactiveui", "Rx", "Reactive Extensions", "Observable", "LINQ", "Events", "xamarin", "android", "ios", "forms", "monodroid", "monotouch", "xamarin.android", "xamarin.ios", "xamarin.forms", "wpf", "winforms", "uwp", "winrt", "net45", "netcore", "wp", "wpdev", "windowsphone", "windowsstore"},
        ReleaseNotes             = new List<string>(releaseNotes.Notes),

        Symbols                  = true,
        Verbosity                = NuGetVerbosity.Detailed,
        OutputDirectory          = artifactDirectory,
        BasePath                 = basePath,
    });
};

Action<string> SourceLink = (solutionFileName) =>
{
    GitLink("./", new GitLinkSettings() {
        RepositoryUrl = "https://github.com/reactiveui/ReactiveUI",
        SolutionFileName = solutionFileName,
        ErrorsAsWarnings = treatWarningsAsErrors,
    });
};


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

Task("BuildEventBuilder")
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
        var solution = "./EventBuilder.sln";

        MSBuild(solution, new MSBuildSettings()
            .SetConfiguration(configuration)
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false));

       SourceLink(solution);
    }
});

Task("GenerateEvents")
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

                var directory = "src/ReactiveUI.Events/";
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

Task("BuildEvents")
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
            var solution = System.IO.Path.Combine("./src/ReactiveUI.Events/", filename);

            // UWP (project.json) needs to be restored before it will build.
            NuGetRestore (solution);

            Information("Building {0} with MSBuild {1} ", solution, platform);

            MSBuild(solution, new MSBuildSettings()
                .SetConfiguration(configuration)
                .SetMSBuildPlatform(platform)
                .WithProperty("NoWarn", "1591") // ignore missing XML doc warnings
                .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
                .SetVerbosity(Verbosity.Minimal)
                .SetNodeReuse(false));

            SourceLink(solution);
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

Task("PackageEvents")
    .IsDependentOn("BuildEvents")
    .Does (() =>
{
    Package("./src/ReactiveUI-Events.nuspec", "./src/ReactiveUI.Events");
});

Task("UpdateAppVeyorBuildNumber")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(semVersion);
});

Task("UpdateAssemblyInfo")
    .IsDependentOn("UpdateAppVeyorBuildNumber")
    .Does (() =>
{
    var file = "./src/CommonAssemblyInfo.cs";

    CreateAssemblyInfo(file, new AssemblyInfoSettings {
        Product = "ReactiveUI",
        Version = version,
        FileVersion = version,
        InformationalVersion = semVersion,
        Copyright = "Copyright (c) ReactiveUI and contributors"
    });
});

Task("RestorePackages").Does (() =>
{
    NuGetRestore ("./EventBuilder.sln");
    NuGetRestore ("./ReactiveUI.sln");
});

Task("Package")
    .IsDependentOn("PackageEvents")
    .Does (() =>
{
    if(isRunningOnUnix)
    {
        throw new NotImplementedException("Packaging on OSX is not implemented yet.");    }
    else
    {

    }
});

Task("Publish")
  .IsDependentOn("Package")
  .WithCriteria(() => !local)
  .WithCriteria(() => !isPullRequest)
  .WithCriteria(() => isMainReactiveUIRepo)
  .Does (() =>
{
    if(isRunningOnUnix)
    {
        throw new NotImplementedException("Packaging on OSX is not implemented yet.");
    }
    else
    {
        // Resolve the API key.
        var apiKey = EnvironmentVariable("MYGET_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Could not resolve MyGet API key.");
        }

        // only push whitelisted packages.
        foreach(var package in new[] { "ReactiveUI-Events" })
        {
            // only push the package which was created during this build run.
            var packagePath = artifactDirectory + File(string.Concat(package, ".", semVersion, "*.nupkg"));

            // Push the package.
            NuGetPush(packagePath, new NuGetPushSettings {
                Source = "https://www.myget.org/F/reactiveui/api/v2/package",
                ApiKey = apiKey
            });
        }
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget("Publish");
