// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Kaylumah.Ssg.Manager.Site.Service;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    class MarkdownExtensionFixInlineLink : IMarkdownExtension
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
}