# Import Formats

## Overview

The Investor Dashboard supports CSV imports for positions and transactions. This document specifies the required formats.

---

## Positions Import

### File Format
- **Type**: CSV (Comma-Separated Values)
- **Encoding**: UTF-8
- **Header Row**: Required
- **Max File Size**: 10 MB

### Required Columns

| Column   | Type    | Required | Description                          | Example  |
|----------|---------|----------|--------------------------------------|----------|
| Ticker   | String  | Yes      | Stock ticker symbol (uppercase)      | AAPL     |
| Quantity | Decimal | Yes      | Number of shares (positive)          | 100.5    |
| AvgCost  | Decimal | Yes      | Average cost per share (non-negative)| 150.25   |

### Optional Columns

| Column      | Type   | Description                  | Example    |
|-------------|--------|------------------------------|------------|
| LastUpdated | Date   | Last update timestamp        | 2024-01-15 |
| Notes       | String | Additional notes             | "Long-term"|

### Validation Rules

1. **Ticker**:
   - Non-empty
   - Alphanumeric + hyphen only
   - Converted to uppercase
   - Max 10 characters

2. **Quantity**:
   - Must be > 0
   - Up to 4 decimal places
   - No scientific notation

3. **AvgCost**:
   - Must be ≥ 0
   - Up to 4 decimal places
   - No scientific notation

### Example CSV

```csv
Ticker,Quantity,AvgCost
AAPL,100,150.00
MSFT,50.5,350.25
GLD,200,170.00
SPY,75.25,420.50
```

### Import Behavior

- **New Positions**: Created with provided data
- **Existing Positions**: 
  - Quantity and AvgCost **replaced** (not added)
  - Warning returned in response
- **Duplicates in File**: Last occurrence wins
- **Invalid Rows**: Skipped with error message; valid rows still imported

### API Endpoint

```http
POST /api/imports/positions
Content-Type: multipart/form-data

file: [CSV file]
portfolioId: 1 (optional)
```

### Response Example

```json
{
  "success": true,
  "imported": 4,
  "updated": 1,
  "errors": [],
  "warnings": [
    "Row 2: MSFT position updated (previously had 40 shares)"
  ]
}
```

---

## Transactions Import

### File Format
- **Type**: CSV (Comma-Separated Values)
- **Encoding**: UTF-8
- **Header Row**: Required
- **Max File Size**: 10 MB

### Required Columns

| Column   | Type    | Required | Description                     | Example      |
|----------|---------|----------|---------------------------------|--------------|
| Date     | Date    | Yes      | Transaction date (YYYY-MM-DD)   | 2024-01-15   |
| Ticker   | String  | Yes      | Stock ticker symbol             | AAPL         |
| Type     | String  | Yes      | "Buy" or "Sell" (case-insensitive) | Buy       |
| Quantity | Decimal | Yes      | Number of shares (positive)     | 100          |
| Price    | Decimal | Yes      | Price per share (positive)      | 150.25       |

### Optional Columns

| Column        | Type    | Description                   | Example      |
|---------------|---------|-------------------------------|--------------|
| Commission    | Decimal | Transaction fee               | 9.99         |
| Notes         | String  | Additional notes              | "Initial buy"|

### Validation Rules

1. **Date**:
   - Format: YYYY-MM-DD (ISO 8601)
   - Must be valid calendar date
   - Cannot be in the future
   - Must be after 1900-01-01

2. **Ticker**:
   - Non-empty
   - Alphanumeric + hyphen only
   - Converted to uppercase
   - Max 10 characters

3. **Type**:
   - Must be "Buy" or "Sell" (case-insensitive)
   - Normalized to "Buy"/"Sell" on import

4. **Quantity**:
   - Must be > 0
   - Up to 4 decimal places

5. **Price**:
   - Must be > 0
   - Up to 4 decimal places

6. **Commission** (optional):
   - Must be ≥ 0
   - Up to 2 decimal places

