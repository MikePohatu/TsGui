# TreeView

* [Overview](#overview)
* [Example Option 'Tree'](#example-option-tree)
* [Setting an Option 'Not Selectable'](#setting-an-option-not-selectable)
* [Full Configuration Example](#full-configuration-example)

## Overview

The TreeView GuiOption is used to display hierarchical data. This feature is currently considered BETA and has a few layout 'quirks'.


The selectable options are configured similar to a [DropDownList](./DropDownList.md), but you can also add child ```<Option>``` elements to each Option in addition to a value and text.

## Example Option 'Tree'

```xml
<Option>
    <Text>Australia</Text>
    <Value>AU</Value>
    <Option>
        <Text>Sydney</Text>
        <Value>Syd</Value>
    </Option>
    <Option>
        <Text>Brisbane</Text>
        <Value>Bri</Value>
    </Option>
</Option>
```

## Setting an Option 'Not Selectable'
In some cases you may wish a root node to contain the child options, but not be selectable by the user. In this case set the **Selectable** attribute to **FALSE**.

```xml
<Option Selectable="FALSE">
    <Text>Australia</Text>
    <Value>AU</Value>
    <Option>
        <Text>Sydney</Text>
        <Value>Syd</Value>
    </Option>
    <Option>
        <Text>Brisbane</Text>
        <Value>Bri</Value>
    </Option>
</Option>
```

## Full Configuration Example

```xml
<GuiOption Type="TreeView">
    <!-- The NoSelectionMessage element overrides the default "Please select a value" text when nothing is selected by the user -->
    <NoSelectionMessage>Please select something</NoSelectionMessage>
    <Variable>Location</Variable>
    <Label>Location:</Label>
    <SetValue>
        <Value>Takapuna</Value>
    </SetValue>

    <Option>
        <Text>Australia</Text>
        <Value>AU</Value>
    </Option>
    <!-- Setting the Selectable attribute to FALSE will make this behave as a 'root node' for collapsing etc, but will not be 
            selectable -->
    <Option Selectable="FALSE">
        <Text>New Zealand</Text>
        <Value>NZ</Value>
        <Option>
            <Text>Auckland</Text>
            <Value>Akl</Value>
            <Option>
                <Text>Takapuna</Text>
                <Value>Takapuna</Value>
                <Option>
                    <Text>Street1</Text>
                    <Value>Street1</Value>
                    <Option>
                        <Text>House1</Text>
                        <Value>House1</Value>
                    </Option>
                    <Option>
                        <Text>House2</Text>
                        <Value>House2</Value>
                    </Option>
                </Option>
                <Option>
                    <Text>Street2</Text>
                    <Value>Street2</Value>
                </Option>
            </Option>
            <Option>
                <Text>North Shore</Text>
                <Value>NorthShore</Value>
            </Option>
        </Option>
        <Option>
            <Text>Wellington</Text>
            <Value>Wlg</Value>
        </Option>
    </Option>
    <Option>
        <Text>USA</Text>
        <Value>USA</Value>
    </Option>
</GuiOption>
```