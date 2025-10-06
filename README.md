# Investor Dashboard

A comprehensive portfolio management and analytics platform built with .NET 8 Web API and Angular.

## Features

- **Portfolio Metrics**: CAGR, Sharpe Ratio, Maximum Drawdown
- **Rebalancing Engine**: Target allocation management with drift band detection
- **Data Import**: CSV import for positions and transactions
- **Export**: CSV and PDF export capabilities
- **Market Data**: Extensible provider system with mock implementation
- **Real-time Dashboard**: Interactive charts and KPI displays

## Tech Stack

### Backend
- .NET 8 Web API
- Entity Framework Core (SQLite)
- Serilog for logging
- Swagger/OpenAPI
- xUnit for testing

### Frontend
- Angular 19+ (zoneless)
- TypeScript
- SCSS
- Chart.js for visualizations

## Project Structure

```
├── src/
│   ├── InvestorDashboard.Api/          # Web API project
│   ├── InvestorDashboard.Core/         # Domain models, interfaces, services
│   ├── InvestorDashboard.Infrastructure/ # EF Core, data providers
│   └── investor-dashboard-ui/          # Angular frontend
├── tests/
│   ├── InvestorDashboard.Tests.Unit/   # Unit tests
│   └── InvestorDashboard.Tests.Integration/ # API tests
├── docs/                                # Documentation
└── scripts/                             # Build & deployment scripts
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 20+ and npm
- Visual Studio Code or Visual Studio 2022

### Backend Setup

```powershell
# Restore packages
dotnet restore

# Apply migrations
cd src/InvestorDashboard.Api
dotnet ef database update

# Run API
dotnet run
```

API will be available at `https://localhost:5001` with Swagger UI at `/swagger`.

### Frontend Setup

```powershell
# Install dependencies
cd src/investor-dashboard-ui
npm install

# Run dev server
npm start
```

Angular app will be available at `http://localhost:4200`.

## Configuration

Configuration is managed via `appsettings.json` and user secrets:

```json
{
  "PortfolioSettings": {
    "RiskFreeRate": 0.04,
    "MinNotional": 100.00,
    "DriftBand": 0.05
  },
  "MarketDataProvider": {
    "Type": "Mock"
  }
}
```

**Never commit secrets to the repository.** Use:
```powershell
dotnet user-secrets set "ApiKey" "your-key-here"
```

## API Endpoints

- `GET /api/metrics/summary?from=&to=&benchmark=SPY` - Portfolio metrics summary
- `POST /api/settings/targets` - Set target allocations
- `GET /api/rebalance/suggestions` - Get rebalancing suggestions
- `GET /api/export/summary.csv` - Export summary as CSV
- `GET /api/export/summary.pdf` - Export summary as PDF
- `GET /health` - Health check endpoint

See [API Documentation](docs/API.md) for detailed endpoint specifications.

## Testing

```powershell
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageDirectory=../coverage /p:CoverletOutputFormat=opencover

# View coverage report
reportgenerator -reports:coverage/coverage.opencover.xml -targetdir:coverage/report
```

**Coverage Target**: ≥85% line coverage in `/src`

## Development Workflow

### Conventional Commits
Use conventional commit format:
- `feat:` New features
- `fix:` Bug fixes
- `docs:` Documentation changes
- `refactor:` Code refactoring
- `test:` Test additions/changes
- `build:` Build system changes
- `chore:` Maintenance tasks

### Branch Naming
- `feature/feature-name`
- `fix/bug-description`

### Definition of Done
- ✅ All tests pass with ≥85% coverage
- ✅ Documentation updated (CHANGELOG.md, relevant docs)
- ✅ OpenAPI spec regenerated
- ✅ `dotnet format` clean, StyleCop warnings = 0
- ✅ No secrets committed

## Documentation

- [Architecture](docs/ARCHITECTURE.md) - System design and component structure
- [Algorithms](docs/ALGORITHMS.md) - Metric calculations and formulas
- [API Reference](docs/API.md) - Endpoint documentation
- [Import Formats](docs/IMPORT_FORMATS.md) - CSV schema specifications
- [Decisions](docs/DECISIONS.md) - Architecture decision records

## Milestones

- **M0**: Skeleton (solution structure, CI, basic infrastructure)
- **M1**: Data & Imports (entities, migrations, CSV parsing)
- **M2**: Metrics & Charts (calculations, API, dashboard UI)
- **M3**: Targets & Rebalancing (CRUD, drift detection, suggestions)
- **M4**: Exports & Polish (CSV/PDF export, performance optimization)

## License

MIT License - See LICENSE file for details

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes with tests
4. Ensure all quality gates pass
5. Submit a pull request

---

**Status**: 🚧 Under Development - M0 Complete
