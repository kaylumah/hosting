// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Linq;
using FluentAssertions;
using HtmlAgilityPack;
using Kaylumah.Ssg.Utilities;
using Markdig;
using Xunit;

namespace Test.Specflow.FormerXunit
{
    public class MarkdownTests
    {
        [Fact]
        public void Test1()
        {
            string markdown = "# Header 1 \r\n## Header 2\r\n### Header 3\r\n#### Header 4\r\n##### Header 5\r\n###### Header 6";
            string expected = "<h1 id=\"header-1\"><a href=\"#header-1\">Header 1</a></h1>\n<h2 id=\"header-2\"><a href=\"#header-2\">Header 2</a></h2>\n<h3 id=\"header-3\"><a href=\"#header-3\">Header 3</a></h3>\n<h4 id=\"header-4\"><a href=\"#header-4\">Header 4</a></h4>\n<h5 id=\"header-5\"><a href=\"#header-5\">Header 5</a></h5>\n<h6 id=\"header-6\"><a href=\"#header-6\">Header 6</a></h6>";
            string result = MarkdownUtil.Transform(markdown);
            result
                .Should()
                .Be(expected);
        }

        [Fact]
        public void Test2()
        {
            string markdown = "| Column | Column |\r\n| - | - |\r\n| A | B |";
            string expected = "<table>\n<thead>\n<tr>\n<th>Column</th>\n<th>Column</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>A</td>\n<td>B</td>\n</tr>\n</tbody>\n</table>";
            string result = MarkdownUtil.Transform(markdown);
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
            string result = MarkdownUtil.Transform(markdown);
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
            string result = MarkdownUtil.Transform(markdown);
            result
                .Should()
                .Be(expected);
        }

        [Fact]
        public void Test5()
        {
            string markdown = ":smiley:".Trim();
            string expected = "<p>:smiley:</p>";
            string result = MarkdownUtil.Transform(markdown);
            result
                .Should()
                .Be(expected);
        }

        [Fact]
        public void Test6()
        {
            string result = MarkdownUtil.Transform(
    @"# heading one
## heading two
### heading three
#### heading four
##### heading five
###### heading six");
            HtmlDocument pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(result);

            HtmlNode root = pageDoc.DocumentNode;
            System.Collections.Generic.List<HtmlNode> nodes = root.Descendants()
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

        [Fact]
        public void Test7()
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseLinkExtension()
                .Build();

            string input = @"[link](https://kaylumah.nl)";
            string output = Markdown.ToHtml(input, pipeline);
        }

        [Fact]
        public void Test8()
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseLinkExtension()
                .Build();

            string input = @"[link](https://google.com)";
            string output = Markdown.ToHtml(input, pipeline);
        }

    }
}
