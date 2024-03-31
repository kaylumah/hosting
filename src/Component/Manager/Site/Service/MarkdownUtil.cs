// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using Kaylumah.Ssg.Manager.Site.Service;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    class PictureInline : IMarkdownExtension
    {
        void IMarkdownExtension.Setup(MarkdownPipelineBuilder pipeline)
        {
            // Empty on purpose
        }

        void IMarkdownExtension.Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                LinkInlineRenderer? inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
                inlineRenderer?.TryWriters.Add(TryLinkInlineRenderer);
            }
        }

        bool TryLinkInlineRenderer(HtmlRenderer renderer, LinkInline linkInline)
        {
            if (linkInline == null)
            {
                return false;
            }

            if (linkInline.IsImage == false)
            {
                return false;
            }

            // handle .gif?

            renderer.Write("<picture>");
            WriteImageTag(renderer, linkInline, ".webp", "image/webp");
            WriteImageTag(renderer, linkInline, string.Empty);
            renderer.Write("</picture>");
            return true;
        }

        void WriteImageTag(HtmlRenderer renderer, LinkInline link, string suffix, string? type = null)
        {
            renderer.Write(string.IsNullOrWhiteSpace(type) ? $"<img loading=\"lazy\" src=\"" : $"<source type=\"{type}\" srcset=\"");
            string escapeUrl = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url! : link.Url!;
            renderer.WriteEscapeUrl($"{escapeUrl}{suffix}");
            renderer.Write("\"");
            renderer.WriteAttributes(link);
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write(" alt=\"");
            }

            bool wasEnableHtmlForInline = renderer.EnableHtmlForInline;
            renderer.EnableHtmlForInline = false;
            renderer.WriteChildren(link);
            renderer.EnableHtmlForInline = wasEnableHtmlForInline;
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("\"");
            }

            if (renderer.EnableHtmlForInline)
            {
                renderer.Write(" />");
            }
        }
    }

    public static class MarkdownUtil
    {
        public static string Transform(string source)
        {
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/YamlSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AutoIdentifierSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/PipeTableSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/GenericAttributesSpecs.md

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseAutoIdentifiers()
                .UsePipeTables()
                .UseGenericAttributes()
                .Build();

            MarkdownDocument doc = Markdown.Parse(source, pipeline);
            ModifyHeaders(doc);
            ModifyLinks(doc);

            // Render the doc
            StringWriter writer = new StringWriter();
            HtmlRenderer renderer = new HtmlRenderer(writer);
            pipeline.Setup(renderer);
            renderer.Render(doc);
            string intermediateResult = writer.ToString();
            // string intermediateResult = Markdown.ToHtml(doc, pipeline);
            string result = intermediateResult.Trim();
            return result;
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