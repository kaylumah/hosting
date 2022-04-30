// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using FluentAssertions;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Access.Artifact.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test.Unit.Mocks;
using Xunit;

namespace Test.Unit;

public class ArtifactAccessTests
{
    [Fact]
    public async Task Test1()
    {
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection()
            .BuildServiceProvider();


        var fileSystemMock = new FileSystemMock();
        var loggerMock = new LoggerMock<ArtifactAccess>();
        var strategies = new IStoreArtifactsStrategy[] {
                new FileSystemStoreArtifactsStrategy(new LoggerMock<FileSystemStoreArtifactsStrategy>().Object, fileSystemMock.Object)
            };
        var sut = new ArtifactAccess(loggerMock.Object, strategies);
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
}
