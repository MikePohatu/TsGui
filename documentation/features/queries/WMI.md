# WMI Queries

* [Overview](#overview)
* [Configuration](#configuration)
  * [Wql](#wql)
  * [NameSpace](#namespace)
  * [Separator](#separator)
* [Full Config Example](#full-config-example)

## Overview
Windows Management Instrumentation provides a way to query many different parts of Windows. It uses a SQL like language to query various parts of Windows.


Example config file is available in [Config_Queries.xml](/Config_Examples/Config_Queries.xml).

How-to video: [https://youtu.be/YhIiRorF9SA](https://youtu.be/YhIiRorF9SA)

## Configuration

### Wql
The ```<Wql>``` element represents the WQL query to run to get the data you need. **wbemtest** is a good tool for testing your WQL query to make sure you are getting back the data you require.

### NameSpace
The ```<NameSpace>``` element sets the namespace to run the query against. If this element is not present, ```root\WIMV2``` is assumed.

### Separator
The ```<Separator>``` element sets the text to insert between property values when creating the text to put into your control. If this is not set, ```, ``` (note the is assumed.

## Full Config Example

```xml
<!--This option presents a DropDownList of available fixed harddisks for the partition and format step in SCCM task sequences-->
<GuiOption Type="DropDownList">
    <Variable>OSDDiskIndex</Variable>
    <Label>Disk</Label>
    <SetValue>
        <Value>0</Value>
    </SetValue>
    
    <!-- Option list built from Wmi query -->
    <Query Type="Wmi">
        <Wql>select Index,Caption,Size FROM Win32_DiskDrive where MediaType='Fixed hard disk media'</Wql>
        <NameSpace>root\CIMV2</NameSpace> <!-- Option to set the namespace -->
        <Separator>, </Separator>
        <Property Name="Index"/>
        <Property Name="Index">
            <Prefix>ID: </Prefix>
        </Property>
        <Property Name="Size">
            <Calculate DecimalPlaces="2">VALUE/1073741824</Calculate>
            <Prefix></Prefix>
            <Append>GB</Append>
        </Property>
        <Property Name="Caption"/>
    </Query>
</GuiOption>
```