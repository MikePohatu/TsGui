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

All items in the tree can have configuration applied to them e.g. [Style](/documentation/features/Styles.md), [Grouping](/documentation/features/GroupsAndToggles.md), [ReadOnly](/documentation/options/README.md#readonly) option. Configuration applied to an item higher in the tree will propogate down the tree

## Page
A TsGui UI contains one or more pages. Each page is contained within a ```<Page>``` element, and can contain one or more [rows](#row). If more than one page exists, TsGui will automatically configure the Next/Previous buttons appropriately.

Note that when you [Style](/documentation/features/Styles.md) the height or width of a page, it will override the values set for the size of the window. In the example below, the second page will have height of 350 and width of 400. Pages 1 & 3 will have the default height of 230 and width of 300.

```xml
<TsGui>
    <Height>230</Height>
	<Width>300</Width>

    <!-- Page 1 -->
    <Page>
        ...
    </Page>
    
    <!-- Page 2 -->
    <Page>
        <Style>
            <Height>350</Height>
            <Width>400</Width>
        </Style>
        ...
    </Page>

    <!-- Page 3 -->
    <Page>
        ...
    </Page>
</TsGui>
```

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