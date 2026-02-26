# Lists

* [Overview](#overview)
* [Sets vs Lists](#sets-vs-lists)
* [List types](#list-types)
* [Creating Lists](#creating-lists)
  * [Lists in the Lists Block](#lists-in-the-lists-block)
  * [Lists in a Set](#lists-in-a-set)
  * [List IDs](#list-ids)
* [Prefix](#prefix)
* [CountLength](#countlength)


## Overview
Lists create multiple variables. They are designed to be used with the "[Install applications according to dynamic variable list](https://learn.microsoft.com/en-us/mem/configmgr/osd/understand/task-sequence-steps#BKMK_InstallApplication)" task sequence option in ConfigMgr. 

Lists require TsGui version 2.3.0.0 or above. 

## Sets vs Lists
A [Set](./Sets.md) can contain one or more Variables, and one or more [Lists](./Lists.md). Think of a List as creating a dataset, a Set as a container of one or more datasets. 

A Set can also be [enabled or disabled](#enabling-a-set) dynamically. If you need to enable/disable a List based on some logic, you need to include it in a set.  


## List types

There are two types of Lists:

* [Option Lists](./OptionLists.md) - Create a dynamic list of variables based on the values of TsGui options
* [File Lists](./FileLists.md) - Create a list of variables based on an imported file. 




## Creating Lists
You can create lists using the \<Lists> element in the TsGui config, you can create a List as part of a [Set](./Sets.md), or a List will be [automatically created](./OptionLists.md#adding-an-option-to-a-list) when set on a TsGui option. 


### Lists in the Lists Block
```xml
<TsGui>
    <Lists>
        <List ID="AppList1" CountLength="3" />
        <List File="static_variables.txt" />
        <List File="dynamic_variables.txt" Prefix="AppList2" />
    </Lists>
</TsGui>
```

### Lists in a Set
```xml
<TsGui>
    ...
    <Sets>
        <Set ID="Example set">
            <!-- Configure whether the Set is enabled based on a query -->
            <Enabled>
                <Query Type="IfElse">
                    <IF SourceID="TsGui_IsServer" Equals="TRUE" Result="FALSE" />
                </Query>
            </Enabled>

            <List ID="AppList3" CountLength="2" />
            <List File="file.txt" />
            <List File="dynamic_set_variables.txt" Prefix="AppList4" />
        </Set>
    </Sets>
</TsGui>
```

### List IDs
Each List must have a unique ID. The ID is used to connect various parts of the configuration together. Either create a List in the \<Lists> block of your config, or in one and only one [Set](./Sets.md).

For [File Lists](./FileLists.md), if no ID attribute is set, the File attribute will be used as the ID. If for some reason you want to create two lists based on one file, use the same File attribute, but set a unique ID attribute on each List. 

For [Option Lists](./OptionLists.md), the ID is used to add other [TsGui options](/documentation/options/README.md) to the List using the **Lists** attribute.

## Prefix

The Prefix attribute defines the prefix of a list of variables to create. Each variable will be created with an incrementing number. The number of digits can be configured with the [CountLength](#countlength) attribute. 

On a File List, if you don't set a prefix it will be treated as a [Static](./FileLists.md#static-list-from-text-file) list.


## CountLength

The CountLength attribute defines how many numbers are used when incrementing the number part of the variable name. The default is 2.

```xml
<List Prefix="AppList_" CountLength="3" />
```

For example, if the prefix is AppList_:

CountLength="4"\
AppSet1_0001=Value1\
AppSet1_0002=Value2\
AppSet1_0003=Value3

CountLength="2"\
AppSet1_01=Value1\
AppSet1_02=Value2\
AppSet1_03=Value3




