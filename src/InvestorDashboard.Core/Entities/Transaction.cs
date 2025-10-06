namespace InvestorDashboard.Core.Entities;

/// <summary>
/// Represents a transaction (buy, sell, dividend)
/// </summary>
public class Transaction
{
    public int Id { get; set; }
    
    public int SecurityId { get; set; }
    public Security Security { get; set; } = null!;
    
    /// <summary>
    /// Type of transaction
    /// </summary>
    public TransactionType Type { get; set; }
    
    /// <summary>
    /// Number of shares
    /// </summary>
    public decimal Quantity { get; set; }
    
    /// <summary>
    /// Price per share
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Transaction date
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Optional transaction fee/commission
    /// </summary>
    public decimal? Commission { get; set; }
    
    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// When this transaction was recorded in the system
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Total value of the transaction
    /// </summary>
    public decimal TotalValue => Quantity * Price;
    
    /// <summary>
    /// Total cost including commission
    /// </summary>
    public decimal TotalCost => TotalValue + (Commission ?? 0);
}

/// <summary>
/// Type of transaction
/// </summary>
public enum TransactionType
{
    Buy,
    Sell,
    Dividend
}
