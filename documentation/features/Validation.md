# Validation

Documentation is on the to-do list for this feature. For usage information, see the how-to video on [YouTube](https://youtu.be/4GjpDAYp-CU) or the [example configuration file](/Config_Examples/Config_Validation.xml).

## Rules
Valid rule types are listed below:

```xml
<Rule Type="IsNumeric"/>
<Rule Type="GreaterThan">10</Rule>
<Rule Type="GreaterThanOrEqualTo">10</Rule>
<Rule Type="LessThan">10</Rule>
<Rule Type="LessThanOrEqualTo">10</Rule>
<Rule Type="StartsWith" CaseSensitive="TRUE">AB</Rule>
<Rule Type="EndsWith">YZ</Rule>
<Rule Type="Contains">YZ</Rule>
<Rule Type="Characters">`~!@#$%^*()</Rule>
<Rule Type="RegEx" CaseSensitive="TRUE">^(YZ|XZ)[0-9]{6}$</Rule>
<Rule Type="Equals" CaseSensitive="TRUE">*NULL</Rule>
```

## Controlling Tooltips
By default, tooltip messages will be shown immediately after a validation failure. If there are many GuiOptions with validation failures, this can result in the UI becoming full of tooltips. To control this you can change when these tooltips are shown using the **ShowValidationOn** option. Options are: 

* Immediately - open on validation failure [default]
* NextFinish - when the Next or Finish buttons are clicked
* Hover - when the mouse hovers over the GuiOption control e.g. text box

Example:
```xml
<GuiOption>
    <ShowValidationOn>Hover</ShowValidationOn>
</GuiOption>
```

In all cases, the control will show a red border when validation has failed. 