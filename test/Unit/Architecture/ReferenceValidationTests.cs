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
        List<ComponentDefinition> _Components;
        
        public ReferenceValidationTests()
        {
            Type artifactAccessHostingType = typeof(Kaylumah.Ssg.Access.Artifact.Hosting.ServiceCollectionExtensions);
            ComponentDefinition<IArtifactAccess, ArtifactAccess> artifactAccess = new (artifactAccessHostingType);

            Type siteManagerHostingType = typeof(Kaylumah.Ssg.Manager.Site.Hosting.ServiceCollectionExtensions);
            ComponentDefinition<ISiteManager, SiteManager> siteManager = new (siteManagerHostingType);
            
            _Components = new();
            _Components.Add(artifactAccess);
            _Components.Add(siteManager);
        }
        
        public abstract Type GetImplementationType();
        
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
        }
    }

    public class ArtifactAccessReferenceValidationTests : ReferenceValidationTests
    {
        public override Type GetImplementationType()
        {
            Type artifactAccess = typeof(ArtifactAccess);
            return artifactAccess;
        }
    }
    
    public class SiteManagerReferenceValidationTests : ReferenceValidationTests
    {
        public override Type GetImplementationType()
        {
            Type siteManager = typeof(SiteManager);
            return siteManager;
        }
    }
}