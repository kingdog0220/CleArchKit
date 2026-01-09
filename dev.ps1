param(
    [Parameter(Position=0)]
    [string]$Command,

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$Args
)

function Help {
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  ./dev.ps1 <command>"
    Write-Host ""
    Write-Host "Commands:"
    Write-Host "  up              docker compose up -d"
    Write-Host "  down            docker compose down"
    Write-Host "  ps              docker compose ps"
    Write-Host "  logs            docker compose logs -f"
    Write-Host ""
    Write-Host "  qodo            qodo command (node-tools)"
    Write-Host ""
}

switch ($Command) {

    "up" {
        docker compose up -d
    }

    "down" {
        docker compose down
    }

    "ps" {
        docker compose ps
    }

    "logs" {
        docker compose logs -f
    }

    "qodo" {
        ./scripts/host/qodo.ps1 @Args
    }

    default {
        Help
    }
}