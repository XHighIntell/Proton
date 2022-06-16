
@echo off

if not exist "\nuget.exe" (
    curl.exe --output "\nuget.exe" --url https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
    rem file exists
)

\nuget.exe pack Proton.nuspec
pause