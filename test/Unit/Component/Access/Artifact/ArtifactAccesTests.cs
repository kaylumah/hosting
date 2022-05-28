// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using FluentAssertions;
using Kaylumah.Ssg.Access.Artifact.Hosting;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test.Unit.Mocks;
using Xunit;

namespace Test.Unit;

public class ArtifactAccessTests
{
    [Fact]
    public async Task Test_ArtifactAccess_StoreWithoutClean()
    {
        var fileSystemMock = new FileSystemMock();

        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection()
            .AddArtifactAccess(configuration)
            .AddSingleton(fileSystemMock.Object)
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
        fileSystemMock.CreatedDirectories.Count.Should().Be(1);
    }

    [Fact]
    public async Task Test_ArtifactAccess_StoreWithSubdirectory()
    {
        var fileSystemMock = new FileSystemMock();

        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection()
            .AddArtifactAccess(configuration)
            .AddSingleton(fileSystemMock.Object)
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
        fileSystemMock.CreatedDirectories.Count.Should().Be(1);
    }

    [Fact]
    public async Task Test_ArtifactAccess_StoreWithClean()
    {
        var fileSystemMock = new FileSystemMock();

        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection()
            .AddArtifactAccess(configuration)
            .AddSingleton(fileSystemMock.Object)
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
        fileSystemMock.CreatedDirectories.Count.Should().Be(1);
    }
}
