﻿// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Kaylumah.Ssg.Access.Artifact.Hosting;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.Unit;

public class ArtifactAccessTests
{
    [Fact]
    public async Task Test_ArtifactAccess_StoreWithoutClean()
    {
        var fileSystemMock = new MockFileSystem();
        var currentCount = fileSystemMock.AllDirectories.Count();

        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection()
            .AddArtifactAccess(configuration)
            .AddSingleton<IFileSystem>(fileSystemMock)
            .BuildServiceProvider();

        var sut = serviceProvider.GetRequiredService<IArtifactAccess>();

        await sut.Store(new StoreArtifactsRequest
        {
            OutputLocation = new FileSystemOutputLocation()
            {
                Path = "dist",
                Clean = false
            },
            Artifacts = new Artifact[] {
                    new Artifact()
                    {
                        Path = "test.txt",
                        Contents = Encoding.UTF8.GetBytes(string.Empty)
                    }
                }
        });
        var createdCount = fileSystemMock.AllDirectories.Count() - currentCount;
        createdCount.Should().Be(1);
    }

    [Fact]
    public async Task Test_ArtifactAccess_StoreWithSubdirectory()
    {
        var fileSystemMock = new MockFileSystem();
        var currentCount = fileSystemMock.AllDirectories.Count();

        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection()
            .AddArtifactAccess(configuration)
            .AddSingleton<IFileSystem>(fileSystemMock)
            .BuildServiceProvider();

        var sut = serviceProvider.GetRequiredService<IArtifactAccess>();

        await sut.Store(new StoreArtifactsRequest
        {
            OutputLocation = new FileSystemOutputLocation()
            {
                Path = "dist",
                Clean = false
            },
            Artifacts = new Artifact[] {
                    new Artifact()
                    {
                        Path = Path.Combine("assets", "test.txt"),
                        Contents = Encoding.UTF8.GetBytes(string.Empty)
                    }
                }
        });
        var createdCount = fileSystemMock.AllDirectories.Count() - currentCount;
        createdCount.Should().Be(2);
    }

    [Fact]
    public async Task Test_ArtifactAccess_StoreWithClean()
    {
        var fileSystemMock = new MockFileSystem();
        var currentCount = fileSystemMock.AllDirectories.Count();

        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection()
            .AddArtifactAccess(configuration)
            .AddSingleton<IFileSystem>(fileSystemMock)
            .BuildServiceProvider();

        var sut = serviceProvider.GetRequiredService<IArtifactAccess>();

        await sut.Store(new StoreArtifactsRequest
        {
            OutputLocation = new FileSystemOutputLocation()
            {
                Path = "dist",
                Clean = true
            },
            Artifacts = new Artifact[] {
                    new Artifact()
                    {
                        Path = "test.txt",
                        Contents = Encoding.UTF8.GetBytes(string.Empty)
                    }
                }
        });
        var createdCount = fileSystemMock.AllDirectories.Count() - currentCount;
        createdCount.Should().Be(1);
    }
}
