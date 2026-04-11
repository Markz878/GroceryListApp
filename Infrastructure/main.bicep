param sharedResourceGroupName string
param sharedPlanNameName string
@minLength(3)
param webSiteName string = 'grocerylisthelper'
param allowedIps string[] = []

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' existing = {
  name: sharedPlanNameName
  scope: resourceGroup(sharedResourceGroupName)
}

module appInsights 'appinsights.bicep' = {
  name: 'appInsights'
  params: {
    webSiteName: webSiteName
  }
}

module vnet 'vnet.bicep' = {
  name: 'vnet'
  params: {
    webSiteName: webSiteName
  }
}

module cosmos 'cosmos.bicep' = {
  name: 'cosmos'
  params: {
    webSiteName: webSiteName
    allowedIps: allowedIps
    vnetId: vnet.outputs.vnetId
    cosmosDbSubnetId: vnet.outputs.subnet1Id
  }
}

module signalR 'signalr.bicep' = {
  name: 'signalR'
  params: {
    webSiteName: webSiteName
  }
}

module webapp 'webapp.bicep' = {
  name: 'webapp'
  params: {
    webSiteName: webSiteName
    appInsightsName: appInsights.outputs.appInsightsName
    appServicePlanId: appServicePlan.id
    cosmosDbName: cosmos.outputs.cosmosDbAccountName
    signalRResourceName: signalR.outputs.signalRResourceName
    subnetId: vnet.outputs.subnet0Id
  }
}
