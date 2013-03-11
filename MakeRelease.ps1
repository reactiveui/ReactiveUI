$Archs = {"Net40", "Net45", "SL5", "SL4-WindowsPhone71", "WP8", "WinRT45", "Mono", "Monodroid", "Monotouch"}
$Projects = {"ReactiveUI", "ReactiveUI.Testing", "ReactiveUI.Xaml", "ReactiveUI.Routing", "ReactiveUI.Blend", "ReactiveUI.Cocoa", "ReactiveUI.Gtk", "ReactiveUI.Android", "ReactiveUI.NLog", "ReactiveUI.Mobile"}

$SlnFileExists = Test-Path ".\ReactiveUI.sln"
if ($SlnFileExists -eq $False) {
    echo "*** ERROR: Run this in the project root ***"
    exit -1
}

###
### Build the Release directory
###

rmdir -r --force .\Release

foreach-object $Archs | %{mkdir -p ".\Release\$_"}

foreach-object $Archs | %{
    $currentArch = $_
    
    foreach-object $Projects | %{cp -r -fo ".\$_\bin\Release\$currentArch\*" ".\Release\$currentArch"}
    
    #ls -r | ?{$_.FullName.Contains("bin\Release\$currentArch") -and $_.Length} | %{echo cp $_.FullName ".\Release\$currentArch"}
}

ls -r .\Release | ?{$_.FullName.Contains("Clousot")} | %{rm $_.FullName}


###
### Build NuGet Packages
###

rm -r -fo .\NuGet-Release. .
cp -r .\NuGet .\NuGet-Release

$libDirs = ls -r .\NuGet-Release | ?{$_.Name -eq "lib"}
$nugetReleaseDir = Resolve-Path ".\NuGet-Release"

foreach ($dir in $libDirs) {
    $projName = $dir.FullName.Split("\\")[-2]
    $arches = ls $dir.FullName
    
    foreach ($arch in $arches) {
        $files = ls $arch.FullName

        foreach ($file in $files) {
            $src =  ".\Release\" + $arch.Name + "\\" + $file.Name
            cp -fo $src $file.FullName
        }        
    }
}

$stubs = ls -r -file .\NuGet-Release | ?{$_.Length -eq 0}
if ($stubs.Length -gt 0) {
    echo "*** BUILD FAILED ***"
    echo ""
    echo "*** There are still stubs in the NuGet output, did you fully build? (Hint: Check Silverlight) ***"
    #exit 1
}

$specFiles = ls -r .\NuGet-Release | ?{$_.Name.EndsWith(".nuspec")}
$specFiles | %{.\.nuget\NuGet.exe pack $_.FullName}
