# README

Use the following information to work with the Azure Event Hub via this code.

## Create a Storage Account

For auto-capture (Standard tier or better), create a storage account with a container that is Hierarchical

1. Create a storage account to auto-ingest (make sure to turn on `Enable hierarchical namespace` on the Advanced tab for Big Data ingestion).
1. Create a container for auto-capture.
1. Create the event hub Namespace
1. Create the event hub (if auto-capturing, point to the storage account container created earlier)

## Create SAS Tokens

Create Consumer (Listen) and Producer (Send) SAS tokens in the hub, get the hub name and the two connection strings for use in your secrets files

1. Create a Consumer (Listen) SAS Token. Put the connection string in the user secrets for the EventHubConsumer project

1. Create a Producer (Send) SAS Token.  Put the connection string in the user secrtes for the WorkingWithEventHub project

## Manually ingest data

In addition to auto-capture, this program manually ingests the data into another storage account container. 

1. Create a new container (in same or different storage account, your choice, it does not matter)
1. Get the storage account connection string and container name for the manual ingestion of data
1. Put the storage account connection string into the user-secrets for the EventHubConsumer project

## Directory.Build.props

The project is set to read all the versions from the Directory.Build.props file.  Use that to leverage which version you want to run and/or update any NuGet references to new versions that are out by the time you review this code.

The default version at time of publish is DN6.  You can leverage DN7 by changing the extension on the props to dn6 then the dn7 extension to props and it will default to version 7.  Later, you can change it to DN8 or DN9/10/11, etc and just update the other libraries as needed as well (check for updates in the Nuget Packages for Solution)

## Conclusion

Use this project to help you learn how to interact with the Azure Event Hub for producing and consuming events, including auto-capture and manual ingestion of data.
