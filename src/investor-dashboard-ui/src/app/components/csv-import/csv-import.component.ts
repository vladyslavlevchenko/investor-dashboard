import { Component, signal, inject, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CsvService, CsvSecurityRow } from '../../services/csv.service';
import { ApiService } from '../../services/api.service';
import { CreateSecurityRequest } from '../../models/security.model';

@Component({
  selector: 'app-csv-import',
  imports: [CommonModule],
  templateUrl: './csv-import.component.html',
  styleUrl: './csv-import.component.scss'
})
export class CsvImportComponent {
  private readonly csvService = inject(CsvService);
  private readonly apiService = inject(ApiService);

  // Output event when import is complete
  readonly importComplete = output<void>();

  protected readonly isDragging = signal(false);
  protected readonly isProcessing = signal(false);
  protected readonly parsedSecurities = signal<CsvSecurityRow[]>([]);
  protected readonly importResults = signal<{
    success: string[];
    failed: { ticker: string; error: string }[];
  } | null>(null);
  protected readonly error = signal<string | null>(null);

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.handleFile(input.files[0]);
    }
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragging.set(false);

    const files = event.dataTransfer?.files;
    if (files && files[0]) {
      this.handleFile(files[0]);
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragging.set(true);
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragging.set(false);
  }

  private handleFile(file: File): void {
    if (!file.name.endsWith('.csv')) {
      this.error.set('Please upload a CSV file');
      return;
    }

    this.error.set(null);
    this.importResults.set(null);
    
    const reader = new FileReader();
    reader.onload = (e) => {
      const content = e.target?.result as string;
      this.parseAndPreview(content);
    };
    reader.onerror = () => {
      this.error.set('Failed to read file');
    };
    reader.readAsText(file);
  }

  private parseAndPreview(content: string): void {
    try {
      const securities = this.csvService.parseSecuritiesCsv(content);
      this.parsedSecurities.set(securities);
      this.error.set(null);
    } catch (err: any) {
      this.error.set(err.message);
      this.parsedSecurities.set([]);
    }
  }

  async importSecurities(): Promise<void> {
    const securities = this.parsedSecurities();
    if (securities.length === 0) return;

    this.isProcessing.set(true);
    this.error.set(null);

    const success: string[] = [];
    const failed: { ticker: string; error: string }[] = [];

    // Import securities one by one
    for (const security of securities) {
      try {
        const request: CreateSecurityRequest = {
          ticker: security.ticker,
          name: security.name,
          assetClass: security.assetClass
        };
        
        await this.apiService.createSecurity(request).toPromise();
        success.push(security.ticker);
      } catch (err: any) {
        const errorMessage = err.error?.message || err.message || 'Unknown error';
        failed.push({ ticker: security.ticker, error: errorMessage });
      }
    }

    this.importResults.set({ success, failed });
    this.isProcessing.set(false);

    // If any succeeded, notify parent
    if (success.length > 0) {
      this.importComplete.emit();
    }
  }

  downloadSample(): void {
    this.csvService.downloadSampleCsv();
  }

  reset(): void {
    this.parsedSecurities.set([]);
    this.importResults.set(null);
    this.error.set(null);
  }
}
