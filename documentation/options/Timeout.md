# Timeout

Configuring a Timeout in TsGui will create a time after which TsGui will automatically exit, either finishing normally and thereby creating the associated [output](/documentation/features/TsGuiOutput.md), or behaving as though the Cancel button has been pressed. 

The ```<Timeout>``` element is created at the root of the configuration. The timeout information can also be displayed to the user using ```<GuiOption Type="Timeout">```.


## Configuring the Timeout
The Timeout section sets the time after which TsGui will forcibly close. This will happen either after the specified elapsed time set in ```<After>```, or at the the specified in ```<At>```, whichever comes first. If ```<At>``` has already     passed, TsGui will immediately exit.

The time configured in the ```<At>``` element must be in the format _YYYY-MM-dd HH:mm:ss_, e.g. 2040-01-30 15:30:00.

The following example will exit after 5 mins 30 seconds, or if the system time is after 3:30pm on 30 Jan 2027. 

```xml
<TsGui LiveData="TRUE">
  <Timeout>
    <After>
      <Days>0</Days>
      <Hours>0</Hours>
      <Minutes>5</Minutes>
      <Seconds>30</Seconds>
      <Milliseconds>0</Milliseconds>
    </After>

    <At>2027-01-30 15:30:00</At>
  </Timeout>
</TsGui>
```

### IgnoreValidation
If you set this to FALSE, TsGui will not forcibly close if there are invalid values in the GUI.

### ResetOnActivity
Setting this to TRUE will restart the countdown set in <After> when a mouse click or key press is detected

### CancelOnTimeout
Setting this to TRUE will cancel rather than finish TsGui.

### Timeout GuiOption
To show the status of the timeout to the user, add a Timeout GuiOption to your configuration.

If you set ShowElapsed to FALSE the specific date set in <At> will be displayed

```xml
<GuiOption Type="Timeout">
    <Label>Timeout: </Label>
    <ShowElapsed>TRUE</ShowElapsed>
</GuiOption>
```


## Full Example Configuration
```xml
<TsGui LiveData="TRUE">
  
  <!-- Timeout section sets time after which TsGui will forcibly close.
    This will happen either after the specified elapsed time in <After>, 
    or at the the specified in <At>, whichever comes first. If <At> has already 
    passed, TsGui will immediately exit-->
  <Timeout>
    <After>
      <Days>0</Days>
      <Hours>0</Hours>
      <Minutes>5</Minutes>
      <Seconds>0</Seconds>
      <Milliseconds>0</Milliseconds>
    </After>

    <At>2040-01-30 15:30:00</At> <!-- This must be in the format YYYY-MM-dd HH:mm:ss -->
    
    <IgnoreValidation>TRUE</IgnoreValidation> <!-- If you set this to FALSE, TsGui will not close if there are invalid values in the gui -->
    <ResetOnActivity>FALSE</ResetOnActivity> <!-- Setting this to TRUE will restart the countdown set in <After> when a mouse click or key press is detected -->
    <CancelOnTimeout>FALSE</CancelOnTimeout> <!-- Setting this to TRUE will cancel rather than finish TsGui -->
  </Timeout>

  <Page>
    <Row>
      <Column>
        <GuiOption Type="ComputerName" />
  
        <!-- The Timeout GuiOption will display the timeout information from the <Timeout> section above. -->
        <GuiOption Type="Timeout">
          <Label>Timeout: </Label>
          <ShowElapsed>TRUE</ShowElapsed> <!-- If you set ShowElapsed to FALSE the specific date set in <At> will be displayed -->
        </GuiOption>
      </Column>
    </Row>
  </Page>
</TsGui>
```