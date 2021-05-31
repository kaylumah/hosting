
```xml
<PropertyGroup>
  <Day Condition=" '$(Day)' == '' ">0</Day>
  <BuildNumber Condition=" '$(BuildNumber)' == '' ">0</BuildNumber>
  <VersionPrefix>4.6.$(Day).$(BuildNumber)</VersionPrefix>
</PropertyGroup>
```

```sh
dotnet msbuild /t:Restore;Pack /p:Configuration=Release /p:Day=123 /p:BuildNumber=3
```

```xml
<VersionSuffix Condition=" '$(Configuration)' == 'Debug' ">dev</VersionSuffix>
```

```csharp
var version = Assembly.GetEntryAssembly()
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    .InformationalVersion;
    //typeof(MyClass).Assembly.…
```

https://github.com/dotnet/Nerdbank.GitVersioning 
https://github.com/dasMulli/SimpleGitVersion
https://www.cazzulino.com/git-info-from-msbuild-and-code.html -> GitInfo
https://ml-software.ch/posts/versioning-made-easier-with-nerdbank-gitversioning  -> alternative o gitinfo

```xml
<PropertyGroup>
  <Build>$([System.DateTime]::op_Subtraction($([System.DateTime]::get_Now().get_Date()),$([System.DateTime]::new(2000,1,1))).get_TotalDays())</Build>
  <Revision>$([MSBuild]::Divide($([System.DateTime]::get_Now().get_TimeOfDay().get_TotalSeconds()), 2).ToString('F0'))</Revision>
  <Version>1.2.$(Build).$(Revision)</Version>
</PropertyGroup>
```

```
<Today>$([System.DateTime]::Now.ToString("yyyy.MM.dd"))</Today>
```

Directory.Build.targets
```xml
<Project>
  <Target Name="DetermineOctopackVersion" BeforeTargets="Build" Condition=" '$(RunOctoPack)' == 'true' ">
    <Exec Command="git rev-parse HEAD" ConsoleToMSBuild="true" StandardOutputImportance="low">
      <Output TaskParameter="ConsoleOutput" PropertyName="GitCommitHash" />
    </Exec>
    <PropertyGroup>
      <VersionDatePart>$([System.DateTime]::get_Now().ToString(yyyyMMdd.HHmmss))</VersionDatePart>
      <OctopackPackageVersion>2.$(VersionDatePart)-git-$(GitCommitHash.Substring(0,16))</OctopackPackageVersion>
    </PropertyGroup>
  </Target>
</Project>
```

```ps
$commitHash = (git rev-parse HEAD).Substring(0,16)
$versionDatePart = [System.DateTime]::Now.ToString('yyyyMMdd.HHmmss')
$version = "1.$versionDatePart-git-$commitHash"
```

<Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />

```xml
  <PropertyGroup>
    <CodeFile>$(MSBuildProjectDirectory)\HelloGit.cs</CodeFile>
    <CodeFileText>$([System.IO.File]::ReadAllText("$(CodeFile)"))</CodeFileText>
    <CodeFileText>$([MSBuild]::Unescape("$(CodeFileText)"))</CodeFileText>
  </PropertyGroup>

  <UsingTask TaskName="HelloGit" TaskFactory="CodeTaskFactory" AssemblyFile="$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll">
    <ParameterGroup>
      <Sha1 ParameterType="System.String" Required="False" Output="True" />
      <Repository ParameterType="System.String" Required="True" Output="False" />
    </ParameterGroup>
    <Task>
      <Reference Include="System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
      <Reference Include="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\LibGit2Sharp.dll" />
      <Using Namespace="System" />
      <Using Namespace="System.IO" />
      <Code Type="Class" Language="cs">
        $(CodeFileText)
      </Code>
    </Task>
  </UsingTask>

  <Target Name="RunTask" AfterTargets="BeforeBuild">

    <HelloGit Repository="$(MSBuildProjectDirectory)">
      <Output TaskParameter="Sha1" PropertyName="CommitId" />
    </HelloGit>
    <Warning Text="Your commit is: $(CommitId)" />

  </Target>
```

```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using LibGit2Sharp;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;


namespace GitIntrospection {
    public class HelloGit : Task {
        public override bool Execute() {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                if (args.Name.Contains("LibGit2Sharp")) {
                    return Assembly.LoadFrom("C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE\\CommonExtensions\\Microsoft\\TeamFoundation\\Team Explorer\\LibGit2Sharp.dll");
                }
                return null;
            };

            GetCommit();

            return !Log.HasLoggedErrors;
        }

        // [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private void GetCommit() {
            Repository repository = new Repository(Repository);

            this.Sha1 = repository.Commits.First().Id.Sha;
        }

        [Output]
        public string Sha1 { get; set; }

        [Required]
        public string Repository { get; set; }
    }
}
```



  https://jekyllrb.com/tutorials/convert-site-to-jekyll/#9-simplify-your-site-with-includes





https://khalidabuhakmeh.com/parse-markdown-front-matter-with-csharp
