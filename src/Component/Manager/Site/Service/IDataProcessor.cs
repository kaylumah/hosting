// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static class Extensions
    {
        public static List<string> GetTags(this IEnumerable<PageMetaData> pages)
        {
            IEnumerable<PageMetaData> pagesWithTags = pages.HasTag();
            IEnumerable<PageMetaData> taggedArticles = pagesWithTags.IsArticle();
            IEnumerable<PageMetaData> taggedAnnouncements = pagesWithTags.IsAnnouncement();

            IEnumerable<string> tagsFromArticles = taggedArticles.SelectMany(article => article.Tags);
            IEnumerable<string> tagsFromAnnouncements = taggedAnnouncements.SelectMany(article => article.Tags);
            IEnumerable<string> allTags = tagsFromArticles.Union(tagsFromAnnouncements);
            IEnumerable<string> uniqueTags = allTags.Distinct();
            List<string> result = uniqueTags.ToList();
            return result;
        }
    }

    public interface IDataProcessor
    {
        bool IsApplicable(IFileSystemInfo file);

        void Execute(SiteMetaData siteMetaData, IFileSystemInfo file);
    }

    public interface IKnownFileProcessor : IDataProcessor
    {
        string KnownFileName
        { get; }

        bool IDataProcessor.IsApplicable(IFileSystemInfo file)
        {
            string fileName = file.Name;
            bool fileNameMatches = fileName.Equals(KnownFileName, StringComparison.Ordinal);
            return fileNameMatches;
        }
    }

    public interface IKnownExtensionProcessor : IDataProcessor
    {
        string KnownExtension
        { get; }

        bool IDataProcessor.IsApplicable(IFileSystemInfo file)
        {
            string extension = file.Extension;
            bool extensionMatches = extension.Equals(KnownExtension, StringComparison.Ordinal);
            return extensionMatches;
        }
    }

    public class YamlFileProcessor : IKnownExtensionProcessor
    {
        readonly IYamlParser _YamlParser;

        public YamlFileProcessor(IYamlParser yamlParser)
        {
            _YamlParser = yamlParser;
        }

        public string KnownExtension => ".yml";

        public void Execute(SiteMetaData siteMetaData, IFileSystemInfo file)
        {
            object result = _YamlParser.Parse<object>(file);
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            siteMetaData.Data[fileName] = result;
        }
    }

    public partial class TagFileProcessor : IKnownFileProcessor
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Warning,
            Message = "TagFile is missing `{Tags}`")]
        private partial void LogMissingTags(string tags);

        readonly ILogger _Logger;
        readonly IYamlParser _YamlParser;

        public string KnownFileName => Constants.KnownFiles.Tags;

        public TagFileProcessor(ILogger logger, IYamlParser yamlParser)
        {
            _Logger = logger;
            _YamlParser = yamlParser;
        }

        public void Execute(SiteMetaData siteMetaData, IFileSystemInfo file)
        {
            TagMetaDataCollection tagData = _YamlParser.Parse<TagMetaDataCollection>(file);
            List<string> tags = siteMetaData.Pages.GetTags();
            IEnumerable<string> otherTags = tagData.Keys.Except(tags);
            IEnumerable<string> unmatchedTags = tags
                .Except(tagData.Keys)
                .Concat(otherTags);
            string unmatchedTagsString = string.Join(",", unmatchedTags);
            LogMissingTags(unmatchedTagsString);
            siteMetaData.TagMetaData.AddRange(tagData);
            siteMetaData.Data["tags"] = siteMetaData.TagMetaData.Dictionary;
        }
    }

    public class OrganizationFileProcessor : IKnownFileProcessor
    {
        readonly IYamlParser _YamlParser;

        public OrganizationFileProcessor(IYamlParser yamlParser)
        {
            _YamlParser = yamlParser;
        }

        public string KnownFileName => Constants.KnownFiles.Organizations;

        public void Execute(SiteMetaData siteMetaData, IFileSystemInfo file)
        {
            OrganizationMetaDataCollection organizationData = _YamlParser.Parse<OrganizationMetaDataCollection>(file);
            siteMetaData.OrganizationMetaData.AddRange(organizationData);
            siteMetaData.Data["organizations"] = siteMetaData.OrganizationMetaData.Dictionary;
        }
    }

    public class AuthorFileProcessor : IKnownFileProcessor
    {
        readonly IYamlParser _YamlParser;

        public AuthorFileProcessor(IYamlParser yamlParser)
        {
            _YamlParser = yamlParser;
        }

        public string KnownFileName => Constants.KnownFiles.Authors;

        public void Execute(SiteMetaData siteMetaData, IFileSystemInfo file)
        {
            AuthorMetaDataCollection authorData = _YamlParser.Parse<AuthorMetaDataCollection>(file);
            siteMetaData.AuthorMetaData.AddRange(authorData);
            siteMetaData.Data["authors"] = siteMetaData.AuthorMetaData.Dictionary;
        }
    }
}
