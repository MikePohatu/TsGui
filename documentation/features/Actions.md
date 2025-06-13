# Actions

An action is used to initiate some process not directly related to setting a variable e.g. refresh validation status, run a script, or initiate [Authentication](/documentation/Authentication/README.md) 

* [Authentication](/documentation/Authentication/ActiveDirectoryAuthentication.md#the-actionbutton-guioption) - initiate an authentication process.
* [Reprocess](/documentation/features/Queries.md#reprocessing-queries) - Reprocess the queries on an option.
* [PowerShell](/documentation/features/Scripts.md#as-an-action) - Run a script.

### The ActionButton GuiOption
The ActionButton provides a button that the user can click to initiate the relevant process. An **Action** element must be defined, with one of the types specified above. 



The **ButtonText** element configures the text in the button to be displayed to the user.

The **IsDefault** attribute on the GuiOption element will make the button attempt to click when the Enter key is pressed. Note that this behaviour may be overridden if another element is focused that already handles the key press event.  

```
<GuiOption Type="ActionButton" IsDefault="TRUE">
    <Action Type="Authentication" AuthID="conf_auth"/>
    <ButtonText>Login</ButtonText>
</GuiOption>
```
---
