# Compliance (pre-flight checks)

Documentation is on the to-do list for this feature. For usage information, see the how-to video on [YouTube](https://youtu.be/RiOLjTdrd1U) or the [example configuration file](/Config_Examples/Config_Compliance.xml).

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