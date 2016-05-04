@echo off
"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" /t:build ..\Release.sln /p:Configuration=Release
if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   exit /b %errorlevel%
)

"NuGet.exe" Pack "DevUtils.MEFExtensions.Core.nuspec"
"NuGet.exe" Pack "DevUtils.MEFExtensions.WCF.nuspec"
"NuGet.exe" Pack "DevUtils.MEFExtensions.Web.nuspec"
