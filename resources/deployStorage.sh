#note: ensure you are on the correct subscription and create a resource group#
rg='QueueingMessagingAndEvents'
loc='centralus'
az group create --name $rg --location $loc
#deploy storage #
templateFilePath='storage.bicep'
parametersFilePath='storage.parameters.json'
az deployment group create --resource-group $rg --template-file $templateFilePath --parameters $parametersFilePath
