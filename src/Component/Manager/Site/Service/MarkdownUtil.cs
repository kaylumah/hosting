// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Markdig;

namespace Kaylumah.Ssg.Utilities
{

    public class MarkdownUtil
    {
        readonly string _BaseUrl;

        public MarkdownUtil(string baseUrl)
        {
            _BaseUrl = baseUrl;
        }

        public string ToHtml(string source)
        {
            MarkdownPipeline pipeline = BuildPipeline();
            string intermediateResult = Markdown.ToHtml(source, pipeline);
            string result = intermediateResult.Trim();
            return result;
        }

        public string ToText(string source)
        {
            MarkdownPipeline pipeline = BuildPipeline();
            string intermediateResult = Markdown.ToPlainText(source, pipeline);
            string result = intermediateResult.Trim();
            return result;
        }

        MarkdownPipeline BuildPipeline()
        {
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/YamlSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AutoIdentifierSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/PipeTableSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/GenericAttributesSpecs.md

            MarkdownExtensionEnsureAbsoluteLink markdownExtensionEnsureAbsoluteLink = new MarkdownExtensionEnsureAbsoluteLink(_BaseUrl);
            MarkdownExtensionEnsureExternalLink markdownExtensionEnsureExternalLink = new MarkdownExtensionEnsureExternalLink(_BaseUrl);
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter() // needed to remove any frontmatter
                .UseAutoIdentifiers() // used for clickable headers
                .UsePipeTables() // support for tables
                .UseGenericAttributes() // support for inline attributes (like width, height)
                .Use(markdownExtensionEnsureAbsoluteLink)
                .Use(markdownExtensionEnsureExternalLink)
                .Use<MarkdownExtensionClickableHeaderLink>()
                .Use<MarkdownExtensionUsePictures>()
                .Build();
            return pipeline;
        }
    }
}