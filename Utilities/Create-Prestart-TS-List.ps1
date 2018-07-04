Param (
    [Parameter(Mandatory=$true)][string]$SiteCode,
    [string]$SccmServer='localhost'
)

[string]$guioption = "<GuiOption Type=`"DropDownList`">" + 
    "`r`n`t<Variable>SMSTSPreferredAdvertID</Variable>" + 
    "`r`n`t<Label>Task Sequence:</Label>"

$wminamespace = "root\sms\site_$($SiteCode)"

Get-WmiObject -ComputerName $SccmServer -namespace $wminamespace -Query "Select * from SMS_TaskSequencePackage" | 
    where-object {$_.TsEnabled -eq "True"} | 
    select-object name, PackageID | 
    foreach {
        $guioption = $guioption + "`r`n`t<Option>`r`n`t`t<Text>$($_.Name)</Text>`r`n`t`t<Value>$($_.PackageID)</Value>`r`n`t</Option>"
    }

$guioption = $guioption + "`r`n</GuiOption>"

$guioption