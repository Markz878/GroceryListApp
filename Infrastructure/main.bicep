param location string = resourceGroup().location
param webSiteName string = 'grocerylisthelper'
param vnetName string = 'vnet-${webSiteName}'
param planName string = 'asp-${webSiteName}'
param logAnalyticsName string = 'log-${webSiteName}'
param appInsightsName string = 'ai-${webSiteName}'
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

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  kind: 'web'
  location: location
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

resource vnet 'Microsoft.Network/virtualNetworks@2019-11-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'default'
        properties: {
          addressPrefix: '10.0.0.0/24'
          serviceEndpoints: [
            {
              service: 'Microsoft.Storage'
              locations: [
                'northeurope'
                'westeurope'
              ]
            }
          ]
          delegations: [
            {
              name: 'delegation'
              properties: {
                serviceName: 'Microsoft.Web/serverfarms'
              }
            }
          ]
        }
      }
    ]
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_ZRS'
  }
  properties: {
    allowSharedKeyAccess: false
    publicNetworkAccess: 'Enabled'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    defaultToOAuthAuthentication: true
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      bypass: 'None'
      virtualNetworkRules: [
        {
          id: resourceId('Microsoft.Network/virtualNetworks/subnets', vnetName, 'default')
          action: 'Allow'
        }
      ]
      ipRules: []
      defaultAction: 'Deny'
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
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

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: webSiteName
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: false
    vnetRouteAllEnabled: true
    virtualNetworkSubnetId: vnet.properties.subnets[0].id
    siteConfig: {
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      linuxFxVersion: 'DOTNETCORE|7.0'
      webSocketsEnabled: true
      use32BitWorkerProcess: false
      healthCheckPath: '/health'
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'TableStorageUri'
          value: storageAccount.properties.primaryEndpoints.table
        }
      ]
    }
  }
}

resource appServiceVnetConnection 'Microsoft.Web/sites/virtualNetworkConnections@2022-09-01' = {
  parent: appService
  name: guid(resourceGroup().id, appService.id, vnet.id)
  properties: {
    vnetResourceId: vnet.properties.subnets[0].id
    isSwift: true
  }
}

resource app_storage_roleassignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(resourceGroup().id, appService.id, storage_table_contributor.id)
  scope: storageAccount
  properties: {
    roleDefinitionId: storage_table_contributor.id
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource storage_table_contributor 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: storageAccount
  name: '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3'
}
