# Azure Queueing Messaging and Events

In this repo you'll find the code and references for my talk on Azure Messaging, Queueing, and Event Services.

When you first start working with Azure serverless solutions, it can be tricky to determine which solution is the best for messaging and event handling.  It can even be confusing as to what the Event Hub and Event Grid are and why you need both and why sometimes they are referenced by each other.

Hopefully, this talk/repo will help to clear these things up, as well as which solution to use for messaging and queuing in various scenarios.

## Presentation Slides  

The slides are available to view on my personal training site (free). [You can view the slides here](https://training.majorguidancesolutions.com/courses/blog-posts-and-talks/contents/6664a6e3863a0). 

## Event Hub

The Azure Event Hub is your choice for Big Data stream ingestion into Azure.  Along with Event Hub, you have the IoT Hub, which is a subset of the Event Hub. If you have a stream of data coming in at hundreds or thousands of records per second, the Event Hub is your stream ingestion tool of choice.

To create an event hub, you need a namespace.  You then place one or more hubs in the namespace.  The namespace determines the overall throughput and the hub determines how the data is separated and how many individual applications can read their own version of the data.

Most importantly, the talk helps to shed light on the information in this table:

[Comparison of services](https://learn.microsoft.com/azure/service-bus-messaging/compare-messaging-services#comparison-of-services?WT.mc_id=AZ-MVP-5004334)

References for Event Hub and the Event Hub Namespace from this talk include:

- [FAQ - What is an Event Hub Namespace](https://learn.microsoft.com/azure/event-hubs/event-hubs-faq?WT.mc_id=AZ-MVP-5004334)  
- [What is Azure Event Hub](https://learn.microsoft.com/azure/event-hubs/event-hubs-about?WT.mc_id=AZ-MVP-5004334)  
- [Event Hub Terminology](https://learn.microsoft.com/azure/event-hubs/event-hubs-features?WT.mc_id=AZ-MVP-5004334)  
- [Capture events to Azure Storage](https://learn.microsoft.com/azure/event-hubs/event-hubs-capture-overview?WT.mc_id=AZ-MVP-5004334)  
- [Use .Net to send and receive events](https://learn.microsoft.com/azure/event-hubs/event-hubs-dotnet-standard-getstarted-send?WT.mc_id=AZ-MVP-5004334&tabs=passwordless%2Croles-azure-portal)

## Event Grid

The Azure event grid is the tool of choice when events are fired at Azure and you need to respond.  Common examples are storage blobs created or virtual machines restarted.  Essentially, everything you do in Azure is monitored, and if you want to subscribe to the event you can likely make it happen.

In some cases you will also choose to write your own custom events.  This is a bit trickier, but you can utilize the same data payload structure as a typical azure event to also create an event topic and subscription(s) to your custom events.

- [What is Azure Event Grid](https://learn.microsoft.com/azure/event-grid/overview?WT.mc_id=AZ-MVP-5004334)  
- [How to filter events with event grid](https://learn.microsoft.com/azure/event-grid/how-to-filter-events?WT.mc_id=AZ-MVP-5004334)  
- [Event Grid message delivery and retry](https://learn.microsoft.com/azure/event-grid/delivery-and-retry?WT.mc_id=AZ-MVP-5004334)  
- [Dead Letter and Retry Policies](https://learn.microsoft.com/azure/event-grid/manage-event-delivery?WT.mc_id=AZ-MVP-5004334)  
- [Quickstart: Custom event handling with Event Grid](https://learn.microsoft.com/azure/event-grid/custom-event-quickstart?WT.mc_id=AZ-MVP-5004334)

## Service Bus

Service Bus has two types of use in Azure.  The first is simple pub/sub for messaging.  This is much easier than a custom event to implement, so if you just need to send information from one application to another in a disconnected fashion, you should likely choose service bus.  

Service bus topics can then be subscribed to from one or more consumers.  Each consumer can further filter the data they care about.  For example, you could have three apps all responding to blob storage, but each could be filtering for different things like a specific type of image or any file with a common prefix.  Finally, you could even filter to something like the container name or a custom subject that is included with the message.

Service Bus Queues are the tool of choice when you need a guaranteed FIFO object at azure to ensure message processing is handled in the order the messages are received.  Once a queue is created, any application with the correct SAS for listening can consume messages from the Queue.  Any application with the SAS for writing can publish messages to the queue.  

- [Queues, Topics, and Subscriptions](https://docs.microsoft.com/azure/service-bus-messaging/service-bus-queues-topics-subscriptions?WT.mc_id=AZ-MVP-5004334)
- [Getting started with Queues](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues?WT.mc_id=AZ-MVP-5004334&tabs=passwordless)
- [Duplicate Message Detection](https://learn.microsoft.com/azure/service-bus-messaging/duplicate-detection?WT.mc_id=AZ-MVP-5004334)
- [Dead letter Queues](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-dead-letter-queues?WT.mc_id=AZ-MVP-5004334)
- [Topics and Subscriptions](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-dotnet-how-to-use-topics-subscriptions?WT.mc_id=AZ-MVP-5004334&tabs=passwordless)
- [Topic Filters and Actions](https://learn.microsoft.com/azure/service-bus-messaging/topic-filters?WT.mc_id=AZ-MVP-5004334)
- [Using Filters](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-filter-examples?WT.mc_id=AZ-MVP-5004334)
- [Service Bus Messaging Quotas](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-quotas?WT.mc_id=AZ-MVP-5004334)  

## Azure Storage Queue

The Azure storage queue is used to read and write messages similar to the service bus queue.  In storage queue, order cannot be guaranteed.  The messages in a storage queue are smaller than the messages in service bus, but storage queue can store millions of messages and they are automatically purged after 7 days.  Storage queue also has the ability for clients to take out a lease and if something fails the message goes back into the queue.

- [What are Azure Storage Queues](https://learn.microsoft.com/azure/storage/queues/storage-queues-introduction?WT.mc_id=AZ-MVP-5004334)
- [Working with Storage Queues](https://learn.microsoft.com/azure/storage/queues/storage-quickstart-queues-dotnet?WT.mc_id=AZ-MVP-5004334&tabs=passwordless%2Croles-azure-portal%2Cenvironment-variable-windows%2Csign-in-azure-cli)  


## Storage Queues vs Service Bus Queues

[When do you use each one?](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-azure-and-service-bus-queues-compared-contrasted?WT.mc_id=AZ-MVP-5004334)  


## Run on your own sub

If you want to run this on your own subscription, complete the following tasks:

### Subscription level deploy all resources

If you want to just do the whole thing at once, just deploy to the subscription.

1. Make sure you are logged in to your subscription
1. Make sure you are in the correct subscription in a terminal window
1. Validate/change any parameters in the `allMessagingResources.parmaeters.json` file
1. Run one of the following commands (depending on your shell):

    PowerShell:  

    ```PowerShell
    bash deployAll.sh
    ```  
  
    or Bash:  
  
    ```Bash
    bash ./deployAll.sh
    ```

### Deploy each resource individually

If you want to deploy each resource individually, you can do so by navigating to the folder of the resource you want to deploy and running the deploy script.

1. Make sure you are logged in to your subscription
1. Make sure you are in the correct subscription in a terminal window
1. Manually create a resource group for the resource you want to deploy in the location of your choice
1. Validate/change any parameters in the `<resource>.parmaeters.json` file
1. Run one of the following commands (depending on your shell):

    PowerShell:  

    ```PowerShell
    bash deploy<resource>.sh
    ```  
  
    or Bash:  
  
    ```Bash
    bash ./deploy<resource>.sh
    ```  
### Create SAS Access Connection Strings

You will need to manually create the SAS connection strings in your event hub and service bus resources.

#### Event Hub (not the event hub namespace!)

Create access for the SDK using SAS tokens from your event hub.

1. Open the Event Hub (not the namespace, the hub!)
1. Click on Shared Access Policies
1. Click on Add
1. Add the `Producer` with `Send` permissions
1. Add the `Consumer` with `Listen` permissions
1. Get the values of the connection string for use in the Event Hub Code projects

#### Service Bus

Create access for the SDK using SAS tokens from your service bus.

1. Open the Service Bus 
1. Click on Shared Access Policies
1. Get the RootManageSharedAccessKey connection string for use in the Service Bus Administrator code project

##### Service Bus Queues

Navigate to the created Queue  

1. Click on Shared Access Policies
1. Click on Add
1. Add the `Producer` with `Send` permissions
1. Add the `Consumer` with `Listen` permissions
1. Get the connection strings for use in the code modifications

##### Service Bus Topics  

Navigate to the created Topic

1. Click on Shared Access Policies
1. Click on Add
1. Add the `Producer` with `Send` permissions
1. Add the `Consumer` with `Listen` permissions
1. Get the connection strings for use in the code modifications

### Modify Code

After getting all the access tokens created, you will need to modify the code in the projects you want to use

1. Open the code project in Visual Studio
1. Modify the `secrets.json` file with the connection strings you created [use USER SECRETS!]
1. Leverage the user-secrets-example.txt file to help determine what secrets you need to add to the `secrets.json` file. 

### Run the code

After modifying the code, you can run the code in the projects you want to use.