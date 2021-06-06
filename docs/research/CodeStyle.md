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

https://docs.microsoft.com/en-us/dotnet/core/project-sdk/overview -> sdk style project https://docs.microsoft.com/en-us/nuget/resources/check-project-format





https://docs.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#code-analysis-properties

| MSBuild Property | Description |
| - | - |
| AnalysisLevel | The AnalysisLevel property lets you specify a code-analysis level. For example, if you want access to preview code analyzers, set AnalysisLevel to preview. |
| AnalysisMode | Starting with .NET 5.0, the .NET SDK ships with all of the "CA" code quality rules. By default, only some rules are enabled as build warnings. The AnalysisMode property lets you customize the set of rules that are enabled by default. |
| CodeAnalysisTreatWarningsAsErrors | The CodeAnalysisTreatWarningsAsErrors property lets you configure whether code quality analysis warnings (CAxxxx) should be treated as warnings and break the build. |
| EnableNETAnalyzers | .NET code quality analysis is enabled, by default, for projects that target .NET 5.0 or later. You can enable .NET code analysis for SDK-style projects that target earlier versions of .NET by setting the EnableNETAnalyzers property to true. To disable code analysis in any project, set this property to false. |
| EnforceCodeStyleInBuild | NET code style analysis is disabled, by default, on build for all .NET projects. You can enable code style analysis for .NET projects by setting the EnforceCodeStyleInBuild property to true. |




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










https://www.hanselman.com/blog/editorconfig-code-formatting-from-the-command-line-with-net-cores-dotnet-format-global-tool
https://github.com/dotnet/format
https://docs.microsoft.com/en-us/visualstudio/ide/create-portable-custom-editor-options?view=vs-2019
https://www.meziantou.net/enforce-dotnet-code-style-in-ci-with-dotnet-format.htm

https://asp.net-hacker.rocks/2020/07/08/editorconfig-msbuild.html
https://asp.net-hacker.rocks/2020/07/24/editorconfig-netframework.html

https://www.meziantou.net/enforce-dotnet-code-style-in-ci-with-dotnet-format.htm
https://devkimchi.com/2019/09/25/static-code-analysis-with-editorconfig/



https://newbedev.com/enabling-microsoft-s-code-analysis-on-net-core-projects

https://dev.to/ssukhpinder/enforce-code-cleanup-on-build-5hnd


--- https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2019