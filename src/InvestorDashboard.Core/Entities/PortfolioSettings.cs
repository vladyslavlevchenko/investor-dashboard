namespace InvestorDashboard.Core.Entities;

/// <summary>
/// Portfolio settings including target allocations
/// </summary>
public class PortfolioSettings
{
    public int Id { get; set; }
    
    /// <summary>
    /// Risk-free rate for Sharpe ratio calculation (annual rate, e.g., 0.04 for 4%)
    /// </summary>
    public decimal RiskFreeRate { get; set; } = 0.04m;
    
    /// <summary>
    /// Minimum notional value for rebalancing trades (e.g., $100)
    /// </summary>
    public decimal MinNotional { get; set; } = 100m;
    
    /// <summary>
    /// Drift band percentage for rebalancing (e.g., 0.05 for ±5%)
    /// </summary>
    public decimal DriftBandPercent { get; set; } = 0.05m;
    
    /// <summary>
    /// When these settings were last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<TargetAllocation> TargetAllocations { get; set; } = new List<TargetAllocation>();
}
