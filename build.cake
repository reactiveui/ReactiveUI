// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Cake.FileHelpers&version=3.1.0"
#addin "nuget:?package=Cake.Codecov&version=0.5.0"
#addin "nuget:?package=Cake.Coverlet&version=2.2.1"

//////////////////////////////////////////////////////////////////////
// MODULES
//////////////////////////////////////////////////////////////////////

#module "nuget:?package=Cake.DotNetTool.Module&version=0.1.0"

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=vswhere&version=2.5.9"
#tool "nuget:?package=xunit.runner.console&version=2.4.1"
#tool "nuget:?package=Codecov&version=1.1.0"
#tool "nuget:?package=ReportGenerator&version=4.0.9"

//////////////////////////////////////////////////////////////////////
// DOTNET TOOLS
//////////////////////////////////////////////////////////////////////

#tool "dotnet:?package=SignClient&version=1.0.82"
#tool "dotnet:?package=coverlet.console&version=1.4.1"
#tool "dotnet:?package=nbgv&version=2.3.38"

//////////////////////////////////////////////////////////////////////
// CONSTANTS
//////////////////////////////////////////////////////////////////////

const string project = "ReactiveUI";

// Whitelisted Packages
var packageWhitelist = new List<string> 
{ 
    "ReactiveUI",
    "ReactiveUI.Testing",
    "ReactiveUI.Events",
    "ReactiveUI.Events.XamEssentials",
    "ReactiveUI.Events.XamForms",
    "ReactiveUI.Fody",
    "ReactiveUI.Fody.Helpers",
    "ReactiveUI.AndroidSupport",
    "ReactiveUI.XamForms",
};

if (IsRunningOnWindows())
{
    packageWhitelist.AddRange(new []
    {
        "ReactiveUI.Blend",
        "ReactiveUI.WPF",
        "ReactiveUI.Winforms",
        "ReactiveUI.Events.WPF",
        "ReactiveUI.Events.Winforms",
        // TODO: seems the leak tests never worked as part of the CI, fix. For the moment just make sure it compiles.
        "ReactiveUI.LeakTests"
    });
}

var packageTestWhitelist = new List<string>
{
    "ReactiveUI.Tests",    
};

if (IsRunningOnWindows())
{
    packageTestWhitelist.AddRange(new[]
    {     
        "ReactiveUI.Fody.Tests"
    });
}

var coverageTestFrameworks = new List<string>
{ 
    "netcoreapp2.0"
};

if (IsRunningOnWindows())
{
    coverageTestFrameworks.Add("net461");
}

var eventGenerators = new List<(string targetName, string destination)>
{
    ("android", "src/ReactiveUI.Events/"),
    ("ios", "src/ReactiveUI.Events/"),
    ("mac", "src/ReactiveUI.Events/"),
    ("tizen4", "src/ReactiveUI.Events/"),
    ("essentials", "src/ReactiveUI.Events.XamEssentials/"),
    ("tvos", "src/ReactiveUI.Events/"),
    ("xamforms", "src/ReactiveUI.Events.XamForms/"),
};

if (IsRunningOnWindows())
{
    eventGenerators.AddRange(new []
    {
        ("wpf", "src/ReactiveUI.Events.WPF/"),
        ("uwp", "src/ReactiveUI.Events/"),
        ("winforms", "src/ReactiveUI.Events.Winforms/"),
    });
}

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

