## Pull Request Checklist

Please ensure your PR meets the following criteria before requesting review:

### Definition of Done

- [ ] All unit tests pass locally
- [ ] Code coverage ≥ 85% in `/src` (run `dotnet test /p:CollectCoverage=true`)
- [ ] All integration tests pass
- [ ] `dotnet format` applied (no formatting issues)
- [ ] StyleCop warnings = 0 (or justified in PR description)
- [ ] No secrets committed (API keys, connection strings, etc.)
- [ ] CHANGELOG.md updated with changes
- [ ] Documentation updated (if applicable):
  - [ ] README.md
  - [ ] ARCHITECTURE.md (if structure changes)
  - [ ] ALGORITHMS.md (if metric/calculation changes)
  - [ ] API.md (if endpoint changes)
  - [ ] IMPORT_FORMATS.md (if CSV format changes)
- [ ] OpenAPI spec regenerated (if API changes): `swagger tofile --output docs/openapi.json`
- [ ] Demo data seed updated (if database schema changed)

### Type of Change

- [ ] 🐛 Bug fix (non-breaking change which fixes an issue)
- [ ] ✨ New feature (non-breaking change which adds functionality)
- [ ] 💥 Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] 📝 Documentation update
- [ ] ♻️ Refactoring (no functional changes)
- [ ] ✅ Test additions/improvements
- [ ] 🔧 Build/CI changes

### Description

<!-- Provide a clear and concise description of your changes -->

### Related Issues

<!-- Link related issues using #issue-number -->
Closes #

### Testing

<!-- Describe the tests you ran to verify your changes -->

**Test Configuration**:
- .NET SDK version:
- Node.js version:
- OS:

**Test Evidence**:
<!-- Paste relevant test output, screenshots, or logs -->

```
# Example: Test output
dotnet test
  Passed: 42
  Failed: 0
  Coverage: 87.3%
```

### Screenshots (if applicable)

<!-- Add screenshots for UI changes -->

### Checklist for Reviewers

- [ ] Code follows project conventions and style
- [ ] Changes are properly tested
- [ ] Documentation is clear and accurate
- [ ] No obvious performance issues
- [ ] Security considerations addressed
- [ ] Error handling is appropriate

### Additional Notes

<!-- Any additional information that reviewers should know -->

---

**Commit Message Format**: Ensure your commits follow [Conventional Commits](https://www.conventionalcommits.org/):
- `feat(scope): description` - New feature
- `fix(scope): description` - Bug fix
- `docs(scope): description` - Documentation
- `refactor(scope): description` - Code refactoring
- `test(scope): description` - Test changes
- `build(scope): description` - Build system changes
- `chore(scope): description` - Maintenance tasks

Example: `feat(metrics): implement Sharpe Ratio calculation`
