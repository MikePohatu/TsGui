# Actions

An action is used to initiate some process not directly related to setting a variable e.g. refresh validation status, run a script, or initiate [Authentication](/documentation/Authentication/README.md) 

* [Authentication](/documentation/Authentication/ActiveDirectoryAuthentication.md#the-actionbutton-guioption) - initiate an authentication process.
* [Reprocess](/documentation/features/Queries.md#reprocessing-queries) - Reprocess the queries on an option or rerun a script.
* [PowerShell](/documentation/features/Scripts.md#as-an-action) - Run a script.

### The ActionButton GuiOption
The ActionButton provides a button that the user can click to initiate the relevant process. One or more **Action** elements must be defined, with one of the types specified above. 



The **ButtonText** element configures the text in the button to be displayed to the user.

The **IsDefault** attribute on the GuiOption element will make the button attempt to click when the Enter key is pressed. Note that this behaviour may be overridden if another element is focused that already handles the key press event.  

Single action:
```
<GuiOption Type="ActionButton" IsDefault="TRUE">
    <Action Type="Authentication" AuthID="conf_auth"/>
    <ButtonText>Login</ButtonText>
</GuiOption>
```

---

### Multiple Actions
You can run multiple actions with a single ActionButton (requires version 2.4.0.6 or higher). Add multiple \<Action> elements to your GuiOtion as below:

```
<GuiOption Type="ActionButton" IsDefault="TRUE">
    <Action Type="Authentication" AuthID="conf_auth"/>
    <Action Type="PowerShell">
        <Name>Example.ps1</Name> 
        <Parameter Name="Message" Value="Why hello there" />
        <Switch Name="Verbose" />
    </Action>
    <ButtonText>Login</ButtonText>
</GuiOption>
```

### Ordered vs Unordered Actions
By default, if you add multiple actions to your ActionButton, they will be processed in order one after the other. Each action will wait for the previous one to complete before starting. 

You can also run all the actions at once. All actions will be initiated at once. To do this, set the **OrderedActions** attribute to FALSE.

```
<GuiOption Type="ActionButton" IsDefault="TRUE" OrderedActions="FALSE">
    <Action Type="Authentication" AuthID="conf_auth"/>
    <Action Type="PowerShell">
        <Name>Example.ps1</Name> 
        <Parameter Name="Message" Value="Why hello there" />
        <Switch Name="Verbose" />
    </Action>
    <ButtonText>Login</ButtonText>
</GuiOption>
```