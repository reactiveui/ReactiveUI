// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#load nuget:https://www.myget.org/F/reactiveui/?package=ReactiveUI.Cake.Recipe&prerelease

const string project = "ReactiveUI";

//////////////////////////////////////////////////////////////////////
// PROJECTS
//////////////////////////////////////////////////////////////////////

// Whitelisted Packages
var packageWhitelist = new List<FilePath> 
{ 
    MakeAbsolute(File("./src/ReactiveUI/ReactiveUI.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Testing/ReactiveUI.Testing.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Events/ReactiveUI.Events.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Events.XamEssentials/ReactiveUI.Events.XamEssentials.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Events.XamForms/ReactiveUI.Events.XamForms.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Fody/ReactiveUI.Fody.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Fody.Helpers/ReactiveUI.Fody.Helpers.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.AndroidSupport/ReactiveUI.AndroidSupport.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.XamForms/ReactiveUI.XamForms.csproj")),
};

if (IsRunningOnWindows())
{
    packageWhitelist.AddRange(new []
    {
        MakeAbsolute(File("./src/ReactiveUI.Blend/ReactiveUI.Blend.csproj")),
        MakeAbsolute(File("./src/ReactiveUI.WPF/ReactiveUI.WPF.csproj")),
        MakeAbsolute(File("./src/ReactiveUI.Winforms/ReactiveUI.Winforms.csproj")),
        MakeAbsolute(File("./src/ReactiveUI.Events.WPF/ReactiveUI.Events.WPF.csproj")),
        MakeAbsolute(File("./src/ReactiveUI.Events.Winforms/ReactiveUI.Events.Winforms.csproj")),
        // TODO: seems the leak tests never worked as part of the CI, fix. For the moment just make sure it compiles.
        MakeAbsolute(File("./src/ReactiveUI.LeakTests/ReactiveUI.LeakTests.csproj"))
    });
}

var packageTestWhitelist = new List<FilePath>
{
    MakeAbsolute(File("./src/ReactiveUI.Tests/ReactiveUI.Tests.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Splat.Tests/ReactiveUI.Splat.Tests.csproj"))
};

if (IsRunningOnWindows())
{
    packageTestWhitelist.AddRange(new[]
    {     
        MakeAbsolute(File("./src/ReactiveUI.Fody.Tests/ReactiveUI.Fody.Tests.csproj"))
    });
}

var eventGenerators = new List<(string targetName, DirectoryPath destination)>
{
    ("android", MakeAbsolute(Directory("src/ReactiveUI.Events/"))),
    ("ios", MakeAbsolute(Directory("src/ReactiveUI.Events/"))),
    ("mac", MakeAbsolute(Directory("src/ReactiveUI.Events/"))),
    ("tvos", MakeAbsolute(Directory("src/ReactiveUI.Events/"))),
};

if (IsRunningOnWindows())
{
    eventGenerators.AddRange(new []
    {
        ("wpf", MakeAbsolute(Directory("src/ReactiveUI.Events.WPF/"))),
        ("uwp", MakeAbsolute(Directory("src/ReactiveUI.Events/"))),
        ("winforms", MakeAbsolute(Directory("src/ReactiveUI.Events.Winforms/"))),
    });
}

//////////////////////////////////////////////////////////////////////
// SETUP
//////////////////////////////////////////////////////////////////////

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            title: project,
                            whitelistPackages: packageWhitelist,
                            whitelistTestPackages: packageTestWhitelist,
                            artifactsDirectory: "./artifacts",
                            sourceDirectory: "./src");

ToolSettings.SetToolSettings(context: Context);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("BuildEventBuilder")
    .Does(() =>
{
    BuildProject("./src/EventBuilder.sln", false);
});

Task("GenerateEvents")
    .IsDependentOn("BuildEventBuilder")
    .Does (() =>
{
    var eventsArtifactDirectory = BuildParameters.ArtifactsDirectory.Combine("Events");
    EnsureDirectoryExists(eventsArtifactDirectory);

    var workingDirectory = MakeAbsolute(Directory("./src/EventBuilder/bin/Release/netcoreapp2.1"));
    var eventBuilder = workingDirectory.CombineWithFilePath("EventBuilder.dll");

    DirectoryPath referenceAssembliesPath = null;
    if (IsRunningOnWindows())
    {
        referenceAssembliesPath = ToolSettings.VsLocation.Combine("./Common7/IDE/ReferenceAssemblies/Microsoft/Framework");
    }
    else
    {
        referenceAssembliesPath = Directory("⁨/Library⁩/Frameworks⁩/Libraries/⁨mono⁩");
    }

    foreach (var eventGenerator in eventGenerators)
    {
        var (platform, directory) = eventGenerator;

        var settings = new DotNetCoreExecuteSettings
        {
            WorkingDirectory = workingDirectory,
        };

        var filename = String.Format("Events_{0}.cs", platform);
        var output = directory.CombineWithFilePath(filename);

        Information("Generating events for '{0}'", platform);
        DotNetCoreExecute(
                    eventBuilder,
                    new ProcessArgumentBuilder()
                        .AppendSwitch("--platform","=", platform)
                        .AppendSwitchQuoted("--reference","=", referenceAssembliesPath.ToString())
                        .AppendSwitchQuoted("--output-path", "=", output.ToString()),
                    settings);

        Information("The events have been written to '{0}'", output);
    }

    CopyFiles(GetFiles("./src/ReactiveUI.**/Events_*.cs"), eventsArtifactDirectory);
});

BuildParameters.Tasks.BuildTask.IsDependentOn("GenerateEvents");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Build.Run();