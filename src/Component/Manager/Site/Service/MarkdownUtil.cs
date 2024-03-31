// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using Markdig;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    public static class MarkdownUtil
    {
        public static string Transform(string source)
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

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
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

            MarkdownDocument doc = Markdown.Parse(source, pipeline);

            // Process headings to insert an intermediate LinkInline
            IEnumerable<HeadingBlock> blocks = doc.Descendants<HeadingBlock>();
            foreach (HeadingBlock headingBlock in blocks)
            {
                LinkInline inline = new LinkInline($"#{headingBlock.GetAttributes().Id}", null!);
                ContainerInline previousInline = headingBlock.Inline!;
                headingBlock.Inline = null;
                inline.AppendChild(previousInline);
                headingBlock.Inline = inline;
            }

            IEnumerable<LinkInline> anchorTags = doc.Descendants<LinkInline>();
            foreach (LinkInline anchor in anchorTags)
            {
                string anchorUrl = anchor.Url!;
                if (!anchor.IsImage)
                {
                    bool isRelative = anchorUrl.StartsWith('/');
                    if (isRelative)
                    {
                        Uri uri = GlobalFunctions.AbsoluteUri(anchorUrl);
                        anchorUrl = uri.ToString();
                        anchor.Url = anchorUrl;
                    }

                    if (!anchorUrl.StartsWith(GlobalFunctions.Url.Value!, StringComparison.Ordinal))
                    {
                        anchor.GetAttributes().AddClass("external");
                    }
                }

                // TODO disable pending Medium response...
                if (anchor.IsImage)
                {
                    if (anchorUrl.StartsWith("/assets", StringComparison.Ordinal))
                    {
                        anchor.Url = GlobalFunctions.Url.Value + anchor.Url;
                    }
                }
            }

            // Render the doc
            StringWriter writer = new StringWriter();
            HtmlRenderer renderer = new HtmlRenderer(writer);
            pipeline.Setup(renderer);
            renderer.Render(doc);

            string result = writer.ToString().Trim();
            return result;
        }
    }

    public static class MarkdownPipelineBuilderExtensions
    {
#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static MarkdownPipelineBuilder UseLinkExtension(this MarkdownPipelineBuilder pipeline)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            OrderedList<IMarkdownExtension> extensions;
            extensions = pipeline.Extensions;

            if (!extensions.Contains<LinkExtension>())
            {
                LinkExtension ext = new LinkExtension();
                extensions.Add(ext);
            }

            return pipeline;
        }
    }

    public class LinkExtension : IMarkdownExtension
    {
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public void Setup(MarkdownPipelineBuilder pipeline)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            pipeline.DocumentProcessed += Pipeline_DocumentProcessed;
        }

        void Pipeline_DocumentProcessed(MarkdownDocument document)
        {
            IEnumerable<MarkdownObject> descendants = document.Descendants();
            foreach (MarkdownObject node in descendants)
            {
                if (node is Inline)
                {
                    if (node is LinkInline linkInlineNode)
                    {
                        string uri = linkInlineNode.Url!;
                        if (!uri.StartsWith("https://kaylumah.nl", StringComparison.Ordinal))
                        {
                            linkInlineNode.GetAttributes().AddClass("img-fluid");
                        }
                    }
                }
            }
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            // throw new System.NotImplementedException();
        }
    }
}