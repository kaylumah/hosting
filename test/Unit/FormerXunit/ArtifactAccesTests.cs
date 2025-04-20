// Copyright (c) Kaylumah, 2025. All rights reserved.
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

namespace Test.Unit.FormerXunit
{
    public class ArtifactAccessTests
    {
        [Fact]
        public async Task Test_ArtifactAccess_Store()
        {
            MockFileSystem fileSystemMock = new MockFileSystem();
            int currentCount = fileSystemMock.AllDirectories.Count();

            IConfigurationRoot configuration = new ConfigurationBuilder().Build();
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddTestLogging()
                .AddArtifactAccess(configuration)
                .AddSingleton<IFileSystem>(fileSystemMock)
                .BuildServiceProvider();

            IArtifactAccess sut = serviceProvider.GetRequiredService<IArtifactAccess>();

            FileSystemOutputLocation fileSystemOutputLocation = new FileSystemOutputLocation("dist");
            byte[] emptyBytes = Encoding.UTF8.GetBytes(string.Empty);
            Artifact artifact = new Artifact("test.txt", emptyBytes);
            Artifact[] artifacts = new Artifact[1];
            artifacts[0] = artifact;
            StoreArtifactsRequest storeArtifactsRequest = new StoreArtifactsRequest(fileSystemOutputLocation, artifacts);
            await sut.Store(storeArtifactsRequest);
            int createdCount = fileSystemMock.AllDirectories.Count() - currentCount;
            createdCount.Should().Be(1);
        }

        [Fact]
        public async Task Test_ArtifactAccess_StoreWithSubdirectory()
        {
            MockFileSystem fileSystemMock = new MockFileSystem();
            int currentCount = fileSystemMock.AllDirectories.Count();

            IConfigurationRoot configuration = new ConfigurationBuilder().Build();
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddArtifactAccess(configuration)
                .AddSingleton<IFileSystem>(fileSystemMock)
                .BuildServiceProvider();

            IArtifactAccess sut = serviceProvider.GetRequiredService<IArtifactAccess>();

            FileSystemOutputLocation fileSystemOutputLocation = new FileSystemOutputLocation("dist");
            byte[] emptyBytes = Encoding.UTF8.GetBytes(string.Empty);
            string fileName = Path.Combine("assets", "test.txt");
            Artifact artifact = new Artifact(fileName, emptyBytes);
            Artifact[] artifacts = new Artifact[1];
            artifacts[0] = artifact;
            StoreArtifactsRequest storeArtifactsRequest = new StoreArtifactsRequest(fileSystemOutputLocation, artifacts);
            await sut.Store(storeArtifactsRequest);
            int createdCount = fileSystemMock.AllDirectories.Count() - currentCount;
            createdCount.Should().Be(2);
        }
    }
}
