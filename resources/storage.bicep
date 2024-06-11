@description('Storage account location')
param location string = resourceGroup().location

@minLength(3)
@maxLength(16)
@description('Provide a unique name for the event hub storage account. Use only lower case letters and numbers, at least 3 and less than 17 chars')
param storageNameHub string = 'hubcoldstorage'
var storageAccountNameHub = substring('${storageNameHub}${uniqueString(resourceGroup().id)}', 0, 24)

@minLength(3)
@maxLength(16)
@description('Provide a unique name for the event grid triggered storage account. Use only lower case letters and numbers, at least 3 and less than 17 chars')
param storageNameEvents string = 'triggerstorage'
var storageAccountNameEvents = substring('${storageNameEvents}${uniqueString(resourceGroup().id)}', 0, 24)

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

@description('The storage account.  Toggle the public access to true if you want public blobs on the account in any containers')
resource hubstorage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountNameHub
  location: location
  kind: 'StorageV2'
  sku: {
    name: storageSku
  }
  properties: {
    allowBlobPublicAccess: allowBlobPublicAccess
    accessTier: storageTier
    allowSharedKeyAccess: allowSharedKeyAccess
    isHnsEnabled: enableHierarchicalNamespace
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: blobEncryptionEnabled
        }
        file: {
          enabled: fileEncryptionEnabled
        }
      }
    }
  }
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: hubstorage
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      enabled: enableBlobRetention
      days: blobRetentionDays
    }
  }
}

// Create the eh auto-capture container
resource capturedEventsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  name: capturedEventsBlobContainerName
  parent: blobServices
  properties: {
    metadata: {}
    publicAccess: 'None'
  }
}

// Create the code processed events storage container
resource processedEventsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  name: processedEventsBlobContainerName
  parent: blobServices
  properties: {
    metadata: {}
    publicAccess: 'None'
  }
}

@description('The event trigger storage account.')
resource eventStorage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountNameEvents
  location: location
  kind: 'StorageV2'
  sku: {
    name: storageSku
  }
  properties: {
    allowBlobPublicAccess: allowBlobPublicAccess
    accessTier: storageTier
    allowSharedKeyAccess: allowSharedKeyAccess
    isHnsEnabled: enableHierarchicalNamespace
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: blobEncryptionEnabled
        }
        file: {
          enabled: fileEncryptionEnabled
        }
      }
    }
  }
}

resource blobServicesEvents 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: eventStorage
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      enabled: enableBlobRetention
      days: blobRetentionDays
    }
  }
}

// Create the cool storage container
resource eventTriggerContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  name: eventTriggerContainerName
  parent: blobServicesEvents
  properties: {
    metadata: {}
    publicAccess: 'None'
  }
}

output hubStorageAccountId string = hubstorage.id
output hubStorageAccountName string = hubstorage.name
output hubCapturedEventsContainerName string = capturedEventsContainer.name
output hubProcessedEventsContainerName string = processedEventsContainer.name

output storageAccountEventsId string = eventStorage.id
output storageAccountEventsFullname string = eventStorage.name
output eventTriggerBlobContainerName string = eventTriggerContainer.name
