$wd = $PSScriptRoot
$testConfigs = Get-ChildItem "$wd" -filter "*.xml"

$runCompare = $false
$prompt = Read-Host "Do you want to run comparative tests? (y/n)"
if ($prompt -eq 'y') { $runCompare = $true }

$prompt = Read-Host "Which test number do you want to start with?"
if ($prompt -match "^[\d\.]+$") { 
    $testStart = $prompt 
    if ($testStart.Length -lt 2) { $testStart = "0$($testStart)"}
}

#dummy proc object
if (-not $compare) { 
    $proc2 = New-Object PSObject -Property @{ HasExited = $true} 
}

$testExe = "$($wd)\Test\TsGui.exe"
$testExeVer = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($testExe).FileVersion
if ($runCompare) { 
    $refExe = "$($wd)\Reference\TsGui.exe"
    $refExeVer = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($refExe).FileVersion
}

$startTests = $false
foreach ($configFile in $testConfigs) {
    if (-not $startTests -and $configFile.Name.Contains("Config.$($testStart)")) { $startTests = $true }
    
    if ($startTests) {
        Write-Host "Starting test: $($configFile.Name)" -NoNewline

        $tsguiArgs = @("-config","$($configFile.FullName)")

        Write-Host " | test ($testExeVer)" -NoNewline
        $proc1 = Start-Process -FilePath $testExe -ArgumentList $tsguiArgs -PassThru
        if ($runCompare) {
            Start-Sleep -Seconds 3
            Write-Host " | reference ($refExeVer)" -NoNewline -ForegroundColor Green
            $proc2 = Start-Process -FilePath $refExe -ArgumentList $tsguiArgs -PassThru
        }

        Start-Sleep -Seconds 1
        while ($proc1.HasExited -eq $false -or $proc2.HasExited -eq $false) {
            Start-Sleep -Seconds 1
        }
        Write-Host " |"
    }
}