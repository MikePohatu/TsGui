$ErrorActionPreference= 'silentlycontinue'
try {
    ((quser) -replace '^>', '' -replace '\s{20}', ',' -replace '\s{2,}', ',' -replace 
    'IDLE TIME', 'IdleTime' -replace 'USERNAME', 'UserName' -replace 'SESSIONNAME', 'SessionName' -replace 'STATE', 'State' -replace 'LOGON TIME', 'LogonTime') | ConvertFrom-Csv
}
catch {
    return $null
}
