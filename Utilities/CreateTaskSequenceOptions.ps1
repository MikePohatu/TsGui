Param (
    [Parameter(Mandatory=$true)][string]$SiteCode,
    [string]$SccmServer='localhost'
)

$wminamespace = "root\sms\site_$($SiteCode)"

Get-WmiObject -ComputerName $SccmServer -namespace $wminamespace -Query "Select * from SMS_TaskSequencePackage" | 
    where-object {$_.TsEnabled -eq "True"} | 
    select-object name, PackageID | 
    foreach {
        write-output "<Option>`r`n`t<Text>$($_.Name)</Text>`r`n`t<Value>$($_.PackageID)</Value>`r`n</Option>"
    }