// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Manager.Site.Service;
using Xunit;

namespace Test.Unit.Architecture
{
    public abstract class ServiceContractValidationTests
    {
        protected abstract Type GetImplementationType();

        protected virtual void ValidateServiceContractForType(Type serviceContractType)
        {
            // ServiceContractAttribute? serviceContractAttribute = serviceContractType.GetCustomAttribute<ServiceContractAttribute>();
        }
        
        [Fact]
        public void ValidServiceContracts()
        {
            Type implementationType = GetImplementationType();
            Type[] interfaceTypes = implementationType.GetInterfaces();
            // IEnumerable<Type> interfacesToExclude = _markerInterfaces.AsEnumerable();
            // interfaceTypes = interfaceTypes.Except(interfacesToExclude).ToArray();

            // Type[] excludedTypes = GetExcludedContractValidations();
            // interfaceTypes = interfaceTypes.Except(excludedTypes).ToArray();
            // Assert.IsTrue(interfaceTypes.Length > 0, $"The type '{implementationType.FullName}' does not have any interfaces to verify!");

            foreach (Type contractType in interfaceTypes)
            {
                ValidateServiceContractForType(contractType);
            }
        }
    }

    public class SiteManagerServiceContractValidationTests : ServiceContractValidationTests
    {
        protected override Type GetImplementationType()
        {
            Type implementationType = typeof(SiteManager);
            return implementationType;
        }
    }
    
    public class ArtifactAccessServiceContractValidationTests : ServiceContractValidationTests
    {
        protected override Type GetImplementationType()
        {
            Type implementationType = typeof(SiteManager);
            return implementationType;
        }
    }
}