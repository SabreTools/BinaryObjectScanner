#! /bin/bash

# This batch file assumes the following:
# - .NET 7.0 (or newer) SDK is installed and in PATH
# - zip is installed and in PATH
# - The relevant commandline programs are already downloaded
#   and put into their respective folders
#
# If any of these are not satisfied, the operation may fail
# in an unpredictable way and result in an incomplete output.

# Set the current directory as a variable
BUILD_FOLDER=$PWD

# Restore Nuget packages for all builds
echo "Restoring Nuget packages"
dotnet restore

# .NET 6.0 Debug
echo "Building .NET 6.0 debug"
dotnet publish Test/Test.csproj -f net6.0 -r win-x64 -c Debug --self-contained true -p:PublishSingleFile=true
dotnet publish Test/Test.csproj -f net6.0 -r linux-x64 -c Debug --self-contained true -p:PublishSingleFile=true
dotnet publish Test/Test.csproj -f net6.0 -r osx-x64 -c Debug --self-contained true -p:PublishSingleFile=true

# .NET 6.0 Release
echo "Building .NET 6.0 release"
dotnet publish Test/Test.csproj -f net6.0 -r win-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish Test/Test.csproj -f net6.0 -r linux-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish Test/Test.csproj -f net6.0 -r osx-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:DebugType=None -p:DebugSymbols=false

# .NET 7.0 Debug
echo "Building .NET 7.0 debug"
dotnet publish Test/Test.csproj -f net7.0 -r win-x64 -c Debug --self-contained true -p:PublishSingleFile=true
dotnet publish Test/Test.csproj -f net7.0 -r linux-x64 -c Debug --self-contained true -p:PublishSingleFile=true
dotnet publish Test/Test.csproj -f net7.0 -r osx-x64 -c Debug --self-contained true -p:PublishSingleFile=true

# .NET 7.0 Release
echo "Building .NET 7.0 release"
dotnet publish Test/Test.csproj -f net7.0 -r win-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish Test/Test.csproj -f net7.0 -r linux-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish Test/Test.csproj -f net7.0 -r osx-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:DebugType=None -p:DebugSymbols=false

# Create Test Debug archives
cd $BUILD_FOLDER/Test/bin/Debug/net6.0/win-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net6.0_win-x64_debug.zip .
cd $BUILD_FOLDER/Test/bin/Debug/net6.0/linux-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net6.0_linux-x64_debug.zip .
cd $BUILD_FOLDER/Test/bin/Debug/net6.0/osx-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net6.0_osx-x64_debug.zip .
cd $BUILD_FOLDER/Test/bin/Debug/net7.0/win-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net7.0_win-x64_debug.zip .
cd $BUILD_FOLDER/Test/bin/Debug/net7.0/linux-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net7.0_linux-x64_debug.zip .
cd $BUILD_FOLDER/Test/bin/Debug/net7.0/osx-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net7.0_osx-x64_debug.zip .

# Create Test Release archives
cd $BUILD_FOLDER/Test/bin/Release/net6.0/win-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net6.0_win-x64_release.zip .
cd $BUILD_FOLDER/Test/bin/Release/net6.0/linux-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net6.0_linux-x64_release.zip .
cd $BUILD_FOLDER/Test/bin/Release/net6.0/osx-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net6.0_osx-x64_release.zip .
cd $BUILD_FOLDER/Test/bin/Release/net7.0/win-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net7.0_win-x64_release.zip .
cd $BUILD_FOLDER/Test/bin/Release/net7.0/linux-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net7.0_linux-x64_release.zip .
cd $BUILD_FOLDER/Test/bin/Release/net7.0/osx-x64/publish/
zip -r $BUILD_FOLDER/BinaryObjectScanner_net7.0_osx-x64_release.zip .
