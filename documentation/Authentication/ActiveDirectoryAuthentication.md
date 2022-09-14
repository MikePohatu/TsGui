# Active Directory Authentication (BETA)

Active Directory authentication provides authentication against a domain for use with AD queries. 

It should be noted that there are limitations to what is possible in WinPE due to a only a limited subset of the .Net Framework being avaiable. For this reason Active Directory Authentication is considered as a beta feature and requires WinPE customisation that will not be supported by Microsoft. 

* [Prerequisites](#Prerequisites)
* [Overview](#Overview)
* [The Authentication Block](#The-Authentication-Block)
* [The PasswordBox GuiOption](#The-PasswordBox-GuiOption)
* [The ActionButton GuiOption](#The-ActionButton-GuiOption)
* [Login Example](#Login-Example)
* [Active Directory Queries](#Active-Directory-Queries)
* [ADGroupMembers Query](#ADGroupMembers-Query)
* [ADOU Query](#ADOU-Query)
* [Query Usage examples](#Query-Usage-examples)
* [Complete Config Example](#Complete-Config-Example)
---
<br>

## Prerequisites
This feature makes use of the ADSI components of .Net which is not included in WinPE. To make this feature work in WinPE ADSI must be added to WinPE. This is not supported by Microsoft. See for an example of how to do this: https://deploymentresearch.com/adsi-plugin-for-winpe-5-0/ 

---
<br>

## Overview
Active Directory Authentication requires three things in your TsGui configuration:

1. An **\<Authentication  Type="ActiveDirectory"\>** block where the authentication settings i.e. password, is defined. This must have an **AuthID** attribute defined to identify it within the config
2. A **\<GuiOption Type="UsernameBox"\>** to enter the username, which must be configured to reference the AuthID of the Authentication block. 
3. A **\<GuiOption Type="PasswordBox"\>** to enter the password, which must be configured to reference the AuthID of the Authentication block. Hitting Enter will initiate the authentication process.
4. (Optional) You may also optionally configure a **\<GuiOption Type="ActionButton"\>** to add a 'Login' button to TsGui. This will also initiate the authentication process.  
---
<br>

### The Authentication Block
The Authentication element of your configuration must have a **Type="ActiveDirectory"** attribute, a **Domain** attribute with the FQDN of the relevant domain, and an **AuthID** attribute with a value that is unique within the config. This AuthID will be used by GuiOptions to reference and trigger the Authentication process

```
<Authentication Type="ActiveDirectory" AuthID="ad_auth" Domain="domain.local"/>
```

When an authentication process is initiated, the [Username](#The-UsernameBox-GuiOption) and [Password](#The-PasswordBox-GuiOption) values entered by the user are used to authenticate against the configured domain. 

---
<br>


### The UsernameBox GuiOption
The UsernameBox provides an input for the user to enter their AD username. An **AuthID** attribute is required to reference the Authentication block configured above. The value of this GuiOption can be saved to a Task Sequence Variable using the \<Variable\> element. 

```
<GuiOption Type="UsernameBox" AuthID="ad_auth" >
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

```
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

```
<GuiOption Type="ActionButton" IsDefault="TRUE">
    <Action Type="Authentication" AuthID="ad_auth"/>
    <ButtonText>Login</ButtonText>
</GuiOption>
```
---
<br>

### Login Example
The following example shows the configuration items above in position in a TsGui config file. Note the **ad_auth** value across the configuration pieces. 

```
<TsGui LiveData="TRUE">
d	<Authentication Type="ActiveDirectory" AuthID="ad_auth" Domain="domain.local"/>
	
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

## Active Directory Queries
In order to query Active Directory, you must first authenticate. Once the user has been successfully authenticated using the options above, you can then use that session to run queries against AD. TsGui currently supports the following Active Directory queries:

* ADOU - Queries an OU structure. This is ideal for use with a TreeView GuiOption
* ADGroupMembers - Queries the members of an AD group. This is ideal for use with a DropDownList GuiOption

All Active Directory queries require the **AuthID** attribute. This will run the query using the autheticated session created above.

---
<br>


### ADGroupMembers Query
* GroupName - the SamAccountName of the group to be queried
* \<Property\> - two or more Property values must be set. The first Property will become the **Value** of the Task Sequence variable. The remaining Property elements will concatenated together using the value of the \<Separator\> element, and displayed in the UI. 

Note that only the following properties are available using this query:

* DisplayName
* DistinguishedName
* Enabled
* Name 
* SamAccountName
* Sid
* UserPrincipalName


#### ADGroupMembers query example
```
<Query Type="ADGroupMembers" AuthID="ad_auth">
    <GroupName>Domain Users</GroupName>
    <Property Name="DistinguishedName"/>
    <Property Name="Name"/>
    <Property Name="Enabled"/>			
    <Separator>, </Separator>				
</Query>
```
Using the query above, **Name, Enabled** of each group member will be displayed in the GUI e.g. *Mike Pohatu, True*, and the **DistinguishedName** property will be set on the Task Sequence Variable.

---
<br>

### ADOU Query
* \<BaseOU\> - configures the root of the tree to be returned by the query
* \<Property\> - two or more Property elements must be set. The first Property will become the **Value** of the Task Sequence variable. The remaining Property elements will concatenated together using the value of the \<Separator\> element, and displayed in the UI.  

Note that only certain properties are available using this query:

* distinguishedName
* Name 
* OU (this usually returns the same as Name)


#### ADOU query example
```
<Query Type="ADOU" AuthID="ad_auth">
    <BaseOU>OU=SubOU,DC=domain,DC=local</BaseOU>
    <Property Name="distinguishedName"/>
    <Property Name="Name"/>	
</Query>
```
Using the query above, the **Name** property of each OU will be displayed in the GUI, and the **distinguishedName** property will be set on the Task Sequence Variable.

---
<br>


### Query Usage examples
Note the **ad_auth** value across the configuration pieces. 

```
<GuiOption Type="DropDownList">
    <Variable>AD_User</Variable>
    <Label>AD User:</Label>

    <Query Type="ADGroupMembers" AuthID="ad_auth">
        <GroupName>Domain Users</GroupName>
        <Property Name="DistinguishedName"/>
        <Property Name="DisplayName"/>				
    </Query>
</GuiOption>
```

```
<GuiOption Type="TreeView">
    <Variable>AD_OU</Variable>
    <Label>AD OU:</Label>

    <Query Type="ADOU" AuthID="ad_auth">
        <BaseOU>OU=SubOU,DC=domain,DC=local</BaseOU>
        <Property Name="distinguishedName"/>
        <Property Name="Name"/>				
    </Query>
</GuiOption>
```

---
<br>

## Complete Config Example

```
<TsGui LiveData="TRUE">
  <Height>500</Height>
  <Width>400</Width>

  <Heading>
    <Title>Active Directory example</Title>
    <Text></Text>
  </Heading>

  <Authentication Type="ActiveDirectory" AuthID="ad_auth" Domain="domain.local"/>

  <Style>
    <LeftCellWidth>100</LeftCellWidth>
    <RightCellWidth>250</RightCellWidth>
  </Style>

  <Page>
    <Row>
      <Column>
        <GuiOption Type="UsernameBox" AuthID="ad_auth" >
          <Variable>VAR_Username</Variable>
        </GuiOption>

        <GuiOption Type="PasswordBox" AuthID="ad_auth" />

        <GuiOption Type="ActionButton" IsDefault="TRUE">
          <Action Type="Authentication" AuthID="ad_auth"/>
          <ButtonText>Login</ButtonText>
        </GuiOption>

        <GuiOption Type="ComputerName" ID="link_name" />

        <GuiOption Type="DropDownList">
          <Variable>AD_User</Variable>
          <Label>AD User:</Label>

          <Query Type="ADGroupMembers" AuthID="ad_auth">
            <GroupName>Domain Users</GroupName>
            <Property Name="DistinguishedName"/>
            <Property Name="Name"/>
          </Query>
        </GuiOption>

        <GuiOption Type="TreeView">
          <Variable>AD_OU</Variable>
          <Label>AD OU:</Label>

          <Query Type="ADOU" AuthID="ad_auth">
            <BaseOU>OU=LAB,DC=home,DC=local</BaseOU>
            <Property Name="distinguishedName"/>
            <Property Name="Name"/>
          </Query>

          <Style>
            <Control>
              <Height>100</Height>
            </Control>
          </Style>
        </GuiOption>
      </Column>
    </Row>
  </Page>
</TsGui>
```