# Compliance Options (Tick/Cross & TrafficLight)

* [Overview](#overview)
* [Compliance Configuration](#compliance-configuration)
  * [States](#states)
  * [Rules](#rules)
  * [Default State](#default-state)
  * [Grouping](#grouping)
* [Refreshing Compliance State](#refreshing-compliance-state)
* [Full GuiOption example config](#full-guioption-example-config)

## Overview
Compliance options check that values meet certain criteria before continuing. This is sometimes also known as 'pre-flight checks'.



To create a compliance option, use the **TickCross** or **TrafficLight** GuiOption type. The two types are identical other than the icons they display. 
```xml
<GuiOption Type="TickCross">
```
![TrafficLightOK](/documentation/images/tickcross-ok.png) ![TrafficLightWarn](/documentation/images/tickcross-warn.png) ![TrafficLightErr](/documentation/images/tickcross-err.png)

```xml
<GuiOption Type="TrafficLight">
```
![TickCrossOK](/documentation/images/trafficlight-ok.png) ![TickCrossWarn](/documentation/images/trafficlight-warn.png) ![TickCrossErr](/documentation/images/trafficlight-err.png)

Full example configuration file is available in the [example file](/Config_Examples/Config_Compliance.xml)

---

## Compliance Configuration
A compliance GuiOption is configured in the normal way to set it's value using the ```<SetValue>``` element. Variable name, label, and style are also set in the normal way. 

To create the compliance evaluation of the current value, you need to add one or ```<Compliance>``` elements to your GuiOption, which will look something like below:

```xml
<Compliance>    
    <Message>Tooltip message to display when compliance is not in OK state</Message>			
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
```
The parts of the \<Compliance> configuration are outlined in more detail below.

### States
A compliance option evaluate the current value against one or more [rules](#rules), will result in it being in one of the following states (ordered from least to most restrictive):

* OK - show an OK icon e.g. green traffic light
* Warning - show a warning icon e.g. yellow traffic light
* Error - show a error icon e.g. red traffic light, but don't block TsGui from finishing
* Invalid - show a error icon e.g. red traffic light, and block TsGui from finishing

When evaluating compliance, the most restrictive state will win (see [Rules](#rules) for more details).

### Rules
The value of the compliance option is set using the standard ```<SetValue>``` element, which will require a [query](/documentation/features/Querys.md) to get the value dynamically. The value returned will be evaluated to see which states match.

Each state is evaluated against one or more rules, which are the same as those used in the [Validation](/documentation/features/Validation.md#rules) feature. If any of the rules match, the state will match. Remember that if multiple states match, the most restrictive state will win.

```xml
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
```

### Default State
If one of the other rules don't match, the compliance evaluation will default to the value set by the ```<DefaultState>``` element. If no DefaultState element is set, it will default to ```<DefaultState>Invalid</DefaultState>```

In the example below, the compliance evaluation will return OK if the value contains '2' or equals '*NULL', otherwise it will return Warning. Error and Invalid have no rules so will never be matched.

```xml
<Compliance>
    <Message>Please connect the power</Message>		
    <DefaultState>Warning</DefaultState>
    <OK>
        <Rule Type="Contains">2</Rule>
        <Rule Type="Equals">*NULL</Rule>
    </OK>
</Compliance>
```

### Grouping
Compliance supports the use of [Groups](/documentation/features/GroupsAndToggles.md) to enable & disable different compliance options. For example you may have a CheckBox setup as a [toggle](/documentation/features/GroupsAndToggles.md#toggles) that sets whether Photoshop is going to be installed. You want the compliance evaluation to change to require more memory for Photoshop usage.

You configure the toggle to set two groups, one for Photoshop enabled, and one disabled:

**Toggle configuration**
```xml
<GuiOption Type="CheckBox">
    <Variable>App_Photoshop</Variable>
    <Label>Photoshop</Label>
    <HAlign>left</HAlign>
    
    <!-- Note the use of multiple toggles to turn different compliance sections on and off -->
    <Toggle Group="Group_Photoshop">
        <Enabled>TRUE</Enabled>
        <Disabled>FALSE</Disabled>
    </Toggle>
    
    <Toggle Group="Group_No_Photoshop">
        <Enabled>FALSE</Enabled>
        <Disabled>TRUE</Disabled>
    </Toggle>
</GuiOption>
```

You can now create two ```Compliance``` blocks on your compliance GuiOption, with the groups assigned accordingly. When the group is disabled, the compliance will also be disabled. 

**Compliance configuration**
```xml
<Compliance>
    <!-- Note the group set here -->
    <Group>Group_No_Photoshop</Group>
    
    <Message>Tooltip message to display on validation error</Message>			
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
    <!-- And the other group set here -->
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
```

## Refreshing Compliance State
Compliance options often need a way for the user to refresh the state, for example if the compliance option checks power is connected or WiFi is disconnected. 

You can do this using a ```ComplianceRefreshButton``` GuiOption. When clicked, all Complicance GuiOptions will re-run all their queries and re-evaluate their state. 

```xml
<GuiOption Type="ComplianceRefreshButton">
    <ButtonText>Refresh</ButtonText>
</GuiOption>
```

---

## Full GuiOption example config

```xml
<!-- Option to check whether the wifi is connected -->
<GuiOption Type="TrafficLight">
    <Variable>Compliance_WifiStatus</Variable>
    <Label>Wifi connection</Label>
    <ShowComplianceValue>FALSE</ShowComplianceValue>

    <!-- SetValue sets the value of the GuiOption, against which compliance is evaluated -->
    <SetValue>
        <Query Type="Wmi">
            <Wql>SELECT * FROM Win32_NetworkAdapter WHERE (AdapterType="Ethernet 802.3") AND (NetConnectionStatus=2)</Wql>
            <Property Name="Name"/>	
            <Separator></Separator>
        </Query>
    </SetValue>

    <!-- Compliance starts here -->
    <Compliance>
        <Message>Please disconnect the wifi</Message>
        <DefaultState>Warning</DefaultState>
        <Invalid>
            <Rule Type="Contains">Wireless</Rule>
            <Rule Type="Contains">Wifi</Rule>
            <Rule Type="Contains">WLAN</Rule>
        </Invalid>
    </Compliance>
</GuiOption>

<!-- Button to reprocess queries and re-evaluate compliance rules. Re-checks compliance options on the page -->
<GuiOption Type="ComplianceRefreshButton">
    <ButtonText>Refresh</ButtonText>
</GuiOption>

```