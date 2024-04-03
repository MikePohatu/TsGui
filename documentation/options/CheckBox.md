# CheckBox
* [Overview](#overview)
* [Checked by Default](#checked-by-default)
* [Changing the Default Values](#changing-the-default-values)

## Overview
The CheckBox type is a simple check box ideal for binary decisions. It has one value for checked, one for unchecked (by default TRUE and FALSE respectively).

## Checked by Default
To set the CheckBox as checked by default, add ```<Checked />``` to the GuiOption (note that this may be overridden if you also set [Queries](/documentation/features/Queries.md) in the ```<SetValue>``` element):

```xml
<GuiOption Type="CheckBox">
    <Variable>App_MsOffice</Variable>
    <Label>Microsoft Office</Label>
    <Checked />
</GuiOption>
```

## Changing the Default Values
By default, the CheckBox GuiOption will set a value of **TRUE** when checked, and **FALSE** when unchecked.

To change this, set the ```<TrueValue>``` and/or the ```<FalseValue>``` elements.

```xml
<GuiOption Type="CheckBox">
    <Variable>App_MsOffice</Variable>
    <Label>Microsoft Office</Label>
    <Checked />
    <TrueValue>InstallPlease</TrueValue>
    <FalseValue>NahItsOK</FalseValue>
</GuiOption>
```