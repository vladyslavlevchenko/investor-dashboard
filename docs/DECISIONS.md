# Architecture Decision Records

This document captures key architectural decisions made during the project.

---

## ADR-001: Clean Architecture with Layer Separation

**Date**: 2025-01-15  
**Status**: Accepted

### Context
Need to organize code for maintainability, testability, and clear separation of concerns for a portfolio management system.

### Decision
Adopt Clean Architecture with three main layers:
1. **Core** (Domain): Business logic, entities, interfaces - no external dependencies
2. **Infrastructure**: Data access (EF Core), external services (market data APIs)
3. **API**: HTTP endpoints, DTOs, OpenAPI documentation

### Consequences
**Positive**:
- Clear dependency direction (API → Infrastructure → Core)
- Core logic testable without database or HTTP
- Easy to swap implementations (e.g., SQLite → SQL Server)
- Domain models independent of persistence concerns

**Negative**:
- More projects/files than a simple layered architecture
- Requires mapping between domain entities and DTOs
- Learning curve for developers unfamiliar with pattern

**Alternatives Considered**:
- Monolithic layered architecture (rejected: harder to test, tight coupling)
- Vertical slice architecture (rejected: overkill for current scale)

---

## ADR-002: Angular over Blazor for Frontend

**Date**: 2025-01-15  
**Status**: Accepted

### Context
Need to choose a modern frontend framework for the investor dashboard UI.

### Decision
Use Angular (latest, v19+) with TypeScript instead of Blazor Server or Blazor WebAssembly.

### Rationale
- **Ecosystem Maturity**: Rich ecosystem of chart libraries (Chart.js, D3.js), UI components (Angular Material)
- **Performance**: Client-side rendering, no SignalR overhead (vs Blazor Server)
- **Developer Experience**: Strong TypeScript support, excellent CLI tooling
- **Separation of Concerns**: Clear API contract via OpenAPI, enables future mobile apps
- **Industry Standard**: Easier to hire Angular developers

### Consequences
**Positive**:
- API-first design enforces clean separation
- Can deploy frontend independently (Azure Static Web Apps, Netlify)
- Excellent charting and visualization libraries
- Strong tooling (Angular CLI, VS Code extensions)

