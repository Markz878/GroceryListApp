param location string = resourceGroup().location
param webSiteName string = 'GroceryListHelper'

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
  name: 'string'
  location: 'string'
  tags: {
    tagName1: 'tagValue1'
    tagName2: 'tagValue2'
  }
  properties: {
    accessPolicies: [
      {
        applicationId: 'string'
        objectId: 'string'
        permissions: {
          certificates: [
            'string'
          ]
          keys: [
            'string'
          ]
          secrets: [
            'string'
          ]
          storage: [
            'string'
          ]
        }
        tenantId: 'string'
      }
    ]
    createMode: 'string'
    enabledForDeployment: bool
    enabledForDiskEncryption: bool
    enabledForTemplateDeployment: bool
    enablePurgeProtection: bool
    enableRbacAuthorization: bool
    enableSoftDelete: bool
    networkAcls: {
      bypass: 'string'
      defaultAction: 'string'
      ipRules: [
        {
          value: 'string'
        }
      ]
      virtualNetworkRules: [
        {
          id: 'string'
          ignoreMissingVnetServiceEndpoint: bool
        }
      ]
    }
    provisioningState: 'string'
    publicNetworkAccess: 'string'
    sku: {
      family: 'A'
      name: 'string'
    }
    softDeleteRetentionInDays: int
    tenantId: 'string'
    vaultUri: 'string'
  }
}

