targetScope = 'subscription'

@description('The Azure region for all resources.')
param location string = 'northeurope'

@description('Environment name. Used for resource naming.')
param environmentName string = 'prod'

@description('Unique suffix for global resource names.')
param uniqueSuffix string = uniqueString(subscription().id, environmentName)

@description('App Service Plan SKU name (F1, B1, etc.).')
param appServicePlanSkuName string = 'F1'

@description('App Service Plan SKU tier (Free, Basic, etc.).')
param appServicePlanSkuTier string = 'Free'

// ── Resource Group ──────────────────────────────────────────
resource resourceGroup 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: 'rg-bigclubdebate-${environmentName}'
  location: location
}

// ── App Service Plan ────────────────────────────────────────
module appServicePlan 'modules/appserviceplan.bicep' = {
  name: 'appServicePlan'
  scope: resourceGroup
  params: {
    name: 'plan-bigclubdebate-${environmentName}'
    location: location
    sku: {
      name: appServicePlanSkuName
      tier: appServicePlanSkuTier
    }
  }
}

// ── App Service ─────────────────────────────────────────────
module webApp 'modules/appservice.bicep' = {
  name: 'webApp'
  scope: resourceGroup
  params: {
    name: 'app-bigclubdebate-${environmentName}-${uniqueSuffix}'
    location: location
    appServicePlanId: appServicePlan.outputs.id
  }
}

// ── Outputs ─────────────────────────────────────────────────
output AZURE_LOCATION string = location
output AZURE_RESOURCE_GROUP string = resourceGroup.name
output WEB_APP_NAME string = webApp.outputs.name
output WEB_APP_URL string = webApp.outputs.defaultHostName
