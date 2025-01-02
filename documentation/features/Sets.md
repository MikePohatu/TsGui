# Sets

* [Overview](#overview)
* [Config Overview](#config-overview)
* [Importing from Files](#importing-from-files)
  * [File Paths](#file-paths)
  * [Dynamic List From Text File](#dynamic-list-from-text-file)
    * [Example](#example)
  * [Ini Files](#ini-files)
    * [Ini file format](#ini-file-format)
    * [Example ini file](#example-ini-file)


## Overview
The 'Sets' feature allows the creation of multiple variables in a simple list format, rather than having to create individual GuiOptions or NoUiOptions. 

You can import from text files to simplify and your configuration files. 

## Config Overview


```xml
<TsGui>
    ...
    <Sets>
        <Set>
            <!-- Configure whether the Set is enabled based on a query -->
            <Enabled>
                <Query Type="IfElse">
                    <IF SourceID="TsGui_IsServer" Equals="TRUE" Result="TRUE" />
                </Query>
            </Enabled>

            <!-- Configure whether the Set is enabled based on a group -->
            <Group>TestGroup</Group>

            <!-- Create variables based on an imported ini file -->
            <List File="file.ini" />
            
            <!-- Create a dynamic list of variables imported from file -->
            <List File="file.txt" Prefix="AppNamePrefix" />

            <!-- Manually create variables in the set -->
            <Variable Name="OSDRegisteredOrgName" Value="Contoso" />
            <Variable Name="OSDTimeZone" Value="New Zealand Standard Time" />
            <Variable Name="OSDDomainOUName">
                <Value>LDAP://OU=MyOu,DC=Servers,DC=contoso,DC=com</Value>
            </Variable>
        </Set>
    </Sets>
</TsGui>
```

## Importing from Files

### File Paths
File paths can specified be to normal file/UNC paths, or to web URLs. TsGui will treat any path starting with http:// or https:// as a web URL, otherwise a normal file/UNC path.

When specifying file/UNC paths, if a full path isn't specified, the following lookup order will apply:

1. Search for the file relative to the _Files_ directory next to TsGui.exe.
2. Search for the file relative to TsGui.exe.

### Dynamic List From Text File
This option is designed to be used with the "[Install applications according to dynamic variable list](https://learn.microsoft.com/en-us/mem/configmgr/osd/understand/task-sequence-steps#BKMK_InstallApplication)" task sequence option in ConfigMgr. 

**Example XML**
```xml
<Set>
    <List File="file.txt" Variable="AppNamePrefix" />
</Set>
```

The **File** attribute defines the text file to read in.\
The **Variable** attribute defines the prefix of a list of variables to create. Each variable will be created with an incrementing two digit number.

Each line in the text file contains the value of variables to create. 

#### Example
When **Variable="AppList"** is set and a text file containing the following:
```
MS Office
MS Project
7-zip
```

The following variables will be created:\
_AppList01=MS Office\
AppList02=MS Project\
AppList03=7-zip_



### Ini Files
You can import a .ini file to set individual variables and a dynamic list of variables.


```xml
<Set>
    <List File="file.ini" />
</Set>
```

#### Ini file format
Entries under the **[Variables]** section are set using the format **VariableName=VariableValue** e.g. _OSDRegisteredOrgName=Contoso_

When the Value contains an '=', only the first one is evaluated when separating the variable name and value.

For every other section in the file, the section name will be treated as the variable prefix like the [Dynamic List From Text File](#dynamic-list-from-text-file) option. Each line within the section will be used as a value. For example to create these variables:

AppList01=MS Office\
AppList02=MS Project\
AppList03=7-zip

You would create the following in the ini file:\
_[AppList]\
MS Office\
MS Project\
7-zip_


#### Example ini file
```ini
[Variables]
OSDRegisteredOrgName=Contoso
OSDTimeZone=New Zealand Standard Time
OSDDomainOUName=LDAP://OU=Project Office,DC=Workstations,DC=contoso,DC=com</Value

[AppsList]
Microsoft Office
7-zip
Microsoft Project
Microsoft Visio
```
