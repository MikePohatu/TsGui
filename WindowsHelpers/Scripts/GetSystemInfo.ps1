
Function IsRebootPending {
    $existkeys = @(
        "HKLM:\Software\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootPending",
        "HKLM:\Software\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootInProgress",
        "HKLM:\Software\Microsoft\Windows\CurrentVersion\Component Based Servicing\PackagesPending",
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\PostRebootReporting",
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired",
        "HKLM:\SOFTWARE\Microsoft\ServerManager\CurrentRebootAttempts"
    )

    ForEach ($key in $existkeys) {
    
        #write-host "Key: $_"
        if (Test-Path $key) { 
            Write-Information "Reboot pending, key: $key"
            return $true
        }
    }

    $existvalues = @( @("HKLM:\SYSTEM\CurrentControlSet\Control\Session Manager", "PendingFileRenameOperations"),
        @("HKLM:\SYSTEM\CurrentControlSet\Control\Session Manager", "PendingFileRenameOperations2"),
        @("HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", "DVDRebootSignal"),
        @("HKLM:\SYSTEM\CurrentControlSet\Services\Netlogon", "JoinDomain"),
        @("HKLM:\SYSTEM\CurrentControlSet\Services\Netlogon", "AvoidSpnSet")
        )

    ForEach ($keyval in $existvalues) {
        $Key = $keyval[0]
        $Value = $keyval[1]

        #write-host "Key: $Key"
        #write-host "Value: $Value"

        try {
            Get-ItemProperty -Path $Key | Select-Object -ExpandProperty $Value -ErrorAction Stop | Out-Null
            Write-Information "Reboot pending, key: $Key\$Value"
            return $true
        }
        catch { }
    }

    return $false
}

Function Get-MemorySize {
    $memsize = 0
    Get-WmiObject -Query 'SELECT Capacity FROM Win32_PhysicalMemory' | ForEach-Object {
        $memsize += $_.Capacity
    }
    return $memsize
}

function Get-ConfigMgrClientStatus {
    $client =  Get-WmiObject -NameSpace Root\CCM -Class Sms_Client -Property ClientVersion -ErrorAction SilentlyContinue
    if ($client) {
        $service = Get-Service -Name 'ccmexec' -ErrorAction SilentlyContinue
        if ($service) {
            return $service.Status.ToString()
        } else {
            return 'ServiceError'
        }
    }
    else {
        return 'NotInstalled'
    }
}

Function Get-PowerInfo {
    $batt = Get-WmiObject -Query 'SELECT * FROM Win32_Battery'
    #BatteryStatus -eq means power is plugged in. Null means no battery
    if ($batt.BatteryStatus) {
        $charge = "$($batt.EstimatedChargeRemaining)%"
        if ($batt.Status -eq $null) {
            $status = "Unknown"
        } else {
            $status = $batt.Status
        }

        if ($batt.BatteryStatus -eq 2) {
            $connected = $true
        }
        else {
            $connected = $false
        }
    }
    else {
        $charge = "N/A"
        $status = "N/A"
        $connected = $true
    }

    return @{
        Connected = $connected
        Status = $status
        Charge = $charge
    }
}

function Get-ProductType {
    $product = (Get-WmiObject -Query 'SELECT ProductType FROM Win32_OperatingSystem').ProductType
    if ($product -eq 1) { return "Client" }
    elseif ($product -eq 2) { return "Domain Controller" }
    elseif ($product -eq 3) { return "Server" }
}

