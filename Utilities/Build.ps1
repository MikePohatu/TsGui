$version = '2.1.0.1'

Function SignAssembliesInPath {
    Param(
        [Parameter(Mandatory=$true)][string]$PackagePath,
        [string]$SignPath='C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x64',
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

function Set-ProjectDetails {
    Param (
        [Parameter(Mandatory=$true)][string]$ProjectPath,
        [Parameter(Mandatory=$true)][string]$Version,
        [string]$Copyright,
        [string]$Product
    )

    #https://stackoverflow.com/q/57666790
    $assemblyInfoPath = "$($ProjectPath)\Properties\AssemblyInfo.cs"

    $assemblyInfoText = (Get-Content -Path $assemblyInfoPath -Encoding UTF8 -ReadCount 0)
    $assemblyInfoText = $assemblyInfoText -replace '\[assembly: AssemblyVersion\("((\d)+|(\.))*"\)\]', "[assembly: AssemblyVersion(`"$Version`")]"
    $assemblyInfoText = $assemblyInfoText -replace '\[assembly: AssemblyFileVersion\("((\d)+|(\.))*"\)\]', "[assembly: AssemblyFileVersion(`"$Version`")]" 
    if ($Copyright) {
        $assemblyInfoText = $assemblyInfoText -replace '\[assembly: AssemblyCopyright\(".*"\)\]', "[assembly: AssemblyCopyright(`"$Copyright`")]" 
    }
    if ($Product) {
        $assemblyInfoText = $assemblyInfoText -replace '\[assembly: AssemblyProduct\(".*"\)\]', "[assembly: AssemblyProduct(`"$Product`")]" 
    }
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

#Define details
$copyRight = "Copyright © 20Road Limited $(get-date -Format yyyy)"
$repoRoot = 'C:\Source\repos\TsGui'
$devenv = 'C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe'
$cmd = "cmd"
$productName = "TsGui"
$ReleaseRootPath='C:\Source\release'
$ProductReleasePath="$($ReleaseRootPath)\tsgui"
$ProjectReleasePath="$($repoRoot)\TsGui\bin\Release"
$BuildFile = "$($repoRoot)\TsGui\TsGui.csproj"

Write-Host "Updating product details"
Write-Host "Product name: $productName"
Write-Host "Product version: $version"
Write-Host "Product copyright: $copyright"

Set-ProjectDetails -ProjectPath "$($repoRoot)\MessageCrap" -Version $version -Copyright $copyRight -Product $productName
Set-ProjectDetails -ProjectPath "$($repoRoot)\TsGui" -Version $version -Copyright $copyRight -Product $productName
Set-ProjectDetails -ProjectPath "$($repoRoot)\Core" -Version $version -Copyright $copyRight -Product $productName
Set-ProjectDetails -ProjectPath "$($repoRoot)\CustomActions" -Version $version -Copyright $copyRight -Product $productName
Set-ProjectDetails -ProjectPath "$($repoRoot)\MessageCrap" -Version $version -Copyright $copyRight -Product $productName
Set-ProjectDetails -ProjectPath "$($repoRoot)\WindowsHelpers" -Version $version -Copyright $copyRight -Product $productName
Set-ProjectDetails -ProjectPath "$($repoRoot)\TsGui.Tests" -Version $version -Copyright $copyRight -Product $productName


#kill any tsgui processes
$runningProcs = Get-Process 'tsgui' -ErrorAction SilentlyContinue
if ($runningProcs) {
    $confirm = Read-Host -Prompt "Running tsgui processes found. Kill and continue (y/n)?"
    if ( $confirm -match "[yY]" ) {
        $runningProcs | Stop-Process -Force
    }
}


#run builds
Write-Host "Building $BuildFile"
Start-Process -WorkingDirectory $repoRoot -FilePath $cmd -ArgumentList "/c `"`"$devenv`" `"$BuildFile`" /rebuild Release`""

Write-Host ""
Read-Host -Prompt "Press Enter when build has completed"

if (Test-Path $ProductReleasePath) { 
    Remove-Item $ProductReleasePath -Force -Recurse
} 


mkdir $ProductReleasePath -Force
mkdir "$($ProductReleasePath)\Config_Examples" -Force
mkdir "$($ProductReleasePath)\images" -Force
mkdir "$($ProductReleasePath)\scripts" -Force

Copy-Item -Path "$($repoRoot)\Config_Examples\*" -Destination "$($ProductReleasePath)\Config_Examples" -Recurse -Force
Copy-Item -Path "$($repoRoot)\images\*" -Destination "$($ProductReleasePath)\images" -Recurse -Force
Copy-Item -Path "$($ProjectReleasePath)\scripts\*" -Destination "$($ProductReleasePath)\scripts" -Recurse -Force
Copy-Item -Path "$($repoRoot)\Release notes.txt" -Destination "$($ProductReleasePath)" -Force
Copy-Item -Path "$($repoRoot)\Documentation and Source.url" -Destination "$($ProductReleasePath)" -Force
Copy-Item -Path "$($repoRoot)\Config.xml" -Destination "$($ProductReleasePath)" -Force
Copy-Item -Path "$($repoRoot)\Config_demo.xml" -Destination "$($ProductReleasePath)" -Force


$appFiles = @("TsGui.exe", 
    "TsGui.exe.config", 
    "Core.dll", 
    "Messaging.dll", 
    "Microsoft.Management.Infrastructure.dll", 
    "Newtonsoft.Json.dll", 
    "NLog.dll", "NLog.config",
    "System.Management.Automation.dll", 
    "WindowsHelpers.dll"
)

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

    if ($errorfound) {
        Write-Error "Something went wrong, please review the output."
    }

    #Create the signed package zip
    PackageFolder -PackageFolder $ProductReleasePath -DestinationPath "$($ReleaseRootPath)\$($productName)_$($version)_signed.zip"
}

