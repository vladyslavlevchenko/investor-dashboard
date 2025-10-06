# Algorithms

## Metrics Calculations

### 1. Compound Annual Growth Rate (CAGR)

**Formula**:
```
CAGR = (Ending Value / Beginning Value)^(1 / Years) - 1
```

**Implementation Details**:
- Years calculated as: `(endDate - startDate).TotalDays / 365.25`
- Handles leap years correctly
- Returns null if beginning value ≤ 0

**Example**:
```
Portfolio value on 2015-01-01: $100,000
Portfolio value on 2025-01-01: $250,000
Years: 10.0

CAGR = (250000 / 100000)^(1/10) - 1
     = 2.5^0.1 - 1
     = 1.0959 - 1
     = 0.0959 or 9.59%
```

**Edge Cases**:
- Zero or negative beginning value → return null
- Same start/end date → return 0%
- Single data point → return 0%

---

### 2. Sharpe Ratio

**Formula**:
```
Sharpe Ratio = (Mean Return - Risk-Free Rate) / Standard Deviation of Returns
```

**Implementation Details**:
- Returns calculated from daily price changes: `(P_t - P_{t-1}) / P_{t-1}`
- Annualized by multiplying by `sqrt(252)` (trading days per year)
- Risk-free rate (configurable, default 4%) converted to daily rate
- Minimum 30 data points required for statistical validity

**Example**:
```
Daily returns (sample): [0.01, -0.005, 0.02, 0.015, -0.01, ...]
Mean daily return: 0.0008
Std dev of daily returns: 0.012
Risk-free rate (annual): 0.04
Risk-free rate (daily): 0.04 / 252 = 0.000159

Daily Sharpe = (0.0008 - 0.000159) / 0.012 = 0.0534
Annualized Sharpe = 0.0534 × sqrt(252) = 0.848
```

**Interpretation**:
- < 1.0: Poor risk-adjusted returns
- 1.0 - 2.0: Good
- 2.0 - 3.0: Very good
- \> 3.0: Excellent

**Edge Cases**:
- < 30 data points → return null
- Zero std dev (flat returns) → return null (avoid division by zero)
- All negative returns → negative Sharpe (valid)

---

### 3. Maximum Drawdown (MDD)

**Formula**:
```
MDD = (Trough Value - Peak Value) / Peak Value
```

**Algorithm** (Running Maximum):
```
1. Initialize: runningMax = firstValue, maxDrawdown = 0
2. For each value in series:
   a. If value > runningMax: runningMax = value
   b. drawdown = (value - runningMax) / runningMax
   c. If drawdown < maxDrawdown: maxDrawdown = drawdown
3. Return maxDrawdown (as negative percentage)
```

**Implementation Details**:
- MDD always ≤ 0 (represents loss from peak)
- Reported as positive percentage for UI (e.g., "18.5% drawdown")
- Computed on daily portfolio values
- Identifies worst peak-to-trough decline

**Example**:
```
Portfolio values: [100K, 110K, 105K, 120K, 90K, 95K, 115K]

Running max:      [100K, 110K, 110K, 120K, 120K, 120K, 120K]
Drawdowns:        [0%,   0%,  -4.5%, 0%,  -25%, -20.8%, -4.2%]

MDD = -25% (from peak of $120K to trough of $90K)
```

**Properties** (verified by property-based tests):
- MDD is monotonic: adding data can only maintain or increase MDD (never decrease)
- MDD = 0 if portfolio only increases
- MDD ∈ [-100%, 0%]

---

### 4. Benchmark Comparison

**Implementation**:
- Fetch benchmark ticker (e.g., SPY) for same date range
- Calculate CAGR, Sharpe, MDD for benchmark using same algorithms
- Return side-by-side comparison in API response

**Relative Metrics**:
- **Alpha** (future): Portfolio CAGR - Benchmark CAGR
- **Beta** (future): Covariance(portfolio, benchmark) / Variance(benchmark)

---

## Rebalancing Algorithm

### Target Allocation Drift Detection

**Inputs**:
- Current positions with market values
- Target allocations (% weights, must sum to ~100%)
- Configuration: `driftBand` (e.g., 5%), `minNotional` (e.g., $100)

