# Groups and Toggles

How-to videos for this feature are available here: [Part 1](https://youtu.be/r3nrYElUs88) & [Part 2](https://youtu.be/Tzyo-j92LG0). 

Full example configuration file is available [here](/Config_Examples/Groups_and_Toggles.xml).

## Overview

The Groups and Toggles feature is used to enable and disable parts of the UI based on the options selected by the user. For example hide certain options if a certain business unit or build type is selected from a [DropDownList](/documentation/options/DropDownList.md).

One or more elements are added to a **Group**. This group can then be _enabled_ or _disabled_ and all members of the group (and their child items) will be enabled/disabled accordingly.

It should be noted that a [Page](/documentation/Layout.md#page), [Row](/documentation/Layout.md#row), [Column](/documentation/Layout.md#column), or [Container](/documentation/Layout.md#containers) can be added as a member of a group. Enabling/disabling these 'parent' elements will cause all child items to follow suit.

Group members can be Enabled, Disabled or Hidden. The Disabled or Hidden states are known as **Inactive** states. 

A Group is enabled and disabled using a **Toggle**. A Toggle is added to an option in the configuration. If the value of that option matches the toggle rules, the group will be enabled or disabled appropriately. 


## Creating a Group
A Group is created by adding a ```<Group>``` element to an element in the UI, along with an associated **Name** as the value, e.g.

```xml
<GuiOption Type="FreeText">
    <Group>Example_Group_Name</Group>
    ...
</GuiOption>
```

This item is now a member of the _Example_Group_Name_ group. 

You can also add a [layout element](/documentation/Layout.md) such as a Row or Column as a group member. All child items will become members of the group. For example all the GuiOptions in the configuration below will be members of the _Example_Group_Name2_ group.

```xml
<Row>
    <Group>Example_Group_Name2</Group>

    <Column>
        <GuiOption Type="FreeText">
            ...
        </GuiOption>

        <GuiOption Type="CheckBox">
            ...
        </GuiOption>

        <GuiOption Type="CheckBox">
            ...
        </GuiOption>
    </Column>
</Row>
```

An item can be a member of multiple groups. See [Group Precendence](#group-precedence) for details on the behaviour in this scenario.

## Adding a Toggle
An option can be configured as a Toggle using a ```<Toggle>``` element. Note that you can create more than one Toggle per option e.g. to control multiple groups. 

It is best practise to only configure 1 toggle per group. If you configure more than one toggle, unexpected behaviour may occur as they may conflict. 

### Toggle Configuration

* Set the **group name** associated with the Toggle using the **Group** attribute. 
* Set the values that will enable the Group using one or more ```<Enabled>``` elements (if not set, TRUE is assumed).
* Set the values that will disable the Group using one or more ```<Disabled>``` elements (if not set, FALSE is assumed).
* If you wish the UI elements in the Group to be fully hidden (rather than just disabled), add the ```<Hide />``` element. 


### Simple Example
```xml
<GuiOption Type="CheckBox">
    <Variable>TOGGLE_Apps</Variable>
    <Label>Additional applications</Label>
    <HAlign>left</HAlign>
    
    <Toggle Group="Group_Apps">
        <Enabled>TRUE</Enabled>
        <Disabled>FALSE</Disabled>
        <Hide/>
    </Toggle>
</GuiOption>
```

### DropDownList Toggle Example
Toggle with multiple ```<Enabled>``` values.

```xml
<GuiOption Type="DropDownList">
    <Group>Group_IT</Group>
    <Variable>VAR_IT_Teams</Variable>
    <Label>Team:</Label>

    <Toggle Group="Group_EngineeringTeams">
        <Enabled>Desktop</Enabled>
        <Enabled>Server</Enabled>
        <Enabled>ServiceDesk</Enabled>
        <Hide/>
    </Toggle>


    <SetValue>
        <Value>Desktop</Value>
    </SetValue>

    <Option>
        <Text>Business Analysts</Text>
        <Value>BA</Value>
    </Option>
    <Option>
        <Text>Desktop</Text>
        <Value>Desktop</Value>
    </Option>
    <Option>
        <Text>Server</Text>
        <Value>Server</Value>
    </Option>
    <Option>
        <Text>Service Desk</Text>
        <Value>ServiceDesk</Value>
    </Option>
</GuiOption>
```

### Setting Toggles on DropDownList Options
To make configuring toggles based on the options in a [DropDownList](/documentation/options/DropDownList.md) easier to read, you can add the ```<Toggle>``` element directly within the ```<Option>``` as below, optionally adding a ```<Hide />``` element as well.

```xml
<GuiOption Type="DropDownList" Sort="FALSE">
    <Variable>BuildType</Variable>
    <Label>Build type:</Label>
    <SetValue>
        <Value>Desktop</Value>
    </SetValue>
    <Option>
        <Text>Desktop</Text>
        <Value>Desktop</Value>

        <Toggle Group="Group_D_Build">
            <Hide/>
        </Toggle>
    </Option>
    <Option>
        <Text>Kiosk</Text>
        <Value>Kiosk</Value>

        <Toggle Group="Group_K_Build">
            <Hide/>
        </Toggle>
    </Option>
</GuiOption>
```

## Group Precedence
Where an item is a member of multiple groups, if any group is enabled, the option will be enabled.	

If none are enabled, if the groups toggle is configure with ```<Hide />```, the option will be hidden. 

Otherwise, the option will be disabled.

## Values of Inactive Options
By default, when an item is inactive it's value will be set to **TSGUI_INACTIVE**. This is to ensure unintended values are not passed on into a running Task Sequence.

In some instances you may wish to override this behaviour, either by setting a custom value, or by removing it completely (this is known as [purging](#purgeinactive) the value). 

### InactiveValue
You can use the **InactiveValue** attribute to override the value if the item is disabled. In the example below, the _Install_Office_ will value will be set to _FALSE_ if the checkbox is inactive. 

```xml
<GuiOption Type="CheckBox" InactiveValue="FALSE">
    <Variable>Install_Office</Variable>
    <Label>Install MS Office</Label>
    <HAlign>left</HAlign>
</GuiOption>
```

### PurgeInactive
You can use the Groups and Toggles feature to create complete replacements for parts of the UI based on group memberships. As an example you may wish one version of a Page to display for build type X, another version for build type Y.

In this case you may end up with duplicate variables being configured that will cause a conflict. 

To resolve this, you can use the **PurgeInactive="TRUE"** attribute. This will result in the inactive item to not create an value in the [output](/documentation/features/TsGuiOutput.md) at all, thereby removing the conflict.

As an example, the first page of the config below contains a DropDownList to select a build type, either Desktop or Kiosk. Each option toggles an associated group.

The following two pages then both create a ComputerName GuiOption. Both GuiOptions would normally attempt to create an OSDComputerName output, but because the PurgeInactive attribute is set, only the active one will do so. 

The group is set on the ```<Page>``` element, so the entire page will be hidden from the UI if inactive. 

```xml
<Page>
    <Row>
        <Column>
            <GuiOption Type="DropDownList">
                <Variable>BuildType</Variable>
                <Label>Build type:</Label>
                <SetValue>
                    <Value>Desktop</Value>
                </SetValue>
                <Option>
                    <Text>Desktop</Text>
                    <Value>Desktop</Value>
                    <Toggle Group="Group_D_Build">
                        <Hide/>
                    </Toggle>
                </Option>
                <Option>
                    <Text>Kiosk</Text>
                    <Value>Kiosk</Value>
                    <Toggle Group="Group_K_Build">
                        <Hide/>
                    </Toggle>
                </Option>
            </GuiOption>
        </Column>
    </Row>
</Page>


<Page  PurgeInactive="TRUE">
    <Group>Group_D_Build</Group>

    <GuiOption Type="ComputerName" />
    ...
</Page>

<Page  PurgeInactive="TRUE">
    <Group>Group_K_Build</Group>

    <GuiOption Type="ComputerName" />
    ...
</Page>
```


