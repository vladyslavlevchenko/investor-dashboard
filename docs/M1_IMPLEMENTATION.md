# M1 Implementation Summary

## Overview
Completed the core data layer and infrastructure for the Investor Dashboard application, enabling data persistence and mock market data retrieval.

## Components Implemented

### 1. Domain Entities (Core Layer)
Created 7 entity classes in `src/InvestorDashboard.Core/Entities/`:

- **Security.cs** - Represents stocks/ETFs with ticker, name, asset class
- **PositionLot.cs** - Purchase lots for FIFO cost basis calculation (18,4 decimal precision)
- **Transaction.cs** - Buy/Sell/Dividend transactions with computed TotalValue and TotalCost
- **PriceBar.cs** - Historical OHLCV price data
- **PortfolioSettings.cs** - Portfolio configuration (risk-free rate, drift band, min notional)
- **TargetAllocation.cs** - Target portfolio weights per ticker
- **BenchmarkSeries.cs** - Benchmark comparison data (SPY, AGG, etc.)

### 2. Core Interfaces
Created 2 abstraction interfaces in `src/InvestorDashboard.Core/Interfaces/`:

- **IMarketDataProvider.cs** - Price fetching abstraction
  - `GetCurrentPriceAsync(ticker)` - Get latest price
  - `GetDailyPricesAsync(ticker, from, to)` - Get historical prices as PriceBar objects
  - `GetCurrentPricesAsync(tickers)` - Batch price retrieval

- **IPortfolioRepository.cs** - Data access abstraction with full CRUD for:
  - Securities (by ticker, all)
  - Position lots (by ticker, all, add single/batch)
  - Transactions (by ticker, all, add single/batch)
  - Price bars (by ticker and date range, add batch)
  - Portfolio settings (get, update)
  - Target allocations (get, update batch)
  - Benchmark series (by symbol and date range, add batch)

### 3. Database Infrastructure
Implemented in `src/InvestorDashboard.Infrastructure/Data/`:

**AppDbContext.cs**:
- 7 DbSet properties for all entities
- Entity configurations:
  - **Indexes**: Unique (Ticker, Symbol+Date, SecurityId+Date), Performance (PurchaseDate, Date)
  - **Precision**: Decimal(18,4) for prices/quantities, Decimal(10,6) for percentages
  - **Relationships**: Foreign keys with cascade delete
  - **Seed Data**: Default PortfolioSettings (Id=1, RiskFreeRate=0.04, MinNotional=100, DriftBandPercent=0.05)

**EF Core Migration**:
- **InitialCreate** migration generated
- SQLite database created with all tables, indexes, and seed data
- Database file: `investor_dashboard.db` (configurable via appsettings.json)

### 4. Repository Implementation
**PortfolioRepository.cs** (`src/InvestorDashboard.Infrastructure/Repositories/`):
- Implements `IPortfolioRepository` with EF Core
- Features:
  - AsNoTracking for read operations (performance)
  - FIFO ordering for position lots (OrderBy PurchaseDate)
  - Includes related entities (Security navigation properties)
  - CancellationToken support for all async operations
  - Auto-creates PortfolioSettings if missing (graceful handling)
  - Bulk operations (AddPositionLotsAsync, AddTransactionsAsync, AddPriceBarsAsync)

### 5. Mock Market Data Provider
**MockMarketDataProvider.cs** (`src/InvestorDashboard.Infrastructure/Providers/`):
- Implements `IMarketDataProvider` per ADR-005
- **Deterministic price generation**:
  - Base price: 100 with ticker-based offset (50-500 range)
  - Daily growth: 0.02% (~5% annual)
  - Volatility: 2% daily deterministic "noise" based on date hash
  - MD5 hashing for stable ticker and date offsets
- Returns full PriceBar objects with Open, High, Low, Close, Volume
- Skips weekends automatically
- Thread-safe and repeatable (same input = same output)

### 6. Dependency Injection Setup
Updated `Program.cs`:
```csharp
// Add database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories and services
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IMarketDataProvider, MockMarketDataProvider>();
```

### 7. API Controller
**SecuritiesController.cs** (`src/InvestorDashboard.Api/Controllers/`):
- RESTful endpoints for securities management
- **GET /api/securities** - List all securities
- **GET /api/securities/{ticker}** - Get security by ticker
- **POST /api/securities** - Create new security (with duplicate check)
- **GET /api/securities/{ticker}/price** - Get current price from mock provider
- Includes DTOs: `CreateSecurityRequest`, `PriceResponse`
- Proper HTTP status codes (200, 201, 400, 404)
- CreatedAtAction for POST response with Location header

## Database Schema

