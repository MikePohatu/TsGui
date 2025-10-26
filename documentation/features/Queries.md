# Queries

Queries can be used in a number of places within TsGui to get system state. Primarily they are used in the \<SetValue> element to assign the value to a GuiOption. They are also used in the [Compliance](Compliance.md) and [Validation](Validation.md) features, as well as being options for setting property values for [scripts](Scripts.md). 

A how-to video can be viewed [here](https://youtu.be/YhIiRorF9SA?si=f5Jc5r65HsFzAHbV).

* [Query Types](#query-types)
* [Query Results](#query-results)
  * [String Results](#string-results)
  * [Collection Results](#collection-results)
    * [Use a query to select an item in a list](#use-a-query-to-select-an-item-in-a-list)
    * [Use a query to generate items in a list](#use-a-query-to-generate-items-in-a-list)
  * [Selecting Properties \& Property Order](#selecting-properties--property-order)
  * [Separator](#separator)
  * [Transforming Properties](#transforming-properties)
    * [\<Append\>](#append)
    * [\<Calculate\>](#calculate)
    * [\<Prefix\>](#prefix)
    * [\<Replace\>](#replace)
    * [\<RegexReplace\>](#regexreplace)
    * [\<ToLower\>](#tolower)
    * [\<ToUpper\>](#toupper)
    * [\<Truncate\>](#truncate)
* [Ignoring Values](#ignoring-values)
* [Multiple Queries \& Null/Empty values](#multiple-queries--nullempty-values)
* [Queries with Null or Empty Property Values](#queries-with-null-or-empty-property-values)
* [Reprocessing queries](#reprocessing-queries)


## Query Types
Each \<Query> element requires a "Type" attribute to define which type of query you want to complete:

* [Combined](queries/Combined.md)
* [EnvironmentVariable](queries/EnvironmentVariable.md)
* [If/Else](queries/IfElse.md)
* [PowerShell](Scripts.md#in-a-query)
* [Registry](queries/Registry.md)
* [Value-Only](queries/Value.md)
* [WMI](queries/WMI.md)
  
## Query Results
A query will either return a single string, or one or more objects. For example a query for environmental variable will return a single string. 

However a WMI query such as this will return a list of objects:
```sql
select Index,Caption,Size,Description FROM Win32_DiskDrive where MediaType='Fixed hard disk media'
```

In this case, each returned object will have 4 properties, Index, Caption, Size, and Description. 

GuiOptions that use the result of the queries either expect a single string, or a collection of objects. The sections below cover how the query results are handled. 

### String Results
GuiOptions that require a single string are FreeText, ComputerName, InfoBox, Heading, and CheckBox.

When the result of the query is single string already, this is obviously set as the value.

When the result of the query is an object, the property values of the object are concatenated together to make a single string.

If the result is a list of objects, the property values of *all* the objects are concatenated together. Usually this wouldn't be desireable. When using queries like WMI, it's recommended to return only a single object from your WQL query if outputting to a single string. 

### Collection Results
In opposition to the GuiOptions above, the DropDownList and TreeView options need to contain multiple items to select from. In this instance, you can use a query to either generate the list of items, or to select the default value for the existing list. 

#### Use a query to select an item in a list

When the \<Query> element exists as a child of the \<SetValue> element, the query will be used to select an item from the list. As with string results above, a query returning an object will have properties concatenated to get a final result. 

This will then select the item with the matching *value* (not the text). In the example below, if the WMI query  returns HP, Microsoft, or Lenovo, it will select the appropriate option. 

```xml
<GuiOption Type="DropDownList">
    <Option Value="HP" Text="HP" />
    <Option Value="Microsoft" Text="MS" />
    <Option Value="Lenovo" Text="Lenovo" />
    <SetValue>
        <Query Type="Wmi">
            <Wql>SELECT Manufacturer FROM Win32_OperatingSystem</Wql>
        </Query>
    </SetValue>
</GuiOption>
```

#### Use a query to generate items in a list

When the \<Query> element exists as a child of the \<GuiOption> element, the query will generate a list of items. 

Each returned object is added as a selectable item in the list. The values of each item are set as outlined below in [Selecting Properties & Property Order](#selecting-properties--property-order).

### Selecting Properties & Property Order
When a query is returning objects, you will usually want to control which properties you display, and in what order. Lets use our WMI query from above:

```sql
select Index,Caption,Size,Description FROM Win32_DiskDrive where MediaType='Fixed hard disk media'
```

In this instance each returned object will have 4 properties, but we may only want Index and Caption. 

To do this we add \<Property> elements in our query. Use the *Name* attribute to select the property:

```xml
<Query Type="Wmi">
    <Wql>select Index,Caption,Size,Description FROM Win32_DiskDrive where MediaType='Fixed hard disk media'</Wql>
    <Property Name="Index" />
    <Property Name="Index" />
    <Property Name="Caption" />
</Query>
```

In the example above the Index property is set twice. This isn't a typo. The first \<Property> element is the **key** property. For collection types the key property is the one that is set as the value. The remaining \<Property> elements are used to define which properties are concatenated together as the text. 

So if the WMI query returns two objects:
```json
{
    Index: 0,
    Caption: "Disk drive 1"
},
{
    Index: 1,
    Caption: "Disk drive 2"
}
```

These objects would be converted to options in the DropDownList, with a result equivalent to the following:

```xml
<GuiOption Type="DropDownList">
    <Option Text="0, Disk drive 1" Value="0" />
    <Option Text="1, Disk drive 2" Value="1" />
</GuiOption>
```
Note that the key property has been used as the value. The second \<Property Name="Index" /> has added the same property as the first part of the text.

### Separator
Be default, the property values will be concatenated together with comma and trailing space. To change this, set the \<Separator> element with the desired value:

```xml
<Query Type="Wmi">
    <Wql>select Index,Caption,Size,Description FROM Win32_DiskDrive where MediaType='Fixed hard disk media'</Wql>
    <Property Name="Index" />
    <Property Name="Index" />
    <Property Name="Caption" />
	<Separator> | </Separator>
</Query>
```

### Transforming Properties
If you want to transform properties, e.g. add prefix or suffix, do math, do a find/replace, you can add one or more transformation elements. In the example below, you can see examples of the Prefix, Calcuate, and Append transformations. 

Each transformation is performed in order, e.g. on the Size property, a calculation is done first, and then GB is appended to the result.  

```xml
<Query Type="Wmi">
    <Wql>select Index,Caption,Size FROM Win32_DiskDrive where MediaType='Fixed hard disk media'</Wql>
    <NameSpace>root\CIMV2</NameSpace>						<!-- Option to set the namespace -->
    <Property Name="Index"/>
    <Property Name="Index">
        <Prefix>ID: </Prefix>
    </Property>
    <Property Name="Size">
        <Calculate DecimalPlaces="2">VALUE/1073741824</Calculate>
        <Append>GB</Append>
    </Property>
    <Property Name="Caption"/>
    <Separator> | </Separator>
</Query>
```

In this example, the value in a DropDownList would be the drive Index (e.g. 0), and the text in each item would look something like 

*ID: 0 | 540GB | Hard disk 1*

Valid transformations are listed below.

#### \<Append>
Add text to the end of the property value
```xml
<Append>GB</Append>
```

#### \<Calculate>
Perform the specified calculation on the property value. Value must be a valid number. Use the text *VALUE* as a variable to represent the property value. 

Use the *DecimalPlaces* attribute to limted the decimal places returned by the calculation.

```xml
<Calculate DecimalPlaces="2">VALUE/1073741824</Calculate>
```

#### \<Prefix>
Add text to the start of the property value.

```xml
<Prefix>ID: </Prefix>
```


#### \<Replace>
Search for text in the value and replace it. Use the *Search* element or attribute to set the search, and the *Replace* element or attribute to set what to replace it with. 

Note that the xml:space="preserve" is sometimes required so that XML won't ignore your spaces as invalid whitepsace.

```xml
<Replace xml:space="preserve">
    <Search>Disk </Search>
    <Replace></Replace>
</Replace>
```

#### \<RegexReplace>
Search for text in the value using a Regex pattern and replace it. Use the *Regex* element or attribute to set the search pattern, and the *Replace* element or attribute to set what to replace it with. 

Note that the xml:space="preserve" is sometimes required so that XML won't ignore your spaces as invalid whitepsace.

```xml
<Replace xml:space="preserve" Regex="^0*" Replace="" />
```

#### \<ToLower>
Convert to lower case text

#### \<ToUpper>
Convert to upper case text

#### \<Truncate>
Truncate shortens the value to the desired length. Set the *Type* attribute to set how you want to truncate: KeepFromEnd, KeepFromStart, RemoveFromEnd, or RemoveFromStart

```xml
<Truncate Type="KeepFromStart">6</Truncate>
<Truncate Type="KeepFromEnd">4</Truncate>
<Truncate Type="RemoveFromStart">6</Truncate>
<Truncate Type="RemoveFromEnd">4</Truncate>
```

## Ignoring Values

Sometimes the resulting value aren't what we wanted returned from our query. An example is the default names set in WinPE during operating system deployments. 

To ignore a value and an \<Ignore> element to your query. 

```xml
<Query Type="EnvironmentVariable">
    <Variable Name="OSDComputerName"/>
    <Ignore>MININT</Ignore>
    <Ignore>MINWIN</Ignore>
</Query>
```

## Multiple Queries & Null/Empty values
By default, TsGui will ignore the a query that returns null or is empty. If you have multiple \<Query> blocks defined, a null result will make TsGui skip the query and try the next one. 

To override this behaviour, add the \<IgnoreEmpty> element with a value of FALSE. You might want to do this if you want the result in your GuiOption to be empty.

```xml
<Query Type="Wmi">
    <Wql>SELECT * FROM Win32_OperatingSystem WHERE Version LIKE "10.0.22%"</Wql>
    <Property Name="LocalDateTime">
        <Truncate Type="KeepFromStart">6</Truncate>								<!-- Truncate Type options: KeepFromEnd, KeepFromStart, RemoveFromEnd, RemoveFromStart -->
        <Truncate Type="KeepFromEnd">4</Truncate>
        <Prefix>PC-</Prefix>
    </Property>
    <Separator></Separator>
    <IgnoreEmpty>FALSE</IgnoreEmpty>
</Query>
```

## Queries with Null or Empty Property Values
By default, if a query returns multiple properties, TsGui will include a null value as part of the result. For example consider the query below to be concatenated together as the value for a FreeText GuiOption:


```xml
<Query Type="Wmi">
    <Wql>SELECT * FROM Win32_OperatingSystem</Wql>
    <Property Name="Caption" />
    <Property Name="Organization" />
    <Property Name="Status" />
    <Separator> | </Separator>
</Query>
```

In this instance the Organization property could be empty, in which case the result could end up as something like this:

**Micrsoft Windows 11 Enterprise | | OK**

Note the double bar with empty text between them, caused by there being an empty value for the Organization property. 

To override this behaviour, add the \<IncludeNullValues> element with a value of FALSE. Now the empty value will not be included:

**Micrsoft Windows 11 Enterprise | OK**


```xml
<Query Type="Wmi">
    <Wql>SELECT * FROM Win32_OperatingSystem</Wql>
    <Property Name="Caption" />
    <Property Name="Organization" />
    <Property Name="Status" />
    <Separator> | </Separator>
    <IncludeNullValues>FALSE</IncludeNullValues>
</Query>
```

## Reprocessing queries
There are some query types that may benefit from being manually run by the end user, rather than being dynmically run following changes of GuiOptions.  

For example suppose we have some info that is retrieved by running a script that takes a while to return. We might only want to run this when the user requests it. 

```xml
<GuiOption Type="InfoBox" ID="LongRunningScript">
    <Variable>INFO_Script</Variable>
    <Label>Something: </Label>

    <SetValue>
        <Query Type="PowerShell" xml:space="preserve">
            #Some long running script
        </Query>
    </SetValue>
</GuiOption>
```

In this case we set an ID on the GuiOption or NoUIOption that we want to reprocess, and then use an [ActionButton](/documentation/features/Actions.md) to reprocess the queries associated with that option. 

The **Type** attribute of the \<Action> element must be set to **Reprocess**.


```xml
<GuiOption Type="ActionButton">
    <Action Type="Reprocess" ID="LongRunningScript"/>
    <ButtonText>Refresh</ButtonText>
</GuiOption>
```

If you want to reprocess more that one option when the button is pressed, set multiple \<ID> elements. The options associated with those IDs will be processed in order. 

Note that if you set one or more \<ID> elements, the ID attribute on the \<Action> element will be ignored. 

```xml
<GuiOption Type="ActionButton">
    <Action Type="Reprocess">
        <ID>LongRunningScript</ID>
        <ID>SomeOtherID</ID>
    </Action>
    <ButtonText>Refresh</ButtonText>
</GuiOption>
```

**Note** that IDs set on an option must be unique within the config. These are the same IDs as those used for [Option Linking](/documentation/features/OptionLinking.md).