**Negative**:
- Separate frontend/backend projects to manage
- CORS configuration required
- Duplicate model definitions (TypeScript types from C# DTOs)

**Alternatives Considered**:
- Blazor Server (rejected: stateful server connections, less mature ecosystem)
- Blazor WebAssembly (rejected: large initial download, .NET runtime in browser)
- React (rejected: project preference for opinionated framework)
- Vue (rejected: smaller enterprise adoption)

---

## ADR-003: SQLite for Development, Migratable to SQL Server

**Date**: 2025-01-15  
**Status**: Accepted

### Context
Need a database for development that's easy to set up while keeping production options open.

### Decision
Use SQLite for local development and testing, design schema to be compatible with SQL Server/PostgreSQL for production.

### Rationale
- **Zero Setup**: No database server required for developers
- **Easy Testing**: In-memory SQLite for integration tests
- **EF Core Abstractions**: Same code works with multiple providers
- **Migration Path**: Can switch to SQL Server/PostgreSQL by changing connection string

### Consequences
**Positive**:
- Developers can `dotnet run` without Docker or cloud dependencies
- Fast integration tests (in-memory database)
- Single-file database (easy backup for demo data)

**Negative**:
- Limited to SQLite feature set during development (no stored procedures, limited concurrency)
- Must avoid SQLite-specific syntax in migrations
- Production migration requires testing

**Migration Strategy**:
```csharp
// Program.cs
var dbProvider = configuration["Database:Provider"]; // "Sqlite" or "SqlServer"
if (dbProvider == "SqlServer") {
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
} else {
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(connectionString));
}
```

---

## ADR-004: FIFO Cost Basis Method

**Date**: 2025-01-15  
**Status**: Accepted

### Context
Need to calculate realized gains/losses for rebalancing suggestions and tax reporting.

### Decision
Implement FIFO (First-In-First-Out) cost basis method exclusively in v1.

### Rationale
- **Tax Compliance**: FIFO is the default method for US tax reporting if no election made
- **Simplicity**: Easier to implement and test than multiple methods
- **Deterministic**: No ambiguity in which lots are sold

### Consequences
**Positive**:
- Clear, unambiguous P&L calculations
- Straightforward testing (no configuration variants)
- Matches most users' default tax treatment

**Negative**:
- No tax optimization (can't select high-cost lots to minimize gains)
- May not match broker's method if they use Average Cost or Specific Identification
- Suboptimal for tax-loss harvesting

**Future Enhancements**:
- Add configuration option for cost basis method (FIFO, LIFO, Average Cost, Specific ID)
- Implement tax-loss harvesting suggestions
- Track wash sale periods

**Alternatives Considered**:
- Average Cost (rejected: not allowed for stocks in US, only mutual funds)
- Specific Identification (deferred: requires UI for lot selection)
- LIFO (rejected: uncommon, not advantageous for growing portfolios)

---

## ADR-005: Mock Market Data Provider for V1

**Date**: 2025-01-15  
**Status**: Accepted

### Context
Need market price data for metrics calculations and rebalancing, but don't want v1 blocked on API integration.

### Decision
Implement `IMarketDataProvider` interface with a Mock provider that returns deterministic data. Defer real API integration to post-v1.

### Mock Behavior
```csharp
public interface IMarketDataProvider
{
    Task<decimal> GetCurrentPrice(string ticker);
    Task<IEnumerable<PricePoint>> GetHistoricalPrices(string ticker, DateTime from, DateTime to);
}
```

Mock returns:
- Current price: Simple hash of ticker → $50-$500 range
- Historical: Linear growth from `from` date to `to` date (e.g., 8% annual)

### Rationale
- **No External Dependencies**: Developers don't need API keys or internet
- **Deterministic Testing**: Golden tests work reliably
- **Fast Development**: Unblocks metrics and rebalancing work
- **Clear Interface**: Real provider drop-in replacement later

### Consequences
**Positive**:
- Can develop and test all features offline
- No rate limits or API costs during development
- Predictable data for screenshot/demo purposes

**Negative**:
- Not real market data (demo only)
- Must clearly document "DEMO MODE" in UI

**Future Providers** (priority order):
1. **Yahoo Finance** (free, no API key, good historical data)
2. **Alpha Vantage** (free tier: 5 req/min, 500 req/day)
3. **IEX Cloud** (generous free tier, excellent API)

---

## ADR-006: Conventional Commits & PR Template

**Date**: 2025-01-15  
**Status**: Accepted

### Context
Need consistent commit messages and PR quality standards.

### Decision
Enforce Conventional Commits format and use a PR template with DoD checklist.

### Format
```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types**: `feat`, `fix`, `docs`, `refactor`, `test`, `build`, `chore`

**Examples**:
- `feat(metrics): add Sharpe Ratio calculation`
- `fix(import): handle malformed CSV gracefully`
- `docs(api): update rebalance endpoint examples`

### PR Template Checklist
- [ ] All tests pass (≥85% coverage in /src)
- [ ] CHANGELOG.md updated
- [ ] OpenAPI spec regenerated (if API changes)
- [ ] Documentation updated (ARCHITECTURE.md, API.md, etc.)
- [ ] `dotnet format` applied, StyleCop warnings = 0
- [ ] No secrets committed
- [ ] Demo data seed updated (if schema changed)

### Consequences
**Positive**:
- Automated changelog generation possible
- Clear commit history for code archaeology
- Consistent PR quality
- Forces documentation discipline

**Negative**:
- Slight learning curve for contributors
- Rejected commits if format wrong (if enforced by Git hook)

**Tooling**:
- commitlint (future: Git hook to validate messages)
- GitHub PR template (.github/PULL_REQUEST_TEMPLATE.md)

---

## ADR-007: Property-Based Testing for Invariants

**Date**: 2025-01-15  
**Status**: Accepted

### Context
Complex algorithms (rebalancing, drawdown) have mathematical invariants that should always hold.

### Decision
Use property-based testing (FsCheck or similar) to verify invariants across thousands of randomized inputs.

### Examples
**Rebalancing Invariants**:
1. After rebalancing, all weights within ±driftBand of targets
2. Sum of trade notionals ≈ 0 (cash-neutral)
3. No trades suggested if already within band

**Drawdown Invariants**:
1. MDD is monotonic: adding data never decreases MDD
2. MDD ∈ [-100%, 0%]
3. MDD = 0 if series only increases

### Rationale
- **Comprehensive Coverage**: Tests edge cases developers don't think of
- **Confidence**: Proves algorithms correct under wide range of inputs
- **Documentation**: Properties serve as executable specification

### Consequences
**Positive**:
- Catches subtle bugs (e.g., floating-point precision issues)
- Validates edge cases (empty portfolio, single asset, extreme weights)
- Builds confidence in refactoring

**Negative**:
- Slower test suite (thousands of cases per property)
- Harder to debug failures (random inputs)
- Learning curve for developers unfamiliar with technique

**Implementation**:
```csharp
[Property]
public Property Rebalancing_ShouldResultInWeightsWithinBand(
    PositiveDecimal totalValue,
    NonEmptyArray<TargetAllocation> targets)
{
    var suggestions = _engine.GenerateSuggestions(/* ... */);
    var newWeights = CalculateWeightsAfterTrades(suggestions);
    
    return newWeights.All(w => 
        Math.Abs(w.Actual - w.Target) <= _driftBand)
        .ToProperty();
}
```

---

## ADR-008: Defer Authentication to Post-V1

**Date**: 2025-01-15  
**Status**: Accepted

### Context
Multi-user support requires authentication, authorization, and data isolation.

### Decision
V1 is single-user (implicitly user ID = 1). Defer JWT authentication and multi-tenancy to v2.

### Rationale
- **Scope Control**: Keeps v1 focused on core features (metrics, rebalancing)
- **Faster Delivery**: Authentication adds significant complexity (user management, password reset, etc.)
- **Clear Architecture**: Design database schema with UserId FK from day 1, just hardcode in v1

### V1 Implementation
```csharp
// Hardcoded in repositories
private const int DEFAULT_USER_ID = 1;

public async Task<Portfolio> GetPortfolio(int portfolioId)
{
    return await _context.Portfolios
        .Where(p => p.Id == portfolioId && p.UserId == DEFAULT_USER_ID)
        .FirstOrDefaultAsync();
}
```

### V2 Migration Path
1. Add Identity tables (AspNetUsers, etc.)
2. Add JWT middleware
3. Replace `DEFAULT_USER_ID` with `HttpContext.User.GetUserId()`
4. Add `[Authorize]` attributes to controllers

### Consequences
**Positive**:
- V1 delivers faster
- Can demo and test core features without auth complexity
- Database schema ready for multi-user (UserId FK exists)

**Negative**:
- V1 not suitable for public deployment (no security)
- Must be clear this is localhost-only during v1

**Alternatives Considered**:
- Add auth in v1 (rejected: scope creep, delays core features)
- No UserId in schema (rejected: painful migration later)

---

## ADR-009: Zoneless Angular Architecture

**Date**: 2025-01-15  
**Status**: Accepted

### Context
Angular 19 introduces "zoneless" mode that removes Zone.js dependency.

### Decision
Use zoneless Angular with signals for state management.

### Rationale
- **Performance**: Eliminates Zone.js overhead (change detection happens explicitly)
- **Future-Proof**: Angular is moving toward zoneless by default
- **Modern Patterns**: Signals provide reactive state management
- **Smaller Bundle**: No Zone.js polyfill (~50KB savings)

### Consequences
**Positive**:
- Faster runtime performance
- More predictable change detection
- Aligns with Angular's roadmap
- Cleaner async handling with signals

**Negative**:
- Slightly different patterns than traditional Angular
- Some third-party libraries may expect Zone.js
- Requires explicit `ChangeDetectorRef.markForCheck()` in some cases

**Implementation**:
```typescript
// Component with signals
export class DashboardComponent {
  private metricsService = inject(MetricsService);
  
  metrics = signal<MetricsSummary | null>(null);
  
  async ngOnInit() {
    const data = await this.metricsService.getSummary();
    this.metrics.set(data); // Triggers update
  }
}
```

---

## Future ADRs to Document

- [ ] **ADR-010**: CSV vs JSON for import/export formats
- [ ] **ADR-011**: Chart.js vs D3.js for visualizations
- [ ] **ADR-012**: Azure deployment strategy (App Service vs Container Apps)
- [ ] **ADR-013**: Error handling and logging strategy
- [ ] **ADR-014**: API versioning approach (if needed)

---

**Last Updated**: M0 Completion
