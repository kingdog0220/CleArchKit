param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

if (-not $MigrationName) {
    Write-Error "Migration name is required."
    exit 1
}

# プロジェクトディレクトリを取得
$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$migrationsProject = Join-Path $projectDir "BlazorWasmTemplate.Migrations"

# 出力先ディレクトリ（プロジェクト内の相対パス）
$outputDir = "Postgresql\Migrations"

dotnet ef migrations add $MigrationName --project $migrationsProject --output-dir $outputDir

if ($LASTEXITCODE -ne 0) {
    Write-Error "Migration creation failed."
    exit $LASTEXITCODE
}

Write-Host "Migration created successfully."