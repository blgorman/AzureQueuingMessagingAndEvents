#note: ensure you are logged in on the correct subscription and have created a resource group#
rg='QueueingMessagingAndEvents'
loc='centralus'
az group create --name $rg --location $loc
#deploy event hub #
templateFilePath='serviceBus.bicep'
parametersFilePath='serviceBus.parameters.json'
az deployment group create --resource-group $rg --template-file $templateFilePath --parameters $parametersFilePath
