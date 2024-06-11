#note: ensure you are logged in and on the correct subscription and pre-create a resource group#
#note2: You should not run this until you have first run deployStorage.sh or you will get errors#
rg='QueueingMessagingAndEvents'
loc='centralus'
az group create --name $rg --location $loc
#deploy event hub #
templateFilePath='eventHub.bicep'
parametersFilePath='eventHub.parameters.json'
az deployment group create --resource-group $rg --template-file $templateFilePath --parameters $parametersFilePath
