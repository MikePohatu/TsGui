# ComputerName

## Overview

The ComputerName type is a pre-configured [FreeText](./FreeText.md) GuiOption designed for entering valid Windows computer names. This makes sure there is a value, invalid characters are stripped, maximum 15 characters etc. The variable set is the default **OSDComputerName** task sequence variable. 

The value will be set to the first valid entry from the following:

* Current value of OSDComputerName variable ('MININT' & 'MINWIN' values are ignored)
* Current value of _SMSTSMachineName variable ('MININT' & 'MINWIN' values are ignored)
* Current value of ComputerName variable ('MININT' & 'MINWIN' values are ignored)
* The value of SMBIOSAssetTag from WMI (the Asset Tag value from BIOS, 'No Asset Tag' values ignored)
* The serial number of the device
  
The following is the XML for an equivalent FreeText GuiOption: 

```xml
<GuiOption Type="FreeText" MaxLength="15">
    <Variable>OSDComputerName_FullQuery</Variable>
    <Label>Computer Name:</Label>
    <HelpText>Enter a computer name for the device</HelpText>
    <CharacterCasing>Normal</CharacterCasing> <!-- Sets and enforces the case of text. Options are Normal, Upper, and Lower -->

    <!-- Query for the default value. Return the first valid value. Note
    the Ignore values for evaluating the value returned by the query -->
    <SetValue UseCurrent="False">
        <Query Type="EnvironmentVariable">
            <Variable Name="OSDComputerName"/>
            <Ignore>MININT</Ignore>
            <Ignore>MINWIN</Ignore>						
        </Query>				
        <Query Type="EnvironmentVariable">
            <Variable Name="_SMSTSMachineName"/>
            <Ignore>MININT</Ignore>
            <Ignore>MINWIN</Ignore>					
        </Query>					
        <Query Type="EnvironmentVariable">
            <Variable Name="ComputerName"/>		
            <Ignore>MININT</Ignore>
            <Ignore>MINWIN</Ignore>						
        </Query>					
        <Query Type="Wmi">
            <Wql>SELECT SMBIOSAssetTag FROM Win32_SystemEnclosure</Wql>
        <IncludeNullValues>FALSE</IncludeNullValues>
        <Ignore>No Asset Tag</Ignore>
        <Ignore>NoAssetTag</Ignore>
        <Ignore>No Asset Information</Ignore>
        <Ignore>NoAssetInformat</Ignore>
        </Query>					
        <Query Type="Wmi">
            <Wql>SELECT SerialNumber FROM Win32_BIOS</Wql>
        </Query>
    </SetValue>

   <Validation ValidateEmpty="TRUE">
        <MaxLength>15</MaxLength>
        <MinLength>1</MinLength>
        <Invalid>
            <Rule Type="Characters">`~!@#$%^*()+={}[]\\/|,.? :;"'>&amp;&lt;</Rule>
        </Invalid>
    </Validation>
</GuiOption>	

```