using InvestorDashboard.Core.Entities;

namespace InvestorDashboard.Core.Interfaces;

/// <summary>
/// Repository for portfolio data access
/// </summary>
public interface IPortfolioRepository
{
    // Securities
    Task<Security?> GetSecurityByTickerAsync(string ticker, CancellationToken cancellationToken = default);
    Task<Security> AddSecurityAsync(Security security, CancellationToken cancellationToken = default);
    Task<IEnumerable<Security>> GetAllSecuritiesAsync(CancellationToken cancellationToken = default);

    // Position Lots
    Task<IEnumerable<PositionLot>> GetPositionLotsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PositionLot>> GetPositionLotsByTickerAsync(string ticker, CancellationToken cancellationToken = default);
    Task<PositionLot> AddPositionLotAsync(PositionLot lot, CancellationToken cancellationToken = default);
    Task AddPositionLotsAsync(IEnumerable<PositionLot> lots, CancellationToken cancellationToken = default);

    // Transactions
    Task<IEnumerable<Transaction>> GetTransactionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetTransactionsByTickerAsync(string ticker, CancellationToken cancellationToken = default);
    Task<Transaction> AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task AddTransactionsAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default);

    // Price Bars
    Task<IEnumerable<PriceBar>> GetPriceBarsAsync(string ticker, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task AddPriceBarsAsync(IEnumerable<PriceBar> priceBars, CancellationToken cancellationToken = default);

    // Portfolio Settings
    Task<PortfolioSettings?> GetPortfolioSettingsAsync(CancellationToken cancellationToken = default);
    Task<PortfolioSettings> UpdatePortfolioSettingsAsync(PortfolioSettings settings, CancellationToken cancellationToken = default);

    // Target Allocations
    Task<IEnumerable<TargetAllocation>> GetTargetAllocationsAsync(CancellationToken cancellationToken = default);
    Task UpdateTargetAllocationsAsync(IEnumerable<TargetAllocation> allocations, CancellationToken cancellationToken = default);

    // Benchmark
    Task<IEnumerable<BenchmarkSeries>> GetBenchmarkSeriesAsync(string symbol, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task AddBenchmarkSeriesAsync(IEnumerable<BenchmarkSeries> series, CancellationToken cancellationToken = default);
}
