# Environment Variable queries


## Order of Query

Environment variables can be set in multiple places. The variable name is queried in the following order, and the first valid result is returned:

1. ConfigMgr Task Sequence variables
2. Process Environment Variables
3. System Environment Variables
4. User Environment Variables

## Configuration

```xml
<Query Type="EnvironmentVariable">
    <Variable Name="OSDComputerName"/>
    <Ignore>MININT</Ignore>
    <Ignore>MINWIN</Ignore>
</Query>
```

