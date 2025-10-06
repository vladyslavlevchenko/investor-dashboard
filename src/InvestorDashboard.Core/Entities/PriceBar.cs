namespace InvestorDashboard.Core.Entities;

/// <summary>
/// Stores historical price data for a security
/// </summary>
public class PriceBar
{
    public int Id { get; set; }

    public int SecurityId { get; set; }
    public Security Security { get; set; } = null!;

    /// <summary>
    /// Date of this price bar
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Closing price
    /// </summary>
    public decimal Close { get; set; }

    /// <summary>
    /// Optional: Opening price
    /// </summary>
    public decimal? Open { get; set; }

    /// <summary>
    /// Optional: Highest price during the day
    /// </summary>
    public decimal? High { get; set; }

    /// <summary>
    /// Optional: Lowest price during the day
    /// </summary>
    public decimal? Low { get; set; }

    /// <summary>
    /// Optional: Trading volume
    /// </summary>
    public long? Volume { get; set; }

    /// <summary>
    /// When this price was fetched/stored
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
