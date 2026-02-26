# Sets

* [Overview](#overview)
* [Sets vs Lists](#sets-vs-lists)
* [Config Overview](#config-overview)
* [Enabling a Set](#enabling-a-set)
  * [Using Groups](#using-groups)
  * [Using Queries](#using-queries)
* [Testing](#testing)


## Overview
The 'Sets' feature allows the creation of multiple variables using a simple list format in a text file, rather than having to create individual GuiOptions or NoUiOptions using XML. 

You can import from text files to simplify your configuration files. 

This feature requires TsGui version 2.1.0.0 or above.

## Sets vs Lists
A [Set](./Sets.md) can contain one or more Variables, and one or more [Lists](./Lists.md). Think of a List as creating a dataset, a Set as a container of one or more datasets. 

A Set can also be [enabled or disabled](#enabling-a-set) dynamically. If you need to enable/disable a List based on some logic, you need to include it in a set.  

## Config Overview

The **\<Sets>** element can contain one or more sets, each defined in a **\<Set>** element. The \<Set> configuration is outlined in more detail in the following sections. 

```xml
<TsGui>
    ...
    <Sets>
        <Set ID="Example set">
            <!-- Configure whether the Set is enabled based on a query -->
            <Enabled>
                <Query Type="IfElse">
                    <IF SourceID="TsGui_IsServer" Equals="TRUE" Result="TRUE" />
                </Query>
            </Enabled>

            <!-- Configure whether the Set is enabled based on a group -->
            <Group>TestGroup</Group>
            
            <!-- Create a static list of variables imported from file -->
            <List File="file.txt" />

            <!-- Create a dynamic list of variables imported from file -->
            <List File="file.txt" Prefix="AppNamePrefix" />

            <!-- Manually create variables in the set -->
            <Variable Name="OSDRegisteredOrgName" Value="Contoso" />
            <Variable Name="OSDTimeZone" Value="New Zealand Standard Time" />
            <Variable Name="OSDDomainOUName">
                <Value>LDAP://OU=MyOu,DC=Servers,DC=contoso,DC=com</Value>
            </Variable>
        </Set>
    </Sets>
</TsGui>
```

## Enabling a Set
Sets can be enabled and disabled based on options set elsewhere in TsGui using either the [Groups and Toggles](./GroupsAndToggles.md) feature or by using a TsGui [Query](./Queries.md). 

If neither of these options are set, the set will always be enabled. 

If both options are set, both need to be enabled for the set to be enabled. 

### Using Groups
Sets can be enabled and disabled using the [Groups](GroupsAndToggles.md) feature the same way GuiOptions and NoUIOptions are. 

If you add a \<Group\> element to your set, the Set will be enabled when the group is enabled. When the group is disabled, no variables from the set will be created i.e. the [InactiveValue](GroupsAndToggles.md#inactivevalue) option doesn't apply to Sets.

```xml
<Set>
    <Group>ExampleGroup</Group>
    ...
</Set>
```

### Using Queries

To use a [TsGui Query](./Queries.md) to enable the set, add an \<Enabled\> element to your set. Within the Enabled element you can add one or more queries. Each query will be processed in order. If any of the queries return the value **TRUE** then the set will be enabled. 

Usually this option would be used an [IfElse](./queries/IfElse.md#configuration-short-hand) query with the [Option Linking](./OptionLinking.md) feature. This will enable the set based on the value of another GuiOption e.g. a [DropDownList](/documentation/options/DropDownList.md).

```xml
<Set>
    <Enabled>
        <Query Type="IfElse">
            <IF SourceID="TsGui_IsServer" Equals="TRUE" Result="TRUE" />
        </Query>
    </Enabled>
    ...
</Set>
```



## Testing

Sets are displayed in the _Sets_ area in the [LiveData](/documentation/TestMode.md#the-livedata-window) window. To make identifying multiple sets easier in the testing window, set an ID attribute on the set. 

```xml
<Set ID="ExampleID">
```

The ID isn't usable with the [Option Linking](./OptionLinking.md) feature.