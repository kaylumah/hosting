// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    /// <summary>
    /// Inspired by
    /// https://github.com/TerribleDev/blog.terrible.dev/blob/a74dec639a0ced2aa67717774588eed783f01464/src/TerribleDev.Blog.Web/Factories/BlogFactory.cs#L9
    /// https://github.com/TerribleDev/blog.terrible.dev/blob/a74dec639a0ced2aa67717774588eed783f01464/src/TerribleDev.Blog.Web/MarkExtension/PictureInline.cs#L4
    /// </summary>
    class MarkdownExtensionUsePictures : IMarkdownExtension
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

            if(linkInline.Url!.EndsWith(".gif", System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if(linkInline.Url!.EndsWith(".svg", System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            renderer.Write("<picture>");
            WriteSourceTag(renderer, linkInline);
            WriteImgTag(renderer, linkInline);
            renderer.Write("</picture>");
            return true;
        }

        void WriteSourceTag(HtmlRenderer renderer, LinkInline link)
        {
            string escapeUrl = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url! : link.Url!;
            string webpUrl = $"{escapeUrl}.webp";
            renderer.Write($"<source type=\"image/webp\" srcset=\"{webpUrl}\">");
        }

        void WriteImgTag(HtmlRenderer renderer, LinkInline link)
        {
            // $"<img loading=\"lazy\" src=\""
            string webpUrl = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url! : link.Url!;
            renderer.Write($"<img src=\"{webpUrl}\"");
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
}