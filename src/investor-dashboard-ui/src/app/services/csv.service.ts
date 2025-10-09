import { Injectable } from '@angular/core';

export interface CsvSecurityRow {
  ticker: string;
  name: string;
  assetClass: string;
}

@Injectable({
  providedIn: 'root'
})
export class CsvService {
  
  /**
   * Parse CSV file containing securities data
   * Expected format: Ticker,Name,AssetClass
   */
  parseSecuritiesCsv(fileContent: string): CsvSecurityRow[] {
    const lines = fileContent.split('\n').filter(line => line.trim());
    
    if (lines.length === 0) {
      throw new Error('CSV file is empty');
    }

    // Skip header row if it exists
    const firstLine = lines[0].toLowerCase();
    const startIndex = firstLine.includes('ticker') || firstLine.includes('symbol') ? 1 : 0;
    
    const securities: CsvSecurityRow[] = [];
    const errors: string[] = [];

    for (let i = startIndex; i < lines.length; i++) {
      const line = lines[i].trim();
      if (!line) continue;

      const parts = line.split(',').map(p => p.trim());
      
      if (parts.length < 3) {
        errors.push(`Line ${i + 1}: Invalid format (expected 3 columns, got ${parts.length})`);
        continue;
      }

      const [ticker, name, assetClass] = parts;

      // Validation
      if (!ticker) {
        errors.push(`Line ${i + 1}: Ticker is required`);
        continue;
      }

      if (!name) {
        errors.push(`Line ${i + 1}: Name is required`);
        continue;
      }

      if (!assetClass) {
        errors.push(`Line ${i + 1}: Asset class is required`);
        continue;
      }

      // Validate ticker format (alphanumeric, dots, hyphens)
      if (!/^[A-Z0-9.-]+$/i.test(ticker)) {
        errors.push(`Line ${i + 1}: Invalid ticker format "${ticker}"`);
        continue;
      }

      securities.push({
        ticker: ticker.toUpperCase(),
        name,
        assetClass
      });
    }

    if (errors.length > 0 && securities.length === 0) {
      throw new Error('CSV parsing failed:\n' + errors.join('\n'));
    }

    return securities;
  }

  /**
   * Generate a sample CSV template
   */
  generateSampleCsv(): string {
    return `Ticker,Name,AssetClass
AAPL,Apple Inc.,US Equity
GOOGL,Alphabet Inc.,US Equity
MSFT,Microsoft Corporation,US Equity
TSLA,Tesla Inc.,US Equity
SPY,SPDR S&P 500 ETF,US ETF
AGG,iShares Core U.S. Aggregate Bond ETF,US Bond ETF`;
  }

  /**
   * Download sample CSV file
   */
  downloadSampleCsv(): void {
    const csv = this.generateSampleCsv();
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'securities_sample.csv';
    link.click();
    window.URL.revokeObjectURL(url);
  }
}
