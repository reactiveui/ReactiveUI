$dirs = Get-ChildItem .\ -include bin,obj -Recurse

foreach ($dir in $dirs)
{
	Write-Host "Removing $dir"
	Remove-Item $dir.FullName -Force -Recurse
}