#!/usr/bin/env bash
set -euo pipefail

#####################################
# 引数チェック
#####################################
MIGRATION_NAME="${1:-}"
ENVIRONMENT="${2:-Development}"

if [[ -z "$MIGRATION_NAME" ]]; then
  echo "[ERROR] Migration name is required."
  echo "Usage: add-migration.sh <MigrationName> [Environment]"
  exit 1
fi

#####################################
# 環境変数
#####################################
export ASPNETCORE_ENVIRONMENT="$ENVIRONMENT"

#####################################
# パス解決（どこからでもOK）
#####################################
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

MIGRATIONS_PROJECT="$ROOT_DIR/CleArchKit.Migrations"
STARTUP_PROJECT="$ROOT_DIR/CleArchKit.Infrastructure"
OUTPUT_DIR="Postgresql\Migrations"

#####################################
# 事前チェック
#####################################
if [[ ! -f "$ROOT_DIR/CleArchKit.sln" ]]; then
  echo "[ERROR] Solution file not found: $ROOT_DIR/CleArchKit.sln"
  exit 1
fi

if [[ ! -d "$MIGRATIONS_PROJECT" ]]; then
  echo "[ERROR] Migrations project not found: $MIGRATIONS_PROJECT"
  exit 1
fi

#####################################
# 実行
#####################################
echo "========================================"
echo " Adding EF Core Migration"
echo "----------------------------------------"
echo " Name        : $MIGRATION_NAME"
echo " Environment : $ENVIRONMENT"
echo " Root        : $ROOT_DIR"
echo "========================================"

dotnet ef migrations add "$MIGRATION_NAME" \
  --project "$MIGRATIONS_PROJECT" \
  --startup-project "$STARTUP_PROJECT" \
  --output-dir "$OUTPUT_DIR"

#####################################
# 成功ログ
#####################################
echo "----------------------------------------"
echo "[SUCCESS] Migration '$MIGRATION_NAME' created successfully."
echo " Output directory: $MIGRATIONS_PROJECT/$OUTPUT_DIR"
echo "========================================"
