@echo off
powershell -ExecutionPolicy Unrestricted ./build.ps1 %CAKE_ARGS% %*
