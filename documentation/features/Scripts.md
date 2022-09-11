# Scripts



* [Adding a Script](#adding-a-script)
* [Metadata](#metadata)
* [Output](#output)
  * [Logging](#logging)
  * [Data](#data)
    * [**OutputType**: List](#outputtype-list)
    * [**OutputType**: Object](#outputtype-object)
    * [**OutputType**: Text](#outputtype-text)
    * [**OutputType**: None](#outputtype-none)


---

## Adding a Script

To add a script, copy your custom .ps1 script file to the **Scripts** folder. 

---

## Metadata

Metadata can be added to your script to control how your scripts output is read by TsGui.

To add metadata, copy and paste the following somewhere in your PowerShell script. This is a comment block so will not effect the functionality of your script. 

```json
<#ScriptSettings
{
    "Description": "List deployments for the connected client",
    "DisplayName": "ConfigMgr Deployments",
    "LogOutput": false,
    "LogScriptContent": false,
    "OutputType": "List",
}
ScriptSettings#>
```

Update the fields appropriately:
* **Description**: The description to appear in TsGui
* **DisplayName**: The name to be shown in TsGui. If this is empty the script file name will be used
* **LogOutput**: Will log the output of the script to the output pane, even if DisplayElement is Tab or Modal
* **LogScriptContent**: Set this to true to output the content of the script file to the logging pane in TsGui
* **OutputType**: Define the expected output of the script. Valid options (case sensitive): 
  * [List](#outputtype-list)
  * [Object](#outputtype-object)
  * [Text](#outputtype-text)
  * [None](#outputtype-none)

The json in the comment block will be parsed when the script is read to create the appropriate configuration.


---
## Output

### Logging
TsGui will capture Write-Information, Write-Warning, Write-Error etc. from your script and output it to the *Output* pane.

If you wish to highlight your text in the Output pane, add **\*\*** to the start of your message.


### Data
The output of your script can be viewed in a number of ways. These are configured in the custom script [metadata](#metadata) using the **OutputType** fields:



#### **OutputType**: List
List of PowerShell objects


#### **OutputType**: Object
A single PowerShell object created from a hashtable

#### **OutputType**: Text
Simple text output

#### **OutputType**: None
No output