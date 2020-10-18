// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#load nuget:https://pkgs.dev.azure.com/dotnet/ReactiveUI/_packaging/ReactiveUI/nuget/v3/index.json?package=ReactiveUI.Cake.Recipe&prerelease

const string project = "ReactiveUI";

private const string PharmacistTool = "#tool dotnet:?package=Pharmacist&prerelease";

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
    MakeAbsolute(File("./src/ReactiveUI.Fody.Analyzer/ReactiveUI.Fody.Analyzer.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Fody.Helpers/ReactiveUI.Fody.Helpers.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.AndroidSupport/ReactiveUI.AndroidSupport.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.AndroidX/ReactiveUI.AndroidX.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.XamForms/ReactiveUI.XamForms.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Uno/ReactiveUI.Uno.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Blazor/ReactiveUI.Blazor.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.Drawing/ReactiveUI.Drawing.csproj")),
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
    MakeAbsolute(File("./src/ReactiveUI.Splat.Tests/ReactiveUI.Splat.Tests.csproj")),
    MakeAbsolute(File("./src/ReactiveUI.XamForms.Tests/ReactiveUI.XamForms.Tests.csproj"))
};

if (IsRunningOnWindows())
{
    packageTestWhitelist.AddRange(new[]
    {     
        MakeAbsolute(File("./src/ReactiveUI.Fody.Tests/ReactiveUI.Fody.Tests.csproj")),
        MakeAbsolute(File("./src/ReactiveUI.Fody.Analyzer.Test/ReactiveUI.Fody.Analyzer.Test.csproj"))
    });
}

var eventGenerators = new List<(string[] targetNames, DirectoryPath destination)>
{
    (new[] { "android", "ios", "mac", "tvos" }, MakeAbsolute(Directory("src/ReactiveUI.Events/"))),
    (new[] { "wpf" }, MakeAbsolute(Directory("src/ReactiveUI.Events.WPF/"))),
    (new[] { "winforms" }, MakeAbsolute(Directory("src/ReactiveUI.Events.Winforms/"))),
};

if (IsRunningOnWindows())
{
    eventGenerators.AddRange(new []
    {
        (new[] { "uwp" }, MakeAbsolute(Directory("src/ReactiveUI.Events/"))),
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

Task("GenerateEvents")
    .Does(() => RequireGlobalTool(PharmacistTool, () =>
{
    var eventsArtifactDirectory = BuildParameters.ArtifactsDirectory.Combine("Events");
    EnsureDirectoryExists(eventsArtifactDirectory);

    foreach (var eventGenerator in eventGenerators)
    {
        var (platforms, directory) = eventGenerator;

        Information("Generating events for '{0}'", string.Join(", ", platforms));
        StartProcess(Context.Tools.Resolve("Pharmacist*").ToString(), new ProcessSettings {
                    Arguments = new ProcessArgumentBuilder()
                        .Append("generate-platform")
                        .AppendSwitch("-p", string.Join(",", platforms))
                        .AppendSwitch("-o", directory.ToString())
                        .AppendSwitch("--output-prefix", "Events_")
        });

        Information("The events have been written to '{0}'", directory);
    }

    CopyFiles(GetFiles("./src/ReactiveUI.**/Events_*.cs"), eventsArtifactDirectory);
}));

BuildParameters.Tasks.BuildTask.IsDependentOn("GenerateEvents");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Build.Run();
