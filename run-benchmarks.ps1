#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Runs ReactiveUI benchmarks and generates performance reports.

.DESCRIPTION
    This script runs the ReactiveUI BenchmarkDotNet benchmarks and generates
    performance reports in markdown format. It supports running all benchmarks
    or specific benchmark categories.

.PARAMETER Category
    Specific benchmark category to run (e.g., "ReactiveCommand", "Navigation")

.PARAMETER OutputPath
    Path where benchmark results should be saved (default: ./BenchmarkDotNet.Artifacts)

.PARAMETER ExportMarkdown
    Export results in GitHub-compatible markdown format

.EXAMPLE
    .\run-benchmarks.ps1
    Runs all benchmarks with default settings

.EXAMPLE
    .\run-benchmarks.ps1 -Category "ReactiveCommand" -ExportMarkdown
    Runs only ReactiveCommand benchmarks and exports markdown

.NOTES
    Requirements:
    - Windows 10/11 (some projects are Windows-specific)
    - .NET 8.0 SDK or later
    - PowerShell 5.1 or PowerShell Core 6+
#>

param(
    [string]$Category = "",
    [string]$OutputPath = "./BenchmarkDotNet.Artifacts",
    [switch]$ExportMarkdown = $false
)

# Ensure we're in the correct directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$benchmarkDir = Join-Path $scriptDir "src/Benchmarks"

if (-not (Test-Path $benchmarkDir)) {
    Write-Error "Benchmark directory not found at: $benchmarkDir"
    Write-Error "Please run this script from the ReactiveUI repository root."
    exit 1
}

# Check for .NET SDK
try {
    $dotnetVersion = dotnet --version
    Write-Host "Using .NET SDK version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Error ".NET SDK not found. Please install .NET 8.0 SDK or later."
    exit 1
}

# Navigate to benchmark directory
Push-Location $benchmarkDir

try {
    Write-Host "Building benchmark project..." -ForegroundColor Yellow
    dotnet build -c Release
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed. Please check the output above."
        exit 1
    }

    # Prepare benchmark arguments
    $benchmarkArgs = @("-c", "Release")
    
    if ($Category) {
        $benchmarkArgs += "--filter"
        $benchmarkArgs += "*$Category*"
        Write-Host "Running benchmarks for category: $Category" -ForegroundColor Cyan
    } else {
        Write-Host "Running all benchmarks..." -ForegroundColor Cyan
    }

    if ($ExportMarkdown) {
        $benchmarkArgs += "--exporters"
        $benchmarkArgs += "github"
        Write-Host "Markdown export enabled" -ForegroundColor Green
    }

    # Run benchmarks
    Write-Host "Starting benchmark execution..." -ForegroundColor Yellow
    Write-Host "This may take several minutes to complete." -ForegroundColor Yellow
    
    & dotnet run @benchmarkArgs

    if ($LASTEXITCODE -eq 0) {
        Write-Host "`nBenchmarks completed successfully!" -ForegroundColor Green
        
        # Show results location
        $resultsPath = Join-Path (Get-Location) "BenchmarkDotNet.Artifacts"
        if (Test-Path $resultsPath) {
            Write-Host "Results saved to: $resultsPath" -ForegroundColor Green
            Get-ChildItem $resultsPath -Recurse | Where-Object { $_.Extension -eq ".md" } | ForEach-Object {
                Write-Host "  - $($_.Name)" -ForegroundColor Cyan
            }
        }
    } else {
        Write-Error "Benchmark execution failed."
        exit 1
    }

} catch {
    Write-Error "An error occurred: $($_.Exception.Message)"
    exit 1
} finally {
    Pop-Location
}

Write-Host "`nBenchmark run completed. Check the artifacts directory for detailed results." -ForegroundColor Green