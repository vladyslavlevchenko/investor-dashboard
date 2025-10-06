using System.Security.Cryptography;
using System.Text;
using InvestorDashboard.Core.Entities;
using InvestorDashboard.Core.Interfaces;

namespace InvestorDashboard.Infrastructure.Providers;

/// <summary>
/// Mock market data provider for testing and development.
/// Returns deterministic prices based on ticker hash and date.
/// Implements ADR-005: Use mock provider until external API integration.
/// </summary>
public class MockMarketDataProvider : IMarketDataProvider
{
    private const decimal BasePrice = 100.0m;
    private const decimal DailyGrowthRate = 0.0002m; // ~5% annual growth
    private const decimal Volatility = 0.02m; // 2% daily volatility

    public Task<decimal?> GetCurrentPriceAsync(string ticker, CancellationToken cancellationToken = default)
    {
        var price = CalculatePrice(ticker, DateTime.Today);
        return Task.FromResult<decimal?>(price);
    }

    public Task<IEnumerable<PriceBar>> GetDailyPricesAsync(
        string ticker,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var priceBars = new List<PriceBar>();

        for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
        {
            // Skip weekends (Saturday = 6, Sunday = 0)
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                continue;

            var price = CalculatePrice(ticker, date);
            priceBars.Add(new PriceBar
            {
                SecurityId = 0, // Will be set by caller
                Date = date,
                Close = price,
                Open = price * 0.995m, // Slightly lower open
                High = price * 1.01m,  // Slightly higher high
                Low = price * 0.99m,   // Slightly lower low
                Volume = 1000000 + (int)(price * 10000) // Deterministic volume
            });
        }

        return Task.FromResult<IEnumerable<PriceBar>>(priceBars);
    }

    public async Task<Dictionary<string, decimal?>> GetCurrentPricesAsync(
        IEnumerable<string> tickers,
        CancellationToken cancellationToken = default)
    {
        var prices = new Dictionary<string, decimal?>();

        foreach (var ticker in tickers)
        {
            prices[ticker] = await GetCurrentPriceAsync(ticker, cancellationToken);
        }

        return prices;
    }

    /// <summary>
    /// Calculate a deterministic price based on ticker and date.
    /// Uses ticker hash for base offset and linear growth over time.
    /// </summary>
    private static decimal CalculatePrice(string ticker, DateTime date)
    {
        // Get a deterministic offset based on ticker (0-1 range)
        var tickerOffset = GetTickerOffset(ticker);

        // Calculate days since epoch (Jan 1, 2020)
        var epoch = new DateTime(2020, 1, 1);
        var daysSinceEpoch = (date - epoch).Days;

        // Base price adjusted by ticker (ranges from 50 to 500)
        var adjustedBase = BasePrice * (0.5m + (tickerOffset * 4.5m));

        // Apply linear growth
        var growthFactor = 1.0m + (DailyGrowthRate * daysSinceEpoch);

        // Add deterministic "volatility" based on date hash
        var dateHash = GetDateHash(ticker, date);
        var volatilityFactor = 1.0m + (Volatility * (dateHash - 0.5m));

        var price = adjustedBase * growthFactor * volatilityFactor;

        // Round to 2 decimal places
        return Math.Round(price, 2);
    }

    /// <summary>
    /// Get a deterministic value (0-1) based on ticker.
    /// </summary>
    private static decimal GetTickerOffset(string ticker)
    {
        var hash = ComputeHash(ticker);
        return (decimal)hash / uint.MaxValue;
    }

    /// <summary>
    /// Get a deterministic value (0-1) based on ticker and date combination.
    /// </summary>
    private static decimal GetDateHash(string ticker, DateTime date)
    {
        var combined = $"{ticker}:{date:yyyy-MM-dd}";
        var hash = ComputeHash(combined);
        return (decimal)hash / uint.MaxValue;
    }

    /// <summary>
    /// Compute a stable 32-bit hash from a string.
    /// </summary>
    private static uint ComputeHash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = MD5.HashData(bytes);

        // Take first 4 bytes and convert to uint
        return BitConverter.ToUInt32(hash, 0);
    }
}
