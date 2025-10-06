using InvestorDashboard.Core.Entities;

namespace InvestorDashboard.Core.Interfaces;

/// <summary>
/// Abstraction for fetching market data (prices)
/// Implementations: Mock, Yahoo Finance, Alpha Vantage, etc.
/// </summary>
public interface IMarketDataProvider
{
    /// <summary>
    /// Get the current (latest) price for a ticker
    /// </summary>
    Task<decimal?> GetCurrentPriceAsync(string ticker, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get historical daily prices for a ticker within a date range
    /// </summary>
    Task<IEnumerable<PriceBar>> GetDailyPricesAsync(
        string ticker,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get multiple tickers' prices in a single call (optional optimization)
    /// </summary>
    Task<Dictionary<string, decimal?>> GetCurrentPricesAsync(
        IEnumerable<string> tickers,
        CancellationToken cancellationToken = default);
}
