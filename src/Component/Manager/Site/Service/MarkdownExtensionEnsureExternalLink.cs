// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    class MarkdownExtensionEnsureExternalLink : IMarkdownExtension
    {
        readonly string _BaseUrl;

        public MarkdownExtensionEnsureExternalLink(string baseUrl)
        {
            _BaseUrl = baseUrl;
        }

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
            if (linkInline.Url == null)
            {
                return false;
            }

            bool success = Uri.TryCreate(linkInline.Url, UriKind.RelativeOrAbsolute, out Uri? parsedResult);
            if (success == false || parsedResult == null)
            {
                return false;
            }

            bool notAbsolute = parsedResult.IsAbsoluteUri == false;
            if (notAbsolute)
            {
                return false;
            }

            Uri? uri;
            // Only process absolute Uri
            if (!Uri.TryCreate(linkInline.Url, UriKind.RelativeOrAbsolute, out uri) || !uri.IsAbsoluteUri)
            {
                return false;
            }

            RenderTargetAttribute(linkInline, uri);

            return false;
        }

        void RenderTargetAttribute(LinkInline linkInline, Uri uri)
        {
            string uriAsString = uri.ToString();
            bool isExternal = uriAsString.StartsWith(_BaseUrl, StringComparison.Ordinal) == false;
            if (isExternal)
            {
                linkInline.GetAttributes().AddClass("external");
            }

            // linkInline.SetAttributes(new HtmlAttributes() { Properties = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("target", "_blank"), new KeyValuePair<string, string>("rel", "noopener"), } });
        }
    }
}