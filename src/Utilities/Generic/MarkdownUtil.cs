// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.IO;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    public class MarkdownUtil
    {
        public string Transform(string source)
        {
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/YamlSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/BootstrapSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/EmphasisExtraSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/DefinitionListSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/FootnotesSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AutoLinks.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/ListExtraSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/MediaSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AbbreviationSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/HardlineBreakSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/FigureFooterAndCiteSpecs.md
            // https://github.com/ilich/Markdig.Prism/blob/main/src/Markdig.Prism/PrismCodeBlockRenderer.cs

            var pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/EmojiSpecs.md
                //.UseEmojiAndSmiley(new Markdig.Extensions.Emoji.EmojiMapping(new Dictionary<string, string>() { { ":smiley:", "♥" } }, new Dictionary<string, string>()))

                // UseAdvancedExtensions 2021-01-25
                // .UseAbbreviations()
                // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AutoIdentifierSpecs.md
                .UseAutoIdentifiers()
                // .UseCitations()
                // .UseCustomContainers()
                // .UseDefinitionLists()
                // .UseEmphasisExtras()
                // .UseFigures()
                // .UseFooters()
                // .UseFootnotes()
                //.UseGridTables()
                // .UseMathematics()
                // .UseMediaLinks()
                // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/PipeTableSpecs.md
                .UsePipeTables()
                // .UseListExtras()
                // .UseTaskLists()
                // .UseDiagrams()
                // .UseAutoLinks()
                // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/GenericAttributesSpecs.md
                .UseGenericAttributes()
                .Build();

            var doc = Markdown.Parse(source, pipeline);

            // Process headings to insert an intermediate LinkInline
            foreach (var headingBlock in doc.Descendants<HeadingBlock>())
            {
                var inline = new LinkInline($"#{headingBlock.GetAttributes().Id}", null);
                var previousInline = headingBlock.Inline;
                headingBlock.Inline = null;
                inline.AppendChild(previousInline);
                headingBlock.Inline = inline;
            }

            var anchorTags = doc.Descendants<LinkInline>();
            foreach (var anchor in anchorTags)
            {
                if (anchor is LinkInline link && !link.IsImage)
                {
                    if (!anchor.Url.StartsWith(GlobalFunctions.Instance.Url))
                    {
                        link.GetAttributes().AddClass("external");
                    }

                }

                // TODO disable pending Medium response...
                // if (anchor is LinkInline imageLink && imageLink.IsImage)
                // {
                //     if (imageLink.Url.StartsWith("/assets"))
                //     {
                //         imageLink.Url = GlobalFunctions.Instance.Url + imageLink.Url;
                //     }
                // }
            }

            // Render the doc
            var writer = new StringWriter();
            var renderer = new HtmlRenderer(writer);
            pipeline.Setup(renderer);
            renderer.Render(doc);

            return writer.ToString().Trim();
        }
    }
}