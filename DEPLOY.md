# Deployment Guide

The app deploys to **Azure Container Apps** via the Aspire CLI. Container-based deployment keeps costs low (~$5-6/mo on consumption tier).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) — **must be running** (Aspire containerizes the app)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) — logged in via `az login`
- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) v1.25+

```powershell
# Check versions
dotnet --version     # 10.x
docker info          # must show "Server Version"
az account show      # must show your subscription
azd version          # 1.25.x or later
```

## One-time setup

### 1. Configure Azure subscription

Edit `BigClubDebate.AppHost/appsettings.json` and set your subscription ID and region:

```json
{
  "Azure": {
    "SubscriptionId": "your-subscription-id",
    "Location": "westeurope"
  }
}
```

> **Why:** The Aspire CLI looks for `Azure:SubscriptionId` and `Azure:Location` in the AppHost configuration. It does NOT use `azd` environment variables like `AZURE_SUBSCRIPTION_ID`.

### 2. Install Aspire hosting package

This is already set up in the project. If you ever need to re-add it:

```powershell
aspire add azure-appcontainers --apphost BigClubDebate.AppHost
```

## Deploy

```powershell
# From the solution root
aspire deploy --apphost BigClubDebate.AppHost --environment Production --non-interactive
```

**What happens:**
1. Builds the AppHost project
2. Containerizes `BigClubDebate.Web` via Docker
3. Pushes the image to Azure Container Registry
4. Provisions/updates the Container Apps environment + Container App
5. Deploys the new container revision

This takes ~2-3 minutes. Look for `✅ Pipeline succeeded` at the end.

### If switching deployment targets

If you previously deployed to App Service or Docker, clear the cache:

```powershell
aspire deploy --apphost BigClubDebate.AppHost --environment Production --clear-cache --non-interactive
```

## After first deploy: enable public access

The Container App deploys without public ingress by default. Enable it once:

```powershell
az containerapp ingress enable `
  --name web `
  --resource-group rg-aspire-bigclubdebateapphost `
  --type external `
  --target-port 8080 `
  --transport auto
```

This is a one-time step. Subsequent deploys preserve the ingress setting.

## Verify

Open the URL printed at the end of `aspire deploy`, or find it with:

```powershell
az containerapp show --name web --resource-group rg-aspire-bigclubdebateapphost --query "properties.configuration.ingress.fqdn" -o tsv
```

The page shows "Loading..." while the Blazor Server SignalR connection establishes — that's normal.

## Costs

| Resource | SKU | ~/mo |
|----------|-----|------|
| Container Apps Environment | Consumption | $0 |
| Container App (web) | Consumption | ~$0.50 |
| Azure Container Registry | Basic | $4.20 |
| Log Analytics | Pay-as-you-go | ~$1-2 |
| **Total** | | **~$5-6** |

The Container App scales to zero when idle and spins up on the first request (~30s cold start). Blazor Server handles this through its automatic reconnection UI.

## Troubleshooting

### `An Azure subscription id is required`

The `Azure:SubscriptionId` setting is missing from `BigClubDebate.AppHost/appsettings.json`. Add it (see step 1 above).

### `Docker is not running`

Start Docker Desktop. The Aspire deploy pipeline containerizes your app and needs the Docker daemon.

### `service host 'aspire' is unsupported`

Your `azd` version is too old. Update:
```powershell
powershell -ex AllSigned -c "Invoke-RestMethod 'https://aka.ms/install-azd.ps1' | Invoke-Expression"
```

### `No public endpoints` after deploy

Run the `az containerapp ingress enable` command from step "After first deploy" above.

### Site shows blank page or "Loading..." forever

This is normal for a cold start. The consumption tier scales to zero when idle. First request triggers a cold start (~30s), then Blazor Server establishes its SignalR connection. Refresh if it takes longer than a minute.

## Local development

```powershell
# From the solution root — hot reload included
aspire start

# Or run just the web project
dotnet watch run --project BigClubDebate.Web
```
