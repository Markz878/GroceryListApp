param webSiteName string
param location string = resourceGroup().location
param appServicePlanId string
param subnetId string
param appInsightsName string
param cosmosDbName string
param signalRResourceName string

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-12-01-preview' existing = {
  name: cosmosDbName
}

resource signalR 'Microsoft.SignalRService/signalR@2024-03-01' existing = {
  name: signalRResourceName
}

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: webSiteName
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    clientAffinityEnabled: false
    vnetRouteAllEnabled: true
    virtualNetworkSubnetId: subnetId
    siteConfig: {
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      linuxFxVersion: 'DOTNETCORE|10.0'
      webSocketsEnabled: true
      use32BitWorkerProcess: false
      healthCheckPath: '/health'
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'ConnectionStrings__CosmosDb'
          value: cosmosDbAccount.properties.documentEndpoint
        }
        {
          name: 'Azure__SignalR__ConnectionString'
          value: 'Endpoint=https://${signalR.properties.hostName};AuthType=azure.msi;Version=1.0;'
        }
      ]
    }
  }
}

resource appServiceVnetConnection 'Microsoft.Web/sites/virtualNetworkConnections@2024-04-01' = {
  parent: appService
  name: guid(resourceGroup().id, appService.id, subnetId)
  properties: {
    vnetResourceId: subnetId
    isSwift: true
  }
}

resource appinsights_monitoring_publisher 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: appInsights
  name: '3913510d-42f4-4e42-8a64-420c390055eb'
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

resource contributorRoleDefinition 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2024-12-01-preview' existing = {
  name: '00000000-0000-0000-0000-000000000002'
  parent: cosmosDbAccount
}

resource sqlRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2024-12-01-preview' = {
  name: guid(contributorRoleDefinition.id, appService.id, cosmosDbAccount.id)
  parent: cosmosDbAccount
  properties: {
    principalId: appService.identity.principalId
    roleDefinitionId: contributorRoleDefinition.id
    scope: cosmosDbAccount.id
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
