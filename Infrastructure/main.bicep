param location string = resourceGroup().location
param webSiteName string = 'grocerylisthelper'
param vnetName string = 'vnet-${webSiteName}'
param planName string = 'asp-${webSiteName}'
param logAnalyticsName string = 'log-${webSiteName}'
param appInsightsName string = 'ai-${webSiteName}'
param cosmosName string = 'cosmos-${webSiteName}'
param signalRName string = 'sigr-${webSiteName}'
param allowedIps string[] = []
param sku string = 'B1'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
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

resource vnet 'Microsoft.Network/virtualNetworks@2024-05-01' = {
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
          defaultOutboundAccess: false
          // serviceEndpoints: [
          //   {
          //     service: 'Microsoft.AzureCosmosDB'
          //     locations: [
          //       '*'
          //     ]
          //   }
          // ]
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
      {
        name: 'cosmos'
        properties: {
          addressPrefix: '10.0.1.0/24'
          privateEndpointNetworkPolicies: 'Disabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
          defaultOutboundAccess: false
        }
      }
    ]
  }
}

resource subnetDefault 'Microsoft.Network/virtualNetworks/subnets@2024-05-01' existing = {
  parent: vnet
  name: 'default'
}

resource subnetCosmos 'Microsoft.Network/virtualNetworks/subnets@2024-05-01' existing = {
  parent: vnet
  name: 'cosmos'
}

var allowedIpRules = [
  for ip in allowedIps: {
    ipAddressOrRange: ip
  }
]

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-12-01-preview' = {
  name: cosmosName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    backupPolicy: {
      type: 'Continuous'
      continuousModeProperties: {
        tier: 'Continuous7Days'
      }
    }
    publicNetworkAccess: 'Enabled'
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: true
    capacityMode: 'Serverless'
    disableLocalAuth: true
    isVirtualNetworkFilterEnabled: false
    // virtualNetworkRules: [
    //   {
    //     id: subnetDefault.id
    //     ignoreMissingVNetServiceEndpoint: false
    //   }
    // ]
    ipRules: allowedIpRules
  }
}

resource sqlDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-11-15' = {
  name: webSiteName
  parent: cosmosDbAccount
  properties: {
    resource: {
      id: webSiteName
    }
  }
}

resource cosmosPrivateEndpoint 'Microsoft.Network/privateEndpoints@2024-05-01' = {
  name: 'pe-cosmos-${webSiteName}'
  location: location
  properties: {
    subnet: {
      id: subnetCosmos.id
    }
    customNetworkInterfaceName: 'pe-cosmos-${webSiteName}-nic'
    privateLinkServiceConnections: [
      {
        name: 'pe-cosmos-grocerylisthelper'
        properties: {
          privateLinkServiceId: cosmosDbAccount.id
          groupIds: [
            'Sql'
          ]
        }
      }
    ]
  }
}

resource cosmosPrivateDnsZone 'Microsoft.Network/privateDnsZones@2024-06-01' = {
  name: 'privatelink.documents.azure.com'
  location: 'global'
}

resource cosmosPrivateDnsZoneLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2024-06-01' = {
  name: 'pe-cosmos-${webSiteName}-link'
  parent: cosmosPrivateDnsZone
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: { id: vnet.id }
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

var containersData = [
  {
    name: 'cartproducts'
    partitionKey: '/userId'
    excludedPaths: ['/unitPrice/?', '/amount/?', '/order/?', '/isCollected/?', '/_ts/?']
  }
  {
    name: 'storeproducts'
    partitionKey: '/userId'
    excludedPaths: ['/unitPrice/?', '/_ts/?']
  }
  {
    name: 'users'
    partitionKey: '/id'
    excludedPaths: ['/_ts/?']
  }
  {
    name: 'dataprotectionkeys'
    partitionKey: '/id'
    excludedPaths: ['/xmlData/?', '/_ts/?']
  }
  {
    name: 'cartgroups'
    partitionKey: '/id'
    excludedPaths: ['/name/?', '/_ts/?']
  }
]

resource containers 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-12-01-preview' = [for container in containersData: {
  parent: sqlDb
  name: container.name
  properties: {
    resource: {
      id: container.name
      partitionKey: {
        paths: [container.partitionKey]
        kind: 'Hash'
      }
      indexingPolicy: {
        includedPaths: [{ path: '/*' }]
        excludedPaths: [for path in container.excludedPaths: { path: path }]
      }
    }
  }
}]

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: planName
  location: location
  kind: 'linux'
  sku: {
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
    virtualNetworkSubnetId: subnetDefault.id
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
  name: guid(resourceGroup().id, appService.id, vnet.id)
  properties: {
    vnetResourceId: subnetDefault.id
    isSwift: true
  }
}

resource signalR 'Microsoft.SignalRService/signalR@2024-03-01' = {
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
    // networkACLs: {
    //   defaultAction: 'Deny'
    //   publicNetwork: {
    //     allow: [
    //       'ClientConnection'
    //       'ServerConnection'
    //       'RESTAPI'
    //       'Trace'
    //     ]
    //   }
    // }
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
