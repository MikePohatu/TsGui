# Option Linking

Option Linking enables the value of one option to influence the value of another. This differs from groups in that groups only set whether an option is enabled or not, Linking will update an option as the options they are linked to change.

A how-to video is also available on [YouTube](https://youtu.be/cZdb_qSu1eE)

**Contents**
* [Configuration](#configuration)
  * [Source](#source)
  * [Target](#target)
* [Config examples](#config-examples)
  * [Standard OptionValue query](#standard-optionvalue-query)
  * [LinkTo query](#linkto-query)
  * [LinkTrue query](#linktrue-query)
  * [LinkFalse query](#linkfalse-query)
  * [NotLinkTrue query](#notlinktrue-query)
  * [NotLinkFalse query](#notlinkfalse-query)
  * [OptionValue query inside an IfElse query](#optionvalue-query-inside-an-ifelse-query)
  * [Altering the OptionValue result](#altering-the-optionvalue-result)


---

## Configuration
Option linking requires two things, a *source*, and one or more *targets*. A target will react to any changes to the value of the source. for example: 

* OptionA - Type="CheckBox" - Install all apps"
* OptionB - Type="CheckBox" - "Office"
* OptionC - Type="CheckBox" - "Firefox"

If the desired behaviour is that when you select OptionA both OptionB and OptionC are also selected, OptionA is the source, OptionB and OptionC are the targets. 

### Source
The source option must have the **ID** attribute set. The ID will be referenced by any targets. In the example below, the link ID is *link_name*, and will be referenced in the target queries below.

```xml
<GuiOption Type="ComputerName" ID="link_name"/>
```

### Target
A target option will use a query to set its value based on the value of the source. The standard option linking query is the ```OptionValue``` query type, however there are other 'shorthand' query types to make setting certain types of linking up easier. These are outlined below in [Config examples](#config-examples)

The standard OptionValue query type will reference the ID attribute configured in the source in the ID element as below:
```xml
<ID Name="source_id">
```

The 'shorthand' types such as *LinkTo, LinkTrue, LinkFalse* all set the source ID as the value of the element (see below).

---

## Config examples
The [Config_Linking.xml](../../Config_Examples/Config_Linking.xml) example file contains linking configuration options in a full TsGui configuration.

### Standard OptionValue query
```xml
<Query Type="OptionValue">
    <ID Name="link_name"/>
</Query>
```

### LinkTo query
A LinkTo query is shorthand for "Set the value to whatever the source is whenever the source changes"
```xml
<Query Type="LinkTo">link_name</Query>
```

### LinkTrue query
A LinkTrue query is shorthand for "Set the value to TRUE when the source value is set to TRUE"
```xml
<Query Type="LinkTrue">link_name</Query>
```

### LinkFalse query
A LinkFalse query is shorthand for "Set the value to FALSE when the source value is set to FALSE"

```xml
<Query Type="LinkFalse">link_name</Query> 
```

### NotLinkTrue query
A NotLinkTrue query is shorthand for "Set the value to FALSE when the source value is set to TRUE", in other words an inverted LinkTrue
```xml
<Query Type="LinkTrue">link_name</Query>
```

### NotLinkFalse query
A NotLinkFalse query is shorthand for "Set the value to TRUE when the source value is set to FALSE", in other words an inverted LinkFalse

```xml
<Query Type="LinkFalse">link_name</Query> 
```

### OptionValue query inside an IfElse query
An OptionValue query can also be used inside a 'wrapper' query such as an IfElse or Combined query. In the IfElse example below the value of the source option is evaluated to see if it starts with 'test' or contains '12'. If both match then the query will return 'TRUE', otherwise it will return 'FALSE'

```xml
<Query Type="IfElse">
    <IF>
        <Source>
            <Query Type="OptionValue">
                <ID Name="link_name"/>
            </Query>
        </Source>
        <Ruleset Type="AND">
            <Rule Type="StartsWith">test</Rule>
            <Rule Type="Contains">12</Rule>
        </Ruleset>
        <Result>
            <Value>TRUE</Value>
        </Result>
    </IF>
    <ELSE>
        <Value>FALSE</Value>
    </ELSE>
</Query>
```

### Altering the OptionValue result
The \<ID> element can be used in the same way as a \<Property> element in a WMI query i.e. Append, Prefix, Truncate, Calculate. See [Config_Queries.xml](../../Config_Examples/Config_Queries.xml) for examples. The example below will append '-Append' to whatever the value of the source is

```xml
<Query Type="OptionValue">
    <ID Name="link_name">
        <Append>-Append</Append>
    </ID>
</Query>
```