### Example CSV

```csv
Date,Ticker,Type,Quantity,Price,Commission
2024-01-15,AAPL,Buy,100,150.00,9.99
2024-02-20,MSFT,Buy,50,350.25,9.99
2024-06-10,AAPL,Sell,25,175.50,9.99
2024-09-01,GLD,Buy,200,170.00,14.99
```

### Import Behavior

- **Chronological Order**: Sorted by date automatically
- **Cost Basis**: Used for FIFO calculations
- **Position Updates**: Automatically updates positions table
- **Duplicates**: Allowed (same ticker, date, type) - creates separate records
- **Invalid Rows**: Skipped with error message; valid rows still imported

### Business Rules

1. **Sell Validation**:
   - System checks if sufficient quantity exists
   - Warning (not error) if selling more than held
   - FIFO applied: oldest lots sold first

2. **Cost Basis Calculation**:
   - Buy: Adds lot to FIFO queue
   - Sell: Consumes oldest lots, calculates realized P&L

### API Endpoint

```http
POST /api/imports/transactions
Content-Type: multipart/form-data

file: [CSV file]
portfolioId: 1 (optional)
```

### Response Example

```json
{
  "success": true,
  "imported": 4,
  "errors": [],
  "warnings": [
    "Row 3: Selling 25 AAPL - sufficient quantity available (100 held)"
  ],
  "summary": {
    "buys": 3,
    "sells": 1,
    "totalCost": 52509.96,
    "totalProceeds": 4378.51
  }
}
```

---

## Common Error Messages

### File Errors
- `"File is required"` - No file uploaded
- `"File must be a CSV"` - Wrong file type (checked by extension and content-type)
- `"File exceeds maximum size of 10 MB"` - File too large
- `"File is empty"` - No data rows after header

### Format Errors
- `"Row X: Missing required column 'Ticker'"` - Column not found in header
- `"Row X: Invalid date format 'ABC'. Use YYYY-MM-DD"` - Date parsing failed
- `"Row X: Invalid number format for 'Quantity'"` - Not a valid decimal
- `"Row X: Empty value for required field 'Type'"` - Required field missing

### Validation Errors
- `"Row X: Quantity must be positive, got -10"` - Negative quantity
- `"Row X: Invalid ticker 'A@PPL'. Use alphanumeric and hyphen only"` - Invalid characters
- `"Row X: Transaction date '2026-01-01' is in the future"` - Future date
- `"Row X: Type must be 'Buy' or 'Sell', got 'Trade'"` - Invalid transaction type

---

## Best Practices

### 1. Data Preparation
- Remove empty rows at end of file
- Ensure consistent decimal formatting (use period, not comma)
- Validate dates before export from source system
- Use UTF-8 encoding (avoid Excel's default encoding issues)

### 2. Excel Export Tips
```
1. Prepare data in Excel
2. Save As → CSV UTF-8 (Comma delimited) (*.csv)
3. Verify in text editor before upload
```

### 3. Large Files
- For files > 1000 rows, split into multiple uploads
- Upload transactions chronologically (oldest first)
- Monitor import responses for warnings

### 4. Testing
- Test with small sample file first (5-10 rows)
- Verify positions/transactions appear correctly in UI
- Check that metrics calculate as expected

---

## Export for Backup

The system can export current positions and full transaction history in the same CSV formats:

```http
GET /api/export/positions.csv
GET /api/export/transactions.csv
```

These exports can be re-imported to restore data or migrate to another instance.

---

## Future Enhancements (Out of Scope for v1)

- [ ] Support for Excel (XLSX) files
- [ ] Automatic broker statement parsing (PDF/OFX)
- [ ] Multi-currency support
- [ ] Crypto asset imports
- [ ] Options/futures formats
- [ ] Bulk validation endpoint (pre-check without import)

---

**Last Updated**: M0 Completion
