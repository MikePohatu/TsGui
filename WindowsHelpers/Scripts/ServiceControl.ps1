
function RestartService {
    Param (
        [Parameter(Mandatory)][string]$ServiceName
    )

    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

    if ($service) {
        if ($service.Status -eq 'Running') {
            Write-Information "Stopping service $ServiceName"
            $service.Stop()
            while ($service.Status -ne 'Stopped')
            {
                Start-Sleep -seconds 5
                $service.Refresh()
            }
        }

        Write-Information "Starting service $ServiceName"
        $service.Start()
        while ($service.Status -ne 'Running')
        {
            Start-Sleep -seconds 5
            $service.Refresh()
        }

        Write-Information "Done"
    } else {
        Write.Error "Service not found: $ServiceName"
    }

    return $service
}

function StopService {
    Param (
        [Parameter(Mandatory)][string]$ServiceName
    )

    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

    if ($service) {
        if ($service.Status -eq 'Starting') {
            Write-Information "Waiting for  $ServiceName service to start"
            while ($service.Status -ne 'Running')
            {
                Start-Sleep -seconds 5
                $service.Refresh()
            }
        }

        if ($service.Status -eq 'Running') {
            Write-Information "Stopping service $ServiceName"
            $service.Stop()
            while ($service.Status -ne 'Stopped')
            {
                Start-Sleep -seconds 5
                $service.Refresh()
            }
            Write-Information "Service $ServiceName stopped"
        }

        Write-Information "Done"
    } else {
        Write.Error "Service not found: $ServiceName"
    }
    return $service
}


function StartService {
    Param (
        [Parameter(Mandatory)][string]$ServiceName
    )

    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

    if ($service) {
        if ($service.Status -eq 'Stopping') {
            Write-Information "Waiting for  $ServiceName service to stop"
            while ($service.Status -ne 'Stopped')
            {
                Start-Sleep -seconds 5
                $service.Refresh()
            }
        }

        if ($service.Status -eq 'Stopped') {
            Write-Information "Starting service $ServiceName"
            $service.Start()
            while ($service.Status -ne 'Running')
            {
                Start-Sleep -seconds 5
                $service.Refresh()
            }
            Write-Information "Service $ServiceName started"
        }

        Write-Information "Done"
    } else {
        Write.Error "Service not found: $ServiceName"
    }
    return $service
}
