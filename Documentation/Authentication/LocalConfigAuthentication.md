# Local Config Authentication

Local config authentication provides a basic 'enter password to continue' type functionality. 

**Important** - Local Config Authentication should **NOT** be considered a strong security measure. A programmer with the config file and the source code can easily reverse the password hash and view the password. The password is obscured, not encrypted. 

Note also that the configuration below does not create a Task Sequence variable. It is only to restrict further access to areas of TsGui

* [Overview](#Overview)
* [The Authentication Block](#The-Authentication-Block)
* [The PasswordBox GuiOption](#The-PasswordBox-GuiOption)
* [The ActionButton GuiOption](#The-ActionButton-GuiOption)
* [Config Example](#Config-Example)
<br>
<br>

## Overview
Local Config Authentication requires two things in your TsGui configuration:

1. An **\<Authentication  Type="Password"\>** block where the authentication settings i.e. password, is defined. This must have an **AuthID** attribute defined to identify it within the config
2. A **\<GuiOption Type="PasswordBox"\>** to enter the password, which must be configured to reference the AuthID of the Authentication block. Hitting Enter will initiate the authentication process.

3. (Optional) You may also optionally configure a **\<GuiOption Type="ActionButton"\>** to add a 'Login' button to TsGui. This will also initiate the authentication process.  
<br>


### The Authentication Block
The Authentication element of your configuration must have a **Type** attribute with a value of **Password**, and an **AuthID** attribute with a value that is unique within the config. This AuthID will be used by GuiOptions to reference and trigger the Authentication process

With the Authentication element you must have one or more **Password** elements. These contain **PasswordHash** and **Key** elements which are the obscure password. When you initiate an authentication, any matching password will be considered a success. 

To generate these values, the TsGui executable has a **-hash** parameter which will build this for you. Use the following command line to create the Authentication block, where **password1** is the new password you wish to use:

```
tsgui -hash password1
```

A window will pop up with the Authentication block filled in that you can copy and paste into your config. This is applied at the top level i.e. just below the \<TsGui\> element.

```
    <Authentication Type="Password" AuthID="conf_auth">
		<Password>
			<PasswordHash>4EMa/tlplQTGBUKWHYUIcw2BvvbI+sTF4SHmaMX8tCY=</PasswordHash>
			<Key>xqIfUNJlD456ATunONj9tplQwas6drR+m/1C9ro3CHUmCgqOa8BvpLQ8jya25zXB</Key>
		</Password>
	</Authentication>
```
<br>
<br>


### The PasswordBox GuiOption
The PasswordBox provides an input for the user. An **AuthID** attribute is required to reference the Authentication block configured above.

```
<GuiOption Type="PasswordBox" AuthID="conf_auth">
    <Label>Password:</Label>
</GuiOption>
```

Note that the Label element is optional. If not configured, the label will be set to "Password:"

If authentication fails, validation will fail on the PasswordBox GuiOption, halting further progress in TsGui i.e. you won't be able to use the Next or Finish buttons. Normal validation warning messages will appear to notify the user. 
<br>
<br>




### The ActionButton GuiOption
The ActionButton provides a 'login' button that the user can click to initiate the authentication process. An **Action** element must be defined, with a **Type="Authentication"** attribute and the **AuthID** attribute configured as it was for the PasswordBox.

The **ButtonText** element configures the text in the button to be displayed to the user.

The **IsDefault** attribute on the GuiOption element will make the button attempt to click when the Enter key is pressed. Note that this behaviour may be overridden if another element is focused that already handles the key press event.  

```
<GuiOption Type="ActionButton" IsDefault="TRUE">
    <Action Type="Authentication" AuthID="conf_auth"/>
    <ButtonText>Login</ButtonText>
</GuiOption>
```
<br>
<br>


#### Config Example
The following example shows the configuration items above in position in a TsGui config file.

```
<TsGui LiveData="TRUE">
	<Authentication Type="Password" AuthID="conf_auth">
		<Password>
			<PasswordHash>4EMa/tlplQTGBUKWHYUIcw2BvvbI+sTF4SHmaMX8tCY=</PasswordHash>
			<Key>xqIfUNJlD456ATunONj9tplQwas6drR+m/1C9ro3CHUmCgqOa8BvpLQ8jya25zXB</Key>
		</Password>
	</Authentication>

	<Page>
		<Row>
			<Column>
				<GuiOption Type="PasswordBox" AuthID="conf_auth">
                    <Label>Password:</Label>
                </GuiOption>

				<GuiOption Type="ActionButton" IsDefault="TRUE">
					<Action Type="Authentication" AuthID="conf_auth"/>
					<ButtonText>Login</ButtonText>
				</GuiOption>
			</Column>
		</Row>
	</Page>
</TsGui>
```