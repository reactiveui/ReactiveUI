$currentDirectory = split-path $MyInvocation.MyCommand.Definition

# See if we have the ClientSecret available
if([string]::IsNullOrEmpty($env:SIGNCLIENT_SECRET)){
    Write-Error "Client Secret not found, not signing packages";
    [System.Environment]::Exit(1);  
}

# Setup Variables we need to pass into the sign client tool

$appSettings = "$currentDirectory\SignPackages.json"

$appPath = "$currentDirectory\packages\SignClient\tools\netcoreapp2.0\SignClient.dll"

$nupgks = ls $currentDirectory\artifacts\*.nupkg | Select -ExpandProperty FullName

foreach ($nupkg in $nupgks){
    Write-Host "Submitting $nupkg for signing"

    dotnet $appPath 'sign' -c $appSettings -i $nupkg -r $env:SIGNCLIENT_USER -s $env:SIGNCLIENT_SECRET -n 'ReactiveUI' -d 'ReactiveUI' -u 'https://reactiveui.net' 

    Write-Host "Finished signing $nupkg"
}

Write-Host "Sign-package complete"