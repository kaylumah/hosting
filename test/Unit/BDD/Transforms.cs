﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Reqnroll;
using Test.Unit.Entities;

namespace Test.Unit.BDD
{
    [Binding]
    public class Transforms
    {
        // Broken in Specflow to Reqnroll migration
        //[StepArgumentTransformation]
        //public static string ToNullableString(string value)
        //{
        //    return Constants.NullIndicator.Equals(value, System.StringComparison.Ordinal) ? null : value;
        //}

        [StepArgumentTransformation]
        public List<string> TransformToListOfString(string commaSeparatedList)
        {
            List<string> result = commaSeparatedList.Split(Constants.Separator).ToList();
            return result;
        }

        [StepArgumentTransformation]
        public static ArticleCollection ToArticles(Table table)
        {
            IEnumerable<Article> articles = table.CreateSet<Article>();
            ArticleCollection collection = new ArticleCollection();
            collection.AddRange(articles);
            return collection;
        }

        [StepArgumentTransformation]
        public static DefaultMetadatas ToDefaultMetadatas(Table table)
        {
            DefaultMetadatas defaultMetaDatas = new DefaultMetadatas();
            List<(string Scope, string Path, string Key, string Value)> items = table
                .CreateSet<(string Scope, string Path, string Key, string Value)>()
                .ToList();
            IEnumerable<IGrouping<string, (string Scope, string Path, string Key, string Value)>> groupedByScope = items.GroupBy(x => x.Scope);
            foreach (IGrouping<string, (string Scope, string Path, string Key, string Value)> scopeGroup in groupedByScope)
            {
                string scope = scopeGroup.Key;
                IEnumerable<IGrouping<string, (string Scope, string Path, string Key, string Value)>> groupedByPath = scopeGroup.GroupBy(x => x.Path);
                foreach (IGrouping<string, (string Scope, string Path, string Key, string Value)> pathGroup in groupedByPath)
                {
                    string path = pathGroup.Key;
                    FileMetaData fileMetaData = new FileMetaData();
                    foreach ((string Scope, string Path, string Key, string Value) item in pathGroup)
                    {
                        fileMetaData.Add(item.Key, item.Value);
                    }

                    DefaultMetadata defaultMetadata = new DefaultMetadata();
                    defaultMetadata.Path = path;
                    defaultMetadata.Scope = scope;
                    defaultMetadata.Values = fileMetaData;
                    defaultMetadata.Extensions = [".html"];
                    defaultMetaDatas.Add(defaultMetadata);
                }
            }

            return defaultMetaDatas;
        }

#pragma warning disable IDE0051
        [StepArgumentTransformation]
        static FileFilterCriteria ToFileFilterCriteria(Table table)
        {
            FileFilterCriteria criteria = table.CreateInstance<FileFilterCriteria>();
            criteria.RootDirectory = Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourceDirectory;
            return criteria;
        }
#pragma warning restore IDE0051
    }
}
