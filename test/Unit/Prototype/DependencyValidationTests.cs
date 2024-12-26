// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Utilities.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Test.Unit.Utilities;
using Xunit;

#nullable enable

namespace Test.Unit.Prototype
{
    public class ServiceDependencyValidatorOptions : ServiceProviderOptions
    {
        public bool ValidateCanConstruct { get; set; }

        public string[] AllowedNamespaces { get; set; }

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
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

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
        public void ValidateServices()
        {
            IServiceCollection services = CreateDefaultServices();

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

        protected virtual IServiceCollection CreateDefaultServices()
        {
            IServiceCollection services = new ServiceCollection();

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            ApplyConfiguration(configurationBuilder);
            IConfiguration configuration = configurationBuilder.Build();

            // TODO: this does not belong here
            services.AddFileSystem();

            RegisterServiceDependencies(services, configuration);

            return services;
        }

        protected virtual void ApplyConfiguration(IConfigurationBuilder configurationBuilder)
        {
            // Empty on purpose
        }

        protected abstract void RegisterServiceDependencies(IServiceCollection services, IConfiguration configuration);
    }

    public class ArtifactAccessDependencyValidationTests : DependencyValidationTests
    {
        protected override void RegisterServiceDependencies(IServiceCollection services, IConfiguration configuration)
        {
            Kaylumah.Ssg.Access.Artifact.Hosting.ServiceCollectionExtensions.AddArtifactAccess(services, configuration);
        }
    }

    public class SiteManagerDependencyValidationTests : DependencyValidationTests
    {
        protected override void RegisterServiceDependencies(IServiceCollection services, IConfiguration configuration)
        {
            Kaylumah.Ssg.Manager.Site.Hosting.ServiceCollectionExtensions.AddSiteManager(services, configuration);
            // TODO better solution for this?
            // Bug?
            // services.AddSingleton<IArtifactAccess>();
            ArtifactAccessMock artifactAccessMock = new ArtifactAccessMock();
            IArtifactAccess artifactAccess = artifactAccessMock.Object;
            services.AddSingleton(artifactAccess);
        }

        protected override void ApplyConfiguration(IConfigurationBuilder configurationBuilder)
        {
            Dictionary<string, string?> data = new Dictionary<string, string?>();
            // TODO: can we do better than this for config?
            data.Add("Site:Lang", "en");
            data.Add("Metadata:Defaults:Title", "Title");
            configurationBuilder.AddInMemoryCollection(data);
        }
    }

    /*
     
     Following section is for future blogpost
     
     * [TestClass]
       public class ServiceDependencyValidatorTests
       {
           static ServiceDependencyValidatorOptions CreateDefaultOptions()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = new ServiceDependencyValidatorOptions(Array.Empty<string>());

               serviceProviderOptions.ValidateScopes = true;
               serviceProviderOptions.ValidateOnBuild = true;
               serviceProviderOptions.ValidateCanConstruct = true;

               return serviceProviderOptions;
           }

           static ServiceDependencyValidator Create(ServiceDependencyValidatorOptions options)
           {
               using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
               ILogger<ServiceDependencyValidator> logger = loggerFactory.CreateLogger<ServiceDependencyValidator>();
               ServiceDependencyValidator validator = new ServiceDependencyValidator(options, logger);
               return validator;
           }

           [TestMethod]
           public void Test_Fail_On_NullServiceCollection()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = CreateDefaultOptions();
               ServiceDependencyValidator validator = Create(serviceProviderOptions);

               // force it to be null
               IServiceCollection services = null!;
               Assert.ThrowsException<ArgumentNullException>(() => validator.Validate(services));
           }

           [TestMethod]
           public void Test_Fail_On_EmptyServiceCollection()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = CreateDefaultOptions();
               ServiceDependencyValidator validator = Create(serviceProviderOptions);

               IServiceCollection services = new ServiceCollection();
               Assert.ThrowsException<ArgumentOutOfRangeException>(() => validator.Validate(services));
           }

           [TestMethod]
           public void Test_Fail_On_NoValidation()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = new ServiceDependencyValidatorOptions(Array.Empty<string>());
               ServiceDependencyValidator validator = Create(serviceProviderOptions);

               IServiceCollection services = new ServiceCollection();
               services.AddSingleton<IMyService, MyService>();
               Assert.ThrowsException<InvalidOperationException>(() => validator.Validate(services));
           }

           [TestMethod]
           public void Test_Fail_On_Missing_Dependency()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = CreateDefaultOptions();
               ServiceDependencyValidator validator = Create(serviceProviderOptions);

               IServiceCollection services = new ServiceCollection();
               services.AddSingleton<IMyService, MyService>();
               Assert.ThrowsException<AggregateException>(() => validator.Validate(services));
           }

           [TestMethod]
           public void Test_ConstructorWithoutValidation_Succeeds()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = CreateDefaultOptions();
               serviceProviderOptions.ValidateCanConstruct = false;
               ServiceDependencyValidator validator = Create(serviceProviderOptions);

               IServiceCollection services = new ServiceCollection();
               services.AddSingleton<IMyService, MyService>();
               services.AddSingleton<IDependency, Dependency>();
               validator.Validate(services);
           }

           [TestMethod]
           public void Test_Constructor_Fails()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = CreateDefaultOptions();
               ServiceDependencyValidator validator = Create(serviceProviderOptions);

               IServiceCollection services = new ServiceCollection();
               services.AddSingleton<IMyService, MyService>();
               services.AddSingleton<IDependency, Dependency>();
               Assert.ThrowsException<AggregateException>(() => validator.Validate(services));
           }

           [TestMethod]
           public void Test_Singleton()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = CreateDefaultOptions();
               ServiceDependencyValidator validator = Create(serviceProviderOptions);

               IServiceCollection services = new ServiceCollection();
               services.AddSingleton<IDependency, Dependency>();
               validator.Validate(services);
           }

           [TestMethod]
           public void Test_Scoped()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = CreateDefaultOptions();
               ServiceDependencyValidator validator = Create(serviceProviderOptions);

               IServiceCollection services = new ServiceCollection();
               services.AddScoped<IDependency, Dependency>();
               validator.Validate(services);
           }

           [TestMethod]
           public void Test_Transient()
           {
               ServiceDependencyValidatorOptions serviceProviderOptions = CreateDefaultOptions();
               ServiceDependencyValidator validator = Create(serviceProviderOptions);

               IServiceCollection services = new ServiceCollection();
               services.AddTransient<IDependency, Dependency>();
               validator.Validate(services);
           }

           // TODO "Strategy" / "IEnumerable"
           // TODO Factory
           // TODO Scoped

           // TODO validate if it is a root provider
       }
       
       public interface IMyService { }
       public class MyService : IMyService
       {
           public MyService(IDependency dependency)
           {
               // Simulate a logic bug inside the constructor
               throw new InvalidOperationException("A logic error occurred in the constructor.");
           }
       }

       public interface IDependency { }
       public class Dependency : IDependency { }

       public interface IScopedService { }
       public class ScopedService : IScopedService { }

       public interface ITransientService { }
       public class TransientService : ITransientService { }

       // Example of open generic repository
       public interface IRepository<T> { }
       public class Repository<T> : IRepository<T> { }
     */
}