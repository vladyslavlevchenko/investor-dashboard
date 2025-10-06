# API Reference

Base URL: `https://localhost:5001/api`

All timestamps in ISO 8601 format. All monetary values as decimal numbers.

---

## Metrics

### GET /api/metrics/summary

Get portfolio performance metrics for a date range.

**Query Parameters**:
- `from` (string, optional): Start date in YYYY-MM-DD format. Default: portfolio inception
- `to` (string, optional): End date in YYYY-MM-DD format. Default: today
- `benchmark` (string, optional): Benchmark ticker for comparison (e.g., "SPY")

**Response** (200 OK):
```json
{
  "portfolio": {
    "cagr": 0.0959,
    "sharpeRatio": 1.23,
    "maxDrawdown": -0.185,
    "totalReturn": 1.50,
    "currentValue": 250000.00,
    "beginningValue": 100000.00
  },
  "benchmark": {
    "ticker": "SPY",
    "cagr": 0.084,
    "sharpeRatio": 1.05,
    "maxDrawdown": -0.22
  },
  "period": {
    "from": "2015-01-01",
    "to": "2025-01-01",
    "years": 10.0
  }
}
```

**Errors**:
- `400 Bad Request`: Invalid date format
- `404 Not Found`: Portfolio not found

**Example**:
```
GET /api/metrics/summary?from=2020-01-01&to=2024-12-31&benchmark=SPY
```

---

## Settings

### POST /api/settings/targets

Set or update target allocations for portfolio rebalancing.

**Request Body**:
```json
{
  "targets": [
    { "ticker": "AAPL", "weight": 0.40 },
    { "ticker": "MSFT", "weight": 0.40 },
    { "ticker": "GLD", "weight": 0.20 }
  ]
}
```

**Validations**:
- Sum of weights must be between 0.95 and 1.05 (tolerance for rounding)
- Each weight must be between 0.0 and 1.0
- Ticker must be non-empty

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Targets updated successfully",
  "targetCount": 3
}
```

**Errors**:
- `400 Bad Request`: Validation errors (weights don't sum to 100%, invalid ticker)

---

### GET /api/settings/targets

Get current target allocations.

**Response** (200 OK):
```json
{
  "targets": [
    { "ticker": "AAPL", "weight": 0.40 },
    { "ticker": "MSFT", "weight": 0.40 },
    { "ticker": "GLD", "weight": 0.20 }
  ],
  "lastUpdated": "2025-01-15T10:30:00Z"
}
```

---

## Rebalancing

### GET /api/rebalance/suggestions

Get suggested trades to rebalance portfolio to target allocations.

**Query Parameters**:
- `driftBand` (decimal, optional): Drift threshold (default: from config, e.g., 0.05 = 5%)
- `minNotional` (decimal, optional): Minimum trade size in dollars (default: from config)

**Response** (200 OK):
```json
{
  "suggestions": [
    {
      "ticker": "AAPL",
      "action": "SELL",
      "quantity": 85.5,
      "currentPrice": 175.50,
      "notional": 15004.25,
      "currentWeight": 0.55,
      "targetWeight": 0.40,
      "deviation": 0.15
    },
    {
      "ticker": "MSFT",
      "action": "BUY",
      "quantity": 25.3,
      "currentPrice": 395.25,
      "notional": 9999.825,
      "currentWeight": 0.30,
      "targetWeight": 0.40,
      "deviation": 0.10
    }
  ],
  "currentValue": 100000.00,
  "estimatedValueAfterRebalance": 100000.00,
  "generatedAt": "2025-01-15T14:20:00Z"
}
```

**Business Rules**:
- Only includes suggestions where deviation > driftBand
- Only includes suggestions where notional ≥ minNotional
- Quantities rounded to 1 decimal place
- Sum of trade notionals ≈ 0 (cash-neutral)

**Errors**:
- `400 Bad Request`: Invalid parameters
- `404 Not Found`: No target allocations set

---

## Export

### GET /api/export/summary.csv

Export portfolio summary and holdings as CSV.

**Query Parameters**:
- `from` (string, optional): Start date
- `to` (string, optional): End date

**Response** (200 OK):
```
Content-Type: text/csv
Content-Disposition: attachment; filename="portfolio-summary-2025-01-15.csv"

