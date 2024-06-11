# Run on your own sub

If you want to run this on your own subscription, complete the following tasks:

## Deploy the resources

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
## Create SAS Access Connection Strings

After Deployment, you will need to manually create the SAS connection strings in your event hub and service bus resources.

### Event Hub (not the event hub namespace!)

Create access for the SDK using SAS tokens from your event hub.

1. Open the Event Hub (not the namespace, the hub!)
1. Click on Shared Access Policies
1. Click on Add
1. Add the `Producer` with `Send` permissions
1. Add the `Consumer` with `Listen` permissions
1. Get the values of the connection string for use in the Event Hub Code projects

### Service Bus

Create access for the SDK using SAS tokens from your service bus.

1. Open the Service Bus 
1. Click on Shared Access Policies
1. Get the RootManageSharedAccessKey connection string for use in the Service Bus Administrator code project

#### Service Bus Queues

Navigate to the created Queue  

1. Click on Shared Access Policies
1. Click on Add
1. Add the `Producer` with `Send` permissions
1. Add the `Consumer` with `Listen` permissions
1. Get the connection strings for use in the code modifications

#### Service Bus Topics  

Navigate to the created Topic

1. Click on Shared Access Policies
1. Click on Add
1. Add the `Producer` with `Send` permissions
1. Add the `Consumer` with `Listen` permissions
1. Get the connection strings for use in the code modifications

## Modify Code

After getting all the access tokens created, you will need to modify the code in the projects you want to use

1. Open the code project in Visual Studio
1. Modify the `secrets.json` file with the connection strings you created [use USER SECRETS!]
1. Leverage the user-secrets-example.txt file to help determine what secrets you need to add to the `secrets.json` file. 

## Run the code

After modifying the code, you can run the code in the projects you want to use.