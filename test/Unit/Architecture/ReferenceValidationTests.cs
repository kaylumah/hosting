// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Access.Artifact.Service;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Xunit;

namespace Test.Unit.Architecture
{
    public abstract class ComponentDefinition
    {
        public Assembly? Interface
        { get; protected init; }

        public Assembly? Service
        { get; protected init; }

        public Assembly? Hosting
        { get; protected init; }
    }

    public class ComponentDefinition<TInterface, TService> : ComponentDefinition
    {
        public ComponentDefinition(Type hostingType)
        {
            Interface = typeof(TInterface).Assembly;
            Service = typeof(TService).Assembly;
            Hosting = hostingType.Assembly;
        }
    }
    
    public abstract class ReferenceValidationTests
    {
        static readonly List<ComponentDefinition> _Components;

        static ReferenceValidationTests()
        {
            Type artifactAccessHostingType = typeof(Kaylumah.Ssg.Access.Artifact.Hosting.ServiceCollectionExtensions);
            ComponentDefinition<IArtifactAccess, ArtifactAccess> artifactAccess = new (artifactAccessHostingType);

            Type siteManagerHostingType = typeof(Kaylumah.Ssg.Manager.Site.Hosting.ServiceCollectionExtensions);
            ComponentDefinition<ISiteManager, SiteManager> siteManager = new (siteManagerHostingType);

            _Components = new()
            {
                artifactAccess,
                siteManager
            };
            
            Assembly[] knownAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Assembly> discoveredServiceAssemblies = knownAssemblies
                .Where(a => a.GetName().Name?.EndsWith(".Hosting", StringComparison.InvariantCulture) == true)
                .Distinct()
                .ToList();

            IEnumerable<Assembly> assemblies = _Components.Select(component => component.Hosting!);
            List<Assembly> unregistered = discoveredServiceAssemblies.Except(assemblies).ToList();
            
            if (0 < unregistered.Count)
            {
                #pragma warning disable IDESIGN103
                string missingNames = string.Join(", ", unregistered.Select(a => a.GetName().Name));
                throw new InvalidOperationException($"Missing ComponentDefinition registrations for: {missingNames}");
                #pragma warning restore IDESIGN103
            }
        }
        
        protected abstract Type GetImplementationType();

        protected virtual Type[] GetAllowedDependencyTypes()
        {
            Type[] result = [];
            return result;
        }
        
        [Fact]
        public void TestValidateArchitectureConstraints()
        {
            Type type = GetImplementationType();
            Assembly assembly = type.Assembly;
            
            ComponentDefinition componentDefinition = _Components.Single(component => component.Service == assembly);

            componentDefinition.Hosting.Should().Reference(componentDefinition.Interface);
            componentDefinition.Hosting.Should().Reference(componentDefinition.Service);
            
            componentDefinition.Interface.Should().NotReference(componentDefinition.Hosting);
            componentDefinition.Interface.Should().NotReference(componentDefinition.Service);
            
            componentDefinition.Service.Should().NotReference(componentDefinition.Hosting);
            componentDefinition.Service.Should().Reference(componentDefinition.Interface);
            
            Type[] dependencyTypes = GetAllowedDependencyTypes();
            List<ComponentDefinition> allowedComponents = new();
            foreach (Type dependencyType in dependencyTypes)
            {
                Assembly dependencyAssembly = dependencyType.Assembly;
                ComponentDefinition dependencyDefinition = _Components.Single(component => component.Service == dependencyAssembly);
                allowedComponents.Add(dependencyDefinition);
            }

            foreach (ComponentDefinition allowedComponent in allowedComponents)
            {
                // TODO this is wrong
                // componentDefinition.Hosting.Should().NotReference(allowedComponent.Hosting);
                // componentDefinition.Hosting.Should().NotReference(allowedComponent.Interface);
                // componentDefinition.Hosting.Should().NotReference(allowedComponent.Service);
            
                componentDefinition.Interface.Should().NotReference(allowedComponent.Hosting);
                componentDefinition.Interface.Should().NotReference(allowedComponent.Interface);
                componentDefinition.Interface.Should().NotReference(allowedComponent.Service);
            
                componentDefinition.Service.Should().NotReference(allowedComponent.Hosting);
                componentDefinition.Service.Should().Reference(allowedComponent.Interface);
                componentDefinition.Service.Should().NotReference(allowedComponent.Service);
            }

            List<ComponentDefinition> forbiddenComponents = _Components
                .Except(allowedComponents)
                .ToList();
            forbiddenComponents.Remove(componentDefinition);
            
            foreach (ComponentDefinition forbidden in forbiddenComponents)
            {
                componentDefinition.Service.Should().NotReference(forbidden.Interface);
                componentDefinition.Service.Should().NotReference(forbidden.Service);
                componentDefinition.Service.Should().NotReference(forbidden.Hosting);
            }
        }
    }

    public class ArtifactAccessReferenceValidationTests : ReferenceValidationTests
    {
        protected override Type GetImplementationType()
        {
            Type artifactAccess = typeof(ArtifactAccess);
            return artifactAccess;
        }
    }
    
    public class SiteManagerReferenceValidationTests : ReferenceValidationTests
    {
        protected override Type GetImplementationType()
        {
            Type siteManager = typeof(SiteManager);
            return siteManager;
        }

        protected override Type[] GetAllowedDependencyTypes()
        {
            Type[] result = new Type[1];
            result[0] = typeof(ArtifactAccess);
            return result;
        }
    }
}