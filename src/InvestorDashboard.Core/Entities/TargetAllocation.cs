namespace InvestorDashboard.Core.Entities;

/// <summary>
/// Target allocation for a specific ticker
/// </summary>
public class TargetAllocation
{
    public int Id { get; set; }
    
    public int PortfolioSettingsId { get; set; }
    public PortfolioSettings PortfolioSettings { get; set; } = null!;
    
    /// <summary>
    /// Ticker symbol for this allocation
    /// </summary>
    public string Ticker { get; set; } = string.Empty;
    
    /// <summary>
    /// Target weight as a percentage (e.g., 0.40 for 40%)
    /// </summary>
    public decimal TargetWeight { get; set; }
    
    /// <summary>
    /// When this allocation was set/updated
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}
