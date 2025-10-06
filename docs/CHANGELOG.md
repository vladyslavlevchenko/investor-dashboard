# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial project structure with .NET 8 Web API
- Angular 19 frontend with zoneless architecture
- Solution structure with Core, Infrastructure, and API layers
- Comprehensive documentation (README, ARCHITECTURE, ALGORITHMS, API, IMPORT_FORMATS)
- Test project scaffolding (Unit and Integration)
- OpenAPI/Swagger configuration
- Entity Framework Core with SQLite provider
- Project conventions and quality gates defined

### Changed

### Deprecated

### Removed

### Fixed

### Security

---

## [0.1.0] - M0: Skeleton - 2025-01-XX

### Added
- Solution file and project structure
- Core entities: Portfolio, Position, Transaction, TargetAllocation
- Repository interfaces in Core layer
- API controllers scaffolding (Metrics, Settings, Rebalance, Export, Health)
- Angular application with routing
- Database context and initial migrations
- Logging configuration with Serilog
- Health check endpoint
- CI/CD pipeline configuration (GitHub Actions)
- StyleCop analyzer configuration
- User secrets template
- Demo data seeder

### Documentation
- Complete README with setup instructions
- Architecture decision records
- Algorithm specifications for metrics and rebalancing
- API endpoint reference
- Import format specifications
- Development workflow and DoD checklist

---

## Future Milestones

### [0.2.0] - M1: Data & Imports (Planned)
- CSV import for positions and transactions
- MarketDataProvider abstraction
- Mock market data provider implementation
- Entity configurations and relationships
- Repository implementations
- Cost basis calculator (FIFO)
- Import validation and error handling

### [0.3.0] - M2: Metrics & Charts (Planned)
- CAGR calculation implementation
- Sharpe Ratio calculation
- Maximum Drawdown calculation
- Benchmark comparison
- Time-series API endpoints
- Angular dashboard component
- Chart.js integration
- KPI cards UI

### [0.4.0] - M3: Targets & Rebalancing (Planned)
- Target allocation CRUD operations
- Drift detection algorithm
- Rebalancing suggestion engine
- Angular rebalance panel
- Target editor UI
- Trade execution simulation

### [0.5.0] - M4: Exports & Polish (Planned)
- CSV export implementation
- PDF export with charts
- Performance optimization
- Code coverage to 85%+
- Final documentation polish
- Release notes
- Deployment guide

---

## Template for Future Entries

```markdown
## [X.Y.Z] - YYYY-MM-DD

### Added
- New features

### Changed
- Changes to existing functionality

### Deprecated
- Features marked for removal

### Removed
- Removed features

### Fixed
- Bug fixes

### Security
- Security patches and improvements
```

---

**Commit Message Format**: `type(scope): description`

Types: `feat`, `fix`, `docs`, `refactor`, `test`, `build`, `chore`

Examples:
- `feat(metrics): implement CAGR calculation`
- `fix(import): handle empty CSV files gracefully`
- `docs(api): add examples for rebalance endpoint`
- `test(metrics): add property-based tests for drawdown`

---

**Last Updated**: M0 - Project Initialization
