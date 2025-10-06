namespace InvestorDashboard.Core.Entities;

/// <summary>
/// Represents a lot (batch) of shares purchased at a specific time and price
/// Used for FIFO cost basis calculation
/// </summary>
public class PositionLot
{
    public int Id { get; set; }
    
    public int SecurityId { get; set; }
    public Security Security { get; set; } = null!;
    
    /// <summary>
    /// Number of shares in this lot
    /// </summary>
    public decimal Quantity { get; set; }
    
    /// <summary>
    /// Cost basis per share (what was paid per share)
    /// </summary>
    public decimal CostBasis { get; set; }
    
    /// <summary>
    /// Date when this lot was purchased
    /// </summary>
    public DateTime PurchaseDate { get; set; }
    
    /// <summary>
    /// Date when this lot was created/updated in the system
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Total cost of this lot
    /// </summary>
    public decimal TotalCost => Quantity * CostBasis;
}
