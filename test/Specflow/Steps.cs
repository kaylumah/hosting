// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Microsoft.Extensions.DependencyInjection;
using SolidToken.SpecFlow.DependencyInjection;
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
            var services = new ServiceCollection();

            services.AddScoped<MetadataParserOptions>();

            return services;
        }
    }
}
