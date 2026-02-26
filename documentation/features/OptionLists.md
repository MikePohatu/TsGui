# Option Lists

* [Overview](#overview)
* [Creating a List](#creating-a-list)
* [Adding an option to a List](#adding-an-option-to-a-list)
* [How values are set](#how-values-are-set)
  * [ValueTest attribute](#valuetest-attribute)
  * [UseValue attribute](#usevalue-attribute)
* [Inheritance](#inheritance)


## Overview

An Option List links to other options in your TsGui config, and creates a list of variables based on those. Option Lists are primarily designed to be used with the "[Install applications according to dynamic variable list](https://learn.microsoft.com/en-us/mem/configmgr/osd/understand/task-sequence-steps#BKMK_InstallApplication)" feature in ConfigMgr. 


## Creating a List

A List can be created in multiple ways:
* Specified in the [\<Lists> block](./Lists.md#the-lists-block) of the configuration
* Automatically when you [add an option to a list](#adding-an-option-to-a-list)
* Specified within a [Set](./Sets.md)

The **Prefix** attribute defines the prefix of a list of variables to create. Each variable will be created with an incrementing number. If you don't set a prefix, the ID of the list will be used as the prefix. 

## Adding an option to a List
You add an Option to one or more lists by adding the Lists attribute to it with the ID of the desired list. You can add multiple list IDs separated by a comma.

```xml
<GuiOption Type="CheckBox" Lists="Lis1,List2">
```

A list will be automatically created when you add an option if it doesn't already exist (i.e. created in the [Lists block](#the-lists-block) or in a [Set](./Sets.md)) with the specified ID and the prefix set to the same as the ID. 

In this way the configuration can be simplified if you only need the default List settings and you don't need to dynamically enable or disable the list using a Set. Just add the relevant options to the List and you're done. 


## How values are set
Because Option Lists are designed to be used with [Install applications according to dynamic variable list](https://learn.microsoft.com/en-us/mem/configmgr/osd/understand/task-sequence-steps#BKMK_InstallApplication), the default behaviour of an Option List is setup accordingly. In TsGui this is likely to be represented by a list of CheckBoxes as below. 
 

```xml
<Container Lists="AppList">
    <GuiOption Type="CheckBox" Variable="Microsoft Office 365" />
    <GuiOption Type="CheckBox" Variable="7-zip" />
    <GuiOption Type="CheckBox" Variable="Adobe Creative Cloud" />
</Container>
```

Note the use of a [Container](/documentation/Layout.md#containers) to set the Lists attribute on everything inside it. Also note that if you don't set a Label on a GuiOption, [the Variable will be used](/documentation/options/README.md#label). 

By default, when you click Finish, the Option List will evaluate all associated options. 

1. If the option isn't [enabled](/documentation/features/GroupsAndToggles.md), it will be ignored
2. The value of the option is compared against an attribute called **ValueTest**. The default value of ValueTest is TRUE i.e. the default value of a ticked CheckBox. 
3. If ValueTest matches the value of the option, the option is processed as part of the list
4. Variables are created with a name based on the prefix, and an incrementing number. The value is the **Variable Name** of the option. 

Note that the GuiOptions added to the List will still create their configured variables as normal.

This behaviour can be overridden using the attributes below.

### ValueTest attribute
To change the default value to compare against, set the **ValueTest** attribute. For example to add items to the list when a CheckBox is unticked:

```xml
<List ID="AppList" ValueTest="FALSE" />
```

### UseValue attribute
If you want to create variables using the value of option rather than the variable name, set the **UseValue** attribute to **TRUE**

```xml
<List ID="VariableList1" UseValue="TRUE" />
```

## Inheritance
The Lists attributes follows the same inhertihance flow as things like [Styles](./Styles.md). You can set the Lists attribute on a [Page](/documentation/Layout.md#page), [Row](/documentation/Layout.md#row), [Column](/documentation/Layout.md#column), or [Container](/documentation/Layout.md#containers), and it will apply to all child options. 

**Note** if the Lists attribute is overridden on a child node, it will replace what was set on the parent in its entirety.  
```xml
<TsGui>
    <Page>
        <Row>
            <Column>
                <Container Lists="AppList">
                    <GuiOption Type="CheckBox" Variable="Microsoft Office 365" />
                    <GuiOption Type="CheckBox" Variable="7-zip" />
                    <GuiOption Type="CheckBox" Variable="Adobe Creative Cloud" />
                </Container>
            </Column>
        </Row>
    </Page>
<TsGui>
    