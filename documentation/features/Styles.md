# Styles

**Contents**
* [Overview](#overview)

## Overview

Styles are previously known as 'Formatting'. The \<Formatting\> element is still valid in your configuration for backwards compatibility. Full example configuration file is available in the [example file](/Config_Examples/Config_Styles.xml)

How to video is available here: [https://youtu.be/Mjy65yHkoi8](https://youtu.be/Mjy65yHkoi8)

## Base configuration
Each Style object contains a number of options for changning the look of the desired item. 

A Style object consists of the options below:

```xml
<Style>
    <Width>Auto</Width>
    <Height>Auto</Height>

    <Padding>2,2,2,2</Padding>
    <Margin>2,2,2,2</Margin>

    <!-- These two alignments are for the control itself, i.e. the dropdownlist or label, not the text inside the control -->
    <VerticalAlignment>Bottom</VerticalAlignment>	<!-- Options available: Top, Bottom, Center, Stretch -->
    <HorizontalAlignment>Stretch</HorizontalAlignment>  <!-- Options available: Left, Right, Center, Stretch -->

    <!-- These two alignments are for the content inside the control, e.g. the text -->
    <VerticalContentAlignment>Bottom</VerticalContentAlignment> <!-- Options available: Top, Bottom, Center, Stretch -->
    <HorizontalContentAlignment>Stretch</HorizontalContentAlignment>    <!-- Options available: Left, Right, Center, Stretch -->
    
    <TextAlignment>Left</TextAlignment> <!-- Options available: Left, Right, Center -->
    <Font>
        <Weight>Normal</Weight>	    <!-- Options available: Normal, Bold, ExtraBold, Light -->
        <Size>11</Size>
        <Style>Normal</Style>	    <!-- Options available: Normal, Italic, Oblique -->
        <Color>Black</Color>
    </Font>
</Style>
```

It is important to note that GuiOptions are made up of multiple items:
* Label
* Control
* The GuiOption itself which wraps the Label and the Control together

Each of these items has it's own Style object (see below in the [Style Tree](#style-tree) section for how these are structured). 

## Style Tree
A 'Style Tree' is set on a 'root' element i.e. a GuiOption, which contains a Label and a Control element. All three items can be styled separately, plus the GuiOption element has some specific options e.g. which side to put the Label and Control.

These are structured as follows:

```xml
<Style>     <!-- The 'root' Style -->
    <Label /> <!-- The Style for the Label item -->
    <Control /> <!-- The Style for the Control item -->
</Style>
```

Each of the three items above is a \<Style /> element as shown above in the [Styles](#styles). An expanded configuration is shown below:

```xml
<Style>     <!-- The 'root' Style -->
    <LabelOnRight>FALSE</LabelOnRight>
    <LeftCellWidth>110</LeftCellWidth>
    <RightCellWidth>160</RightCellWidth>

    <Label> <!-- The Style for the Label item -->
        <Width>Auto</Width>
        <Height>Auto</Height>

        <Padding>2,2,2,2</Padding>
        <Margin>2,2,2,2</Margin>

        <!-- These two alignments are for the control itself, i.e. the dropdownlist or label, not the text inside the control -->
        <VerticalAlignment>Bottom</VerticalAlignment>	<!-- Options available: Top, Bottom, Center, Stretch -->
        <HorizontalAlignment>Stretch</HorizontalAlignment>  <!-- Options available: Left, Right, Center, Stretch -->

        <!-- These two alignments are for the content inside the control, e.g. the text -->
        <VerticalContentAlignment>Bottom</VerticalContentAlignment> <!-- Options available: Top, Bottom, Center, Stretch -->
        <HorizontalContentAlignment>Stretch</HorizontalContentAlignment>    <!-- Options available: Left, Right, Center, Stretch -->
        
        <TextAlignment>Left</TextAlignment> <!-- Options available: Left, Right, Center -->
        <Font>
            <Weight>Normal</Weight>	    <!-- Options available: Normal, Bold, ExtraBold, Light -->
            <Size>11</Size>
            <Style>Normal</Style>	    <!-- Options available: Normal, Italic, Oblique -->
            <Color>Black</Color>
        </Font>
    </Label>

    <Control> <!-- The Style for the Control item -->
        <!-- All options listed above for the Label and also valid for the Control -->
    </Control>
</Style>
```

## Global Styles
Styles can be defined globally, then applied to any UI elements as required. In this way a Style can be defined once and applied many times without having to worry about problems with [Inheritance](#inheritance) or differences with styling different element types.

### Defining Global Styles
To define global styles, create a \<Styles> attribute in the root of your configration with an ID attribute e.g. 

```xml
<TsGui>
    <Styles>
        <Style ID="Example_Style1">
            ...
        </Style>
        <Style ID="Example_Style2">
            ...
        </Style>
        <Style ID="Example_Style3">
            ...
        </Style>
    </Styles>
</TsGui>
```
The ID attribute is used the reference the Style elsewhere in the configuration.

## Applying Global Styles

You can apply the style to an element by setting a \<Styles> attribute on the desired element. Multiple Styles can be applied by separating the style IDs with a comma 

```xml
<Row Styles="Example_Style1">
    <Column>
        <GuiOption Type="ComputerName" Styles="Example_Style2, Example_Style3" />
    </Column>
</Row>
```


## Inheritance
Styles are inherited down the tree and applied in the following order:

1. A clone of the complete Style from the parent element (Page/Row/Column etc) is is applied
2. Global styles are imported
3. The \<Style> set on the UI element is applied
