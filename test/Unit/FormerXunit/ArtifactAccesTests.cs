// Copyright (c) Kaylumah, 2024. All rights reserved.
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
        public async Task Test_ArtifactAccess_StoreWithoutClean()
        {
            MockFileSystem fileSystemMock = new MockFileSystem();
            int currentCount = fileSystemMock.AllDirectories.Count();

            IConfigurationRoot configuration = new ConfigurationBuilder().Build();
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddArtifactAccess(configuration)
                .AddSingleton<IFileSystem>(fileSystemMock)
                .BuildServiceProvider();

            IArtifactAccess sut = serviceProvider.GetRequiredService<IArtifactAccess>();

            await sut.Store(new StoreArtifactsRequest(new FileSystemOutputLocation("dist", false), new Artifact[] {
                    new Artifact("test.txt", Encoding.UTF8.GetBytes(string.Empty))
                }));
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

            await sut.Store(new StoreArtifactsRequest(new FileSystemOutputLocation("dist", false), new Artifact[] {
                    new Artifact(Path.Combine("assets", "test.txt"), Encoding.UTF8.GetBytes(string.Empty))
                }
            ));
            int createdCount = fileSystemMock.AllDirectories.Count() - currentCount;
            createdCount.Should().Be(2);
        }

        [Fact]
        public async Task Test_ArtifactAccess_StoreWithClean()
        {
            MockFileSystem fileSystemMock = new MockFileSystem();
            int currentCount = fileSystemMock.AllDirectories.Count();

            IConfigurationRoot configuration = new ConfigurationBuilder().Build();
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddArtifactAccess(configuration)
                .AddSingleton<IFileSystem>(fileSystemMock)
                .BuildServiceProvider();

            IArtifactAccess sut = serviceProvider.GetRequiredService<IArtifactAccess>();

            await sut.Store(new StoreArtifactsRequest(new FileSystemOutputLocation("dist", true), new Artifact[] {
                    new Artifact("test.txt", Encoding.UTF8.GetBytes(string.Empty))
                }));
            int createdCount = fileSystemMock.AllDirectories.Count() - currentCount;
            createdCount.Should().Be(1);
        }
    }
}
