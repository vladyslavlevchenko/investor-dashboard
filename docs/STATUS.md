# Project Status

## Milestone M0: Skeleton - ✅ COMPLETE

**Completion Date**: January 15, 2025

### Deliverables

#### ✅ Solution Structure
- [x] .NET 8 solution file created
- [x] InvestorDashboard.Api (Web API project)
- [x] InvestorDashboard.Core (Domain layer)
- [x] InvestorDashboard.Infrastructure (Data access)
- [x] InvestorDashboard.Tests.Unit (Unit tests)
- [x] InvestorDashboard.Tests.Integration (API tests)
- [x] Angular 19 frontend (zoneless architecture)

#### ✅ Configuration
- [x] appsettings.json with portfolio settings
- [x] CORS configuration for Angular dev server
- [x] Health check endpoint
- [x] Swagger/OpenAPI configuration
- [x] SQLite connection string
- [x] .gitignore (no secrets committed)

#### ✅ Documentation
- [x] README.md with setup instructions
- [x] ARCHITECTURE.md with layer descriptions
- [x] ALGORITHMS.md with metric formulas
- [x] API.md with endpoint specifications
- [x] IMPORT_FORMATS.md with CSV schemas
- [x] CHANGELOG.md with version history
- [x] DECISIONS.md with ADRs

#### ✅ CI/CD
- [x] GitHub Actions workflow (ci.yml)
- [x] Backend build and test jobs
- [x] Frontend build and lint jobs
- [x] Coverage threshold check (85%)
- [x] StyleCop and formatting checks
- [x] Security scanning
- [x] OpenAPI spec generation
- [x] Pull request template

#### ✅ Build & Test
- [x] Solution builds successfully
- [x] All projects target .NET 8
- [x] Test projects configured with xUnit, Moq, FluentAssertions
- [x] FsCheck for property-based testing
- [x] Coverlet for code coverage
- [x] Build script (build.ps1)
- [x] Test script with coverage (test.ps1)

#### ✅ Quality Gates
- [x] No build errors
- [x] All tests pass (2 placeholder tests)
- [x] No secrets in repository
- [x] Conventional commits documented
- [x] PR template with DoD checklist

### Statistics
- **Projects**: 6 (.NET + Angular)
- **Documentation Pages**: 7
- **Lines of Code**: ~500 (infrastructure)
- **Test Coverage**: N/A (placeholder tests only)
- **Build Time**: ~2.7s

---

## Next Milestone: M1 - Data & Imports

**Target Date**: TBD  
**Status**: 🚧 Not Started

### Planned Tasks

#### Domain Models (Core)
- [ ] Portfolio entity
- [ ] Position entity
- [ ] Transaction entity
- [ ] TargetAllocation entity
- [ ] RebalanceSuggestion entity
- [ ] PricePoint value object

#### Repositories (Infrastructure)
- [ ] AppDbContext with EF Core
- [ ] Entity configurations
- [ ] Initial migration
- [ ] PortfolioRepository implementation
- [ ] PositionRepository implementation
- [ ] TransactionRepository implementation

#### Market Data Provider
- [ ] IMarketDataProvider interface
- [ ] MockMarketDataProvider implementation
- [ ] Price caching logic

#### CSV Import
- [ ] CsvPositionParser
- [ ] CsvTransactionParser
- [ ] Validation logic
- [ ] ImportController endpoints

#### Tests
- [ ] Repository tests with in-memory database
- [ ] CSV parser tests with valid/invalid data
- [ ] FIFO cost basis calculator tests

---

## Roadmap

### M2: Metrics & Charts (After M1)
- Implement CAGR, Sharpe, MDD calculations
- Create MetricsService
- Build Angular dashboard with Chart.js
- Golden tests for metric validation

### M3: Targets & Rebalancing (After M2)
- Target allocation CRUD
- Rebalancing engine
- Drift detection
- Angular rebalance panel

### M4: Exports & Polish (After M3)
- CSV export
- PDF report generation
- Performance optimization
- Final documentation polish
- Coverage to 85%+

---

## Known Issues & Technical Debt

### Current
- None (M0 complete)

### Future Considerations
- Migration from SQLite to SQL Server for production
- Add authentication (JWT) for multi-user support
- Implement real market data provider (Yahoo Finance)
- Add transaction cost modeling
- Tax-loss harvesting suggestions

---

## Dependencies

### Backend
- .NET 8 SDK
- Entity Framework Core 8.0
- Swashbuckle (Swagger) 6.5
- Serilog 8.0
- xUnit 2.6
- Moq 4.20
- FluentAssertions 6.12
- FsCheck 2.16

### Frontend
- Node.js 20+
- Angular 19+
- TypeScript
- SCSS
- (Chart.js to be added in M2)

---

## Team Notes

### Development Environment Setup
1. Install .NET 8 SDK
2. Install Node.js 20+
3. Clone repository
4. Run `dotnet restore`
5. Run `cd src/investor-dashboard-ui && npm install`
6. Start API: `dotnet run --project src/InvestorDashboard.Api`
7. Start Angular: `cd src/investor-dashboard-ui && npm start`

### Commit Conventions
- Use conventional commits: `feat/fix/docs/refactor/test/build/chore`
- Reference issues: `feat(metrics): add CAGR calculation #123`
- Keep commits atomic and focused

### Code Review Checklist
- Tests pass with 85%+ coverage
- Documentation updated
- No StyleCop warnings
- No secrets committed
- Follows project conventions

---

**Last Updated**: M0 Completion - January 15, 2025
