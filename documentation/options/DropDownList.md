# DropDownList

* [Creating selectable options](#creating-selectable-options)
* [Manually creating options](#manually-creating-options)
* [Creating options using queries](#creating-options-using-queries)
  * [Full Example](#full-example)
  * [Full Example XML](#full-example-xml)


The DropDownList GuiOption creates a list in the UI, more specifically a WPF ComboBox. 

How to video on creating DropDownLists from WMI can be found [here](https://youtu.be/YhIiRorF9SA).

## Creating selectable options

You can create options in the list either manually, or using a [query](/documentation/features/Queries.md). Each option in the list is made up of two things:

1. The text displayed to the user
2. The value to be assigned to the output e.g. Task Sequence Variable

## Manually creating options

Options can be created in the following format:

```xml
<GuiOption Type="DropDownList">
    <Option Text="Example1" Value="Example_Value1" />
    <Option Text="Example2" Value="Example_Value2" />
    <Option Text="Example3" Value="Example_Value3" />
    <Option Text="Example4" Value="Example_Value4" />
</GuiOption>
```

## Creating options using queries

To build a list of options using a query, add a [query](/documentation/features/Queries.md) to your GuiOption XML, for example:

```xml
<GuiOption Type="DropDownList">
    <!-- Option list built from Wmi query -->
    <Query Type="Wmi">
        .....
    </Query>
</GuiOption>
```

The main thing to consider when using a query to generate your list of options is that there is the value, and then the text to display to the user. 

If we assume the query will return a list of objects, each object will represent an option in the list. Each object will consist of 1 or more properties. 

In our configuration we will specify the properties we want to use in our UI with a list of \<Property> elements. 

* The first property element will define the **value** of the item in the list
* The remaining properties will be concatenated together to form the text displayed to the user. This will be separated using the text defined in the \<Separator> element
* If there is only one Property in the returned object, the text will be set to the *value* of the list item


### Full Example

The following example creates a list of hard disks for the user to select. Note that the **Index** property is set twice. This is because we want the Index property to be used as the value, but we *also* want the index to be shown to the user. 

A text value shown to the user would be something like:

*ID: 0, 20.35GB, Some hard disk model*

### Full Example XML

```xml
<GuiOption Type="DropDownList">
    <Variable>OSDDiskIndex</Variable>
    <Label>Disk</Label>
    <SetValue>
        <Value>0</Value>
    </SetValue>

    <!-- Option list built from Wmi query -->
    <Query Type="Wmi">
        <Wql>select Index,Caption,Size FROM Win32_DiskDrive where MediaType='Fixed hard disk media'</Wql>
        <NameSpace>root\CIMV2</NameSpace>						<!-- Option to set the namespace -->
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
        <Separator>, </Separator>
    </Query>
</GuiOption>
```