```
Securities
├─ Id (PK, auto-increment)
├─ Ticker (unique index)
├─ Name
├─ AssetClass
└─ CreatedAt

PositionLots
├─ Id (PK)
├─ SecurityId (FK → Securities, cascade delete, indexed)
├─ Quantity (decimal 18,4)
├─ CostBasis (decimal 18,4)
├─ PurchaseDate (indexed)
├─ CreatedAt
└─ UpdatedAt

Transactions
├─ Id (PK)
├─ SecurityId (FK → Securities, cascade delete, indexed)
├─ Type (enum: Buy=0, Sell=1, Dividend=2)
├─ Quantity (decimal 18,4)
├─ Price (decimal 18,4)
├─ Date (indexed)
├─ Commission (nullable, decimal 18,4)
├─ Notes
└─ CreatedAt

PriceBars
├─ Id (PK)
├─ SecurityId (FK → Securities, cascade delete)
├─ Date
├─ Close (required, decimal 18,4)
├─ Open (nullable, decimal 18,4)
├─ High (nullable, decimal 18,4)
├─ Low (nullable, decimal 18,4)
├─ Volume (nullable, bigint)
├─ CreatedAt
└─ Unique index on (SecurityId, Date)

PortfolioSettings
├─ Id (PK)
├─ RiskFreeRate (decimal 10,6, default 0.04)
├─ MinNotional (decimal 18,4, default 100.0)
├─ DriftBandPercent (decimal 10,6, default 0.05)
└─ UpdatedAt

TargetAllocations
├─ Id (PK)
├─ PortfolioSettingsId (FK → PortfolioSettings, cascade delete)
├─ Ticker
├─ TargetWeight (decimal 10,6)
├─ CreatedAt
├─ UpdatedAt
└─ Unique index on (PortfolioSettingsId, Ticker)

BenchmarkSeries
├─ Id (PK)
├─ Symbol
├─ Date
├─ Close (decimal 18,4)
├─ CreatedAt
└─ Unique index on (Symbol, Date)
```

## Testing
You can test the API endpoints using Swagger UI at `http://localhost:5092/swagger` when the app is running.

### Example API Calls

**1. Create a security:**
```bash
POST /api/securities
{
  "ticker": "AAPL",
  "name": "Apple Inc.",
  "assetClass": "US Equity"
}
```

**2. Get all securities:**
```bash
GET /api/securities
```

**3. Get security by ticker:**
```bash
GET /api/securities/AAPL
```

**4. Get current mock price:**
```bash
GET /api/securities/AAPL/price
```
Returns deterministic price generated by MockMarketDataProvider.

## Next Steps for M1 Completion

1. **CSV Import Functionality** (per IMPORT_FORMATS.md):
   - Positions CSV parser (Ticker, Quantity, CostBasis, Date)
   - Transactions CSV parser (Ticker, Type, Quantity, Price, Date, Commission)
   - Validation (ticker format, positive quantities, valid dates)
   - FIFO lot creation for position imports

2. **Additional Controllers**:
   - PositionsController - CRUD for position lots
   - TransactionsController - CRUD for transactions
   - PortfolioController - Settings and target allocations
   - PricesController - Fetch and store historical prices

3. **Unit Tests** (M1 DoD: ≥85% coverage):
   - Entity tests (FIFO logic, computed properties)
   - Repository tests (using in-memory DbContext)
   - MockMarketDataProvider tests (deterministic behavior)
   - Controller tests (using WebApplicationFactory)

4. **Integration with Frontend**:
   - Angular service for API calls
   - Portfolio summary component
   - Position list component
   - CSV upload component

## Files Changed (Commit d5c5281)
- 18 files changed, 1758 insertions(+), 12 deletions(-)
- Deleted: Class1.cs placeholders (Core, Infrastructure)
- Added: 7 entities, 2 interfaces, DbContext, migration, repository, provider, controller
- Modified: Program.cs (DI registration)

## Technical Decisions
- **SQLite for development**: Easy setup, no external dependencies
- **Decimal precision**: 18,4 for prices (max $99,999,999,999,999.9999)
- **FIFO ordering**: Position lots ordered by PurchaseDate for correct tax lot matching
- **Deterministic mocking**: Hash-based price generation for repeatable tests
- **UTC timestamps**: All CreatedAt/UpdatedAt fields use UTC to avoid timezone issues
- **Cascade delete**: Deleting a Security removes all related PositionLots, Transactions, PriceBars

## Validation
- ✅ Solution builds without errors
- ✅ EF Core migration created and applied
- ✅ Database schema created with indexes and seed data
- ✅ Repository implements all interface methods with CancellationToken support
- ✅ MockMarketDataProvider returns consistent deterministic prices
- ✅ SecuritiesController provides working REST endpoints
- ✅ All dependencies registered in DI container
- ✅ Unit tests pass (2 placeholder tests, will expand in next phase)
