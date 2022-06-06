$InstalledSoftware64 = Get-ChildItem "HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall" -Depth 0
$InstalledSoftware32 = Get-ChildItem "HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall" -Depth 0

$AllInstalledSoftware = @()

$InstalledSoftware32 | ForEach-Object {
    $displayName = $_.GetValue('DisplayName')
    $version = $_.GetValue('DisplayVersion')
    $publisher = $_.GetValue('Publisher')
    $uninstall = $_.GetValue('UninstallString')

    if ($displayName) {
        $AllInstalledSoftware += [pscustomobject]@{
            Name = $displayName
            Publisher = $publisher
            Version = $version
            BitDepth = "32bit"
            UninstallString = $uninstall
        }
    }
}

$InstalledSoftware64 | ForEach-Object {
    $displayName = $_.GetValue('DisplayName')
    $version = $_.GetValue('DisplayVersion')
    $publisher = $_.GetValue('Publisher')
    $uninstall = $_.GetValue('UninstallString')

    if ($displayName) {
        $AllInstalledSoftware += [pscustomobject]@{
            Name = $displayName
            Publisher = $publisher
            Version = $version
            BitDepth = "64bit"
            UninstallString = $uninstall
        }
    }
}

$AllInstalledSoftware | Sort-Object -Property Name