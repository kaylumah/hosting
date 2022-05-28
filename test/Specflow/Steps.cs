// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Microsoft.Extensions.DependencyInjection;
using SolidToken.SpecFlow.DependencyInjection;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using TechTalk.SpecFlow;

namespace Test.Specflow
{
    [Binding]
    public class Steps
    {
        /*
        private readonly IServiceProvider _serviceProvider;

        public Steps(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        */

        private readonly MetadataParserOptions _metadataParserOptions;

        public Steps(MetadataParserOptions metadataParserOptions)
        {
            _metadataParserOptions = metadataParserOptions;
        }

        [Given("the following")]
        public void GivenSomething()
        {
        }
    }

    public static class DiContainer
    {
        [ScenarioDependencies]
        public static IServiceCollection Register()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(builder => { });
            services.AddScoped<IYamlParser, YamlParser>();
            services.AddScoped<IMetadataProvider, YamlFrontMatterMetadataProvider>();
            services.AddScoped<IFileMetadataParser, FileMetadataParser>();
            services.AddScoped<MetadataParserOptions>();
            return services;
        }
    }
}
