# FreeText
* [Overview](#overview)
* [Properties](#properties)
  * [CharacterCasing](#charactercasing)
* [Delay](#delay)
* [MaxLength](#maxlength)
  * [ReadOnly](#readonly)
* [Example XML](#example-xml)

## Overview

The FreeText type is a textbox for enter text data. The text can be pre-configured with a value built using the [Query](/documentation/features/Queries.md) functionality. 

## Properties
The following properties are unique to the FreeText GuiOption. These can be set as either an XML attribute or child element. 

### CharacterCasing
Can be used to enforce upper or lower case in your value. Options are Normal, Upper, and Lower

## Delay
The Delay property is the time in milliseconds between the user entering text and the value being updated in the rest of TsGui. This allows for things like [Validation](/documentation/features/Validation.md) to not reprocess for every key press, but instead allow the user to finish typing and then update. Default is 500. 

## MaxLength
The maximum length of string that can be entered into the textbox. The textbox will not allow the user to enter more characters. 

### ReadOnly
When set to true, this will 'grey out' the text box control so the user can't edit the value. This is different from using the [Groups & Toggles](/documentation/features/GroupsAndToggles.md) feature which will disable both the control and the label and also change or remove the value of the GuiOption. **ReadOnly** will only disable the control. 



## Example XML

```xml
<GuiOption Type="FreeText" MaxLength="15" ReadOnly="FALSE">
    <Variable>OSDComputerName_FullQuery</Variable>
    <Label>Computer Name:</Label>
    <HelpText>Enter a computer name for the device</HelpText>
    <CharacterCasing>Normal</CharacterCasing>
    <!-- Sets and enforces the case of text. Options are Normal, Upper, and Lower -->

    <!-- Query for the default value. Return the first valid value. Note
            the Ignore values for evaluating the value returned by the query -->
    <SetValue>
        <Query Type="EnvironmentVariable">
            <Variable Name="OSDComputerName"/>
            <Ignore>MINNT</Ignore>
            <Ignore>MINWIN</Ignore>
        </Query>
        <Query Type="EnvironmentVariable">
            <Variable Name="_SMSTSMachineName"/>
            <Ignore>MINNT</Ignore>
            <Ignore>MINWIN</Ignore>
        </Query>
        <Query Type="EnvironmentVariable">
            <Variable Name="ComputerName"/>
            <Ignore>MINNT</Ignore>
            <Ignore>MINWIN</Ignore>
        </Query>
        <Query Type="Wmi">
            <Wql>SELECT SMBIOSAssetTag FROM Win32_SystemEnclosure</Wql>
            <Ignore>No Asset Tag</Ignore>
            <Ignore>No Asset Information</Ignore>
            <Ignore>NoAssetInformat</Ignore>
        </Query>
        <Query Type="Wmi">
            <Wql>SELECT SerialNumber FROM Win32_BIOS</Wql>
        </Query>
    </SetValue>

    <!-- For validation examples, see Config_Validation.xml and the associated how-to video -->
    <Validation ValidateEmpty="TRUE">
        <MaxLength>15</MaxLength>
        <MinLength>1</MinLength>
        <Invalid>
            <Rule Type="Characters">`~!@#$%^*()+={}[]\\/|,.? :;"'>&amp;&lt;</Rule>
        </Invalid>
    </Validation>
</GuiOption>
```