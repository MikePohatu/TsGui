Param (
    [string]$Message = "Example script"
)

Write-Information $Message
Write-Output $Message
<#ScriptSettings
{
    "LogOutput": true,
    "LogScriptContent": false,
    "OutputType": "Text",
}
ScriptSettings#>