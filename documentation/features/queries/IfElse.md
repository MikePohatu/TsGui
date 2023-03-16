# If/Else Queries

If/Else queries are how you do conditional logic in TsGui. 

* [Configuration](#configuration)
  * [Source](#source)
  * [RuleSet](#ruleset)
  * [Rules](#rules)
  * [Result](#result)
  * [The ELSE element](#the-else-element)
  * [Full Configuration Example](#full-configuration-example)
  * [Configuration 'short hand'](#configuration-short-hand)
    * [SourceID Attribute](#sourceid-attribute)
    * [SourceWmi Attribute](#sourcewmi-attribute)
    * [ResultSet Attributes](#resultset-attributes)
    * [Short-hand example](#short-hand-example)


## Configuration
An If/Else query contains one or more IF statements, and optionally an ELSE statement. Each IF statement contains 3 items:

* A 'source' i.e. what we are evaluating
* A 'ruleset' containing one or more rules i.e. what rules are we evaluating the source against
* A 'result' if the ruleset finds a match

In PowerShell, these items would look something like: 

```powershell
if ($source -eq 'Rule1' -or $source -eq 'Rule2') { return $result }
```

The **source** and **result** can be built using other TsGui queries which can make this quite powerful. It can also make the config file quite large. To try and combat this, a config [short-hand](#configuration-short-hand) has been created to allow common types of logic to be created using single lines of XML. The short-hand is expanded to the full version at run time. 


### Source
The *source* value can be queried using any TsGui query type e.g. WMI or a value from another GuiOption. Simply enter your \<Query> block into the \<Source> element like you would in \<SetValue>

Whatever the query returns will be the value you will be evaluating i.e. $source from the PowerShell above.

```xml
<Query Type="IfElse">
    <IF>
        <Source>
            <!-- Source of IF condition is the value of another option (option linking feature). -->
            <Query Type="OptionValue">
                <ID Name="TsGui_IsVirtualMachine"/>
            </Query>
        </Source>
        <Ruleset>
            ...
        </Ruleset>
        <Result>
            ...
        </Result>
    </IF>
</Query>
```

### RuleSet
The **Ruleset** element contains one or more **Rule** elements. Each rule in a ruleset will be evaluated either using *AND* logic or *OR* logic as defined by the **Type** attribute.

A Ruleset functions similar to parentheses in something like PowerShell, containing the evaluation logic. For example for the PowerShell below:

```powershell
if ($source -contains "Windows" -and $source -endswith "10") 
{ return $result }
```
The equivalent XML would be:

```xml
<Query Type="IfElse">
    <IF>
        <Source>
            ...
        </Source>
        <!-- Ruleset to compare the Source value against -->
        <Ruleset Type="AND">
            <Rule Type="Contains">Windows</Rule>
            <Rule Type="EndsWith">10</Rule>
        </Ruleset>
        <Result>
            ...
        </Result>
    </IF>
</Query>
```

A Ruleset is also a type of rule. You can use this fact to replicate nested parentheses from something like PowerShell. So for the nested parentheses structure below:

```powershell
if ($source -contains "Windows" -and ($source -endswith "10" -or $source -endswith "8.1")) 
{ return $result }
```
The equivalent XML would be:

```xml
<Query Type="IfElse">
    <IF>
        <Source>
            ...
        </Source>
        <!-- Ruleset to compare the Source value against -->
        <Ruleset Type="AND">
            <Rule Type="Contains">Windows</Rule>
            <Ruleset Type="OR">
                <Rule Type="EndsWith">10</Rule>
                <Rule Type="EndsWith">8.1</Rule>
            </Ruleset>
        </Ruleset>
        <Result>
            ...
        </Result>
    </IF>
</Query>
```

### Rules

Rules are the same as those used for Validation. Valid rule types are listed below:

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


### Result
The *Result* value, can also be queried using any TsGui query type e.g. WMI or a value from another GuiOption. Simply enter your \<Query> block into the \<Source> element like you would in \<SetValue>

Whatever the query returns will be the result to use if the Ruleset matches. This is often a simple \<Value> element (Value is actually a query type, but is just a static value).

```xml
<Query Type="IfElse">
    <IF>
        <Source>
            ...
        </Source>
        <Ruleset>
            ...
        </Ruleset>
        <Result>
            <Value>Some example</Value>
        </Result>
    </IF>
</Query>
```

---


### The ELSE element
An ELSE element has no need for the *source* or *ruleset* elements, so this can be thought of as just having a *result*. 

An \<ELSE> element can contain any type of query (including a \<Value> element) to set the value if none of the preceding \<IF> statements returns a match. 

```xml
<Query Type="IfElse">
    <IF>
        ...
    </IF>
    <IF>
        ...
    </IF>
    <ELSE>
        <Value>Nothing matched!</Value>
    </ELSE>
</Query>
```

### Full Configuration Example

```xml
<Query Type="IfElse">
    <IF>
        <Source>
            <!-- Source of IF condition is the value of another option (option linking feature). -->
            <Query Type="OptionValue">
                <ID Name="TsGui_IsVirtualMachine"/>
            </Query>
        </Source>

        <!-- Ruleset to compare the Source value against -->
        <Ruleset>
            <Rule Type="Equals">TRUE</Rule>
        </Ruleset>

        <!-- The result to return if the ruleset matches -->
        <Result>
            <Value>Virtual Machine</Value>
        </Result>
    </IF>
    <IF>
        <Source>
            <!-- Source of IF condition is the value of another option. -->
            <Query Type="OptionValue">
                <ID Name="TsGui_IsLaptop"/>
            </Query>
        </Source>

        <!-- Ruleset to compare the Source value against -->
        <Ruleset>
            <Rule Type="Equals">TRUE</Rule>
        </Ruleset>

        <!-- The result to return if the ruleset matches -->
        <Result>
            <Value>Laptop/Mobile device</Value>
        </Result>
    </IF>
    <ELSE>
        <Value>Desktop Device</Value>
    </ELSE>
</Query>
```

---
### Configuration 'short hand'
It is clear from the above that XML is not well suited to creating If/Else logic. The query options available in TsGui make this worse, resulting in large config files. 

To combat this, the source, rules, and result can be set in XML attributes to create one-liner IF statements. These are expanded at run-time to the full XML configuration. 

There are limitations when using this, specifically:

* You can only use WMI or OptionValue queries as a source
* WMI queries can't specify the namespace. Only root\CIMV2 can be used
* Advanced WMI query options aren't available e.g. returning multiple properties concatenated together, suffixes and prefixes, math etc. You can only return one property value
* OptionValue queries are created using a 'LinkTo' query, i.e. it will only return the value of the referenced item. Like with WMI, you can't use advanced features
* Only one Ruleset is created. You can use multiple rules, but it is either AND or OR. Nested rulesets aren't supported
  
#### SourceID Attribute
SourceID uses the Option Linking feature to query another element in your TsGui configuration e.g. a GuiOption with an ID set, or one of the variables created by the built-in TsGui HardwareEval. SourceID="SomeID" is equivalent to:
```xml
<Source>
    <Query Type="LinkTo">SomeID</Query>
</Source>
```

#### SourceWmi Attribute
SourceWmi creates a simple Wmi TsGui query. SourceWmi="SomeWql" is equivalent to:
```xml
<Source>
    <Query Type="Wmi">
        <Wql>SomeWql</Wql>
    </Query>
</Source>
```

#### ResultSet Attributes
Any of the valid [rule types](#rules) can be set as an attribute on the IF element. A rule will be created for each one found. Additionally, the **AndOr** attribute can be used to set whether the ruleset uses and AND/OR to evaluate any rules (OR is the default if nothing is set).

*EndsWith="Something" StartsWith="Else" AndOr="AND"*

is equivalent to

```xml
<Ruleset Type="AND">
    <Rule Type="EndsWith">Something</Rule>
    <Rule Type="StartsWith">Else</Rule>
</Ruleset>
```

#### Short-hand example
```xml
<Query Type="IfElse">
    <IF SourceID="TsGui_IsServer" Equals="TRUE" Result="Server" />
    <IF SourceID="TsGui_IsVirtualMachine" Equals="TRUE" Result="VM" />
    <IF SourceID="TsGui_IsLaptop" Equals="TRUE" Result="Lappy" />
    <IF SourceID="TsGui_IsDesktop" Equals="TRUE" Result="DesktopPC" />
    <IF SourceWmi="SELECT SerialNumber FROM Win32_BIOS" StartsWith="System" Result="There isn't a serial number" />
</Query>
```

