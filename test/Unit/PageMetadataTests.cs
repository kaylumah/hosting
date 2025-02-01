// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Ssg.Extensions.Metadata.Abstractions;
using Xunit;

namespace Test.Unit
{

#pragma warning disable

    public class PageMetadataTests
    {
        [Fact]
        public void PageMetaData_ShouldInitialize_FromDictionary()
        {
            // Arrange
            var initialData = new Dictionary<string, object?>
            {
                { "id", "123" },
                { "Title", "Test Page" },
                { "Description", "This is a test description" },
                { "Language", "en" },
                { "Sitemap", true },
                { "Published", new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero) },
                { "Modified", new DateTimeOffset(2024, 2, 15, 8, 0, 0, TimeSpan.Zero) },
                { "Tags", new List<string> { "test", "metadata" } }
            };

            // Act
            var page = new PageMetaData(initialData);

            // Assert
            page.Id.Should().Be(new PageId("123")); // Updated to use PageId struct
            page.Title.Should().Be("Test Page");
            page.Description.Should().Be("This is a test description");
            page.Language.Should().Be("en");
            page.Sitemap.Should().BeTrue();
            page.Published.Should().Be(new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero));
            page.Modified.Should().Be(new DateTimeOffset(2024, 2, 15, 8, 0, 0, TimeSpan.Zero));
            page.Tags.Should().Contain(new[] { "test", "metadata" });
        }

        [Fact]
        public void PageMetaData_ShouldUpdate_MutableProperties()
        {
            // Arrange
            var page = new PageMetaData(new Dictionary<string, object?>());

            // Act
            page.Id = new PageId("456");
            page.Content = "Updated content";
            page.Tags = new List<string> { "updated", "test" };

            // Assert
            page.Id.Should().Be(new PageId("456"));
            page.Content.Should().Be("Updated content");
            page.Tags.Should().Contain(new[] { "updated", "test" });
        }

        [Fact]
        public void PageMetaData_ShouldHandle_MissingValuesGracefully()
        {
            // Arrange
            var page = new PageMetaData(new Dictionary<string, object?>());

            // Act & Assert
            page.Id.Should().Be(new PageId(null)); // Default to empty PageId
            page.Title.Should().BeNull();
            page.Sitemap.Should().BeFalse(); // Default to false if missing
            page.Published.Should().Be(default); // Default to DateTimeOffset.MinValue
            page.Modified.Should().Be(default); // Default to DateTimeOffset.MinValue
            page.Tags.Should().BeEmpty(); // Default empty list
        }

        [Fact]
        public void PageMetaData_ShouldCompute_CanonicalUri_Correctly()
        {
            // Arrange
            var initialData = new Dictionary<string, object?>
            {
                { "BaseUri", "https://example.com/" },
                { "Uri", "index.html" }
            };
            var page = new PageMetaData(initialData);

            // Act
            var canonicalUri = page.CanonicalUri;

            // Assert
            canonicalUri.Should().Be(new Uri("https://example.com/")); // Should resolve to base URI
        }

        [Fact]
        public void PageMetaData_ShouldResolve_WebImage_Correctly()
        {
            // Arrange
            var initialData = new Dictionary<string, object?>
            {
                { "BaseUri", "https://example.com/" },
                { "Image", "images/pic.jpg" }
            };
            var page = new PageMetaData(initialData);

            // Act
            var webImage = page.WebImage;

            // Assert
            webImage.Should().Be(new Uri("https://example.com/images/pic.jpg")); // Correctly resolves full image path
        }
    }
}