# CODE COVERAGE

https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux

dotnet test --collect:"XPlat Code Coverage"
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

dotnet reportgenerator "-reports:test/Unit/TestResults/2a416370-74ff-4c37-9418-b31e88d736a0/coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html
