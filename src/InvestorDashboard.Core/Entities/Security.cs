namespace InvestorDashboard.Core.Entities;

/// <summary>
/// Represents a security (stock, ETF, etc.)
/// </summary>
public class Security
{
    public int Id { get; set; }

    /// <summary>
    /// Ticker symbol (e.g., AAPL, SPY)
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the security
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Asset class (e.g., Stock, ETF, Bond)
    /// </summary>
    public string AssetClass { get; set; } = "Stock";

    /// <summary>
    /// When this security was added to the system
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<PositionLot> PositionLots { get; set; } = new List<PositionLot>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<PriceBar> PriceBars { get; set; } = new List<PriceBar>();
}
