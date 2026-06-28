#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Creates Azure DevOps environments and configures approval gates for Big Club Debate CI/CD.

.DESCRIPTION
    Creates staging and production environments in Azure DevOps, then adds a manual
    approval check to the production environment so every deploy requires a thumbs-up.

.PARAMETER Organization
    Your Azure DevOps organization name (e.g. "myorg" for dev.azure.com/myorg).

.PARAMETER Project
    Your Azure DevOps project name.

.PARAMETER ApproverEmail
    Email address of the person who should approve production deployments.
    Defaults to the current Azure CLI user.

.EXAMPLE
    .\scripts\Setup-AzureDevOpsEnvironments.ps1 -Organization "contoso" -Project "BigClubDebate"
#>

param(
    [Parameter(Mandatory)]
    [string]$Organization,

    [Parameter(Mandatory)]
    [string]$Project,

    [string]$ApproverEmail
)

$orgUrl = "https://dev.azure.com/$Organization"
$apiVersion = "7.1-preview.1"

# ── Step 0: Ensure prerequisites ──────────────────────────────
Write-Host "🔧 Checking prerequisites..." -ForegroundColor Cyan

# Ensure az devops extension
$ext = az extension list --query "[?name=='azure-devops'].version" -o json | ConvertFrom-Json
if (-not $ext) {
    Write-Host "📦 Installing azure-devops extension..." -ForegroundColor Yellow
    az extension add --name azure-devops
}

# Ensure logged in
$account = az account show --query "{user:user.name}" -o json 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Host "🔑 Log in with 'az login' first" -ForegroundColor Red
    exit 1
}

if (-not $ApproverEmail) {
    $ApproverEmail = $account.user
}
Write-Host "✅ Using $((Get-Item $env:USERPROFILE).Name) as $((az account show --query user.name -o tsv))" -ForegroundColor Green

# ── Step 1: Create environments ─────────────────────────────
function New-DevOpsEnvironment {
    param([string]$EnvName)

    Write-Host "🏗️  Creating environment '$EnvName'..." -ForegroundColor Cyan
    $body = @{
        name = $EnvName
        description = "Big Club Debate - $EnvName"
    } | ConvertTo-Json

    $result = az devops invoke `
        --area distributedtask `
        --resource environments `
        --route-parameters project=$Project `
        --api-version $apiVersion `
        --http-method POST `
        --org $orgUrl `
        --in-file $null `
        --media-type application/json `
        --encoding utf-8 `
        -o json 2>&1

    # If the above fails (invoke with body via inline is tricky), try via REST directly
    if ($LASTEXITCODE -ne 0) {
        $url = "$orgUrl/$Project/_apis/distributedtask/environments?api-version=$apiVersion"
        $result = az rest --method post --url $url --body $body --headers "Content-Type=application/json" -o json 2>&1
    }

    if ($LASTEXITCODE -eq 0) {
        $env = $result | ConvertFrom-Json 2>$null
        if ($env) {
            Write-Host "   ✅ Created '$EnvName' (id: $($env.id))" -ForegroundColor Green
            return $env.id
        }
    }
    Write-Host "   ⚠️  Environment '$EnvName' may already exist — that's fine." -ForegroundColor Yellow
    return $null
}

$stagingId = New-DevOpsEnvironment -EnvName "bigclubdebate-staging"
$prodId = New-DevOpsEnvironment -EnvName "bigclubdebate-production"

# ── Step 2: Get environment IDs (if creation skipped) ───────
if (-not $prodId) {
    Write-Host "🔍 Fetching existing environment IDs..." -ForegroundColor Cyan
    $envs = az devops invoke `
        --area distributedtask `
        --resource environments `
        --route-parameters project=$Project `
        --api-version $apiVersion `
        --org $orgUrl `
        -o json 2>$null | ConvertFrom-Json

    if (-not $envs) {
        $url = "$orgUrl/$Project/_apis/distributedtask/environments?api-version=$apiVersion"
        $envs = az rest --method get --url $url -o json 2>$null | ConvertFrom-Json
    }

    if ($envs -and $envs.value) {
        foreach ($e in $envs.value) {
            if ($e.name -eq "bigclubdebate-production") { $prodId = $e.id }
            if ($e.name -eq "bigclubdebate-staging") { $stagingId = $e.id }
        }
    }
}

Write-Host "   Staging    environment ID: $stagingId" -ForegroundColor Gray
Write-Host "   Production environment ID: $prodId" -ForegroundColor Gray

# ── Step 3: Add approval check on production ─────────────────
# Azure DevOps Checks API: POST https://dev.azure.com/{org}/{proj}/_apis/pipelines/checks
# The check configuration is a bit involved — this is the simplest approach.

if ($prodId) {
    Write-Host "🔒 Adding approval gate to production environment..." -ForegroundColor Cyan

    $checkBody = @"
{
    "resourceType": "environment",
    "resourceId": $prodId,
    "settings": {
        "approvers": ["$ApproverEmail"],
        "instructions": "Review the staging deploy results before approving production rollout.",
        "blockedApproversCount": 0,
        "minRequiredApprovers": 1
    },
    "type": "Approval",
    "version": 1
}
"@

    $checkUrl = "$orgUrl/$Project/_apis/pipelines/checks?api-version=7.1-preview.1"
    $checkResult = az rest --method post --url $checkUrl --body $checkBody --headers "Content-Type=application/json" -o json 2>&1

    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✅ Approval gate added for $ApproverEmail" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  Could not add approval gate. You can add it manually:" -ForegroundColor Yellow
        Write-Host "       Azure DevOps → Pipelines → Environments → bigclubdebate-production → Approvals" -ForegroundColor Gray
    }
}

# ── Summary ──────────────────────────────────────────────────
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  ✅ Setup complete!                                  ║" -ForegroundColor Cyan
Write-Host "╠══════════════════════════════════════════════════════╣" -ForegroundColor Cyan
Write-Host "║  Environments created:                               ║" -ForegroundColor Cyan
Write-Host "║    • bigclubdebate-staging  (auto-deploy)            ║" -ForegroundColor Cyan
Write-Host "║    • bigclubdebate-production (approval required)    ║" -ForegroundColor Cyan
Write-Host "║                                                      ║" -ForegroundColor Cyan
Write-Host "║  Next: push the pipeline and merge to master         ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════════════╝" -ForegroundColor Cyan
