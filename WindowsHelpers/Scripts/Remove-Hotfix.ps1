
Function Remove-Hotfix {
    Param (
        [Parameter(Mandatory=$true)][string]$KB
    )
    $SearchUpdates = dism /online /get-packages | findstr "Package_for"
    $updates = $SearchUpdates.replace("Package Identity : ", "") | findstr $KB

    #$updates

    DISM.exe /Online /Remove-Package /PackageName:$updates /quiet /norestart
    Write-Information "Finished uninstalling $KB"
}
