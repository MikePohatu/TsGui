# InfoBox

The Infobox control creates a non interactive text block containing  information either set statically using a ```<Value>```, or dynamically from a [query](/documentation/features/Queries.md). 
				
This GuiOption creates a Task Sequence Variable so the value can be used e.g. display the serial number and also use it later in a script. 
				
Similar to FreeText GuiOptions, a query result will be truncated to MaxLength. 

Characters set in the ```<Disallowed>``` element will be automatically removed.

```xml
<GuiOption Type="InfoBox" MaxLength="20">
    <Disallowed>='-</Disallowed>
    <Variable>INFO_Serial</Variable>
    <Label>Serial: </Label>
    <HelpText>Serial number of the device</HelpText>

    <SetValue>
        <Query Type="Wmi">
            <Wql>SELECT SerialNumber FROM Win32_BIOS</Wql>
            <Ignore>Parallels</Ignore>
            <Ignore>VMWare</Ignore>
        </Query>
        <Value>Virtual machine</Value>
    </SetValue>
</GuiOption>
```