set-strictmode -version 2.0
$ErrorActionPreference="Stop"

$AndroidToolPath = "${env:ProgramFiles(x86)}\Android\android-sdk\tools\android"
#$AndroidToolPath = "$env:localappdata\Android\android-sdk\tools\android"

Function Get-AllAndroidSDKs() {
    $output = & $AndroidToolPath list sdk --all
    $sdks = $output |% {
        if ($_ -match '(?<index>\d+)- (?<sdk>.+), revision (?<revision>[\d\.]+)') {
            $sdk = New-Object PSObject
            Add-Member -InputObject $sdk -MemberType NoteProperty -Name Index -Value $Matches.index
            Add-Member -InputObject $sdk -MemberType NoteProperty -Name Name -Value $Matches.sdk
            Add-Member -InputObject $sdk -MemberType NoteProperty -Name Revision -Value $Matches.revision
            $sdk
        }
    }
    $sdks
}

Function Execute-AndroidSDKInstall() {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true, Position=0)]
        [PSObject[]]$sdks
    )

    $sdkIndexes = $sdks |% { $_.Index }
    $sdkIndexArgument = [string]::Join(',', $sdkIndexes)
    Echo 'y' | & $AndroidToolPath update sdk -u -a -t $sdkIndexArgument
}

Function Install-AndroidSDK
{
    param([string]$Level)

    $sdks = Get-AllAndroidSDKs |? { $_.name -like "sdk platform*API $Level*" -or $_.name -like "google apis*api $Level" }
    Execute-AndroidSDKInstall -sdks $sdks
}

#Install-AndroidSDK 10
#Install-AndroidSDK 11
#Install-AndroidSDK 12
#Install-AndroidSDK 13
#Install-AndroidSDK 14
#Install-AndroidSDK 15
Install-AndroidSDK 16
#Install-AndroidSDK 18
#Install-AndroidSDK 19
#Install-AndroidSDK 20
#Install-AndroidSDK 21
#Install-AndroidSDK 20
#Install-AndroidSDK 21
#Install-AndroidSDK 22
#Install-AndroidSDK 23
Install-AndroidSDK 24
