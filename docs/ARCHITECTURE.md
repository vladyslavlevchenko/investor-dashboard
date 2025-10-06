# Architecture

## Overview

The Investor Dashboard follows a clean architecture pattern with clear separation of concerns across multiple layers.

## Layer Structure

### 1. API Layer (`InvestorDashboard.Api`)
**Responsibility**: HTTP endpoints, request/response handling, API documentation

**Components**:
- Controllers (MetricsController, SettingsController, RebalanceController, ExportController, HealthController)
- DTOs (Data Transfer Objects)
- Middleware (error handling, logging)
- OpenAPI/Swagger configuration

**Dependencies**: → Core, Infrastructure

### 2. Core Layer (`InvestorDashboard.Core`)
**Responsibility**: Business logic, domain models, interfaces

**Components**:
- **Entities**: Portfolio, Position, Transaction, TargetAllocation, RebalanceSuggestion
- **Services**: MetricsCalculator, RebalancingEngine, CostBasisCalculator
- **Interfaces**: IMarketDataProvider, IPortfolioRepository, IMetricsService
- **Value Objects**: Money, Percentage, DateRange

**Dependencies**: None (pure domain logic)

### 3. Infrastructure Layer (`InvestorDashboard.Infrastructure`)
**Responsibility**: Data access, external services, I/O

**Components**:
- **Data Access**: EF Core DbContext, Repositories
- **Migrations**: Database schema versioning
- **Providers**: MockMarketDataProvider, (future: YahooFinanceProvider)
- **Import/Export**: CSV parsers, PDF generators
- **Configuration**: Entity configurations

**Dependencies**: → Core

### 4. Angular Frontend (`investor-dashboard-ui`)
**Responsibility**: User interface, client-side logic

**Structure**:
```
src/
├── app/
│   ├── core/                  # Singleton services, guards, interceptors
│   │   ├── services/
│   │   │   ├── api.service.ts
│   │   │   ├── metrics.service.ts
│   │   │   └── portfolio.service.ts
│   │   └── interceptors/
│   ├── features/              # Feature modules
│   │   ├── dashboard/
│   │   │   ├── components/
│   │   │   └── dashboard.component.ts
│   │   ├── holdings/
│   │   ├── rebalance/
│   │   └── imports/
│   ├── shared/                # Shared components, pipes, directives
│   │   ├── components/
│   │   ├── models/
│   │   └── utils/
│   └── app.routes.ts
└── environments/
```

## Data Flow

### Metrics Calculation Flow
```
1. User requests metrics via Angular → HTTP GET /api/metrics/summary
2. MetricsController receives request
3. Controller calls IMetricsService.CalculateSummary()
4. MetricsService:
   - Fetches positions from IPortfolioRepository
   - Fetches historical prices from IMarketDataProvider
   - Calculates CAGR, Sharpe, MDD using time-series algorithms
   - Optionally fetches benchmark data
5. Returns MetricsSummaryDto to client
6. Angular charts component renders KPIs and graphs
```

### Rebalancing Flow
```
1. User sets target allocations → POST /api/settings/targets
2. SettingsController persists targets via repository
3. User requests suggestions → GET /api/rebalance/suggestions
4. RebalanceController:
   - Fetches current positions and market values
   - Fetches target allocations
   - Calls RebalancingEngine.GenerateSuggestions()
5. RebalancingEngine:
   - Calculates current weights
   - Identifies drift > driftBand
   - Generates buy/sell suggestions respecting minNotional
   - Uses CostBasisCalculator for realized P&L (FIFO)
6. Returns RebalanceSuggestionDto[] to client
7. Angular displays suggestions table with execute button
```

### Import Flow
```
1. User uploads CSV in Angular
2. File sent to POST /api/imports/positions or /api/imports/transactions
3. ImportController:
   - Validates CSV format
   - Parses rows using CsvPositionParser/CsvTransactionParser
   - Maps to domain entities
   - Saves via repository in transaction
4. Returns import summary (success count, errors)
5. Angular shows success notification and refreshes holdings
```

## Database Schema

**Entity Relationship Diagram**:
```
Portfolio (1) ──< (N) Position
          (1) ──< (N) Transaction
          (1) ──< (N) TargetAllocation

Position (N) ──< (1) Ticker [implied]
Transaction (N) ──< (1) Ticker [implied]
```

**Key Tables**:
- `Portfolios`: Id, Name, CreatedAt
- `Positions`: Id, PortfolioId, Ticker, Quantity, AvgCost, LastUpdated
- `Transactions`: Id, PortfolioId, Ticker, Type (Buy/Sell), Quantity, Price, Date
- `TargetAllocations`: Id, PortfolioId, Ticker, TargetWeight
- `MarketData`: Ticker, Date, Close (cached prices)

## Key Design Patterns

### Repository Pattern
Abstracts data access; `IPortfolioRepository` in Core, implemented in Infrastructure.

### Strategy Pattern
`IMarketDataProvider` interface allows swapping Mock → Yahoo Finance → Alpha Vantage.

### Dependency Injection
All dependencies injected via ASP.NET Core DI container configured in `Program.cs`.

### DTO Pattern
API uses DTOs to decouple domain models from API contracts; AutoMapper for mapping.

## Security Considerations

- **No secrets in code**: API keys via user-secrets or environment variables
- **CORS**: Configured to allow Angular dev server (localhost:4200)
- **Input validation**: FluentValidation for request DTOs
- **SQL injection prevention**: EF Core parameterized queries
- **Future**: Add authentication (JWT) and authorization for multi-tenant support

## Performance Considerations

- **Caching**: Market data cached for 24h to reduce external API calls
- **Async/await**: All I/O operations asynchronous
- **Pagination**: Large result sets paginated (future enhancement)
- **Indexing**: Database indexes on PortfolioId, Ticker, Date
- **Lazy loading disabled**: Explicit includes to avoid N+1 queries

## Testing Strategy

### Unit Tests
- Pure logic in Core (metrics calculations, rebalancing math)
- Repository interfaces mocked with Moq

### Integration Tests
- WebApplicationFactory for in-memory API testing
- SQLite in-memory database for data layer tests

### Property-Based Tests
- FsCheck for rebalancing invariants (weights sum to 100%, monotonic drawdown)

### Golden Tests
- Fixed 10y SPY/QQQ data → deterministic metric outputs verified

## Deployment Architecture (Future)

```
[Angular SPA] → Azure Static Web Apps
      ↓ HTTPS
[.NET API] → Azure App Service / Container Apps
      ↓
[SQLite] → migrate to Azure SQL Database
      ↓
[Market Data API] → External (Yahoo Finance, etc.)
```

## Extension Points

1. **New Market Data Provider**: Implement `IMarketDataProvider`
2. **New Import Format**: Add parser implementing `IImportParser<T>`
3. **New Metric**: Extend `MetricsCalculator` with new method
4. **Authentication**: Add JWT middleware and `[Authorize]` attributes
5. **Notifications**: Implement `INotificationService` for email/SMS alerts

---

**Last Updated**: M0 Completion
