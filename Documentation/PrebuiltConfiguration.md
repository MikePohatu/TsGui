# Prebuilt Configuration Options

In order to reduce configuration complexity and learning curve, certain common pieces of configuratio can be replaced by some configuration 'short hand'. 

These configuration pieces can be enabled by using the **Prebuilt="XXX"** attribute on the appropriate element. 

* [DiskIndex](#DiskIndex) - list of hard disks for **Format and Partition Hard Disk** step in Task Sequence
* [PowerConnected](#PowerConnected) - is power connected to the device

<br>

---
<br><br>


### DiskIndex
The **DiskIndex** prebuilt is designed to create a list of hard disks. The selected value is used to set a value for the SCCM Task Sequence variable [OSDDiskIndex](https://docs.microsoft.com/en-us/configmgr/osd/understand/task-sequence-variables#OSDDiskIndex).

Note that this variable is used by the **Format and Partition Hard Disk** step, not by the **Install Operating System**. You need to save the partition to a variable for use by the Install Operating System step.

* [Example](#DiskIndex-Example)
* [Expanded Configuration](#DiskIndex-Expanded-Configuration)


#### Supported locations

* DropDownList GuiOption


#### DiskIndex Example
```
<GuiOption Type="DropDownList" Prebuilt="DiskIndex" />
```


#### DiskIndex Expanded Configuration
```
<GuiOption Type="DropDownList">
    <Variable>OSDDiskIndex</Variable>
    <Label>Disk</Label>
    <SetValue>
        <Value>0</Value>
    </SetValue>
    
    <Query Type="Wmi">
        <Wql>select Index,Caption,Size FROM Win32_DiskDrive where MediaType='Fixed hard disk media'</Wql>
        <NameSpace>root\CIMV2</NameSpace>
        <Property Name="Index"/>
        <Property Name="Index">
            <Prefix>ID: </Prefix>
        </Property>
        <Property Name="Size">
            <Calculate DecimalPlaces="2">VALUE/1073741824</Calculate>
            <Prefix></Prefix>
            <Append>GB</Append>
        </Property>
        <Property Name="Caption"/>					
        <Separator>, </Separator>
    </Query>
</GuiOption>
```
---
<br><br>



### PowerConnected
The PowerConnected prebuilt is a compliance option to indicate whether power is supplied to the device. 

* [Example](#PowerConnected-Example)
* [Expanded Configuration](#PowerConnected-Expanded-Configuration)

#### Supported locations

* TrafficLight GuiOption
* TickCross GuiOption

#### PowerConnected Example
```
<GuiOption Type="TrafficLight" Prebuilt="PowerConnected" />
```

#### PowerConnected Expanded Configuration
```
<GuiOption Type="TrafficLight">
    <Variable>Compliance_PowerStatus</Variable>
    <Label>Power connection</Label>
    <ShowComplianceValue>FALSE</ShowComplianceValue>
    <SetValue>
        <Query Type="Wmi">
            <Wql>SELECT BatteryStatus FROM Win32_Battery</Wql>
            <Property Name="BatteryStatus"/>	
            <Separator></Separator>
        </Query>
    </SetValue>
    <Compliance>
        <Message>Please connect the power</Message>		
        <DefaultState>Warning</DefaultState>
        <OK>
            <Rule Type="Contains">2</Rule>
            <Rule Type="Equals">*NULL</Rule>
        </OK>
    </Compliance>
</GuiOption>
```
---
<br><br>

### WifiDisconnected
The WifiDisconnected prebuilt is a compliance option to indicate whether WiFi is disconnected from the device. 

* [Example](#WifiDisconnected-Example)
* [Expanded Configuration](#WifiDisconnected-Expanded-Configuration)

#### Supported locations

* TrafficLight GuiOption
* TickCross GuiOption

#### WifiDisconnected Example
```
<GuiOption Type="TrafficLight" Prebuilt="WifiDisconnected" />
```

#### WifiDisconnected Expanded Configuration

```
<GuiOption Type="TrafficLight">
    <Variable>Compliance_WifiStatus</Variable>
    <Label>WiFi connection</Label>
    <ShowComplianceValue>FALSE</ShowComplianceValue>
    <SetValue>
        <Query Type="Wmi">
            <Wql>SELECT * FROM Win32_NetworkAdapter WHERE (AdapterType="Ethernet 802.3") AND (NetConnectionStatus = 2)</Wql>
            <Property Name ="Name"/>
            <Separator></Separator>
        </Query>
    </SetValue>
    <Compliance>
        <Message>Please disconnect the wifi</Message>	   
        <DefaultState>Warning</DefaultState>
        <Invalid>
            <Rule Type="Contains">Wireless</Rule>
            <Rule Type="Contains">Wifi</Rule>
            <Rule Type="Contains">WLAN</Rule>
        </Invalid>		 
    </Compliance>
</GuiOption>
```



