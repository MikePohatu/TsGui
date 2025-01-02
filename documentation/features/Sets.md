# Sets

* [Overview](#overview)
* [Config Overview](#config-overview)
* [Importing from Files](#importing-from-files)
  * [File Paths](#file-paths)
  * [Dynamic List From Text File](#dynamic-list-from-text-file)
    * [Example file](#example-file)
  * [Static List From Text File](#static-list-from-text-file)
    * [File format](#file-format)
    * [Example file](#example-file-1)


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

## Importing from Files

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
