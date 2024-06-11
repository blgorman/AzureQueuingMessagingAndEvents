/*note: This is a subscription level deployment that ensures/creates the resource group for all resources */

@description('Storage account location')
param location string = 'centralus'

@description('Resource Group Name')
param rgName string = 'rg-aqmetalk'

/* storage accounts */
@minLength(3)
@maxLength(16)
@description('Provide a unique name for the event hub storage account. Use only lower case letters and numbers, at least 3 and less than 17 chars')
param storageNameHub string = 'hubcoldstorage'

@minLength(3)
@maxLength(16)
@description('Provide a unique name for the event grid triggered storage account. Use only lower case letters and numbers, at least 3 and less than 17 chars')
param storageNameEvents string = 'triggerstorage'

@description('Storage account sku')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
  'Standard_ZRS'
  'Premium_LRS'
  'Premium_ZRS'
  'Standard_GZRS'
  'Standard_RAGZRS'
])
param storageSku string = 'Standard_LRS'

@description('Allow blob public access')
param allowBlobPublicAccess bool = false

@description('Allow shared key access')
param allowSharedKeyAccess bool = true

@description('Storage account access tier, Hot for frequently accessed data or Cool for infreqently accessed data')
@allowed([
  'Hot'
  'Cool'
])
param storageTier string = 'Hot'

@description('Enable blob encryption at rest')
param blobEncryptionEnabled bool = true

@description('Enable file encryption at rest')
param fileEncryptionEnabled bool = true

@description('Enable Blob Retention')
param enableBlobRetention bool = false

@description('Number of days to retain blobs')
param blobRetentionDays int = 7

@description('Blob Container name for Event Hub Capture.')
param capturedEventsBlobContainerName string = 'capturedehevents'

@description('Blob Container name for cool storage')
param processedEventsBlobContainerName string = 'processedehevents'

@description('Enable Hierarchical Namespace')
param enableHierarchicalNamespace bool = true

@description('Event Grid storage account container name')
param eventTriggerContainerName string = 'uploads'



/* Event Hub */
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
param captureNameFormat string = '{Namespace}/{EventHub}/{PartitionId}/{Year}/{Month}/{Day}/{Hour}/{Minute}/{Second}'

/* Service Bus */
@description('Specifies a project name that is used to generate the Event Hub name and the Namespace name.')
param sbProjectName string = 'aqmetalk'

@description('Sku for the service bus must be standard or better for topics and subscriptions.')
@allowed([
  'Standard'
  'Premium'
])
param serviceBusSku string = 'Standard'

@description('Name of the SErvice Bus Queue')
param serviceBusQueueName string = 'FileProcessingQueue'

@description('Name of the Topic')
param serviceBusTopicName string = 'MoviesToReview'

@description('Lock duration')
param lockDuration string = 'PT5M'

@description('Max size in megabytes')
param maxSizeInMegabytes int = 1024

@description('Require duplicate detection')
param requiresDuplicateDetection bool = false

@description('Require session')
param requiresSession bool = false

@description('Default time to live for the messages')
param defaultMessageTimeToLive string = 'P10675199DT2H48M5.4775807S'

@description('Dead lettering on message expiration')
param deadLetteringOnMessageExpiration bool = false

@description('Duplicate detection history time window')
param duplicateDetectionHistoryTimeWindow string = 'PT10M'

@description('Max delivery count')
param maxDeliveryCount int = 10

@description('Auto delete on idle')
param autoDeleteOnIdle string = 'P10675199DT2H48M5.4775807S'

@description('Enable partitioning')
param enablePartitioning bool = false

@description('Enable express')
param enableExpress bool = false

@description('Enable batched operations')
param enableBatchedOperations bool = true

@description('Support ordering')  
param supportOrdering bool = true

targetScope = 'subscription'

resource aqmeResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location
}

module deployStorage 'storage.bicep' = {
  name: 'deployStorage'
  scope: aqmeResourceGroup
  params: {
    location: location
    storageNameHub: storageNameHub
    storageNameEvents: storageNameEvents
    storageSku: storageSku
    allowBlobPublicAccess: allowBlobPublicAccess
    allowSharedKeyAccess: allowSharedKeyAccess
    storageTier: storageTier
    blobEncryptionEnabled: blobEncryptionEnabled
    fileEncryptionEnabled: fileEncryptionEnabled
    enableBlobRetention: enableBlobRetention
    blobRetentionDays: blobRetentionDays
    capturedEventsBlobContainerName: capturedEventsBlobContainerName
    processedEventsBlobContainerName: processedEventsBlobContainerName
    enableHierarchicalNamespace: enableHierarchicalNamespace
    eventTriggerContainerName: eventTriggerContainerName
  }
}

var storageAccountName = deployStorage.outputs.hubStorageAccountName
var captureBlobContainerName = deployStorage.outputs.hubCapturedEventsContainerName

@description('Deploy the Event Hub with capture to the storage account container')
module deployEventHub 'eventHub.bicep' = {
  name: 'deployEventHub'
  scope: aqmeResourceGroup
  params: {
    location: location
    storageName: storageAccountName
    captureBlobContainerName: captureBlobContainerName
    eventHubProjectName: eventHubProjectName
    eventHubSku: eventHubSku
    eventHubSKUCapacity: eventHubSKUCapacity
    isAutoInflateEnabled: isAutoInflateEnabled
    disableLocalAuth: disableLocalAuth
    skipEmptyArchives: skipEmptyArchives
    maximumThroughputUnits: maximumThroughputUnits
    messageRetentionInDays: messageRetentionInDays
    partitionCount: partitionCount
    captureEnabled: captureEnabled
    captureEncodingFormat: captureEncodingFormat
    captureTime: captureTime
    captureSize: captureSize
    captureNameFormat: captureNameFormat
  }
}

module deployServiceBus 'serviceBus.bicep' = {
  name: 'deployServiceBus'
  scope: aqmeResourceGroup
  params: {
    location: location
    sbProjectName: sbProjectName
    serviceBusSku: serviceBusSku
    serviceBusQueueName: serviceBusQueueName
    serviceBusTopicName: serviceBusTopicName
    lockDuration: lockDuration
    maxSizeInMegabytes: maxSizeInMegabytes
    requiresDuplicateDetection: requiresDuplicateDetection
    requiresSession: requiresSession
    defaultMessageTimeToLive: defaultMessageTimeToLive
    deadLetteringOnMessageExpiration: deadLetteringOnMessageExpiration
    duplicateDetectionHistoryTimeWindow: duplicateDetectionHistoryTimeWindow
    maxDeliveryCount: maxDeliveryCount
    autoDeleteOnIdle: autoDeleteOnIdle
    enablePartitioning: enablePartitioning
    enableExpress: enableExpress
    enableBatchedOperations: enableBatchedOperations
    supportOrdering: supportOrdering
  }
}
