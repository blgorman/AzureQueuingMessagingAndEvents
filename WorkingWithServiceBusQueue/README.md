# Read me

To make this project work, you need to complete a few things

## Create an Azure Service Bus Namespace & Queue

First, you must create the Namespace and Queue.

1. Create the namespace
1. Create the Queue

## Create the SAS tokens

Next, Create SAS tokens

1. Create a Listen (Consumer) token

    Get the token and put it in the secrets for the ServiceBusQueueConsumer project

1. Create a Send (Producer) token

    Get the token and put it in the secrets for the ServiceBusQueuePublisher project

## Ensure your secrets

Ensure you have leveraged the example secrets file to fill in the blanks for each project in the project secrets file

## Run the program

The default/startup program is the publisher.  Run it and validate you have the messages in your Queue

## Debug -> Start New Instance

Right-click on the Consumer project and select Debug-> Start New Instance to consume the messages from the queue.

## Directory.Build.props

The project is set to read all the versions from the Directory.Build.props file.  Use that to leverage which version you want to run and/or update any NuGet references to new versions that are out by the time you review this code.

The default version at time of publish is DN6.  You can leverage DN7 by changing the extension on the props to dn6 then the dn7 extension to props and it will default to version 7.  Later, you can change it to DN8 or DN9/10/11, etc and just update the other libraries as needed as well (check for updates in the Nuget Packages for Solution)

## Conclusion

Use this project to help you learn how to interact with the Service Bus Queue
