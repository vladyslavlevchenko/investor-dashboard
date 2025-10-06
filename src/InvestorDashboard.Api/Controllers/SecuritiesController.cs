using InvestorDashboard.Core.Entities;
using InvestorDashboard.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvestorDashboard.Api.Controllers;

/// <summary>
/// Controller for managing securities (stocks, ETFs, etc.)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SecuritiesController : ControllerBase
{
    private readonly IPortfolioRepository _repository;
    private readonly IMarketDataProvider _marketDataProvider;
    private readonly ILogger<SecuritiesController> _logger;

    public SecuritiesController(
        IPortfolioRepository repository,
        IMarketDataProvider marketDataProvider,
        ILogger<SecuritiesController> logger)
    {
        _repository = repository;
        _marketDataProvider = marketDataProvider;
        _logger = logger;
    }

    /// <summary>
    /// Get all securities
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Security>>> GetAll(CancellationToken cancellationToken)
    {
        var securities = await _repository.GetAllSecuritiesAsync(cancellationToken);
        return Ok(securities);
    }

    /// <summary>
    /// Get a security by ticker symbol
    /// </summary>
    /// <param name="ticker">Ticker symbol (e.g., AAPL, GOOGL)</param>
    [HttpGet("{ticker}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Security>> GetByTicker(
        string ticker,
        CancellationToken cancellationToken)
    {
        var security = await _repository.GetSecurityByTickerAsync(ticker, cancellationToken);
        if (security == null)
        {
            return NotFound(new { Message = $"Security with ticker '{ticker}' not found" });
        }

        return Ok(security);
    }

    /// <summary>
    /// Add a new security
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Security>> Create(
        [FromBody] CreateSecurityRequest request,
        CancellationToken cancellationToken)
    {
        // Check if ticker already exists
        var existing = await _repository.GetSecurityByTickerAsync(request.Ticker, cancellationToken);
        if (existing != null)
        {
            return BadRequest(new { Message = $"Security with ticker '{request.Ticker}' already exists" });
        }

        var security = new Security
        {
            Ticker = request.Ticker.ToUpperInvariant(),
            Name = request.Name,
            AssetClass = request.AssetClass
        };

        var created = await _repository.AddSecurityAsync(security, cancellationToken);
        _logger.LogInformation("Created security {Ticker} with ID {Id}", created.Ticker, created.Id);

        return CreatedAtAction(
            nameof(GetByTicker),
            new { ticker = created.Ticker },
            created);
    }

    /// <summary>
    /// Get current price for a security
    /// </summary>
    [HttpGet("{ticker}/price")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PriceResponse>> GetCurrentPrice(
        string ticker,
        CancellationToken cancellationToken)
    {
        var security = await _repository.GetSecurityByTickerAsync(ticker, cancellationToken);
        if (security == null)
        {
            return NotFound(new { Message = $"Security with ticker '{ticker}' not found" });
        }

        var price = await _marketDataProvider.GetCurrentPriceAsync(ticker, cancellationToken);
        if (price == null)
        {
            return NotFound(new { Message = $"Price not available for ticker '{ticker}'" });
        }

        return Ok(new PriceResponse
        {
            Ticker = ticker,
            Price = price.Value,
            AsOf = DateTime.Today
        });
    }
}

/// <summary>
/// Request model for creating a new security
/// </summary>
public record CreateSecurityRequest
{
    public required string Ticker { get; init; }
    public required string Name { get; init; }
    public required string AssetClass { get; init; }
}

/// <summary>
/// Response model for current price
/// </summary>
public record PriceResponse
{
    public required string Ticker { get; init; }
    public required decimal Price { get; init; }
    public required DateTime AsOf { get; init; }
}