Function Get-LoggedOnUsers {
    $ErrorActionPreference= 'silentlycontinue'
    try {
        $users = ((quser) -replace '^>', '' -replace '\s{20}', ',' -replace '\s{2,}', ',') | ConvertFrom-Csv
    }
    catch {
        return @{
            activeUsers = ""
            disconnectedUsers = ""
            consoleUser = ""
        }
    }

    $active = $users | Where {$_.STATE -eq 'Active'}
    $disconnected = $users | Where {$_.STATE -eq 'Disc'}
    $consoleuser = $users | Where {$_.SESSIONNAME -eq 'Console'}

    $activeUsersString = [string]::Empty
    $disconnectedUsersString = [string]::Empty
    $consoleUserString = [string]::Empty

    if ($active -and $active.USERNAME) {
        $activeUsersString =  [string]::Join(', ', $active.USERNAME)
    }

    if ($disconnected -and $disconnected.USERNAME) {
        $disconnectedUsersString =  [string]::Join(', ', $disconnected.USERNAME)
    }

    if ($consoleuser) {
        $consoleUserString = $consoleuser.USERNAME
    }

    return @{
        activeUsers = $activeUsersString
        disconnectedUsers = $disconnectedUsersString
        consoleUser = $consoleUserString
    }
}

function Get-BitLockerInfo {
    try {
        $bitLocker = Get-BitLockerVolume -ErrorAction SilentlyContinue -MountPoint $env:SystemDrive | Select MountPoint, VolumeStatus, EncryptionPercentage, ProtectionStatus
    } catch {
        Write-Information "BitLocker not available on device."
    }
    
    if ($bitLocker) {
        return @{
            percent = "$($bitLocker.EncryptionPercentage)%"
            status = $bitLocker.ProtectionStatus
        }
    } else {
        return @{
            percent = "0%"
            status = "N/A"
        }
    }
    
}

$ipv4s = Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.InterfaceIndex -ne 1} | Select IPAddress
$ipv6s = Get-NetIPAddress -AddressFamily IPv6 | Where-Object {$_.InterfaceIndex -ne 1} | Select IPAddress
$compSys = Get-WmiObject -Query 'SELECT * FROM Win32_ComputerSystem' | Select Manufacturer, Model, SystemType, TotalPhysicalMemory
$compOS = Get-WmiObject -Query 'SELECT * FROM Win32_OperatingSystem' | Select BuildNumber, Caption, LastBootUpTime, OSArchitecture, Version, WindowsDirectory
$compBIOS = Get-WmiObject -Query 'SELECT * FROM Win32_BIOS' | Select SerialNumber, SMBIOSBIOSVersion
$memory = [Math]::Round($compSys.TotalPhysicalMemory /1024 /1024 )
$users = Get-LoggedOnUsers
$power = Get-PowerInfo
$bitLocker = Get-BitLockerInfo
$ipv4Addresses = ""
if ($ipv4s.IPAddress) { $ipv4Addresses = [string]::Join(", ", $ipv4s.IPAddress) }
$ipv6Addresses = ""
if ($ipv6s.IPAddress) { $ipv6Addresses = [string]::Join(", ", $ipv6s.IPAddress) }

$systemInfo = @{
    EncryptionStatus = $bitLocker.status
    EncryptionPercentage = $bitLocker.percent
    PendingReboot = IsRebootPending
    Type = $compOS.OSArchitecture
    MemorySize = "$memory MB"
    IPv4Addresses = $ipv4Addresses
    IPv6Addresses = $ipv6Addresses
    ConsoleUser = $users.consoleUser
    ActiveUsers = $users.activeUsers
    DisconnectedUsers = $users.disconnectedUsers
    Model = $compSys.Model
    BiosVersion = $compBIOS.SMBIOSBIOSVersion
    Serial = $compBIOS.SerialNumber
    Manufacturer = $compSys.Manufacturer
    Architecture = $compOS.OSArchitecture
    Version = $compOS.Version
    Build = $compOS.BuildNumber
    LastBoot = ([System.Management.ManagementDateTimeConverter]::ToDateTime($compOS.LastBootUpTime).ToUniversalTime()).ToString("dd-MMM-yyyy HH:mm:ss")
    OS = $compOS.Caption
    Name = $env:COMPUTERNAME
    ConfigMgrClientStatus = Get-ConfigMgrClientStatus
    BatteryCharge = $power.Charge
    BatteryStatus = $power.Status
    PowerConnected = $power.Connected
}

$systemInfo