#!/bin/sh

CONFIGURATION=Release

dotnet restore

dotnet build --configuration $CONFIGURATION --no-restore

# dotnet test --configuration $CONFIGURATION --no-build --verbosity normal
dotnet test --configuration $CONFIGURATION --no-build --verbosity normal /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=TestResults/lcov.info

# dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/ /p:CoverletOutputFormat=lcov
# dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
# dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=TestResults/lcov.info

# Generate coverage report
# Publish coverage report

