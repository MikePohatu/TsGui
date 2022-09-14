Param (
    [string]$Message = "Example script"
)

Write-Information $Message
Write-Output $Message
<#ScriptSettings
{
    "LogOutput": false,
    "LogScriptContent": false,
    "OutputType": "Text",
}
ScriptSettings#>