using InvestorDashboard.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestorDashboard.Infrastructure.Data;

/// <summary>
/// Database context for the Investor Dashboard
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Security> Securities => Set<Security>();
    public DbSet<PositionLot> PositionLots => Set<PositionLot>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<PriceBar> PriceBars => Set<PriceBar>();
    public DbSet<PortfolioSettings> PortfolioSettings => Set<PortfolioSettings>();
    public DbSet<TargetAllocation> TargetAllocations => Set<TargetAllocation>();
    public DbSet<BenchmarkSeries> BenchmarkSeries => Set<BenchmarkSeries>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Security configuration
        modelBuilder.Entity<Security>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Ticker).IsUnique();
            entity.Property(e => e.Ticker).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.AssetClass).HasMaxLength(50).IsRequired();
        });

        // PositionLot configuration
        modelBuilder.Entity<PositionLot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SecurityId);
            entity.HasIndex(e => e.PurchaseDate);
            
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.CostBasis).HasPrecision(18, 4);
            
            entity.HasOne(e => e.Security)
                  .WithMany(s => s.PositionLots)
                  .HasForeignKey(e => e.SecurityId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SecurityId);
            entity.HasIndex(e => e.Date);
            
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.Price).HasPrecision(18, 4);
            entity.Property(e => e.Commission).HasPrecision(18, 4);
            
            entity.HasOne(e => e.Security)
                  .WithMany(s => s.Transactions)
                  .HasForeignKey(e => e.SecurityId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // PriceBar configuration
        modelBuilder.Entity<PriceBar>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SecurityId, e.Date }).IsUnique();
            
            entity.Property(e => e.Close).HasPrecision(18, 4);
            entity.Property(e => e.Open).HasPrecision(18, 4);
            entity.Property(e => e.High).HasPrecision(18, 4);
            entity.Property(e => e.Low).HasPrecision(18, 4);
            
            entity.HasOne(e => e.Security)
                  .WithMany(s => s.PriceBars)
                  .HasForeignKey(e => e.SecurityId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // PortfolioSettings configuration
        modelBuilder.Entity<PortfolioSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.RiskFreeRate).HasPrecision(10, 6);
            entity.Property(e => e.MinNotional).HasPrecision(18, 2);
            entity.Property(e => e.DriftBandPercent).HasPrecision(10, 6);
        });

        // TargetAllocation configuration
        modelBuilder.Entity<TargetAllocation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PortfolioSettingsId, e.Ticker }).IsUnique();
            
            entity.Property(e => e.Ticker).HasMaxLength(10).IsRequired();
            entity.Property(e => e.TargetWeight).HasPrecision(10, 6);
            
            entity.HasOne(e => e.PortfolioSettings)
                  .WithMany(ps => ps.TargetAllocations)
                  .HasForeignKey(e => e.PortfolioSettingsId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // BenchmarkSeries configuration
        modelBuilder.Entity<BenchmarkSeries>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Symbol, e.Date }).IsUnique();
            
            entity.Property(e => e.Symbol).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Close).HasPrecision(18, 4);
        });

        // Seed default portfolio settings
        modelBuilder.Entity<PortfolioSettings>().HasData(
            new PortfolioSettings
            {
                Id = 1,
                RiskFreeRate = 0.04m,
                MinNotional = 100m,
                DriftBandPercent = 0.05m,
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
