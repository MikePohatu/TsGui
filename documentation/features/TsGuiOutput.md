# TsGui Output

**Contents**
* [Overview](#overview)
* [ConfigMgr Output](#configmgr-output)
* [Registry Output](#registry-output)
  * [DefaultPath](#defaultpath)
  * [HardwareEval](#hardwareeval)


## Overview
TsGui can output to ConfigMgr/SCCM task sequence variables (default), or to registry keys.

To change the output, set the ```OutputType``` attribute at the top of your configuration as below:

**ConfigMgr task sequence variable output**
```xml
<TsGui LiveData="TRUE" Output="ConfigMgr">
```

**Registry output**
```xml
<TsGui LiveData="TRUE" Output="Registry">
```

## ConfigMgr Output
If you don't set an ```Output``` attribute, ConfigMgr task sequence variable output is assumed. If TsGui cannot connect to the ConfigMgr task sequence COM object, it will assume you are running it outside of a task sequence for testing. Test mode will automatically launch so you can check your layout.

## Registry Output
When outputing to registry, the following points should be noted:

* Be aware of the context you are running TsGui as. If attempting to set HKLM keys as a standard user or not elevated you will likely get errors
* Valid keys start with HKEY_CURRENT_USER, HKEY_LOCAL_MACHINE, or HKEY_USERS

When setting registry values, the ```<Variable>``` element from each option (GuiOption or NoUIOption) will be used as the name of the registry value to set. Additionally, each option requires the ```Path``` property to be set which represents the registry key. This can be set as an attribute on the option like this:

```xml
<GuiOption Type="FreeText" Path="HKEY_CURRENT_USER\Software\20Road\Example">
```

Or as an element like this:
```xml
<GuiOption Type="FreeText">
    <Path>"HKEY_CURRENT_USER\Software\20Road\Example"</Path>
```
If set in both as an attribute and as an element by mistake, the attribute will win.

### DefaultPath
To save configuration, you can set a ```DefaultPath``` at the root of your configuration like this:

```xml
<TsGui LiveData="TRUE" Output="Registry">
    <DefaultPath>HKEY_CURRENT_USER\Software\20Road\Example</DefaultPath>
```

Any option without a ```Path``` property set will use this value.

### HardwareEval
The hardware evaluator creates a number of output values. By default, the ```Path``` property will be set to the one set by the ```DefaultValue```. You can override this using the Path attribute on the ```<HardwareEval />``` element in your config as follows:

```xml
<HardwareEval Path="HKEY_CURRENT_USER\Software\20Road\Example\HardwareEval"/>
```