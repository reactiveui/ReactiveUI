//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin "Cake.FileHelpers"

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool "GitReleaseManager"
#tool "GitVersion.CommandLine"
#tool "GitLink"

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

// Should MSBuild & GitLink treat any errors as warnings?
var treatWarningsAsErrors = false;

// Build configuration
var local = BuildSystem.IsLocalBuild;
var isRunningOnUnix = IsRunningOnUnix();
var isRunningOnWindows = IsRunningOnWindows();

var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var isRepository = StringComparer.OrdinalIgnoreCase.Equals("reactiveui/reactiveui", AppVeyor.Environment.Repository.Name);

var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", AppVeyor.Environment.Repository.Branch);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

var githubOwner = "reactiveui";
var githubRepository = "reactiveui";
var githubUrl = string.Format("https://github.com/{0}/{1}", githubOwner, githubRepository);

// Version
var gitVersion = GitVersion();
var majorMinorPatch = gitVersion.MajorMinorPatch;
var semVersion = gitVersion.SemVer;
var informationalVersion = gitVersion.InformationalVersion;
var nugetVersion = gitVersion.NuGetVersion;
var buildVersion = gitVersion.FullBuildMetaData;

// Artifacts
var artifactDirectory = "./artifacts/";
var packageWhitelist = new[] { "ReactiveUI-Testing", "ReactiveUI-Events", "ReactiveUI-Events-XamForms", "ReactiveUI", "ReactiveUI-Core", "ReactiveUI-AndroidSupport", "ReactiveUI-Blend", "ReactiveUI-Winforms", "ReactiveUI-XamForms" };

// Define global marcos.
Action Abort = () => { throw new Exception("a non-recoverable fatal error occurred."); };

Action<string> RestorePackages = (solution) =>
{
    NuGetRestore(solution, new NuGetRestoreSettings() { ConfigFile = "./src/.nuget/NuGet.config" });
};

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

        Version                  = nugetVersion,
        Tags                     = new [] {"mvvm", "reactiveui", "Rx", "Reactive Extensions", "Observable", "LINQ", "Events", "xamarin", "android", "ios", "forms", "monodroid", "monotouch", "xamarin.android", "xamarin.ios", "xamarin.forms", "wpf", "winforms", "uwp", "winrt", "net45", "netcore", "wp", "wpdev", "windowsphone", "windowsstore"},
        ReleaseNotes             = new [] { string.Format("{0}/releases", githubUrl) },

        Symbols                  = false,
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
Setup(context =>
{
    if (!isRunningOnWindows)
    {
        throw new NotImplementedException("ReactiveUI will only build on Windows (w/Xamarin installed) because it's not possible to target UWP, WPF and Windows Forms from UNIX.");
    }

    Information("Building version {0} of ReactiveUI. (isTagged: {1})", informationalVersion, isTagged);
});

Teardown(context =>
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
    var solution = "./src/EventBuilder.sln";

    MSBuild(solution, new MSBuildSettings()
        .SetConfiguration("Release")
        .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
        .SetVerbosity(Verbosity.Minimal)
        .SetNodeReuse(false));

    SourceLink(solution);
});

Task("GenerateEvents")
    .IsDependentOn("BuildEventBuilder")
    .Does (() =>
{
    var eventBuilder = "./src/EventBuilder/bin/Release/EventBuilder.exe";
    var workingDirectory = "./src/EventBuilder/bin/Release";

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
    generate("mac");
    generate("xamforms");

    generate("net45");
    
    generate("wpa81");
    generate("uwp");
});

Task("BuildEvents")
    .IsDependentOn("GenerateEvents")
    .Does (() =>
{
    Action<string> build = (filename) =>
    {
        var solution = System.IO.Path.Combine("./src/ReactiveUI.Events/", filename);

        // UWP (project.json) needs to be restored before it will build.
        RestorePackages (solution);

        Information("Building {0}", solution);

        MSBuild(solution, new MSBuildSettings()
            .SetConfiguration("Release")
            .WithProperty("NoWarn", "1591") // ignore missing XML doc warnings
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false));

        SourceLink(solution);
    };

    build("ReactiveUI.Events_Android.sln");
    build("ReactiveUI.Events_iOS.sln");
    build("ReactiveUI.Events_MAC.sln");
    build("ReactiveUI.Events_XamForms.sln");

    build("ReactiveUI.Events_NET45.sln");

    build("ReactiveUI.Events_WPA81.sln");
    build("ReactiveUI.Events_UWP.sln");
});

Task("PackageEvents")
    .IsDependentOn("BuildEvents")
    .Does (() =>
{
    Package("./src/ReactiveUI-Events.nuspec", "./src/ReactiveUI.Events");
    Package("./src/ReactiveUI-Events-XamForms.nuspec", "./src/ReactiveUI.Events");
});

Task("BuildReactiveUI")
    .IsDependentOn("RestorePackages")
    .IsDependentOn("UpdateAssemblyInfo")
    .Does (() =>
{
    Action<string> build = (solution) =>
    {
        Information("Building {0}", solution);

        MSBuild(solution, new MSBuildSettings()
            .SetConfiguration("Release")
            .WithProperty("NoWarn", "1591") // ignore missing XML doc warnings
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false));

        SourceLink(solution);
    };

    build("./src/ReactiveUI.sln");
});


Task("PackageReactiveUI")
    .IsDependentOn("BuildReactiveUI")
    .IsDependentOn("RunUnitTests")
    .Does (() =>
{
    // use pwd as as cake needs a basePath, even if making a meta-package that contains no files.
    Package("./src/ReactiveUI.nuspec", "./");
    Package("./src/ReactiveUI-Core.nuspec", "./src/ReactiveUI");

    Package("./src/ReactiveUI-AndroidSupport.nuspec", "./src/ReactiveUI.AndroidSupport");
    Package("./src/ReactiveUI-Blend.nuspec", "./src/ReactiveUI.Blend");
    Package("./src/ReactiveUI-Testing.nuspec", "./src/ReactiveUI.Testing");
    Package("./src/ReactiveUI-Winforms.nuspec", "./src/ReactiveUI.Winforms");
    Package("./src/ReactiveUI-XamForms.nuspec", "./src/ReactiveUI.XamForms");
});

Task("UpdateAppVeyorBuildNumber")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(buildVersion);
});

Task("UpdateAssemblyInfo")
    .IsDependentOn("UpdateAppVeyorBuildNumber")
    .Does (() =>
{
    var file = "./src/CommonAssemblyInfo.cs";

    CreateAssemblyInfo(file, new AssemblyInfoSettings {
        Product = "ReactiveUI",
        Version = majorMinorPatch,
        FileVersion = majorMinorPatch,
        InformationalVersion = informationalVersion,
        Copyright = "Copyright (c) ReactiveUI and contributors"
    });
});

Task("RestorePackages").Does (() =>
{
    RestorePackages("./src/EventBuilder.sln");
    RestorePackages("./src/ReactiveUI.sln");
});

Task("RunUnitTests")
    .IsDependentOn("BuildReactiveUI")
    .Does(() =>
{
    XUnit2("./src/ReactiveUI.Tests/bin/Release/Net45/ReactiveUI.Tests_Net45.dll", new XUnit2Settings {
        OutputDirectory = artifactDirectory,
        XmlReportV1 = true,
        NoAppDomain = true
    });
});

Task("Package")
    .IsDependentOn("PackageEvents")
    .IsDependentOn("PackageReactiveUI")
    .Does (() =>
{

});

Task("PublishPackages")
    .IsDependentOn("RunUnitTests")
    .IsDependentOn("Package")
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