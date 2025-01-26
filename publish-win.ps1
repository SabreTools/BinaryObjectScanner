# This batch file assumes the following:
# - .NET 9.0 (or newer) SDK is installed and in PATH
# - 7-zip commandline (7z.exe) is installed and in PATH
# - Git for Windows is installed and in PATH
#
# If any of these are not satisfied, the operation may fail
# in an unpredictable way and result in an incomplete output.

# Optional parameters
param(
    [Parameter(Mandatory = $false)]
    [Alias("UseAll")]
    [switch]$USE_ALL,

    [Parameter(Mandatory = $false)]
    [Alias("IncludeDebug")]
    [switch]$INCLUDE_DEBUG,

    [Parameter(Mandatory = $false)]
    [Alias("NoBuild")]
    [switch]$NO_BUILD,

    [Parameter(Mandatory = $false)]
    [Alias("NoArchive")]
    [switch]$NO_ARCHIVE
)

# Set the current directory as a variable
$BUILD_FOLDER = $PSScriptRoot

# Set the current commit hash
$COMMIT = git log --pretty=format:"%H" -1

# Output the selected options
Write-Host "Selected Options:"
Write-Host "  Use all frameworks (-UseAll)          $USE_ALL"
Write-Host "  Include debug builds (-IncludeDebug)  $INCLUDE_DEBUG"
Write-Host "  No build (-NoBuild)                   $NO_BUILD"
Write-Host "  No archive (-NoArchive)               $NO_ARCHIVE"
Write-Host " "

