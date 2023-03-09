# PasswordBox

The PasswordBox type is used with [Authentication](/documentation/Authentication/README.md) configuration, but can also be used standalone, saving the password into a task sequence variable. 

**Note:** This is not recommended from a security point of view. TS variables can be saved in the _smsts.log_ file which can be viewed after the task sequence has finished. 

To enable this function, set the **ExposePassword** attribute on the PasswordBox, and make sure to assign a variable. 


```xml
<GuiOption Type="PasswordBox" ExposePassword="TRUE">
    <Variable>PW</Variable>
    <Label>Password:</Label>
    <NoPasswordMessage>Password cannot be empty</NoPasswordMessage>
    <AllowEmpty>FALSE</AllowEmpty>
</GuiOption>
```