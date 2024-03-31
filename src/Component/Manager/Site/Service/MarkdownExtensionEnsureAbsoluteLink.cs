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
    class MarkdownExtensionEnsureAbsoluteLink : IMarkdownExtension
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
            if (linkInline.Url == null)
            {
                return false;
            }

            LinkInline.GetUrlDelegate? existingDynamicUrl = linkInline.GetDynamicUrl;
            linkInline.GetDynamicUrl = () =>
            {
                string escapeUrl;
                if (existingDynamicUrl != null)
                {
                    string existingResult = existingDynamicUrl();
                    escapeUrl = existingResult ?? linkInline.Url;
                }
                else
                {
                    escapeUrl = linkInline.Url;
                }

                bool success = Uri.TryCreate(escapeUrl, UriKind.RelativeOrAbsolute, out Uri? parsedResult);
                if (success == false || parsedResult == null)
                {
                    throw new ArgumentException("Failed to create URI");
                }

                string result;
                if (parsedResult.IsAbsoluteUri)
                {
                    result = escapeUrl;
                }
                else
                {
                    if (escapeUrl.StartsWith('#'))
                    {
                        result = escapeUrl;
                    }
                    else
                    {
                        Uri uri = GlobalFunctions.AbsoluteUri(escapeUrl);
                        result = uri.ToString();
                    }
                }

                return result;
            };

            return false;
        }
    }
}