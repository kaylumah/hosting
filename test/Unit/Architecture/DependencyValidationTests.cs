// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Utilities.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Test.Unit.Utilities;
using VerifyTests;
using VerifyXunit;
using Xunit;

#nullable enable

namespace Test.Unit.Architecture
{
    public class ServiceDependencyValidatorOptions : ServiceProviderOptions
    {
        public bool ValidateCanConstruct
        { get; set; }

        public string[] AllowedNamespaces
        { get; set; }

        public ServiceDependencyValidatorOptions(string[] allowedNamespaces)
        {
            AllowedNamespaces = allowedNamespaces;
        }
    }

    public class ServiceDependencyValidator
    {
        readonly ServiceDependencyValidatorOptions _ServiceDependencyValidatorOptions;
        readonly ILogger<ServiceDependencyValidator> _Logger;

        public ServiceDependencyValidator(ServiceDependencyValidatorOptions serviceDependencyValidatorOptions,
            ILogger<ServiceDependencyValidator> logger)
        {
            _ServiceDependencyValidatorOptions = serviceDependencyValidatorOptions;
            _Logger = logger;
        }

        public void Validate(IServiceCollection services)
        {
            EnsureServiceCollectionIsValid(services);

            bool atLeastOneValidationEnabled = _ServiceDependencyValidatorOptions.ValidateScopes
                                               || _ServiceDependencyValidatorOptions.ValidateOnBuild
                                               || _ServiceDependencyValidatorOptions.ValidateCanConstruct;

            if (atLeastOneValidationEnabled == false)
            {
                throw new InvalidOperationException("Validation requires at least 1 validation option to be enabled");
            }

            // Build the IServiceProvider based on provided configuration
            IServiceProvider serviceProvider = services.BuildServiceProvider(_ServiceDependencyValidatorOptions);

            if (_ServiceDependencyValidatorOptions.ValidateCanConstruct == true)
            {
                IEnumerable<IGrouping<ServiceLifetime, ServiceDescriptor>> groupedServiceCollection =
                    services.GroupBy(serviceDescriptor => serviceDescriptor.Lifetime);
                Dictionary<ServiceLifetime, List<ServiceDescriptor>> servicesByLifetime =
                    groupedServiceCollection.ToDictionary(group => group.Key, group => group.ToList());

                // Validate Singleton Services
                ValidateServices(servicesByLifetime, ServiceLifetime.Singleton, serviceProvider);

                // Validate Scoped Services
                ValidateServices(servicesByLifetime, ServiceLifetime.Scoped, serviceProvider);

                // Validate Transient Services
                ValidateServices(servicesByLifetime, ServiceLifetime.Transient, serviceProvider);
            }
        }

        void ValidateServices(Dictionary<ServiceLifetime, List<ServiceDescriptor>> servicesByLifetime, ServiceLifetime lifetime,
            IServiceProvider rootProvider)
        {
            bool hasServicesForLifetime = servicesByLifetime.TryGetValue(lifetime, out List<ServiceDescriptor>? services);
            if (hasServicesForLifetime == false || services == null)
            {
                return;
            }

            if (lifetime == ServiceLifetime.Singleton)
            {
                ValidateServices(services, rootProvider, lifetime);
            }
            else
            {
                using IServiceScope scope = rootProvider.CreateScope();
                IServiceProvider scopedProvider = scope.ServiceProvider;
                ValidateServices(services, scopedProvider, lifetime);
            }
        }

