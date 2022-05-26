// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service;

public class SiteManager : ISiteManager
{
    private readonly IArtifactAccess _artifactAccess;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly IFileProcessor _fileProcessor;
    private readonly SiteInfo _siteInfo;
    private readonly ITransformationEngine _transformationEngine;
    private readonly SiteMetadataFactory _siteMetadataFactory;
    private readonly FeedGenerator _feedGenerator;

    public SiteManager(
        IFileProcessor fileProcessor,
        IArtifactAccess artifactAccess,
        IFileSystem fileSystem,
        ILogger<SiteManager> logger,
        SiteInfo siteInfo,
        ITransformationEngine transformationEngine,
        SiteMetadataFactory siteMetadataFactory,
        FeedGenerator feedGenerator
        )
    {
        _siteMetadataFactory = siteMetadataFactory;
        _fileProcessor = fileProcessor;
        _artifactAccess = artifactAccess;
        _fileSystem = fileSystem;
        _logger = logger;
        _siteInfo = siteInfo;
        _transformationEngine = transformationEngine;
        _feedGenerator = feedGenerator;
    }

    private Artifact[] CreateFeedArtifacts(SiteMetaData siteMetaData)
    {
        var result = new List<Artifact>();
        var feed = _feedGenerator.Create(siteMetaData);
        var bytes = feed
            .SaveAsAtom10();    
        result.Add(new Artifact
        { 
            Contents = bytes,
            Path = "feed.xml"
        });
        return result.ToArray();
    }

    public async Task GenerateSite(GenerateSiteRequest request)
    {
        GlobalFunctions.Instance.Url = _siteInfo.Url;
        GlobalFunctions.Instance.BaseUrl = _siteInfo.BaseUrl;
        var siteGuid = _siteInfo.Url.CreateSiteGuid();

        var processed = await _fileProcessor.Process(new FileFilterCriteria
        {
            DirectoriesToSkip = new string[] {
                    request.Configuration.LayoutDirectory,
                    request.Configuration.PartialsDirectory,
                    request.Configuration.DataDirectory,
                    request.Configuration.AssetDirectory
                },
            FileExtensionsToTarget = _siteInfo.SupportedFileExtensions.ToArray()
        });

        var pageMetadatas = processed
            .ToPages(siteGuid);
        var siteMetadata = _siteMetadataFactory
            .EnrichSite(
                request.Configuration,
                siteGuid,
                pageMetadatas.ToList()
        );

        var requests = pageMetadatas
            .Select(pageMetadata => new MetadataRenderRequest {
                Metadata = new RenderData
                {
                    Site =  siteMetadata,
                    Page = pageMetadata
                },
                Template = pageMetadata.GetValue<string>("layout")
            })
            .ToArray();
        var renderResults = await _transformationEngine.Render(requests);


        var artifacts = processed.Select((t, i) =>
        {
            var renderResult = renderResults[i];
            return new Artifact
            {
                Path = $"{t.MetaData.Uri}",
                Contents = Encoding.UTF8.GetBytes(renderResult.Content)
            };
        }).ToList();

        var feedArtifacts = CreateFeedArtifacts(siteMetadata);
        artifacts.AddRange(feedArtifacts);


        var assets = _fileSystem
            .GetFiles(Path.Combine("_site", request.Configuration.AssetDirectory), true)
            .Where(x => !x.IsDirectory());

        var env = Path.Combine(Environment.CurrentDirectory, "_site") + Path.DirectorySeparatorChar;

        artifacts.AddRange(assets.Select(asset =>
        {
            return new Artifact
            {
                Path = asset.FullName.Replace(env, ""),
                Contents = _fileSystem.GetFileBytes(asset.FullName)
            };
        }));

        await _artifactAccess.Store(new StoreArtifactsRequest
        {
            Artifacts = artifacts.ToArray(),
            OutputLocation = new FileSystemOutputLocation()
            {
                Clean = false,
                Path = request.Configuration.Destination
            }
        });
    }
}
