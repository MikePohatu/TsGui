# LDAP Authentication (BETA)

LDAP authentication provides authentication against a domain. 

It should be noted that this feature is new and hasn't been through a security review. Use at your own risk. 

## Overview
Active Directory Authentication requires three things in your TsGui configuration:

1. An **\<Authentication  Type="LDAP"\>** block where the authentication settings i.e. password, is defined. This must have an **AuthID** attribute defined to identify it within the config
2. A **\<GuiOption Type="UsernameBox"\>** to enter the username, which must be configured to reference the AuthID of the Authentication block. The username must be the **UserPrincipalName** of the user.  
3. A **\<GuiOption Type="PasswordBox"\>** to enter the password, which must be configured to reference the AuthID of the Authentication block. Hitting Enter will initiate the authentication process.
4. (Optional) You may also optionally configure a **\<GuiOption Type="ActionButton"\>** to add a 'Login' button to TsGui. This will also initiate the authentication process.  
---
<br>

### The Authentication Block
The Authentication element of your configuration must have a **Type="LDAP"** attribute, a **Domain** attribute with the FQDN of the relevant domain, and an **AuthID** attribute with a value that is unique within the config. This AuthID will be used by GuiOptions to reference and trigger the Authentication process

```xml
<Authentication Type="LDAP" AuthID="ad_auth" Domain="domain.local"/>
```

When an authentication process is initiated, the [Username](#The-UsernameBox-GuiOption) and [Password](#The-PasswordBox-GuiOption) values entered by the user are used to authenticate against the configured domain. 

#### Limiting authentication by group membership

You can accept authentication based on the AD groups the user is a member of the using one of the following formats. In either case you can set a **BaseDN** attribute for the root of the search for the user account and the groups. In most cases, this will be the root of the domain. If you do not set this attribute, the BaseDN will be auto generated using the **Domain** attribute. For example domain.local will create _DC=domain,DC=local_ for the BaseDN.

<ins>As an attribute</ins><br>
You can add a single group by adding the DistinguishedName of the group to the **Group** attribute. Make note of any spaces. Don't add a space before or after the group DN.
```xml
<Authentication Type="LDAP" AuthID="ad_auth" Domain="domain.local" Group="CN=Group Name,OU=Groups,DC=domain,DC=local" BaseDN="DC=domain,DC=local" />
```

<ins>As elements</ins><br>
Add a **Groups** element, then add a \<Group> element for the DistinguishedName of each group. 
```xml
<Authentication Type="LDAP" AuthID="ad_auth" Domain="domain.local" >
  <Groups>
    <Group>CN=Group1,OU=Groups,DC=domain,DC=local</Group>
    <Group>CN=Group2,OU=Groups,DC=domain,DC=local</Group>
  </Group>
</Authentication>
```

#### Any vs All groups
By default, the authentication will be accepted if the user is a member of **any** of the specified groups (or if none are specifed). To change this behaviour to require **all** groups, set the **RequireAllGroups** attribute to TRUE:

```xml
<Authentication Type="LDAP" AuthID="ad_auth" Domain="domain.local" BaseDN="DC=domain,DC=local" RequireAllGroups="TRUE" >
  <Groups>
    <Group>CN=Group1,OU=Groups,DC=domain,DC=local</Group>
    <Group>CN=Group2,OU=Groups,DC=domain,DC=local</Group>
  </Group>
</Authentication>
```

#### Using group membership to change the UI
It is sometimes useful to change the UI based on who is logged in. By setting the **CreateGroupIDs** attribute to TRUE, TsGui will create a variable with a matching ID for each AD group that can be used with the [option linking feature](/documentation/features/OptionLinking.md). The ID created for each group will in the format **%AuthID%_%GroupDN%**\*, with the = sign, comma, and space characters replaced by underscores, and characters that aren't valid for Task Sequence Variable names removed. See [here](https://learn.microsoft.com/en-us/intune/configmgr/osd/understand/using-task-sequence-variables#bkmk_custom) for more details on valid task sequence variable names. 

For example, for the group configured below: 

```xml
<Authentication Type="LDAP" AuthID="ad_auth" Domain="domain.local" Group="CN=Group Name,OU=Groups,DC=domain,DC=local" />
```

The ID would be **ad_auth_CN_Group_Name_OU_Groups_DC_domain_DC_local**


As an example, the following is a simple login page with two additional checkboxes. The checkboxes will be ticked and unticked based on the group memberships matched after authentication.  

```xml
    <Authentication Type="LDAP" AuthID="ad_auth" Domain="domain.local">
        <Groups>
            <Group>CN=Group1,OU=Groups,DC=domain,DC=local</Group>
            <Group>CN=Group2,OU=Groups,DC=domain,DC=local</Group>
        </Group>
    </Authentication>

    <Page>
        <Row>
            <Column>
                <GuiOption Type="UsernameBox" AuthID="ad_auth">
                    <Variable>VAR_Username</Variable>
                </GuiOption>

                <GuiOption Type="PasswordBox" AuthID="ad_auth" />

                <GuiOption Type="ActionButton" IsDefault="TRUE">
                    <Action Type="Authentication" AuthID="ad_auth" />
                    <ButtonText>Login</ButtonText>
                </GuiOption>

                <GuiOption Type="CheckBox">
                    <Variable>group_1</Variable>
                    <Label>group 1</Label>
                    <SetValue>
                        <Query Type="LinkTo">ad_auth_Group1_Groups_domain_local</Query>
                    </SetValue>
                </GuiOption>

                <GuiOption Type="CheckBox">
                    <Variable>group_2</Variable>
                    <Label>group 2</Label>
                    <SetValue>
                        <Query Type="LinkTo">ad_auth_Group2_Groups_domain_local</Query>
                    </SetValue>
                </GuiOption>

            </Column>
        </Row>
    </Page>
```

---
<br>


### The UsernameBox GuiOption
The UsernameBox provides an input for the user to enter their AD username. An **AuthID** attribute is required to reference the Authentication block configured above. The value of this GuiOption can be saved to a Task Sequence Variable using the \<Variable\> element. 

The username must be the **UserPrincipalName** of the user. It is strongly recommended to add a [PlaceHolder](/documentation/options/FreeText.md#placeholder) attribute to indicate to the user the format required. 

```xml
<GuiOption Type="UsernameBox" AuthID="ad_auth" PlaceHolder="username@domain.local" >
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
<GuiOption Type="PasswordBox" AuthID="conf_auth">
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
    <Action Type="Authentication" AuthID="ad_auth"/>
    <ButtonText>Login</ButtonText>
</GuiOption>
```
---
<br>

### Login Example
The following example shows the configuration items above in position in a TsGui config file. Note the **ad_auth** value across the configuration pieces. 

```xml
<TsGui LiveData="TRUE">
d	<Authentication Type="LDAP" AuthID="ad_auth" Domain="domain.local"/>
    
    <Page>
        <Row>
            <Column>
                <!-- Username and password GuiOptions -->
                <GuiOption Type="UsernameBox" AuthID="ad_auth" />
                <GuiOption Type="PasswordBox" AuthID="ad_auth" />

                <GuiOption Type="ActionButton" IsDefault="TRUE">
                    <Action Type="Authentication" AuthID="ad_auth"/>
                </GuiOption>
            </Column>
        </Row>
    </Page>
</TsGui>
```
---
<br>

