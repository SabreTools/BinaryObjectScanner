@echo OFF

REM This batch file assumes the following:
REM - .NET 8.0 (or newer) SDK is installed and in PATH
REM - 7-zip commandline (7z.exe) is installed and in PATH
REM - The relevant commandline programs are already downloaded
REM   and put into their respective folders
REM
REM If any of these are not satisfied, the operation may fail
REM in an unpredictable way and result in an incomplete output.

REM Set the current directory as a variable
set BUILD_FOLDER=%~dp0

REM Restore Nuget packages for all builds
echo Restoring Nuget packages
dotnet restore

REM Debug
echo Building debug
dotnet publish Test\Test.csproj -f net6.0 -r win-x64 -c Debug --self-contained true -p:PublishSingleFile=true
dotnet publish Test\Test.csproj -f net6.0 -r linux-x64 -c Debug --self-contained true -p:PublishSingleFile=true
dotnet publish Test\Test.csproj -f net6.0 -r osx-x64 -c Debug --self-contained true -p:PublishSingleFile=true
dotnet publish Test\Test.csproj -f net8.0 -r win-x64 -c Debug --self-contained true -p:PublishSingleFile=true
dotnet publish Test\Test.csproj -f net8.0 -r linux-x64 -c Debug --self-contained true -p:PublishSingleFile=true
dotnet publish Test\Test.csproj -f net8.0 -r osx-x64 -c Debug --self-contained true -p:PublishSingleFile=true

REM Release
echo Building release
dotnet publish Test\Test.csproj -f net6.0 -r win-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish Test\Test.csproj -f net6.0 -r linux-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish Test\Test.csproj -f net6.0 -r osx-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish Test\Test.csproj -f net8.0 -r win-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish Test\Test.csproj -f net8.0 -r linux-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish Test\Test.csproj -f net8.0 -r osx-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false

REM Create Test Debug archives
cd %BUILD_FOLDER%\Test\bin\Debug\net6.0\win-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net6.0_win-x64_debug.zip *
cd %BUILD_FOLDER%\Test\bin\Debug\net6.0\linux-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net6.0_linux-x64_debug.zip *
cd %BUILD_FOLDER%\Test\bin\Debug\net6.0\osx-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net6.0_osx-x64_debug.zip *
cd %BUILD_FOLDER%\Test\bin\Debug\net8.0\win-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net8.0_win-x64_debug.zip *
cd %BUILD_FOLDER%\Test\bin\Debug\net8.0\linux-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net8.0_linux-x64_debug.zip *
cd %BUILD_FOLDER%\Test\bin\Debug\net8.0\osx-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net8.0_osx-x64_debug.zip *

REM Create Test Release archives
cd %BUILD_FOLDER%\Test\bin\Release\net6.0\win-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net6.0_win-x64_release.zip *
cd %BUILD_FOLDER%\Test\bin\Release\net6.0\linux-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net6.0_linux-x64_release.zip *
cd %BUILD_FOLDER%\Test\bin\Release\net6.0\osx-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net6.0_osx-x64_release.zip *
cd %BUILD_FOLDER%\Test\bin\Release\net8.0\win-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net8.0_win-x64_release.zip *
cd %BUILD_FOLDER%\Test\bin\Release\net8.0\linux-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net8.0_linux-x64_release.zip *
cd %BUILD_FOLDER%\Test\bin\Release\net8.0\osx-x64\publish\
7z a -tzip %BUILD_FOLDER%\BinaryObjectScanner_net8.0_osx-x64_release.zip *
