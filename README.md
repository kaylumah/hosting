![logo](meta/resources/logo.svg)

-----

# SSG

![.NET (CI)](https://github.com/Kaylumah/SSG/workflows/.NET%20(CI)/badge.svg)
## Design System

### Header

* Background-color is `bg-gray-900`
* Text-color is `text-gray-100`
* Anchor hover is `text-gray-400`

### Footer

* Background-color is `bg-gray-900`
* Text-color is `text-gray-100`
* Anchor hover is `text-gray-400`


## Docs

- Code coverage
https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux

dotnet test --collect:"XPlat Code Coverage"
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

dotnet reportgenerator "-reports:test/Unit/TestResults/2a416370-74ff-4c37-9418-b31e88d736a0/coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html

# Config



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

---

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[] { "# Test", string.Empty };
            // yield return new object[] { string.Empty, string.Empty };

            // yield return new object[] { "*foo*", "<p><em>foo</em></p>\n" };

            // yield return new object[] { "# This is the header", "<h1 id=\"this-is-the-header\">This is the header</h1>\n" };
            // yield return new object[] { "## This is the header", "<h2 id=\"this-is-the-header\">This is the header</h2>\n" };
            // yield return new object[] { "### This is the header", "<h3 id=\"this-is-the-header\">This is the header</h3>\n" };
            // yield return new object[] { "#### This is the header", "<h4 id=\"this-is-the-header\">This is the header</h4>\n" };
            // yield return new object[] { "##### This is the header", "<h5 id=\"this-is-the-header\">This is the header</h5>\n" };
            // yield return new object[] { "###### This is the header", "<h6 id=\"this-is-the-header\">This is the header</h6>\n" };

            // // 6 Inlines
            // yield return new object[] { "`foo`", "<p><code>foo</code></p>\n" };
        }

        [Theory]
        [MemberData(nameof(GetData))]
        //[InlineData("", "")]
        // [InlineData("*Hello*", "")]
        public void Test1(string input, string expected)
        {
            var actual = _fixture.Render.Render(input);
            Assert.Equal(expected, actual);
        }

        ---

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


  https://jekyllrb.com/tutorials/convert-site-to-jekyll/#9-simplify-your-site-with-includes





https://khalidabuhakmeh.com/parse-markdown-front-matter-with-csharp