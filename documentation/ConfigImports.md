# Configuration Imports
Configuration imports allow you to break your TsGui configuration into multiple files. This is useful to help create smaller configuration files which are easier to read, or to create modular configuration files that be used in multiple TsGui setups.

* [The \<Import> element](#the-import-element)
* [Config Parts](#config-parts)
* [Resulting Configuration](#resulting-configuration)


## The \<Import> element
The ```<Import>``` element defines a location in the config where you want to insert the content of the external config file. The ```Path``` attribute defines the path to the external config file. TsGui will detect if a local or web address has been used (http://, https://) and load it accordingly. 

```xml
<TsGui LiveData="TRUE">
    <Height>460</Height>
	<Width>500</Width>
	
	<Import Path="http://tsgui/PART_Auth.xml" />

	<Page>
		<Row>
		<Column>
			<LabelWidth>170</LabelWidth>
			<ControlWidth>250</ControlWidth>
			
			<GuiOption Type="ComputerName" />

		</Column>
		</Row>		
	</Page>
	
</TsGui>
```

---
## Config Parts
A config 'part' is the external file. The file must have be a valid XML file, i.e. it must contain a single root element. The standard is to use TsGuiPart as the root element, but anything will work. 

Any XML child elements under the root will be inserted into the parent configuration, replacing the ```<Import>``` element. 

```xml
<TsGuiPart>
    <Authentication Type="ActiveDirectory" AuthID="ad_auth" Domain="home.local"/>
    <Page>
        <Row>
            <Column>
                <LabelWidth>170</LabelWidth>
                <ControlWidth>250</ControlWidth>

                    <GuiOption Type="UsernameBox" AuthID="ad_auth" ID="link_pw">
                        <Variable>VAR_Username</Variable>
                    </GuiOption>
                    
                    <GuiOption Type="PasswordBox" AuthID="ad_auth"  ID="link_user">
                        <Variable>VAR_Password</Variable>
                    </GuiOption>
                    
                    <GuiOption Type="ActionButton" ID="link_actionbtn">
                        <Variable>VAR_ActionButton</Variable>
                        <Action Type="ADAuthentication" AuthID="ad_auth">
                            <Domain>domain.local</Domain>
                        </Action>
                    </GuiOption>
            </Column>
        </Row>
    </Page>
</TsGuiPart>
```

---
## Resulting Configuration
Using the two examples above, the resulting configuration that will be loaded by TsGui will be the following:


```xml
<TsGui LiveData="TRUE">
    <Height>460</Height>
	<Width>500</Width>
	
    <!-- <Import Path="http://tsgui/PART_Auth.xml" /> -->
	<Authentication Type="ActiveDirectory" AuthID="ad_auth" Domain="home.local"/>
    <Page>
        <Row>
            <Column>
                <LabelWidth>170</LabelWidth>
                <ControlWidth>250</ControlWidth>

                    <GuiOption Type="UsernameBox" AuthID="ad_auth" ID="link_pw">
                        <Variable>VAR_Username</Variable>
                    </GuiOption>
                    
                    <GuiOption Type="PasswordBox" AuthID="ad_auth"  ID="link_user">
                        <Variable>VAR_Password</Variable>
                    </GuiOption>
                    
                    <GuiOption Type="ActionButton" ID="link_actionbtn">
                        <Variable>VAR_ActionButton</Variable>
                        <Action Type="ADAuthentication" AuthID="ad_auth">
                            <Domain>domain.local</Domain>
                        </Action>
                    </GuiOption>
            </Column>
        </Row>
    </Page>
    <!-- <Import Path="http://tsgui/PART_Auth.xml" /> -->
    
	<Page>
		<Row>
		<Column>
			<LabelWidth>170</LabelWidth>
			<ControlWidth>250</ControlWidth>
			
			<GuiOption Type="ComputerName" />

		</Column>
		</Row>		
	</Page>
	
</TsGui>
```