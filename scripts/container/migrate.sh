#!/usr/bin/env bash
set -e

ENVIRONMENT=${1:-Development}
export ASPNETCORE_ENVIRONMENT=$ENVIRONMENT

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

MIGRATIONS_PROJECT="$ROOT_DIR/CleArchKit.Migrations"
STARTUP_PROJECT="$ROOT_DIR/CleArchKit.Infrastructure"

dotnet run \
  --project "$MIGRATIONS_PROJECT" \
  --startup-project "$STARTUP_PROJECT"

#####################################
# 成功ログ
#####################################
echo "----------------------------------------"
echo "[SUCCESS] Migration executed successfully."
echo "========================================"
