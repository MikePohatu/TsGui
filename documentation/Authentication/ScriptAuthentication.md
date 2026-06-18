# Script Authentication


* [Overview](#overview)
  * [The Authentication Block](#the-authentication-block)
  * [The authentication script](#the-authentication-script)
  * [Authorization - TsGui or Script](#authorization---tsgui-or-script)
  * [Limiting authentication by group membership](#limiting-authentication-by-group-membership)
    * [Any vs All groups](#any-vs-all-groups)
  * [Using group membership to change the UI](#using-group-membership-to-change-the-ui)
  * [The UsernameBox GuiOption](#the-usernamebox-guioption)
  * [The PasswordBox GuiOption](#the-passwordbox-guioption)
  * [The ActionButton GuiOption](#the-actionbutton-guioption)
* [Complete Config Example](#complete-config-example)
* [Example authentication script](#example-authentication-script)


## Overview
Script authentication lets you use your own scripts to complete authentication. This would be useful if you want to authentication users against an internal web service for example. This feature requires version 2.4.0.5 or higher. 

This makes use of the [Scripts](/documentation/features/Scripts.md) feature. 

Script Authentication requires three things in your TsGui configuration:

1. An **\<Authentication  Type="Script"\>** block where the authentication settings i.e. password, is defined. This must have an **AuthID** attribute defined to identify it within the config
2. A **\<GuiOption Type="UsernameBox"\>** to enter the username, which must be configured to reference the AuthID of the Authentication block. 
3. A **\<GuiOption Type="PasswordBox"\>** to enter the password, which must be configured to reference the AuthID of the Authentication block. Hitting Enter will initiate the authentication process.
4. (Optional) You may also optionally configure a **\<GuiOption Type="ActionButton"\>** to add a 'Login' button to TsGui. This will also initiate the authentication process.  
---
<br>

### The Authentication Block
The Authentication element of your configuration must have a **Type="Script"** attribute and an **AuthID** attribute with a value that is unique within the config. This AuthID will be used by GuiOptions to reference and trigger the Authentication process.

A **Script** element is requried to define the settings for the script. See the [Scripts](/documentation/features/Scripts.md#script-files) documentation for advanced details of the \<Script\> block.

```xml
<Authentication Type="Script" AuthID="auth">
  <Script Type="PowerShell" Name="Auth.ps1" />
</Authentication>
```

### The authentication script

TsGui will pass a the username as a string, and the password as a SecureString to the Username and Password parameters respectively. These values will come from the [UsernameBox](#the-usernamebox-guioption) and [PasswordBox](#the-passwordbox-guioption).


Note that you don't have to add the Username and Password as [parameters](/documentation/features/Scripts.md#parameters) when configuring your \<Script\> block above. These are automatically added at runtime using the values from the UsernameBox and PasswordBox. 

```PowerShell
Param (
    [string]$Username,
    [securestring]$Password
)
```

TsGui is expecting the script to return a specific type of object as below. Your script is responsible for doing whatever processing is required, then updating the values of the object before returning it. 

```PowerShell
$result = [PSCustomObject]@{
    IsAuthenticated = $false
    IsAuthorized = $false
    Message = $null
    GroupMemberships = @{}
}
```

*The message field is not used at this time, but may be used to pass messages to the UI in future*

### Authorization - TsGui or Script
The object above has two ways to configure authorization. 
* IsAuthorized - The script updates this value to indicate whether the user is authorized to login.
* GroupMemberships - The script returns a hashtable of groups and whether the user is a member. TsGui then compares this against the configured rules (see [below](#limiting-authentication-by-group-membership)).

IsAuthorized assumes the script will evaluate user authorization, whereas GroupMemberships will return a list of memberships and TsGui will do the evaluation. 

Generally it is recommended to choose one approach or the other. If you include both in your configuration, GroupMemberships will win. 


### Limiting authentication by group membership

You can accept authentication based on the groups the user is a member of. This is evaluation against the **GroupMemberships** field returned by the script.

**Important**: the group names from your script must match the ones in your TsGui configuration. These are dictionary keys, and are case sensitive. TsGui will show an error if the keys are not found. 

GroupMemberships is a hash table with the group name as the key, and a boolean to indicate whether the user is a member or not. 

```PowerShell
$result = [PSCustomObject]@{
    IsAuthenticated = $true
    IsAuthorized = $false
    Message = $null
    GroupMemberships = @{
        "group 1" = $true
        "group 2" = $false
    }
}

```

<ins>As an attribute</ins><br>
Use the **Groups** attribute to list your groups by name, separated by a comma. Make note of any spaces. Don't add a space before or after the group name.
```xml
<Authentication Type="Script" AuthID="auth" Groups="group 1, group 2">
  <Script Type="PowerShell" Name="Auth.ps1" />
</Authentication>
```

<ins>As elements</ins><br>
Add a **Groups** element, then add a \<Group> element for each AD group
```xml
<Authentication Type="Script" AuthID="auth">
  <Script Type="PowerShell" Name="Auth.ps1" />
  <Groups>
    <Group>group 1</Group>
    <Group>group 2</Group>
  </Groups>
</Authentication>
```

#### Any vs All groups
By default, the authentication will be accepted if the user is a member of **any** of the specified groups (or if none are specifed). To change this behaviour to require **all** groups, set the **RequireAllGroups** attribute to TRUE:

```xml
<Authentication Type="Script" AuthID="auth" Groups="group 1, group 2" RequireAllGroups="TRUE" />
```

### Using group membership to change the UI
It is sometimes useful to change the UI based on who is logged in. By setting the **CreateGroupIDs** attribute to TRUE, TsGui will create a variable with a matching ID for each AD group that can be used with the [option linking feature](/documentation/features/OptionLinking.md). The ID created for each group will in the format **%AuthID%_%GroupName%**\*. 

Note that characters that aren't valid for Task Sequence Variable names will be removed. See [here](https://learn.microsoft.com/en-us/intune/configmgr/osd/understand/using-task-sequence-variables#bkmk_custom) for more details on valid task sequence variable names. 

As an example, the following is a simple login page with two additional checkboxes. The checkboxes will be ticked and unticked based on the group memberships matched after authentication.  

```xml
    <Authentication Type="Script" AuthID="auth" RequireAllGroups="FALSE" CreateGroupIDs="TRUE">
		<Script Type="PowerShell" Name="auth.ps1" >
		</Script>
		<Groups>
			<Group>group1</Group>
			<Group>group2</Group>
		</Groups>
	</Authentication>

    <Page>
        <Row>
            <Column>
                <GuiOption Type="UsernameBox" AuthID="auth">
                    <Variable>VAR_Username</Variable>
                </GuiOption>

                <GuiOption Type="PasswordBox" AuthID="auth" />

                <GuiOption Type="ActionButton" IsDefault="TRUE">
                    <Action Type="Authentication" AuthID="auth" />
                    <ButtonText>Login</ButtonText>
                </GuiOption>

                <GuiOption Type="CheckBox">
                    <Variable>group_1</Variable>
                    <Label>group 1</Label>
                    <SetValue>
                        <Query Type="LinkTo">auth_group1</Query>
                    </SetValue>
                </GuiOption>

                <GuiOption Type="CheckBox">
                    <Variable>group_2</Variable>
                    <Label>group 2</Label>
                    <SetValue>
                        <Query Type="LinkTo">auth_group2</Query>
                    </SetValue>
                </GuiOption>

            </Column>
        </Row>
    </Page>
```

---
<br>


### The UsernameBox GuiOption
The UsernameBox provides an input for the user to enter their username. An **AuthID** attribute is required to reference the Authentication block configured above. The value of this GuiOption can be saved to a Task Sequence Variable using the \<Variable\> element. 

```xml
<GuiOption Type="UsernameBox" AuthID="auth" >
    <Variable>VAR_Username</Variable>
</GuiOption>
```
---
<br>


### The PasswordBox GuiOption
The PasswordBox provides an input for the user to enter their password. An **AuthID** attribute is required to reference the Authentication block configured above.

The following additional option elements are available:
* Label - the normal GuiOption label
* FailureMessage - the message displayed to the user if the authentication fails
* NoPasswordMessage - the message displayed to the user if authentication is attempted without a password specified.

```xml
<GuiOption Type="PasswordBox" AuthID="auth">
    <Label>Password:</Label>
    <FailureMessage>Authorization failed</FailureMessage>
    <NoPasswordMessage>Password cannot be empty</NoPasswordMessage>
</GuiOption>
```

If not configured, they will be set the values in the example above. 

If authentication fails, validation will fail on the PasswordBox GuiOption, halting further progress in TsGui i.e. you won't be able to use the Next or Finish buttons. Validation error messages will appear to notify the user (see **FailureMessage** and **NoPasswordMessage** above). 

On successful authentication, the border of the password box will go green.

---
<br>




### The ActionButton GuiOption
The ActionButton provides a 'login' button that the user can click to initiate the authentication process. An **Action** element must be defined, with a **Type="Authentication"** attribute and the **AuthID** attribute configured as it was for the PasswordBox.

The **ButtonText** element configures the text in the button to be displayed to the user.

The **IsDefault** attribute on the GuiOption element will make the button attempt to click when the Enter key is pressed. Note that this behaviour may be overridden if another element is focused that already handles the key press event.  

```xml
<GuiOption Type="ActionButton" IsDefault="TRUE">
    <Action Type="Authentication" AuthID="auth"/>
    <ButtonText>Login</ButtonText>
</GuiOption>
```
---
<br>


## Complete Config Example

```xml
<TsGui LiveData="TRUE">
  <Height>500</Height>
  <Width>400</Width>

  <Heading>
    <Title>Script auth example</Title>
    <Text></Text>
  </Heading>

  <Authentication Type="Script" AuthID="auth" RequireAllGroups="FALSE" CreateGroupIDs="TRUE">
    <Script Type="PowerShell" Name="Auth.ps1" />
    <Groups>
        <Group>CN=SCCM Admins,OU=Groups,OU=LAB,DC=domain,DC=private</Group>
        <Group>CN=Domain Admins,CN=Users,DC=domain,DC=private</Group>
    </Groups>
  </Authentication>

  <Style>
    <LeftCellWidth>100</LeftCellWidth>
    <RightCellWidth>250</RightCellWidth>
  </Style>

  <Page>
    <Row>
      <Column>
        <GuiOption Type="UsernameBox" AuthID="auth" >
          <Variable>VAR_Username</Variable>
        </GuiOption>

        <GuiOption Type="PasswordBox" AuthID="auth" />

        <GuiOption Type="ActionButton" IsDefault="TRUE">
          <Action Type="Authentication" AuthID="auth"/>
          <ButtonText>Login</ButtonText>
        </GuiOption>

        <GuiOption Type="ComputerName" ID="link_name" />
      </Column>
    </Row>
  </Page>
</TsGui>
```

## Example authentication script

```PowerShell
Param (
    [string]$Username,
    [securestring]$Password
)

# The default authentication result. Update this object with the actual authentication and 
# authorization results before returning it to the caller.

$result = [PSCustomObject]@{
    IsAuthenticated = $false
    IsAuthorized = $false
    Message = $null
    GroupMemberships = @{}
}


# Implement your authentication logic here. Update the $result object accordingly. 


return $result

<#ScriptSettings
{
    "LogOutput": false,
    "LogScriptContent": false
}
ScriptSettings#>
```