var configuration = Argument("configuration", "Release");
if (string.IsNullOrWhiteSpace(configuration))
{
    configuration = "Release";
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
var isRepository = StringComparer.OrdinalIgnoreCase.Equals($"reactiveui/{project}", TFBuild.Environment.Repository.RepoName);

FilePath msBuildPath = null;
DirectoryPath referenceAssembliesPath = null;
if (IsRunningOnWindows())
{
    var vsWhereSettings = new VSWhereLatestSettings() { IncludePrerelease = includePrerelease };
    var vsLocation = string.IsNullOrWhiteSpace(vsLocationString) ? VSWhereLatest(vsWhereSettings) : new DirectoryPath(vsLocationString);
    msBuildPath = string.IsNullOrWhiteSpace(msBuildPathString) ? vsLocation.CombineWithFilePath("./MSBuild/15.0/Bin/MSBuild.exe") : new FilePath(msBuildPathString);
    referenceAssembliesPath = vsLocation.Combine("./Common7/IDE/ReferenceAssemblies/Microsoft/Framework");
}
else
{
    referenceAssembliesPath = Directory("⁨/Library⁩/Frameworks⁩/Libraries/⁨mono⁩");
}

//////////////////////////////////////////////////////////////////////
// FOLDERS
//////////////////////////////////////////////////////////////////////

// Artifacts
var artifactDirectory = "./artifacts/";
var testsArtifactDirectory = artifactDirectory + "tests/";
var binariesArtifactDirectory = artifactDirectory + "binaries/";
var packagesArtifactDirectory = artifactDirectory + "packages/";
var eventsArtifactDirectory = artifactDirectory + "Events/";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    StartProcess(Context.Tools.Resolve("nbgv*").ToString(), "cloud");

    CleanDirectories(artifactDirectory);
    CreateDirectory(testsArtifactDirectory);
    CreateDirectory(binariesArtifactDirectory);
    CreateDirectory(packagesArtifactDirectory);
    CreateDirectory(eventsArtifactDirectory);
});

Teardown(context =>
{
    // Executed AFTER the last task.
});

//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////
Action<string, string, bool> Build = (solution, packageOutputPath, doNotOptimise) =>
{
    Information("Building {0} using {1}", solution, msBuildPath);

    var msBuildSettings = new MSBuildSettings() {
            ToolPath = msBuildPath,
            ArgumentCustomization = args => args.Append("/m /NoWarn:VSX1000"),
            NodeReuse = false,
            Restore = true
        }
        .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors.ToString())
        .SetConfiguration(configuration)     
        .WithTarget("build;pack")                   
        .SetVerbosity(Verbosity.Minimal);

    if (!string.IsNullOrWhiteSpace(packageOutputPath))
    {
        msBuildSettings = msBuildSettings.WithProperty("PackageOutputPath",  MakeAbsolute(Directory(packageOutputPath)).ToString().Quote());
    }

    if (doNotOptimise)
    {
        msBuildSettings = msBuildSettings.WithProperty("Optimize",  "False");
    }

    MSBuild(solution, msBuildSettings);
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("BuildEventBuilder")
    .Does(() =>
{
    Build("./src/EventBuilder.sln", artifactDirectory + "eventbuilder", false);
});

Task("GenerateEvents")
    .IsDependentOn("BuildEventBuilder")
    .Does (() =>
{
    var workingDirectory = MakeAbsolute(Directory("./src/EventBuilder/bin/Release/netcoreapp2.1"));
    var eventBuilder = workingDirectory + "/EventBuilder.dll";

    foreach (var eventGenerator in eventGenerators)
    {
        var (platform, directory) = eventGenerator;

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
    }

    CopyFiles(GetFiles("./src/ReactiveUI.**/Events_*.cs"), Directory(eventsArtifactDirectory));
});

Task("Build")
    .IsDependentOn("GenerateEvents")
    .Does (() =>
{

    // Clean the directories since we'll need to re-generate the debug type.
    CleanDirectories($"./src/**/obj/{configuration}");
    CleanDirectories($"./src/**/bin/{configuration}");

    foreach(var packageName in packageWhitelist)
    {
        Build($"./src/{packageName}/{packageName}.csproj", packagesArtifactDirectory, false);
    }

    CopyFiles(GetFiles($"./src/**/bin/{configuration}/**/*"), Directory(binariesArtifactDirectory), true);
});

