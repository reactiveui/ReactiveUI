## The quick version

1. Open `ReactiveUI_Net4Only.sln` in VS2010 / VS2012
1. Run tests, build ReactiveUI.sln in VS2012 on Win8
1. Submit PR


## How to start hacking on ReactiveUI (the more verbose version)

1. Fork and Clone the source
1. Create a new branch for your feature / bugfix
1. Open the ReactiveUI_Net4Only.sln solution - this is the one you should use unless you're hacking on platform-specific code. 
1. Run all the tests, make sure they pass.
1. Write some new tests that fail
1. Make your change
1. See those same tests pass! Hurrah!
1. Build ReactiveUI.sln, see if you broke any other platforms (easy to do, sadly)
1. Push that branch to GitHub (`git push -u origin my-cool-new-feature`)
1. Go to your fork on GitHub, you should see a button with your branch next to it labeled 'Pull Request'
1. Type up some information about your change

## To make a new NuGet release for private use

*This looks hard, but once you get your environment set up, it's really only 'Build in VS, build in Mono, run script'*

1. Put the source into DropBox or another way you can share the same folder between a Mac and a PC (Parallels Shared Folders works too)
1. Edit `**/AssemblyInfo.cs` (i.e. every file named AssemblyInfo.cs) and bump the versions
1. Edit `NuGet/**/*.nuspec` (every nuspec file under the root NuGet folder) and do the same. Make sure to bump the dependent versions too!
1. Open ReactiveUI.sln and build it in Release mode under VS2012 on Windows 8 with the WP8 SDK installed (nothing earlier is supported)
1. Open MonoDevelop, and build ReactiveUI_Mono.sln in Release mode
1. Back on the PC, run `MakeRelease.ps1`
1. You'll end up with two new folders, `Release` and `Nuget-Release`, as well as the `.nupkg` files in the root directory.

## Some quirks

* The only 100% guaranteed .sln files to be maintained are ReactiveUI_Net4Only.sln and ReactiveUI.sln - the others may be missing projects
* Please follow my coding convention when submitting PRs - `if` statements have the brackets on the same line, non-public methods shouldBeCasedLikeThis, etc etc. I know I'm weird, Deal With It(tm).