        void ValidateServices(IEnumerable<ServiceDescriptor> services, IServiceProvider provider, ServiceLifetime lifetime)
        {
            EnsureServiceCollectionIsValid(services);
            bool servicesMatchLifetime = services.All(serviceDescriptor => serviceDescriptor.Lifetime == lifetime);

            if (servicesMatchLifetime == false)
            {
                throw new InvalidOperationException($"Not all provided services are of scope '{lifetime}'");
            }

            int numberOfServiceDescriptors = services.Count();
            _Logger.LogInformation("Validating '{NumberOfServiceDescriptors}' services for lifetime '{ServiceLifetime}'", numberOfServiceDescriptors,
                lifetime);

            string[] allowedNamespaces = _ServiceDependencyValidatorOptions.AllowedNamespaces;
            List<Exception>? exceptions = null;
            foreach (ServiceDescriptor serviceDescriptor in services)
            {
                try
                {
                    bool validationAllowed = ShouldValidateService(serviceDescriptor, allowedNamespaces);
                    if (validationAllowed == false)
                    {
                        _Logger.LogTrace("Validation was skipped for '{ServiceType}'", serviceDescriptor.ServiceType.FullName);
                        continue;
                    }

                    // Track resolution time
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    ValidateServiceDescriptor(serviceDescriptor, provider);
                    stopwatch.Stop();

                    // Log the elapsed time
                    _Logger.LogTrace("Resolved service '{ServiceType}' in {ElapsedMilliseconds} ms", serviceDescriptor.ServiceType.FullName,
                        stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    string descriptorAsString = serviceDescriptor.ToString();
                    string actualError = ex.Message;
                    string errorMessage = $"The service '{descriptorAsString}' failed to be constructed with error '{actualError}'";
                    InvalidOperationException error = new InvalidOperationException(errorMessage, ex);
                    exceptions ??= new List<Exception>();
                    exceptions.Add(error);
                }
            }

            if (exceptions != null)
            {
                throw new AggregateException("Some services are not able to be constructed", exceptions);
            }
        }

        void EnsureServiceCollectionIsValid(IEnumerable<ServiceDescriptor> services)
        {
            ArgumentNullException.ThrowIfNull(services);

            bool isServiceCollectionEmpty = services.Any() == false;
            if (isServiceCollectionEmpty)
            {
                throw new ArgumentOutOfRangeException(nameof(services), "Validation requires at least 1 service to be registered");
            }
        }

        bool ShouldValidateService(ServiceDescriptor serviceDescriptor, string[] allowedNamespaces)
        {
            bool namespaceValidationDisabled = allowedNamespaces.Length == 0;
            bool allowOnServiceType = IsTypeAllowed(serviceDescriptor.ServiceType, allowedNamespaces);
            bool allowOnImplementationType = IsTypeAllowed(serviceDescriptor.ImplementationType, allowedNamespaces);

            bool validationAllowed = namespaceValidationDisabled || allowOnServiceType || allowOnImplementationType;
            return validationAllowed;
        }

        void ValidateServiceDescriptor(ServiceDescriptor serviceDescriptor, IServiceProvider provider)
        {
            if (serviceDescriptor.ServiceType.IsGenericTypeDefinition)
            {
                _Logger.LogInformation("Validation was skipped for '{ServiceType}' due to being an IsGenericTypeDefinition",
                    serviceDescriptor.ServiceType.FullName);
                return;
            }
            else if (serviceDescriptor.ImplementationType != null)
            {
                // Resolve services registered with a concrete type
                provider.GetRequiredService(serviceDescriptor.ServiceType);
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                // Resolve services registered with a factory method
                object factoryResult = serviceDescriptor.ImplementationFactory(provider);
                if (factoryResult == null)
                {
                    throw new InvalidOperationException($"Factory for service type {serviceDescriptor.ServiceType} returned null.");
                }
            }
            else if (serviceDescriptor.ImplementationInstance != null)
            {
                // Validate services registered as instances
                if (!serviceDescriptor.ServiceType.IsInstanceOfType(serviceDescriptor.ImplementationInstance))
                {
                    throw new InvalidOperationException(
                        $"Instance of type {serviceDescriptor.ImplementationInstance.GetType()} cannot be assigned to service type {serviceDescriptor.ServiceType}.");
                }
            }
        }

        bool IsTypeAllowed(Type? type, string[] allowedNamespaces)
        {
            if (type?.Namespace == null)
            {
                return false;
            }

            // (re)consider
            // var regexList = allowedNamespaces.Select(ns => new Regex(ns, RegexOptions.Compiled)).ToArray();
            // bool allowed = allowedNamespaces.Any(allowedNamespace => Regex.IsMatch(type.Namespace, allowedNamespace));

            bool allowed = allowedNamespaces.Any(allowedNamespace => type.Namespace.StartsWith(allowedNamespace, StringComparison.Ordinal));
            return allowed;
        }
    }

    public abstract class DependencyValidationTests
    {
        [Fact]
        public virtual void ValidateServices()
        {
            IServiceCollection services = CreateDefaultServiceCollection();

            string[] namespaceTargets = Array.Empty<string>();
            ServiceDependencyValidatorOptions options = new ServiceDependencyValidatorOptions(namespaceTargets);
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
            options.ValidateCanConstruct = true;

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
            ILogger<ServiceDependencyValidator> logger = loggerFactory.CreateLogger<ServiceDependencyValidator>();
            ServiceDependencyValidator serviceDependencyValidator = new ServiceDependencyValidator(options, logger);
            serviceDependencyValidator.Validate(services);
        }

