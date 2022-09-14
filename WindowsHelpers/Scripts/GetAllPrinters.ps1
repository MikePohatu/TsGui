  Function Get-AllPrinters {

    $allPrinters = @()
    $RegRootPath = "Registry::HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Print\Providers\Client Side Rendering Print Provider\Servers"

    if (Test-Path $RegRootPath) {
        Get-ChildItem $RegRootPath | ForEach-Object { 
            $Reg = Get-ItemProperty -Path "Registry::$($_)"
            $Server = $Reg.PSChildName
            $RegPath = "Registry::$($_)"
        
            if (Test-Path "$($RegPath)\Printers") {
                Get-ChildItem "$($RegPath)\Printers" | ForEach-Object { 
                    $RegSubPath = "Registry::$($_)"
                    $UNC = Get-ItemPropertyValue -Path $RegSubPath -Name "Description"
                    $Location = Get-ItemPropertyValue -Path $RegSubPath -Name "Location"
                    $Driver = Get-ItemPropertyValue -Path $RegSubPath -Name "Printer Driver"
                    $UserName = ""

            

                    try {
                        if ($Location.StartsWith('\Users\')){
                            $sid = $Location.Split('\')[2]
                            $Username = (New-Object System.Security.Principal.SecurityIdentifier($sid)).Translate([System.Security.Principal.NTAccount]).Value
                        }
                    } catch {
                        Write-Warning "Unable to translate SID to username. The account may have been deleted. SID: $($_)"
                    }
            
                    $allPrinters += [PSCustomObject]@{
                        Name = $Location.Split('\')[-1]
                        Username = $Username
                        Path = $UNC
                        Driver = $Driver
                        Server = $Server
                    }
                }
            }
        }        
    }

    Get-Printer | Where-Object {$_.Type -eq "Local"} | ForEach-Object {
        $allPrinters += [PSCustomObject]@{
            Name = $_.Name
            Username =  "SYSTEM" 
            Path = $_.PortName
            Driver = $_.DriverName
            Server = "Local"
        }
    }

    $allPrinters
}

Get-AllPrinters | Sort-Object -Property Name 
