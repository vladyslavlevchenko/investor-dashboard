# Build Script for Investor Dashboard
# Builds both backend (.NET) and frontend (Angular)

param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Building Investor Dashboard" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

# Build Backend
Write-Host "`n[1/3] Building .NET Backend..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet build --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "✅ Backend build succeeded" -ForegroundColor Green

# Build Frontend
Write-Host "`n[2/3] Building Angular Frontend..." -ForegroundColor Yellow
Push-Location src\investor-dashboard-ui

if (Test-Path "node_modules") {
    Write-Host "node_modules exists, skipping npm install"
} else {
    Write-Host "Installing npm dependencies..."
    npm install
    if ($LASTEXITCODE -ne 0) { 
        Pop-Location
        exit $LASTEXITCODE 
    }
}

Write-Host "Building Angular app..."
npm run build -- --configuration=$Configuration.ToLower()
if ($LASTEXITCODE -ne 0) { 
    Pop-Location
    exit $LASTEXITCODE 
}

Pop-Location
Write-Host "✅ Frontend build succeeded" -ForegroundColor Green

# Run Tests
Write-Host "`n[3/3] Running Tests..." -ForegroundColor Yellow
dotnet test --configuration $Configuration --no-build --verbosity minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "✅ All tests passed" -ForegroundColor Green

Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "✅ Build completed successfully!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
