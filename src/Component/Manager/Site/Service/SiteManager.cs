// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ssg.Extensions.Data.Yaml;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Engine.Transformation.Interface.Rendering;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class SiteManager : ISiteManager
    {
        private readonly IArtifactAccess _artifactAccess;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IFileProcessor _fileProcessor;
        private readonly IYamlParser _yamlParser;
        private readonly SiteInfo _siteInfo;
        private readonly IMetadataRenderer _metadataRenderer;

        public SiteManager(
            IFileProcessor fileProcessor,
            IArtifactAccess artifactAccess,
            IFileSystem fileSystem,
            IYamlParser yamlParser,
            ILogger<SiteManager> logger,
            IOptions<SiteInfo> options,
            IMetadataRenderer metadataRenderer
            )
        {
            _fileProcessor = fileProcessor;
            _artifactAccess = artifactAccess;
            _fileSystem = fileSystem;
            _yamlParser = yamlParser;
            _logger = logger;
            _siteInfo = options.Value;
            _metadataRenderer = metadataRenderer;
        }

        private void EnrichSiteWithData(SiteData site, string dataDirectory)
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

        private void EnrichSiteWithTags(SiteData site, List<PageData> pages)
        {
            var tags = pages
                .Where(x => x.Tags != null)
                .SelectMany(x => x.Tags)
                .Distinct();
            foreach (var tag in tags)
            {
                var tagFiles = pages.Where(x => x.Tags != null && x.Tags.Contains(tag)).ToArray();
                site.Tags.Add(tag, tagFiles);
            }
        }

        private void EnrichSiteWithCollections(SiteData site, Guid siteGuid, List<PageData> files)
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
            var siteInfo = new SiteData(_siteInfo, pages)
            {
                Id = siteGuid.ToString(),
                Data = new Dictionary<string, object>(),
                Tags = new Dictionary<string, PageData[]>(),
                Collections = new Dictionary<string, PageData[]>()
            };

            EnrichSiteWithData(siteInfo, request.Configuration.DataDirectory);
            EnrichSiteWithCollections(siteInfo, siteGuid, pages.ToList());
            EnrichSiteWithTags(siteInfo, pages.ToList());

            var requests = processed.Select(file => 
            {
                var page = file.ToPage();
                page.Id = siteGuid.CreatePageGuid(file.MetaData.Uri).ToString();
                return new MetadataRenderRequest {
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
            var renderResults = await _metadataRenderer.Render(requests);

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
                    OutputLocation = new FileSystemOutputLocation() {
                        Clean = false,
                        Path = request.Configuration.Destination
                    }
                });
            }
        }
    }
}
