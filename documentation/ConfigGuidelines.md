# Configuration Guidelines

The following are some rules, guidelines, and helpers when building your configuration

* [Elements vs Attributes](#elements-vs-attributes)


## Elements vs Attributes
The TsGui configuration loader allows you to use either a child XML element or an XML attribute in many cases. This is intended to allow for 'one liner' configuration to keep your config files more succint. 

For example in the following configuration:

```xml
<Option>
    <Text>Desktop</Text>
    <Value>Desk</Value>
</Option>
```

The Text and Value child elements an be replaced with attributes like this:
```xml
<Option Text="Desktop" Value="Desk" />
```

A child element can only be replaced with an attribute if:
* Only one child element with that name can be set e.g. \<Option> elements in a DropDownList couldn't be replaced with attributes. Only one XML attribute with a given name can be set.
* The child element itself requires no child elements itself
* The child element itself requires no attributes