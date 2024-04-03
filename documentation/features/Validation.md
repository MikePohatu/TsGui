# Validation

* [Overview](#overview)
* [Rules](#rules)
* [Validation Behaviour](#validation-behaviour)
* [MaxLength and MinLength](#maxlength-and-minlength)
* [Validating Empty Values](#validating-empty-values)
* [Tooltips](#tooltips)
* [Multiple Validations](#multiple-validations)
* [Config example](#config-example)
---

## Overview
* [How-to video](https://youtu.be/4GjpDAYp-CU) 
* [Example configuration file](/Config_Examples/Config_Validation.xml).

---
Validation rules allow you to check user input prior to finishing/exiting TsGui, for example confirming valid Windows computer names, or that naming standards or hardware requirements are met.

Each GuiOption can contain one or more Validation blocks. Each Validation block can contain a \<Valid> block, an \<Invalid> block, or both. Each block contains one or more rules to define whether the current value of the GuiOption is valid or not. 

```xml
<GuiOption>
    <Validation ValidateEmpty="FALSE">
        <Valid>
            <Rule></Rule>
            <Rule></Rule>
        </Valid>
        <Invalid>
            <Rule></Rule>
            <Rule></Rule>
        </Invalid>
    </Validation>
</GuiOption>
```
---

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

The value for the rule can also come from a [query](./Queries.md), for example to ensure a GuiOption ends with the serial number of the device:

```xml
<Valid>
    <Rule Type="EndsWith">
        <Query Type="Wmi">
            <Wql>SELECT SerialNumber FROM Win32_BIOS</Wql>
        </Query>
    </Rule>
</Valid>
```

## Validation Behaviour

If an \<Invalid> block is defined but not a \<Valid> block, if any 'invalid' rule matches then the GuiOption will be marked Invalid. All other values will be marked as Valid. 

If a \<Valid> block is defined but not a \<Invalid> block, if any 'valid' rule matches then the GuiOption will be marked Valid. All other values will be marked as Invalid. 

If both are defined, the GuiOption being marked as Invalid if: 
* Any Invalid rules match 
* If there are no matches in either the Invalid or Valid rules


## MaxLength and MinLength
The maximum and minimum lengths of a string can be validated using the MaxLength and MinLength elements. 

Example:
```xml
<GuiOption>
    <Validation>
        <MaxLength>15</MaxLength>
        <MinLength>1</MinLength>
    </Validation>
</GuiOption>
```

## Validating Empty Values
Sometimes you only want to validate an input if one has been set, but an emplty value is allowed. If don't wish to validate empty values, set the ValidateEmpty attribute to FALSE. 

Example:
```xml
<GuiOption>
    <Validation ValidateEmpty="FALSE">
        ...
    </Validation>
</GuiOption>
```

## Tooltips
When validation fails, a tooltip will be shown to the user. The message can be configured using the \<Message> element. 

By default, tooltip messages will be shown immediately after a validation failure. If there are many GuiOptions with validation failures, this can result in the UI becoming full of tooltips. To control this you can change when these tooltips are shown using the **ShowValidationOn** option. Options are: 

* Immediately - open on validation failure [default]
* NextFinish - when the Next or Finish buttons are clicked
* Hover - when the mouse hovers over the GuiOption control e.g. text box

Example:
```xml
<GuiOption>
    <ShowValidationOn>Hover</ShowValidationOn>
</GuiOption>
```

In all cases, the control will show a red border when validation has failed. 


## Multiple Validations
There are scenarios where you may want your validation rules to change, for example you may wish to enforce different naming standards for mobile devices vs desktop ones. 

Multiple Validation blocks can be set and enabled/disabled using the [Groups and Toggles](/documentation/features/GroupsAndToggles.md) feature. 


```xml
<GuiOption>
    <Validation>
        <Group>Group_Mobile</Group>
        ...
    </Validation>
    <Validation>
        <Group>Group_Desktop</Group>
        ...
    </Validation>
</GuiOption>
```

---
## Config example
```xml
<GuiOption>
    <Validation ValidateEmpty="FALSE">
        <!--<Group>Group_1</Group>-->
        <Message>Tooltip message to display on validation error</Message>			
        <MaxLength>15</MaxLength>
        <MinLength>1</MinLength>
        
        <!-- Validation sections with available rule types -->					
        <Valid>
            <Rule Type="StartsWith" CaseSensitive="TRUE">AB</Rule>
            <Rule Type="StartsWith" Not="TRUE" CaseSensitive="TRUE">AB</Rule>
            <Rule Type="EndsWith">AB</Rule>
            <Rule Type="Contains">AB</Rule>
            <Rule Type="Characters">1234567890</Rule>
            <Rule Type="RegEx" CaseSensitive="TRUE">^(WM|WD)[0-9]{6}$</Rule>

            <!-- The value to compare against can also come from a query. See Config_Queries.xml-->
            <Rule Type="StartsWith" CaseSensitive="TRUE">
                <Query Type="Wmi">
                    <Wql>SELECT SerialNumber FROM Win32_BIOS</Wql>
                </Query>
            </Rule>

            <!--<Rule Type="IsNumeric"/>-->
            <!--<Rule Type="GreaterThan">10</Rule>-->
            <!--<Rule Type="GreaterThanOrEqualTo">10</Rule>-->
            <!--<Rule Type="LessThan">10</Rule>-->
            <!--<Rule Type="LessThanOrEqualTo">10</Rule>-->
        </Valid>
        <Invalid>
            <Rule Type="StartsWith" CaseSensitive="TRUE">AB</Rule>
            <Rule Type="EndsWith">YZ</Rule>
            <Rule Type="Contains">YZ</Rule>
            <Rule Type="Characters">`~!@#$%^*()</Rule>
            <Rule Type="RegEx" CaseSensitive="TRUE">^(YZ|XZ)[0-9]{6}$</Rule>
            <Rule Type="Equals" CaseSensitive="TRUE">*NULL</Rule> <!-- *NULL is returned if the WMI query returns a null result -->
            
        </Invalid>
    </Validation>
</GuiOption>
```
