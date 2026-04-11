param webSiteName string
param location string = resourceGroup().location
param allowedIps string[] = []
param vnetId string
param cosmosDbSubnetId string

var allowedIpRules = [
  for ip in allowedIps: {
    ipAddressOrRange: ip
  }
]

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-12-01-preview' = {
  name: 'cosmos-${webSiteName}'
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
      id: cosmosDbSubnetId
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
    virtualNetwork: { id: vnetId }
  }
}

resource cosmosPrivateDnsZoneGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2024-01-01' = {
  name: 'pe-cosmos-${webSiteName}-dnszonegroup'
  parent: cosmosPrivateEndpoint
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'privatelink_documents_azure_com'
        properties: {
          privateDnsZoneId: cosmosPrivateDnsZone.id
        }
      }
    ]
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

resource containers 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-12-01-preview' = [
  for container in containersData: {
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
  }
]

output cosmosDbAccountName string = cosmosDbAccount.name
