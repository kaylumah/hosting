// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Kaylumah.Ssg.Manager.Site.Service;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    //     class AbsoluteLinkConverter : IMarkdownExtension
    //     {
    //         public string BaseUrl
    //         { get; }
    //         public string Domain
    //         { get; }

    //         public AbsoluteLinkConverter(string baseUrl, string domain)
    //         {
    //             BaseUrl = baseUrl;
    //             Domain = domain;
    //         }

    //         public void Setup(MarkdownPipelineBuilder pipeline)
    //         {
    //         }

    //         public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    //         {
    //             HtmlRenderer? htmlRenderer = renderer as HtmlRenderer;
    //             if (htmlRenderer != null)
    //             {
    //                 LinkInlineRenderer? inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
    //                 inlineRenderer?.TryWriters.Add(TryLinkAbsoluteUrlWriter);
    //             }
    //         }
    //         private bool TryLinkAbsoluteUrlWriter(HtmlRenderer renderer, LinkInline linkInline)
    //         {
    //             LinkInline.GetUrlDelegate? prevDynamic = linkInline.GetDynamicUrl;
    //             linkInline.GetDynamicUrl = () => {
    //                 string escapeUrl = prevDynamic != null ? prevDynamic() ?? linkInline.Url! : linkInline.Url!;
    //                 if(!Uri.TryCreate(escapeUrl, UriKind.RelativeOrAbsolute, out var parsedResult))
    //                 {
    // #pragma warning disable CA2201 // Do not raise reserved exception types
    //                     throw new Exception($"Error making link for {escapeUrl} @ {BaseUrl}");
    // #pragma warning restore CA2201 // Do not raise reserved exception types
    //                 }
    //                 if(parsedResult.IsAbsoluteUri)
    //                 {
    //                     return escapeUrl;
    //                 }
    //                 UriBuilder uriBuilder = new UriBuilder(Domain);
    //                 if(!escapeUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase))
    //                 {
    //                     uriBuilder = uriBuilder.WithPathSegment($"/{BaseUrl}/{escapeUrl}");
    //                 }
    //                 else 
    //                 {
    //                     uriBuilder = uriBuilder.WithPathSegment(parsedResult.ToString());
    //                 }

    //                 string result = uriBuilder.Uri.ToString();
    //                 return result;
    //             };

    //             return false;
    //         }
    //     }

    class ClickableHeaderLink : IMarkdownExtension
    {
        void IMarkdownExtension.Setup(MarkdownPipelineBuilder pipeline)
        {
            // Empty on purpose
        }

        void IMarkdownExtension.Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                HeadingRenderer? headingRenderer = htmlRenderer.ObjectRenderers.FindExact<HeadingRenderer>();
                headingRenderer?.TryWriters.Add(TryHeadingBlockRenderer);
            }
        }

        bool TryHeadingBlockRenderer(HtmlRenderer renderer, HeadingBlock headingBlock)
        {
            LinkInline inline = new LinkInline($"#{headingBlock.GetAttributes().Id}", null!);
            ContainerInline previousInline = headingBlock.Inline!;
            headingBlock.Inline = null;
            inline.AppendChild(previousInline);
            headingBlock.Inline = inline;
            // renderer.Write(headingBlock);
            return false;
        }
    }

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

    class FixInline : IMarkdownExtension
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

        bool TryLinkInlineRenderer(HtmlRenderer renderer, LinkInline anchor)
        {
            string anchorUrl = anchor.Url!;
            if (!anchor.IsImage)
            {
                bool selfLink = anchorUrl.StartsWith('#');
                if (selfLink)
                {
                    return false;
                }

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

            return false;
        }
    }

    public static class MarkdownUtil
    {
        public static string ToHtml(string source)
        {
            MarkdownPipeline pipeline = BuildPipeline();

            MarkdownDocument doc = Markdown.Parse(source, pipeline);
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
            string intermediateResult = Markdown.ToPlainText(source, pipeline);
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
                .Use<FixInline>()
                .Use<ClickableHeaderLink>()
                .Build();
            return pipeline;
        }
    }
}