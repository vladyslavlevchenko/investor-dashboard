# Test Script with Coverage Reporting
# Runs all tests and generates coverage report

param(
    [string]$Configuration = "Release",
    [int]$CoverageThreshold = 85
)

$ErrorActionPreference = "Stop"

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Running Tests with Coverage" -ForegroundColor Cyan
Write-Host "Coverage Threshold: $CoverageThreshold%" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

# Ensure build is up to date
Write-Host "`n[1/3] Building projects..." -ForegroundColor Yellow
dotnet build --configuration $Configuration
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Run tests with coverage
Write-Host "`n[2/3] Running tests with coverage..." -ForegroundColor Yellow
dotnet test --configuration $Configuration --no-build `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=opencover `
    /p:CoverageDirectory=./coverage `
    /p:Exclude="[*.Tests*]*"

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Generate coverage report
Write-Host "`n[3/3] Generating coverage report..." -ForegroundColor Yellow

# Install reportgenerator if not already installed
$reportGenInstalled = dotnet tool list -g | Select-String "dotnet-reportgenerator-globaltool"
if (-not $reportGenInstalled) {
    Write-Host "Installing reportgenerator tool..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

# Generate HTML report
reportgenerator `
    -reports:tests/**/coverage/coverage.opencover.xml `
    -targetdir:coverage/report `
    -reporttypes:"Html;TextSummary"

# Display summary
Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "Coverage Summary:" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Get-Content coverage/report/Summary.txt

# Check coverage threshold
$summaryContent = Get-Content coverage/report/Summary.txt -Raw
if ($summaryContent -match "Line coverage:\s+(\d+\.?\d*)%") {
    $coverage = [decimal]$matches[1]
    Write-Host "`nLine Coverage: $coverage%" -ForegroundColor $(if ($coverage -ge $CoverageThreshold) { "Green" } else { "Red" })
    
    if ($coverage -lt $CoverageThreshold) {
        Write-Host "❌ Coverage $coverage% is below threshold of $CoverageThreshold%" -ForegroundColor Red
        exit 1
    } else {
        Write-Host "✅ Coverage meets threshold!" -ForegroundColor Green
    }
}

Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "Coverage report generated at: coverage/report/index.html" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan

# Open report in browser (optional)
$openReport = Read-Host "`nOpen coverage report in browser? (y/n)"
if ($openReport -eq "y" -or $openReport -eq "Y") {
    Start-Process "coverage/report/index.html"
}
