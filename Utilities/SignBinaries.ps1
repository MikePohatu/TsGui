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

        #[Array]$verifyparams = "verify","/pa","/q"
        #$verifyparams += $assemblies

        & $signer $signparams
        #& $signer $verifyparams
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

    add-type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory("$PackageFolder",$DestinationPath,'Optimal',$false)
    #Compress-Archive -Path "$PackageFolder\*" -DestinationPath $DestinationPath
}

$TsguiPath='C:\Source\release\tsgui'

SignAssembliesInPath -PackagePath $TsguiPath -Description "TsGui by 20Road"

$tsguistatus = VerifyAssembliesInPath -PackagePath $TsguiPath

$allstatus = @()
$allstatus += $tsguistatus

$errorfound = $false

$allstatus | ForEach-Object { 
    if ($_.Status -ne "Valid") { 
        $errorfound = $true
        break
    }
}
