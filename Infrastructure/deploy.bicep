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

