// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Kaylumah.Ssg.Manager.Site.Service;
using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    public static class MarkdownUtil
    {
        public static string ToHtml(string source)
        {
            MarkdownPipeline pipeline = BuildPipeline();

            MarkdownDocument doc = Markdown.Parse(source, pipeline);
            ModifyHeaders(doc);
            ModifyLinks(doc);

            // Render the doc
            // StringWriter writer = new StringWriter();
            // HtmlRenderer renderer = new HtmlRenderer(writer);
            // pipeline.Setup(renderer);
            // renderer.Render(doc);
            // string intermediateResult = writer.ToString();
            string intermediateResult = Markdown.ToHtml(doc, pipeline);
            string result = intermediateResult.Trim();
            return result;
        }

        public static string ToText(string source)
        {
            MarkdownPipeline pipeline = BuildPipeline();

            MarkdownDocument doc = Markdown.Parse(source, pipeline);
            string intermediateResult = Markdown.ToHtml(doc, pipeline);
            string result = intermediateResult.Trim();
            return result;
        }

        static MarkdownPipeline BuildPipeline()
        {
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/YamlSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AutoIdentifierSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/PipeTableSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/GenericAttributesSpecs.md

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter() // needed to remove any frontmatter
                .UseAutoIdentifiers() // used for clickable headers
                .UsePipeTables() // support for tables
                .UseGenericAttributes() // support for inline attributes (like width, height)
                .Build();
            return pipeline;
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static void ModifyHeaders(MarkdownDocument doc)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
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
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static void ModifyLinks(MarkdownDocument doc)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
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
        }
    }
}