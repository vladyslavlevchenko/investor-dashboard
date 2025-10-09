import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Security, CreateSecurityRequest, PriceResponse } from '../models/security.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5092/api';

  // Securities endpoints
  getAllSecurities(): Observable<Security[]> {
    return this.http.get<Security[]>(`${this.baseUrl}/securities`);
  }

  getSecurityByTicker(ticker: string): Observable<Security> {
    return this.http.get<Security>(`${this.baseUrl}/securities/${ticker}`);
  }

  createSecurity(request: CreateSecurityRequest): Observable<Security> {
    return this.http.post<Security>(`${this.baseUrl}/securities`, request);
  }

  getCurrentPrice(ticker: string): Observable<PriceResponse> {
    return this.http.get<PriceResponse>(`${this.baseUrl}/securities/${ticker}/price`);
  }
}
