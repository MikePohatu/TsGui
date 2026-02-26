# Option Lists

## Creating a List

A List can be created in multiple ways:
* Specified in the [\<Lists> block](#the-lists-block) of the configuration
* Automatically when you [add an option to a list](#adding-an-option-to-a-list)
* Specified within a [Set](./Sets.md)



The **File** attribute defines the text file to read in.\
The **Prefix** attribute defines the prefix of a list of variables to create. Each variable will be created with an incrementing number.

## Adding an option to a List
You add an Option to one or more lists by adding the Lists attribute to it with the ID of the desired list. You can add multiple list IDs separated by a comma.

```xml
<GuiOption Type=ComputerName Lists="Lis1,List2">
```

A list will be automatically created when you add an option if it doesn't already exist (i.e. created in the [Lists block](#the-lists-block) or in a [Set](./Sets.md)) with the specified ID and the prefix set to the same as the ID. 

In this way the configuration can be simplified if you only need the default List settings and you don't need to dynamically enable or disable the list using a Set. Just add the relevant options to the List and you're done. 

## Inheritance
The Lists attributes follows the same inhertihance flow as things like [Styles](./Styles.md). You can set the Lists attribute on a [Page](/documentation/Layout.md#page), [Row](/documentation/Layout.md#row), [Column](/documentation/Layout.md#column), or [Container](/documentation/Layout.md#containers), and it will apply to all child options. 

**Note** if the Lists attribute is overridden on a child node, it will replace what was set on the parent in its entirety.  
```xml
<TsGui>
    <Page>
        <Row>
            <Column>
                <Container Lists="AppList">
                    <GuiOption Type="CheckBox" Variable="Microsoft Office 365" />
                    <GuiOption Type="CheckBox" Variable="7-zip" />
                    <GuiOption Type="CheckBox" Variable="Adobe Creative Cloud" />
                </Container>
            </Column>
        </Row>
    </Page>
<TsGui>
    