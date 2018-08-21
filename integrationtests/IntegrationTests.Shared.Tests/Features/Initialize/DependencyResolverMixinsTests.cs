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

            Registrations registrations = new Registrations();
            registrations.Register((factory, serviceType) =>
            {
                if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out List<Type> implementationTypes) == false)
                {
                    implementationTypes = new List<Type>();
                    serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                }

                implementationTypes.Add(factory().GetType());
            });

            Type platformRegistrationsType = Type.GetType("ReactiveUI.Wpf.Registrations, ReactiveUI.Wpf");
            if (platformRegistrationsType == null)
            {
                platformRegistrationsType = Type.GetType("ReactiveUI.XamForms.Registrations, ReactiveUI.XamForms");
            }

            if (platformRegistrationsType == null)
            {
                platformRegistrationsType = Type.GetType("ReactiveUI.Winforms.Registrations, ReactiveUI.Winforms");
            }

            if (platformRegistrationsType != null)
            {
                var platformRegistrations = Activator.CreateInstance(platformRegistrationsType);
                System.Reflection.MethodInfo register = platformRegistrationsType.GetMethod("Register");
                Action<Func<object>, Type> registerParameter = new Action<Func<object>, Type>((factory, serviceType) =>
                {
                    if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out List<Type> implementationTypes) == false)
                    {
                        implementationTypes = new List<Type>();
                        serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                    }

                    implementationTypes.Add(factory().GetType());
                });

                register.Invoke(platformRegistrations, new object[] { registerParameter });
            }

            return serviceTypeToImplementationTypes;
        }

        /// <summary>
        /// Checks to make sure that all the types are correctly registered.
        /// </summary>
        [Fact]
        public void Registrations_Should_Resolve_Correctly()
        {
            foreach (KeyValuePair<Type, List<Type>> shouldRegistered in GetServicesThatShouldRegistered())
            {
                IEnumerable<object> resolvedServices = Locator.Current.GetServices(shouldRegistered.Key);
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
