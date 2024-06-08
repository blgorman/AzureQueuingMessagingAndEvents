# Read me

To make this project work, you need to complete a few things

## Create an Azure Service Bus Namespace & Topic (Must be a Standard Tier or better)

First, you must create the Namespace and Topic.

1. Create the namespace
1. Create the Topic (i.e. MoviesToReview).  Keep track of what you name the topic, as that will be critical. Whatever topic name you use will need to be set in the appsettings.json file (defaulted to MoviesToReview as per the book).

## Create the SAS tokens

Next, Create SAS tokens

1. Leverage the RootAccessConnectionString for the topic, or, alternatively, Create a Manage (All things) token on the topic

    Get the token of your choice and put it in the secrets for the ServiceBusAdministrator project.

1. Create a Listen (Consumer) token

    Get the token and put it in the secrets for the ServiceBusConsumer project

1. Create a Send (Producer) token

    Get the token and put it in the secrets for the ServiceBusPublisher project

## Ensure your secrets

Ensure you have leveraged the example secrets file for each project to fill in the blanks for each project in the project secrets file.

## Run the administrator program

While you can create all of the subscriptions in your topic manually (and that might be a great exercise before sitting for the exam), you can right-click and use the Debug -> Start New Instance to run an instance of the ServiceBusAdministrator program so that the subscriptions will be created.  It may also be interesting and a good study point to then review the subscriptions for your topic in the portal.

## Run the program

The default/startup program is the publisher.  Run it and validate you have the messages in your Queue (Subscriptions MUST be created first or this could have some bad side-effects)

## Debug -> Start New Instance

Right-click on the Consumer project and select Debug-> Start New Instance to consume the messages from the queue.  In this program, you get to pick subscriptions to consume each time you run it.  Note that consuming one subscription has no effect on the others (i.e. consume "ALL" and then look in the portal to see that both the Family and Adult movies still have messages)

## Directory.Build.props

The project is set to read all the versions from the Directory.Build.props file.  Use that to leverage which version you want to run and/or update any NuGet references to new versions that are out by the time you review this code.

The default version at time of publish is DN6.  You can leverage DN7 by changing the extension on the props to dn6 then the dn7 extension to props and it will default to version 7.  Later, you can change it to DN8 or DN9/10/11, etc and just update the other libraries as needed as well (check for updates in the Nuget Packages for Solution)

## Conclusion

Use this project to help you learn how to interact with the Service Bus Topics on a Pub/Sub with multiple subscriptions
