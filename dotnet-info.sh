#!/bin/sh

dotnetVersion=$(dotnet --version)
echo "dotnet version $dotnetVersion"

dotnetSdks=$(dotnet --list-sdks)
echo "dotnet sdks $dotnetSdks"

dotnetRuntimes=$(dotnet --list-runtimes)
echo "dotnet runtimes $dotnetRuntimes"