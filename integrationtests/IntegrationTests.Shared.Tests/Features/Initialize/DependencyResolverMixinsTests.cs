using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ReactiveUI;
using Splat;
using Xunit;

namespace IntegrationTests.Shared.Tests.Features.Initialize
{
    public class DependencyResolverMixinsTests
    {
        public Dictionary<Type, List<Type>> GetServicesThatShouldRegistered()
        {
            var serviceTypeToImplementationTypes = new Dictionary<Type, List<Type>>();

            var registrations = new Registrations();
            registrations.Register((factory, serviceType) => {
                if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out var implementationTypes) == false) {
                    implementationTypes = new List<Type>();
                    serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                }
                implementationTypes.Add(factory().GetType());
            });

            var platformRegistrationsType = Type.GetType("ReactiveUI.Wpf.Registrations, ReactiveUI.Wpf");
            if (platformRegistrationsType == null)
                platformRegistrationsType = Type.GetType("ReactiveUI.XamForms.Registrations, ReactiveUI.XamForms");
            if (platformRegistrationsType == null)
                platformRegistrationsType = Type.GetType("ReactiveUI.Winforms.Registrations, ReactiveUI.Winforms");

            if (platformRegistrationsType != null) {
                var platformRegistrations = Activator.CreateInstance(platformRegistrationsType);
                var register = platformRegistrationsType.GetMethod("Register");
                var registerParameter = new Action<Func<object>, Type>((factory, serviceType) => {
                    if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out var implementationTypes) == false) {
                        implementationTypes = new List<Type>();
                        serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                    }
                    implementationTypes.Add(factory().GetType());
                });
                register.Invoke(platformRegistrations, new object[] { registerParameter });
            }

            return serviceTypeToImplementationTypes;
        }

        [Fact]
        public void Registrations_Should_Resolve_Correctly()
        {
            foreach(var shouldRegistered in GetServicesThatShouldRegistered()) {
                var resolvedServices = Locator.Current.GetServices(shouldRegistered.Key);
                Assert.Equal(shouldRegistered.Value.Count, resolvedServices.Count());
                foreach(var implementationType in shouldRegistered.Value) {
                    var isRegistered = resolvedServices.Any(rs => rs.GetType() == implementationType);
                    Assert.Equal(true, isRegistered);
                }
            }
        }
    }
}
