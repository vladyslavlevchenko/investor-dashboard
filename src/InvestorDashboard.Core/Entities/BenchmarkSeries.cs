namespace InvestorDashboard.Core.Entities;

/// <summary>
/// Benchmark series for comparison (e.g., SPY, AGG)
/// </summary>
public class BenchmarkSeries
{
    public int Id { get; set; }

    /// <summary>
    /// Benchmark symbol (e.g., SPY, AGG)
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Date of this price point
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Closing price
    /// </summary>
    public decimal Close { get; set; }

    /// <summary>
    /// When this data was fetched
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
