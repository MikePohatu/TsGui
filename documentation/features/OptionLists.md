# Option Lists


## Overview
Option Lists are designed to be used with the "[Install applications according to dynamic variable list](https://learn.microsoft.com/en-us/mem/configmgr/osd/understand/task-sequence-steps#BKMK_InstallApplication)" task sequence option in ConfigMgr. 


## Creating a List

A List can be created in multiple ways:
* Specified in the <Lists> block of the configuration
* Automatically when you [add an option to a list](#adding-an-option-to-a-list)
* Specified within a [Set](./Sets.md)
```xml
<TsGui>
    <Lists>
        <List ID="AppList1" Prefix="Apps" CountLength="3" />
    </Lists>
</TsGui>
```

A list will be automatically created when you [add an option to a list](#adding-an-option-to-a-list).

## Adding an option to a List
You add an Option to one or more lists by adding the Lists attribute to it with the ID of the desired list. You can add multiple list IDs separated by a comma.

```xml
<GuiOption Type=ComputerName Lists="Lis1,List2">
```
