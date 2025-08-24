# 実行環境によって変える
$Env:ASPNETCORE_ENVIRONMENT = "Development"

# どこからでもマイグレーション実行できるように
$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$migrationsProject = Join-Path $projectDir "BlazorWasmTemplate.Migrations"
$startupProject = Join-Path $projectDir "BlazorWasmTemplate.Infrastructure"

dotnet run --project $migrationsProject --startup-project $startupProject