**Algorithm**:
```
1. Calculate total portfolio value: sum(position.marketValue)
2. For each position:
   a. currentWeight = position.marketValue / totalValue
   b. targetWeight = targetAllocation.weight (from settings)
   c. deviation = |currentWeight - targetWeight|
   d. If deviation > driftBand:
      - Calculate trade notional to bring within band
      - If notional ≥ minNotional: generate suggestion
3. Return list of RebalanceSuggestion { ticker, action, quantity, notional }
```

**Trade Calculation**:
```
targetNotional = totalValue × targetWeight
currentNotional = position.marketValue
tradeNotional = targetNotional - currentNotional

If tradeNotional > 0: BUY (underweight)
If tradeNotional < 0: SELL (overweight)

quantity = |tradeNotional| / currentPrice
```

**Example**:
```
Portfolio Value: $100,000
Drift Band: 5%
Min Notional: $100

Positions:
- AAPL: $55,000 (55%) | Target: 40% | Deviation: 15% → SELL
- MSFT: $30,000 (30%) | Target: 40% | Deviation: 10% → BUY
- GLD:  $15,000 (15%) | Target: 20% | Deviation: 5%  → BUY

Suggestions:
1. SELL AAPL: $55,000 - $40,000 = $15,000
2. BUY MSFT:  $40,000 - $30,000 = $10,000
3. BUY GLD:   $20,000 - $15,000 = $5,000

(Total trades: -$15K + $10K + $5K = $0, balanced)
```

**Properties** (verified by property-based tests):
- After rebalancing, all weights within ±driftBand of targets
- Sum of trade notionals ≈ 0 (cash-neutral, ignoring transaction costs)
- No suggestions for deviations ≤ driftBand
- No suggestions with notional < minNotional

---

## Cost Basis Calculation (FIFO)

**Purpose**: Calculate realized P&L for tax reporting and performance analysis

**Algorithm** (First-In-First-Out):
```
1. Maintain lot queue per ticker (ordered by purchase date)
2. On BUY transaction:
   - Add lot { quantity, price, date } to queue
3. On SELL transaction:
   - While sellQty > 0 and queue not empty:
     a. Pop oldest lot
     b. qtyToSell = min(sellQty, lot.quantity)
     c. realizedPnL += qtyToSell × (sellPrice - lot.price)
     d. If lot.quantity > qtyToSell: push remainder back
     e. sellQty -= qtyToSell
4. Return total realized P&L
```

**Example**:
```
Transactions:
1. BUY 100 AAPL @ $150 on 2023-01-01
2. BUY 50 AAPL @ $160 on 2023-06-01
3. SELL 120 AAPL @ $180 on 2024-01-01

Calculation:
- Sell 100 from lot 1: 100 × ($180 - $150) = +$3,000
- Sell 20 from lot 2:   20 × ($180 - $160) = +$400
Realized P&L: $3,400

Remaining lot: 30 AAPL @ $160
```

**Assumptions**:
- FIFO for tax compliance (US default)
- Ignores wash sales (future enhancement)
- Transaction costs not included (add as enhancement)

---

## Data Import Validation

### CSV Position Format
**Required columns**: `Ticker`, `Quantity`, `AvgCost`

**Validations**:
- Ticker: non-empty, uppercase, alphanumeric + hyphen
- Quantity: positive decimal
- AvgCost: non-negative decimal

### CSV Transaction Format
**Required columns**: `Date`, `Ticker`, `Type`, `Quantity`, `Price`

**Validations**:
- Date: ISO 8601 format (YYYY-MM-DD)
- Type: "Buy" or "Sell" (case-insensitive)
- Quantity: positive decimal
- Price: positive decimal

---

## Assumptions & Limitations

1. **Trading Days**: 252 days per year for annualization
2. **Risk-Free Rate**: Configurable, defaults to 4% (current T-bill rate)
3. **No Short Positions**: Quantities always ≥ 0
4. **Cash Not Tracked**: Rebalancing assumes sufficient cash; doesn't model cash as position
5. **Transaction Costs**: Ignored in v1 (add friction factor in future)
6. **Dividends**: Not included in return calculations (price-only)
7. **Currency**: Single currency (USD assumed)

---

## Performance Targets

- **10 years × 50 tickers**: Metrics calculation < 2 seconds (local dev machine)
- **Golden Test Tolerance**: CAGR ±0.01, Sharpe ±0.05, MDD ±0.005
- **Rebalancing**: < 100ms for typical portfolio (10-20 positions)

---

**Last Updated**: M0 Completion
