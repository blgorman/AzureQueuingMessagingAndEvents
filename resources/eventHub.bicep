@description('Specifies the Azure location for all resources.')
param location string = resourceGroup().location

@description('The storage account resource id for Event Hub Capture [must be pre-created as hierarchical enabled!].')
param storageName string

@description('Blob Container name for Event Hub Capture. Must be pre-created in the storage account used in storageName above')
param captureBlobContainerName string

@description('Specifies a project name that is used to generate the Event Hub name and the Namespace name.')
param eventHubProjectName string = 'aqmetalk'

@description('The messaging tier for service Bus namespace')
@allowed([
  'Basic'
  'Standard'
])
param eventHubSku string = 'Standard'

@description('MessagingUnits for premium namespace')
@allowed([
  1
  2
  4
])
param eventHubSKUCapacity int = 1

@description('Enable or disable AutoInflate')
param isAutoInflateEnabled bool = false

@description('Disable the ability to use local SAS Tokens')
param disableLocalAuth bool = false

@description('Skip or capture empty archives')
param skipEmptyArchives bool = true

@description('Upper limit of throughput units when AutoInflate is enabled, vaule should be within 0 to 20 throughput units.')
@minValue(0)
@maxValue(20)
param maximumThroughputUnits int = 0

@description('How long to retain the data in Event Hub')
@minValue(1)
@maxValue(7)
param messageRetentionInDays int = 2

@description('Number of partitions chosen')
@minValue(2)
@maxValue(32)
param partitionCount int = 4

@description('Enable or disable the Capture feature for your Event Hub')
param captureEnabled bool = true

@description('The encoding format Eventhub capture serializes the EventData when archiving to your storage')
@allowed([
  'Avro'
])
param captureEncodingFormat string = 'Avro'

@description('the time window in seconds for the archival')
@minValue(60)
@maxValue(900)
param captureTime int = 300

@description('the size window in bytes for evetn hub capture')
@minValue(10485760)
@maxValue(524288000)
param captureSize int = 314572800

@description('A Capture Name Format must contain {Namespace}, {EventHub}, {PartitionId}, {Year}, {Month}, {Day}, {Hour}, {Minute} and {Second} fields. These can be arranged in any order with or without delimeters. E.g.  Prod_{EventHub}/{Namespace}\\{PartitionId}_{Year}_{Month}/{Day}/{Hour}/{Minute}/{Second}')
var captureNameFormat = '{Namespace}/{EventHub}/{PartitionId}/{Year}/{Month}/{Day}/{Hour}/{Minute}/{Second}'

var eventHubNamespaceName = substring('${eventHubProjectName}ehns${uniqueString(resourceGroup().id)}', 0, 24)
var eventHubName = '${eventHubProjectName}hub'
var defaultSASKeyName = 'RootManageSharedAccessKey'
var authRuleResourceId = resourceId('Microsoft.EventHub/namespaces/authorizationRules', eventHubNamespaceName, defaultSASKeyName)
var storageAccountName = substring('${storageName}${uniqueString(resourceGroup().id)}', 0, 24)
var storageAccountResourceId = resourceId('Microsoft.Storage/storageAccounts', storageAccountName)

@description('Create the event hub namespace')
resource eventHubNamespace 'Microsoft.EventHub/namespaces@2021-11-01' = {
  name: eventHubNamespaceName
  location: location
  sku: {
    name: eventHubSku
    tier: eventHubSku
    capacity: eventHubSKUCapacity
  }
  properties: {
    isAutoInflateEnabled: isAutoInflateEnabled
    maximumThroughputUnits: maximumThroughputUnits
    disableLocalAuth: disableLocalAuth
  }
}

resource eventHub 'Microsoft.EventHub/Namespaces/eventhubs@2017-04-01' = {
  parent: eventHubNamespace
  name: eventHubName
  properties: {
    messageRetentionInDays: messageRetentionInDays
    partitionCount: partitionCount
    captureDescription: {
      enabled: captureEnabled
      skipEmptyArchives: skipEmptyArchives
      encoding: captureEncodingFormat
      intervalInSeconds: captureTime
      sizeLimitInBytes: captureSize
      destination: {
        name: 'EventHubArchive.AzureBlockBlob'
        properties: {
          storageAccountResourceId: storageAccountResourceId
          blobContainer: captureBlobContainerName
          archiveNameFormat: captureNameFormat
        }
      }
    }
  }
}

resource hubConsumerSAS 'Microsoft.EventHub/namespaces/eventhubs/authorizationrules@2024-01-01' = {
  parent: eventHub
  name: 'Consumer'
  properties: {
    rights: [
      'Listen'
    ]
  }
}

resource hubProducerSAS 'Microsoft.EventHub/namespaces/eventhubs/authorizationrules@2024-01-01' = {
  parent: eventHub
  name: 'Producer'
  properties: {
    rights: [
      'Send'
    ]
  }
  dependsOn: [
    hubConsumerSAS
  ]
}

output eventHubNamespaceName string = eventHubNamespace.name
output eventHubNamespaceId string = eventHubNamespace.id
output eventHubName string = eventHub.name
output eventHubId string = eventHub.id
output authRuleResourceId string = authRuleResourceId
output eventHubConsumerSASId string = hubConsumerSAS.id
output eventHubProducerSASId string = hubProducerSAS.id

