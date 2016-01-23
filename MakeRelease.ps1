Param([string]$version = $null)

$Archs = {
    "Mono",
    "Monoandroid",
    "Monomac",
    "Monotouch",
    "Net45",
    "Portable-Net45+Win8+WP8+WPA81",
    "Portable-Net45+WinRT45+WP8+MonoAndroid10+MonoTouch10",
    "Portable-Win81+Wpa81",
    "uap10.0",
    "WPA81",
    "WP8",
    "WP81"
    "Xamarin.iOS10",
    "Xamarin.Mac10"
}

$Projects = {
    "ReactiveUI",
    "ReactiveUI.AndroidSupport",
    "ReactiveUI.Blend",
    "ReactiveUI.Events",
    "ReactiveUI.Testing",
    "ReactiveUI.Winforms",
    "ReactiveUI.XamForms", 
    "RxUIViewModelGenerator"
}

$MSBuildLocation = "C:\Program Files (x86)\MSBuild\14.0\bin"

$SlnFileExists = Test-Path ".\ReactiveUI_VSAll.sln"
if ($SlnFileExists -eq $False) {
    echo "*** ERROR: Run this in the project root ***"
    exit -1
}


$url = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$nugetExe = "$(pwd)\nuget.exe"

$nugetExists = Test-Path $nugetExe

if($nugetExists -eq $False) {
"NuGet: Downloading latest from [$url]`nSaving at [$nugetExe]" 
    $client = new-object System.Net.WebClient 
    $client.DownloadFile($url, $nugetExe)     
}

& $nugetExe restore .\ReactiveUI.sln
& "$MSBuildLocation\MSBuild.exe" /v:m /t:Rebuild /p:Configuration=Release /p:Platform="Any CPU" /maxcpucount:1 .\ReactiveUI.sln

###
### Build the Release directory
###

if (Test-Path .\Release) {
    rmdir -r -force .\Release
}

foreach-object $Archs | %{mkdir -Path ".\Release\$_" | out-null}

foreach-object $Archs | %{
    $currentArch = $_
     
    foreach-object $Projects | %{cp -r -fo ".\$_\bin\Release\$currentArch\*" ".\Release\$currentArch"}
     
    #ls -r | ?{$_.FullName.Contains("bin\Release\$currentArch") -and $_.Length} | %{echo cp $_.FullName ".\Release\$currentArch"}
}
 
get-childitem -r .\Release | ?{$_.FullName.Contains("Clousot")} | %{rm $_.FullName}
 
 
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
        $xml.package.metadata.version = "$version"

        # get the rxui dependencies and update them
        $deps = $xml.SelectNodes("//ns:dependency[contains(@id, 'reactiveui')]", $nsMgr) 
        foreach($dep in $deps) {
            $dep.version = "[" + $version + "]"
        }
        
        $xml.Save($nuspec)
    }
}

cp -r .\NuGet .\NuGet-Release

$libDirs = get-childitem -r .\NuGet-Release | ?{$_.Name -eq "lib"}
$srcDirs = get-childitem -r .\NuGet-Release | ?{$_.Name -eq "src"} | %{get-childitem $_.FullName}
$toolsDirs = get-childitem -r .\NuGet-Release | ?{$_.Name -eq "tools"}
$nugetReleaseDir = Resolve-Path ".\NuGet-Release"

# copy binaries
foreach ($dir in $libDirs) {
    # only copy binaries which have a matching file in the destination folder
    robocopy ".\Release" $dir.FullName /S /XL
}

# copy tools
foreach ($dir in $toolsDirs) {
    echo "foo"
    echo $dir.FullName
    $files = get-childitem $dir.FullName

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

$stubs = get-childitem -r -file .\NuGet-Release | ?{$_.Length -eq 0} | ?{!$_.FullName.Contains("src")}
if ($stubs) {
    echo "*** BUILD FAILED ***"
    echo ""
    echo "*** There are still stubs in the NuGet output, did you fully build? ***"
    echo $stubs
    #exit 1
}

mkdir -path artifacts -ea silentlycontinue | out-null
$specFiles = get-childitem -r .\NuGet-Release | ?{$_.Name.EndsWith(".nuspec")}
$specFiles | %{& $nugetExe pack -symbols $_.FullName -OutputDirectory artifacts}
