# File Lists

* [Importing from Files](#importing-from-files)
  * [File Paths](#file-paths)
  * [Dynamic List From Text File](#dynamic-list-from-text-file)
    * [Example file](#example-file)
  * [Static List From Text File](#static-list-from-text-file)
    * [File format](#file-format)
    * [Example file](#example-file-1)


## Importing from Files
Lists of variables/values can be imported from text file to simplify creating a list variables. Text files are read and imported when TsGui completes e.g. when the _Finish_ button is pressed. This is done so that only the files needed for the enabled [Sets](./Sets.md) are imported. 


### File Paths
File paths can specified be to normal file/UNC paths, or to web URLs. TsGui will treat any path starting with http:// or https:// as a web URL, otherwise a normal file/UNC path.

When specifying file/UNC paths, if a full path isn't specified, the following lookup order will apply:

1. Search for the file relative to the _Files_ directory next to TsGui.exe.
2. Search for the file relative to TsGui.exe.
3. Search the current working directory.

### Dynamic List From Text File
This option is designed to be used with the "[Install applications according to dynamic variable list](https://learn.microsoft.com/en-us/mem/configmgr/osd/understand/task-sequence-steps#BKMK_InstallApplication)" task sequence option in ConfigMgr. To use this mode set an **Prefix** attribute to specify the prefix for the created variables. 

The **File** attribute defines the text file to read in.\
The **Prefix** attribute defines the prefix of a list of variables to create. Each variable will be created with an incrementing number.

Each line in the text file contains the value of variables to create. 

**Example XML**
```xml
<List File="file.txt" Prefix="VariablePrefix" />
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
<List File="file.txt" />
```

#### File format
Each line contains **Variable_Name=Variable_Value** e.g. _OSDRegisteredOrgName=Contoso_. The first equal sign is used to separate the name and value. Don't use an equal sign in your name.


#### Example file
```
OSDRegisteredOrgName=Contoso
OSDTimeZone=New Zealand Standard Time
OSDDomainOUName=LDAP://OU=Project Office,DC=Workstations,DC=contoso,DC=com
```