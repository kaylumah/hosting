#!/bin/sh

CONFIGURATION=Release

echo "$CONFIGURATION"

dotnet restore

dotnet build --configuration $CONFIGURATION --no-restore

dotnet test --configuration $CONFIGURATION --no-build --verbosity normal