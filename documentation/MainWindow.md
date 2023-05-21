# Configuring main settings

The following section outlines the 'top level' settings for TsGui. These are things like the navigation buttons, border, hardware evaluation etc. When configuring the options below, keep the [configuration guidelines](/documentation/ConfigGuidelines.md#elements-vs-attributes) in mind, specifically the part about [elements vs attributes](/documentation/ConfigGuidelines.md#elements-vs-attributes). This may help keep you configurations smaller.

* [Buttons](#buttons)
* [HardwareEval](#hardwareeval)
* [Border](#border)
* [Footer](#footer)
* [Heading](#heading)


## Buttons
The Buttons configuration allows you to change the text of the navigation buttons. It  is set at the root level of the configuration
```xml
<TsGui>
    <Buttons>
        <Next>Next</Next>
        <Back>Back</Back>
        <Cancel>Cancel</Cancel>
        <Finish>Finish</Finish>
        <HideCancel>FALSE</HideCancel>
        <!-- Set this to TRUE to remove the cancel button from the GUI. -->
    </Buttons>
...
</TsGui>
```

## HardwareEval
The hardware evaluator collects some system information when TsGui launches and automatically creates task sequence variables. When using the [Option Linking](/documentation/features/OptionLinking.md) feature, these variables can be referenced with the same name for the ID.

The variables collected are:

* TsGui_IsLaptop
* TsGui_IsDesktop
* TsGui_IsServer
* TsGui_IsVirtualMachine
* TsGui_IsConvertible
* TsGui_IsDetachable
* TsGui_IsTablet
* TsGui_IPv4
* TsGui_IPv6
* TsGui_DefaultGateway4
* TsGui_DefaultGateway6
* TsGui_IPSubnetMask4
* TsGui_IPSubnetMask6
* TsGui_DHCPServer

Enable the hardware evaluator by added a \<HardwareEval /> element to your configuration
```xml
<TsGui>
    <HardwareEval />
...
</TsGui>
```




## Border
The \<Border> element defines the border around the main TsGui window. This is set in the root of the configuration and can be configured as follows  

```xml
<TsGui>
    <Border>
        <Color>Black</Color>
        <Thickness>3</Thickness>
    </Border>
...
</TsGui>
```

## Footer
The \<Footer> element configures the lower bar text that appears in TsGui. The default _'Powered by TsGui - www.20road.com'_ message can be changed to include your organisation name or branding if desired.

```xml
<TsGui>
    <Footer>
        <Text>Powered by TsGui - www.20road.com</Text>
        <Height>17</Height>
        <HAlign>right</HAlign>
    </Footer>
...
</TsGui>
```

## Heading
The \<Heading> element configures the default header shown in the TsGui window. This includes the main text (\<Title>), sub text (\<Text>), background color (\<Bg-Color>) and font color (\<Font-Color>).


```xml
<TsGui>
    <Heading>
        <Title>Title Text</Title>
        <Text>Text</Text>
        <Bg-Color>#FF006AD4</Bg-Color>
        <Font-Color>#FFFFFF</Font-Color>
    </Heading>
...
</TsGui>
```

Note that if desired, the child elements of the \<Heading> can be replaced with a [full layout](/documentation//Layout.md) like that from the main window e.g.

```xml
<TsGui>
    <Heading>
        <Row>
            <Column>
                <GuiOption>
                    ...
                </GuiOption>
            </Column>
        </Row>
    </Heading>
...
</TsGui>
```

This will give you more flexibility and control than the built in default layout. 