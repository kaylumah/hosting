# MSbuild Metadata

https://stu.dev/adding-assemblymetadataattribute-using-new-sdk-project-with-msbuild/

# https://docs.microsoft.com/en-us/dotnet/core/tools/csproj
RepositoryType="git"
echo "$RepositoryType"
RepositoryUrl=$(git config --get remote.origin.url)
echo "$RepositoryUrl"
RepositoryBranch=$(git rev-parse --abbrev-ref HEAD)
echo "$RepositoryBranch"
RepositoryCommit=$(git rev-parse HEAD)
echo "$RepositoryCommit"

dotnet publish -c Release /property:CommitHash=MyHash


          <ItemGroup>
    <!-- https://stu.dev/adding-assemblymetadataattribute-using-new-sdk-project-with-msbuild/ -->
    <!-- https://github.com/dotnet/sdk/pull/3440 -->
    <AssemblyAttribute Include="System.Reflection.AssemblyMetadataAttribute" Condition="'$(BuildNumber)' != ''">
      <_Parameter1>BuildNumber</_Parameter1>
      <_Parameter2>$(BuildNumber)</_Parameter2>
    </AssemblyAttribute>
    <AssemblyMetadata Include="Foo" Value="Bar" />
    <AssemblyMetadata Include="Bar" Value="Baz" />
    <AssemblyMetadata Include="SdkVersion" Value="$(ArcadeSdkVersion)" />
  </ItemGroup>
