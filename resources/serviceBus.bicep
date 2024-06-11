@description('Location for all resources.')
param location string = resourceGroup().location

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

@description('Unique string for the sb namespace')
var serviceBusNamespaceName = substring('${sbProjectName}sbns${uniqueString(resourceGroup().id)}', 0, 24)

resource serviceBusNamespaceResource 'Microsoft.ServiceBus/namespaces@2018-01-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: serviceBusSku
  }
  properties: {
  }
}

resource serviceBusQueueResource 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBusNamespaceResource
  name: serviceBusQueueName
  properties: {
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
  }
}

resource serviceBusTopicResource 'Microsoft.ServiceBus/namespaces/topics@2017-04-01' = {
  parent: serviceBusNamespaceResource
  name: serviceBusTopicName
  properties: {
    defaultMessageTimeToLive: defaultMessageTimeToLive
    maxSizeInMegabytes: maxSizeInMegabytes
    requiresDuplicateDetection: requiresDuplicateDetection
    duplicateDetectionHistoryTimeWindow: duplicateDetectionHistoryTimeWindow
    enableBatchedOperations: enableBatchedOperations
    supportOrdering: supportOrdering
    autoDeleteOnIdle: autoDeleteOnIdle
    enablePartitioning: enablePartitioning
    enableExpress: enableExpress
  }
}

resource queueConsumerSAS 'Microsoft.ServiceBus/namespaces/queues/authorizationRules@2022-10-01-preview' = {
  parent: serviceBusQueueResource
  name: 'Consumer'
  properties: {
    rights: [
      'Listen'
    ]
  }
  dependsOn: [
    serviceBusNamespaceResource
    serviceBusQueueResource
  ]
}

resource queueProducerSAS 'Microsoft.ServiceBus/namespaces/queues/authorizationRules@2022-10-01-preview' = {
  parent: serviceBusQueueResource
  name: 'Producer'
  properties: {
    rights: [
      'Send'
    ]
  }
  dependsOn: [
    serviceBusNamespaceResource
    serviceBusQueueResource
    queueConsumerSAS
  ]
}

resource topicConsumerSAS 'Microsoft.ServiceBus/namespaces/topics/authorizationrules@2022-10-01-preview' = {
  parent: serviceBusTopicResource
  name: 'Consumer'
  properties: {
    rights: [
      'Listen'
    ]
  }
  dependsOn: [
    serviceBusNamespaceResource
    serviceBusTopicResource
    queueProducerSAS
  ]
}

resource topicProducerSAS 'Microsoft.ServiceBus/namespaces/topics/authorizationrules@2022-10-01-preview' = {
  parent: serviceBusTopicResource
  name: 'Producer'
  properties: {
    rights: [
      'Send'
    ]
  }
  dependsOn: [
    serviceBusNamespaceResource
    serviceBusTopicResource
    queueConsumerSAS
    queueProducerSAS
    topicConsumerSAS
  ]
}

/** All of this is done in code but wanted to capture it in IAC **/
// @description('Name of the Subscription')
// param serviceBusSubscriptionAllMovies string = 'AllMovies'
// param serviceBusSubscriptionFamilyMovies string = 'AllFamilyMovies'
// param serviceBusSubscriptionAdultMovies string = 'AllAdultMovies'

// @description('Name of the Rule')
// param serviceBusRuleAllMovies string = 'AllMoviesRule'
// param serviceBusRuleFamilyMovies string = 'AllFamilyMoviesRule'
// param serviceBusRuleAdultMovies string = 'AllAdultMoviesRule'

// resource serviceBusTopicAllMoviesSubscriptionResource 'Microsoft.ServiceBus/namespaces/topics/Subscriptions@2017-04-01' = {
//   parent: serviceBusTopicResource
//   name: serviceBusSubscriptionAllMovies
//   properties: {
//     lockDuration: 'PT1M'
//     requiresSession: false
//     defaultMessageTimeToLive: 'P10675199DT2H48M5.4775807S'
//     deadLetteringOnMessageExpiration: false
//     maxDeliveryCount: 10
//     enableBatchedOperations: true
//     autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
//   }
// }

// resource serviceBusRuleResource 'Microsoft.ServiceBus/namespaces/topics/Subscriptions/Rules@2017-04-01' = {
//   parent: serviceBusTopicAllMoviesSubscriptionResource
//   name: serviceBusRuleAllMovies
//   properties: {
//   }
// }

output sbnamespaceId string = serviceBusNamespaceResource.id
output sbQueueId string = serviceBusQueueResource.id
output sbTopicId string = serviceBusTopicResource.id
output sbNamespaceName string = serviceBusNamespaceResource.name
output sbQueueName string = serviceBusQueueResource.name
output sbTopicName string = serviceBusTopicResource.name
output sbTopicConsumerSASID string = topicConsumerSAS.id
output sbTopicProducerSASID string = topicProducerSAS.id
output sbQueueConsumerSASID string = queueConsumerSAS.id
output sbQueueProducerSASID string = queueProducerSAS.id
