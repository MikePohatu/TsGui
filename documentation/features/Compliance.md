# Compliance (pre-flight checks)

* [Overview](#overview)
* [Compliance Behaviour](#compliance-behaviour)
* [Rules](#rules)
* [Multiple Compliance blocks](#multiple-compliance-blocks)
* [Refreshing Compliance](#refreshing-compliance)
* [Config example](#config-example)


## Overview

* [How-to video](https://youtu.be/RiOLjTdrd1U)
* [Example configuration file](//Config_Examples/Config_Compliance.xml).

---
Compliances rules function very similar to [Validation](./Validation.md), but instead of checking user input, are designed to check system state e.g. hardware requirements. Compliance works with the **TickCross** and **TrafficLight** GuiOptions. 

Each compliance GuiOption can contain one or more Compliance blocks. Each Compliance block can contain blocks defining the following states:

* OK - all OK, green icon
* Warning - warning message, orange icon
* Error - error state, red icon. TsGui will allow user to continue if Next/Finish is clicked
* Invalid - error state, red icon. TsGui will not allow user to continue if Next/Finish is clicked

Each block contains one or more rules to define that state. 

```xml
<GuiOption Type="TrafficLight">
    <SetValue>
        ...
    </SetValue>
    <Compliance>
        <OK>
            <Rule></Rule>
            <Rule></Rule>
        </OK>
        <Warning>
            <Rule></Rule>
            <Rule></Rule>
        </Warning>
        <Error>
            <Rule></Rule>
            <Rule></Rule>
        </Error>
        <Invalid>
            <Rule></Rule>
            <Rule></Rule>
        </Invalid>
    </Compliance>
</GuiOption>
```
---

## Compliance Behaviour
When compliance is evaluated, the worst matching state wins:

Invalid > Error > Warning > OK. 

If there are no matches the default state is set. You can set the default state using the \<DefaultState> element. The default value for DefaultState is **Invalid**. 

```xml
<Compliance>
    <DefaultState>Warning</DefaultState>
</Compliance>
```

When the compliance state is anything other than OK, the tooltip of the GuiOption is set using the \<Message> element, overriding the message set in the \<HelpText>.

## Rules
\<Rule> blocks define matches of the current GuiOption value. Note that string matching rules e.g. Contains or EndsWith are not case sensitive by default. To make them case snsitive, set the CaseSensitive attribute to TRUE. 

Valid rule types are listed below:

```xml
<Rule Type="IsNumeric"/>
<Rule Type="GreaterThan">10</Rule>
<Rule Type="GreaterThanOrEqualTo">10</Rule>
<Rule Type="LessThan">10</Rule>
<Rule Type="LessThanOrEqualTo">10</Rule>
<Rule Type="StartsWith" CaseSensitive="TRUE">AB</Rule>
<Rule Type="EndsWith" CaseSensitive="FALSE">YZ</Rule>
<Rule Type="Contains">YZ</Rule>
<Rule Type="Characters">`~!@#$%^*()</Rule>
<Rule Type="RegEx">^(YZ|XZ)[0-9]{6}$</Rule>
<Rule Type="Equals" CaseSensitive="TRUE">*NULL</Rule>
```

## Multiple Compliance blocks
There are scenarios where you may want your compliance rules to change, for example hardware requirements might change based on the applications selected for install. 

Multiple Compliance blocks can be set and enabled/disabled using the [Groups and Toggles](/documentation/features/GroupsAndToggles.md) feature. 


```xml
<GuiOption Type=TrafficLight>
    <SetValue>
        <Query Type="Wmi">
            <Wql>SELECT TotalPhysicalMemory from Win32_ComputerSystem</Wql>
            <Property Name="TotalPhysicalMemory">
                <Calculate DecimalPlaces="0">VALUE/1073741824</Calculate>
            </Property>	
            <Separator></Separator>
        </Query>
    </SetValue>
    <Compliance>
        <Message>You may not have enough memory to run Photoshop<Message>
        <Group>Group_Photoshop</Group>
        ...
    </Compliance>
    <Compliance>
        <Group>Group_NoPhotoshop</Group>
        ...
    </Compliance>
</GuiOption>
```

## Refreshing Compliance

To allow a user to refresh the compliance state after they have changed or fixed a compliance issue e.g. connecting a power adapter, you can add a **ComplianceRefreshButton** GuiOption. 

This will refresh all compliance GuiOptions (TickCross or TrafficLight) on a page.


```xml
<GuiOption Type="ComplianceRefreshButton">
    <ButtonText>Refresh</ButtonText>
    <Style>
        <LabelOnRight>TRUE</LabelOnRight>
    </Style>
    <HAlign>left</HAlign>
</GuiOption>
```

---
## Config example
```xml
<GuiOption Type="TickCross">
    <Variable>Compliance_InstalledMemory</Variable>
    <Label>System Memory</Label>
    <SetValue>
        <Query Type="Wmi">
            <Wql>SELECT TotalPhysicalMemory from Win32_ComputerSystem</Wql>
            <Property Name="TotalPhysicalMemory">
                <Calculate DecimalPlaces="0">VALUE/1073741824</Calculate>
            </Property>	
            <Separator></Separator>
        </Query>
    </SetValue>
    
    <!-- The compliance sections. The rules are the same as for validation. For all availble rule
    types, see Config_Validation.xml example file -->
    <Compliance>
        <Group>Group_No_Photoshop</Group>
        
        <Message>Tooltip message to display on compliance error</Message>			
        <OK>
            <Rule Type="GreaterThanOrEqualTo">8</Rule>
        </OK>
        <Warning>
            <Rule Type="LessThan">8</Rule>
        </Warning>
        <Error>
            <Rule Type="LessThan">4</Rule>
        </Error>
        <Invalid>
            <Rule Type="LessThan">2</Rule>
        </Invalid>
    </Compliance>
    
    <Compliance>
        <Group>Group_Photoshop</Group>
        
        <Message>You may not have enough memory to run Photoshop</Message>
        <OK>
            <Rule Type="GreaterThanOrEqualTo">16</Rule>
        </OK>
        <Warning>
            <Rule Type="LessThan">16</Rule>
        </Warning>
        <Error>
            <Rule Type="LessThan">8</Rule>
        </Error>
        <Invalid>
            <Rule Type="LessThan">4</Rule>
        </Invalid>
    </Compliance>
</GuiOption>

<GuiOption Type="ComplianceRefreshButton">
    <ButtonText>Refresh</ButtonText>
    <Style>
        <LabelOnRight>TRUE</LabelOnRight>
    </Style>
    <HAlign>left</HAlign>
</GuiOption>
```