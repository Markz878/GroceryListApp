param location string = resourceGroup().location
param webSiteName string = 'grocerylisthelper'
param vnetName string = 'vnet-${webSiteName}'
param planName string = 'asp-${webSiteName}'
param logAnalyticsName string = 'log-${webSiteName}'
param appInsightsName string = 'ai-${webSiteName}'
param storageName string = 'st${webSiteName}'
param signalRName string = 'sigr-${webSiteName}'
param sku string = 'B1'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
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
    DisableLocalAuth: true
    RetentionInDays: 30
  }
}

resource vnet 'Microsoft.Network/virtualNetworks@2023-02-01' = {
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
      defaultAction: 'Deny'
      virtualNetworkRules: [
        {
          id: resourceId('Microsoft.Network/virtualNetworks/subnets', vnetName, 'default')
          action: 'Allow'
        }
      ]
      ipRules: []
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
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
      linuxFxVersion: 'DOTNETCORE|9.0'
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
        {
          name: 'Azure__SignalR__ConnectionString'
          value: 'Endpoint=https://${signalR.properties.hostName};AuthType=azure.msi;Version=1.0;'
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

resource app_storage_roleassignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
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

resource appinsights_monitoring_roleassignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, appService.id, appinsights_monitoring_publisher.id)
  scope: appInsights
  properties: {
    roleDefinitionId: appinsights_monitoring_publisher.id
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource appinsights_monitoring_publisher 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: appInsights
  name: '3913510d-42f4-4e42-8a64-420c390055eb'
}

resource signalR 'Microsoft.SignalRService/signalR@2022-02-01' = {
  name: signalRName
  location: location
  sku: {
      capacity: 1
      name: 'Free_F1'
  }
  kind: 'SignalR'
  identity: {
      type: 'None'
  }
  properties: {
      tls: {
          clientCertEnabled: false
      }
      disableLocalAuth: true
      features: [
          {
              flag: 'ServiceMode'
              value: 'Default'
          }
          {
              flag: 'EnableConnectivityLogs'
              value: 'True'
          }
          {
              flag: 'EnableMessagingLogs'
              value: 'False'
          }
          {
              flag: 'EnableLiveTrace'
              value: 'False'
          }
      ]
      cors: {
          allowedOrigins: [
              '*'
          ]
      }
      networkACLs: {
          defaultAction: 'Deny'
          publicNetwork: {
              allow: [
                  'ClientConnection'
                  'ServerConnection'
                  'RESTAPI'
                  'Trace'
              ]
          }
      }
  }
}
resource signalRAppServerRole 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: signalR
  name: '420fcaa2-552c-430f-98ca-3264be4806c7'
}
resource webappSignalRRoleassignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, appService.id, signalRAppServerRole.id)
  scope: signalR
  properties: {
      roleDefinitionId: signalRAppServerRole.id
      principalId: appService.identity.principalId
      principalType: 'ServicePrincipal'
  }
}
