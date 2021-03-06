# SSG

![.NET (CI)](https://github.com/Kaylumah/SSG/workflows/.NET%20(CI)/badge.svg)

## Setup TailwindCSS

[docs](https://tailwindcss.com/docs/installation)

1. npm install tailwindcss@latest postcss@latest autoprefixer@latest
2. npm install postcss-cli
3. npm install @tailwindcss/typography
4. npx tailwindcss init -p
5. ./node_modules/.bin/postcss styles.css -o compiled.css (npm run dev)

npx tailwindcss init tailwind-full.config.js --full
posscss 
tailwindcss: {
    config: './tailwind-full.config.js'
}

const colors = require('tailwindcss/colors')


Create css file

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

## Design System

### Header

* Background-color is `bg-gray-900`
* Text-color is `text-gray-100`
* Anchor hover is `text-gray-400`

### Footer

* Background-color is `bg-gray-900`
* Text-color is `text-gray-100`
* Anchor hover is `text-gray-400`

## MSBuild Extensions

https://www.hanselman.com/blog/adding-a-git-commit-hash-and-azure-devops-build-number-and-build-id-to-an-aspnet-website 

* `SourceRevisionId`
* `BuildNumber` or `BuildId`

## Reading list

https://tailwindcss.com/docs/optimizing-for-production#enabling-manually

https://refactoringui.com/book/

https://v1.tailwindcss.com/resources

http://www.zondicons.com/

https://simpleicons.org/

https://heroicons.com/

https://github.com/tailwindlabs/blog.tailwindcss.com


Taftse02/16/2021
The Logos come from https://simpleicons.org/ as far as I know

I think they only used simple Icons for the brand logos because they are not part of heroicons

They could also be part of http://www.zondicons.com/ which is also made by Steve as far as I know





- https://github.com/tailwindlabs/tailwindcss-typography

- https://medium.com/@mattront/the-complete-guide-to-customizing-a-tailwind-css-theme-ef423eccf863

- https://tjaddison.com/blog/2020/08/updating-to-tailwind-typography-to-style-markdown-posts/

- https://github.com/tailwindlabs/tailwindcss.com/blob/master/tailwind.config.js

- https://github.com/tailwindlabs/tailwindcss.com/blob/bb5eacf1778bb377b9bb190bccc2cac494cdfb56/src/css/utilities.css#L101

- https://github.com/tailwindlabs/tailwindcss.com/blob/bb5eacf1778bb377b9bb190bccc2cac494cdfb56/src/components/Heading.js


## Docs

- Code coverage
https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux

dotnet test --collect:"XPlat Code Coverage"
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

dotnet reportgenerator "-reports:test/Unit/TestResults/2a416370-74ff-4c37-9418-b31e88d736a0/coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html

# Config

[link](https://jekyllrb.com/docs/variables/)

| variable | description |
| - | - |
| content | The content of the Page, rendered or un-rendered. |
| title | The title of the Page. |
| excerpt | The un-rendered excerpt of a document. |
| url | The URL of the Post without the domain, but with a leading slash. |
| date | The Date assigned to the Post. |
| id | An identifier unique to a document in a Collection or a Post. |
| collection | The label of the collection to which this document belongs. |
| tags | The list of tags to which this post belongs |
| dir | The path between the source directory and the file of the post or page. |
| name | The filename of the post or page, e.g. |
| path | The path to the raw post or page. |
|  |  |

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


The <meta> tag defines metadata about an HTML document. Metadata is data (information) about data.

<meta> tags always go inside the <head> element, and are typically used to specify character set, page description, keywords, author of the document, and viewport settings.

<meta name="keywords" content="HTML, CSS, JavaScript">
<meta name="description" content="Free Web tutorials for HTML and CSS">
<meta name="author" content="John Doe">
https://www.geeksforgeeks.org/whats-the-difference-between-meta-name-and-meta-property/
https://www.wordstream.com/meta-tags 
https://www.searchenginejournal.com/technical-seo/meta-robots-tags-robots-txt/#close

```cs
            for(var i = 0; i < processed.Count(); i++)
            {
                var element = processed.ElementAt(i);
                if (".html".Equals(Path.GetExtension(element.Name)))
                {
                    // https://html-agility-pack.net/knowledge-base/16645257/how-to-use-html-agility-pack-for-html-validations
                    // https://html-agility-pack.net/knowledge-base/2354653/grabbing-meta-tags-and-comments-using-html-agility-pack
                    var html = renderResults[i].Content;
                    var document = new HtmlAgilityPack.HtmlDocument
                    {
                        OptionFixNestedTags = true
                    };
                    // html += "<p>ssds";
                    document.LoadHtml(html);
                    var errors = document.ParseErrors;
                    bool hasHead = document.DocumentNode.SelectSingleNode("html/head") != null;
                    bool hasBody = document.DocumentNode.SelectSingleNode("html/body") != null;

                    var rootNode = document.DocumentNode.SelectSingleNode("html");

                    var head = document.DocumentNode.SelectSingleNode("html/head");
                    var metaTags = head.SelectNodes("meta");
                    var titleTag = head.SelectSingleNode("title");
                }
            }
```