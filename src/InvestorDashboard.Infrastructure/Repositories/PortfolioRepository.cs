using InvestorDashboard.Core.Entities;
using InvestorDashboard.Core.Interfaces;
using InvestorDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvestorDashboard.Infrastructure.Repositories;

/// <summary>
/// Implementation of portfolio repository using Entity Framework Core.
/// Provides CRUD operations for all portfolio entities.
/// </summary>
public class PortfolioRepository : IPortfolioRepository
{
    private readonly AppDbContext _context;

    public PortfolioRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #region Security Operations

    public async Task<Security?> GetSecurityByTickerAsync(string ticker, CancellationToken cancellationToken = default)
    {
        return await _context.Securities
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Ticker == ticker, cancellationToken);
    }

    public async Task<Security> AddSecurityAsync(Security security, CancellationToken cancellationToken = default)
    {
        _context.Securities.Add(security);
        await _context.SaveChangesAsync(cancellationToken);
        return security;
    }

    public async Task<IEnumerable<Security>> GetAllSecuritiesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Securities
            .AsNoTracking()
            .OrderBy(s => s.Ticker)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Position Lot Operations

    public async Task<IEnumerable<PositionLot>> GetPositionLotsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PositionLots
            .AsNoTracking()
            .Include(p => p.Security)
            .Where(p => p.Quantity > 0)
            .OrderBy(p => p.Security.Ticker)
            .ThenBy(p => p.PurchaseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PositionLot>> GetPositionLotsByTickerAsync(string ticker, CancellationToken cancellationToken = default)
    {
        var security = await GetSecurityByTickerAsync(ticker, cancellationToken);
        if (security == null)
            return Enumerable.Empty<PositionLot>();

        return await _context.PositionLots
            .AsNoTracking()
            .Where(p => p.SecurityId == security.Id && p.Quantity > 0)
            .OrderBy(p => p.PurchaseDate) // FIFO order
            .ToListAsync(cancellationToken);
    }

    public async Task<PositionLot> AddPositionLotAsync(PositionLot lot, CancellationToken cancellationToken = default)
    {
        _context.PositionLots.Add(lot);
        await _context.SaveChangesAsync(cancellationToken);
        return lot;
    }

    public async Task AddPositionLotsAsync(IEnumerable<PositionLot> lots, CancellationToken cancellationToken = default)
    {
        _context.PositionLots.AddRange(lots);
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Transaction Operations

    public async Task<IEnumerable<Transaction>> GetTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Security)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByTickerAsync(string ticker, CancellationToken cancellationToken = default)
    {
        var security = await GetSecurityByTickerAsync(ticker, cancellationToken);
        if (security == null)
            return Enumerable.Empty<Transaction>();

        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.SecurityId == security.Id)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<Transaction> AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);
        return transaction;
    }

    public async Task AddTransactionsAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default)
    {
        _context.Transactions.AddRange(transactions);
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Price Bar Operations

    public async Task<IEnumerable<PriceBar>> GetPriceBarsAsync(string ticker, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var security = await GetSecurityByTickerAsync(ticker, cancellationToken);
        if (security == null)
            return Enumerable.Empty<PriceBar>();

        return await _context.PriceBars
            .AsNoTracking()
            .Where(p => p.SecurityId == security.Id
                     && p.Date >= from
                     && p.Date <= to)
            .OrderBy(p => p.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task AddPriceBarsAsync(IEnumerable<PriceBar> priceBars, CancellationToken cancellationToken = default)
    {
        _context.PriceBars.AddRange(priceBars);
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Portfolio Settings Operations

    public async Task<PortfolioSettings?> GetPortfolioSettingsAsync(CancellationToken cancellationToken = default)
    {
        // Return the default settings (Id = 1) or create if not exists
        var settings = await _context.PortfolioSettings
            .Include(s => s.TargetAllocations)
            .FirstOrDefaultAsync(cancellationToken);

        if (settings == null)
        {
            // This shouldn't happen due to seed data, but handle it gracefully
            settings = new PortfolioSettings
            {
                RiskFreeRate = 0.04m,
                MinNotional = 100.0m,
                DriftBandPercent = 0.05m
            };
            _context.PortfolioSettings.Add(settings);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return settings;
    }

    public async Task<PortfolioSettings> UpdatePortfolioSettingsAsync(PortfolioSettings settings, CancellationToken cancellationToken = default)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _context.PortfolioSettings.Update(settings);
        await _context.SaveChangesAsync(cancellationToken);
        return settings;
    }

    #endregion

    #region Target Allocation Operations

    public async Task<IEnumerable<TargetAllocation>> GetTargetAllocationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TargetAllocations
            .AsNoTracking()
            .OrderBy(t => t.Ticker)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateTargetAllocationsAsync(IEnumerable<TargetAllocation> allocations, CancellationToken cancellationToken = default)
    {
        // Remove all existing allocations
        var existing = await _context.TargetAllocations.ToListAsync(cancellationToken);
        _context.TargetAllocations.RemoveRange(existing);

        // Add new allocations with timestamps
        var now = DateTime.UtcNow;
        foreach (var allocation in allocations)
        {
            allocation.CreatedAt = now;
            allocation.UpdatedAt = now;
        }
        _context.TargetAllocations.AddRange(allocations);
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Benchmark Operations

    public async Task<IEnumerable<BenchmarkSeries>> GetBenchmarkSeriesAsync(string symbol, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _context.BenchmarkSeries
            .AsNoTracking()
            .Where(b => b.Symbol == symbol
                     && b.Date >= from
                     && b.Date <= to)
            .OrderBy(b => b.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task AddBenchmarkSeriesAsync(IEnumerable<BenchmarkSeries> series, CancellationToken cancellationToken = default)
    {
        _context.BenchmarkSeries.AddRange(series);
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