Ticker,Quantity,Avg Cost,Market Value,Current Price,Gain/Loss,Gain/Loss %
AAPL,100,150.00,17550.00,175.50,2550.00,17.00
MSFT,50,350.00,19762.50,395.25,2262.50,12.93
GLD,200,170.00,34800.00,174.00,800.00,2.35
```

---

### GET /api/export/summary.pdf

Export portfolio summary as PDF report.

**Query Parameters**:
- `from` (string, optional): Start date
- `to` (string, optional): End date
- `benchmark` (string, optional): Include benchmark comparison

**Response** (200 OK):
```
Content-Type: application/pdf
Content-Disposition: attachment; filename="portfolio-report-2025-01-15.pdf"

[Binary PDF content]
```

**PDF Contents**:
- Header with date range
- KPI summary (CAGR, Sharpe, MDD)
- Holdings table
- Performance chart (if date range > 30 days)
- Benchmark comparison (if specified)

---

## Imports

### POST /api/imports/positions

Import positions from CSV file.

**Request**:
```
Content-Type: multipart/form-data

file: [CSV file]
portfolioId: 1 (optional, defaults to user's default portfolio)
```

**CSV Format**:
```
Ticker,Quantity,AvgCost
AAPL,100,150.00
MSFT,50,350.00
```

**Response** (200 OK):
```json
{
  "success": true,
  "imported": 2,
  "errors": [],
  "warnings": [
    "GLD: Existing position updated"
  ]
}
```

**Errors**:
- `400 Bad Request`: Invalid CSV format, validation errors
- `413 Payload Too Large`: File > 10 MB

**Error Response** (400):
```json
{
  "success": false,
  "imported": 0,
  "errors": [
    "Row 3: Invalid quantity '-10'",
    "Row 5: Missing required field 'Ticker'"
  ]
}
```

---

### POST /api/imports/transactions

Import transactions from CSV file.

**Request**:
```
Content-Type: multipart/form-data

file: [CSV file]
```

**CSV Format**:
```
Date,Ticker,Type,Quantity,Price
2024-01-15,AAPL,Buy,100,150.00
2024-06-20,MSFT,Sell,25,375.00
```

**Response** (200 OK):
```json
{
  "success": true,
  "imported": 2,
  "errors": []
}
```

**Validations**:
- Date must be valid and not in future
- Type must be "Buy" or "Sell" (case-insensitive)
- Quantity and Price must be positive

---

## Health Check

### GET /health

Liveness/readiness probe for deployment orchestrators.

**Response** (200 OK):
```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "marketDataProvider": "Healthy"
  },
  "timestamp": "2025-01-15T14:20:00Z"
}
```

**Response** (503 Service Unavailable):
```json
{
  "status": "Unhealthy",
  "checks": {
    "database": "Unhealthy: Connection timeout",
    "marketDataProvider": "Healthy"
  },
  "timestamp": "2025-01-15T14:20:00Z"
}
```

---

## Error Responses

All endpoints may return standard HTTP error codes:

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "from": ["Invalid date format. Use YYYY-MM-DD."],
    "targets": ["Weights must sum to 100%."]
  }
}
```

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource not found",
  "status": 404,
  "detail": "Portfolio with ID 123 not found."
}
```

### 500 Internal Server Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500
}
```

---

## Rate Limiting (Future)

Not implemented in v1. Future versions will include:
- 100 requests per minute per IP
- 429 Too Many Requests response with `Retry-After` header

---

## Authentication (Future)

Not implemented in v1. Future versions will use JWT:
```
Authorization: Bearer <token>
```

---

**OpenAPI Spec**: Available at `/swagger/v1/swagger.json`

**Interactive Docs**: Available at `/swagger/index.html`

---

**Last Updated**: M0 Completion
