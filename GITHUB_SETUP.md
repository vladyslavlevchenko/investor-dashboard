# Git Setup and GitHub Push Guide

## Step 1: Create GitHub Repository

1. Go to https://github.com/new
2. Repository name: `investor-dashboard`
3. Description: "Comprehensive portfolio management and analytics platform with .NET 8 Web API and Angular 19"
4. **Important**: Do NOT initialize with README, .gitignore, or license (we already have these)
5. Click "Create repository"

## Step 2: Initialize Local Git Repository

Run these commands in PowerShell from the project root:

```powershell
# Navigate to project directory
cd "c:\Users\VladyslavLevchenko\source\repos\investor_dashboard"

# Initialize git repository
git init

# Add all files
git add .

# Create initial commit
git commit -m "feat: initial project setup - M0 skeleton complete

- ASP.NET Core 8 Web API with Swagger/OpenAPI
- Angular 19 frontend (zoneless architecture)
- Clean architecture (Core, Infrastructure, API layers)
- Entity Framework Core with SQLite
- xUnit testing with Moq, FluentAssertions, FsCheck
- GitHub Actions CI/CD pipeline
- Comprehensive documentation (2000+ lines)
- Health check endpoint
- 85% code coverage enforcement
- Security scanning and quality gates

Milestone: M0 - Skeleton Complete"
```

## Step 3: Connect to GitHub

Replace `YOUR_USERNAME` with your actual GitHub username:

```powershell
# Add remote repository
git remote add origin https://github.com/YOUR_USERNAME/investor-dashboard.git

# Verify remote
git remote -v

# Push to GitHub
git branch -M main
git push -u origin main
```

## Step 4: Verify Push

After pushing, visit your GitHub repository and verify:
- ✅ All files are present
- ✅ README.md displays correctly
- ✅ GitHub Actions workflow is detected
- ✅ No secrets or sensitive data committed

## Alternative: Using GitHub CLI (gh)

If you have GitHub CLI installed:

```powershell
# Create repository and push in one go
gh repo create investor-dashboard --public --source=. --remote=origin --push

# Or for private repository
gh repo create investor-dashboard --private --source=. --remote=origin --push
```

## Step 5: Enable GitHub Actions

1. Go to your repository on GitHub
2. Click "Actions" tab
3. Enable workflows if prompted
4. The CI/CD pipeline will run automatically on push

## Common Issues

### Issue: "fatal: not a git repository"
**Solution**: Make sure you ran `git init` first

### Issue: "Authentication failed"
**Solution**: 
- Use GitHub Personal Access Token instead of password
- Or use SSH keys: https://docs.github.com/en/authentication/connecting-to-github-with-ssh

### Issue: "failed to push some refs"
**Solution**: 
```powershell
git pull origin main --rebase
git push -u origin main
```

## Repository Settings (After Push)

### Recommended Settings

1. **Branch Protection** (Settings → Branches):
   - Require pull request before merging
   - Require status checks to pass (CI/CD)
   - Require branches to be up to date

2. **Topics/Tags**:
   - Add topics: `dotnet`, `angular`, `portfolio-management`, `investment`, `typescript`, `csharp`, `entity-framework`

3. **About Section**:
   - Description: "Portfolio management and analytics platform"
   - Website: (if you deploy it)
   - Topics: See above

4. **Security**:
   - Enable Dependabot alerts
   - Enable secret scanning

## Quick Command Summary

```powershell
# All-in-one script
cd "c:\Users\VladyslavLevchenko\source\repos\investor_dashboard"
git init
git add .
git commit -m "feat: initial project setup - M0 skeleton complete"
git remote add origin https://github.com/YOUR_USERNAME/investor-dashboard.git
git branch -M main
git push -u origin main
```

## After First Push

Future commits should follow Conventional Commits:

```powershell
# Make changes...
git add .
git commit -m "feat(core): add portfolio entity model"
git push

# Or for fixes
git commit -m "fix(api): correct CORS configuration"
git push
```

## Need Help?

- GitHub Docs: https://docs.github.com/en/get-started
- Git Documentation: https://git-scm.com/doc
- GitHub CLI: https://cli.github.com/

---

**Next Steps After Push**:
1. Add project description and topics on GitHub
2. Create first issue for M1 milestone
3. Set up branch protection rules
4. Watch the CI/CD pipeline run
5. Share repository link with team!
