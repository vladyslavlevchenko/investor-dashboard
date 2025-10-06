# Investor Dashboard Project Instructions

## Project Overview
This is a comprehensive investor dashboard built with .NET 8 Web API, Angular (latest), and Entity Framework Core. The project includes portfolio metrics calculation, rebalancing suggestions, and comprehensive testing.

## Development Guidelines

### Code Quality
- Follow conventional commits (feat/fix/docs/refactor/test/build/chore)
- Maintain 85% code coverage in /src
- Use StyleCop for code formatting
- All unit tests must pass

### Testing Requirements
- Unit tests for metrics (CAGR, Sharpe, MDD)
- Property-based tests for rebalancing math
- Golden tests for deterministic fixtures
- API tests using WebApplicationFactory

### Documentation
- Update CHANGELOG.md for each change
- Maintain API.md for endpoint changes
- Update ARCHITECTURE.md for structural changes

## Project Structure
- `/src` - Main application code
- `/tests` - All test projects
- `/docs` - Documentation
- `/scripts` - Build and deployment scripts

## Quality Gates
- ✅ All tests pass with ≥85% coverage
- ✅ Documentation updated
- ✅ OpenAPI spec regenerated
- ✅ StyleCop warnings = 0
- ✅ No secrets in repository
