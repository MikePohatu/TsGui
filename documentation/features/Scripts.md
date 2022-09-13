# Scripts


* [Adding a Script](#adding-a-script)
* [Parameters](#parameters)
* [Metadata](#metadata)
* [Script Output](#script-output)
* [Logging](#logging)

---

## Adding a Script

To add a script, copy your custom .ps1 script file to the **Scripts** folder. 

---

## Parameters
Parameters & Switches can be passed to the script using a \<Parameter> or a \<Switch> element. 

A Parameter can either have a value set statically, or set using a query similar to a GuiOption using the \<SetValue> element. Using a query allows you to pass values from the UI or other values from WMI or environment variables.

```xml
<!-- Set a parameter like this. This is equivalent to 
  Example.ps1 -Message 'Why hello there' -->
<Parameter Name="Message" Value="Why hello there" />

<!-- Set a switch like this. Combined with the above this is equivalent to:
  Example.ps1 -Message 'Why hello there' -Verbose -->
<Switch Name="Verbose" />

<!-- You can use SetValue like you do with a GuiOption for the value of the parameter -->
<Parameter Name="AppSearch">
  <SetValue>
    <Query Type="LinkTo">Source_ID</Query>
  </SetValue>
</Parameter>
```

---

## Metadata

Metadata can be added to your script to control how your script output is read by TsGui.

To add metadata, copy and paste the following somewhere in your PowerShell script. This is a comment block so will not effect the functionality of your script. 

```json
<#ScriptSettings
{
    "LogOutput": false,
    "LogScriptContent": false
}
ScriptSettings#>
```

Update the fields appropriately:
* **LogOutput**: Will log the output of the script to the logging pane
* **LogScriptContent**: Set this to true to output the content of the script file to the logging pane in TsGui

The json in the comment block will be parsed when the script is read to create the appropriate configuration.

---

## Script Output
Remember that certain GuiOption types require two values, one for the value, and one for the display text i.e. collection types such as DropDownList and TreeView. To achieve this you need to be able to configure which properties are used to create these values. 

If your script is being used as a query for one of these types, make sure your scripts returns a list of object data types that set properties, such as a hashtable or custom object. Returning a simple string or integer won't work. 

If used in a FreeText, InfoBox, Heading type, you can return an object type or a 'primitive' type. Object data types will concatenate the property values into a string like normal.

## Logging
TsGui will capture Write-Information, Write-Warning, Write-Error etc. from your script and output it to the *Output* pane.

If you wish to highlight your text in the Output pane, add **\*\*** to the start of your message.
