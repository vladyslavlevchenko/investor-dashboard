export interface Security {
  id: number;
  ticker: string;
  name: string;
  assetClass: string;
  createdAt: string;
}

export interface CreateSecurityRequest {
  ticker: string;
  name: string;
  assetClass: string;
}

export interface PriceResponse {
  ticker: string;
  price: number;
  asOf: string;
}
