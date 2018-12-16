@echo off
tools\nuget\nuget.exe install Cake -OutputDirectory tools -ExcludeVersion -Version 0.31.0

tools\Cake\Cake.exe build.cake --target=%1 

exit /b %errorlevel%