        [Fact]
        public virtual async Task VerifyDependencies()
        {
            IEnumerable<ServiceDescriptor> services = CreateDefaultServiceCollection();

            VerifySettings settings = new VerifySettings();

            /*
            static void ScrubAssemblyVersions(StringBuilder sb)
            {
                string input = sb.ToString();
                string cleaned = Regex.Replace(
                    input,
                    @"Version=\d+\.\d+\.\d+\.\d+",
                    "Version=*"
                );
                sb.Clear();
                sb.Append(cleaned);
            }
            
            settings.AddScrubber(ScrubAssemblyVersions);
            */

            settings.AddScrubber(sb =>
            {
                string input = sb.ToString();
                string cleaned = Regex.Replace(
                    input,
                    @"Mock<([^>:]+):\d+>",
                    "Mock<$1:#>"
                );
                sb.Clear();
                sb.Append(cleaned);
            });

            settings.ScrubMember<TimeProvider>(timeProvider => timeProvider.LocalTimeZone);
            settings.ScrubMember<TimeProvider>(timeProvider => timeProvider.TimestampFrequency);

            // Ignore Keyed fields on ServiceDescriptor as we don't use them.
            settings.IgnoreMember<ServiceDescriptor>(serviceDescriptor => serviceDescriptor.KeyedImplementationType);
            settings.IgnoreMember<ServiceDescriptor>(serviceDescriptor => serviceDescriptor.KeyedImplementationInstance);
            settings.IgnoreMember<ServiceDescriptor>(serviceDescriptor => serviceDescriptor.KeyedImplementationFactory);
            settings.IgnoreMember<ServiceDescriptor>(serviceDescriptor => serviceDescriptor.IsKeyedService);

            await Verifier.Verify(services, settings);
        }

        protected virtual IServiceCollection CreateDefaultServiceCollection()
        {
            IServiceCollection services = new ServiceCollection();

            ConfigurationManager configurationManager = CreateDefaultConfigurationManager();
            // This part is used for every test to register their own configuration
            ConfigureComponent(configurationManager);

            services.AddSingleton<IConfiguration>(configurationManager);
            services.AddSingleton<IConfigurationRoot>(configurationManager);
            services.AddSingleton<IConfigurationManager>(configurationManager);

            // TODO: this does not belong here
            services.AddFileSystem();

            // This part is used for every test to register their own dependencies
            ConfigureServices(services, configurationManager);

            return services;
        }

        protected virtual ConfigurationManager CreateDefaultConfigurationManager()
        {
            ConfigurationManager configurationManager = new ConfigurationManager();
            return configurationManager;
        }

        protected abstract void ConfigureComponent(IConfigurationManager configuration);

        protected abstract void ConfigureServices(IServiceCollection services, IConfigurationManager configuration);
    }

    public class ArtifactAccessDependencyValidationTests : DependencyValidationTests
    {
        protected override void ConfigureComponent(IConfigurationManager configuration)
        {
            // Empty on purpose
        }

        protected override void ConfigureServices(IServiceCollection services, IConfigurationManager configuration)
        {
            Kaylumah.Ssg.Access.Artifact.Hosting.ServiceCollectionExtensions.AddArtifactAccess(services, configuration);
        }

        [Fact]
        public override void ValidateServices()
        {
            base.ValidateServices();
        }
    }

    public class SiteManagerDependencyValidationTests : DependencyValidationTests
    {
        protected override void ConfigureComponent(IConfigurationManager configuration)
        {
            Dictionary<string, string?> data = new()
            {
                // TODO: can we do better than this for config?
                { "Site:Lang", "en" },
                { "Metadata:Defaults:Title", "Title" }
            };
            configuration.AddInMemoryCollection(data);
        }

        protected override void ConfigureServices(IServiceCollection services, IConfigurationManager configuration)
        {
            Kaylumah.Ssg.Manager.Site.Hosting.ServiceCollectionExtensions.AddSiteManager(services, configuration);
            // TODO better solution for this?
            // Bug?
            // services.AddSingleton<IArtifactAccess>();
            ArtifactAccessMock artifactAccessMock = new ArtifactAccessMock();
            IArtifactAccess artifactAccess = artifactAccessMock.Object;
            services.AddSingleton(artifactAccess);
        }

        [Fact]
        public override void ValidateServices()
        {
            base.ValidateServices();
        }
    }
}