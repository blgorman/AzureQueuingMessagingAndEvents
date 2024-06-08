# Read me

To make this project work, you need to complete a few things

## Create an Azure Storage Account with a Storage Queue

First, you must create the Storage Account and Queue.

1. Create the Storage Account
1. Create the Queue

## Get the Queue Name and Connection String

Use the Access Keys from the account

1. Set the QueueName into user secrets (or appsettings.json)
1. Set the ConnectionString into user secrets

## Ensure your secrets

Ensure you have leveraged the example secrets file to fill in the required information in the secrets file

## Run the program

This program interacts with the queue, creating if it doesn't exist, then adding messages, asks you if you want to lease (don't do that unless you want to see how to lock messages for an hour), then deletes the messages.  The program then asks if you want to delete the queue and then completes.

## Directory.Build.props

The project is set to read all the versions from the Directory.Build.props file.  Use that to leverage which version you want to run and/or update any NuGet references to new versions that are out by the time you review this code.

The default version at time of publish is DN6.  You can leverage DN7 by changing the extension on the props to dn6 then the dn7 extension to props and it will default to version 7.  Later, you can change it to DN8 or DN9/10/11, etc and just update the other libraries as needed as well (check for updates in the Nuget Packages for Solution)

## Conclusion

Use this project to help you learn how to interact with the Azure Storage Queue
