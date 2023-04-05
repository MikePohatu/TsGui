# Scripts

**[Example Config](/Config_Examples/Config_Scripts.xml)**

* [Adding a Script](#adding-a-script)
  * [In a Query](#in-a-query)
  * [As an action](#as-an-action)
  * [Global Scripts](#global-scripts)
  * [Parameters](#parameters)
  * [Passwords \& SecureStrings](#passwords--securestrings)
    * [Masked Task Sequence Variables](#masked-task-sequence-variables)
    * [Using a PasswordBox](#using-a-passwordbox)
* [Metadata](#metadata)
* [Script Output](#script-output)
* [Logging](#logging)

---

## Adding a Script

You can add a script in a query, as an action, or globally.

First copy your custom .ps1 script file to the **Scripts** folder.

If you don't want to copy the script with your TsGui files e.g. you're using a script installed elsewhere, you can use the \<Path> element with the path to the files, rather than the \<Name> element.

### In a Query
Scripts can be added as a query by using the 'PowerShell' type and adding a \<Script> block. The following sub elements 

* Name - the filename of the script copied above
* Parameter - one or more Parameter elements can be used to set script parameters. See the [Parameters](#parameters) section below.

```xml
<Query Type="PowerShell">
  <Script>
    <Name>GetApplications.ps1</Name>
    <Parameter Name="AppSearch">
      <!-- You can use SetValue like you do with a GuiOption for the value of the parameter -->
      <SetValue>
        <Query Type="LinkTo">ID_AppSearch</Query>
      </SetValue>
    </Parameter>
  </Script>
  

  <!-- Properties from the output to include in the list -->
  <Property Name="Name"/>
  <Property Name="Name" />
  <Property Name="Version">
    <Prefix>Version: </Prefix>
  </Property>
  <Separator> | </Separator>
</Query>
```

**Note** Scripts in queries will only run once by default. This is to prevent constant updating causing multiple long running processing from hanging the application. This is most common with FreeText options being [linked](/documentation/features/OptionLinking.md) to [parameters](#parameters).

To override this behaviour, set the 'Reprocess' attribute on the query as below:

```xml
<Query Type="PowerShell" Reprocess="TRUE">
```
<br/>

---

### As an action
If you want to trigger a script manually from TsGui, rather than using it to get data for the UI, you can do this using an Action. 

To do this, create an "ActionButton" GuiOption, then add an \<Action Type="PowerShell"> element. The content of the \<Action> element is identical to the \<Script> element used in the [query](#in-a-query)

```xml
<GuiOption Type="ActionButton">
  <ButtonText>Run script</ButtonText>
  
  <Action Type="PowerShell">
    <Name>Example.ps1</Name> 
    <Parameter Name="Message" Value="Why hello there" />
    <Switch Name="Verbose" />
  </Action>
</GuiOption>
```

The script will be run whenever the button is pressed.
<br/><br/>

---

### Global Scripts
Scripts can also be defined globally and then referenced by one or more queries. This can be useful if you want to define all your scripts in a separate XML file and then use [Configuration Imports](/documentation/ConfigImports.md).

**Note:** Option Linking is not supported in parameters of global scripts, so you can't use the value of another GuiOption for a parameter in your script. This may work is specific cases, but is not guaranteed to work consistently.

To do this, create a \<Scripts> element in the root of your TsGui config. You can then add multiple \<Script> blocks which are the same as those in a [query](#in-a-query).

On each Script block, set a unique ID attribute you will use to reference the script elsewhere.

```xml
<TsGui>
  <Scripts>
    <Script Type="PowerShell" ID="Global_Example" Name="Example.ps1">
      <Parameter Name="Message" Value="This one is global" />
    </Script>
  </Scripts>
</TsGui>
```

To use the script, in your GuiOption create a "PowerShell" query type, then create a \<Global> attribute set with the value of the ID you created above.


```xml
<GuiOption Type="FreeText">
  <Variable>Example</Variable>
  <Label>Example</Label>

  <SetValue>
    <Query Type="PowerShell" Global="Global_Example" />
  </SetValue>
</GuiOption>
```
<br/>

---

### Parameters
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
<br/>

---

### Passwords & SecureStrings
Passwords and secrets should never be stored in clear text e.g. inside scripts. To deal with the requirement to pass secure credentials when using scripts, the following approaches can be taken:
* Masked Task Sequence Variables
* Using a PasswordBox
<br/><br/>

#### Masked Task Sequence Variables
When using a ConfigMgr task sequence, store your secret in a task sequence variable. This can then be passed into the script as a [parameter](#parameters), queried using an [EnvironmentVariable query](/documentation/features/queries/EnvironmentVariable.md) in the \<SetValue> element

![TrafficLightErr](/documentation/images/configmgr_masked_var.png)
<br/><br/>

#### Using a PasswordBox
Alternatively you can use a [PasswordBox](/documentation/options/PasswordBox.md) GuiOption. The user can then enter the password at run time. This is already stored as a SecureString object internally, so can be passed directly to the script parameter. To use this approach:

   * The PasswordBox GuiOption must have the **ID** attribute set. This is the same ID used by the [Option Linking](/documentation/features/OptionLinking.md) feature. 
   * Use a **SecureParameter** rather than Parameter element. 
   * Set the **SourceID** attribute on the SecureParameter. This must match the ID set on the PasswordBox 


```xml
<GuiOption Type="PasswordBox" ID="id_of_passwordbox">
    <Label>Password:</Label>
    <NoPasswordMessage>Password cannot be empty</NoPasswordMessage>
    <AllowEmpty>FALSE</AllowEmpty>
</GuiOption>
```
```xml
<Script Type="PowerShell" Name="Example.ps1">
  <SecureParameter Name="pw" SourceID="id_of_passwordbox" />
</Script>

```

Note although the ID is used, this does not function like Option Linking e.g. when the source changes a refresh is triggered. The SecureString value of the PasswordBox will be queried each time the script is run. 
<br/><br/>

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