Task("RunUnitTests")
    .Does(() =>
{
    foreach (var packageName in packageTestWhitelist)
    {
        var projectName = $"./src/{packageName}/{packageName}.csproj";
        Build(projectName, null, true);
            
        foreach (var testFramework in coverageTestFrameworks)
        {
            Information($"Performing coverage tests on {packageName} on framework {testFramework}");

            var testFile = $"./src/{packageName}/bin/{configuration}/{testFramework}/{packageName}.dll";

            StartProcess(Context.Tools.Resolve("coverlet*").ToString(), new ProcessSettings {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = new ProcessArgumentBuilder()
                    .AppendQuoted(testFile)
                    .AppendSwitch("--include", $"[{project}*]*")
                    .AppendSwitch("--exclude", "[*.Tests*]*")
                    .AppendSwitch("--exclude", "[*]*Legacy*")
                    .AppendSwitch("--exclude", "[*]*ThisAssembly*")
                    .AppendSwitch("--exclude-by-file", "*ApprovalTests*")
                    .AppendSwitchQuoted("--output", testsArtifactDirectory + $"testcoverage-{packageName}-{testFramework}.xml")
                    .AppendSwitch("--format", "cobertura")
                    .AppendSwitch("--target", "dotnet")
                    .AppendSwitchQuoted("--targetargs", $"test {projectName} --no-build -c {configuration} --logger:trx;LogFileName=testresults-{packageName}-{testFramework}.trx -r {MakeAbsolute(Directory(testsArtifactDirectory))}")
                });

            Information($"Finished coverage testing {packageName}");
        }
    }

    // Generate both a summary and a combined summary.
    ReportGenerator(
        GetFiles($"{testsArtifactDirectory}**/testcoverage-*.xml"),
        testsArtifactDirectory + "report/",
        new ReportGeneratorSettings 
        {
            ReportTypes = new[] { ReportGeneratorReportType.Cobertura, ReportGeneratorReportType.Html },
        });
})
.ReportError(exception =>
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

    if(EnvironmentVariable("CODECOV_TOKEN") == null)
    {
        throw new Exception("Codecov token not found, not sending code coverage data.");
    }

    if (!string.IsNullOrEmpty(token))
    {
        var testCoverageOutputFile = MakeAbsolute(File(testsArtifactDirectory + "Report/Cobertura.xml"));

        Information("Upload {0} to Codecov server", testCoverageOutputFile);
        
        // Upload a coverage report.
        Codecov(testCoverageOutputFile.ToString(), token);
    }
});

Task("SignPackages")
    .IsDependentOn("Build")
    .WithCriteria(() => !local)
    .WithCriteria(() => !isPullRequest)
    .Does(() =>
{
    if(EnvironmentVariable("SIGNCLIENT_SECRET") == null)
    {
        throw new Exception("Client Secret not found, not signing packages.");
    }

    var nupkgs = GetFiles(packagesArtifactDirectory + "*.nupkg");
    foreach(FilePath nupkg in nupkgs)
    {
        var packageName = nupkg.GetFilenameWithoutExtension();
        Information($"Submitting {packageName} for signing");

        StartProcess(Context.Tools.Resolve("SignClient*").ToString(), new ProcessSettings {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            Arguments = new ProcessArgumentBuilder()
                .Append("sign")
                .AppendSwitch("-c", "./SignPackages.json")
                .AppendSwitch("-i", nupkg.FullPath)
                .AppendSwitch("-r", EnvironmentVariable("SIGNCLIENT_USER"))
                .AppendSwitch("-s", EnvironmentVariable("SIGNCLIENT_SECRET"))
                .AppendSwitch("-n", "ReactiveUI")
                .AppendSwitch("-d", "ReactiveUI")
                .AppendSwitch("-u", "https://reactiveui.net")
            });

        Information($"Finished signing {packageName}");
    }
    
    Information("Sign-package complete");
});

Task("Package")
    .IsDependentOn("Build")
    .IsDependentOn("SignPackages")
    .Does (() =>
{
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package")
    .IsDependentOn("RunUnitTests")
    .Does (() =>
{
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);