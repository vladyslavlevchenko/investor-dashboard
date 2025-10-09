import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Security, CreateSecurityRequest } from '../../models/security.model';
import { CsvImportComponent } from '../csv-import/csv-import.component';

@Component({
  selector: 'app-securities',
  imports: [CommonModule, FormsModule, CsvImportComponent],
  templateUrl: './securities.component.html',
  styleUrl: './securities.component.scss'
})
export class SecuritiesComponent implements OnInit {
  private readonly apiService = inject(ApiService);

  protected readonly securities = signal<Security[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly showAddForm = signal(false);
  protected readonly showCsvImport = signal(false);
  protected readonly prices = signal<Map<string, number>>(new Map());

  // Form fields
  protected newSecurity: CreateSecurityRequest = {
    ticker: '',
    name: '',
    assetClass: ''
  };

  ngOnInit(): void {
    this.loadSecurities();
  }

  loadSecurities(): void {
    this.loading.set(true);
    this.error.set(null);

    this.apiService.getAllSecurities().subscribe({
      next: (securities) => {
        this.securities.set(securities);
        this.loading.set(false);
        
        // Load prices for all securities
        securities.forEach(security => this.loadPrice(security.ticker));
      },
      error: (err) => {
        this.error.set('Failed to load securities: ' + err.message);
        this.loading.set(false);
      }
    });
  }

  loadPrice(ticker: string): void {
    this.apiService.getCurrentPrice(ticker).subscribe({
      next: (priceResponse) => {
        const currentPrices = this.prices();
        currentPrices.set(ticker, priceResponse.price);
        this.prices.set(new Map(currentPrices));
      },
      error: (err) => {
        console.error(`Failed to load price for ${ticker}:`, err);
      }
    });
  }

  toggleAddForm(): void {
    this.showAddForm.update(v => !v);
    if (!this.showAddForm()) {
      this.resetForm();
    }
    // Close CSV import if open
    if (this.showAddForm()) {
      this.showCsvImport.set(false);
    }
  }

  toggleCsvImport(): void {
    this.showCsvImport.update(v => !v);
    // Close manual form if open
    if (this.showCsvImport()) {
      this.showAddForm.set(false);
    }
  }

  onCsvImportComplete(): void {
    this.showCsvImport.set(false);
    this.loadSecurities();
  }

  addSecurity(): void {
    if (!this.newSecurity.ticker || !this.newSecurity.name || !this.newSecurity.assetClass) {
      this.error.set('All fields are required');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.apiService.createSecurity(this.newSecurity).subscribe({
      next: (security) => {
        this.securities.update(list => [...list, security]);
        this.loadPrice(security.ticker);
        this.resetForm();
        this.showAddForm.set(false);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to create security: ' + (err.error?.message || err.message));
        this.loading.set(false);
      }
    });
  }

  resetForm(): void {
    this.newSecurity = {
      ticker: '',
      name: '',
      assetClass: ''
    };
  }

  getPrice(ticker: string): number | null {
    return this.prices().get(ticker) ?? null;
  }
}
