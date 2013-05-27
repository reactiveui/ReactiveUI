<#
.SYNOPSIS
    Generates Reactive-UI objects from interfaces
.DESCRIPTION
    Use this script to auto-scaffold your Reactive-UI applications.
.PARAMETER TemplateType
    The RxUI object type you want to generate, one of: 'ViewModel', 
    'GeneratedViewModel', 'XamlControl', 'XamlCodeBehind'
.PARAMETER InterfaceFile
    The file containing the interfaces you want to generate Reactive-UI
    object from
.EXAMPLE
    C:\PS> ls -Recurse -Filter I*ViewModel.cs | Generate-RxUIObjects 'ViewModel'
    Generates a Reactive-UI ViewModel for all view model interfaces in 
    the current folder tree.
.NOTES
    
#>
function Generate-RxUIObjects {
  [CmdletBinding()]
  param (
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateSet('ViewModel', 'GeneratedViewModel', 'XamlControl', 'XamlCodeBehind')]
    [string]$TemplateType,
    
    [Parameter(Mandatory=$true, ValueFromPipeline=$true, Position=1)]
    [string]$InterfaceFile
  )
  
  begin {}
  process {
      $exe = ls -r -fi 'RxUIViewModelGenerator.exe' | select -First 1
      if(-not $exe -or -not (Test-Path $exe.FullName)) {
        throw "$exe not found"
      }
      if(-not (Test-Path $InterfaceFile)) {
        $InterfaceFile = Join-Path $pwd $InterfaceFile
        if(-not (Test-Path $InterfaceFile)) {
            throw "$InterfaceFile does not exist"
        }
      }
      $InterfaceFile = (gi $InterfaceFile).FullName
      $cmd = "$($exe.FullName)"
      $args = "-t=$TemplateType", "-i=$InterfaceFile"
       & $cmd $args
  }
  end {}
}

Register-TabExpansion 'Generate-RxUIObjects' @{
    'TemplateType' = {
        "ViewModel", 
        "GeneratedViewModel", 
        "XamlControl", 
        "XamlCodeBehind"
    }
}

Export-ModuleMember Generate-RxUIObjects