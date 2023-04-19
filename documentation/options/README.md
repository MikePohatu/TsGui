# TsGui Options

**Contents**
* [GuiOptions](#guioptions)
  * [GuiOption Types](#guioption-types)
* [NoUI Options](#noui-options)


## GuiOptions
A TsGui user interface is built with one or more ```<GuiOption Type="xxx">``` elements. Each different type creates a different type of control.  

### GuiOption Types

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
