// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Xunit;

namespace Test.Unit.Component.Manager.Site
{
    public class BasePageConverterTests
    {
        readonly string _BaseUrl;
        readonly Guid _SiteGuid;

        public BasePageConverterTests()
        {
            _BaseUrl = "https://localhost";
            _SiteGuid = Guid.Empty;
        }

        [Fact]
        public void EmptyFileList_ReturnsEmptyList()
        {
            TextFile[] files = [];
            List<BasePage> result = BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            result.Should().BeEmpty();
        }

        [Fact]
        public void EmptyFile_Skipped()
        {
            FileMetaData fileMetaData = CreateFileMetaData();
            TextFile textFile = CreateTextFile(fileMetaData);
            TextFile[] files = [textFile];
            List<BasePage> result = BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            result.Should().BeEmpty();
        }

        [Fact]
        public void UnknownType_Throws()
        {
            FileMetaData fileMetaData = CreateFileMetaData("UnknownType");
            TextFile textFile = CreateTextFile(fileMetaData);
            TextFile[] files = [textFile];
            Action act = () => BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void StaticTextFile_MapsTo_StaticContent()
        {
            FileMetaData fileMetaData = CreateFileMetaData("Static", "https://localhost");
            TextFile textFile = CreateTextFile(fileMetaData);
            TextFile[] files = [textFile];
            List<BasePage> result = BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            result.Should().HaveCount(1);
            StaticContent page = result[0].Should().BeOfType<StaticContent>().Subject;
        }
        
        [Fact]
        public void PageTextFile_MapsTo_()
        {
            FileMetaData fileMetaData = CreateFileMetaData("Page", "https://localhost");
            TextFile textFile = CreateTextFile(fileMetaData);
            TextFile[] files = [textFile];
            List<BasePage> result = BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            result.Should().HaveCount(1);
            PageMetaData page = result[0].Should().BeOfType<PageMetaData>().Subject;
        }
        
        [Fact]
        public void AnnouncementTextFile_MapsTo_()
        {
            FileMetaData fileMetaData = CreateFileMetaData("Announcement", "https://localhost");
            TextFile textFile = CreateTextFile(fileMetaData);
            TextFile[] files = [textFile];
            List<BasePage> result = BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            result.Should().HaveCount(1);
            PageMetaData page = result[0].Should().BeOfType<PageMetaData>().Subject;
        }
        
        [Fact]
        public void ArticleTextFile_MapsTo_()
        {
            FileMetaData fileMetaData = CreateFileMetaData("Article", "https://localhost");
            TextFile textFile = CreateTextFile(fileMetaData);
            TextFile[] files = [textFile];
            List<BasePage> result = BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            result.Should().HaveCount(1);
            ArticlePublicationPageMetaData page = result[0].Should().BeOfType<ArticlePublicationPageMetaData>().Subject;
        }
        
        [Fact]
        public void TalkTextFile_MapsTo_()
        {
            FileMetaData fileMetaData = CreateFileMetaData("Talk", "https://localhost");
            TextFile textFile = CreateTextFile(fileMetaData);
            TextFile[] files = [textFile];
            List<BasePage> result = BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            result.Should().HaveCount(1);
            TalkPublicationPageMetaData page = result[0].Should().BeOfType<TalkPublicationPageMetaData>().Subject;
        }

        FileMetaData CreateFileMetaData(string? type = null, string? uri = null)
        {
            FileMetaData fileMetaData = new();

            if (type != null)
            {
                fileMetaData["type"] = type;
            }

            if (uri != null)
            {
                fileMetaData["uri"] = uri;
            }

            return fileMetaData;
        }

        TextFile CreateTextFile(FileMetaData fileMetaData, string? encoding = null, byte[]? contents = null)
        {
            if (contents == null)
            {
                contents = [];
            }

            if (encoding == null)
            {
                encoding = "UTF-8";
            }

            TextFile file = new TextFile(fileMetaData, contents, encoding);
            return file;
        }
    }
}