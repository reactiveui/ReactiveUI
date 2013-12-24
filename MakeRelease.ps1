Param([string]$version = $null)

$Archs = {"Portable-Net45+WinRT45+WP8", "Net45", "WP8", "WinRT45", "Mono", "Monoandroid", "Monotouch", "Monomac"}
$Projects = {
    "ReactiveUI", "ReactiveUI.Testing", "ReactiveUI.Platforms", "ReactiveUI.Blend", 
    "ReactiveUI.NLog", "ReactiveUI.Mobile", "RxUIViewModelGenerator", "ReactiveUI.Events"
}

$MSBuildLocation = "C:\Program Files (x86)\MSBuild\12.0\bin"

$SlnFileExists = Test-Path ".\ReactiveUI_VSAll.sln"
if ($SlnFileExists -eq $False) {
    echo "*** ERROR: Run this in the project root ***"
    exit -1
}

& "$MSBuildLocation\MSBuild.exe" /t:Rebuild /p:Configuration=Release /p:Platform="Any CPU" /maxcpucount:1 .\ReactiveUI.sln

###
### Build the Release directory
###

if (Test-Path .\Release) {
    rmdir -r -force .\Release
}

foreach-object $Archs | %{mkdir -Path ".\Release\$_"}

foreach-object $Archs | %{
    $currentArch = $_
    
    foreach-object $Projects | %{cp -r -fo ".\$_\bin\Release\$currentArch\*" ".\Release\$currentArch"}
    
    #ls -r | ?{$_.FullName.Contains("bin\Release\$currentArch") -and $_.Length} | %{echo cp $_.FullName ".\Release\$currentArch"}
}

ls -r .\Release | ?{$_.FullName.Contains("Clousot")} | %{rm $_.FullName}


###
### Build NuGet Packages
###

if (Test-Path .\NuGet-Release) {
    rm -r -fo .\NuGet-Release
}

# Update Nuspecs if we have a version
if($version) {
    $nuspecs = ls -r .\NuGet\*.nuspec

    foreach($nuspec in $nuspecs) {
        $xml = New-Object XML
        $xml.Load($nuspec)
        
        # specify NS
        $nsMgr = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
        $nsMgr.AddNamespace("ns", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")

        # PowerShell makes editing XML docs so easy!
        $xml.package.metadata.version = $version

        # get the rxui dependencies and update them
        $deps = $xml.SelectNodes("//ns:dependency[contains(@id, 'reactiveui')]", $nsMgr) 
        foreach($dep in $deps) {
            $dep.version = "[" + $version + "]"
        }
        
        $xml.Save($nuspec)
    }
}

cp -r .\NuGet .\NuGet-Release

$libDirs = ls -r .\NuGet-Release | ?{$_.Name -eq "lib"}
$srcDirs = ls -r .\NuGet-Release | ?{$_.Name -eq "src"} | %{ls $_.FullName}
$toolsDirs = ls -r .\NuGet-Release | ?{$_.Name -eq "tools"}
$nugetReleaseDir = Resolve-Path ".\NuGet-Release"

# copy binaries
foreach ($dir in $libDirs) {
    $arches = ls $dir.FullName
    
    foreach ($arch in $arches) {
        $files = ls $arch.FullName

        foreach ($file in $files) {
            $src =  ".\Release\" + $arch.Name + "\\" + $file.Name
            cp -fo $src $file.FullName
        }        
    }
}

# copy tools
foreach ($dir in $toolsDirs) {
    echo "foo"
    echo $dir.FullName
    $files = ls $dir.FullName

    foreach ($file in $files) {
        echo "bar" 
        echo $file.FullName
        $src = ".\Release\Net45\" + $file.Name
        cp -fo "$src" $file.FullName
    }        
}

# copy source
foreach ($dir in $srcDirs) {
    $projName = $dir.Name
    $projFolderName = $projName.Replace("-", ".")

    robocopy ".\$projFolderName\" "$($dir.FullName)" *.cs /S
}

$stubs = ls -r -file .\NuGet-Release | ?{$_.Length -eq 0} | ?{!$_.FullName.Contains("src")}
if ($stubs.Length -gt 0) {
    echo "*** BUILD FAILED ***"
    echo ""
    echo "*** There are still stubs in the NuGet output, did you fully build? ***"
    #exit 1
}

$specFiles = ls -r .\NuGet-Release | ?{$_.Name.EndsWith(".nuspec")}
$specFiles | %{.\.nuget\NuGet.exe pack -symbols $_.FullName}
