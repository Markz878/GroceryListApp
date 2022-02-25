param location string = resourceGroup().location
@minLength(2)
@maxLength(60)
param webSiteName string = 'GroceryListHelper'
param keyVaultName string = 'grocery-list-keyvault'
var planName = '${webSiteName}Plan'

resource appservicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: planName
  location: location
  kind: 'app'
  sku:{
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 0
  }
}

resource appService 'Microsoft.Web/sites@2021-03-01' = {
  name: webSiteName
  location: location
  tags:{
    'hidden-related:/subscriptions/43f3a14a-9e3b-4005-a3ec-e5b3d4f5e564/resourceGroups/BlazeGagRG/providers/Microsoft.Web/serverFarms/BlazeGagPlan': 'empty'
  }
  properties: {
    serverFarmId: '/subscriptions/43f3a14a-9e3b-4005-a3ec-e5b3d4f5e564/resourceGroups/BlazeGagRG/providers/Microsoft.Web/serverfarms/BlazeGagPlan'
  }
}

resource symbolicname 'Microsoft.KeyVault/vaults@2021-11-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    accessPolicies: [
      {
        applicationId: appService.id
        objectId: 'a364b528-bd1c-4e4b-8f9b-851eb745a8f9'
        permissions: {
          secrets: [
            'all'
          ]
        }
        tenantId: '0424f4a0-f409-4185-9d9d-76ce62c20e4d'
      }
    ]
    createMode: 'default'
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
    enablePurgeProtection: false
    enableRbacAuthorization: true
    enableSoftDelete: false
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: '0424f4a0-f409-4185-9d9d-76ce62c20e4d'
    vaultUri: 'https://${keyVaultName}.${environment().suffixes.keyvaultDns}}'
  }
}

