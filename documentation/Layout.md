# Layout

**Contents**
* [Overview](#overview)
* [Page](#page)
* [Row](#row)
* [Column](#column)
* [GuiOptions](#guioptions)
* [Containers](#containers)

## Overview
TsGui is built around a grid structure with rows and columns similar to an HTML table:

```xml
<Page>
    <Row>
        <Column>
            <GuiOption Type="ComputerName" />
        </Column>
    </Row>
</Page>
<Page>
    <Row>
        <Column>
            <GuiOption Type="FreeText" />
        </Column>
    </Row>
</Page>
```

All items in the tree can have configuration applied to them e.g. [Style](/documentation/features/Styles.md)  or [Grouping](/documentation/features/GroupsAndToggles.md). Configuration applied to an item higher in the tree will propogate down the tree

## Page
A TsGui UI contains one or more pages. Each page is contained within a ```<Page>``` element, and can contain one or more [rows](#row). If more than one page exists, TsGui will automatically configure the Next/Previous buttons appropriately.

## Row
Each page can contain one or more rows to break up the page space horizontally. Each row can contain one or more [columns](#column).

## Column
Each row can contain one or more columns to break up the page space vertically. Each column can contain one or more [GuiOptions](#guioptions) or [Containers](#containers).

## GuiOptions
The actual control and text elements in the UI are created using GuiOptions. Refer to the GuiOption [documentation](/documentation/options/README.md) for more details.


## Containers
A container is a logical grouping of items e.g. GuiOptions within the tree. This allows all items in the container to inherit the configuration applied to the container. This allows you apply configuration to multiple items without having to apply to the whole page, row, or column.

For example, in the configuration below, the ```<Container>``` element contains a ```<Style>``` configuration block. All the ```<GuiOption>``` elements in the container will inherit this style block, while the GuiOption outside it (link_source) will not. 

```xml
<GuiOption Type="CheckBox" ID="link_source">
    <Variable>link_source</Variable>
    <Label>SOURCE</Label>
    <HAlign>left</HAlign>
</GuiOption>

<Container>
    <Style>
        <Label>
            <Font>
                <Size>13</Size>
                <Color>Blue</Color>
            </Font>
        </Label>
    </Style>

    <GuiOption Type="CheckBox">
        <Variable>linkto</Variable>
        <Label>Match SOURCE</Label>
        <HAlign>right</HAlign>
        <Checked/>
        <SetValue>
        <Query Type="LinkTo">link_source</Query>
        </SetValue>
    </GuiOption>

    <GuiOption Type="CheckBox">
        <Variable>linktrue</Variable>
        <Label>Check when SOURCE is checked</Label>
        <HAlign>right</HAlign>
        <SetValue>
        <Query Type="LinkTrue">link_source</Query>
        </SetValue>
    </GuiOption>

    <GuiOption Type="CheckBox">
        <Variable>linkfalse</Variable>
        <Label>Uncheck when SOURCE is unchecked</Label>
        <HAlign>right</HAlign>
        <SetValue>
        <Query Type="LinkFalse">link_source</Query>
        </SetValue>
    </GuiOption>
</Container>

```