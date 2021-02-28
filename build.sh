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
dotnet "artifacts/bin/Kaylumah.Ssg.Client.SiteGenerator/$CONFIGURATION/netcoreapp3.1/Kaylumah.Ssg.Client.SiteGenerator.dll" SiteConfiguration:AssetDirectory=assets
  AssetDirectory
