param location string = resourceGroup().location
param webSiteName string = 'grocerylisthelper'
param planName string = 'asp-${webSiteName}'
param logAnalyticsName string = 'log-${webSiteName}'
param appInsightsName string = 'ai-${webSiteName}'
param keyVaultName string = 'kv-${webSiteName}'
param storageName string = 'st${webSiteName}'
param sku string = 'B1'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    workspaceCapping: {
      dailyQuotaGb: json('0.1')
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource appinsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  kind: 'web'
  location: location
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_ZRS'
  }
  properties: {
    allowSharedKeyAccess: false
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
    supportsHttpsTrafficOnly: true
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: tenant().tenantId
    enableRbacAuthorization: true
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
}

resource appservicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: planName
  location: location
  kind: 'linux'
  sku:{
    name: sku
  }
  properties: {
    reserved: true
  }
}

resource appService 'Microsoft.Web/sites@2021-03-01' = {
  name: webSiteName
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appservicePlan.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      linuxFxVersion: 'DOTNET|7.0'
      webSocketsEnabled: true
      use32BitWorkerProcess: false
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appinsights.properties.ConnectionString
        }
        {
          name: 'KeyVaultUri'
          value: keyVault.properties.vaultUri
        }
        {
          name: 'TableStorageUri'
          value: storageaccount.properties.primaryEndpoints.table
        }
      ]

    }
  }
}

resource app_keyvaultreader_roleassignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(resourceGroup().id, appService.id, keyvault_reader.id)
  scope: keyVault
  properties: {
    roleDefinitionId: keyvault_reader.id
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource app_storage_roleassignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(resourceGroup().id, appService.id, storage_table_contributor.id)
  scope: storageaccount
  properties: {
    roleDefinitionId: storage_table_contributor.id
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource storage_table_contributor 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: storageaccount
  name: '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3'
}

resource keyvault_reader 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: keyVault
  name: '4633458b-17de-408a-b874-0445c86b69e6'
}


