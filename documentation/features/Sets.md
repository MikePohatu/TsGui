# Sets

* [Overview](#overview)
* [Config Overview](#config-overview)
* [Enabling a Set](#enabling-a-set)
  * [Using Groups](#using-groups)
  * [Using Queries](#using-queries)
* [Importing from Files](#importing-from-files)
  * [File Paths](#file-paths)
  * [Dynamic List From Text File](#dynamic-list-from-text-file)
    * [Example file](#example-file)
  * [Static List From Text File](#static-list-from-text-file)
    * [File format](#file-format)
    * [Example file](#example-file-1)
* [Testing](#testing)


## Overview
The 'Sets' feature allows the creation of multiple variables using a simple list format in a text file, rather than having to create individual GuiOptions or NoUiOptions using XML. 

You can import from text files to simplify your configuration files. 

This feature requires TsGui version 2.1.0.0 or above.


## Config Overview

The **\<Sets>** element can contain one or more sets, each defined in a **\<Set>** element. The \<Set> configuration is outlined in more detail in the following sections. 

```xml
<TsGui>
    ...
    <Sets>
        <Set ID="Example set">
            <!-- Configure whether the Set is enabled based on a query -->
            <Enabled>
                <Query Type="IfElse">
                    <IF SourceID="TsGui_IsServer" Equals="TRUE" Result="TRUE" />
                </Query>
            </Enabled>

            <!-- Configure whether the Set is enabled based on a group -->
            <Group>TestGroup</Group>
            
            <!-- Create a static list of variables imported from file -->
            <List File="file.txt" Prefix="AppNamePrefix" />

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

## Enabling a Set
Sets can be enabled and disabled based on options set elsewhere in TsGui using either the [Groups and Toggles](./GroupsAndToggles.md) feature or by using a TsGui [Query](./Queries.md). 

If neither of these options are set, the set will always be enabled. 

If both options are set, both need to be enabled for the set to be enabled. 

### Using Groups
Sets can be enabled and disabled using the [Groups](GroupsAndToggles.md) feature the same way GuiOptions and NoUIOptions are. 

If you add a \<Group\> element to your set, the Set will be enabled when the group is enabled. When the group is disabled, no variables from the set will be created i.e. the [InactiveValue](GroupsAndToggles.md#inactivevalue) option doesn't apply to Sets.

```xml
<Set>
    <Group>ExampleGroup</Group>
    ...
</Set>
```

### Using Queries

To use a [TsGui Query](./Queries.md) to enable the set, add an \<Enabled\> element to your set. Within the Enabled element you can add one or more queries. Each query will be processed in order. If any of the queries return the value **TRUE** then the set will be enabled. 

Usually this option would be used an [IfElse](./queries/IfElse.md#configuration-short-hand) query with the [Option Linking](./OptionLinking.md) feature. This will enable the set based on the value of another GuiOption e.g. a [DropDownList](/documentation/options/DropDownList.md).

```xml
<Set>
    <Enabled>
        <Query Type="IfElse">
            <IF SourceID="TsGui_IsServer" Equals="TRUE" Result="TRUE" />
        </Query>
    </Enabled>
    ...
</Set>
```

## Importing from Files
Lists of variables/values can be imported from text file to simplify creating a list variables. Text files are read and imported when TsGui completes e.g. when the _Finish_ button is pressed. This is done so that only the files needed for the enabled sets are imported. 


### File Paths
File paths can specified be to normal file/UNC paths, or to web URLs. TsGui will treat any path starting with http:// or https:// as a web URL, otherwise a normal file/UNC path.

When specifying file/UNC paths, if a full path isn't specified, the following lookup order will apply:

1. Search for the file relative to the _Files_ directory next to TsGui.exe.
2. Search for the file relative to TsGui.exe.
3. Search the current working directory.

### Dynamic List From Text File
This option is designed to be used with the "[Install applications according to dynamic variable list](https://learn.microsoft.com/en-us/mem/configmgr/osd/understand/task-sequence-steps#BKMK_InstallApplication)" task sequence option in ConfigMgr. To use this mode set an **Prefix** attribute to specify the prefix for the created variables. 

The **File** attribute defines the text file to read in.\
The **Prefix** attribute defines the prefix of a list of variables to create. Each variable will be created with an incrementing two digit number.

Each line in the text file contains the value of variables to create. 

**Example XML**
```xml
<Set>
    <List File="file.txt" Prefix="VariablePrefix" />
</Set>
```


#### Example file
When **Prefix="AppList"** is set and the text file contains the following:

```
MS Office
MS Project
7-zip
```

The following variables will be created:\
_AppList01=MS Office\
AppList02=MS Project\
AppList03=7-zip_



### Static List From Text File
You can import a .txt file to set specific variables in a _Variable_Name=Variable_Value_ format. Don't set a _Prefix_ attribute to use this mode. 

```xml
<Set>
    <List File="file.txt" />
</Set>
```

#### File format
Each line contains **Variable_Name=Variable_Value** e.g. _OSDRegisteredOrgName=Contoso_. The first equal sign is used to separate the name and value. Don't use an equal sign in your name.


#### Example file
```
OSDRegisteredOrgName=Contoso
OSDTimeZone=New Zealand Standard Time
OSDDomainOUName=LDAP://OU=Project Office,DC=Workstations,DC=contoso,DC=com
```

## Testing

Sets are displayed in the _Sets_ area in the [LiveData](/documentation/TestMode.md#the-livedata-window) window. To make identifying multiple sets easier in the testing window, set an ID attribute on the set. 

```xml
<Set ID="ExampleID">
```

The ID isn't usable with the [Option Linking](./OptionLinking.md) feature.