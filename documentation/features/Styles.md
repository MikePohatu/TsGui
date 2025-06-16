# Styles

**Contents**
* [Overview](#overview)
* [Base configuration](#base-configuration)
* [Style Tree](#style-tree)
* [Global Styles](#global-styles)
  * [Defining Global Styles](#defining-global-styles)
* [Applying Global Styles](#applying-global-styles)
* [Inheritance](#inheritance)
* [ShowGridLines](#showgridlines)
* [Loading External Font Files](#loading-external-font-files)
* [In more detail - a practical example](#in-more-detail---a-practical-example)

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
        <Family>Segoe UI</Family>   <!-- https://learn.microsoft.com/en-us/typography/font-list/ -->
    </Font>
</Style>
```

It is important to note that GuiOptions are made up of multiple items:
* Label
* Control
* A grid which contains the Label and the Control

Each of these items has it's own Style object (see below in the [Style Tree](#style-tree) section for how these are structured). 

## Style Tree
A 'Style Tree' is set on a 'root' element i.e. a grid, which contains a Label and a Control element. All three items can be styled separately, plus the grid element has some specific options e.g. which side to put the Label and Control.

These are structured as follows:

```xml
<Style>     <!-- The Style for the the grid element -->
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
            <Family>Segoe UI</Family>   <!-- https://learn.microsoft.com/en-us/typography/font-list/ -->
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

## ShowGridLines

To help with styling, you can use the **\<ShowGridLines** config option to add dashes around UI elements:
```xml
<TsGui>
    <ShowGridLines />
    ...
</TsGui>
```
These dashes will only show in [test mode](/documentation/TestMode.md).

## Loading External Font Files
When using the \<Family> option to change the font, you can load external fonts from file e.g. using a .ttf file. To do this you need to using a specific format. 

For example, if we download the [Painting with Chocolate](https://www.fontspace.com/painting-with-chocolate-font-f29268) font, we will download a file called *Paintingwithchocolate-K5mo.ttf* to C:\Temp\Fonts. To load this font we would use this format:

file:///%folder_path%#%font_name%

Note the following:
1. There are three slashes after *file:*
2. The folder path replaces the \ with / and is the path to the folder containing the font file (Paintingwithchocolate-K5mo.ttf in this case).
3. After the # is the **font** name, not the file name. So we would use "Painting with Chocolate" or "Painting with Chocolate Regular", not "Paintingwithchocolate-K5mo.ttf".

So specifying our font would look like this:

```xml
<Family>file:///C:/Temp/Fonts/#Painting with Chocolate Regular</Family>
```

To load a file reletive to TsGui.exe you use './' type notation, e.g. to use a Fonts subfolder with your TsGui files:
```xml
<Family>file:///./Fonts/#Painting with Chocolate Regular</Family>
``` 
Note that TsGui will expand the './' notation at run time to the parent directory of TsGui.exe. This isn't relative to the working directory of the process. 

## In more detail - a practical example

The following is an example of how the styling options of one item can effect the layout of others, and the why this can happen. Note the **\<ShowGridLines>** config option is used in the examples below.


---

A GuiOption is made up of three parts, the Label, the Control, and the Grid that contains the other two parts. 

When you set options at the root level of the \<Style> element, you are styling the Grid. Things set there don’t flow down to the Label and Control. They are specifically for the Grid. The Grid contains two cells, and the larger of the two cell heights dictates the height of the Grid itself as the two Grid cells can’t have different heights. 
 
In practise what this can mean is that expanding the overall height of the **Control** (e.g. with Padding and/or Margin) can effect the height of the cell that contains it, which then expands the height Grid, which then expands the height of the cell containing the **Label** (and vice versa expanding Label can effect Control).

---
For example, setting Padding on the label expands the overall height of the label:
```xml
<Label>
    <Padding>10</Padding>
    …
</Label>
``` 

![StyleExample1](/documentation/images/styles-ex1.png)
 
Note how the CPU text moves within the cell and the cell height gets larger. The cell height effects the Grid, which is also effecting the Control, but the text in the Control doesn’t move because it doesn’t have padding set.

--- 
Compared that to 0 padding and the Grid height is now lower:
```xml
<Label>
    <Padding>0</Padding>
    …
</Label>
```
 
![StyleExample2](/documentation/images/styles-ex2.png)

---
However if the Control has a centered \<VerticalAlignment> combined with the padding of the Label from the first example:

```xml
<Label>
    <Padding>10<Padding>
    …
</Label>
<Control>
    <VerticalAlignment>Center</VerticalAlignment>
    …
</Control>
```

![StyleExample3](/documentation/images/styles-ex3.png)
 
Now it starts to look like you have padding effecting both cells. Actually the padding is effecting the height of the Grid, making it look like you have padding in the Control. Note how the Control text is left aligned, indicating there isn’t any left padding set.

---