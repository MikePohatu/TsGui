# Value-Only Queries

A Value query is simply a static value. It is treated as a query internally by TsGui so it can be used interchangeably with any other query type. 

## Configuration

To use a Value type, simply set your value inside the \<Query Type="Value"> element. You can also simply use a \<Value> element to have the same effect. 

```xml
<Query Type="Value">Some value</Query>
<Value>Some other value</Value>


```