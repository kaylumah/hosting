# .NET Code Style

https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/code-style-rule-options

https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview#code-style-analysis

https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/



https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2019#automatically-configure-rule-severity 

https://docs.microsoft.com/en-us/visualstudio/ide/reference/add-file-header?view=vs-2019 


TreatWarningsAsErrors -> https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2019
MSBuildTreatWarningsAsErrors




https://community.sonarsource.com/t/configuration-of-warningsaserrors-for-net-build/32393

https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/errors-warnings


---
https://docs.microsoft.com/en-us/visualstudio/code-quality/migrate-from-legacy-analysis-to-net-analyzers?view=vs-2019 

https://docs.microsoft.com/en-us/visualstudio/code-quality/migrate-from-fxcop-analyzers-to-net-analyzers?view=vs-2019

https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/code-quality-rule-options

--------





## Code Analysis...
https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview

NET compiler platform (Roslyn) analyzers inspect your C# or Visual Basic code for code quality and style issues. Starting in .NET 5.0, these analyzers are included with the .NET SDK and you don't need to install them separately. If your project targets .NET 5 or later, code analysis is enabled by default. If your project targets a different .NET implementation, for example, .NET Core, .NET Standard, or .NET Framework, you must manually enable code analysis by setting the EnableNETAnalyzers property to true.

.NET analyzers are target-framework agnostic. That is, your project does not need to target a specific .NET implementation. The analyzers work for projects that target net5.0 as well as earlier .NET versions, such as netcoreapp3.1 and net472. However, to enable code analysis using the EnableNETAnalyzers property, your project must reference a project SDK.
see:
https://docs.microsoft.com/en-us/dotnet/core/project-sdk/overview
https://docs.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#code-analysis-properties

https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-options
https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files

https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/suppress-warnings

https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/code-quality-rule-options
https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/predefined-configurations

https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/code-style-rule-options
https://docs.microsoft.com/en-us/visualstudio/ide/create-portable-custom-editor-options?view=vs-2019

https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/
https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/

dotnet build -warnaserror

dotnet_naming_rule.interface_should_be_begins_with_i.severity = suggestion
dotnet_naming_rule.interface_should_be_begins_with_i.symbols = interface
dotnet_naming_rule.interface_should_be_begins_with_i.style = begins_with_i