# TsGui Options

**Contents**
* [Common Configuration Elements](#common-configuration-elements)
  * [Variable](#variable)
  * [SetValue](#setvalue)
  * [ReadOnly](#readonly)
* [GuiOptions](#guioptions)
  * [GuiOption Layout Elements](#guioption-layout-elements)
  * [Label](#label)
  * [HelpText (Tooltip)](#helptext-tooltip)
  * [Example FreeText UI element](#example-freetext-ui-element)
* [GuiOption Types](#guioption-types)
* [NoUI Options](#noui-options)
  * [NoUI Containers](#noui-containers)

TsGui is built with a number of 'options' of various types. These options are used to generate [output](/documentation/features/TsGuiOutput.md) such as Task Sequence Variables. 

These are most commonly UI elements known as [GuiOptions](#guioptions). You can also create items with no associated UI element known as [NoUIOptions](#noui-options).


## Common Configuration Elements
The following are the configuration elements that are common to all option types. 

### Variable
Set the variable name. By default this will configure the Task Sequence variable name. If [outputting to registry](/documentation/features/TsGuiOutput.md#registry-output), this will configure the name of the registry value. 

### SetValue
To set the default value for the option, you use the ```<SetValue>``` element. This can contain one or more [Queries](/documentation/features/Queries.md) that will generate the value. Note that a ```<Value>``` element is actually a type of query that contains a static value.


### ReadOnly
When set to TRUE, this will 'grey out' the control so the user can't edit the value. This is different from using the [Groups & Toggles](/documentation/features/GroupsAndToggles.md) feature which will disable both the control and the label and also change or remove the value of the GuiOption. **ReadOnly** will only disable the control. 

Note that setting this option at a higher level in the tree e.g. Row or Column, will cause this option to flow down to child items in the tree. 


## GuiOptions
A TsGui user interface is built with one or more ```<GuiOption Type="xxx">``` elements. Each different type creates a different type of control. These are added to the layout as outlined in the [Layout](/documentation/Layout.md) documentation.

### GuiOption Layout Elements

It is useful to understand what makes up each GuiOption, especially when applying [Styles](/documentation/features/Styles.md) to alter the layout and appearance of TsGui.

GuiOptions consist of two elements, a **Label** and a **Control** which is different for each type of GuiOption. For example the FreeText GuiOption uses a WPF [TextBox](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/textbox-overview?view=netframeworkdesktop-4.8) for its control, whereas an InfoBox uses a WPF [TextBlock](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/textblock-overview?view=netframeworkdesktop-4.8).

The two elements are contained within a WPF [Grid](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/grid?view=netframeworkdesktop-4.8). By default the Label is on the left, the control on the right. This can be altered using the ```<LabelOnRight>TRUE</LabelOnRight>``` option in the [root style](/documentation/features/Styles.md#style-tree).

A practical example of how these elements might interact when applying styling can be found in the [Styles](/documentation/features/Styles.md#in-more-detail---a-practical-example) documentation. 

---

GuiOptions have the following common configuration elements:

### Label
This will configure the text of the label to be shown to the left or right of the GuiOption control. 

### HelpText (Tooltip) 
Setting a value in the ```<HelpText>``` will configure the text of the tooltip that will appear to the user when they hover over the GuiOption.

### Example FreeText UI element

```xml
<GuiOption Type="FreeText">
  <Variable>Engineer</Variable>
  <Label>Engineer name:</Label>

  <SetValue>
    <Value>Jane Doe</Value>
  </SetValue>
  <HelpText>Enter the user to be using this device</HelpText>
</GuiOption>
```

## GuiOption Types

* [FreeText](FreeText.md) - a simple free text box
* [ComputerName](ComputerName.md)  - a **FreeText** GuiOption that is preconfigured for setting Windows device names in a ConfigMgr task sequence 
* [DropDownList](DropDownList.md)  - a Drop Down List/ComboBox control, either created with static options, or populated by a query
* [CheckBox](CheckBox.md)  - a tick box for TRUE/FALSE type selection
* [InfoBox](InfoBox.md)  - a non-editable text box option for displaying information to the user
* [Heading](Heading.md)  - a text label only, with no separate user control
* [PasswordBox](PasswordBox.md)  - a password box for use with [authentication](/documentation/Authentication/README.md) and SecureString parameters in [PowerShell scripts](/documentation/features/Scripts.md#passwords--securestrings)
* [TreeView](TreeView.md)  - a tree structure
* [Image](Image.md)  - display an image in the UI
* [Timeout](Timeout.md)  - a countdown timer before TsGui is cancelled or auto-finished
* [TrafficLight](ComplianceOptions.md)  - a green/red/yellow dot indicating compliance with a certain value(s)
* [TickCross](ComplianceOptions.md)  - a tick/cross indicating compliance with a certain value(s)
* [ComplianceRefreshButton](ComplianceOptions.md#refreshing-compliance-state)  - a button that will re-check any compliance options (TrafficLight, TickCross) on the page


## NoUI Options

TsGui can also create items that don't create an element in the UI, but otherwise can be used in the same way i.e. create TS variables, use [queries](/documentation/features/Queries.md), etc. 

NoUI options can be used with most TsGui features e.g. [Groups and Toggles](/documentation/features/GroupsAndToggles.md), [Option Linking](/documentation/features/OptionLinking.md), [Scripts](/documentation/features/Scripts.md).

Because there is no UI and therefore no way to interact with these items, the following features are not available or not applicable:

* Compliance
* Validation
* Styles

The NoUI options are contained within the \<NoUI> block of the configuration. 

```xml
<TsGui>
  ...
  <NoUI>
    <NoUIOption>
      <Variable>NoUIOption_1</Variable>
      <SetValue>
        <Value>NoUIOption_1_Val</Value>
      </SetValue>
      <InactiveValue>NoUIOption_1_Inactive</InactiveValue>
    </NoUIOption>
  </NoUI>
<TsGui>
```

### NoUI Containers
If you are using NoUI options with the [Groups and Toggles](/documentation/features/GroupsAndToggles.md) feature, you can contain multiple NoUIOption elements within a \<Container> element. 

You can then set the group in the \<Container> element. The group will apply to all child NoUIOption items.

```xml
<TsGui>
  ...
  <NoUI>
    <Container>
			<Group>Group_Test</Group>
			<NoUIOption Variable="NoUIOption_3" Value="NoUIOption_3_Val"/>
			<NoUIOption>
				<Variable>NoUIOption_4</Variable>
				<SetValue>
					<Value>NoUIOption_4_Val</Value>
				</SetValue>
				<InactiveValue>NoUIOption_4_Inactive</InactiveValue>
			</NoUIOption>
			<NoUIOption Variable="NoUIOption_5" Value="NoUIOption_5_Val" PurgeInactive="TRUE"/>
		</Container>

    <NoUIOption>
      <Variable>NoUIOption_1</Variable>
      <SetValue>
        <Value>NoUIOption_1_Val</Value>
      </SetValue>
      <InactiveValue>NoUIOption_1_Inactive</InactiveValue>
    </NoUIOption>
  </NoUI>
<TsGui>

    ```