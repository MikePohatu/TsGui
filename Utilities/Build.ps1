Function SignAssembliesInPath {
    Param(
        [Parameter(Mandatory=$true)][string]$PackagePath,
        [string]$SignPath='C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64',
        [string]$Description
    )

    $assemblies = Get-ChildItem $PackagePath -Recurse -Include ('*.exe', '*.dll') | Foreach-Object {
        $signature = Get-AuthenticodeSignature $_ 
        if ($signature.Status -ne "Valid") { $_.FullName }
    }


    if ($assemblies) {
        $signer = "$SignPath\signtool.exe"
        [Array]$signparams = "sign","/v","/n","20Road Limited","/d",$Description,"/du","https://20road.com","/tr","http://ts.ssl.com","/td","SHA256","/fd","SHA256"
        $signparams += $assemblies

        & $signer $signparams
    }
}

Function VerifyAssembliesInPath {
    Param(
        [Parameter(Mandatory=$true)][string]$PackagePath
    )

    Get-ChildItem $PackagePath -Recurse -Include ('*.exe', '*.dll') | Foreach-Object {
        $signature = Get-AuthenticodeSignature $_ 
        if ($signature.Status -eq "Valid") { Write-Information "Valid: $($_.FullName)" }
        else { Write-Error -Message "Issue: $($_.FullName)"}

        $outobj = New-Object -TypeName psobject -Property @{
            'FilePath'= $_.FullName 
            'Status' = $signature.Status }

        write-output $outobj
    }
}

Function PackageFolder {
    Param(
        [Parameter(Mandatory=$true)][string]$PackageFolder,
        [Parameter(Mandatory=$true)][string]$DestinationPath
    )

    if (Test-Path -Path $DestinationPath) { Remove-Item -Path $DestinationPath }

    add-type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory("$PackageFolder",$DestinationPath,'Optimal',$false)
}

function Set-ProjectVersion {
    Param (
        [Parameter(Mandatory=$true)][string]$ProjectPath,
        [Parameter(Mandatory=$true)][string]$Version
    )

    #https://stackoverflow.com/q/57666790
    $assemblyInfoPath = "$($ProjectPath)\Properties\AssemblyInfo.cs"

    Write-Host "Updating $assemblyInfoPath to $Version"

    $assemblyInfoText = (Get-Content -Path $assemblyInfoPath -Encoding UTF8 -ReadCount 0)
    $assemblyInfoText = $assemblyInfoText -replace '\[assembly: AssemblyVersion\("((\d)+|(\.))*"\)\]', "[assembly: AssemblyVersion(`"$Version`")]"
    $assemblyInfoText = $assemblyInfoText -replace '\[assembly: AssemblyFileVersion\("((\d)+|(\.))*"\)\]', "[assembly: AssemblyFileVersion(`"$Version`")]" 
    $assemblyInfoText | Set-Content -Path $assemblyInfoPath -Encoding UTF8 | Out-Null
}

Function pause ()
{
    Param(
        [string]$message = 'Press any key to continue...'
    )
    # Check if running Powershell ISE
    if ($psISE)
    {
        Add-Type -AssemblyName System.Windows.Forms
        [System.Windows.Forms.MessageBox]::Show("$message")
    }
    else
    {
        Write-Host "$message" -ForegroundColor Yellow
        $x = $host.ui.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
}

#run builds
$version = '1.5.1.2'
$repoRoot = 'C:\Source\repos\TsGui'
$devenv = 'C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe'
$dotnet = 'dotnet.exe'
$cmd = "cmd"
$productName = "TsGui"
$ReleaseRootPath='C:\Source\release'
$ProductReleasePath="$($ReleaseRootPath)\tsgui"
$ProjectReleasePath="$($repoRoot)\TsGui\bin\Release"
$BuildFile = "$($repoRoot)\TsGui\TsGui.csproj"

Write-Host "Updating $productName versions to $version"
Set-ProjectVersion -ProjectPath "$($repoRoot)\MessageCrap" -Version $version
Set-ProjectVersion -ProjectPath "$($repoRoot)\TsGui" -Version $version




Write-Host "Building $BuildFile"
Start-Process -WorkingDirectory $repoRoot -FilePath $cmd -ArgumentList "/c `"`"$devenv`" `"$BuildFile`" /rebuild Release`""

pause -message "Click OK when build has complete"

if (Test-Path $ProductReleasePath) { 
    Remove-Item $ProductReleasePath -Force -Recurse
} 


md $ProductReleasePath -Force
md "$($ProductReleasePath)\Config_Examples" -Force
md "$($ProductReleasePath)\images" -Force
md "$($ProductReleasePath)\scripts" -Force

Copy-Item -Path "$($repoRoot)\Config_Examples\*" -Destination "$($ProductReleasePath)\Config_Examples" -Recurse -Force
Copy-Item -Path "$($repoRoot)\images\*" -Destination "$($ProductReleasePath)\images" -Recurse -Force
Copy-Item -Path "$($ProjectReleasePath)\scripts\*" -Destination "$($ProductReleasePath)\scripts" -Recurse -Force
Copy-Item -Path "$($repoRoot)\Release notes.txt" -Destination "$($ProductReleasePath)" -Force
Copy-Item -Path "$($repoRoot)\Documentation and Source.url" -Destination "$($ProductReleasePath)" -Force
Copy-Item -Path "$($repoRoot)\Config.xml" -Destination "$($ProductReleasePath)" -Force
Copy-Item -Path "$($repoRoot)\Config_demo.xml" -Destination "$($ProductReleasePath)" -Force


$appFiles = @("TsGui.exe", 
    "Core.dll", 
    "Messaging.dll", 
    "Microsoft.Management.Infrastructure.dll", 
    "Newtonsoft.Json.dll", 
    "NLog.dll", "NLog.config",
    "System.Management.Automation.dll", 
    "WindowsHelpers.dll")

$appFiles | ForEach-Object {
    Copy-Item -Path "$($ProjectReleasePath)\$_" -Destination "$($ProductReleasePath)" -Force
}

#Create the unsigned package zip
PackageFolder -PackageFolder $ProductReleasePath -DestinationPath "$($ReleaseRootPath)\$($productName)_$($version).zip"


$confirm = Read-Host -Prompt "Do you want to sign binaries?[y/n]"
if ( $confirm -match "[yY]" ) { 
    Write-Host "Signing assemblies"
    SignAssembliesInPath -PackagePath $ProductReleasePath -Description $productName

    $appstatus = VerifyAssembliesInPath -PackagePath $ProductReleasePath

    $allstatus = @()
    $allstatus += $appstatus

    $errorfound = $false

    $allstatus | ForEach-Object { 
        if ($_.Status -ne "Valid") { 
            $errorfound = $true
            break
        }
    }

    #Create the signed package zip
    PackageFolder -PackageFolder $ProductReleasePath -DestinationPath "$($ReleaseRootPath)\$($productName)_$($version)_signed.zip"
}

