# Build and Deploy Scripts

This directory contains scripts for building, testing, and deploying the Investor Dashboard.

## Scripts

### Build Script (`build.ps1`)
Builds the entire solution including backend and frontend.

```powershell
.\scripts\build.ps1
```

### Test Script (`test.ps1`)
Runs all tests with coverage reporting.

```powershell
.\scripts\test.ps1
```

### Deploy Script (`deploy.ps1`)
Deploys the application to Azure (to be implemented).

```powershell
.\scripts\deploy.ps1
```

## Usage

All scripts should be run from the solution root directory:

```powershell
cd c:\Users\VladyslavLevchenko\source\repos\investor_dashboard
.\scripts\build.ps1
```

## CI/CD

The GitHub Actions workflow in `.github/workflows/ci.yml` uses these same build and test commands to ensure consistency between local and CI environments.
