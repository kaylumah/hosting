// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Text;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ssg.Extensions.Data.Yaml;

namespace Kaylumah.Ssg.Manager.Site.Service;

public class SiteManager : ISiteManager
{
    private readonly IArtifactAccess _artifactAccess;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly IFileProcessor _fileProcessor;
    private readonly IYamlParser _yamlParser;
    private readonly SiteInfo _siteInfo;
    private readonly ITransformationEngine _transformationEngine;

    public SiteManager(
        IFileProcessor fileProcessor,
        IArtifactAccess artifactAccess,
        IFileSystem fileSystem,
        IYamlParser yamlParser,
        ILogger<SiteManager> logger,
        IOptions<SiteInfo> options,
        ITransformationEngine transformationEngine
        )
    {
        _fileProcessor = fileProcessor;
        _artifactAccess = artifactAccess;
        _fileSystem = fileSystem;
        _yamlParser = yamlParser;
        _logger = logger;
        _siteInfo = options.Value;
        _transformationEngine = transformationEngine;
    }

    private void EnrichSiteWithData(SiteMetaData site, string dataDirectory)
    {
        var extensions = _siteInfo.SupportedDataFileExtensions.ToArray();
        var dataFiles = _fileSystem.GetFiles(dataDirectory)
            .Where(file => extensions.Contains(Path.GetExtension(file.Name)))
            .ToList();
        var data = new Dictionary<string, object>();
        foreach (var file in dataFiles)
        {
            var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            var raw = reader.ReadToEnd();
            var result = _yamlParser.Parse<object>(raw);
            data[Path.GetFileNameWithoutExtension(file.Name)] = result;
        }
        site.Data = data;
    }

    private void EnrichSiteWithTags(SiteMetaData site, List<PageMetaData> pages)
    {
        var tags = pages
            .Where(x => x.Tags != null)
            .Where(x => ContentType.Article.Equals(x.Type)) // filter out anything that is not an article
            .SelectMany(x => x.Tags)
            .Distinct();
        foreach (var tag in tags)
        {
            var tagFiles = pages.Where(x => x.Tags != null && x.Tags.Contains(tag)).ToArray();
            site.Tags.Add(tag, tagFiles);
        }
    }

    private void EnrichSiteWithYears(SiteMetaData site, List<PageMetaData> pages)
    {
        var years = pages
            .Where(x => x.ContainsKey("date"))
            .Where(x => ContentType.Article.Equals(x.Type)) // filter out anything that is not an article
            .Select(x => ((DateTimeOffset)x["date"]).Year)
            .Distinct();
        foreach (var year in years)
        {
            var yearFiles = pages.Where(x => x.ContainsKey("date") && ((DateTimeOffset)x["date"]).Year.Equals(year)).ToArray();
            site.Years.Add(year, yearFiles);
        }
    }

    private void EnrichSiteWithSeries(SiteMetaData site, List<PageMetaData> pages)
    {
        var series = pages
            .Where(x => x.Series != null)
            .Select(x => x.Series)
            .Distinct();

        foreach (var serie in series)
        {
            var seriesFiles = pages.Where(x => x.Series != null && serie.Equals(x.Series)).OrderBy(x => x.Url).ToArray();
            site.Series.Add(serie, seriesFiles);
        }
    }

    private void EnrichSiteWithTypes(SiteMetaData site, List<PageMetaData> pages)
    {
        var blockedTypes = new ContentType[] { ContentType.Unknown, ContentType.Page };
        var types = pages
            .Where(x => !blockedTypes.Contains(x.Type))
            .Select(x => x.Type)
            .Distinct();
        foreach (var type in types)
        {
            var typeFiles = pages.Where(x => /*x.Type != null && */ x.Type.Equals(type)).ToArray();
            site.Types.Add(type.ToString(), typeFiles);
        }
    }

    private void EnrichSiteWithCollections(SiteMetaData site, Guid siteGuid, List<PageMetaData> files)
    {
        var collections = files
            .Where(x => x.Collection != null)
            .Select(x => x.Collection)
            .Distinct()
            .ToList();

        for (var i = collections.Count - 1; i > 0; i--)
        {
            var collection = collections[i];
            if (_siteInfo.Collections.Contains(collection))
            {
                var collectionSettings = _siteInfo.Collections[collection];
                if (!string.IsNullOrEmpty(collectionSettings.TreatAs))
                {
                    if (_siteInfo.Collections.Contains(collectionSettings.TreatAs))
                    {
                        // todo log
                        var collectionFiles = files
                            .Where(x => x.Collection != null && x.Collection.Equals(collection));
                        foreach (var file in collectionFiles)
                        {
                            file.Collection = collectionSettings.TreatAs;
                        }
                        collections.RemoveAt(i);
                    }
                }
            }
        }

        foreach (var collection in collections)
        {
            site.Collections.Add(collection,
                files
                .Where(x => x.Collection != null
                    && x.Collection.Equals(collection))
                .ToArray()
            );
        }
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
        var pages = processed.ToArray().ToPages(siteGuid);

        var info = new AssemblyUtil().RetrieveAssemblyInfo(Assembly.GetExecutingAssembly());
        _logger.LogInformation(info.Metadata["RepositoryUrl"]);
        var buildInfo = new BuildData(info);
        var siteInfo = new SiteMetaData(pages)
        {
            Id = siteGuid.ToString(),
            Title = _siteInfo.Title,
            Description = _siteInfo.Description,
            Language = _siteInfo.Lang,
            Url = _siteInfo.Url,
            Author = null,
            Data = new Dictionary<string, object>(),
            Tags = new SortedDictionary<string, PageMetaData[]>(),
            Collections = new SortedDictionary<string, PageMetaData[]>(),
            Types = new SortedDictionary<string, PageMetaData[]>(),
            Series = new SortedDictionary<string, PageMetaData[]>(),
            Years = new SortedDictionary<int, PageMetaData[]>()
        };

        EnrichSiteWithData(siteInfo, request.Configuration.DataDirectory);
        EnrichSiteWithCollections(siteInfo, siteGuid, pages.ToList());
        EnrichSiteWithTags(siteInfo, pages.ToList());
        EnrichSiteWithYears(siteInfo, pages.ToList());
        EnrichSiteWithTypes(siteInfo, pages.ToList());
        EnrichSiteWithSeries(siteInfo, pages.ToList());

        var requests = processed.Select(file =>
        {
            var page = file.ToPage();
            page.Id = siteGuid.CreatePageGuid(file.MetaData.Uri).ToString();
            return new MetadataRenderRequest
            {
                Metadata = new RenderData()
                {
                    Build = buildInfo,
                    Site = siteInfo,
                    Page = page
                },
                Template = file.MetaData.Layout
            };
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

        // TODO can we do this better?
        var directoryContents =
                        _fileSystem.GetDirectoryContents("");
        var rootFile = directoryContents.FirstOrDefault();
        if (rootFile != null)
        {
            var root = rootFile.PhysicalPath.Replace(rootFile.Name, "");
            // var root2 = Directory.GetCurrentDirectory();

            var assets = _fileSystem.GetFiles(request.Configuration.AssetDirectory, true)
                .Select(x => x.PhysicalPath.Replace(root, string.Empty));
            artifacts.AddRange(assets.Select(asset =>
            {
                return new Artifact
                {
                    Path = $"{asset}",
                    Contents = _fileSystem.GetFileBytes(asset)
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
}