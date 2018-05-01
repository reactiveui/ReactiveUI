@echo off
tools\nuget\nuget.exe update -self
tools\nuget\nuget.exe install Cake -OutputDirectory tools -ExcludeVersion -Version 0.22.1 

tools\Cake\Cake.exe build.cake --target=%1 --verbosity=diagnostic

exit /b %errorlevel%
