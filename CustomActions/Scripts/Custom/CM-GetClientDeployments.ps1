$DeviceName = "{{CLIENT}}"
$NameSpace = "{{CM_SITE_NAMESPACE}}"
$Server = "{{CM_SERVER}}"

$Collections = Get-WmiObject -Namespace $NameSpace -ComputerName $Server -Query "SELECT SMS_Collection.* FROM SMS_FullCollectionMembership, SMS_Collection where name = '$DeviceName' and SMS_FullCollectionMembership.CollectionID = SMS_Collection.CollectionID"

foreach ($Collection in $Collections) {
    Get-WmiObject -Namespace $NameSpace -ComputerName $Server -Query "select * from SMS_DeploymentInfo WHERE CollectionID='$($Collection.CollectionID)'" | Select TargetName, CollectionName | Sort-Object -Property TargetName
}
 


<#ActionSettings
{
    "DisplayName": "ConfigMgr Deployments",
    "DisplayElement": "Tab",
    "OutputType": "List",
    "Description": "List deployments for the connected client",
    "RunOnConnect": false,
    "RunOnClient": false,
    "RequiresServerConnect": true,
    "LogScriptContent": false,
    "LogOutput": false,
    "FilterProperties": ["TargetName", "CollectionName"]
}
ActionSettings#>