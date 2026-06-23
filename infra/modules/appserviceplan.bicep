param name string
param location string

@description('The SKU for the App Service Plan.')
param sku object

resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: name
  location: location
  kind: 'linux'
  sku: sku
  properties: {
    reserved: true
  }
}

output id string = plan.id
output name string = plan.name
