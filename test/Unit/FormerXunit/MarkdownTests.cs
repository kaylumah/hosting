﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HtmlAgilityPack;
using Kaylumah.Ssg.Utilities;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Test.Unit.FormerXunit
{

    public class MarkdownTests
    {

        [Theory]
        [MemberData(nameof(GetBlogPages))]
        public async Task Verify_MarkdownConversion_HtmlContents(string path)
        {
            string rawContents = await File.ReadAllTextAsync(path);
            string html = new MarkdownUtil("https://kaylumah.nl").ToHtml(rawContents);
            html = html.Replace("/Users/maxhamulyak/", "/ExamplePath/");

            VerifySettings settings = new VerifySettings();
            settings.UseDirectory("snapshots");
            
            // Simulate v3-style parameter name
            string encodedPath = path.Replace("/", "-").Replace("\\", "-");
            string methodName = $"{nameof(Verify_MarkdownConversion_HtmlContents)}_path={encodedPath}";
            settings.UseMethodName(methodName);

            await Verifier.Verify(html, "html", settings);
        }

        [Theory]
        [MemberData(nameof(GetBlogPages))]
        public async Task Verify_MarkdownConversion_TxtContents(string path)
        {
            string rawContents = await File.ReadAllTextAsync(path);
            string txt = new MarkdownUtil("https://kaylumah.nl").ToText(rawContents);
            txt = txt.Replace("/Users/maxhamulyak/", "/ExamplePath/");
            
            VerifySettings settings = new VerifySettings();
            settings.UseDirectory("snapshots");
           
            // Simulate v3-style parameter name
            string encodedPath = path.Replace("/", "-").Replace("\\", "-");
            string methodName = $"{nameof(Verify_MarkdownConversion_TxtContents)}_path={encodedPath}";
            settings.UseMethodName(methodName);

            await Verifier.Verify(txt, "txt", settings);
        }

        public static IEnumerable<object[]> GetBlogPages()
        {
            string[] fileNames = Directory.GetFiles("assets/posts", "*.md");
            foreach (string fileName in fileNames)
            {
                yield return new object[] { fileName };
            }
        }

        [Fact]
        public void Test1()
        {
            string markdown = "# Header 1 \r\n## Header 2\r\n### Header 3\r\n#### Header 4\r\n##### Header 5\r\n###### Header 6";
            string expected = "<h1 id=\"header-1\"><a href=\"#header-1\">Header 1</a></h1>\n<h2 id=\"header-2\"><a href=\"#header-2\">Header 2</a></h2>\n<h3 id=\"header-3\"><a href=\"#header-3\">Header 3</a></h3>\n<h4 id=\"header-4\"><a href=\"#header-4\">Header 4</a></h4>\n<h5 id=\"header-5\"><a href=\"#header-5\">Header 5</a></h5>\n<h6 id=\"header-6\"><a href=\"#header-6\">Header 6</a></h6>";
            string result = new MarkdownUtil("https://kaylumah.nl").ToHtml(markdown);
            result
                .Should()
                .Be(expected);
        }

        [Fact]
        public void Test2()
        {
            string markdown = "| Column | Column |\r\n| - | - |\r\n| A | B |";
            string expected = "<table>\n<thead>\n<tr>\n<th>Column</th>\n<th>Column</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>A</td>\n<td>B</td>\n</tr>\n</tbody>\n</table>";
            string result = new MarkdownUtil("https://kaylumah.nl").ToHtml(markdown);
            result
                .Should()
                .Be(expected);
        }

        [Fact]
        public void Test3()
        {
            string markdown = @"
| | | |
|-|-|-|
|__Bold Key__| Value1 |
| Normal Key | Value2 |
".Trim();
            string expected = "<table>\n<thead>\n<tr>\n<th></th>\n<th></th>\n<th></th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td><strong>Bold Key</strong></td>\n<td>Value1</td>\n<td></td>\n</tr>\n<tr>\n<td>Normal Key</td>\n<td>Value2</td>\n<td></td>\n</tr>\n</tbody>\n</table>";
            string result = new MarkdownUtil("https://kaylumah.nl").ToHtml(markdown);
            result
                .Should()
                .Be(expected);
        }

        [Fact]
        public void Test4()
        {
            string markdown = @"
```cs
public class Program
{
	public static void Main(string[] args) {}
}
```
".Trim();
            string expected = "<pre><code class=\"language-cs\">public class Program\n{\n\tpublic static void Main(string[] args) {}\n}\n</code></pre>";
            string result = new MarkdownUtil("https://kaylumah.nl").ToHtml(markdown);
            result
                .Should()
                .Be(expected);
        }

        [Fact]
        public void Test5()
        {
            string markdown = ":smiley:".Trim();
            string expected = "<p>:smiley:</p>";
            string result = new MarkdownUtil("https://kaylumah.nl").ToHtml(markdown);
            result
                .Should()
                .Be(expected);
        }

        [Fact]
        public void Test6()
        {
            string result = new MarkdownUtil("https://kaylumah.nl").ToHtml(
    @"# heading one
## heading two
### heading three
#### heading four
##### heading five
###### heading six");
            HtmlDocument pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(result);

            HtmlNode root = pageDoc.DocumentNode;
            List<HtmlNode> nodes = root.Descendants()
                .Where(n => n.NodeType == HtmlNodeType.Element)
                .ToList();
            nodes.Count.Should().Be(12);

            nodes.ElementAt(0)
                .Id.Should().Be("heading-one");
            nodes.ElementAt(0)
                .Name.Should().Be("h1");

            nodes.ElementAt(2)
                .Id.Should().Be("heading-two");
            nodes.ElementAt(2)
                .Name.Should().Be("h2");

            nodes.ElementAt(4)
                .Id.Should().Be("heading-three");
            nodes.ElementAt(4)
                .Name.Should().Be("h3");

            nodes.ElementAt(6)
                .Id.Should().Be("heading-four");
            nodes.ElementAt(6)
                .Name.Should().Be("h4");

            nodes.ElementAt(8)
                .Id.Should().Be("heading-five");
            nodes.ElementAt(8)
                .Name.Should().Be("h5");

            nodes.ElementAt(10)
                .Id.Should().Be("heading-six");
            nodes.ElementAt(10)
                .Name.Should().Be("h6");
        }
    }
}
