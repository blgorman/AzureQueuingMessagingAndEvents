#note: ensure you are logged in on the correct subscription#
#Deploy all resources#
loc='centralus'
deploymentName='manualDeploymentOfAQMETalkResources'
templateFilePath='allMessagingResources.bicep'
parametersFilePath='allMessagingResources.parameters.json'

az deployment sub create --name $deploymentName --location $loc --template-file $templateFilePath --parameters $parametersFilePath
