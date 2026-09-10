#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

if ! command -v docker >/dev/null 2>&1; then
  echo "Docker was not found. Install Docker Desktop and try again."
  exit 1
fi

if ! docker info >/dev/null 2>&1; then
  echo "Docker is installed but not running. Start Docker Desktop and try again."
  exit 1
fi

echo "Starting SQL Server in Docker..."
docker compose up -d --wait

if lsof -nP -iTCP:5099 -sTCP:LISTEN >/dev/null 2>&1; then
  echo "The customer web app is already running at http://localhost:5099"
  exit 0
fi

echo "Starting the web app at http://localhost:5099"
dotnet run --project "$ROOT/Oblig4Azure/Oblig4Azure.csproj"
