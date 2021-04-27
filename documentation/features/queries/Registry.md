# Registry Queries

* [Overview](#overview)
* [Configuration](#configuration)
  * [Root](#root)
  * [Key](#key)
  * [Value](#value)
* [Multi-Value Registry Values](#multi-value-registry-values)

## Overview

## Configuration

```xml
<Query Type="Registry">
    <Root>HKCU</Root>
    <Key>Software\20Road\Example</Key>
    <Value>ValueToQuery</Value>
</Query>
```
For the sections below we will use  *HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\CurrentBuild* as our example registry value we want to query.

### Root
The ```<Root>``` element sets the hive e.g. HKEY_LOCAL_MACHINE. Valid options are:

* HKEY_LOCAL_MACHINE
* HKEY_CURRENT_USER
* HKEY_USERS
* HKEY_CLASSES_ROOT

For our example above, the value will obviously be ```<Root>HKEY_LOCAL_MACHINE</Root>```

### Key
The ```<Key>``` element defines the path to they key under the root. For the example above the key will be:\
 ```<Key>SOFTWARE\Microsoft\Windows NT\CurrentVersion</Key>```

### Value
The ```<Value>``` element defines the specific registry value to query. In our example above, this will be:\
```<Value>CurrentBuild</Value>```

## Multi-Value Registry Values
When you query a REG_MULTI_SZ value, multiple values may be returned. 

If you are using the query in a single item control e.g. a FreeText or InfoBox, the values will be concatenated together. You may wish to configure a separator to insert between the values by inserting a ```<Separator>``` element into your query, for example:


```xml
<Query Type="Registry">
    <Separator>, </Separator>
    ...
```

If you are using a collection item control e.g. a DropDownList or TreeView, each value from the REG_MULTI_SZ will be inserted as an option in the collection. The data from each value will be used as both the Text and Value of the item in the collection.