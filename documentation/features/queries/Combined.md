# Combined Queries

A Combined query concatenates one or more child queries together. 

## Configuration
To use a Combined query, simply create queries inside the \<Query Type="Combined"> element.

```xml
<Query Type="Combined">
    <Value>D-</Value>
    <Query Type="Wmi">
        <Wql>Select Model from Win32_ComputerSystem</Wql>
    </Query>
</Query>
```

If the model of the device was 'EliteDesk 800 G6', output of the query would be 'D-EliteDesk 800 G6'. 