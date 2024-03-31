// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using Kaylumah.Ssg.Manager.Site.Service;
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
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseAutoIdentifiers()
                .UsePipeTables()
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
            // StringWriter writer = new StringWriter();
            // HtmlRenderer renderer = new HtmlRenderer(writer);
            // pipeline.Setup(renderer);
            // renderer.Render(doc);
            // string result = writer.ToString().Trim();
            string intermediateResult = Markdown.ToHtml(doc, pipeline);
            string result = intermediateResult; //.Trim();
            return result;
        }
    }
}