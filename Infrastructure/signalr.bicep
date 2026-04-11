param webSiteName string
param location string = resourceGroup().location

resource signalR 'Microsoft.SignalRService/signalR@2024-03-01' = {
  name: 'sigr-${webSiteName}'
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
  }
}

output signalRResourceName string = signalR.name