# Create the build matrix arrays
$FRAMEWORKS = @('net9.0')
$RUNTIMES = @('win-x86', 'win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'osx-x64', 'osx-arm64')

# Use expanded lists, if requested
if ($USE_ALL.IsPresent) {
    $FRAMEWORKS = @('net20', 'net35', 'net40', 'net452', 'net462', 'net472', 'net48', 'netcoreapp3.1', 'net5.0', 'net6.0', 'net7.0', 'net8.0', 'net9.0')
}

# Create the filter arrays
$SINGLE_FILE_CAPABLE = @('net5.0', 'net6.0', 'net7.0', 'net8.0', 'net9.0')
$VALID_APPLE_FRAMEWORKS = @('net6.0', 'net7.0', 'net8.0', 'net9.0')
$VALID_CROSS_PLATFORM_FRAMEWORKS = @('netcoreapp3.1', 'net5.0', 'net6.0', 'net7.0', 'net8.0', 'net9.0')
$VALID_CROSS_PLATFORM_RUNTIMES = @('win-arm64', 'linux-x64', 'linux-arm64', 'osx-x64', 'osx-arm64')

# Only build if requested
if (!$NO_BUILD.IsPresent) {
    # Restore Nuget packages for all builds
    Write-Host "Restoring Nuget packages"
    dotnet restore

    # Create Nuget Package
    dotnet pack BinaryObjectScanner\BinaryObjectScanner.csproj --output $BUILD_FOLDER

    # Build ExtractionTool
    foreach ($FRAMEWORK in $FRAMEWORKS) {
        foreach ($RUNTIME in $RUNTIMES) {
            # Output the current build
            Write-Host "===== Build ExtractionTool - $FRAMEWORK, $RUNTIME ====="

            # If we have an invalid combination of framework and runtime
            if ($VALID_CROSS_PLATFORM_FRAMEWORKS -notcontains $FRAMEWORK -and $VALID_CROSS_PLATFORM_RUNTIMES -contains $RUNTIME) {
                Write-Host "Skipped due to invalid combination"
                continue
            }

            # If we have Apple silicon but an unsupported framework
            if ($VALID_APPLE_FRAMEWORKS -notcontains $FRAMEWORK -and $RUNTIME -eq 'osx-arm64') {
                Write-Host "Skipped due to no Apple Silicon support"
                continue
            }

            # Only .NET 5 and above can publish to a single file
            if ($SINGLE_FILE_CAPABLE -contains $FRAMEWORK) {
                # Only include Debug if set
                if ($INCLUDE_DEBUG.IsPresent) {
                    dotnet publish ExtractionTool\ExtractionTool.csproj -f $FRAMEWORK -r $RUNTIME -c Debug --self-contained true --version-suffix $COMMIT -p:PublishSingleFile=true
                }
                dotnet publish ExtractionTool\ExtractionTool.csproj -f $FRAMEWORK -r $RUNTIME -c Release --self-contained true --version-suffix $COMMIT -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
            }
            else {
                # Only include Debug if set
                if ($INCLUDE_DEBUG.IsPresent) {
                    dotnet publish ExtractionTool\ExtractionTool.csproj -f $FRAMEWORK -r $RUNTIME -c Debug --self-contained true --version-suffix $COMMIT
                }
                dotnet publish ExtractionTool\ExtractionTool.csproj -f $FRAMEWORK -r $RUNTIME -c Release --self-contained true --version-suffix $COMMIT -p:DebugType=None -p:DebugSymbols=false
            }
        }
    }

    # Build ProtectionScan
    foreach ($FRAMEWORK in $FRAMEWORKS) {
        foreach ($RUNTIME in $RUNTIMES) {
            # Output the current build
            Write-Host "===== Build ProtectionScan - $FRAMEWORK, $RUNTIME ====="

            # If we have an invalid combination of framework and runtime
            if ($VALID_CROSS_PLATFORM_FRAMEWORKS -notcontains $FRAMEWORK -and $VALID_CROSS_PLATFORM_RUNTIMES -contains $RUNTIME) {
                Write-Host "Skipped due to invalid combination"
                continue
            }

            # If we have Apple silicon but an unsupported framework
            if ($VALID_APPLE_FRAMEWORKS -notcontains $FRAMEWORK -and $RUNTIME -eq 'osx-arm64') {
                Write-Host "Skipped due to no Apple Silicon support"
                continue
            }

            # Only .NET 5 and above can publish to a single file
            if ($SINGLE_FILE_CAPABLE -contains $FRAMEWORK) {
                # Only include Debug if set
                if ($INCLUDE_DEBUG.IsPresent) {
                    dotnet publish ProtectionScan\ProtectionScan.csproj -f $FRAMEWORK -r $RUNTIME -c Debug --self-contained true --version-suffix $COMMIT -p:PublishSingleFile=true
                }
                dotnet publish ProtectionScan\ProtectionScan.csproj -f $FRAMEWORK -r $RUNTIME -c Release --self-contained true --version-suffix $COMMIT -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
            }
            else {
                # Only include Debug if set
                if ($INCLUDE_DEBUG.IsPresent) {
                    dotnet publish ProtectionScan\ProtectionScan.csproj -f $FRAMEWORK -r $RUNTIME -c Debug --self-contained true --version-suffix $COMMIT
                }
                dotnet publish ProtectionScan\ProtectionScan.csproj -f $FRAMEWORK -r $RUNTIME -c Release --self-contained true --version-suffix $COMMIT -p:DebugType=None -p:DebugSymbols=false
            }
        }
    }

    # Build PackerScan
    foreach ($FRAMEWORK in $FRAMEWORKS) {
        foreach ($RUNTIME in $RUNTIMES) {
            # Output the current build
            Write-Host "===== Build PackerScan - $FRAMEWORK, $RUNTIME ====="

            # If we have an invalid combination of framework and runtime
            if ($VALID_CROSS_PLATFORM_FRAMEWORKS -notcontains $FRAMEWORK -and $VALID_CROSS_PLATFORM_RUNTIMES -contains $RUNTIME) {
                Write-Host "Skipped due to invalid combination"
                continue
            }

            # If we have Apple silicon but an unsupported framework
            if ($VALID_APPLE_FRAMEWORKS -notcontains $FRAMEWORK -and $RUNTIME -eq 'osx-arm64') {
                Write-Host "Skipped due to no Apple Silicon support"
                continue
            }

            # Only .NET 5 and above can publish to a single file
            if ($SINGLE_FILE_CAPABLE -contains $FRAMEWORK) {
                # Only include Debug if set
                if ($INCLUDE_DEBUG.IsPresent) {
                    dotnet publish PackerScan\PackerScan.csproj -f $FRAMEWORK -r $RUNTIME -c Debug --self-contained true --version-suffix $COMMIT -p:PublishSingleFile=true
                }
                dotnet publish PackerScan\PackerScan.csproj -f $FRAMEWORK -r $RUNTIME -c Release --self-contained true --version-suffix $COMMIT -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
            }
            else {
                # Only include Debug if set
                if ($INCLUDE_DEBUG.IsPresent) {
                    dotnet publish PackerScan\PackerScan.csproj -f $FRAMEWORK -r $RUNTIME -c Debug --self-contained true --version-suffix $COMMIT
                }
                dotnet publish PackerScan\PackerScan.csproj -f $FRAMEWORK -r $RUNTIME -c Release --self-contained true --version-suffix $COMMIT -p:DebugType=None -p:DebugSymbols=false
            }
        }
    }
}

# Only create archives if requested
if (!$NO_ARCHIVE.IsPresent) {
    # Create ExtractionTool archives
    foreach ($FRAMEWORK in $FRAMEWORKS) {
        foreach ($RUNTIME in $RUNTIMES) {
            # Output the current build
            Write-Host "===== Archive ExtractionTool - $FRAMEWORK, $RUNTIME ====="

            # If we have an invalid combination of framework and runtime
            if ($VALID_CROSS_PLATFORM_FRAMEWORKS -notcontains $FRAMEWORK -and $VALID_CROSS_PLATFORM_RUNTIMES -contains $RUNTIME) {
                Write-Host "Skipped due to invalid combination"
                continue
            }

            # If we have Apple silicon but an unsupported framework
            if ($VALID_APPLE_FRAMEWORKS -notcontains $FRAMEWORK -and $RUNTIME -eq 'osx-arm64') {
                Write-Host "Skipped due to no Apple Silicon support"
                continue
            }

            # Only include Debug if set
            if ($INCLUDE_DEBUG.IsPresent) {
                Set-Location -Path $BUILD_FOLDER\ExtractionTool\bin\Debug\${FRAMEWORK}\${RUNTIME}\publish\
                7z a -tzip $BUILD_FOLDER\ExtractionTool_${FRAMEWORK}_${RUNTIME}_debug.zip *
            }
        
            Set-Location -Path $BUILD_FOLDER\ExtractionTool\bin\Release\${FRAMEWORK}\${RUNTIME}\publish\
            7z a -tzip $BUILD_FOLDER\ExtractionTool_${FRAMEWORK}_${RUNTIME}_release.zip *
        }
    }

    # Create ProtectionScan archives
    foreach ($FRAMEWORK in $FRAMEWORKS) {
        foreach ($RUNTIME in $RUNTIMES) {
            # Output the current build
            Write-Host "===== Archive ProtectionScan - $FRAMEWORK, $RUNTIME ====="

            # If we have an invalid combination of framework and runtime
            if ($VALID_CROSS_PLATFORM_FRAMEWORKS -notcontains $FRAMEWORK -and $VALID_CROSS_PLATFORM_RUNTIMES -contains $RUNTIME) {
                Write-Host "Skipped due to invalid combination"
                continue
            }

            # If we have Apple silicon but an unsupported framework
            if ($VALID_APPLE_FRAMEWORKS -notcontains $FRAMEWORK -and $RUNTIME -eq 'osx-arm64') {
                Write-Host "Skipped due to no Apple Silicon support"
                continue
            }

            # Only include Debug if set
            if ($INCLUDE_DEBUG.IsPresent) {
                Set-Location -Path $BUILD_FOLDER\ProtectionScan\bin\Debug\${FRAMEWORK}\${RUNTIME}\publish\
                7z a -tzip $BUILD_FOLDER\ProtectionScan_${FRAMEWORK}_${RUNTIME}_debug.zip *
            }
        
            Set-Location -Path $BUILD_FOLDER\ProtectionScan\bin\Release\${FRAMEWORK}\${RUNTIME}\publish\
            7z a -tzip $BUILD_FOLDER\ProtectionScan_${FRAMEWORK}_${RUNTIME}_release.zip *
        }
    }

    # Create PackerScan archives
    foreach ($FRAMEWORK in $FRAMEWORKS) {
        foreach ($RUNTIME in $RUNTIMES) {
            # Output the current build
            Write-Host "===== Archive PackerScan - $FRAMEWORK, $RUNTIME ====="

            # If we have an invalid combination of framework and runtime
            if ($VALID_CROSS_PLATFORM_FRAMEWORKS -notcontains $FRAMEWORK -and $VALID_CROSS_PLATFORM_RUNTIMES -contains $RUNTIME) {
                Write-Host "Skipped due to invalid combination"
                continue
            }

            # If we have Apple silicon but an unsupported framework
            if ($VALID_APPLE_FRAMEWORKS -notcontains $FRAMEWORK -and $RUNTIME -eq 'osx-arm64') {
                Write-Host "Skipped due to no Apple Silicon support"
                continue
            }

            # Only include Debug if set
            if ($INCLUDE_DEBUG.IsPresent) {
                Set-Location -Path $BUILD_FOLDER\PackerScan\bin\Debug\${FRAMEWORK}\${RUNTIME}\publish\
                7z a -tzip $BUILD_FOLDER\PackerScan_${FRAMEWORK}_${RUNTIME}_debug.zip *
            }
        
            Set-Location -Path $BUILD_FOLDER\PackerScan\bin\Release\${FRAMEWORK}\${RUNTIME}\publish\
            7z a -tzip $BUILD_FOLDER\PackerScan_${FRAMEWORK}_${RUNTIME}_release.zip *
        }
    }

    # Reset the directory
    Set-Location -Path $PSScriptRoot
}
