using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using Splat;
using Xunit;

namespace IntegrationTests.Shared.Tests.Features.Initialize
{
    /// <summary>
    /// Tests associated with the dependency resolvers.
    /// </summary>
    public class DependencyResolverMixinsTests
    {
        /// <summary>
        /// Gets a dictionary of services that should be registered.
        /// </summary>
        /// <returns>The dictionary of types that should be registered.</returns>
        public Dictionary<Type, List<Type>> GetServicesThatShouldRegistered()
        {
            Dictionary<Type, List<Type>> serviceTypeToImplementationTypes = new Dictionary<Type, List<Type>>();

            var registrations = new Registrations();
            registrations.Register((factory, serviceType) =>
            {
                if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out List<Type> implementationTypes) == false)
                {
                    implementationTypes = new List<Type>();
                    serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                }

                implementationTypes.Add(factory().GetType());
            });

            var platformRegistrations = new PlatformRegistrations();
            platformRegistrations.Register((factory, serviceType) =>
            {
                if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out List<Type> implementationTypes) == false)
                {
                    implementationTypes = new List<Type>();
                    serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                }

                implementationTypes.Add(factory().GetType());
            });

            return serviceTypeToImplementationTypes;
        }

        /// <summary>
        /// Checks to make sure that all the types are correctly registered.
        /// </summary>
        [Fact]
        public void Registrations_Should_Resolve_Correctly()
        {
            var resolver = new ModernDependencyResolver();
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            foreach (KeyValuePair<Type, List<Type>> shouldRegistered in GetServicesThatShouldRegistered())
            {
                IEnumerable<object> resolvedServices = resolver.GetServices(shouldRegistered.Key);
                Assert.Equal(shouldRegistered.Value.Count, resolvedServices.Count());
                foreach (Type implementationType in shouldRegistered.Value)
                {
                    var isRegistered = resolvedServices.Any(rs => rs.GetType() == implementationType);
                    Assert.Equal(true, isRegistered);
                }
            }
        }
    }
}
