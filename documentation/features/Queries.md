# Queries

Queries can be used in a number of places within TsGui to get system state. Primarily they are used in the \<SetValue> element to assign the value to a GuiOption. They are also used in the [Compliance](Compliance.md) and [Validation](Validation.md) features, as well as being options for setting property values for [scripts](Scripts.md). 

* [Query Types](#query-types)
* [Structure](#structure)
* [Ignoring Values](#ignoring-values)

## Query Types

* [Combined](queries/Combined.md)
* [EnvironmentVariable](queries/EnvironmentVariable.md)
* [If/Else](queries/IfElse.md)
* [Registry](queries/Registry.md)
* [Value-Only](queries/Value.md)
* [WMI](queries/WMI.md)

## Structure


## Ignoring Values

Sometimes the values set in the environment variable aren't what we wanted returned from our query. An example is the default names set in WinPE during operating system deployments. 

To ignore a value and an \<Ignore> element to your query. 

```xml
<Query Type="EnvironmentVariable">
    <Variable Name="OSDComputerName"/>
    <Ignore>MININT</Ignore>
    <Ignore>MINWIN</Ignore>
</Query>
```