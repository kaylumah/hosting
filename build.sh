#!/bin/sh
set -e

# https://unix.stackexchange.com/questions/129391/passing-named-arguments-to-shell-scripts
for ARGUMENT in "$@"
do

    KEY=$(echo $ARGUMENT | cut -f1 -d=)
    VALUE=$(echo $ARGUMENT | cut -f2 -d=)   

    case "$KEY" in
            BUILD_ID)              BUILD_ID=${VALUE} ;;
            BUILD_NUMBER)          BUILD_NUMBER=${VALUE} ;;  
            *)   
    esac    


done

if [ -z "$BUILD_ID" ]
then
      echo "BUILD_ID is empty setting default value..."
      BUILD_ID=1
fi

if [ -z "$BUILD_NUMBER" ]
then
      echo "BUILD_NUMBER is empty setting default value..."
      BUILD_NUMBER=$(date +"%Y%m%d.%H%M%S")
fi

echo "BUILD_ID = '$BUILD_ID'"
echo "BUILD_NUMBER = '$BUILD_NUMBER'"
echo "PR_BUILD_ID = '$PR_BUILD_ID'"

_cwd="$PWD"
CONFIGURATION=Release

DIR="dist"
if [ -d "$DIR" ]; then
  rm -rf $DIR
fi

dotnet restore
# build with MSBuild vars
dotnet build --configuration $CONFIGURATION --no-restore /p:BuildId=$BUILD_ID /p:BuildNumber=$BUILD_NUMBER
# test with coverage
dotnet test --configuration $CONFIGURATION --no-build --verbosity normal /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=TestResults/lcov.info
# Publish coverage report

if [ -z "$PR_BUILD_ID" ]
then
      echo "Production Build"
      dotnet "artifacts/bin/Kaylumah.Ssg.Client.SiteGenerator/$CONFIGURATION/net6.0/Kaylumah.Ssg.Client.SiteGenerator.dll" SiteConfiguration:AssetDirectory=assets
else
      echo "PR Build for $PR_BUILD_ID"
fi

cd dist
npm i
npm run build:prod
rm styles.css
rm -rf node_modules
rm package.json package-lock.json
rm tailwind.config.js