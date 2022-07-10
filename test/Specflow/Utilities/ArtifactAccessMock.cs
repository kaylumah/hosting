// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Xml;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Moq;

namespace Test.Specflow.Utilities;

public static class ArtifactAccessMockExtensions
{
    public static byte[] GetArtifactContents(this ArtifactAccessMock artifactAccess, string path)
    {
        var bytes = artifactAccess
            .Artifacts
            .SingleOrDefault(x => path.Equals(x.Path))?.Contents ?? Array.Empty<byte>();
        return bytes;
    }

    public static SyndicationFeed GetFeedArtifact(this ArtifactAccessMock artifactAccess, string path = "feed.xml")
    {
        var atomFeedXmlBytes = artifactAccess.GetArtifactContents(path);
        using var stream = new MemoryStream(atomFeedXmlBytes);
        using var xmlReader = XmlReader.Create(stream);
        var feed = SyndicationFeed.Load(xmlReader);
        return feed;
    }
}

public class ArtifactAccessMock : StrictMock<IArtifactAccess>
{
    private readonly List<StoreArtifactsRequest> _storeArtifactsRequests = new();
    public ReadOnlyCollection<StoreArtifactsRequest> StoreArtifactRequests => new(_storeArtifactsRequests);
    public ReadOnlyCollection<Artifact> Artifacts => new(StoreArtifactRequests.SelectMany(x => x.Artifacts).ToList());

    public ArtifactAccessMock()
    {
        SetupStore();
    }

    public void SetupStore()
    {
        Setup(artifactAccess =>
                artifactAccess.Store(It.IsAny<StoreArtifactsRequest>()))
            .Callback((StoreArtifactsRequest request) =>
            {
                _storeArtifactsRequests.Add(request);
            })
            .Returns(Task.CompletedTask);
    }
}
