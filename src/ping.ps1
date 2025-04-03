while($true) {
    try {
        Invoke-WebRequest -Uri "http://localhost:5000/hangfire" -UseBasicParsing
        Invoke-WebRequest -Uri "http://localhost:5001/api/health" -UseBasicParsing
    } catch {
        Write-Output "Erro: $_"
    }
    Start-Sleep -Seconds 300
}